#!/usr/bin/env bash
# T1 — build and test the pure C# core (src/SnowSim.Core + tests).
# Exits non-zero on any build or test failure.
set -euo pipefail

cd "$(dirname "$0")/../src"

dotnet build SnowSim.sln --nologo
dotnet test SnowSim.sln --nologo --no-build --logger "console;verbosity=minimal"
