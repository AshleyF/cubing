# Solver invariants

- Treat every uncovered solver state as a correctness bug in the pattern set. Do not silently fall back to another method, pair order, stage, or generic search.
- Pattern-set options must either cover every state admitted by their documented preconditions or fail loudly with the exact uncovered state.
- The shared pattern matcher must never recover an unmatched state with cached hints or generic search. Those mechanisms hide incorrect bank selection and coverage holes.
- Benchmarks are valid only after an exhaustive coverage check succeeds for every enabled pattern set. Never omit, replace, or retry failed cases in statistics.
- Pair-order pattern sets have distinct preconditions. Back-first patterns cannot stand in for front-first patterns; generate and validate front-first and corresponding last-pair sets explicitly.

# RouxLab architecture and continuation notes

- This repository is the only canonical home for RouxLab. The old standalone Sites repository was accidental and was emptied after migration. Do not recreate a nested Git repository under `site/lab`.
- GitHub Pages serves the app at `https://ashleyf.github.io/cubing/site/lab/`. All browser asset URLs must remain relative so the `/cubing/site/lab/` base path works.
- RouxLab is a fully static application. GitHub Pages cannot run Node, .NET, or `/api/*` endpoints. Do not reintroduce a server dependency.
- The production solver remains the F# implementation in `library/` and `solver333/`. `solver333/BrowserSolver.fsproj` links those sources and Fable compiles them to `site/lab/solver/`.
- `site/lab/solver-worker.js` loads the compiled solver and runs every solve off the UI thread. `site/lab/app.js` uses separate foreground and option-comparison workers. Settings changes must cancel the previous foreground request, regenerate the current scramble immediately, and leave the previous result visible until stable milestones from the replacement arrive.
- The solve-search UI uses its own disposable worker and evaluates random scrambles sequentially with a frozen snapshot of the current solver settings. Changing the scramble or settings cancels the search. Search ranking lives in `searchObjectives`; keep new goals such as shortest CMLL or stage skips there rather than duplicating the runner. A completed search adopts the already-computed winning result and then restarts ordinary option comparison.
- Foreground solves stream stable FB, SB, CMLL, EO/EOLR, LSE, and L4E milestones. Color neutrality evaluates only the first block for all eight x2/y orientations, then completes only the winning orientation.
- External pattern text files are canonical. `solver333/Tools/GenerateBrowserPatternData.mjs` packages them as `site/lab/pattern-data.json`; it does not implement a second solver.
- Run `site/lab/build.sh` after changing F# solver code or pattern files. It rebuilds the .NET solver, browser solver, embedded pattern data, and static pattern explorer catalog.
- The pattern explorer reads `site/lab/patterns.json` and `site/lab/eolr-cases.json`; it must never call a local API.
- `solver333/Tools/Benchmark.fsx` produces `site/lab/benchmark-results.json`. The report uses the same deterministic 1,000 scrambles per configuration (seed 3332026).
- Inspection rotations (`ColorNeutralOrientation` and `DLEdge`) stay in the playable algorithm but count as zero moves in UI statistics.
- Move cancellations are a browser-side post-processing option and default to enabled.
- Solver switches and the move-cancellation preference persist in browser `localStorage`; restore them before generating the initial solve.
- The Beginner and Optimal preset buttons apply complete configurations, persist them, and immediately regenerate the current scramble. Keep the solver title configuration-neutral.
- Direct L4E is the baseline L4E pattern set. The repetitive `M' U2` beginner bank is retained only for historical comparisons and must not be selected by the web solver.
- EOLR must use `lrIntermediatePatterns`, never the beginner LR bank. Direct LR patterns overlap, so select the shortest explicitly matched action that actually satisfies the LR goal; never generic-search an uncovered state.
- First- and second-block “choose shorter pair order” compare only the two ways of completing that block’s two pairs. Color neutrality compares only first-block cost. Neither may look ahead into later stages.
- Before pushing solver changes, verify the .NET build, rebuild the static solver, serve the repository root with a plain static server, and test `site/lab/` without `server.mjs`.

# Exact CMLLEO + LSE work in progress

- `solver333/Tools/CmllEoSearch.md` is the design contract. The objective is CMLLEO move count plus the exact remaining distance from `lse-policy-v1.dat`, not shortest CMLLEO in isolation.
- The v1 search alphabet is `U/R/r/F` with all powers in STM. Never describe a resulting policy as unrestricted cube-optimal; it is exact within that versioned alphabet.
- `GenerateCmllEoMoveData.fsx` derives cubie and facelet transforms from the canonical F# cube model and performs 1,000 randomized equivalence checks. Do not hand-maintain a second move convention in C.
- `CmllEoKernel.c` currently builds and verifies three admissible pruning tables and proves a weighted optimum for a deterministic sample. `BuildCmllEoSearch.sh WORK_DIRECTORY` reproduces those artifacts. The large PDBs belong in a temporary work directory and are not browser assets.
- No exhaustive CMLLEO policy exists yet. RouxLab's separate `CMLL + EO influence` option is an explicitly non-exact approximation: it merges the complete built-in 42-case CMLL bank with 92 published CMLL/COLL alternatives, validates block preservation, tries applicable AUFs, and minimizes candidate STM plus exact remaining LSE distance. It always uses the exact LSE continuation it scores. The candidate source and provenance live under `solver333/Patterns/Roux/Experimental/`.
- The next exact-generator phase must batch the 82,944 corner/edge-orientation/center abstract inputs and evaluate the 360 parity-compatible LSE edge permutations together; do not run weighted IDA* independently for all 29,859,840 boundary states.
- The v1 combined policy stores one deterministic optimal move. Preserving every tied first move is optional and must not make initial exhaustive generation intractable.
- Phase-two benchmark: one exact 360-permutation batch took 2,067,343,732 nodes / 297.82 seconds (costs 11–16, average 14.555556). Do not extrapolate this across 82,944 abstractions. A 6.23-million-state reverse meet table was memory-latency bound and was also rejected. See `CmllEoSearch.md` before attempting the next generator architecture.
