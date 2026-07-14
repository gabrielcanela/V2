# PRD-001: MejorPrecio — Análisis de precios asistido por IA

## Contexto y Problema
Mantener una lista de precios actualizadas por proveedor, producto, familia y vigencia para permitir seleccionar el mejor precio de compra en una fecha determinada.

**Personas:**
Personal de la oficina de compras que debe decidir compras a granel para una empresa de catering.
Podría ser también utilizado por la gerencia

**Nota:** no hay definido ningún control de acceso (por ejemplo, si distintos usuarios deben ver o no los mismos datos de precios). Si corresponde, falta agregar el/los requerimiento(s) y su(s) criterio(s) de aceptación correspondientes.

## Objetivos
- Poder actualizar la lista de precios mediante un Excel con las siguientes columnas: Código de proveedor, Código de producto, Familia, Precio unitario, Vigencia desde, Vigencia Hasta.
- Poder consultar la lista de precios por fecha, familia, proveedor y descripción del producto para que muestre los precios más bajos para cada tipo de producto indicando proveedor, producto, descripción del producto, familia, precio y vigencia.

## Requerimientos Funcionales
Al importar el Excel de precios deben validarse las siguientes reglas:

- RF-01: El proveedor no debe ser un valor vacío.
- RF-02: El proveedor debe existir en la tabla Proveedor de la base de datos.
- RF-03: El código de producto no debe ser un valor vacío.
- RF-04: El código de producto debe existir en la tabla ProductoProveedor de la base de datos para ese proveedor.
- RF-05: La familia no debe ser un valor vacío.
- RF-06: La familia debe existir en la tabla Familia de la base de datos.
- RF-07: El precio no debe ser un valor vacío.
- RF-08: El precio debe ser un número válido.
- RF-09: El precio debe tener dos lugares decimales.
- RF-10: La vigencia desde no debe ser un valor vacío.
- RF-11: La vigencia desde debe ser una fecha válida.
- RF-12: La vigencia desde debe ser mayor a la fecha actual.
- RF-13: La vigencia hasta debe ser una fecha válida.
- RF-14: La vigencia hasta debe ser mayor a la vigencia desde.
- RF-15: El sistema debe permitir confirmar la actualización vía Excel antes de actualizar la base de datos.

Sobre la consulta de precios:

- RF-16: La consulta debe permitir filtrar por fecha.
- RF-17: La consulta debe permitir filtrar por proveedor o por todos los proveedores.
- RF-18: La consulta debe permitir filtrar por familia o por todas las familias.
- RF-19: La consulta debe permitir filtrar por descripción del producto o dejar ese campo vacío.
- RF-20: La consulta debe mostrar una grilla con las siguientes columnas: Código de proveedor, Descripción del proveedor, Código de producto, Descripción del producto, Familia, Precio unitario, Vigencia desde, Vigencia Hasta.
- RF-21: La grilla de la consulta debe estar ordenada por menor precio.

## Requerimientos No Funcionales
- RNF-01: Tiene que ser una aplicación con arquitectura Razor Pages y lenguaje C#.
- RNF-02: El sistema debe conectarse con el Microsoft SQL Server.
- RNF-03: Los parámetros de la conexión no deben estar en el código.
- RNF-04: El tiempo de respuesta de la consulta no debe ser superior a 2 segundos, medido en el percentil 95 (p95), bajo una carga concurrente de hasta 10 usuarios simultáneos, sobre una base de referencia de hasta 50.000 registros de precios vigentes.

