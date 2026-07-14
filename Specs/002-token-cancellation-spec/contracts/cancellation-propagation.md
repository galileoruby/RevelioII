# Contract: Cancellation Propagation

## Scope
This feature changes internal async contracts so request cancellation can propagate from ASP.NET Core Razor Page handlers to EF Core and outbound HTTP calls. No external route shape or persisted data contract changes are required.

## Request Boundary Contract
Handlers that call async graph operations must accept a request-scoped cancellation token and pass it through unchanged.

Impacted handler categories:
- Home page graph JSON handler
- Node CRUD page handlers
- Relationship CRUD page handlers

## Service Contract Changes
All async methods on `IGraphManagementService` and `GraphManagementService` gain:
- `CancellationToken cancellationToken = default`

Impacted operations:
- Node retrieval, graph retrieval, node lookup, create, update, delete
- Relationship retrieval, lookup, create, update, delete
- Slack notification helpers

Contract rules:
- The token must be forwarded to every repository call made on behalf of the service method.
- Existing validation and DTO projection logic must remain unchanged.

## Repository Contract Changes
All async methods on `IGraphRepository` and `GraphRepository` gain:
- `CancellationToken cancellationToken = default`

Contract rules:
- Every EF Core async method must use the token-aware overload.
- Delete flows that execute multiple async operations must pass the same token to each awaited call.
- No repository method introduces new persistence behavior or schema changes.

## External Notification Contract
`NotifySlackAsync` gains:
- `CancellationToken cancellationToken = default`

Outbound payload:
- Unchanged JSON body with `text`

Contract rules:
- The token is forwarded to `HttpClient.PostAsJsonAsync`.
- Notification dispatch is request-coupled for this feature; if the request is cancelled, the outbound call is expected to cancel as well.

## Non-Goals
- No new REST endpoints
- No database schema changes
- No new background queue or notification infrastructure
