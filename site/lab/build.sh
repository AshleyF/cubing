#!/usr/bin/env bash
set -euo pipefail

lab_dir="$(cd "$(dirname "$0")" && pwd)"
repo_dir="$(cd "$lab_dir/../.." && pwd)"
solver_dir="$repo_dir/solver333"

node "$solver_dir/Tools/GenerateBrowserPatternData.mjs"
dotnet build "$solver_dir/BrowserSolver.fsproj" -c Release --no-restore --disable-build-servers
dotnet build "$solver_dir/Solver.fsproj" -c Release --no-restore --disable-build-servers
"$HOME/.dotnet/tools/fable" "$solver_dir/BrowserSolver.fsproj" --outDir "$lab_dir/solver" --noRestore --noCache --optimize

# Fable writes a blanket ignore file beside its runtime. Keep JavaScript modules
# versioned because GitHub Pages has no package installation/build step.
printf '%s\n' '*' '!*/' '!*.js' > "$lab_dir/solver/fable_modules/.gitignore"
node "$solver_dir/Tools/CheckBrowserModules.mjs"

catalog="$({ cd "$solver_dir"; dotnet bin/Release/net8.0/Solver.dll --patterns; } | sed -n 's/^PATTERN_RESULT|//p')"
if [[ -z "$catalog" ]]; then
  echo "Pattern catalog generation failed." >&2
  exit 1
fi
printf '%s\n' "$catalog" > "$lab_dir/patterns.json"

echo "Built static RouxLab solver in $lab_dir"
