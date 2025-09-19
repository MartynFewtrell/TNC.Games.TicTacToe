# Specification: UI Navigation
Version: v1.01  
Status: Draft  
Date: 2025-09-19  
Supersedes: spec-ui-navigation_v0.01.md  
Authors: UI/UX Working Draft  
Depends On: spec-ui-overview_v1.01.md  
Related Components: MainLayout, ThemeService, Accessibility Layer, Route Stubs

## Change Log
| Version | Date | Summary |
|---------|------|---------|
| v1.01 | 2025-09-19 | Admin link now active (no disabled state). Ensure all nav targets have pages (Play, How to Play, Training, Admin). Updated accessibility guidance (remove aria-disabled case). |
| v0.01 | 2025-09-19 | Initial draft with disabled Admin link. |

---
## 1. Purpose & Scope
Provide detailed functional & technical requirements for the top navigation bar and mobile hamburger drawer.

### 1.1 In-Scope (Updated)
- Desktop top navigation bar (brand + links + action group)
- Hamburger menu behavior below md breakpoint
- Mobile off-canvas drawer & overlay
- Theme toggle & New Game integration points
- Active Admin link (stub page target)

### 1.2 Out of Scope
- Authentication gating (all links public)
- New Game backend mechanics
- Additional feature pages (Statistics, Settings)

### 1.3 Goals
- Intuitive, fully functional navigation
- High accessibility & keyboard usability
- Consistent appearance in light & dark themes

---
## 2. Functional Requirements

### 2.1 Desktop Navigation Structure
Left ? Right:
1. Brand / Title: "Tic Tac Toe" (routes `/`)
2. Primary Links: 
   - Play (`/`) – same as brand link
   - How to Play (`/how-to-play`)
   - Training (`/training`)
   - Admin (`/admin`)
3. Action Group (right-aligned): New Game (button), Theme Toggle (icon)

### 2.2 Mobile Behavior
- Breakpoint: `md` (~768px)
- Below md: Show Brand + Hamburger + Theme Toggle (optional keep toggle visible; decision: keep toggle in drawer only to reduce header clutter)
- Hamburger Icon: Button `aria-label="Open menu"`
- Drawer Order: Play, How to Play, Training, Admin, (divider), New Game (button), Theme Toggle (icon)
- Close Mechanisms: Close button (X), ESC key, overlay click, selecting any navigation link/action
- Focus Trap: See Accessibility spec v1.01

### 2.3 Active Link Indication
- `aria-current="page"` on exact match
- Visual: 2px accent underline + text color = accent

### 2.4 Theme Toggle Integration
- Icon button (sun vs moon)
- `aria-label`: "Switch to dark mode" / "Switch to light mode"
- Resides: Desktop header action group; inside drawer on mobile

### 2.5 New Game Action
- Accent primary button
- Invokes game reset method (implementation plan will bind)

### 2.6 Error / Edge Cases
- If navigation attempt fails (route missing) ? must not occur; stub pages guarantee presence

---
## 3. Non-Functional Requirements
- Performance: Minimal bundle increase
- Accessibility: WCAG 2.1 AA; all nav items keyboard reachable
- Responsiveness: 320px min width supported

---
## 4. Theming & Styling
- Header background: `--color-surface` + bottom border `--color-border`
- Link hover: background `--color-surface-alt` (optional subtle) or underline fade-in (no transition required)
- Drawer: width 260px; background `--color-surface`; overlay token adaptable for dark mode
- Brand text: `--color-text`
- Accent usage restricted to active link, New Game button, focus states, theme toggle icon (if color-coded)

---
## 5. Data & State
| State | Source | Persistence |
|-------|--------|-------------|
| Drawer open | Nav component local | None |
| Theme | ThemeService | localStorage |

---
## 6. Events & Methods
| Event | Trigger | Result |
|-------|---------|--------|
| ToggleDrawer() | Hamburger click | Open/close drawer |
| CloseDrawer() | ESC / overlay / link click | Drawer closes; focus returns to hamburger |
| ToggleTheme() | Theme toggle | Theme switches & persists |
| StartNewGame() | New Game button | Game board reset logic invoked |

---
## 7. Accessibility Details (Updated)
- `nav aria-label="Primary"`
- Drawer: `<aside role="dialog" aria-modal="true" aria-label="Menu">`
- Focus trap ensures tab cycles within open drawer
- Hamburger has `aria-expanded` reflecting drawer state
- Admin link is a standard link (no disabled logic)

---
## 8. Risks & Mitigations
| Risk | Impact | Mitigation |
|------|--------|------------|
| Drawer focus trap errors | Keyboard entrapment | Test with screen readers & keyboard only |
| Overuse of accent color | Visual noise | Limit accent to specified elements |
| Route mismatch | Broken link | Ensure stubs/pages created in implementation |

---
## 9. Open Questions / Future Enhancements
- Add profile/user menu when auth introduced
- Add notification or statistics link later

---
## 10. Acceptance Criteria
- All nav links route successfully
- Drawer accessible & operable with keyboard
- Theme toggle updates theme and aria-label
- Active link styling accurate per route

---
End of spec-ui-navigation_v1.01.md
