# Specification: UI Accessibility Baseline
Version: v1.01  
Status: Draft  
Date: 2025-09-19  
Supersedes: spec-ui-accessibility_v0.01.md  
Authors: UI/UX Working Draft  
Depends On: spec-ui-overview_v1.01.md, spec-ui-theme_v1.01.md, spec-ui-navigation_v1.01.md  
Related Components: Navigation, MainLayout, Game Board Styling

## Change Log
| Version | Date | Summary |
|---------|------|---------|
| v1.01 | 2025-09-19 | Removed disabled Admin semantics; updated navigation focus order; clarified theme toggle aria-label pattern. |
| v0.01 | 2025-09-19 | Initial accessibility baseline. |

---
## 1. Purpose & Scope
Establish accessibility requirements ensuring WCAG 2.1 AA alignment across navigation, theming, and interactive board elements.

### 1.1 In-Scope (Updated)
- Focus visibility
- Keyboard operability (nav, drawer, board)
- Color contrast
- ARIA usage (active links, dialog/drawer semantics)

### 1.2 Out of Scope
- Localization; comprehensive screen reader transcripts

---
## 2. Focus Management
| Element | Behavior |
|---------|----------|
| Hamburger (open) | On click, focus moves to drawer close button |
| Drawer Close | On close, focus returns to hamburger button |
| Board Cells | Tabbable if interactive; focus ring visible |
| Admin Link | Standard link; part of normal tab sequence |

### 2.1 Focus Ring
Unchanged: 2px outline using `--color-focus-ring` with 2px offset.

---
## 3. Keyboard Interaction
| Action | Keys |
|--------|------|
| Open drawer | Enter/Space on hamburger |
| Close drawer | ESC, overlay click, close button, link selection |
| Navigate links | Tab / Shift+Tab |
| Activate theme toggle | Enter/Space |
| Start new game | Enter/Space on New Game button |

---
## 4. Contrast Requirements
Unchanged except Admin no longer exempt from interactive rule.

---
## 5. ARIA & Semantics
| Area | Implementation |
|------|----------------|
| Primary Navigation | `<nav aria-label="Primary">` |
| Mobile Drawer | `<aside role="dialog" aria-modal="true" aria-label="Menu">` |
| Active Link | `aria-current="page"` |
| Theme Toggle | `aria-label` describes target state |

---
## 6. Reduced Motion
Unchanged.

---
## 7. Testing Guidelines
Unchanged plus: verify Admin link functional.

---
## 8. Risks & Mitigations
Unchanged overall.

---
## 9. Acceptance Criteria
- Drawer keyboard operation passes
- All nav links (including Admin) functional
- Focus ring visible on all interactive elements
- Contrast passes automated checks

---
End of spec-ui-accessibility_v1.01.md
