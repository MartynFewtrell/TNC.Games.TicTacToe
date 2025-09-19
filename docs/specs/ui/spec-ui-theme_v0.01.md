# Specification: UI Theme & Tokens
Version: v0.01  
Status: Draft  
Date: 2025-09-19  
Supersedes: (none)  
Authors: UI/UX Working Draft  
Depends On: spec-ui-overview_v0.01.md  
Related Components: ThemeService, TopNavigationComponent, Game Board Styling

---
## 1. Purpose & Scope
Define the theming system (light/dark) for the modernized Tic Tac Toe UI, including CSS variable tokens, semantic roles, toggle behavior, persistence, and accessibility constraints.

### 1.1 In-Scope
- Light & dark base palettes (neutral + blue accent)
- Semantic token mapping layer
- Theme toggle behavior & persistence
- Win highlight stylistic definition
- Focus ring styling

### 1.2 Out of Scope
- Advanced animation/theming transitions
- User-custom color themes
- High-contrast accessibility theme (future)

---
## 2. Design Principles
- Separation: Raw palette tokens (e.g., `--blue-500`) mapped to semantic tokens (e.g., `--color-accent`)
- Consistency: Same semantic token names across light/dark modes
- Accessibility: Minimum 4.5:1 contrast for text against surfaces
- Minimal Motion: No transitions except inherent CSS color swap (no fade required)

---
## 3. Token Structure

### 3.1 Palette Tokens (Proposed)
Neutral scale (light): `--gray-50`, 100, 150, 200, 300, 400, 500, 600, 700, 800, 900  
Neutral scale (dark): mapped inversely (e.g., dark background uses near `--gray-900`)  
Accent blues: `--blue-50`, 100, 200, 300, 400, 500, 600, 700  
Status (minimal placeholders): `--green-500`, `--red-500`, `--yellow-500`

### 3.2 Semantic Tokens (Light)
| Semantic | Maps To (Example) | Usage |
|----------|------------------|-------|
| --color-background | gray-50 | Body background |
| --color-surface | gray-100 | Nav, cards, board background |
| --color-surface-alt | gray-150 | Hover, subtle strips |
| --color-border | gray-300 | Dividers, outlines |
| --color-text | gray-800 | Primary text |
| --color-text-muted | gray-600 | Secondary text |
| --color-accent | blue-600 | Primary interactive elements |
| --color-accent-hover | blue-500 | Hover state |
| --color-accent-active | blue-700 | Active/pressed state |
| --color-focus-ring | blue-500 | Focus outline color |
| --color-danger | red-500 | Error states (future) |
| --color-success | green-500 | Future success states |
| --color-win-highlight | blue-300 | Win cell background overlay |
| --shadow-elevation-1 | rgba(0,0,0,0.06) 0 1px 2px | Light shadow |

### 3.3 Semantic Tokens (Dark)
| Semantic | Maps To (Example) | Notes |
|----------|------------------|-------|
| --color-background | gray-900 | Body background |
| --color-surface | gray-800 | Primary surfaces |
| --color-surface-alt | gray-700 | Hover layers |
| --color-border | gray-600 | Dividers |
| --color-text | gray-50 | Primary text |
| --color-text-muted | gray-300 | Secondary text |
| --color-accent | blue-500 | Adjusted for legibility |
| --color-accent-hover | blue-400 | Hover state |
| --color-accent-active | blue-600 | Active/pressed state |
| --color-focus-ring | blue-400 | Focus outline |
| --color-danger | red-500 | Consistent |
| --color-success | green-500 | Consistent |
| --color-win-highlight | blue-700 | Slightly luminous on dark |
| --shadow-elevation-1 | rgba(0,0,0,0.4) 0 1px 3px | Elevated feel |

### 3.4 Typography & Spacing (Shared Tokens)
| Token | Example | Purpose |
|-------|---------|---------|
| --font-family-base | system-ui, sans-serif | Global font stack |
| --font-size-base | 16px | Root rem basis |
| --radius-sm | 4px | Buttons, cells |
| --radius-md | 8px | Drawer corners if needed |
| --space-1 | 4px | Minor gap |
| --space-2 | 8px | Small gap |
| --space-3 | 12px | Medium gap |
| --space-4 | 16px | Standard padding |
| --space-6 | 24px | Layout spacing |
| --space-8 | 32px | Larger layout spacing |
| --transition-none | 0ms | No motion standard |

