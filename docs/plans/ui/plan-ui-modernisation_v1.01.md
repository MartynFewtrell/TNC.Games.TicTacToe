# Implementation Plan

Plan: UI Modernisation (Navigation, Theming, Layout, Dark Mode, Board Styling, Accessibility)
Version: v1.01  
Status: In Progress  
Date: 2025-09-19  
Based On Specs:  
- spec-ui-overview_v1.01.md  
- spec-ui-navigation_v1.01.md  
- spec-ui-theme_v1.01.md  
- spec-ui-layout_v1.01.md  
- spec-ui-board_v1.01.md  
- spec-ui-routes_v1.01.md  
- spec-ui-accessibility_v1.01.md  
- spec-ui-functional_v1.0.md

## Baseline
Current UI uses sidebar (`NavMenu.razor`) with brand "Tnc.Games.TicTacToe.Web", limited styling, no theme tokens, no dark mode, minimal accessibility enhancements. Pages present: Index (Play), Training, Admin. Missing: How To Play page. Board styling uses ad-hoc Bootstrap-like classes.

## Goals (Incremental Delivery)
Each Work Item delivers a user-visible, runnable increment. After any single Work Item the application builds, runs, and demonstrates the new capability without regressions in core gameplay.

## Work Item Overview (Sequenced)
1. Replace sidebar with top navigation bar + rename branding + How To Play page stub.
2. Introduce semantic light theme token layer + apply to layout & navigation (light mode only).
3. Implement dark mode (ThemeService + toggle + persistence + prefers-color-scheme bootstrap).
4. Restyle game board (semantic tokens, responsive sizing, accent win highlight).
5. Add mobile hamburger drawer (focus trap + accessibility) & center content container (1280px).
6. Integrate New Game into nav action group & refine Training/Admin route stubs content copy.
7. Accessibility hardening (focus rings, aria-current, keyboard audit, contrast validation) & cleanup legacy CSS.

> Justification: Items 1–4 build essential visual identity and core theming; Items 5–7 complete responsiveness, discoverability, and accessibility polish.

---
## Section: Navigation & Basic Layout

- [x] Work Item 1: Top Navigation & Branding + How To Play Page
  - Value: Users see modern horizontal nav with correct app title "Tic Tac Toe" and a new How To Play page (stub) instead of the sidebar; gameplay still functional.
  - Dependencies: None
  - [x] Task 1: Introduce TopNav component & remove sidebar usage
    - [x] Step 1: Create `Shared/TopNav.razor` with brand, links (Play, How to Play, Training, Admin) and placeholder New Game button (non-functional label only for now) & theme toggle placeholder icon (no logic yet, hidden or commented).
    - [x] Step 2: Replace sidebar layout in `MainLayout.razor` with header + main content structure.
    - [x] Step 3: Remove sidebar-specific markup & classes; retain About link if still desired (or migrate to future footer—defer for now, may remove temporarily).
    - [x] Step 4: Update `<PageTitle>` to "Tic Tac Toe".
  - [x] Task 2: Add How To Play Page Stub
    - [x] Step 1: Create `Pages/HowToPlay.razor` route `/how-to-play` with H1 + placeholder rules list.
    - [x] Step 2: Add NavLink to new page in TopNav.
  - [x] Task 3: Adjust existing NavMenu assets
    - [x] Step 1: Mark `NavMenu.razor` as deprecated or remove from layout (retain file until Work Item 7 cleanup to minimize diff noise / fallback option).
  - **Files (created/updated)**:
    - `src/web/Tnc.Games.TicTacToe.Web/Shared/MainLayout.razor`: Replaced layout structure to use TopNav.
    - `src/web/Tnc.Games.TicTacToe.Web/Shared/TopNav.razor`: New component.
    - `src/web/Tnc.Games.TicTacToe.Web/Pages/HowToPlay.razor`: New page.
    - `src/web/Tnc.Games.TicTacToe.Web/Shared/MainLayout.razor.css`: Updated layout CSS to remove sidebar usage and set centered container.
    - `src/web/Tnc.Games.TicTacToe.Web/Shared/TopNav.razor.css`: New styling for top nav.
  - **Verification**:
    - [x] Build succeeds
    - [x] App runs; navigation links route correctly
    - [x] Game board playable from Play (Index) page
    - [x] How To Play page loads stub content
  - **Work Item Dependencies**: None
  - **User Instructions**: Run app, observe new top nav, click each link, ensure game still playable.

---
## Section: Theming (Light First, Then Dark)

