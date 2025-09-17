# Copying & Using the `docs` Folder Templates

The `docs` folder supplies **starter templates** (e.g. specification templates) and guidance to bootstrap documentation in a new project. New repositories should **take a snapshot** of the required template files so they can diverge without impacting the originals.

## What to Bring Across
- `docs/specs/templates/` (latest spec template versions)
- Any additional guidance documents you need (avoid copying unused ones to reduce noise)
- Future (if introduced): plan templates, ADR template, glossary template

## After Inclusion
- Create only necessary subfolders (`docs/specs`, `docs/plans`, etc.).
- Duplicate template files when starting a live spec; never edit templates in place.
- Start drafts at `v0.01`; first stable release at `v1.0`.
- Reference exact spec versions inside related plan documents.

## Using a Spec Template
1. Identify the latest template (e.g. `spec-template_v1.1.md`).
2. Duplicate it into `docs/specs/` as `spec-<scope>-<type>_v0.01.md`.
3. Fill known sections; mark unknowns with `TBD`.
4. Promote to `v1.0` only when content is stable (remove `TBD`).
5. For changes post release, create a new version file (do not overwrite).

## Change Logs
When moving from draft to released versions, include a concise change log section capturing key deltas between versions.

## Adopting Upstream Improvements
- Periodically review upstream template updates.
- If adopting, create a new local version incorporating changes (never edit historical versions).

## Hygiene
- Mark superseded drafts clearly or move them to an archive folder.
- Keep examples generic: no real endpoints, credentials, internal IDs.

*Last updated: 2025-09-12*