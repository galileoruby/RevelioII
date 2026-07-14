---
name: 003-node-properties-prompt
description: "Use when defining the node hover properties feature for the graph UI, including fancy JSON display for node properties in relationship details."
---

# 003 Node Properties

Create a requirement or implementation-ready feature spec for this user story:

As a final user, I want to see more details when I mouse over a node.
I want information about node properties.
Every node has node properties.
I want to display that JSON information in a fancy way inside the relationship details view.
Node properties are stored as straightforward JSON.

## Context

Use these project rules while generating the requirement:
- Respect the graph UI rule from the repo instructions: do not introduce white lines, incomplete rings, or relationship hover visual effects on node hover.
- Keep existing behavior unless the requirement explicitly changes it.
- If backend or service changes are needed, preserve cancellation-token propagation and existing architecture boundaries.
- If the feature changes behavior meaningfully, include required test coverage.

## Expected Output

Write the requirement in clear product language with these sections:

1. User Story
2. Problem Statement
3. Functional Requirements
4. Non-Functional Requirements
5. UI/UX Notes
6. Acceptance Criteria
7. Technical Notes
8. Testing Notes

## Specific Guidance

Make sure the generated requirement clarifies all of the following:
- What exact hover trigger shows the extra information.
- Whether the information appears in a tooltip, popover, side panel, or relationship details panel.
- How node properties JSON should be formatted for readability.
- How nested JSON objects and arrays should be rendered.
- How empty, null, or invalid JSON should be handled.
- Whether all properties are shown or only a curated subset.
- How the UI behaves on desktop and mobile.
- How large JSON payloads should be truncated, collapsed, or expanded.
- How this integrates with existing relationship details without making the graph noisy.

## Output Constraints

- Prefer concise, implementation-ready requirements.
- Include acceptance criteria that are testable.
- Call out ambiguous areas explicitly as open questions instead of guessing.
- If proposing UI behavior, keep the hover effect subtle and node-only.