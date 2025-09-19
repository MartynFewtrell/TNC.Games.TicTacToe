# Implementation Plan

Plan: UI Modernisation (Navigation, Theming, Layout, Dark Mode, Board Styling, Accessibility)
Version: v1.02  
Status: In Progress  
Date: 2025-09-19  
Supersedes: plan-ui-modernisation_v1.01.md  
Based On Specs:  
- spec-ui-overview_v1.01.md  
- spec-ui-navigation_v1.01.md  
- spec-ui-theme_v1.01.md  
- spec-ui-layout_v1.01.md  
- spec-ui-board_v1.01.md  
- spec-ui-routes_v1.01.md  
- spec-ui-accessibility_v1.01.md  
- spec-ui-functional_v1.0.md

## Baseline (From v1.01)
- Top navigation with branding + How To Play page delivered.
- Semantic light theme tokens implemented (`theme.css`).
- Dark mode toggle & persistence (localStorage + early boot script) implemented (delivered earlier than sequence).
- Board modernised: empty cells blank, indices retained, win highlight uses semantic token, legacy yellow highlight removed.
- Form control theming unified across pages.

## Delta (Since v1.01)
- Removed New Game button from top nav (scope adjustment).
- Added dark mode ahead of plan (Work Item 3 pulled forward).
- Updated board to use `--color-win-highlight` token (instead of raw accent) & removed residual highlight artifacts.
- Introduced global unified input/select/textarea styling.

## Carry-Over / Outstanding
- Mobile hamburger drawer + off?canvas navigation (Work Item 5).
- Centered container `max-width:1280px` verification / consolidation.
- Win highlight animation (optional) & confirm focus-visible styling contrast.
- Accessibility hardening & legacy cleanup (Work Item 7) including deletion of `NavMenu.razor`.
- Stub content polish for Training / Admin / How To Play (Work Item 6).

## Updated Work Item Overview
1. (Done) Top Navigation & How To Play.
2. (Done) Semantic Light Theme Tokens.
3. (Done) Dark Mode Toggle & Persistence.
4. (In Progress) Board Styling Modernisation – remaining: optional animation, breakpoint validation, focus-outline contrast confirmation.
5. (Pending) Mobile Hamburger Drawer + Centered Container.
6. (Re-scoped) Content copy polish only (New Game nav action dropped).
7. (Pending) Accessibility Hardening & Cleanup.

---
## Work Item 4 (Remaining Tasks)
- [ ] Add subtle win highlight animation (fade or scale) respecting `prefers-reduced-motion`.
- [ ] Validate focus-visible outline contrast in dark & light themes.
- [ ] Test extreme narrow viewport (<340px) scaling.

## Work Item 5
- [ ] Add hamburger button (<768px) with `aria-label`, `aria-expanded`, `aria-controls`.
- [ ] Off-canvas drawer with overlay; close on ESC, overlay click.
- [ ] Simple focus trap (cycle first/last) & return focus to trigger.
- [ ] Move nav links + theme toggle into drawer for mobile.
- [ ] Ensure layout wrapper: `max-width:1280px; margin:0 auto; padding`.

## Work Item 6 (Re-scoped)
- [ ] How To Play: bullet list of rules + brief strategy tip.
- [ ] Training: descriptive placeholder of forthcoming features.
- [ ] Admin: outline of planned administrative/management capabilities.

## Work Item 7
- [ ] Remove `NavMenu.razor` (+ CSS) & any unused sidebar styles.
- [ ] Keyboard audit (tab order, ESC from drawer, skip navigation optional?).
- [ ] Confirm `aria-current="page"` styling & visibility.
- [ ] Contrast audit (tokens meet WCAG AA for text & interactive components) adjust if needed.
- [ ] Document final token palette in specs (append change log entries).

## Risks (Updated)
| Risk | Impact | Mitigation |
|------|--------|------------|
| Drawer focus trap errors | Confusing keyboard UX | Keep logic minimal, manual tab-cycle test |
| Motion discomfort from animation | Accessibility issue | Guard with `prefers-reduced-motion: reduce` |
| Residual unused CSS | Maintenance overhead | Cleanup pass in Work Item 7 |

## Verification Matrix (Updated)
| Work Item | Increment | Status |
|-----------|-----------|--------|
| 1 | Top nav + How To Play | Done |
| 2 | Light theme tokens | Done |
| 3 | Dark mode toggle | Done |
| 4 | Board highlight + responsive polish | In Progress |
| 5 | Mobile drawer + centered layout | Pending |
| 6 | Polished stub copy | Pending |
| 7 | Accessibility & cleanup | Pending |

## Summary
Core navigation, theming (light/dark), and foundational board UX are complete. Remaining effort focuses on mobile navigation responsiveness, content polish, and accessibility & cleanup. Scope refined to drop redundant New Game nav action, simplifying the interaction model. Completion of Work Items 5–7 will finalize UI modernisation for v1.x and prepare ground for deferred features (statistics, auth, settings).

## Final Semantic Palette (v1.02 Closure)
Light Theme:
- Background: #ffffff (gray-50)
- Surface: #f1f5f9 (gray-100)
- Surface Alt: #e2e8f0 (gray-150)
- Border: #c3ced9 (gray-300)
- Text Primary: #0f1724 (gray-900)
- Text Muted: #475569 (gray-600)
- Accent Primary: #1d4ed8 (blue-600)
- Accent Hover: #3b82f6 (blue-500)
- Accent Active: #1e40af (blue-650)
- Focus Ring: #3b82f6 (blue-500)
- Win Highlight: #93c5fd (blue-300)

Dark Theme:
- Background: #0f1724
- Surface: #111827
- Surface Alt: #1e293b
- Border: #273449
- Text Primary: #e6eef8
- Text Muted: #9aa6b2
- Accent Primary: #60a5fa
- Accent Hover: #93c5fd
- Accent Active: #3b82f6
- Focus Ring: #93c5fd
- Win Highlight: #1e3a8a

Rationale:
- Elevated border contrast (gray-300 / #273449) for clearer separation on light/dark.
- Win highlight kept lighter than accent in light theme for differentiation; inverse in dark theme (darker tone plus glow animation).
- Focus ring reuses accent mid value ensuring WCAG contrast on both backgrounds.

Accessibility Notes:
- Focus indicators: 3px outline on board cells, 2px elsewhere.
- Skip link added; keyboard loop for mobile drawer; ESC closes drawer.
- All interactive elements maintain >= 3:1 contrast for non-text UI and >= 4.5:1 for body text.

Status: Plan execution complete (UI modernisation slice). Further enhancements (statistics page, auth, settings, advanced animations) deferred.

---
**Plan v1.02 Closed**
