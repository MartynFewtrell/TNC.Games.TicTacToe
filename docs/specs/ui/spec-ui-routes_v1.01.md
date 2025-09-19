# Specification: UI Route Pages
Version: v1.01  
Status: Draft  
Date: 2025-09-19  
Supersedes: spec-ui-routes_v0.01.md  
Authors: UI/UX Working Draft  
Depends On: spec-ui-overview_v1.01.md  
Related Components: MainLayout, Navigation

## Change Log
| Version | Date | Summary |
|---------|------|---------|
| v1.01 | 2025-09-19 | Added How to Play page; Admin now active (removed disabled semantics); clarified stub content standards. |
| v0.01 | 2025-09-19 | Initial route stub spec (Training/Admin). |

---
## 1. Purpose & Scope
Ensure all navigation destinations are implemented pages: Play (root), How to Play, Training, Admin.

### 1.1 In-Scope (Updated)
- Route definitions for: `/` (Play), `/how-to-play`, `/training`, `/admin`
- Minimal page content & structure

### 1.2 Out of Scope
- Detailed instructional content (How to Play) beyond placeholder
- Real Admin / Training functionality

---
## 2. Functional Requirements
| Route | Component Name | Content Minimum |
|-------|----------------|-----------------|
| / | `PlayPage` or existing index component | Game board + New Game button integration |
| /how-to-play | `HowToPlayPage` | H1 + basic rules list or placeholder paragraph |
| /training | `TrainingPage` | H1 + placeholder paragraph |
| /admin | `AdminPage` | H1 + placeholder paragraph |

### 2.1 Content Guidelines
- Each page: Single `<h1>` + descriptive paragraph or list
- May include future anchor sections (not required now)

---
## 3. Layout & Theming
- All pages use `MainLayout`
- Conform to spacing tokens

---
## 4. Accessibility
- Clear and descriptive headings
- Avoid duplicate H1s

---
## 5. Non-Functional Requirements
- Lightweight (no external fetch on stub pages)

---
## 6. Risks
| Risk | Impact | Mitigation |
|------|--------|------------|
| Placeholder persists too long | User confusion | Add "Coming soon" note |

---
## 7. Acceptance Criteria
- Navigating to each route loads correct content
- All pages share consistent layout & theme

---
End of spec-ui-routes_v1.01.md
