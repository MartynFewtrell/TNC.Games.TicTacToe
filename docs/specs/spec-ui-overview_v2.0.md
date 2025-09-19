# UI Overview Specification
Version: v2.0  
Status: Released  
Supersedes: (Earlier v1.x specs referenced in legacy plan; original spec files not retained in this repository snapshot)  
Date: 2025-09-19

## Scope
This specification captures the state of the Blazor Web UI as of release v2.0 after the v1.x modernisation initiative (navigation, theming, dark mode, board styling, accessibility).

## Summary of UI Capabilities
| Area | Capability |
|------|------------|
| Navigation | Top horizontal bar with brand, primary route links (Play, How to Play, Training, Admin), theme toggle, responsive hamburger + off?canvas drawer |
| Theming | Semantic CSS variable token layer (light + dark) with persistence (localStorage) & system preference bootstrap |
| Game Board | Responsive 3×3 grid with semantic colors, win highlight animation (reduced-motion aware), accessible focus outline |
| Content Pages | How To Play (rules + strategy tips), Training (self?play controls & placeholders), Admin (Q-table management) |
| Accessibility | Skip link, focus-visible outlines, aria attributes on nav/drawer, keyboard ESC close, focus return, color contrast tokens |
| Forms | Unified styling for inputs/selects/textareas across themes |

## Non-Goals (Deferred)
- Statistics/analytics page
- Auth beyond basic admin credentials configuration
- Settings / preferences panel beyond theme
- High contrast or additional color themes
- Advanced animations (beyond win highlight + drawer slide)

## Theming Tokens
Light & Dark palette documented in `plan-ui-modernisation_v1.02.md` closure section (retained as authoritative). See `wwwroot/css/theme.css` for source of truth.

## Routes
| Route | Component | Notes |
|-------|-----------|-------|
| / | Index (Play) | Main gameplay board & controls |
| /how-to-play | HowToPlay | Rules & tips |
| /training | Training | Self-play job launcher (API integration) |
| /admin | Admin | Ranking/stat management (basic auth) |

## Accessibility Checklist (v2.0 Baseline)
- All interactive elements keyboard reachable.
- Focus indicators visible (>= 2px, high contrast).
- Drawer: ESC closes, focus trapped and returned.
- Skip link to main content.
- Color contrast meets WCAG AA for text & UI surfaces (manual spot check performed).

## Architectural Notes
- No client-side state management framework; simple component state + ThemeService.
- ThemeService uses JS interop only for early attribute set & persistence.
- BoardGrid avoids native disabled styling to maintain consistent cell visuals.

## Risks / Follow-Up
| Risk | Mitigation Path |
|------|-----------------|
| Palette expansion needed for future components | Introduce extended semantic set (e.g., success/warn/error) in a future minor release |
| Increased navigation actions | Convert action group into dedicated right-aligned button cluster / dropdown |
| Drawer scaling on very long link lists | Introduce internal scroll & focus sentinel elements |

## Acceptance
This v2.0 spec reflects implemented UI; any new features will target new spec versions (e.g., `spec-ui-overview_v2.1.md`) and associated plans.
