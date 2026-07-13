# RevelioII

## Propósito del Proyecto
RevelioII es una aplicación web interactiva basada en grafos. El objetivo es crear una experiencia donde se visualiza una estructura de grafos relacional almacenada en SQLite. La arquitectura está diseñada para ser simple, mantenible y rápida de desarrollar, operando como un solo proyecto cohesivo.

La tecnología principal incluye:
- **C# y ASP.NET Core 9 (Razor Pages)** para el backend y estructuración de la UI.
- **SQLite** con Entity Framework Core para almacenar tanto datos relacionales regulares como la estructura del grafo (Nodos y Relaciones).
- **Cytoscape.js o vis-network** para el renderizado e interacción del grafo.
- **Bootstrap** para la capa base de estilos y UI.

## Requisitos Previos
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

## Instalación y Ejecución

Sigue estos pasos para compilar y ejecutar el proyecto de forma local:

1. **Abrir una terminal** y navegar al directorio raíz del proyecto (`RevelioII`).
   
2. **Restaurar las dependencias (NuGet packages):**
   ```bash
   dotnet restore
   ```

3. **Compilar la solución:**
   ```bash
   dotnet build
   ```

4. **Ejecutar la aplicación:**
   ```bash
   dotnet run
   ```

5. **Acceder a la herramienta:**
   Una vez que la aplicación inicie, la consola mostrará la URL de acceso local. Abre tu navegador web e ingresa a dicha dirección (usualmente `http://localhost:5000` o `https://localhost:5001`).

## Ejecución de Pruebas Unitarias
El proyecto incluye un proyecto separado de pruebas de unidad basadas en **xUnit** y **Moq** (`RevelioII.UnitTests`). Para ejecutarlas, corre el siguiente comando en la raíz:

```bash
dotnet test
```