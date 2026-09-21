# ADR-0002: Contratos duplicados en cada servicio, sin proyecto compartido

**Estado:** aceptado, con un costo real que ya apareció
**Ámbito:** acoplamiento entre servicios

## Contexto

Los mensajes `RegistrarSolicitudIngreso` y `LineaDeMensaje` viajan por la cola. Ingesta los publica y el Núcleo los consume, así que los dos necesitan conocer su forma. Lo habitual en .NET sería crear un proyecto `FarmaLog.Contracts` y referenciarlo desde ambos.

## Decisión

Cada servicio tiene su propia copia del contrato. No comparten ningún tipo.

## Razón

Un proyecto compartido vuelve a acoplar los despliegues, que es justo lo que quería evitar al separar los servicios. Con un `Contracts` compartido:

- Agregar un campo obliga a recompilar y desplegar los dos servicios al mismo tiempo.
- En el diagrama hay dos servicios independientes, pero en la práctica se despliegan como uno solo.
- Con el tiempo, ese proyecto tiende a llenarse de cosas que no son contratos (helpers, enums, validaciones) y termina convertido en un dominio compartido.

Al duplicar, cada servicio puede evolucionar por su lado. Ingesta puede agregar un campo y desplegarse sola, y el Núcleo lo ignora hasta que lo necesite.

## El costo, que ya apareció

Duplicar contratos requiere algo que detecte cuando las copias se desalinean, y eso todavía no existe. Lo anoté como deuda antes de que pasara, y pasó el 24 de agosto de 2026.

Una propiedad se llamaba `Errores` en un servicio y `Errors` en el otro. `ReadFromJsonAsync` con `JsonSerializerDefaults.Web` no distingue mayúsculas de minúsculas, pero obviamente no traduce entre idiomas. La propiedad no se enlazó, el record quedó con la lista en `null` y la pantalla lanzó un `NullReferenceException`.

Lo que aprendí es más puntual que "duplicar tiene un costo": cuando renombras algo con ayuda del compilador, ese cambio no llega al otro lado de la red. El compilador revisa referencias, pero no tiene cómo saber que dos tipos que nunca se referencian entre sí describen el mismo mensaje.

Para encontrar el problema llamé a Ingesta directamente con `curl`, sin pasar por el Portal. Con eso separé el problema en dos: o fallaba el servicio o fallaba la comunicación entre ambos. Ver el JSON crudo, con los nombres reales de las propiedades, resolvió la mitad del diagnóstico.

## Solución identificada, pendiente

Un test de contrato por cada frontera: serializar con la copia del que envía, deserializar con la copia del que recibe y comprobar que todos los campos lleguen con valor. Corre en CI sin infraestructura, y fallaría en el commit que introduce el problema en lugar de fallar en el navegador.

Quedó fuera del alcance de esta etapa, pero está registrado como deuda, con la causa y la solución identificadas.

## Alternativas descartadas

- **Proyecto `Contracts` compartido:** acopla los despliegues, que es lo que este ADR busca evitar.
- **Paquete NuGet versionado con el contrato:** evita el acoplamiento de compilación y mantiene una sola fuente. Es una opción válida, y probablemente la correcta a mayor escala. La descarté porque montar la publicación de paquetes era demasiado para dos servicios y un solo desarrollador.
