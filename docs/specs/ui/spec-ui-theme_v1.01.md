# Specification: UI Theme & Tokens
Version: v1.01  
Status: Draft  
Date: 2025-09-19  
Supersedes: spec-ui-theme_v0.01.md  
Authors: UI/UX Working Draft  
Depends On: spec-ui-overview_v1.01.md  
Related Components: ThemeService, Navigation, Game Board Styling

## Change Log
| Version | Date | Summary |
|---------|------|---------|
| v1.01 | 2025-09-19 | Clarified token usage stability; no disabled Admin styling; confirmed accent win highlight; reserved hooks for future success/danger usage. |
| v0.01 | 2025-09-19 | Initial theming draft. |

---
## 1. Purpose & Scope
Define the theming system (light/dark) for the modernized Tic Tac Toe UI, including CSS variable tokens, semantic roles, toggle behavior, persistence, and accessibility constraints.

### 1.1 In-Scope (Unchanged)
- Light & dark base palettes (neutral + blue accent)
- Semantic token mapping layer
- Theme toggle behavior & persistence
- Win highlight stylistic definition
- Focus ring styling

### 1.2 Out of Scope
- User-custom theme builder
- High-contrast separate accessibility theme (future)

---
## 2. Design Principles
(unchanged from v0.01) Separation, Consistency, Accessibility, Minimal Motion

---
## 3. Token Structure
(unchanged palette list; reused from v0.01) ...

(Refer to v0.01 for full palette; implementation will lift directly.)

No changes required to token naming; all components to reference semantic tokens only.

---
## 4. Application & CSS Structure
Unchanged; dark/light selected via `data-theme` attribute.

---
## 5. Theme Toggle Behavior
Unchanged from v0.01.

---
## 6. Focus Ring Specification
Unchanged from v0.01.

---
## 7. Game Board Theming Specifics
Clarification: X and O share accent color in this release; differentiation by glyph only.

---
## 8. Non-Functional Requirements
Unchanged.

---
## 9. Risks & Mitigations
Unchanged; validate contrast before release.

---
## 10. Acceptance Criteria
Unchanged with addition: All navigation pages reflect correct theme tokens.

---
End of spec-ui-theme_v1.01.md
