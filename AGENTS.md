# MejorPrecio — Análisis de precios asistido por IA

## Propósito
App para mantener listas de precios por proveedor, producto, familia y vigencia, y consultar el mejor precio de compra para una fecha dada.

## Stack
- .NET 10, C#, Razor Pages
- Entity Framework Core (acceso a SQL Server)
- MS SQL Server 2019
- Front: HTML + Bootstrap
- Excel: ClosedXML
- Hosting: Internet Information Server / Kestrel

## Cómo correr
- Configurar el connection string en `appsettings.json` (sección `ConnectionStrings`) antes de levantar
- `dotnet run` (corre directo con Kestrel)
- Todavía no hay proyecto de tests

## Qué NO hacer
- NO hardcodear el connection string a SQL Server — va en `appsettings.json`, sección `ConnectionStrings`
- NO agregar features fuera del PRD
- NO enviar mail con los resultados de la consulta (fuera de alcance del PRD)
- NO permitir que "Vigencia desde" se guarde con una fecha anterior o igual a la fecha actual (RF-12)
