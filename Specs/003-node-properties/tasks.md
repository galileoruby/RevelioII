# Tasks: 003-node-properties

**Input**: Design documents from `Specs/003-node-properties/`

**Prerequisites**: plan.md (required), spec.md (required for user stories)

**Tests**: Include targeted tests because this feature changes user-visible interaction and error-state behavior.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2)
- Every task includes an exact file path

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Prepare shared UI hooks and constants used by all story work.

- [X] T001 Add Node Properties section shell in Pages/Index.cshtml
- [X] T002 Add base Node Properties panel styles in wwwroot/css/site.css
- [X] T003 [P] Add feature constants for preview size and thresholds in wwwroot/js/graph-home.js

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core parsing/rendering/state infrastructure required before story-specific behavior.

**⚠️ CRITICAL**: No user story work begins until this phase is complete.

- [X] T004 Implement safe JSON parse helper for node properties in wwwroot/js/graph-home.js
- [X] T005 [P] Implement property payload normalization helper in wwwroot/js/graph-home.js
- [X] T006 [P] Implement reusable JSON value renderer primitives in wwwroot/js/graph-home.js
- [X] T007 Implement shared details-panel state for hovered and selected nodes in wwwroot/js/graph-home.js
- [X] T008 Implement panel reset and update entry points in wwwroot/js/graph-home.js
- [X] T009 Add baseline regression test for graph-view property passthrough in RevelioII.UnitTests/Services/GraphManagementServiceTests.cs

**Checkpoint**: Foundation ready - user stories can now be implemented.

---

## Phase 3: User Story 1 - Inspect Node Properties Quickly (Priority: P1) 🎯 MVP

**Goal**: Show readable node properties in relationship details from graph interaction.

**Independent Test**: Hover/select a node with valid nested JSON and verify readable hierarchical output appears in relationship details and stays visible until selecting another node.

### Tests for User Story 1

- [X] T010 [P] [US1] Add regression test ensuring graph nodes expose Properties consistently in RevelioII.UnitTests/Services/GraphManagementServiceTests.cs

### Implementation for User Story 1

- [X] T011 [P] [US1] Add Node Properties labeled block and placeholder states in Pages/Index.cshtml
- [X] T012 [P] [US1] Add typography and type-color tokens for JSON rendering in wwwroot/css/site.css
- [X] T013 [US1] Wire node hover event to details-panel update flow in wwwroot/js/graph-home.js
- [X] T014 [US1] Wire node selection event to persist details until another node is selected in wwwroot/js/graph-home.js
- [X] T015 [US1] Render curated subset by default with Show all toggle in wwwroot/js/graph-home.js
- [X] T016 [US1] Render nested objects and arrays with expand and collapse controls in wwwroot/js/graph-home.js
- [X] T017 [US1] Enforce node-only hover behavior with no relationship visual effects in wwwroot/js/graph-home.js

**Checkpoint**: User Story 1 is independently functional and testable.

---

## Phase 4: User Story 2 - Handle Empty or Invalid Properties Safely (Priority: P2)

**Goal**: Provide robust non-breaking UX for empty, null, invalid, and large payloads.

**Independent Test**: Hover/select nodes with empty/null/invalid/large properties and verify expected empty/error/collapsed states while the UI remains responsive.

### Tests for User Story 2

- [X] T018 [P] [US2] Add service-level edge-case test coverage for empty or null property values in RevelioII.UnitTests/Services/GraphManagementServiceTests.cs

### Implementation for User Story 2

- [X] T019 [US2] Implement explicit empty-state rendering for null or empty properties in wwwroot/js/graph-home.js
- [X] T020 [US2] Implement invalid-JSON fallback state without breaking panel updates in wwwroot/js/graph-home.js
- [X] T021 [US2] Implement large-payload detection rule greater than 2 KB or more than 20 keys in wwwroot/js/graph-home.js
- [X] T022 [US2] Implement default-collapsed summary behavior for large payloads in wwwroot/js/graph-home.js
- [X] T023 [US2] Implement mobile tap and selection equivalence for property inspection in wwwroot/js/graph-home.js
- [X] T024 [P] [US2] Add empty, error, collapsed, and expanded state styles in wwwroot/css/site.css
- [X] T025 [US2] Add accessibility labels and keyboard support hooks for expand and collapse controls in Pages/Index.cshtml

**Checkpoint**: User Stories 1 and 2 both work independently.

---

## Phase 5: Polish & Cross-Cutting Concerns

**Purpose**: Final quality pass across stories without adding new scope.

- [X] T026 [P] Update manual validation checklist entries for this feature in Specs/003-node-properties/checklists/requirements.md
- [X] T027 Run full regression suite and resolve failing assertions in RevelioII.UnitTests/Services/GraphManagementServiceTests.cs
- [X] T028 [P] Refine Node Properties panel responsive behavior for desktop and mobile in wwwroot/css/site.css
- [X] T029 Verify node-details integration does not alter unrelated graph behaviors in wwwroot/js/graph-home.js

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: Start immediately.
- **Foundational (Phase 2)**: Depends on Phase 1 and blocks all user stories.
- **User Story 1 (Phase 3)**: Depends on Phase 2 completion.
- **User Story 2 (Phase 4)**: Depends on Phase 2 completion and can proceed after US1 baseline behavior is in place.
- **Polish (Phase 5)**: Depends on completion of both user story phases.

### User Story Dependencies

- **US1 (P1)**: No dependency on other stories after foundational tasks.
- **US2 (P2)**: Depends on foundational tasks and reuses US1 panel/render pipeline.

### Within Each User Story

- Write tests first where listed.
- Implement markup and styles before final interaction wiring where applicable.
- Complete rendering behavior before polish and regression cleanup.

### Parallel Opportunities

- **Setup**: T003 can run in parallel with T001 and T002.
- **Foundational**: T005 and T006 can run in parallel after T004 starts.
- **US1**: T011 and T012 can run in parallel, then T013 to T017 sequence.
- **US2**: T024 can run in parallel with T019 to T023.
- **Polish**: T026 and T028 can run in parallel with T029.

---

## Parallel Example: User Story 1

```bash
# Parallel UI scaffold work
Task: "T011 [US1] Add Node Properties labeled block and placeholder states in Pages/Index.cshtml"
Task: "T012 [US1] Add typography and type-color tokens for JSON rendering in wwwroot/css/site.css"

# Then complete interaction pipeline
Task: "T013 [US1] Wire node hover event to details-panel update flow in wwwroot/js/graph-home.js"
Task: "T014 [US1] Wire node selection event to persist details until another node is selected in wwwroot/js/graph-home.js"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1 and Phase 2.
2. Complete Phase 3 (US1).
3. Validate US1 independently before moving on.

### Incremental Delivery

1. Deliver US1 for core node property inspection.
2. Deliver US2 for resilience and edge-case UX.
3. Run polish and regression checks.

### Parallel Team Strategy

1. One developer handles markup and style tasks.
2. One developer handles JS parser and rendering pipeline tasks.
3. Merge at phase checkpoints and validate before advancing.

---

## Notes

- All tasks use the required checklist format and include file paths.
- [P] tasks touch separate files or independent units of work.
- Tasks are intentionally scoped to avoid implementation drift outside `003-node-properties`.
