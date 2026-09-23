# Solver invariants

- Treat every uncovered solver state as a correctness bug in the pattern set. Do not silently fall back to another method, pair order, stage, or generic search.
- Pattern-set options must either cover every state admitted by their documented preconditions or fail loudly with the exact uncovered state.
- Benchmarks are valid only after an exhaustive coverage check succeeds for every enabled pattern set. Never omit, replace, or retry failed cases in statistics.
- Pair-order pattern sets have distinct preconditions. Back-first patterns cannot stand in for front-first patterns; generate and validate front-first and corresponding last-pair sets explicitly.

# RouxLab architecture and continuation notes

- This repository is the only canonical home for RouxLab. The old standalone Sites repository was accidental and was emptied after migration. Do not recreate a nested Git repository under `site/lab`.
- GitHub Pages serves the app at `https://ashleyf.github.io/cubing/site/lab/`. All browser asset URLs must remain relative so the `/cubing/site/lab/` base path works.
- RouxLab is a fully static application. GitHub Pages cannot run Node, .NET, or `/api/*` endpoints. Do not reintroduce a server dependency.
- The production solver remains the F# implementation in `library/` and `solver333/`. `solver333/BrowserSolver.fsproj` links those sources and Fable compiles them to `site/lab/solver/`.
- `site/lab/solver-worker.js` loads the compiled solver and runs every solve off the UI thread. `site/lab/app.js` uses separate foreground and option-comparison workers. Settings changes must cancel the previous foreground request, regenerate the current scramble immediately, and leave the previous result visible until stable milestones from the replacement arrive.
- Foreground solves stream stable FB, SB, CMLL, EO/EOLR, LSE, and L4E milestones. Color neutrality evaluates only the first block for all eight x2/y orientations, then completes only the winning orientation.
- External pattern text files are canonical. `solver333/Tools/GenerateBrowserPatternData.mjs` packages them as `site/lab/pattern-data.json`; it does not implement a second solver.
- Run `site/lab/build.sh` after changing F# solver code or pattern files. It rebuilds the .NET solver, browser solver, embedded pattern data, and static pattern explorer catalog.
- The pattern explorer reads `site/lab/patterns.json` and `site/lab/eolr-cases.json`; it must never call a local API.
- `solver333/Tools/Benchmark.fsx` produces `site/lab/benchmark-results.json`. The report uses the same deterministic 1,000 scrambles per configuration (seed 3332026).
- Inspection rotations (`ColorNeutralOrientation` and `DLEdge`) stay in the playable algorithm but count as zero moves in UI statistics.
- Move cancellations are a browser-side post-processing option and default to enabled.
- Solver switches and the move-cancellation preference persist in browser `localStorage`; restore them before generating the initial solve.
- Direct L4E is the baseline L4E pattern set. The repetitive `M' U2` beginner bank is retained only for historical comparisons and must not be selected by the web solver.
- First- and second-block “choose shorter pair order” compare only the two ways of completing that block’s two pairs. Color neutrality compares only first-block cost. Neither may look ahead into later stages.
- Before pushing solver changes, verify the .NET build, rebuild the static solver, serve the repository root with a plain static server, and test `site/lab/` without `server.mjs`.
