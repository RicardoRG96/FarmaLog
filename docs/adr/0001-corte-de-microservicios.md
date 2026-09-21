# ADR-0001: Criterio para separar los microservicios

**Estado:** aceptado
**Ámbito:** arquitectura general

## Contexto

Este sistema se podía construir como un monolito modular o como varios servicios. Si iba por servicios, necesitaba un criterio claro para decidir dónde cortar. El error más común es crear un microservicio por cada entidad, lo que suma complejidad operativa sin dar autonomía real.

## Decisión

Usé cinco criterios, y a cada servicio le pedí que cumpliera más de uno:

1. Quién es dueño de los datos
2. Cada cuánto cambia
3. Cómo escala
4. Aislamiento de fallas
5. Autonomía para desplegarse

Con eso llegué a esta separación:

| Componente | Decisión | Criterio principal |
|---|---|---|
| Núcleo | Servicio propio, con estado | Es el único dueño de los datos y controla el ciclo de vida del agregado. |
| Ingesta | Servicio propio, sin estado | Ritmo de cambio. El ACL cambia cada vez que entra un laboratorio nuevo o cambia el ERP. El dominio no. |
| Portal | Servicio propio | Forma de escalar. Blazor Server mantiene una conexión SignalR con estado por usuario, así que escala por memoria y conexiones. El Worker escala según los mensajes en cola. Son dos ejes distintos. |
| Replicación | Azure Function (rebanada 2) | Sin estado, reactiva y de ejecución corta. |
| Notificaciones | Azure Function (rebanada 2) | Mismo caso que Replicación. |

El consumidor del Núcleo lo implementé como un `BackgroundService` en Container Apps y no como una Azure Function. Es un proceso de larga duración que necesita un `DbContext`, migraciones y change tracking, y eso no encaja bien con el modelo de ejecución de una Function. Lo que me interesaba de la Function era poder escalar a cero, y eso lo obtuve con KEDA.

## Por qué Ingesta es un servicio y no una entidad más

Ingesta no administra ninguna entidad. De hecho, no tiene base de datos. Su trabajo es traducir el formato de la planilla al contrato que entiende el dominio. La idea es que la parte del sistema que depende de formatos externos quede separada del dominio.

Se ve claro con un ejemplo: si entra un laboratorio nuevo con otra planilla, solo cambia Ingesta. El agregado no se entera.

## Consecuencias

A favor:

- El servicio que guarda estado y el que más cambia se despliegan por separado.
- El Núcleo escala desde cero, así que sin mensajes en la cola no hay costo de cómputo.
- Si mañana agrego un evento `SolicitudDeIngresoAceptada` en un tópico, no tengo que tocar nada de lo que ya existe. Para mí esa es la mejor señal de que el corte quedó bien.

En contra (asumido):

- Los contratos de mensaje quedan duplicados a ambos lados de la cola (ver ADR-0002).
- Un error puede pasar por tres procesos distintos, así que hace falta correlación para diagnosticarlo.
- Hay más cosas que operar que en un monolito modular.

## Alternativa descartada

Monolito modular. Para el volumen actual habría sido defendible. Lo descarté por dos razones: el objetivo era construir una arquitectura distribuida real, y los dos ejes de escalado (conexiones SignalR en el Portal y mensajes en cola en el Worker) no conviven bien en un mismo proceso.
