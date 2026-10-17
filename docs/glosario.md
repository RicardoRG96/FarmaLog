# Glosario de lenguaje ubicuo

Este glosario reúne los términos del negocio que usa el código de FarmaLog. Los conceptos del dominio se escriben en español, tal como los dice el negocio; el andamiaje técnico (repositorios, handlers, mappers) va en inglés. Si una palabra aparece aquí, el código la usa con este significado.

> **Aviso de vocabulario.** En el portal real de la empresa, "Solicitud" es el lote de carga (un archivo con varios pedidos) y "Orden" es el pedido individual. En este modelo, `SolicitudDeIngresoPedido` es el pedido individual. Lo detecté con el modelo ya avanzado y decidí no renombrar. El lote como agregado está identificado y quedó fuera de alcance.

## Negocio

| Término | En el código | Significado |
|---|---|---|
| Operador logístico 3PL | — | Almacena, prepara y despacha por cuenta del laboratorio. El laboratorio factura con su propio RUT. Es el alcance de este proyecto. |
| Operador 4PL | — | Además lleva la contabilidad y la facturación bajo el RUT del operador. Fuera de alcance. |
| Laboratorio | `CodigoLaboratorio` | Cliente del operador. Se identifica con un código numérico de al menos dos dígitos (ej. `23`), que es el prefijo de sus cuentas, direcciones y tipos de orden. |
| Solicitud de Ingreso de Pedido | `SolicitudDeIngresoPedido` | Agregado raíz. La petición de traer un pedido del laboratorio, validarlo y replicarlo en D365. No es el pedido: el pedido vive en D365. |
| Pedido | — | Entidad que vive en Dynamics 365, fuera de la frontera del sistema. Ningún evento de este dominio afirma hechos sobre él. |
| Línea de solicitud | `LineaSolicitud` | Un producto pedido: SKU, cantidad, estado de inventario y lote. Vive dentro de la solicitud. Una solicitud tiene entre 1 y 15 líneas. |
| Estado de la solicitud | `EstadoSolicitud` | `Recibida` → `Aceptada` o `Rechazada`. Los dos últimos son terminales. |
| Recibida | `EstadoSolicitud.Recibida` | Estado de nacimiento. Se mantiene durante todos los reintentos de transporte. |
| Aceptada | `EstadoSolicitud.Aceptada` | D365 aceptó la petición. No significa "el pedido fue creado". |
| Rechazada | `EstadoSolicitud.Rechazada` | D365 rechazó la petición por una regla de negocio. No se reintenta. |
| Motivos de rechazo | `MotivosDeRechazo` | Textos con los que D365 explica un rechazo. Se guardan en la solicitud porque hoy nadie más los conserva. |
| Número de Delivery | `NumeroDelivery` | Número del pedido en el ERP del laboratorio (columna "Número Pedido del Laboratorio" de la planilla). Alfanumérico y puede traer espacios. Junto con el código de laboratorio identifica un pedido de forma única. |
| Cuenta de cliente | `CuentaCliente` | Cliente final del laboratorio en D365. Formato `prefijo-RUT`, con el RUT rellenado con ceros hasta 10 caracteres (ej. `23-0778903671`). |
| Código de despacho | `DireccionDespacho` | Dirección de entrega del cliente final. Formato `prefijo-RUT` + `D` + correlativo (ej. `23-778903671D1`). La planilla dice "Código de Despacho"; el modelo lo llama dirección. |
| Tipo de orden de venta | `TipoOrdenVenta` | Prefijo del laboratorio + letra (`F` o `G`) + correlativo (ej. `23F1`; `F1` es factura). Se elige en la pantalla y vale para todo el archivo. |
| RUT | `RutValidation` | Rol Único Tributario: cuerpo + dígito verificador (0–9 o `K`). El sistema valida su forma; la corrección del dígito verificador la valida D365. |
| Fecha de entrega solicitada | `FechaEntregaSolicitada` | Opcional en la planilla (`dd/mm/aaaa`). Si viene, no puede ser anterior a hoy; si no viene, es hoy. |
| Hoy | `IReloj` | El día actual en la zona horaria `America/Santiago`. Es una decisión de negocio, no una propiedad del servidor. |
| Pedido Cenabast | `EsCenabast` | Pedido para la Central de Abastecimiento del Sistema Nacional de Servicios de Salud. Si es Cenabast, debe traer documento de venta; si no lo es, no puede traerlo. |
| Documento de venta Cenabast | `DocumentoVentaCenabast` | Código correlativo del documento Cenabast. |
| Observación | `Observacion` | Texto libre opcional, de máximo 300 caracteres. |
| Urgencia | `Urgencia` | Marca sí/no. Si no viene, es no. |
| Orden de compra | `OrdenCompra` | Número opcional de la orden de compra del cliente. Viaja sin validación. |
| Estado de inventario | `EstadoInventario` | La bodega desde donde sale el producto (ej. `DISPONIBLE`). Es un conjunto abierto: el sistema solo exige que venga. |
| Lote | `Lote` | Opcional. Viaja sin validación. |
| Almacén FLC1 | — | Único almacén del alcance. Es configuración de la replicación, no un dato de la solicitud. |
| Replicar en D365 | `ReplicarPedidoEnD365Message` | Llevar la solicitud a Dynamics 365. Es un comando (puede rechazarse), no un evento. |
| Administrativo de Servicio al Cliente | — | Rol que hoy digita a mano los pedidos. Es el trabajo que el sistema automatiza. |
| Ventana de carga | — | Franja horaria en que las interfaces de integración están disponibles. Es una política del borde, no una regla de la solicitud. |

