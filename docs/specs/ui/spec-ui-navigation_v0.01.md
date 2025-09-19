# Specification: UI Navigation
Version: v0.01  
Status: Draft  
Date: 2025-09-19  
Supersedes: (none)  
Authors: UI/UX Working Draft  
Depends On: spec-ui-overview_v0.01.md  
Related Components: MainLayout, ThemeService, Accessibility Layer

---
## 1. Purpose & Scope
Provide detailed functional & technical requirements for the new top navigation bar and mobile hamburger drawer in the Blazor Tic Tac Toe UI modernization.

### 1.1 In-Scope
- Desktop top navigation bar (brand + links + action group)
- Hamburger menu behavior below md breakpoint
- Mobile off-canvas drawer & overlay
- Admin disabled state behavior
- Theme toggle & New Game integration points

### 1.2 Out of Scope
- Authentication logic (authorization context always "unauthorized" for Admin in this iteration)
- New Game backend mechanics (assumes existing method/event)
- Statistics / Settings pages

### 1.3 Goals
- Provide intuitive navigation responsive to viewport changes
- Maintain accessibility (ARIA roles, focus management, keyboard navigation)
- Ensure consistent theming across light/dark

---
## 2. Functional Requirements

### 2.1 Desktop Navigation Structure
Order (left ? right):
1. Brand / Title: "Tic Tac Toe" (navigates to root/home)
2. Primary Links: Play (/), How to Play (/how-to-play), Training (/training)
3. Action Group (right-aligned): New Game (button), Theme Toggle (icon), Admin (disabled link or span)

### 2.2 Mobile Behavior
- Breakpoint: `md` (approx 768px) – below this, hide primary links & action group (except brand + hamburger icon)
- Hamburger Icon: Button with aria-label="Open menu"; toggles drawer
- Drawer Content Order: Play, How to Play, Training, New Game (button style), Admin (disabled), Theme Toggle (icon)
- Drawer Close Methods: Close button (X), ESC key, overlay click, selecting a navigation link or action
- Focus Trap: First tabbable inside on open; return focus to hamburger on close

### 2.3 Admin Disabled State
- Render as a non-focusable element OR focusable with `tabindex="-1"` and `aria-disabled="true"`
- Tooltip: Native `title="Admin access required"`
- Styling: Muted text color token + disabled cursor (not-allowed)
- No navigation when clicked

### 2.4 Theme Toggle Integration
- Icon button (sun for light, moon for dark)
- aria-label cycles: "Switch to dark mode" / "Switch to light mode"
- Emits event to ThemeService or directly calls toggle method

### 2.5 New Game Action
- Primary accent button styling
- Invokes existing game reset/new start method (to be integrated in implementation plan)
- Should remain visible in drawer and desktop layouts

### 2.6 Active Link Indication
- Use `aria-current="page"` and a visual indicator (underline or accent bar) derived from accent color token

### 2.7 Accessibility & Keyboard
- Tab order: Brand ? Primary Links (desktop) ? New Game ? Theme Toggle ? Admin
- Hamburger reachable at index 1 in mobile layout
- Drawer: first focus target is close button; SHIFT+TAB from first wraps to last (trap) or prevented
- ESC always closes drawer when open

### 2.8 Error / Edge Cases
- If ThemeService not initialized yet, toggle disabled (fallback: no-op)
- Multiple rapid toggles debounced (optional; spec: not required)

---
## 3. Non-Functional Requirements
- Performance: Minimal layout shift; no heavy libraries
- Accessibility: WCAG 2.1 AA color contrast; focus outline visible
- Responsiveness: Works at 320px width minimum
- Internationalization: Not required (hard-coded English for now)

---
## 4. Theming & Styling
- Use CSS variables from Theme Token Layer for colors, spacing, typography
- Height: ~56px desktop bar; 48–52px mobile
- Background: `--color-surface` with subtle bottom border `--color-border`
- Active link underline thickness: 2px (accent color)
- Hover states: background subtle tint (using surface-alt token) for buttons/links
- Drawer: slides from left; width 260px; background `--color-surface`; overlay rgba(0,0,0,0.4) (color adapted for dark mode via token)

---
## 5. Data & State
| State | Source | Persistence |
|-------|--------|-------------|
| Drawer open/closed | Component local | Not persisted |
| Theme (light/dark) | ThemeService | localStorage |
| Admin authorization flag | (stub: false) | None |

---
## 6. Events & Methods
| Event/Action | Trigger | Result |
|--------------|--------|--------|
| ToggleDrawer() | Hamburger click | Open/close drawer; manage focus |
| CloseDrawer() | ESC, overlay click, link click | Drawer closes; focus returns |
| ToggleTheme() | Theme toggle button | ThemeService switches theme |
| StartNewGame() | New Game button | Calls existing game reset logic |

---
## 7. Accessibility Details
- Role: `<nav aria-label="Primary" />`
- Drawer: `<aside role="dialog" aria-modal="true" aria-label="Menu">`
- Focus trap implemented via JS interop or pure Blazor (looping logic)
- All interactive elements have visible focus outline using focus ring token
- Admin disabled element: `aria-disabled="true"` and excluded from tab order

---
## 8. Risks & Mitigations
| Risk | Impact | Mitigation |
|------|--------|------------|
| Focus trap failure | Keyboard users lost in background | Test with screen readers & implement robust trap logic |
| Drawer flicker on theme change | Minor UX inconsistency | Avoid heavy transitions; re-render minimal scope |
| Admin appears clickable | Confusion | Disabled styling + `cursor: not-allowed` + tooltip |

---
## 9. Open Questions / Future Enhancements
- Add role-based logic for Admin link enabling
- Add Statistics or Settings links later
- Add optional notification badge support

---
## 10. Acceptance Criteria (High-Level)
- Nav renders correctly desktop & mobile
- Drawer opens, traps focus, closes via all specified mechanisms
- Theme toggle updates aria-label and theme immediately
- Admin link disabled (unclickable) with tooltip
- Active route visually indicated & sets `aria-current`

---
End of spec-ui-navigation_v0.01.md
