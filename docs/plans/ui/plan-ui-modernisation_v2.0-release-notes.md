# Release Notes: UI Modernisation v2.0
Version: v2.0  
Date: 2025-09-19  
Status: Released  
Based On: spec-ui-overview_v2.0.md (captures final state)  

## Overview
The v2.0 release delivers a modernised Blazor UI with semantic theming, dark mode, responsive navigation, accessible gameplay board, and improved administrative & training surfaces.

## Key Enhancements
- Navigation: Replaced legacy sidebar with top bar + mobile hamburger drawer.
- Theming: Introduced light/dark semantic token system with persistence and system preference detection.
- Board: Responsive grid, token-based colors, animated win highlight (reduced-motion aware), improved focus outline.
- Accessibility: Skip link, focus-visible outlines, keyboard drawer operation, contrast-focused palette.
- Content Polish: Enhanced How To Play, Training, and Admin pages with clearer instructional text.
- Cleanup: Removed deprecated sidebar components & unused CSS.

## Palette (Semantic Summary)
Refer to `wwwroot/css/theme.css` for the definitive definitions.

## Breaking Changes
- Legacy `NavMenu.razor` removed; any external overrides depending on it must update to `TopNav` / drawer structure.
- Removed New Game nav button concept (simplified interaction model).

## Known Limitations / Deferred Items
| Area | Deferred Item |
|------|---------------|
| Analytics | Statistics / insights page |
| Auth | Expanded authentication/authorization model |
| Personalisation | Settings panel beyond theme toggle |
| Accessibility | High contrast / alternate color theme |
| UI Motion | Additional subtle animations (drawer morph, theme transitions) |

## Upgrade Guidance
- If injecting additional links, update both desktop `.links` container and `MobileMenuDrawer` links list.
- When adding new UI colors, extend the semantic token set rather than hard-coding values.

## Validation
- Build & test pass (`dotnet build`, `dotnet test`).
- Manual keyboard audit complete (Tab / Shift+Tab / ESC on drawer).
- Dark & light mode smoke tested.

## Next Steps (Candidate Backlog for v2.x)
1. Statistics page (game outcomes, opening efficiency).
2. Enhanced admin dashboard (bulk operations UI, filtering, search).
3. High contrast theme variant & contrast automated tests.
4. Settings panel (persisted preferences beyond theme).
5. Optional animations with reduced-motion safeguards (theme fade, drawer icon morph).

## Acknowledgements
Generated using GitHub Copilot across code and documentation.

---
Release v2.0 finalized.
