#!/usr/bin/env bash
#
# repro-netstandard20.sh — Demonstrate NU1701 when a netstandard2.0 consumer
# references StrongGrid 0.114.0 (which ships only net48 + netstandard2.1).
#
# Requirements: dotnet, curl, unzip
# Usage: ./repro/repro-netstandard20.sh

set -euo pipefail

REPO_ROOT="$(cd "$(dirname "$0")/.." && pwd)"
TMPDIR="$(mktemp -d)"
trap 'rm -rf "$TMPDIR"' EXIT

echo "=============================================="
echo " StrongGrid netstandard2.0 TFM repro script"
echo "=============================================="
echo ""
echo "Temp directory: $TMPDIR"
echo ""

# ─── Section 1: Package inspection ──────────────────────────────────────────

echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo " Section 1: Package inspection"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""

echo "▸ Downloading published StrongGrid 0.114.0 nupkg..."
curl -sL https://api.nuget.org/v3-flatcontainer/stronggrid/0.114.0/stronggrid.0.114.0.nupkg \
  -o "$TMPDIR/stronggrid-published.nupkg"

echo ""
echo "▸ TFMs in published 0.114.0:"
unzip -l "$TMPDIR/stronggrid-published.nupkg" | grep "lib/" | grep -v "/$" || true
echo ""
echo "  → Only net48 + netstandard2.1. No netstandard2.0."
echo "  → NuGet falls back to net48 for netstandard2.0 consumers."
echo ""

echo "▸ Packing current branch locally..."
# Use Debug config to avoid StyleCop TreatWarningsAsErrors in Release.
# We only need the nupkg to inspect TFM layout, not a production build.
if dotnet pack "$REPO_ROOT/Source/StrongGrid/StrongGrid.csproj" \
  -c Debug \
  -o "$TMPDIR/local-pack" \
  /p:SemVer=99.0.0-repro \
  --nologo -v quiet 2>&1 | tail -3; then
  echo ""
  echo "▸ TFMs in locally packed nupkg:"
  unzip -l "$TMPDIR/local-pack/StrongGrid.99.0.0-repro.nupkg" | grep "lib/" | grep -v "/$" || true
else
  echo ""
  echo "  ⚠ Local pack failed. Skipping TFM listing."
fi
echo ""
echo "  → Should now include netstandard2.0."
echo ""

# ─── Section 2: Consumer build test ─────────────────────────────────────────

echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo " Section 2: Consumer build test"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "Building a minimal netstandard2.0 class library that references StrongGrid."
echo ""

CONSUMER_DIR="$TMPDIR/consumer"
mkdir -p "$CONSUMER_DIR"

# Minimal class that actually uses a StrongGrid type
cat > "$CONSUMER_DIR/Class1.cs" << 'CSEOF'
using StrongGrid;

namespace ConsumerLib
{
    public class Class1
    {
        public Client GetClient() => new Client("fake-api-key");
    }
}
CSEOF

# Helper: count StrongGrid-specific NU1701 warnings (not transitive deps like HttpMultipartParser)
count_stronggrid_warnings() {
  local count
  count=$(grep "warning NU1701" "$1" 2>/dev/null | grep -ic "Package 'StrongGrid" 2>/dev/null) || true
  echo "${count:-0}"
}
count_all_nu1701() {
  local count
  count=$(grep -c "warning NU1701\|warning MSB3277" "$1" 2>/dev/null) || true
  echo "${count:-0}"
}

# ── Test A: Published 0.114.0 ──

echo "▸ Test A: Build against published StrongGrid 0.114.0"
echo ""

cat > "$CONSUMER_DIR/consumer.csproj" << 'PROJEOF'
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="StrongGrid" Version="0.114.0" />
  </ItemGroup>
</Project>
PROJEOF

BUILD_OUTPUT_A="$TMPDIR/build-output-a.txt"
dotnet build "$CONSUMER_DIR/consumer.csproj" --nologo 2>&1 | tee "$BUILD_OUTPUT_A"
echo ""

SG_WARNINGS_A=$(count_stronggrid_warnings "$BUILD_OUTPUT_A")
ALL_WARNINGS_A=$(count_all_nu1701 "$BUILD_OUTPUT_A")
if [ "$SG_WARNINGS_A" -gt 0 ]; then
  echo "  ⚠ Found NU1701 warning for StrongGrid package:"
  grep "warning NU1701" "$BUILD_OUTPUT_A" | grep -i "StrongGrid" | sort -u | sed 's/^/    /'
