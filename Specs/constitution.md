# Project Constitution

## 1. Purpose
This project will be built as a single deployable web application using C# and ASP.NET Core Razor Pages. The goal is to create an interactive graph-based experience with a relational graph structure stored in SQLite, while keeping the architecture simple, maintainable, and fast to develop.

## 2. Core Technology Stack
- ASP.NET Core Razor Pages
- C# with .NET 9
- Razor Pages and Bootstrap for the UI
- SQLite with Entity Framework Core for graph and relational data storage
- Cytoscape.js or vis-network for graph rendering
- Serilog for logging
- Dependency Injection as the default pattern

## 3. Architectural Principles
- Keep the application as one cohesive project rather than splitting into multiple services.
- Prefer simplicity over over-engineering.
- Keep page model logic thin and move business rules into services.
- Use DTOs to shape data between the backend and the UI.
- Keep graph operations clearly separated from regular CRUD logic.
- Favor readable and explicit code over abstract patterns.

## 4. Project Structure
The solution should follow a clear structure:

- Pages/  
  Razor Pages with their associated PageModels that handle HTTP requests and coordinate with services.

- Models/  
  Represents domain entities and view models.

- Services/  
  Contains business logic and orchestration.

- Repositories/  
  Contains data access logic, especially for graph queries.

- DTOs/  
  Defines request/response contracts for the UI.

- wwwroot/js/  
  Frontend JavaScript for graph interaction and UI behavior.

- Data/  
  Database context, configuration, and migration-related code.

## 5. Data Strategy
- Use SQLite as the primary database for all data, including graph structures.
- Graph data will be stored using a relational model:
  - **Nodes table**: Stores entities with properties (Id, Type, Label, Properties as JSON).
  - **Relationships table**: Stores edges in the format `NodeA -[RelationName]- NodeB` (SourceNodeId, TargetNodeId, RelationType, Properties as JSON).
- Graph queries should return simplified node and edge data for visualization.
- Avoid loading excessive graph data at once; support pagination, filtering, and depth limits.
- Use Entity Framework Core for data access with proper indexing on foreign keys and relationship lookups.

## 6. Graph Experience Rules
The application should prioritize a graph-first user experience:
- Nodes should be visually distinguishable by type.
- Relationships should be readable and clearly labeled.
- Users should be able to search, select, expand, and inspect nodes.
- The graph should support zoom, pan, and selection interactions.
- The UI should remain responsive even for moderately sized graphs.

## 7. Backend Rules
- Use asynchronous programming for database and I/O operations.
- Inject dependencies through the built-in DI container.
- Validate all incoming input on the server side.
- Return consistent JSON responses for API endpoints.
- Handle exceptions gracefully and log them with useful context.

## 8. Frontend Rules
- Keep UI behavior simple and focused on graph exploration.
- Avoid putting complex logic directly in Razor Pages.
- Use JavaScript only where needed for interaction and rendering.
- Ensure the UI remains usable on desktop and tablet screens.

## 9. Quality Standards
- Write unit tests for services and business logic.
- Keep configuration values in environment variables or configuration files.
- Use meaningful names for classes, methods, and view models.
- Avoid premature optimization but remain mindful of performance.

## 10. Non-Goals
This project will not initially:
- Split into separate backend and frontend repositories.
- Use a microservices architecture.
- Introduce unnecessary abstractions before they are needed.

## 11. Decision Rule
When there is a tradeoff, the project will favor:
1. Simplicity
2. Maintainability
3. Fast development
4. Strong graph interaction experience

## 12. Version
- Version: 2.0
- Status: Updated to SQLite and Razor Pages architecture