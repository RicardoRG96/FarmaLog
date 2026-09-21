# ADR-0004: Imagen base Debian en lugar de alpine o chiseled

**Estado:** aceptado
**Ámbito:** empaquetado del Worker del Núcleo

## Contexto

El Worker del Núcleo se despliega en Azure Container Apps. Para imágenes .NET lo que normalmente se recomienda es usar una imagen mínima, como `alpine` o `chiseled`, que pesan bastante menos y reducen la superficie de ataque. Si el único criterio fuera el tamaño, sería la elección obvia.

## Decisión

Usé la imagen base Debian (la variante por defecto de `mcr.microsoft.com/dotnet/aspnet`), aunque sea más grande.

## Razón: la decide el dominio, no la infraestructura

El sistema trabaja con la ventana horaria de carga de FarmaLog, que está en hora de Chile. El código obtiene la zona `America/Santiago` con `TimeZoneInfo.FindSystemTimeZoneById`, y lo hace en el constructor para que, si el entorno está mal configurado, el proceso falle al arrancar y no en medio de un cambio de horario a las tres de la mañana.

Esa llamada depende del paquete `tzdata` del sistema operativo, que es el que trae la base de zonas horarias IANA en `/usr/share/zoneinfo`.

- Las imágenes chiseled de Ubuntu para .NET no incluyen `tzdata`. Es una decisión intencional de sus mantenedores, en línea con el equipo de .NET en Microsoft, para mantener la imagen lo más pequeña posible. Quien necesite zonas horarias tiene que armar su propia variante.
- Las imágenes Alpine para .NET tampoco lo incluyen.
- Sin `tzdata`, `FindSystemTimeZoneById` lanza un `TimeZoneNotFoundException`, con un `DirectoryNotFoundException` interno que apunta a la ruta que falta.

En Chile esto es todavía más delicado. El horario cambia dos veces al año, y Chile continental e Isla de Pascua tienen zonas distintas. Ningún cálculo con un desfase fijo reemplaza a la base IANA actualizada.

En resumen, un requisito de negocio (que la ventana de carga se mida en hora de Chile) terminó definiendo qué imagen base usar. Es el tipo de decisión que sale mal cuando se optimiza solo por métricas de infraestructura sin mirar el dominio.

## Consecuencias

- La imagen es más grande y tiene más paquetes que una chiseled.
- El manejo de horarios es correcto, y si algún día falta la base de zonas horarias, el proceso no arranca en lugar de fallar más tarde.
- Hay más paquetes que mantener actualizados.

## Alternativas descartadas

- **Chiseled con `tzdata` agregado manualmente:** se puede hacer, y a mayor escala probablemente sea lo correcto. Lo descarté por el tiempo que requería construirlo y verificarlo en esta etapa.
- **Guardar todo en UTC y convertir al mostrar:** sirve para almacenar, y de hecho es lo que hago. Pero no resuelve el problema, porque para saber si un pedido entró dentro de la ventana hay que convertir a hora local, y esa conversión también necesita `tzdata`.
- **Usar un desfase fijo de UTC-4:** falla con el cambio de horario. Es un bug con fecha conocida.
