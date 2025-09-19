# Specification: Game Board Styling
Version: v0.01  
Status: Draft  
Date: 2025-09-19  
Supersedes: (none)  
Authors: UI/UX Working Draft  
Depends On: spec-ui-overview_v0.01.md, spec-ui-theme_v0.01.md  
Related Components: Theme Token Layer, MainLayout

---
## 1. Purpose & Scope
Define visual styling for the Tic Tac Toe board consistent with the new theme, ensuring accessible contrast, clear interaction states, and win highlighting.

### 1.1 In-Scope
- Board container styling
- Cell size, spacing, responsive behavior
- Mark (X/O) styling referencing semantic tokens
- Hover, focus, active, win states

### 1.2 Out of Scope
- Game logic, move validation
- Animation sequences or transitions

---
## 2. Functional Requirements
- Board remains a 3x3 grid
- Cells square; scale responsively with viewport up to defined max width
- Win highlight uses accent variant background or border emphasize
- Marks legible in both themes

### 2.1 Sizing
| Viewport Width | Cell Dimension (approx) |
|----------------|-------------------------|
| < 400px | 72px |
| 400–599px | 84px |
| 600–899px | 96px |
| ? 900px | 108px (cap) |

### 2.2 Structure
```
<div class="board">
  <button class="cell" ...></button>
  ... (9 cells)
</div>
```

### 2.3 States
| State | Style Guide |
|-------|-------------|
| Idle | Background: `--color-surface`; Border: `--color-border` |
| Hover (empty) | Background: `--color-surface-alt`; Cursor: pointer |
| Focus | Outline: 2px `--color-focus-ring` (offset 2px) |
| Occupied | Background: `--color-surface`; No hover change |
| Win | Background overlay or inner highlight: `--color-win-highlight` |
| Disabled (game over) | Cursor: default; reduce text/mark opacity to 0.6 |

### 2.4 Mark Rendering
- Font weight: 600–700
- Font size: ~60% of cell dimension (clamp using CSS)
- Color: `--color-accent`
- Distinguish X vs O by glyph only (future: distinct accent vs secondary color if needed)

### 2.5 Layout & Gaps
- Use CSS grid: `grid-template-columns: repeat(3, 1fr);`
- Gap between cells: 8px (`var(--space-2)`) for readability

---
## 3. Accessibility
- Contrast: Mark color vs surface ? 4.5:1 in both themes
- Focus ring visible even on win-highlighted cells
- Do not rely solely on color for win (optional future improvement: subtle symbol glow or border)

---
## 4. Theming Integration
- All colors via semantic tokens
- Win highlight token differs between light/dark (`--color-win-highlight` defined in theme spec)

---
## 5. Non-Functional Requirements
- Board resizes smoothly without causing layout shifts outside container
- No transition animations applied

---
## 6. Risks & Mitigations
| Risk | Impact | Mitigation |
|------|--------|------------|
| Small devices cause cramped board | Difficult interaction | Minimum 72px enforced |
| Low contrast in dark mode | Accessibility issue | Validate tokens with contrast tool |

---
## 7. Acceptance Criteria
- Board responsive at defined breakpoints
- Win cells visibly distinct using accent variant
- Focus outline consistently visible
- Colors adapt correctly between light/dark

---
End of spec-ui-board_v0.01.md