- [ ] Work Item 2: Semantic Light Theme Tokens
  - Value: Consistent professional light theme applied to nav/layout using semantic CSS variables (foundation for dark mode). Users see updated color palette.
  - Dependencies: Work Item 1
  - [ ] Task 1: Add token definitions (light only)
    - [ ] Step 1: Add `wwwroot/css/theme.css` (or `Shared/Theme.css`) with root palette + semantic tokens (light scope only).
    - [ ] Step 2: Reference stylesheet in `index.html` or `_Host.cshtml` (depending on hosting model).
  - [ ] Task 2: Refactor components to semantic tokens
    - [ ] Step 1: Update `TopNav.razor.css` to use semantic tokens for background, text, hover states.
    - [ ] Step 2: Update `MainLayout.razor.css` to use background + surface tokens.
    - [ ] Step 3: Remove hard-coded colors migrated to tokens.
  - **Files**:
    - `src/web/Tnc.Games.TicTacToe.Web/wwwroot/css/theme.css`: New theme token layer.
    - `src/web/Tnc.Games.TicTacToe.Web/Shared/TopNav.razor.css`: Token-based styling.
    - `src/web/Tnc.Games.TicTacToe.Web/Shared/MainLayout.razor.css`: Token-based styling.
  - **Verification**:
    - [ ] Build succeeds
    - [ ] App renders with new colors (no dark toggle yet)
    - [ ] No console CSS load errors
  - **Work Item Dependencies**: 1
  - **User Instructions**: Refresh app, confirm header & background use new palette.

- [ ] Work Item 3: Dark Mode Toggle & Persistence
  - Value: Users can toggle dark/light mode; preference persists across reloads and respects system preference initially.
  - Dependencies: Work Item 2
  - [ ] Task 1: Extend tokens for dark mode
    - [ ] Step 1: Add `[data-theme="dark"]` block to `theme.css` with dark semantic mappings.
  - [ ] Task 2: Implement ThemeService
    - [ ] Step 1: Create `Services/ThemeService.cs` registering as scoped.
    - [ ] Step 2: Add JS interop or minimal inline script (optional) to set initial `data-theme` before render to reduce flash.
    - [ ] Step 3: On first load: check localStorage key; fallback to `prefers-color-scheme`.
  - [ ] Task 3: Theme Toggle UI
    - [ ] Step 1: Add toggle button to `TopNav.razor` (icon changes sun/moon) with `aria-label`.
    - [ ] Step 2: Wire click to ThemeService toggle.
  - **Files**:
    - `src/web/Tnc.Games.TicTacToe.Web/wwwroot/css/theme.css`: Add dark scope.
    - `src/web/Tnc.Games.TicTacToe.Web/Services/ThemeService.cs`: New service.
    - `src/web/Tnc.Games.TicTacToe.Web/Shared/TopNav.razor`: Add toggle binding.
    - `src/web/Tnc.Games.TicTacToe.Web/Shared/TopNav.razor.css`: Optional style for toggle button.
    - `src/web/Tnc.Games.TicTacToe.Web/index.html` or hosting page: Inline script for early theme set.
  - **Verification**:
    - [ ] Build succeeds
    - [ ] Toggle switches theme instantly
    - [ ] LocalStorage updated
    - [ ] Refresh retains chosen theme
  - **Work Item Dependencies**: 2
  - **User Instructions**: Toggle theme; refresh; confirm persistence.

---
## Section: Board & Content Styling

- [ ] Work Item 4: Board Styling Modernisation
  - Value: Users see redesigned board with responsive sizing, semantic colors, accent win highlight.
  - Dependencies: Work Item 3 (for full token set) — could work after 2 but highlight relies on theme parity.
  - [ ] Task 1: Introduce board CSS refactor
    - [ ] Step 1: Create / update `Shared/BoardGrid.razor.css` to use grid layout + token-based colors.
    - [ ] Step 2: Implement responsive cell sizing using CSS clamp and breakpoints.
  - [ ] Task 2: Win Highlight
    - [ ] Step 1: Add conditional class for winning cells referencing `--color-win-highlight`.
  - [ ] Task 3: Focus & Hover states
    - [ ] Step 1: Add `:focus-visible` outline using focus token.
    - [ ] Step 2: Use surface-alt for empty-cell hover.
  - **Files**:
    - `src/web/Tnc.Games.TicTacToe.Web/Shared/BoardGrid.razor`: Add class logic for win highlight if needed.
    - `src/web/Tnc.Games.TicTacToe.Web/Shared/BoardGrid.razor.css`: New/updated responsive styling.
  - **Verification**:
    - [ ] Build succeeds
    - [ ] Board cells resize across window sizes
    - [ ] Winning line visually distinct
    - [ ] Dark mode board colors consistent
  - **Work Item Dependencies**: 3
  - **User Instructions**: Play a game to a win and observe highlight.

