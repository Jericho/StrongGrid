# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

StrongGrid is a strongly typed .NET client library for SendGrid's v3 API. It targets **netstandard2.0** and **net8.0**. The SDK version is pinned to .NET 10.0.101 (see `global.json`).

## Build & Test Commands

The build system uses **Cake** (C# Make). On Unix the default target is `Run-Unit-Tests`.

```bash
# Build and run all unit tests (recommended for local dev)
./build.sh

# Run specific Cake target
dotnet cake build.cake --target=Build
dotnet cake build.cake --target=Run-Unit-Tests

# Run a single test by filter (dotnet test directly)
dotnet test Source/StrongGrid.UnitTests/StrongGrid.UnitTests.csproj --filter "FullyQualifiedName~ApiKeysTests.CreateAsync"

# Restore and build via dotnet CLI
dotnet build Source/StrongGrid.slnx

# Code coverage
dotnet cake build.cake --target=Run-Code-Coverage
```

The solution file is `Source/StrongGrid.slnx` (SLNX format). Build artifacts go to `./artifacts/`.

## Architecture

### Resource Pattern
The core pattern is resource-oriented: each SendGrid API resource has an interface/implementation pair in `Source/StrongGrid/Resources/` (e.g., `IContacts`/`Contacts`, `IMail`/`Mail`). Each resource class:
- Takes a `Pathoschild.Http.Client.IClient` via internal constructor
- Defines a `_endpoint` string constant for its API path
- Uses the fluent HTTP client for all requests (`.GetAsync()`, `.PostAsync()`, etc.)
- All methods are async with `CancellationToken` support

### Client Hierarchy
- `BaseClient` — abstract base handling HTTP setup, logging, retry strategy, and filter pipeline
- `Client` — main client aggregating all resource properties (e.g., `client.Contacts`, `client.Mail`)
- `LegacyClient` — legacy API support with a parallel structure
- Both implement corresponding interfaces (`IClient`, `ILegacyClient`)

### Key Directories
- **Resources/** — API resource implementations (~70 files, interface + implementation pairs)
- **Models/** — DTOs, enums, and domain objects for API request/response payloads
- **Json/** — Custom `System.Text.Json` converters and source-generated serializer context (`StrongGridJsonSerializerContext`). Handles SendGrid quirks like epoch timestamps and boolean-as-integer
- **Utilities/** — Cross-cutting concerns: `SendGridException`, retry strategy, diagnostics, attachment handling
- **Extensions/** — `Public.cs` has convenience/DI extension methods; `Internal.cs` has internal helpers
- **Warmup/** — IP warmup engine with pluggable progress repository (file system or in-memory)

### JSON Serialization
Uses `System.Text.Json` with source generation. Two separate `JsonSerializerOptions` instances exist in `JsonFormatter`:
- `DeserializerOptions` — with custom `DateTimeConverter`
- `SerializerOptions` — with `WhenWritingNull` ignore condition

When adding new models, they must be registered in `StrongGridJsonSerializerContext`.

### Dependency Version Constraints
Several packages are pinned to `[8.0.0, 11)` ranges with "DO NOT UPGRADE" comments referencing [GH issue #538](https://github.com/Jericho/StrongGrid/issues/538): `Microsoft.Extensions.Http`, `Microsoft.Extensions.Logging`, `System.Text.Json`.

## Test Conventions

- **Framework**: xUnit v3 with Microsoft Testing Platform
- **Mocking**: NSubstitute for interfaces, RichardSzalay.MockHttp for HTTP
- **Assertions**: Shouldly (`result.ShouldBe(...)`, `result.ShouldNotBeNull()`)
- **Test structure**: Arrange/Act/Assert pattern with blank line separators between sections
- **Test helper**: `Utils.GetFluentClient()` creates a configured FluentClient with MockHttpMessageHandler. `Utils.GetSendGridApiUri()` builds expected API URIs.
- **Test naming**: Method name matches the async method being tested (e.g., `CreateAsync`, `Parse_json`)
- **JSON fixtures**: Defined as `internal const string` fields at class level
- **Unit tests target**: net8.0 and net10.0
- Integration tests exist but are excluded from CI (for local debugging only)

## Code Style

- **Indentation**: Tabs for C# (4-space tab width), spaces for XML/JSON/YAML (2-space)
- **Line endings**: CRLF for most files, LF for shell scripts
- **StyleCop**: Configured via `Source/stylecop.json` and `Source/StyleCopRules.ruleset`
- **Release builds**: `TreatWarningsAsErrors=true` (except 612, 618 for obsolete warnings)
- **XML docs**: Required on public and internal members; not required on private
- **Regions**: Used in classes (`#region FIELDS`, `#region PROPERTIES`, etc.)
- **Using directives**: Outside namespace

## Git Workflow

GitFlow: `develop` (beta), `release/*` (rc), `main` (stable). Semantic versioning via GitVersion. Commit messages use imperative mood.
