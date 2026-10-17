# ADR-0005: Por qué uso un outbox transaccional

**Estado:** aceptado
**Ámbito:** consistencia de datos distribuidos

## Contexto

Cuando el Núcleo recibe una solicitud desde Ingesta, tiene que hacer dos escrituras: guardar la solicitud en SQL y publicar el comando `ReplicarPedidoEnD365` en la cola `replicar-pedido-d365`. Ese comando lo consume la Function de replicación. No existe una transacción que abarque SQL y Service Bus.

Si el proceso del servicio Worker muere entre guardar y publicar, la solicitud queda persistida y el comando nunca sale. Esto pasa más seguido de lo que parece: un despliegue de una revisión nueva, KEDA bajando réplicas o una falta de memoria terminan el proceso.

La garantía at least once de Service Bus no cubre este caso. Esa garantía empieza cuando Service Bus acepta el mensaje, y un mensaje que nunca se envió no existe para Service Bus. Los reintentos de EF Core tampoco lo resuelven: `EnableRetryOnFailure` reintenta fallas de conexión mientras el proceso está vivo, pero no ayuda si el proceso muere.

## Opciones

### Opción A: completar al final y republicar de forma idempotente

El orden sería guardar, publicar y recién ahí completar el mensaje de entrada. Si el proceso muere antes de completar, el lock vence y Service Bus reentrega el mensaje. Si al reintentar la solicitud ya existe, se publica igual. El comando lleva un `MessageId` determinístico y la cola tiene detección de duplicados.

A favor:

- No agrega tablas ni procesos.

En contra:

- La consistencia depende de la reentrega del mensaje de entrada.
- Si publicar falla de forma persistente, el mensaje de entrada termina en la DLQ después de tres intentos, con la solicitud guardada y sin comando.

### Opción B: outbox transaccional

En la misma transacción guardo la solicitud y una fila en la tabla `Outbox` con el comando serializado. Un publicador lee las filas pendientes, las envía a su cola y las marca como procesadas.

A favor:

- Guardar la solicitud y dejar el comando pendiente es atómico. Si la solicitud quedó guardada, el comando también.
- Sirve para los demás mensajes que el Núcleo tenga que publicar.

En contra:

- Agrega una tabla y un publicador.
- El comando sale solo si el publicador corre. KEDA baja el Worker a cero réplicas cuando la cola de entrada está vacía, y el publicador vive en ese Worker.

## Decisión

Uso un outbox transaccional (opción B), con estas reglas:

- La solicitud y su fila del outbox se guardan en un solo `SaveChanges`.
- La fila guarda el comando ya armado y su destino. El publicador no tiene lógica de negocio: lee, envía y marca.
- El comando lo arma `SolicitarReplicacionEnD365Handler`, que maneja el evento de dominio `SolicitudDeIngresoRegistrada`. El caso de uso despacha los eventos antes de guardar. Los manejadores solo escriben en el mismo `DbContext` y nunca llaman a un servicio externo antes del commit.
- Una fila está pendiente mientras `ProcessedAt` es nulo. No borro las filas al enviarlas, para conservar el registro de qué se envió y cuándo.
- El comando sale con `MessageId` igual al Id de la solicitud.
- `MessageId` no es la clave primaria de la tabla, porque es único por destino y no global. Un segundo manejador del mismo evento escribiría otra fila con el mismo `MessageId` y otro destino. Si `MessageId` fuera la clave primaria, esa fila chocaría y haría fallar el registro de la solicitud. Cada fila tiene su propio `Id`.

### Dónde vive el publicador

| Opción | Resultado |
|---|---|
| | Dentro del Worker: un `BackgroundService` que barre al arrancar y cada 5 segundos mientras la réplica está viva | Elegida |
| Una segunda regla de KEDA con el escalador de SQL Server | Descartada. Consultaría la base cada 30 segundos todo el día, la base nunca entraría en pausa automática y se consumiría la oferta gratuita de Azure SQL |
| Un proceso programado aparte (Container Apps Job o Function con timer) | Descartada. Sería otro desplegable leyendo tablas del Núcleo |
| `minReplicas = 1` | Descartada. Pagaría la réplica todo el mes y perdería el escalado a cero |

