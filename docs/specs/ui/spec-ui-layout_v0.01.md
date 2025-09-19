# Specification: UI Layout
Version: v0.01  
Status: Draft  
Date: 2025-09-19  
Supersedes: (none)  
Authors: UI/UX Working Draft  
Depends On: spec-ui-overview_v0.01.md  
Related Components: TopNavigationComponent, Game Board Styling, ThemeService

---
## 1. Purpose & Scope
Define layout structure changes: centered content region, max width constraint, spacing, and integration of navigation & theme layers.

### 1.1 In-Scope
- Main layout container (max-width 1280px)
- Vertical spacing & responsive paddings
- Integration of top navigation bar
- Content area structure for pages & stubs

### 1.2 Out of Scope
- Grid redesign of game board itself (handled by board styling spec)
- Multi-column dashboard layouts

---
## 2. Functional Requirements
- All primary pages share the same `MainLayout`
- Layout sets a wrapping container with `margin: 0 auto; max-width:1280px; padding: 0 var(--space-4);`
- Provide consistent vertical rhythm: sections separated by `var(--space-6)`
- Navigation bar fixed or static? Decision: static (no fixed positioning) to avoid overlap complexity
- Body background fills viewport using `--color-background`

---
## 3. Structure
```
<html data-theme="light|dark">
  <body>
    <header>
      <Nav />
    </header>
    <main id="main-content" tabindex="-1">
      <!-- Routed Body -->
    </main>
  </body>
</html>
```

### 3.1 Main Region
- `id="main-content"` for skip links (even though skip link not included now, id future-proofs)
- Use flex or block; game board horizontally centered via auto margins

### 3.2 Responsive
| Breakpoint | Behavior |
|------------|----------|
| < 480px | Container horizontal padding reduces to `var(--space-2)` |
| ? 480px | Standard padding `var(--space-4)` |
| ? 1280px | Content capped (centered) |

---
## 4. Spacing Scale Usage
- Headings top margin minimal; bottom margin uses `var(--space-3)`
- Buttons group gap: `var(--space-2)`
- Game board top margin: `var(--space-6)`

---
## 5. Non-Functional Requirements
- No horizontal scroll at 320px width
- Layout shift minimized when toggling theme (no height jumps)

---
## 6. Accessibility Considerations
- Landmarks: `<header>`, `<main>`
- `tabindex="-1"` on main allows programmatic focus (improves screen reader handoff after nav interactions later)

---
## 7. Risks
| Risk | Mitigation |
|------|------------|
| Over-constrained container for future wider boards | Provide easy token change for max width |
| Missing skip link now may delay future a11y | Kept main id & tabindex to ease later addition |

---
## 8. Acceptance Criteria
- All pages render within 1280px centered container
- No overflow at smallest targeted viewport (320px)
- Theme toggle does not cause layout jumps

---
End of spec-ui-layout_v0.01.md
