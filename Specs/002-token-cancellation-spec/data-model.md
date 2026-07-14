# Data Model: Cooperative Cancellation Implementation

## Overview
This feature does not introduce new persisted entities or database schema changes. The design adds cancellation flow metadata to existing asynchronous operations and preserves the existing graph domain model.

## Entities and Contracts

### Cancellation Request Context
- Type: Runtime-only control signal
- Source: ASP.NET Core request pipeline
- Fields:
  - `CancellationToken`: request-scoped cancellation signal
  - `Origin`: browser disconnect, navigation away, or server-initiated request abort
  - `Scope`: one HTTP request and all async work executed on its behalf
- Relationships:
  - Flows from PageModel handlers to `IGraphManagementService`
  - Flows from `IGraphManagementService` to `IGraphRepository`
  - Flows from `GraphManagementService.NotifySlackAsync` to `HttpClient.PostAsJsonAsync`
- State transitions:
  - Active -> CancelRequested -> ObservedByAwaitedOperation -> OperationAborted

### Node
- Existing persisted entity in SQLite
- Role in this feature: node read and write operations must honor the request cancellation token
- Persistence changes: none

### Relationship
- Existing persisted entity in SQLite
- Role in this feature: relationship reads, validation lookups, and write operations must honor the request cancellation token
- Persistence changes: none

### Graph View Projection
- Type: runtime DTO aggregation (`GraphViewDto`, `GraphNodeDto`, `GraphEdgeDto`)
- Role in this feature: graph projection logic remains unchanged, but its underlying data fetch becomes cancellable
- Persistence changes: none

### External Notification Dispatch
- Type: runtime outbound message payload
- Current payload shape: `{ text: string }`
- Role in this feature: outbound Slack notification must be cancellable and remain aligned with the parent request lifetime
- Persistence changes: none

## Validation Rules
- Cancellation token parameters remain optional at interface level for backward compatibility.
- Existing node and relationship validation rules stay unchanged.
- Existing graph DTO projection stays unchanged.
- Cancellation must not introduce partial persistence within a single awaited EF Core call beyond normal transactional behavior already provided by EF Core/SQLite.

## Impacted Boundaries
- Request boundary: Razor Page handlers and JSON handlers
- Service boundary: `IGraphManagementService` / `GraphManagementService`
- Repository boundary: `IGraphRepository` / `GraphRepository`
- External I/O boundary: Slack HTTP POST
