# ADR-0005: Por qué se usa un outbox transaccional

**Estado:** aceptado
**Ámbito:** consistencia de datos distribuidos

## Contexto

Existe un conflicto de doble escritura entre SQL y Service Bus en el momento en que el `Núcleo` recibe una solicitud de `Ingesta`, ya que si el proceso del `Worker` muere entre guardar y publicar el comando `ReplicarPedidoEnD365` (en la cola `replicar-pedido-d365` lo consume la Function de replicación de pedidos en D365) la solicitud queda persistida en la base de datos, pero el comando nunca se publica en la cola de Service Bus. En este escenario hipotético, la garantía de `at least once` de Service Bus, no es aplicable, ya que el mensaje nunca llegó a aceptarse, y es a partir de ese momento que Service Bus promete entregarlo al consumidor al menos una vez. Un mensaje que nunca se envió no existe para Service Bus, así que no hay nada que entregar.

## Opciones

Opción A: completar al final y reintentar de forma idempotente.

Para este caso el orden sería: guardar, publicar y completar el mensaje entrante desde `Ingesta`. Si llega un duplicado, se publica igual y se completa, el comando lleva un `MessageId` determinístico, y la cola `replicar-pedido-d365` tiene activada la detección de duplicados de Service Bus.

A Favor: 

- No agrega tablas ni procesos. 
- Se aprovecha la restricción única de la base de datos (par código laboratorio - número delivery).

En contra: 

- La consistencia depende del reenvío. Si publicar falla siempre, el mensaje entrante termina en la DLQ después de varios intentos, con la solicitud ya guardada.

Opción B: outbox transaccional.

En esta alternativa, se guarda tanto la solicitud como el registro en la tabla outbox con el comando serializado en la misma transacción. Un proceso aparte lee las filas pendientes, las publica y las marca como enviadas.

A favor: 

- Guardar y "quedar pendiente de publicar" es atómico, por lo tanto, asegura que el mensaje se publique al menos una vez, con la solicitud ya persistida.

En contra: 

- Agrega una tabla y un dispatcher. 
- Hay un riesgo propio de este proyecto, que es `KEDA` bajando las réplicas del `Worker` a cero cuando no hay mensajes en la cola, y el nuevo dispatcher vive en ese Worker y por lo tanto, se apaga con él.

## Decisión

Se decide utilizar outbox transaccional.

## Consecuencias

Al utilizar outbox transaccional, el consumidor tiene que ser idempotente, ya que el proceso puede publicar el mismo mensaje varias veces. Se logra añadiendo el ID del agregado como valor a la propiedad `MessageId` y configurando la detección de duplicados en la cola.