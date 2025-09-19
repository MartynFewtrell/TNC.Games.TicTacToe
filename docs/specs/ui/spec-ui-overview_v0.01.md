# Specification: UI Modernisation Overview
Version: v0.01  
Status: Draft  
Date: 2025-09-19  
Supersedes: (none)  
Authors: UI/UX Working Draft  
Related Branch: feature-update-appearance  
Related Specs:  
- spec-ui-navigation_v0.01.md  
- spec-ui-theme_v0.01.md  
- spec-ui-layout_v0.01.md  
- spec-ui-board_v0.01.md  
- spec-ui-routes_v0.01.md  
- spec-ui-accessibility_v0.01.md

---

## 1. Purpose & Scope

### 1.1 Purpose
Modernize and professionalize the Blazor Tic Tac Toe web UI by improving layout, navigation, theming, accessibility foundations, and visual clarity while preserving existing game logic.

### 1.2 In-Scope
- Rename application to “Tic Tac Toe” (title/branding only; favicon unchanged)
- Replace vertical left navigation with a top horizontal bar + mobile hamburger drawer
- Centralize page content with a max width of 1280px
- Introduce a cool neutral + blue accent theme (light + dark modes) via CSS variables
- Implement full dark mode support (toggle + persistence via localStorage; honours `prefers-color-scheme`)
- Add placeholder routes: Training, Admin (non-functional stubs)
- Show Admin link disabled (with tooltip) when unauthorized (no auth implementation yet)
- Update game board styling (semantic tokens; accent-blue win highlight)
- Consistent component styling (4px corner radius; essential focus only, no decorative animation)
- Mobile off?canvas drawer for navigation below medium breakpoint

### 1.3 Out of Scope
- Authentication / authorization implementation
- Training content, Admin functionality, Statistics, Settings, Leaderboards
- Favicon / manifest redesign
- Game engine or backend API changes
- Advanced animation or motion design
- PWA enhancements & offline support
- Internationalization / localization

### 1.4 Objectives
- Improve clarity and professionalism of UI
- Establish a semantic theming layer (extensible for future pages)
- Enhance accessibility baseline (contrast, focus states, keyboard navigation)
- Provide a foundation for future feature pages (Statistics, Admin, Training content)

### 1.5 Success Criteria (Qualitative)
- Navigation intuitive on desktop and mobile
- Theme toggle works reliably across reloads with stored preference
- All interactive elements keyboard reachable with visible focus states
- Color system consistent across light/dark with adequate contrast (WCAG AA)

---

## 2. System Overview

The UI layer is a Blazor application acting as the presentation shell for the Tic Tac Toe game board and related stub pages. Enhancements introduce a structured layout, modular theming, and route placeholders while keeping the domain/game logic untouched.

### 2.1 High-Level Structure
- Layout Shell: Provides top navigation, centered content region, theme root
- Theming Layer: CSS custom properties (light & dark palettes, semantic tokens)
- Navigation System: Horizontal bar (desktop) + hamburger-triggered drawer (mobile)
- Game Board Presentation: Existing component re-styled via tokens only
- Auxiliary Pages: Training (stub), Admin (stub with disabled state logic)
- Client Theme Service: Detects OS preference, applies stored override, exposes toggle

### 2.2 Runtime Behavior (High-Level)
- On load: ThemeService initializes theme (prefers-color-scheme, then localStorage override if present)
- User clicks toggle: Theme switched & persisted
- Mobile viewport: Nav condenses to hamburger; drawer overlays content with focus trap
- Admin link: Renders disabled state (aria-disabled + tooltip) when no authorization context is present (auth deferred)
- Win highlight: Uses accent-blue variant for consistent brand accent

### 2.3 Technology & Patterns
- Framework: Blazor (.NET 9 target)
- Styling: Component-scoped CSS + global root variable layer
- State Persistence: localStorage (theme only)
- Accessibility Practices: semantic landmarks (`nav`, `main`), focus-visible styling, disabled semantics, adequate color contrast
- Minimal JS interop (only if required for focus trap or storage convenience)

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
| MainLayout | Structural container & page composition | Applies max width (1280px), centers content, mounts nav & body. | spec-ui-layout_v0.01.md |
| TopNavigationComponent | Desktop navigation: brand, links, actions | Links: Play, How to Play, Training; Actions: New Game, Theme Toggle, Admin (disabled). | spec-ui-navigation_v0.01.md |
| MobileMenuDrawer | Off-canvas nav for small screens | Focus trap, ESC close, overlay dismissal. | spec-ui-navigation_v0.01.md |
| ThemeService & Toggle | Theme detection & persistence | Light/dark, localStorage key, initial OS preference. | spec-ui-theme_v0.01.md |
| Theme Token Layer | CSS variables (light/dark + semantic) | Neutral scale + blue accent; mapped tokens. | spec-ui-theme_v0.01.md |
| Game Board Styling Module | Visual styling for cells & win states | Semantic-only colors; accent win highlight. | spec-ui-board_v0.01.md |
| Training Page Stub | Placeholder route | Minimal placeholder text. | spec-ui-routes_v0.01.md |
| Admin Page Stub | Placeholder admin area | Disabled nav link until auth added. | spec-ui-routes_v0.01.md |
| Admin Disabled Link Handler | Conditional disabled rendering | Tooltip + aria-disabled; future auth hook. | spec-ui-navigation_v0.01.md |
| Accessibility Baseline | Cross-cutting practices | Focus, contrast, keyboard nav, semantics. | spec-ui-accessibility_v0.01.md |
| Win Highlight Style | Distinct feedback on win line | Accent variant; both themes. | spec-ui-board_v0.01.md |

### 3.1 Cross-Cutting Concerns (High-Level)
- Accessibility: Uniform focus ring token; disabled vs hidden semantics for Admin; keyboard path for hamburger/drawer
- Theming: All colors referenced via semantic CSS variables; dark mode parity
- Responsiveness: Breakpoint to switch desktop nav ? hamburger (md breakpoint)
- Extensibility: Future Statistics / Settings pages plug into same nav/action pattern

### 3.2 Risks / Considerations
- Drawer focus trap must avoid scroll locking bugs (test with keyboard + screen reader)
- Dark mode palette must maintain ?4.5:1 contrast for text on surfaces
- Avoid flash-of-incorrect-theme (inline root class or minimal acceptable flash)
- Disabled Admin link must not present as interactive to assistive tech (aria-disabled + prevent navigation)

### 3.3 Assumptions
- Existing game logic requires no API changes
- Authorization model added later (feature flag or role claim)
- No user personalization beyond theme preference in this iteration
- Performance impact negligible (CSS variable usage only)

### 3.4 Open Items (Handled in Component Specs)
- Exact token values (hex / RGB) for neutral & accent scale
- Focus ring style spec (color/thickness/offset)
- Drawer breakpoint, z-index layering & overlay opacity
- Tooltip implementation (native `title` vs custom component)

---

End of Overview Specification v0.01.