---
## 4. Application & CSS Structure
- Root `<html>` or `<body>` gets `data-theme="light"` or `data-theme="dark"`
- Base palette tokens defined once (light context); dark overrides inside `[data-theme="dark"]` scope
- Semantic tokens referenced by components only (avoid direct palette references)

### 4.1 Example Structure
```css
:root {
  --gray-50:#f8fafc; --gray-100:#f1f5f9; --gray-150:#e9eef4; --gray-200:#e2e8f0; --gray-300:#cbd5e1; --gray-400:#94a3b8; --gray-500:#64748b; --gray-600:#475569; --gray-700:#334155; --gray-800:#1e293b; --gray-900:#0f172a;
  --blue-50:#eff6ff; --blue-100:#dbeafe; --blue-200:#bfdbfe; --blue-300:#93c5fd; --blue-400:#60a5fa; --blue-500:#3b82f6; --blue-600:#2563eb; --blue-700:#1d4ed8;
  --red-500:#ef4444; --green-500:#22c55e; --yellow-500:#eab308;
  --font-family-base: system-ui, sans-serif;
  --font-size-base:16px;
  --radius-sm:4px; --radius-md:8px;
  --space-1:4px; --space-2:8px; --space-3:12px; --space-4:16px; --space-6:24px; --space-8:32px;
  --transition-none:0ms;
}
:root[data-theme="light"] {
  --color-background: var(--gray-50);
  --color-surface: var(--gray-100);
  --color-surface-alt: var(--gray-150);
  --color-border: var(--gray-300);
  --color-text: var(--gray-800);
  --color-text-muted: var(--gray-600);
  --color-accent: var(--blue-600);
  --color-accent-hover: var(--blue-500);
  --color-accent-active: var(--blue-700);
  --color-focus-ring: var(--blue-500);
  --color-danger: var(--red-500);
  --color-success: var(--green-500);
  --color-win-highlight: var(--blue-300);
  --shadow-elevation-1: 0 1px 2px rgba(0,0,0,0.06);
}
:root[data-theme="dark"] {
  --color-background: var(--gray-900);
  --color-surface: var(--gray-800);
  --color-surface-alt: var(--gray-700);
  --color-border: var(--gray-600);
  --color-text: var(--gray-50);
  --color-text-muted: var(--gray-300);
  --color-accent: var(--blue-500);
  --color-accent-hover: var(--blue-400);
  --color-accent-active: var(--blue-600);
  --color-focus-ring: var(--blue-400);
  --color-danger: var(--red-500);
  --color-success: var(--green-500);
  --color-win-highlight: var(--blue-700);
  --shadow-elevation-1: 0 1px 3px rgba(0,0,0,0.4);
}
```

---
## 5. Theme Toggle Behavior
| Behavior | Detail |
|----------|--------|
| Initial Load | Detect localStorage key `ttt-theme`; if absent, read `window.matchMedia('(prefers-color-scheme: dark)')` |
| Allowed Values | `light`, `dark` |
| Persistence | localStorage setItem on each toggle |
| FOUC Mitigation | Inline script in `index.html` sets data-theme ASAP (optional) |
| Accessibility | Toggle has `aria-label` describing target state |

---
## 6. Focus Ring Specification
- Outline: 2px solid `var(--color-focus-ring)`
- Offset: 2px (outline-offset)
- Applied only on `:focus-visible`

---
## 7. Game Board Theming Specifics
- Empty Cell Background: `--color-surface`
- Hover Cell: `--color-surface-alt`
- X / O Color: Use `--color-accent` for both marks (distinction via shape/style only) OR (future) introduce separate secondary token
- Win Highlight: Overlay background or border using `--color-win-highlight` with enough contrast in both modes

---
## 8. Non-Functional Requirements
- Performance: Variable resolution must not cause layout shift
- Maintainability: All component styles reference semantic tokens
- Extensibility: Additional themes can layer new palette tokens without changing semantics

---
## 9. Risks & Mitigations
| Risk | Impact | Mitigation |
|------|--------|------------|
| Insufficient contrast in dark mode | Accessibility failure | Validate with tooling (e.g., axe) |
| Flash of incorrect theme | UX inconsistency | Inline set data-theme early |
| Direct color hard-coding in components | Inconsistent theme | Code review to enforce token usage |

---
## 10. Acceptance Criteria
- Theme toggle switches instantly with no animation
- LocalStorage value reflects last chosen theme
- Semantic tokens apply consistently across components
- Win highlight visually distinct in both modes
- Focus ring visible and meets contrast requirements

---
End of spec-ui-theme_v0.01.md