## Canal web (planilla)

| Término | En el código | Significado |
|---|---|---|
| Planilla | `PlanillaReader`, `PlanillaRow` | El archivo Excel estándar de carga de pedidos. Una fila por línea; las columnas de cabecera se repiten en cada fila del mismo pedido. |
| Carga | `CargaProcessor` | Subir una planilla para un laboratorio y un tipo de orden. Es todo o nada: si un pedido tiene un error, no se publica ninguno. |
| Resumen de validación | `CargaDePedidos.razor` | Lista de errores al pie de la pantalla. Cada error indica qué falló, el valor ofensor, en qué pedido y cómo corregirlo. |
| Validación de formato | `FormatValidator` | Reglas propias del archivo: celdas obligatorias vacías, formato de celda y formato de fecha. Viven en Ingesta. |
| Invariante | — | Regla propia del pedido, que existiría aunque el pedido entrara por teléfono. Vive en el Núcleo. |
| Fila huérfana | — | Fila con datos pero sin Número de Delivery. Es el único error que se informa por número de fila y no por pedido. |

## Mensajería e integración

| Término | En el código | Significado |
|---|---|---|
| Comando | `...Command`, `...Message` | Petición que puede rechazarse. Tiene un solo responsable, viaja por una cola y su nombre va en imperativo. |
| Evento | — | Hecho que ya ocurrió y no se puede rechazar. Puede tener varios interesados, viaja por un tópico y su nombre va en pasado. |
| `...Command` / `...Message` | — | `Command` es una orden dentro del proceso; `Message` es el contrato que viaja por el cable, solo con primitivos. |
| Evento de dominio | `IDomainEvent` | Hecho del agregado. Vive dentro del Núcleo, lleva value objects y no cruza la frontera. |
| Solicitud de Ingreso Registrada | `SolicitudDeIngresoRegistrada` | Evento de dominio que se levanta al crear una solicitud. Nunca se levanta al rehidratarla desde la base. |
| Doble escritura | — | Escribir en SQL y en Service Bus sin una transacción común. La resuelve el outbox (ADR-0005). |
| Outbox | `IOutbox`, `OutboxEntry` | Tabla del Núcleo donde el comando queda pendiente de publicar, en la misma transacción que la solicitud. Una fila está pendiente mientras `ProcessedAt` es nulo. |
| Publicador del outbox | *(nombre de clase pendiente)* | Componente del Worker que envía las filas pendientes a su cola y las marca como procesadas. No tiene lógica de negocio. |
| Correlación | `MessageId`, `CorrelationId` | El comando sale con `MessageId` igual al Id de la solicitud; el resultado vuelve con `CorrelationId` igual a ese mismo Id. |
| Destino del mensaje | `MessageDestination` | Qué hacer con un mensaje recibido: completarlo o mandarlo a la dead-letter. La falla transitoria no tiene valor propio: se deja expirar el lock. |
| Falla no clasificada | — | Excepción que no se tradujo a un destino. Se trata como transitoria: se registra y no se hace settlement. |
| Forma del mensaje | `RegistrarSolicitudMessageDispatcher` | Que el JSON traiga los campos esperados con el tipo esperado. Es distinto de la validez de negocio. |
| Anticorruption layer | `CodigosD365Mapper` | Traducción entre el formato del laboratorio y el de D365. Por ejemplo, la planilla trae el RUT sin prefijo y D365 espera `23-0775634129`. |
