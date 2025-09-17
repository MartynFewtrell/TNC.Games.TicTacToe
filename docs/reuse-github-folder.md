# Reusing the `.github` Folder

The `.github` folder provides reusable prompt instructions and operational guidance for generating specs, plans, and executing refactors. New delivery repositories should **include a snapshot** of the relevant parts of this folder (not a symlink) so they can evolve independently.

## What to Include
- `/.github/prompts/*.prompt.md` – Prompt definitions (research / plan / execute phases) per family (spec, refactor, etc.)
- `/.github/copilot-instructions.md` – Central agent operating guidelines
- (Optional when added) workflow / PR templates under `/.github/`

## Integration Steps (Conceptual)
1. Create a `.github` folder in the new repository if it does not exist.
2. Add only the prompt families you intend to use initially (you can add more later).
3. Add `copilot-instructions.md` to align local agent behaviour.
4. Confirm organisation / team contact details are correct.
5. You may need to update `copilot-instructions.md` to reflect your technology stack and ways of working.
6. (If you later introduce GitHub Actions) ensure any workflows are security reviewed before enabling.

## Using Prompt Files
- Choose correct family & phase: e.g. `spec.research.prompt.md` for requirement discovery; `refactor.plan.prompt.md` for planning debt reduction.
- Provide only essential contextual summaries or file excerpts to the assistant—avoid dumping entire repositories unnecessarily.
- Store generated specs / plans in your project `docs/` folder; **do not edit the original prompt definitions except for local policy adjustments**.

## Maintenance Tips
- Periodically remove or archive (e.g. move to `obsolete/`) unused prompts.
- Consolidate overlapping or redundant instructions to reduce cognitive load.
- Ensure examples stay neutral (no secrets, internal endpoints).

*Last updated: 2025-09-12*