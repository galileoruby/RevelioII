# Specify

This document outlines the workflow and templates for Spec-Driven Development (SDD) in this project.

## Workflow

1. **Draft a Spec**: Create a new markdown file in the `Specs/` directory (e.g., `Specs/001-user-login.md`).
2. **Review & Refine**: Discuss the spec to ensure edge cases are handled before coding.
3. **Approve**: Mark the spec as `Approved`.
4. **Implement**: Write code to fulfill the specification.
5. **Verify**: Ensure the implementation passes the spec's requirements.

## Spec Template

Copy this template into a new file for each feature:

```markdown
# Spec: [Feature Name]

**Status:** [Draft | Approved | Implemented | Obsolete]
**Author:** [Author Name]
**Date:** [YYYY-MM-DD]

## 1. Overview
A brief description of the feature.

## 2. Requirements (User Stories)
- As a [role], I want to [action] so that [benefit].

## 3. Acceptance Criteria
- [ ] Condition 1
- [ ] Condition 2

## 4. Technical Constraints / Architecture
- Dependencies, APIs, schemas, etc.
```
