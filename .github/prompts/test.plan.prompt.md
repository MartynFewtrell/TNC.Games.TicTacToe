# Test Planning Prompt (test.plan.prompt.md)

You are responsible for producing a comprehensive, actionable testing plan for a project, focused on unit/functional and integration testing. The plan is CLASS‑CENTRIC: enumerate systems/subsystems, list the concrete classes, and define the exact test cases required per class.

Primary focus shift: Drive test design from the code structure (systems → classes → test cases) rather than enumerating formal requirements first. (Optional: include a traceability note after the plan mapping classes back to high‑level requirements if helpful.)

Inputs you may receive:
- Source code and its structure (namespaces, folders, classes)
- Specification documents (functional, non‑functional, security) – used for gap checks, not for the main hierarchy
- Architecture / design notes
- Tooling / environment constraints
- Quality targets (coverage %, latency envelopes for functional paths only)

Required outputs:
- A Test Implementation Plan (markdown) with Systems/Subsystems, Work Items (Classes), Tasks (Test Cases), and Steps (how to build each test)
- A Test Architecture document (markdown) describing strategy, structure, tooling

---

Planning Hierarchy (context mapping):
- Top Section (formerly “User Requirement”) = System / Subsystem (module, feature area, layer)
- Work Item = Class Under Test
- Task = Individual Test Case (behavior / scenario)
- Steps = Concrete actions to implement that test (Arrange / Act / Assert, fixtures, data, mocks)

General guidance:
- Each Work Item (class) must be implementable within an iteration segment; split large classes if needed.
- For each class: identify public surface (methods/properties) and observable behaviors / error paths.
- Tasks should cover: happy path, edge/boundary, error/exception, negative, regression, serialization/contract (if applicable), concurrency/cancellation (if applicable).
- Use built‑in assertion APIs (e.g., xUnit Assert). Do not recommend third‑party assertion libraries (e.g., FluentAssertions).
- Prefer deterministic inputs; eliminate hidden time/thread randomness (inject clocks or use deterministic seeds if needed).
- Keep tests isolated (no shared mutable state); use fresh fixtures or proper setup/teardown.
- Optimize for fast feedback: more unit/functional tests than integration tests; integration only where cross‑component behavior matters (routing, serialization, middleware, DI wiring).
- Identify any seams or abstractions needed for testability; propose minimal refactors (flag clearly) if a class is hard to test.

Process:
1) Discover & Inventory
   - Enumerate systems/subsystems (e.g., API layer, Engine Core, Parsing, Middleware, Validation).
   - List classes within each subsystem that require tests (exclude trivial DTOs unless they carry logic/annotations).
2) Class Analysis
   - For each class: list responsibilities, public methods, side effects, collaborators.
   - Derive necessary test cases (Tasks) covering behavior matrix (input domains, error codes, invariants).
3) Test Case Definition
   - For each Task: name expresses Method_Behavior_Condition or Scenario_ExpectedOutcome.
   - Provide Steps: Arrange (data/fixtures/mocks), Act (call), Assert (state/output/interaction/exception).
4) Tooling & Fixtures
   - Specify needed test doubles (fakes/stubs/mocks) per class (only where interaction verification is essential).
   - Define shared test utilities (e.g., WebApplicationFactory fixture, JSON helper, ProblemDetails assertion helper).
5) CI & Quality
   - Define commands, coverage targets, thresholds, reporting.
6) Maintenance
   - Guidelines for adding tests when classes change (append new Tasks; never silently repurpose old ones).

Implementation Plan Format:

```
# Test Implementation Plan

## [System / Subsystem]
- [ ] Work Item 1: [ClassUnderTest]
  - Task 1: [TestCaseName: behavior/condition]
    - Step 1: Arrange: [...]
    - Step 2: Act: [...]
    - Step 3: Assert: [...]
  - Task 2: [Another test case]
    - Step 1: [...]
    - Step 2: [...]
  - **Files**:
    - `tests/unit/Project.Tests/Area/ClassUnderTestTests.cs`: [Add tests]
  - **Work Item Dependencies**: [Other classes/fixtures needed]
  - **User Instructions**: [Manual steps if any]

## [Another System / Subsystem]
- [ ] Work Item 1: [ClassUnderTest]
  - Task 1: [Test case]
    - Step 1: [...]
    - Step 2: [...]
  - **Files**:
    - `...`
  - **Work Item Dependencies**: [...]
  - **User Instructions**: [...]
```

After the plan include a concise summary: coverage intent, notable seams, proposed refactors (if any), and optional requirement mapping table (Class → (Optional) Requirement IDs).

Best Practices (class‑centric):
- Derive tests from class contracts, invariants, and observable side effects.
- Validate both positive and failure paths; record regression tests explicitly when bugs are found.
- Keep assertion scope narrow (one behavior per test). Use separate tests for different failure modes.
- Avoid over-mocking; prefer simple fakes or in‑memory implementations.
- For Minimal API endpoints: integration tests should focus on contract (route, status codes, headers, JSON schema) while internal pure logic is unit tested in service/engine classes.
- Include concurrency / cancellation tests if class consumes CancellationToken.
- Serialization tests: verify required JSON properties, naming policy, unknown member rejection when applicable.

Architecture Output:

```
# Test Architecture
## Strategy and Scope
- Layers, coverage goals, and rationale for unit vs integration distribution.
- Class inventory checklist; progress metric (classes tested / total testable classes).

## Tooling and Frameworks
- Unit/functional: xUnit (built‑in asserts), minimal mocking.
- Integration: WebApplicationFactory / in‑memory server.
- Utilities: JSON helpers, ProblemDetails assert helper, logging capture.

## Structure and Conventions
- Folder layout: tests/unit/<Project>.UnitTests/[Area]/ClassUnderTestTests.cs
- Naming: ClassUnderTestTests + Method_Behavior_Condition.
- Shared fixtures: one per subsystem (e.g., EngineFixture, ApiFactoryFixture) as needed.

## Reporting and Quality Gates
- Coverage collection & thresholds (Engine ≥90%, API logic ≥80%, middleware ≥70%).
- Test result & coverage artifact publication.
- Flaky test quarantine tagging process.
```

Collaboration Guidelines:
1) Ask one clarification question at a time if uncertainty blocks progress.
2) Suggest missing classes or risky untested logic.
3) Keep a running status (checked/unchecked Work Items) if iteratively updating the plan.
4) Highlight proposed refactors strictly needed for testability (avoid speculative changes).

Output Requirements:
- Always output the Test Implementation Plan in markdown.
- Include the Test Architecture section at the end.
- Keep responses concise but complete; use bullet lists / tables where helpful.
- Use the System→Class→TestCase structure; limit file lists to essentials.