---
## Section: Responsiveness & Drawer

- [ ] Work Item 5: Mobile Hamburger Drawer + Centered Container
  - Value: Users on small screens see clean header with hamburger; drawer reveals nav links & actions; desktop content centered max 1280px wide.
  - Dependencies: 4 (drawer uses tokens, but could be after 3; sequencing chosen to avoid double styling passes)
  - [ ] Task 1: Centered Container
    - [ ] Step 1: Update `MainLayout.razor.css` to add `max-width:1280px; margin:0 auto; padding`.
  - [ ] Task 2: Hamburger + Drawer
    - [ ] Step 1: Add hamburger button visible below md (CSS media queries).
    - [ ] Step 2: Create `Shared/MobileMenuDrawer.razor` (or integrate logic in TopNav) with off-canvas panel and overlay.
    - [ ] Step 3: Implement open state in a cascading parameter or internal state.
    - [ ] Step 4: Add focus trap (simple first / fallback: prevent tabbing out by cycling last to first).
  - [ ] Task 3: Drawer Content
    - [ ] Step 1: Move (or duplicate) nav links + New Game + theme toggle into drawer markup.
  - **Files**:
    - `src/web/Tnc.Games.TicTacToe.Web/Shared/TopNav.razor`: Add hamburger + drawer trigger.
    - `src/web/Tnc.Games.TicTacToe.Web/Shared/TopNav.razor.css`: Styles for hamburger + breakpoints.
    - `src/web/Tnc.Games.TicTacToe.Web/Shared/MobileMenuDrawer.razor`: Drawer component.
    - `src/web/Tnc.Games.TicTacToe.Web/Shared/MobileMenuDrawer.razor.css`: Drawer styles.
    - `src/web/Tnc.Games.TicTacToe.Web/Shared/MainLayout.razor.css`: Center container.
  - **Verification**:
    - [ ] Build succeeds
    - [ ] Narrow viewport (<768px) shows hamburger
    - [ ] Drawer opens/closes via button, ESC, overlay
    - [ ] Focus returns to hamburger after close
  - **Work Item Dependencies**: 4
  - **User Instructions**: Resize window; test drawer interactions.

---
## Section: Action Integration & Stub Refinement

- [ ] Work Item 6: New Game Integration & Route Copy Polish
  - Value: Users start a new game directly from the navigation area; Stub pages (How To Play, Training, Admin) have clearer placeholders.
  - Dependencies: 5
  - [ ] Task 1: New Game in Nav
    - [ ] Step 1: Expose existing game reset method via EventCallback / service.
    - [ ] Step 2: Wire New Game button (desktop + drawer) to reset logic.
  - [ ] Task 2: Stub Content Polish
    - [ ] Step 1: Update How To Play with bullet list of basic rules.
    - [ ] Step 2: Training page: add "Coming soon" explanatory text.
    - [ ] Step 3: Admin page: add note about future management functions.
  - **Files**:
    - `src/web/Tnc.Games.TicTacToe.Web/Shared/TopNav.razor`: Hook New Game button.
    - `src/web/Tnc.Games.TicTacToe.Web/Pages/HowToPlay.razor`: Rules content.
    - `src/web/Tnc.Games.TicTacToe.Web/Pages/Training.razor`: Placeholder refinement.
    - `src/web/Tnc.Games.TicTacToe.Web/Pages/Admin.razor`: Placeholder refinement.
    - `src/web/Tnc.Games.TicTacToe.Web/Shared/BoardGrid.razor`: Ensure reset propagates properly if required.
  - **Verification**:
    - [ ] Build succeeds
    - [ ] Clicking New Game resets board state
    - [ ] Stub pages show updated content
  - **Work Item Dependencies**: 5
  - **User Instructions**: Play moves, press New Game, confirm cleared state.

---
## Section: Accessibility & Cleanup

