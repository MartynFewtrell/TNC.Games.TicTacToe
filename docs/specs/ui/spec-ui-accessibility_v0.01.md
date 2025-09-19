# Specification: UI Accessibility Baseline
Version: v0.01  
Status: Draft  
Date: 2025-09-19  
Supersedes: (none)  
Authors: UI/UX Working Draft  
Depends On: spec-ui-overview_v0.01.md, spec-ui-theme_v0.01.md  
Related Components: Navigation, MainLayout, Game Board Styling

---
## 1. Purpose & Scope
Establish accessibility requirements for the modernized UI (navigation, theming, board interaction) ensuring WCAG 2.1 AA alignment.

### 1.1 In-Scope
- Focus visibility
- Keyboard operability (nav, drawer, board)
- Color contrast
- aria usage patterns
- Disabled Admin link semantics

### 1.2 Out of Scope
- Screen reader specific optimization testing scripts
- Localization & language switching

---
## 2. Focus Management
| Element | Behavior |
|---------|----------|
| Hamburger (open) | On click, focus moves to drawer close button |
| Drawer Close | On close, focus returns to hamburger button |
| Board Cells | Tabbable if interactive; focus ring visible |
| Disabled Admin | Not focusable; `aria-disabled="true"` |

### 2.1 Focus Ring
- 2px outline color `--color-focus-ring`, offset 2px, only on `:focus-visible`

---
## 3. Keyboard Interaction
| Action | Keys |
|--------|------|
| Open drawer | Enter/Space on hamburger |
| Close drawer | ESC, overlay click (not keyboard, but accessible), Close button, selecting link |
| Cycle board cells | Tab / Shift+Tab (linear), Arrow keys (future enhancement) |

---
## 4. Contrast Requirements
| Element | Ratio Target |
|---------|--------------|
| Text vs surface | ? 4.5:1 |
| Icons vs surface | ? 3:1 |
| Focus outline vs adjacent surface | ? 3:1 |
| Disabled Admin text | Exempt (informational) but must remain legible ? 3:1 |

---
## 5. ARIA & Semantics
| Area | Implementation |
|------|----------------|
| Primary Navigation | `<nav aria-label="Primary" />` |
| Mobile Drawer | `<aside role="dialog" aria-modal="true" aria-label="Menu" />` |
| Disabled Admin | `aria-disabled="true"` + tooltip via `title` |
| Active Link | `aria-current="page"` |
| Theme Toggle | `aria-label` describes target state |

---
## 6. Reduced Motion
- No non-essential animations; minimal inherent style changes
- Respect `prefers-reduced-motion: reduce` if any future transitions added

---
## 7. Testing Guidelines
| Tool | Purpose |
|------|---------|
| Keyboard only pass | Ensure full navigation without mouse |
| Axe / accessibility linter | Contrast & aria checks |
| Screen reader spot check (NVDA/VoiceOver) | Landmarks & label clarity |

---
## 8. Risks & Mitigations
| Risk | Impact | Mitigation |
|------|--------|------------|
| Focus trap failure | User stuck or lost | Unit test focus management; manual QA |
| Inadequate contrast in dark mode | Accessibility failure | Adjust tokens before release |

---
## 9. Acceptance Criteria
- All interactive elements show focus ring on keyboard navigation
- Drawer fully operable via keyboard (open/close, focus return)
- Contrast checks pass for text & icons
- Admin disabled state is non-interactive & announced appropriately

---
End of spec-ui-accessibility_v0.01.md
