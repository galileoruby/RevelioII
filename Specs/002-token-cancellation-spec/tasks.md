# Tasks: Cooperative Cancellation Implementation for Graph Architecture

**Input**: Design documents from `/specs/002-token-cancellation-spec/`

**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/, quickstart.md

**Tests**: Include focused unit-test updates because the feature success criteria require existing tests to keep passing after async signature changes.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Phase 1: Setup (Shared Preparation)

**Purpose**: Confirm the request-boundary cancellation touchpoints before changing shared async contracts.

- [X] T001 Review request cancellation touchpoints in specs/002-token-cancellation-spec/plan.md, specs/002-token-cancellation-spec/contracts/cancellation-propagation.md, Pages/Index.cshtml.cs, Pages/Nodes/Create.cshtml.cs, and Pages/Relationships/Create.cshtml.cs

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Shared async contract changes that block all user story implementation.

**⚠️ CRITICAL**: No user story work can begin until this phase is complete.

- [X] T002 [P] Update async repository signatures to accept optional cancellation tokens in Repositories/IGraphRepository.cs
- [X] T003 [P] Update async service signatures to accept optional cancellation tokens in Services/IGraphManagementService.cs
- [X] T004 Update EF Core repository methods to forward cancellation tokens in Repositories/GraphRepository.cs
- [X] T005 Update service methods to accept and forward cancellation tokens while preserving mapping and validation logic in Services/GraphManagementService.cs

**Checkpoint**: Shared contracts and core implementations support cancellation token propagation.

---

## Phase 3: User Story 1 - Graceful Database Query Abort (Priority: P1) 🎯 MVP

**Goal**: Ensure graph reads and database-backed page operations stop promptly when the request is cancelled.

**Independent Test**: Abort a graph or CRUD request mid-flight and verify the request stops without breaking subsequent requests.

### Implementation for User Story 1

- [X] T006 [US1] Update the home-page graph data handler to accept and forward request cancellation in Pages/Index.cshtml.cs
- [X] T007 [P] [US1] Update node read and edit/delete handlers to accept and forward request cancellation in Pages/Nodes/Index.cshtml.cs, Pages/Nodes/Edit.cshtml.cs, and Pages/Nodes/Delete.cshtml.cs
- [X] T008 [P] [US1] Update relationship read and edit/delete handlers to accept and forward request cancellation in Pages/Relationships/Index.cshtml.cs, Pages/Relationships/Edit.cshtml.cs, and Pages/Relationships/Delete.cshtml.cs
- [X] T009 [US1] Update relationship creation flows to pass request cancellation through node lookups and persistence in Pages/Relationships/Create.cshtml.cs
- [X] T010 [US1] Expand graph, lookup, update, delete, and relationship validation coverage for token-aware service calls in RevelioII.UnitTests/Services/GraphManagementServiceTests.cs

**Checkpoint**: User Story 1 is independently functional and request cancellation reaches graph/database operations.

---

## Phase 4: User Story 2 - Cancellation of External Service Calls (Priority: P2)

**Goal**: Ensure node creation and outbound notification work are cancelled with the parent request.

**Independent Test**: Abort a node creation request while notification is in progress and verify the outbound HTTP call does not continue detached from the request.

### Implementation for User Story 2

- [X] T011 [US2] Update node creation request handling to pass request cancellation into graph service calls in Pages/Nodes/Create.cshtml.cs
- [X] T012 [US2] Align node creation and Slack notification flow with request-coupled cancellation in Services/GraphManagementService.cs
- [X] T013 [US2] Update the outbound Slack HTTP call to accept and forward cancellation in Services/GraphManagementService.cs
- [X] T014 [US2] Add notification-focused unit coverage for token-aware node creation and Slack dispatch in RevelioII.UnitTests/Services/GraphManagementServiceTests.cs

**Checkpoint**: User Story 2 is independently functional and outbound notification cancellation matches the clarified behavior.

---

## Phase 5: Polish & Cross-Cutting Concerns

**Purpose**: Final validation and documentation updates that affect both user stories.

- [X] T015 [P] Refresh aborted-request validation steps and expected outcomes in specs/002-token-cancellation-spec/quickstart.md
- [X] T016 Run regression validation from specs/002-token-cancellation-spec/quickstart.md against RevelioII.csproj and RevelioII.UnitTests/RevelioII.UnitTests.csproj

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies; can start immediately.
- **Foundational (Phase 2)**: Depends on Setup completion; blocks all user stories because interface and core implementation signatures must change first.
- **User Story 1 (Phase 3)**: Depends on Foundational completion; delivers the MVP cancellation path for graph and database operations.
- **User Story 2 (Phase 4)**: Depends on Foundational completion and is safest after User Story 1 because it reuses the same service surface and request token plumbing.
- **Polish (Phase 5)**: Depends on completion of the desired user stories.

### User Story Dependencies

- **User Story 1 (P1)**: No dependency on User Story 2; it is the MVP.
- **User Story 2 (P2)**: Reuses the cancellation-enabled service contracts from Foundational and the request-boundary approach established in User Story 1.

### Parallel Opportunities

- T002 and T003 can run in parallel because they touch different interface files.
- T007 and T008 can run in parallel after T005 because they update separate page groups.
- T015 can run in parallel with late implementation validation once the behavior is stable.

---

## Parallel Example: User Story 1

```text
T007 Update node read and edit/delete handlers in Pages/Nodes/Index.cshtml.cs, Pages/Nodes/Edit.cshtml.cs, and Pages/Nodes/Delete.cshtml.cs
T008 Update relationship read and edit/delete handlers in Pages/Relationships/Index.cshtml.cs, Pages/Relationships/Edit.cshtml.cs, and Pages/Relationships/Delete.cshtml.cs
```

## Parallel Example: User Story 2

```text
T011 Update node creation request handling in Pages/Nodes/Create.cshtml.cs
T014 Add notification-focused unit coverage in RevelioII.UnitTests/Services/GraphManagementServiceTests.cs
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup.
2. Complete Phase 2: Foundational.
3. Complete Phase 3: User Story 1.
4. Validate aborted graph/database requests before moving to notification work.

### Incremental Delivery

1. Finish shared contract and implementation changes in Phase 2.
2. Deliver User Story 1 as the MVP for request-bound database cancellation.
3. Deliver User Story 2 for outbound notification cancellation.
4. Finish with regression validation and quickstart updates.

### Parallel Team Strategy

1. One developer updates interface and core implementation files in Phase 2.
2. After T005, one developer can take node page handlers while another takes relationship page handlers.
3. Notification behavior and its tests can proceed once the shared service signatures are in place.

---

## Notes

- [P] tasks touch different files and do not depend on incomplete work in the same phase.
- [US1] and [US2] labels map each task directly to the user stories in spec.md.
- Each user story remains independently testable after its phase checkpoint.
- This feature intentionally avoids new infrastructure, schema changes, or background queue work.