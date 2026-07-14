# Research: Cooperative Cancellation Implementation for Graph Architecture

## Decision 1: Propagate request cancellation from Razor Page handlers into the service layer
- Decision: PageModel handlers and JSON handlers that call graph services will accept a `CancellationToken` parameter supplied by ASP.NET Core and forward it to the service layer.
- Rationale: The repository and service layers cannot observe browser disconnects if every call starts with `CancellationToken.None`. The request boundary must pass `HttpContext.RequestAborted` into the first async application call.
- Alternatives considered: Reading `HttpContext.RequestAborted` directly inside services was rejected because it couples business logic to ASP.NET Core abstractions and breaks testability.

## Decision 2: Keep async API changes backward-compatible with optional tokens
- Decision: Add `CancellationToken cancellationToken = default` to async signatures in `IGraphRepository`, `GraphRepository`, `IGraphManagementService`, and `GraphManagementService`.
- Rationale: This preserves existing call sites until they are updated and keeps unit test setup incremental.
- Alternatives considered: Requiring non-optional tokens everywhere was rejected because it would force unrelated call sites and tests to change immediately without adding feature value.

## Decision 3: Use native EF Core async overloads for cancellation
- Decision: Every EF Core async call in `GraphRepository` will use the overload that accepts a cancellation token, including `ToListAsync`, `FirstOrDefaultAsync`, `SaveChangesAsync`, and `FindAsync` with key arrays.
- Rationale: EF Core already exposes cooperative cancellation points for database operations, which satisfies the feature without schema or infrastructure changes.
- Alternatives considered: Wrapping database calls in custom timeout logic was rejected because it adds complexity and does not align with request-driven cancellation semantics.

## Decision 4: Tie external notification delivery to the parent request lifetime
- Decision: `NotifySlackAsync` will accept and forward the same cancellation token used by the request pipeline, and notification dispatch for node creation will no longer behave as detached fire-and-forget work.
- Rationale: The clarified requirement explicitly prefers aborting outbound notifications when the originating request is cancelled. A detached task cannot reliably honor request cancellation once control returns to ASP.NET Core.
- Alternatives considered: Leaving the Slack notification detached and passing a token anyway was rejected because it creates race-prone, misleading cancellation behavior.

## Decision 5: Preserve business logic and validate behavior with focused tests plus manual request-abort checks
- Decision: The implementation will keep existing DTO mapping, validation, and repository orchestration intact while updating tests to verify token propagation and non-regression.
- Rationale: The feature is cross-cutting but narrow. The main risk is behavioral regression in graph mapping and relationship validation rather than data-model change.
- Alternatives considered: Large refactors such as introducing a notification abstraction or repository redesign were rejected because they are outside scope.

## Outstanding Clarifications
- None. The previous conflict around external notification cancellation was resolved in the spec clarification dated 2026-07-13.