- [ ] Work Item 7: Accessibility Hardening & Legacy Cleanup
  - Value: Users (including keyboard/screen reader) experience improved focus indication, aria semantics, consistent tokens; legacy unused sidebar code removed.
  - Dependencies: 6
  - [ ] Task 1: Focus & Aria
    - [ ] Step 1: Add `aria-current="page"` on nav links based on active route (NavLink does this by default; verify styling).
    - [ ] Step 2: Ensure hamburger has `aria-expanded` & `aria-controls`.
    - [ ] Step 3: Add `role="dialog" aria-modal="true"` to drawer root.
  - [ ] Task 2: Contrast & Token Audit
    - [ ] Step 1: Verify contrast (manual or tool) adjust token values if any fail (update theme.css).
  - [ ] Task 3: Remove Legacy Sidebar
    - [ ] Step 1: Delete `NavMenu.razor` and associated CSS if no longer referenced.
    - [ ] Step 2: Remove unused classes in `MainLayout.razor.css`.
  - [ ] Task 4: Docs Update
    - [ ] Step 1: Append change summary to relevant specs if required for v1.02 planning.
  - **Files**:
    - `src/web/Tnc.Games.TicTacToe.Web/Shared/TopNav.razor` & CSS: ARIA updates.
    - `src/web/Tnc.Games.TicTacToe.Web/wwwroot/css/theme.css`: Token adjustments (if needed).
    - `src/web/Tnc.Games.TicTacToe.Web/Shared/MobileMenuDrawer.razor`: Dialog semantics.
    - Remove: `src/web/Tnc.Games.TicTacToe.Web/Shared/NavMenu.razor` & `.css`.
  - **Verification**:
    - [ ] Build succeeds
    - [ ] Keyboard can traverse all interactive elements including drawer
    - [ ] Focus outline visible everywhere
    - [ ] No references to removed sidebar remain
  - **Work Item Dependencies**: 6
  - **User Instructions**: Keyboard test (Tab/Shift+Tab), screen reader smoke test, confirm no broken layout.

---
## Verification Matrix
| Work Item | User-Visible Increment | Regression Considerations |
|-----------|------------------------|---------------------------|
| 1 | Top nav + How To Play page | Game board still functional |
| 2 | New consistent light theme | No visual breakages on board area |
| 3 | Dark mode toggle | No FOUC or incorrect persistence |
| 4 | Modern board styling & win highlight | Input still responsive, accessibility intact |
| 5 | Mobile drawer & centered layout | Desktop unaffected, no scroll issues |
| 6 | New Game in nav & refined stubs | Game reset unchanged logic |
| 7 | Accessible polish & cleanup | No missing routes, no broken styles |

---
## Risk & Mitigation Summary
| Risk | Phase | Mitigation |
|------|-------|------------|
| Flash-of-incorrect-theme | 3 | Early inline script sets theme attribute |
| Drawer focus trap complexity | 5 | Keep minimal: focus first element; manual test; enhance later if needed |
| Board resizing regression | 4 | Use explicit min sizes + manual test on narrow viewport |
| Token misuse/hard-coded colors | 2–7 | Code review & CSS audit before closing each item |

---
## Future (Deferred Beyond v1.01)
- Statistics page
- Auth gating for Admin
- Settings / preferences pane
- High contrast theme variant
- Animations (reduced-motion compatible)

---
## Summary
This plan delivers a vertically sliced modernization: navigation first for immediate brand uplift, then theming foundation, dark mode, visual board improvements, responsive mobile drawer, action integration, and final accessibility polish. Each Work Item yields a runnable, testable increment with minimal risk overlap.

# Architecture
## Overall Technical Approach
Modernize the existing Blazor (Server or WASM) UI by layering a semantic theming system (CSS variables) and restructuring layout/navigation without altering backend APIs. Introduce a lightweight ThemeService for preference persistence. Maintain incremental, vertical slices ensuring gameplay operable throughout.

Mermaid (High-Level Component Interaction):
```mermaid
graph TD
  A[TopNav] --> B[ThemeService]
  A --> C[MobileDrawer]
  A --> D[New Game Action]
  D --> E[BoardGrid]
  E --> F[API Calls (existing)]
  ThemeService -->|data-theme attribute| G[Document Root]
```

## Frontend
- Components:
  - `TopNav` / `MobileMenuDrawer`: Navigation & actions.
  - `BoardGrid`: Renders game state; updated styling tokens.
  - `GameStatus`: Existing unchanged, adopts tokens.
  - Pages: `Index` (Play), `HowToPlay`, `Training`, `Admin`.
- Theming:
  - `wwwroot/css/theme.css` defines palette + semantic tokens for light/dark.
  - `ThemeService` manages persistence (localStorage) + initial detection.
- Layout:
  - `MainLayout` applies centered container (max 1280px) and includes `TopNav`.
- Responsive:
  - CSS media queries switch nav links ? hamburger/drawer below md.

## Backend
- No structural changes; continues serving API endpoints consumed by Board interactions (`/api/v1/turn`, `/api/v1/state`, `/api/v1/selfplay`, admin endpoints).
- No new endpoints required for UI modernization.
- Potential future enhancements (statistics, auth) will extend existing API.

---
End of plan-ui-modernisation_v1.01.md
