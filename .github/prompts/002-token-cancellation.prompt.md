---
name: 002-token-cancellation-prompt
---

# Specification: Cooperative Cancellation Implementation for Graph Architecture

## Context & Objective
Implement cooperative cancellation using `CancellationToken` across the data access and service layers of the `RevelioII` application (Razor Pages + SQLite). This ensures that long-running database queries and external HTTP requests are aborted immediately if a user navigates away or cancels a web request, preventing unnecessary resource consumption.

## Scope of Work

### 1. Interface Updates
*   Modify `IGraphRepository` to append `CancellationToken cancellationToken = default` to all asynchronous method signatures.
*   Modify `IGraphManagementService` to append `CancellationToken cancellationToken = default` to all asynchronous method signatures.

### 2. Repository Layer (`GraphRepository.cs`)
*   Update all async methods to accept the `CancellationToken` parameter.
*   Forward the token into every Entity Framework Core async operation:
    *   `ToListAsync(cancellationToken)`[cite: 2]
    *   `FindAsync(new object[] { id }, cancellationToken)`[cite: 2]
    *   `SaveChangesAsync(cancellationToken)`[cite: 2]
    *   `FirstOrDefaultAsync(..., cancellationToken)`[cite: 2]

### 3. Service Layer (`GraphManagementService.cs`)
*   Update all async methods to accept the `CancellationToken` parameter[cite: 1].
*   Pass the token down to every matching repository invocation (e.g., `_repository.GetNodesAsync(cancellationToken)`)[cite: 1].
*   Pass the token into `HttpClient.PostAsJsonAsync` inside the `NotifySlackAsync` method[cite: 1].

## Strict Verification Constraints (What NOT to do)

*   **Preserve Business Logic:** Do not modify, remove, or rearrange any validation rules, exceptions, or mapping logic (such as the manual projection to `GraphViewDto` in `GetGraphViewAsync`)[cite: 1].
*   **Preserve Slack Flow:** Do not alter the asynchronous invocation of `NotifySlackAsync` inside `CreateNodeAsync`[cite: 1]. The token should only be passed down through the arguments where applicable, without changing the execution flow[cite: 1].
*   **No Placeholders:** Do not emit partial code or comments like `// existing logic...`. Output complete, production-ready files.
*   **No New Dependencies:** Do not introduce external packages, architectural patterns, or logging frameworks.

* **Don't write code inmediately.** First, provide a detailed plan of how you will implement the cooperative cancellation across the repository and service layers, including method signatures and where the `CancellationToken` will be passed. After the plan is approved, proceed to write the code.