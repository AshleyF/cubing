# Solver invariants

- Treat every uncovered solver state as a correctness bug in the pattern set. Do not silently fall back to another method, pair order, stage, or generic search.
- Pattern-set options must either cover every state admitted by their documented preconditions or fail loudly with the exact uncovered state.
- Benchmarks are valid only after an exhaustive coverage check succeeds for every enabled pattern set. Never omit, replace, or retry failed cases in statistics.
- Pair-order pattern sets have distinct preconditions. Back-first patterns cannot stand in for front-first patterns; generate and validate front-first and corresponding last-pair sets explicitly.
