# Specification: UI Modernisation Overview
Version: v1.01  
Status: Draft  
Date: 2025-09-19  
Supersedes: spec-ui-overview_v0.01.md  
Authors: UI/UX Working Draft  
Related Branch: feature-update-appearance  
Related Specs:  
- spec-ui-navigation_v1.01.md  
- spec-ui-theme_v1.01.md  
- spec-ui-layout_v1.01.md  
- spec-ui-board_v1.01.md  
- spec-ui-routes_v1.01.md  
- spec-ui-accessibility_v1.01.md

## Change Log
| Version | Date | Summary |
|---------|------|---------|
| v1.01 | 2025-09-19 | Promote Admin link to active (no disabled state); ensure all nav destinations have working pages (create stubs where missing); align accessibility spec removing disabled Admin semantics. |
| v0.01 | 2025-09-19 | Initial draft baseline. |

---

## 1. Purpose & Scope

### 1.1 Purpose
Modernize and professionalize the Blazor Tic Tac Toe web UI by improving layout, navigation, theming, accessibility foundations, and visual clarity while preserving existing game logic.

### 1.2 In-Scope (Updated)
- Rename application to “Tic Tac Toe” (title/branding only; favicon unchanged)
- Replace vertical left navigation with a top horizontal bar + mobile hamburger drawer
- Centralize page content with a max width of 1280px
- Introduce a cool neutral + blue accent theme (light + dark modes) via CSS variables
- Implement full dark mode support (toggle + persistence via localStorage; honours `prefers-color-scheme`)
- Add / ensure pages exist for: Play (root), How to Play, Training, Admin (stubs created if absent)
- Admin link now fully navigable to existing Admin page (no disabled state)
- Update game board styling (semantic tokens; accent-blue win highlight)
- Consistent component styling (4px corner radius; essential focus only, no decorative animation)
- Mobile off?canvas drawer for navigation below medium breakpoint

### 1.3 Out of Scope (Unchanged Unless Noted)
- Authentication / authorization enforcement (Admin page content remains placeholder)
- Training content depth, Admin functionality, Statistics, Settings, Leaderboards
- Favicon / manifest redesign
- Game engine or backend API changes
- Advanced animation or motion design
- PWA enhancements & offline support
- Internationalization / localization

### 1.4 Objectives
- Improve clarity and professionalism of UI
- Establish a semantic theming layer (extensible for future pages)
- Enhance accessibility baseline (contrast, focus states, keyboard navigation)
- Ensure all navigation links function (no dead or disabled links)

### 1.5 Success Criteria (Qualitative)
- Navigation intuitive on desktop and mobile
- Theme toggle works reliably across reloads with stored preference
- All interactive elements keyboard reachable with visible focus states
- All nav links route to functioning pages (stub or real)
- Color system consistent across light/dark with adequate contrast (WCAG AA)

---

## 2. System Overview

The UI layer is a Blazor application acting as the presentation shell for the Tic Tac Toe game board and ancillary informational/stub pages. Enhancements introduce a structured layout, modular theming, and complete, non-broken navigation while preserving domain/game logic.

### 2.1 High-Level Structure
- Layout Shell: Provides top navigation, centered content region, theme root
- Theming Layer: CSS custom properties (light & dark palettes, semantic tokens)
- Navigation System: Horizontal bar (desktop) + hamburger-triggered drawer (mobile)
- Game Board Presentation: Existing component re-styled via tokens only
- Pages: Play (root), How to Play (stub), Training (stub), Admin (stub)
- Client Theme Service: Detects OS preference, applies stored override, exposes toggle

### 2.2 Runtime Behavior (High-Level)
- On load: ThemeService initializes theme
- User clicks toggle: Theme switched & persisted
- Mobile viewport: Nav condenses to hamburger; drawer overlays content with focus trap
- Admin link: Routes to Admin page placeholder (no role gating yet)
- Win highlight: Accent-blue variant for consistent brand accent

### 2.3 Technology & Patterns
- Framework: Blazor (.NET 9 target)
- Styling: Component-scoped CSS + global root variable layer
- State Persistence: localStorage (theme only)
- Accessibility Practices: semantic landmarks (`nav`, `main`), focus-visible styling, adequate color contrast
- Minimal JS interop (focus trap / storage as needed)

### 2.4 External Dependencies (Unchanged)
- Existing shared libraries & API endpoints (read-only usage in this iteration)
- No new third-party UI frameworks added

### 2.5 Deployment / Ops Impact
- Pure front-end change; no infrastructure or pipeline modifications
- Static asset (CSS/JS) cache invalidation only

---

## 3. Component Descriptions (High-Level)

| Component | Purpose | Notes | Reference Spec |
|-----------|---------|-------|----------------|
| MainLayout | Structural container & page composition | Applies max width (1280px), centers content, mounts nav & body. | spec-ui-layout_v1.01.md |
| TopNavigationComponent | Desktop navigation: brand, links, actions | Links: Play, How to Play, Training, Admin; Actions: New Game, Theme Toggle. | spec-ui-navigation_v1.01.md |
| MobileMenuDrawer | Off-canvas nav for small screens | Focus trap, ESC close, overlay dismissal. | spec-ui-navigation_v1.01.md |
| ThemeService & Toggle | Theme detection & persistence | Light/dark, localStorage key, initial OS preference. | spec-ui-theme_v1.01.md |
| Theme Token Layer | CSS variables (light/dark + semantic) | Neutral scale + blue accent; mapped tokens. | spec-ui-theme_v1.01.md |
| Game Board Styling Module | Visual styling for cells & win states | Semantic-only colors; accent win highlight. | spec-ui-board_v1.01.md |
| Play Page (Root) | Game interaction surface | Hosts board & actions. | spec-ui-routes_v1.01.md |
| How to Play Page | Instructional placeholder | Content stub; future expansion. | spec-ui-routes_v1.01.md |
| Training Page | Placeholder route | Minimal placeholder text. | spec-ui-routes_v1.01.md |
| Admin Page | Placeholder admin area | Placeholder content (no auth gating). | spec-ui-routes_v1.01.md |
| Accessibility Baseline | Cross-cutting practices | Focus, contrast, keyboard nav, semantics. | spec-ui-accessibility_v1.01.md |
| Win Highlight Style | Distinct feedback on win line | Accent variant; both themes. | spec-ui-board_v1.01.md |

### 3.1 Cross-Cutting Concerns (High-Level)
- Accessibility: Uniform focus ring token; working keyboard navigation including drawer
- Theming: All colors via semantic tokens; dark mode parity
- Responsiveness: Breakpoint to switch desktop nav ? hamburger (md breakpoint)
- Extensibility: Additional future pages reuse layout & nav patterns

### 3.2 Risks / Considerations
- Drawer focus trap correctness (keyboard & screen reader)
- Contrast maintenance across light/dark
- Potential future need for Admin gating introducing conditional nav logic

### 3.3 Assumptions
- Existing game logic requires no API changes
- Admin remains a stub until auth introduced
- Performance impact minimal

### 3.4 Open Items (Handled / Deferred in Component Specs)
- Exact token values (unchanged from v0.01 pending final palette QA)
- Potential later addition of skip link & statistics page

---

End of Overview Specification v1.01.
