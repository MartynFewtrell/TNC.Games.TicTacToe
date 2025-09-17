# Documentation & Prompt Assets

This folder contains reusable documentation **templates**, **prompt files (referenced from the root .github folder)** and supporting guidance intended to bootstrap a new delivery repository.

> Do **not** place live / project-specific secrets or proprietary environment details here.

## Topics
- [Managed Capabilities Protocol (.mcp.json) Setup](mcp-setup.md)
- [Reusing the .github Folder in a New Project](reuse-github-folder.md)
- [Copying and Using the docs Folder Templates](reuse-docs-folder.md)

## Additional Guidance
- [Workflow Summary](#workflow-summary)
- [Governance & Hygiene](#governance--hygiene)
- [Support / Contact](#support--contact)

---
## Workflow Summary
| Step | Action | Output |
|------|--------|--------|
| 1 | Copy `.github` folder | Project prompt infrastructure |
| 2 | Copy `docs` folder | Local template set |
| 3 | Create initial draft spec(s) | `spec-<scope>-<type>_v0.01.md` |
| 4 | Run spec research prompt | Clarified requirements Q&A transcript |
| 5 | Generate implementation/refactor plan | `plan-<area>-<purpose>_v0.01.md` |
| 6 | Execute plan using execute prompt | Updated plan with completed checkboxes |
| 7 | Version & freeze spec (v1.0) | Baseline requirements |

---
## Governance & Hygiene
- Keep templates immutable once released (new version instead of overwrite)
- Prune unused prompts quarterly
- Re-evaluate prompts for accuracy vs current engineering standards
- Ensure examples never include secrets or environment identifiers

---
## Support / Contact
For clarification or improvement proposals email: **AbzuDeliveryTeam@UKHO.gov.uk**.

If you add internal-only MCP servers, document them in `mcp-extensions.md` (create if absent).

---
*Last updated: 2025-09-12*