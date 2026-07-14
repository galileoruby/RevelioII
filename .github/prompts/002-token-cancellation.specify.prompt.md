---
name: 002-token-cancellation-specify
---

# Specification: Cooperative Cancellation Implementation for Graph Architecture


Generate a spec named `002-token-cancellation-spec` that outlines the implementation of cooperative cancellation using `CancellationToken` across the data access and service layers of the `RevelioII` application (Razor Pages + SQLite). The spec should include detailed instructions on how to modify the interfaces, repository layer, and service layer to support cancellation tokens, as well as strict verification constraints to ensure that business logic and flow are preserved.

## Context & Objective
Implement cooperative cancellation using `CancellationToken` across the data access and service layers of the `RevelioII` application (Razor Pages + SQLite). This ensures that long-running database queries and external HTTP requests are aborted immediately if a user navigates away or cancels a web request, preventing unnecessary resource consumption.

## Scope of Work

### 1. Interface Updates
*   Modify `IGraphRepository` to append `CancellationToken cancellationToken = default` to all asynchronous method signatures.
*   Modify `IGraphManagementService` to append `CancellationToken cancellationToken = default` to all asynchronous method signatures.

### 2. Repository Layer (`GraphRepository.cs`)
*   Update all async methods to accept the `CancellationToken` parameter.
*   Forward the token into every Entity Framework Core async operation:
    *   `ToListAsync(cancellationToken)`
    *   `FindAsync(new object[] { id }, cancellationToken)`
    *   `SaveChangesAsync(cancellationToken)`
    *   `FirstOrDefaultAsync(..., cancellationToken)`

### 3. Service Layer (`GraphManagementService.cs`)
*   Update all async methods to accept the `CancellationToken` parameter.
*   Pass the token down to every matching repository invocation (e.g., `_repository.GetNodesAsync(cancellationToken)`).
*   Pass the token into `HttpClient.PostAsJsonAsync` inside the `NotifySlackAsync` method.

## Strict Verification Constraints (What NOT to do)

*   **Preserve Business Logic:** Do not modify, remove, or rearrange any validation rules, exceptions, or mapping logic (such as the manual projection to `GraphViewDto` in `GetGraphViewAsync`).
*   **Preserve Slack Flow:** Do not alter the asynchronous invocation of `NotifySlackAsync` inside `CreateNodeAsync`. The token should only be passed down through the arguments where applicable, without changing the execution flow.
*   **No Placeholders:** Do not emit partial code or comments like `// existing logic...`. Output complete, production-ready files.
*   **No New Dependencies:** Do not introduce external packages, architectural patterns, or logging frameworks.
