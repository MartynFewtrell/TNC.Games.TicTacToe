# Specification: UI Route Stubs
Version: v0.01  
Status: Draft  
Date: 2025-09-19  
Supersedes: (none)  
Authors: UI/UX Working Draft  
Depends On: spec-ui-overview_v0.01.md  
Related Components: MainLayout, Navigation

---
## 1. Purpose & Scope
Provide placeholder pages for Training and Admin to ensure navigation integrity during UI modernization.

### 1.1 In-Scope
- Route definitions for /training and /admin
- Minimal page content & structure

### 1.2 Out of Scope
- Actual training modules, admin functions, analytics, settings

---
## 2. Functional Requirements
| Route | Component Name | Content |
|-------|----------------|---------|
| /training | `TrainingPage` | Heading + placeholder paragraph |
| /admin | `AdminPage` | Heading + placeholder paragraph (notes disabled nav behavior) |

### 2.1 Content Guidelines
- Use `<h1>` for primary heading
- Provide a short descriptive paragraph
- Optionally include a muted note about future implementation

---
## 3. Layout & Theming
- Pages use `MainLayout`
- Respect max width & spacing tokens
- Theming consistent (no custom inline colors)

---
## 4. Accessibility
- Proper heading hierarchy (single H1 per page)
- Descriptive text avoids jargon

---
## 5. Non-Functional Requirements
- Fast load (simple markup only)
- No additional JS

---
## 6. Risks
| Risk | Impact | Mitigation |
|------|--------|------------|
| Placeholder content persists too long | User confusion | Add clear "Coming soon" language |

---
## 7. Acceptance Criteria
- Navigating to /training & /admin loads stub pages without errors
- Pages inherit layout & theme

---
End of spec-ui-routes_v0.01.md
