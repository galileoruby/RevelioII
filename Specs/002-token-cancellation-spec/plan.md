# Implementation Plan: Cooperative Cancellation Implementation for Graph Architecture

**Branch**: `002-token-cancellation-spec` | **Date**: 2026-07-13 | **Spec**: `specs/002-token-cancellation-spec/spec.md`

**Input**: Feature specification from `/specs/002-token-cancellation-spec/spec.md`

## Summary

Propagate request cancellation from Razor Page handlers through `IGraphManagementService` and `IGraphRepository` into EF Core and outbound Slack notification calls, while preserving existing graph mapping, validation, and CRUD behavior. The implementation will be delivered in user-story order: first the graph/data-access cancellation path, then the external notification cancellation path, followed by regression-focused validation.

## Technical Context

**Language/Version**: C# on .NET 9 (`net9.0`)

**Primary Dependencies**: ASP.NET Core Razor Pages, Entity Framework Core Sqlite 9.0.15, Microsoft.Extensions.DependencyInjection, xUnit, Moq

**Storage**: SQLite via `AppDbContext`

**Testing**: `dotnet test` with xUnit and Moq in `RevelioII.UnitTests`

**Target Platform**: ASP.NET Core web application on Windows or Linux

**Project Type**: Single-project Razor Pages web application with a separate unit test project

**Performance Goals**: Abandoned browser requests should stop in-flight database and outbound HTTP work promptly instead of running to completion

**Constraints**: No new packages, no database schema changes, preserve DTO projection and validation logic, keep async signatures backward-compatible with optional tokens where possible

**Scale/Scope**: Touches request handlers in `Pages/`, service and repository abstractions/implementations, and unit tests; no frontend asset or persistence model redesign

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- Pass: The work stays within the existing single-app Razor Pages architecture.
- Pass: Page models remain thin by limiting changes to cancellation-token forwarding.
- Pass: Business rules remain in services and data access remains in repositories.
- Pass: Existing DTO shaping remains unchanged.
- Pass: Validation focuses on service behavior and request-driven cancellation without adding unnecessary abstractions.

**Post-Design Re-check**:
- Pass: Research and design artifacts keep the scope narrow to async signature propagation, request-boundary forwarding, and focused regression validation.
- Pass: No constitution violations or complexity justifications are required.

## Project Structure

### Documentation (this feature)

```text
specs/002-token-cancellation-spec/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── cancellation-propagation.md
└── tasks.md
```

### Source Code (repository root)

```text
Data/
├── AppDbContext.cs

DTOs/
├── GraphEdgeDto.cs
├── GraphNodeDto.cs
└── GraphViewDto.cs

Pages/
├── Index.cshtml.cs
├── Nodes/
│   ├── Create.cshtml.cs
│   ├── Edit.cshtml.cs
│   ├── Delete.cshtml.cs
│   └── Index.cshtml.cs
└── Relationships/
    ├── Create.cshtml.cs
    ├── Edit.cshtml.cs
    ├── Delete.cshtml.cs
    └── Index.cshtml.cs

Repositories/
├── IGraphRepository.cs
└── GraphRepository.cs

Services/
├── IGraphManagementService.cs
└── GraphManagementService.cs

RevelioII.UnitTests/
└── Services/
    └── GraphManagementServiceTests.cs
```

**Structure Decision**: Keep the existing single-project web application structure. The feature is cross-cutting across request handlers, service orchestration, repository data access, and unit tests, so no new project or architectural split is needed.

## Phase Plan By User Story

### User Story 1: Graceful Database Query Abort (P1)
- Update request handlers that await graph operations so they accept and pass request cancellation tokens.
- Add optional `CancellationToken` parameters to repository and service interfaces and implementations.
- Forward tokens to all EF Core async operations used by node, relationship, and graph queries.
- Preserve graph DTO projection and relationship validation logic while making awaited calls cancellable.
- Validate with unit tests and a manual aborted-request scenario against graph-loading and CRUD flows.

### User Story 2: Cancellation of External Service Calls (P2)
- Update `NotifySlackAsync` to accept and forward a cancellation token.
- Align node-creation notification flow with the clarified request-coupled cancellation requirement.
- Ensure outbound notification cancellation does not introduce unrelated logic changes beyond the clarified execution model.
- Validate with a manual aborted-request scenario against node creation and outbound HTTP dispatch.

### Cross-Cutting Validation
- Update unit tests and mocks to tolerate or verify token-bearing async signatures.
- Re-run build and unit tests after the implementation.
- Confirm non-cancelled graph and CRUD flows still behave the same.

## Complexity Tracking

No constitution violations or exceptional complexity are anticipated for this feature.
