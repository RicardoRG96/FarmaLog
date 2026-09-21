# ADR-0003: ClosedXML en lugar de EPPlus para leer la planilla

**Estado:** aceptado
**Ámbito:** dependencias de Ingesta

## Contexto

Ingesta tiene que leer archivos `.xlsx` generados con el template de FarmaLog. En .NET las dos opciones más usadas son EPPlus y ClosedXML. EPPlus es la más conocida y la que aparece en la mayoría de los tutoriales.

## Decisión

Usé ClosedXML, que tiene licencia MIT.

## Razón

EPPlus cambió de licencia en la versión 5. Pasó de LGPL a Polyform Noncommercial, que permite usarla gratis en proyectos personales, de investigación o en organizaciones sin fines de lucro, pero exige una licencia pagada por desarrollador para usarla en una empresa.

FarmaLog es un operador logístico comercial, así que un sistema que ingresa sus pedidos al ERP es claramente uso comercial.

Para mí la licencia es una restricción de arquitectura y no un detalle del paquete NuGet. Un problema de licencia no aparece al compilar ni en los tests. Aparece cuando alguien del área legal o de compras revisa las dependencias, normalmente tarde. Y cambiar de librería en ese punto no es solo quitar un paquete: significa reescribir la lectura de archivos, que en este caso es el centro de un servicio completo.

## Consecuencias

- No hay costo de licencia ni riesgo legal para llevar esto a producción.
- ClosedXML tiene una API más acotada que EPPlus (no maneja tablas dinámicas ni fórmulas complejas). Aquí no afecta, porque solo necesito leer celdas.
- La dependencia está aislada en `PlanillaReader`. Ninguna otra clase de Ingesta conoce ClosedXML, así que cambiar de librería implica tocar un solo archivo.
- ClosedXML necesita un stream que permita desplazarse por el archivo (seek), por eso copio el contenido a un `MemoryStream`. Con el límite de 5 MB del Portal no es problema. Si ese límite sube o aparece mucha concurrencia habría que revisarlo, y lo dejé anotado como deuda.

## Alternativas descartadas

- **EPPlus con licencia comercial:** resuelve lo legal pagando, pero no se justifica solo para leer celdas.
- **EPPlus 4.x (la última versión LGPL):** no recibe soporte ni parches desde 2020. Usar una versión abandonada para evitar una licencia es cambiar un problema por otro.
- **Open XML SDK directamente:** es gratis y no tiene problemas de licencia, pero trabaja a nivel del XML interno del formato. Tendría sentido si necesitara un control más fino; aquí solo agregaba complejidad.
