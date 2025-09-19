# Specification: UI Layout
Version: v1.01  
Status: Draft  
Date: 2025-09-19  
Supersedes: spec-ui-layout_v0.01.md  
Authors: UI/UX Working Draft  
Depends On: spec-ui-overview_v1.01.md  
Related Components: Navigation, ThemeService, Game Board Styling

## Change Log
| Version | Date | Summary |
|---------|------|---------|
| v1.01 | 2025-09-19 | Confirmed Admin now active; added guarantee that each nav page uses MainLayout; clarified skip link omission and future readiness. |
| v0.01 | 2025-09-19 | Initial layout draft. |

---
## 1. Purpose & Scope
Define layout structure changes: centered content region, max width constraint, spacing, and integration of navigation & theme layers.

### 1.1 In-Scope (Unchanged)
- Main layout container (max-width 1280px)
- Vertical spacing & responsive paddings
- Integration of top navigation bar
- Content area structure for pages & stubs

### 1.2 Out of Scope
- Grid redesign of game board itself
- Multi-column dashboard layouts

---
## 2. Functional Requirements (Updated Clarifications)
- All pages: Play, How to Play, Training, Admin use `MainLayout`
- Layout sets container: `margin: 0 auto; max-width:1280px; padding: 0 var(--space-4);`
- Root page (Play) hosts game board + actions

---
## 3. Structure
(unchanged diagram from v0.01)

### 3.1 Main Region
- `id="main-content"` retained for potential future skip link addition

### 3.2 Responsive
Unchanged.

---
## 4. Spacing Scale Usage
Unchanged from v0.01.

---
## 5. Non-Functional Requirements
Unchanged.

---
## 6. Accessibility Considerations
- Landmarks: `<header>`, `<main>`
- `tabindex="-1"` on main retained

---
## 7. Risks
Unchanged.

---
## 8. Acceptance Criteria
Add: All navigation targets share consistent container styling.

---
End of spec-ui-layout_v1.01.md