else
  echo "  ✓ No StrongGrid NU1701 warning (unexpected for 0.114.0)."
fi
if [ "$ALL_WARNINGS_A" -gt "$SG_WARNINGS_A" ]; then
  echo "  (Also $((ALL_WARNINGS_A - SG_WARNINGS_A)) transitive NU1701 warning(s) for other packages)"
fi
echo ""

# Clean restore caches for this consumer project before switching packages
dotnet nuget locals http-cache --clear > /dev/null 2>&1 || true
rm -rf "${CONSUMER_DIR:?}/bin" "${CONSUMER_DIR:?}/obj"

# ── Test B: Locally packed (with netstandard2.0 TFM) ──

echo "▸ Test B: Build against locally packed StrongGrid (with netstandard2.0)"
echo ""

# Create a local NuGet source pointing at our local pack
cat > "$CONSUMER_DIR/nuget.config" << NUGETEOF
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="local" value="$TMPDIR/local-pack" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
NUGETEOF

cat > "$CONSUMER_DIR/consumer.csproj" << 'PROJEOF'
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="StrongGrid" Version="99.0.0-repro" />
  </ItemGroup>
</Project>
PROJEOF

BUILD_OUTPUT_B="$TMPDIR/build-output-b.txt"
dotnet build "$CONSUMER_DIR/consumer.csproj" --nologo 2>&1 | tee "$BUILD_OUTPUT_B"
echo ""

SG_WARNINGS_B=$(count_stronggrid_warnings "$BUILD_OUTPUT_B")
ALL_WARNINGS_B=$(count_all_nu1701 "$BUILD_OUTPUT_B")
if [ "$SG_WARNINGS_B" -gt 0 ]; then
  echo "  ⚠ StrongGrid NU1701 warning still present (unexpected with fix):"
  grep "warning NU1701" "$BUILD_OUTPUT_B" | grep -i "StrongGrid" | sort -u | sed 's/^/    /'
else
  echo "  ✓ No StrongGrid NU1701 warning — netstandard2.0 TFM resolved correctly."
fi
if [ "$ALL_WARNINGS_B" -gt 0 ] && [ "$SG_WARNINGS_B" -eq 0 ]; then
  echo "  (HttpMultipartParser NU1701 is expected — it also lacks netstandard2.0)"
fi
echo ""

# ─── Optional: dotnet-inspect analysis ───────────────────────────────────────

if command -v dotnet-inspect > /dev/null 2>&1; then
  echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
  echo " Bonus: dotnet-inspect analysis"
  echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
  echo ""
  echo "▸ Querying published 0.114.0 for netstandard2.0 TFM:"
  dotnet-inspect library --package StrongGrid@0.114.0 --tfm netstandard2.0 --references 2>&1 || true
  echo ""
  echo "▸ Querying published 0.114.0 for net48 TFM (fallback path):"
  dotnet-inspect library --package StrongGrid@0.114.0 --tfm net48 --references 2>&1 || true
  echo ""
  echo "  → Note the System.Net.Http version. The net48 build references 4.2.0.0"
  echo "    which conflicts with the netstandard2.0 reference assembly (4.1.2.0)."
  echo ""
else
  echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
  echo " Bonus: dotnet-inspect (skipped — not installed)"
  echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
  echo ""
  echo "  Install with: dotnet tool install -g dotnet-inspect"
  echo "  See: https://github.com/richlander/dotnet-inspect"
  echo ""
fi

# ─── Summary ─────────────────────────────────────────────────────────────────

echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo " Summary"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "  Published 0.114.0  → StrongGrid NU1701: $SG_WARNINGS_A"
echo "  Local pack (fixed) → StrongGrid NU1701: $SG_WARNINGS_B"
echo ""

if [ "$SG_WARNINGS_A" -gt 0 ] && [ "$SG_WARNINGS_B" -eq 0 ]; then
  echo "  ✓ Repro confirmed: adding netstandard2.0 TFM resolves the StrongGrid NU1701."
elif [ "$SG_WARNINGS_A" -eq 0 ]; then
  echo "  ⚠ Published 0.114.0 did not produce StrongGrid NU1701 in this environment."
  echo "    The warning may depend on the consumer's SDK version or restore behavior."
else
  echo "  ⚠ Local pack still has StrongGrid NU1701 — investigate build output above."
fi

echo ""
echo "Temp files cleaned up automatically."
