# Managed Capabilities Protocol (.mcp.json) Setup

The `.mcp.json` file configures *Managed Capabilities Protocol* servers that extend AI assistants (e.g. GitHub Copilot / compatible Visual Studio agents) with domain or tool specific capabilities. Projects should add this file at the repository root to standardise assistant behaviour.

The `.mcp.json` can also be copied to the users home directory to apply across all repositories.

## Purpose
- Declare available tool providers (GitHub, Azure DevOps, Microsoft Docs search, internal services)
- Standardise automation / knowledge endpoints for contributors
- Provide a controlled, auditable capability surface

## Placement & Scope
- Location: repository root (alongside solution files if .NET)
- Committed so all contributors share the same configuration
- Updated only through a reviewed pull request

## Illustrative Structure
```jsonc
{
  "$schema": "https://example/schema/mcp.json", // replace with actual schema if supplied
  "servers": {
    "github": { "type": "builtin", "scopes": ["repo", "workflow"] },
    "microsoftDocs": { "type": "builtin", "scopes": ["read"] }
  },
  "settings": { "defaultTimeoutSeconds": 30 }
}
```
Adapt servers & scopes to organisational policy. Add internal endpoints (e.g. knowledge base, static analysis) only when justified.

## Usage Flow
1. Developer opens repo with AI features enabled.
2. Assistant loads declared MCP servers.
3. Prompt workflows (e.g. spec research, refactor planning) can invoke those capabilities.
4. Changes to `.mcp.json` are reviewed for principle of least privilege.

## Good Practices
- Keep configuration minimal & documented (comment unusual entries)
- Remove obsolete servers promptly
- Document internal-only additions in `mcp-extensions.md` (name, purpose, auth method)

## When to Update
- New required capability for an approved workflow
- Decommissioned server removal
- Scope reduction for security hardening

## Note
- Visual Studio 2022 only support 128 concurrent tools turned on so you will have to turn some mcp tools off to ensure the number does not exceed 128.

*Last updated: 2025-09-12*