## Criterios de Aceptación
- AC-01 (RF-01): Dado que la celda de Excel en la columna de código de proveedor está vacía, cuando se procese el archivo, entonces debe mostrar un cartel "Falta código de proveedor".
- AC-02 (RF-02): Dado que la celda de Excel en la columna de código de proveedor tiene un código de proveedor inexistente en la tabla Proveedor, cuando se procese el archivo, entonces debe mostrar un cartel "Código de proveedor inexistente".
- AC-03 (RF-03): Dado que la celda de Excel en la columna de código de producto está vacía, cuando se procese el archivo, entonces debe mostrar un cartel "Falta código de producto".
- AC-04 (RF-04): Dado que la celda de Excel en la columna de código de producto tiene una combinación de código de producto y código de proveedor inexistente en la tabla ProductoProveedor, cuando se procese el archivo, entonces debe mostrar un cartel "Código de producto inexistente para el proveedor xxx".
- AC-05 (RF-05): Dado que la celda de Excel en la columna de familia está vacía, cuando se procese el archivo, entonces debe mostrar un cartel "Falta familia".
- AC-06 (RF-06): Dado que la celda de Excel en la columna de familia tiene una familia inexistente en la tabla Familia, cuando se procese el archivo, entonces debe mostrar un cartel "Familia inexistente".
- AC-07 (RF-07): Dado que la celda de Excel en la columna de precio unitario está vacía, cuando se procese el archivo, entonces debe mostrar un cartel "Falta Precio".
- AC-08 (RF-08): Dado que la celda de Excel de precio unitario tiene un contenido no numérico, cuando se procese el archivo, entonces debe mostrar un cartel "Precio no es un número válido".
- AC-09 (RF-09): Dado que la celda de Excel de precio unitario tiene un valor con una cantidad de decimales distinta a dos, cuando se procese el archivo, entonces debe mostrar un cartel "El precio debe tener exactamente dos decimales".
- AC-10 (RF-10): Dado que la celda de Excel en la columna de Vigencia desde está vacía, cuando se procese el archivo, entonces debe mostrar un cartel "Falta Vigencia desde".
- AC-11 (RF-11): Dado que la celda de Excel de Vigencia desde tiene una fecha errónea, cuando se procese el archivo, entonces debe mostrar un cartel "Vigencia desde no es una fecha válida".
- AC-12 (RF-12): Dado que la celda de Excel de Vigencia desde tiene una fecha menor a la fecha actual, cuando se procese el archivo, entonces debe mostrar un cartel "Vigencia desde debe ser mayor a la fecha actual".
- AC-13 (RF-13): Dado que la celda de Excel de Vigencia hasta tiene una fecha errónea, cuando se procese el archivo, entonces debe mostrar un cartel "Vigencia hasta no es una fecha válida".
- AC-14 (RF-14): Dado que la celda de Excel de Vigencia hasta tiene un valor anterior a la Vigencia desde, cuando se procese el archivo, entonces debe mostrar un cartel "Vigencia hasta debe ser posterior a Vigencia desde".
- AC-15 (RF-15): Dado que se cumplieron todas las validaciones, cuando se termine de revisar el Excel, entonces debe mostrar una opción que permita actualizar la base de datos o cancelar.
- AC-16 (RF-16): Dado que el usuario selecciona una fecha en el filtro de consulta, cuando se ejecute la consulta, entonces la grilla debe mostrar únicamente los precios cuya vigencia (desde-hasta) incluya la fecha seleccionada.
- AC-17 (RF-17): Dado que el usuario selecciona un proveedor específico o la opción "Todos los proveedores" en el filtro de consulta, cuando se ejecute la consulta, entonces la grilla debe mostrar únicamente los precios del proveedor seleccionado, o de todos los proveedores si se eligió esa opción.
- AC-18 (RF-18): Dado que el usuario selecciona una familia específica o la opción "Todas las familias" en el filtro de consulta, cuando se ejecute la consulta, entonces la grilla debe mostrar únicamente los precios de la familia seleccionada, o de todas las familias si se eligió esa opción.
- AC-19 (RF-19): Dado que el usuario ingresa un texto en el filtro de descripción del producto, cuando se ejecute la consulta, entonces la grilla debe mostrar únicamente los productos cuya descripción contenga ese texto; y dado que el campo se deja vacío, entonces la consulta no debe aplicar filtro por descripción.
- AC-20 (RF-20): Dado que se ejecuta la consulta, cuando se muestren los resultados, entonces la grilla debe incluir las columnas Código de proveedor, Descripción del proveedor, Código de producto, Descripción del producto, Familia, Precio unitario, Vigencia desde y Vigencia Hasta, en ese orden.
- AC-21 (RF-21): Dado que se ejecuta la consulta con resultados que incluyen múltiples precios para un mismo producto, cuando se muestre la grilla, entonces las filas deben estar ordenadas de menor a mayor precio unitario.

## Fuera de Alcance
- Envío de mail con los resultados de la consulta.

## Riesgos y Dependencias
- Riesgo: Errores en la consulta por duplicación de información.
- Dependencia: Windows Server, Internet Information Server, Microsoft SQL Server.
