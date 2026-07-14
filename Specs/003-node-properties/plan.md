# Implementation Plan: Node Properties Detail on Hover

**Branch**: `003-node-properties` | **Date**: 2026-07-14 | **Spec**: `Specs/003-node-properties/spec.md`

**Input**: Feature specification from `Specs/003-node-properties/spec.md`

## Summary

Add a node-focused details experience that presents node properties JSON in the existing relationship details panel, with readable hierarchical formatting, safe handling for empty/invalid payloads, and stable persistence until another node is selected. The implementation is sequenced by user-story priority: first core rendering and interaction flow, then resilience and edge-state handling, followed by regression-focused validation.

## Technical Context

**Language/Version**: C# on .NET 9 (`net9.0`) plus existing frontend assets in JavaScript/CSS

**Primary Dependencies**: ASP.NET Core Razor Pages, Entity Framework Core Sqlite, existing graph UI stack in `wwwroot/js/graph-home.js`

**Storage**: SQLite via `AppDbContext`

**Testing**: `dotnet test` for backend/service regressions plus manual UI validation for graph interactions

**Target Platform**: ASP.NET Core web application on desktop and mobile browsers

**Project Type**: Single-project Razor Pages web app with separate unit test project

**Performance Goals**: Hover/selection-driven detail updates should feel immediate and avoid noticeable lag during graph exploration

**Constraints**: Preserve existing behavior unless required by spec; maintain node-only hover effects (no relationship hover artifacts), no unnecessary schema changes, keep architecture boundaries intact

**Scale/Scope**: Primarily UI behavior in graph interaction and relationship-details rendering; optional minimal backend touch only if property payload mapping requires adjustment

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- Pass: Scope remains within current architecture boundaries (`Pages` → `Services` → `Repositories`).
- Pass: Feature emphasizes user value and readability without introducing unnecessary abstractions.
- Pass: Existing behavior preservation is explicit in scope and acceptance constraints.
- Pass: UI constraint from project instructions (no white line/incomplete ring/relationship hover effects) is preserved.

**Post-Design Re-check**:
- Pass: Design remains incremental and bounded to node-property detail presentation.
- Pass: No constitution violations or exceptional complexity are expected.

## Project Structure

### Documentation (this feature)

```text
Specs/003-node-properties/
├── spec.md
├── plan.md
└── checklists/
    └── requirements.md
```

### Source Code (repository root)

```text
Pages/
└── Index.cshtml
└── Index.cshtml.cs

DTOs/
├── GraphNodeDto.cs
└── GraphViewDto.cs

Services/
├── IGraphManagementService.cs
└── GraphManagementService.cs

Repositories/
├── IGraphRepository.cs
└── GraphRepository.cs

wwwroot/
├── js/
│   └── graph-home.js
└── css/
    └── site.css

RevelioII.UnitTests/
└── Services/
    └── GraphManagementServiceTests.cs
```

**Structure Decision**: Keep the existing single-project structure. Most changes should remain in graph-page UI assets and existing page/service flows without introducing new projects or major reshaping.

## Phase Plan By User Story

### User Story 1: Inspect Node Properties Quickly (P1)
- Define node interaction contract for desktop hover and mobile tap/selection equivalence.
- Add/update relationship-details section to host `Node Properties` in a clearly labeled block.
- Implement readable JSON presentation with type-aware styling and hierarchical expand/collapse support.
- Implement persistence rule: details remain visible until a different node is selected.
- Implement default visibility rule: curated subset first, with `Show all` to reveal full payload.
- Validate that interaction remains node-only and does not trigger relationship hover visuals.

### User Story 2: Handle Empty or Invalid Properties Safely (P2)
- Define and implement empty-state presentation for null/empty payloads.
- Define and implement non-breaking invalid-JSON state with clear user feedback.
- Implement large-payload behavior using defined threshold (`>2 KB` or `>20` top-level keys) with default collapsed summary and user-driven expansion.
- Validate nested object/array rendering readability and resilience with representative datasets.

### Cross-Cutting Validation
- Run backend and regression tests with `dotnet test` to ensure non-related behavior is preserved.
- Perform manual graph UI verification on desktop and mobile viewport behavior.
- Verify prohibited hover artifacts are absent (no white line, no incomplete ring, no relationship hover effect).
- Confirm relationship details remain stable and readable under normal and edge payload conditions.

## Risk & Mitigation

- **Risk**: Large JSON payloads can degrade UI responsiveness.
  - **Mitigation**: Default collapsed summary with progressive expansion and minimal initial render.

- **Risk**: Ambiguity in curated-subset ordering can lead to inconsistent user expectations.
  - **Mitigation**: Define deterministic subset selection rule during implementation design (for example stable key ordering and fixed preview size).

- **Risk**: Hover-based interaction may be fragile on mobile.
  - **Mitigation**: Use selection/tap equivalence and explicitly validate behavior in mobile viewport tests.

## Complexity Tracking

No constitution violations or exceptional complexity are anticipated for this feature. This is primarily a scoped UX/data-presentation enhancement with constrained interaction rules.
