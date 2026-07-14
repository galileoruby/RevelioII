# Copilot Instructions

## Project Guidelines
- For the graph UI, the user does not want any white line or incomplete ring on node hover; hover should avoid relationship visual effects and only use a safe subtle node-only effect if needed.

## Quick Start Commands
- Restore packages: `dotnet restore`
- Build app and tests: `dotnet build`
- Run web app: `dotnet run`
- Run all tests: `dotnet test`
- Run unit tests only: `dotnet test RevelioII.UnitTests/`

## Architecture Boundaries
- Presentation layer: `Pages/` Razor Pages and page handlers.
- Business layer: `Services/` (primary orchestration in `GraphManagementService`).
- Data access layer: `Repositories/` (`IGraphRepository` + `GraphRepository`).
- Persistence/model layer: `Data/` and `Models/` (EF Core + SQLite).
- DTO contracts for graph payloads: `DTOs/`.

## Project Conventions
- Keep async flows cancellation-aware end-to-end: accept a `CancellationToken` and pass it through every async dependency call.
- Preserve cancellation semantics: if `OperationCanceledException` occurs and cancellation was requested, rethrow.
- Respect DbContext relationship rules: node deletes must remove linked relationships first (FK `Restrict`).
- Preserve nullable intent (`<Nullable>enable</Nullable>`): use nullable annotations and avoid null-forgiving unless unavoidable.
- For HTTP calls, prefer injected `HttpClient` and avoid per-request client creation.

## Code Review Standards

### Documentation
- Ensure functions have docstrings explaining parameters and return types.
- Include XML documentation comments (`///`) for public methods in C#.
- Document complex logic with inline comments explaining the "why", not just the "what".

### Code Quality
- Preserve existing behavior unless the requirement explicitly changes it.
- Add or update tests for meaningful behavior changes.
- In tests, follow Arrange/Act/Assert and verify cancellation token forwarding where relevant.

## Key References
- Project overview and setup: [README.md](../README.md)
- Spec workflow and template: [specify.md](../specify.md)
- Feature specs and implementation artifacts: [Specs/](../Specs/)