Elegí el Worker porque una fila solo queda pendiente si la publicación falla después del commit, y eso es poco frecuente. Cuando la cola se vacía, la réplica sigue viva cinco minutos antes de bajar a cero, y ese tiempo alcanza para reintentar.

## Consecuencias

- Un delivery duplicado sigue siendo un no-op. Si el insert choca con el índice único, la transacción se revierte completa y la fila del outbox tampoco queda.
- El comando puede publicarse más de una vez. Lo cubren dos capas:
  - En el transporte, la cola `replicar-pedido-d365` tiene detección de duplicados por `MessageId`. Solo filtra dentro de su ventana de 10 minutos, y se puede activar únicamente al crear la cola.
  - En el consumidor, la Function de replicación tiene que tolerar un comando repetido. Lo reconoce por la clave de negocio (laboratorio y número de delivery), que viaja en el cuerpo del mensaje.
- Riesgo residual: si la publicación falla y la réplica baja a cero antes de reintentar, la fila espera hasta que llegue el próximo mensaje y el barrido de arranque la publique.
- Camino de mejora: si el negocio fija un tiempo máximo entre el registro y D365, activo la regla de KEDA sobre SQL.
- Deudas que acepto:
  - El repositorio actúa como unidad de trabajo: ante un duplicado limpia todo el contexto, incluida la fila del outbox. El segundo camino de guardado apareció en el publicador, que es infraestructura y guarda por su cuenta. Lo reviso cuando un caso de uso de Application necesite guardar, con el consumidor del resultado de la replicación.
  - El payload se guarda como `object` y pierde el tipado. Es el costo de un outbox genérico.
  - El nombre de la cola queda escrito en Application.
  - Nadie limpia las filas ya enviadas. Si el volumen lo pide, lo hará el barrido del Worker.
  - Cada barrido carga todas las filas pendientes, sin límite de lote. Agrego un `Take` cuando el volumen lo pida.
  - Una fila que falla siempre se reintenta cada 5 segundos y deja un error en el log cada vez. Agrego un contador de intentos cuando haga falta.
  - Dos réplicas pueden enviar la misma fila al mismo tiempo. La detección de duplicados de la cola lo filtra dentro de su ventana; la mejora es leer con bloqueos de SQL (`UPDLOCK`, `READPAST`).

## Cómo se publica

El publicador es un `BackgroundService` del Worker (`OutboxPublisherWorker`), separado del que procesa los mensajes de entrada. Barre al arrancar y después cada 5 segundos.

- El callback del mensaje de entrada no publica nada. Una vez hecho el commit, completa el mensaje de entrada aunque el comando todavía no se haya enviado. Si el destino del mensaje de entrada dependiera de la publicación, volvería al problema de la opción A.
- Un solo camino publica filas. Publicar también dentro del callback no recupera las filas que quedan huérfanas cuando el proceso muere después del commit; eso solo lo hace el barrido. Tener los dos caminos duplicaría la misma lógica.
- No agregué una señal para despertar al barrido después de cada commit. Ganaría como máximo 5 segundos en un flujo que ya es asíncrono. Si el negocio fija un tiempo máximo exigente, la agrego sin tocar el barrido.
- Por cada fila, primero envío y después marco `ProcessedAt`. Si el proceso muere entre los dos pasos, la fila se reenvía, y eso lo toleran las dos capas de duplicados. El orden inverso podría marcar como enviado un comando que nunca salió.
- Guardo una fila a la vez, para que una caída en la mitad del barrido reenvíe como mucho una.
- El segundo `SaveChanges` lo hace `OutboxPublisher`. Es infraestructura y no necesita un puerto de Application.
- Si el envío de una fila falla, lo registro en el log, la fila queda pendiente y sigo con las demás. Una cola rota no frena al resto.
- Si llega una cancelación, por ejemplo el `SIGTERM` que manda Container Apps cuando KEDA baja la réplica, el publicador se detiene en vez de tratarla como un envío fallido. El filtro mira el token y no el tipo de excepción, porque un timeout de red también lanza `OperationCanceledException` y en ese caso corresponde seguir.
- Cada barrido usa su propio scope y su propio `DbContext`.
