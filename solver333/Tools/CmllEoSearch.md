# Exact CMLLEO + LSE search

## Objective

For every state immediately after second block, choose a sequence that reaches a
valid CMLLEO boundary and minimizes

```
length(CMLLEO sequence) + exactLseDistance(resulting boundary)
```

The second term comes from `Data/lse-policy-v1.dat`.  This is deliberately not
the lexicographic objective “shortest CMLLEO, then shortest LSE”: a longer
CMLLEO sequence may win when it leaves a better LSE.

## State boundaries

The input boundary has both Roux blocks solved.  Its free coordinates are:

- the four top corners: `4! * 3^3 = 648` states;
- the six LSE edges plus the M-slice center offset: `46,080` reachable states.

The complete boundary therefore contains `648 * 46,080 = 29,859,840` states.
Corner AUF is not stored twice: it belongs to the corner coordinate until the
CMLLEO boundary is reached, then selects the corresponding AUF coordinate in
the existing LSE policy.

For generation, factor this as 82,944 abstract inputs
(`648 corners * 32 edge-flip patterns * 4 center offsets`) with 360
parity-compatible LSE edge permutations apiece. Search CMLLEO paths once for
an abstract input and score each terminal edge transformation against all 360
permutations. Running IDA* independently 29,859,840 times is explicitly not
the generation strategy.

A terminal state must have both blocks restored, the top corners solved up to
AUF, all six LSE edges oriented, and the centers oriented according to
RouxLab's existing convention (U center is white or yellow).  There are 2,880
such terminal LSE states.  Their exact remaining LSE distances range from 0 to
12 STM.

## Search and proof strategy

1. Generate compact cubie move transforms from the canonical F# cube model.
   The generated transforms are checked against `Cube.executeMove`; the native
   search does not maintain an independent hand-written cube convention.
2. Build admissible pruning tables for corners, block restoration, and EO.  In
   the v1 `U/R/r/F` alphabet the DBL block corner never moves, so the exact
   corner-search orbit is `7! * 3^6 = 3,674,160` states.  The generator asserts
   that count; the larger unrestricted eight-corner coordinate remains a sparse
   address space only.  The corresponding six-block-edge projection has 80,640
   reachable states (inside a sparse 42,577,920-slot coordinate) and maximum
   restoration distance 9 STM.  Edge orientation and the M-axis center offset
   are coupled; their terminal-seeded projection reaches 2,048 states rather
   than the raw `2,048 * 4` product.
3. Enumerate block-restoring CMLLEO paths with IDA*, pruning consecutive turns
   on the same axis and retaining every distinct terminal LSE transformation
   that can still improve the combined score.
4. Evaluate each terminal with the exact LSE distance table. The first policy
   version retains one deterministic optimal first move. An optional later pass
   may retain every equal choice, but that is not required for optimal length.
5. Exhaustively verify all 29,859,840 boundary states.  Any uncovered state is
   a generator failure; there is no fallback to the hand-authored CMLL or EO
   banks.

The first implementation uses a versioned move alphabet.  A generated policy
is “God mode” only within that alphabet and STM metric.  Changing either
invalidates the policy version and requires a full rebuild.

## Artifacts

The generator writes a resumable work file while searching and publishes an
immutable policy only after exhaustive verification.  The final browser file
will contain a header (magic, version, alphabet hash, state count, maximum
distance), a distance byte, and a deterministic optimal next move per state.

Long generation runs must write periodic progress to a log.  They should be
checked infrequently; correctness checks happen at deterministic checkpoints
and at finalization rather than through active polling.

## Human-algorithm approximation now integrated in RouxLab

The exact search described above is still unfinished, but RouxLab now has a
separate, honestly labelled **CMLL + EO influence** mode.  It answers a useful
intermediate question: given a finite library of CMLL/COLL algorithms with
different effects on the six edges, which available corner solution produces
the shortest complete continuation?

This mode uses the same combined objective as the exact search:

```
candidate CMLL length + exactLseDistance(candidate result)
```

It is therefore more than “prefer an EO-preserving CMLL.”  Preserving EO is
not always the best choice, and deliberately changing EO may produce a shorter
complete LSE.  The exact LSE table is the evaluator, so no hand-authored EO
classification has to be trusted.

### Candidate sources

The experimental bank in
`Patterns/Roux/Experimental/CmllEoCandidates.txt` contains the 92 distinct
algorithms displayed on the SpeedcubeQuest CMLL overview as of 2026-09-27:

- <https://speedcube.quest/algorithms/3x3/cmll>

That page exposes commonly used alternatives for all named CMLL cases and
marks some algorithms as COLL.  A COLL label is useful provenance—COLL
preserves the orientation of every edge—but RouxLab does not use the label as
solver truth.  It executes and classifies the sequence with the canonical cube
model instead.

Kian Mansour's CMLL sheet is also important prior art because its stated
organization includes, per case, an EO-preserving algorithm and an
edge-flipping algorithm:

- <https://www.cuberoot.me/wp-content/uploads/2019/11/453-Roux-CMLL-1.pdf>

The site challenged automated PDF retrieval, so that sheet was not silently
or partially imported.  It remains a strong future source if it can be
acquired and reviewed reliably.  The broader idea has appeared under the names
KCLL, CLLEO, and CMLLEO: learn multiple corner algorithms for the same case and
choose according to their edge effect.

The 92 imported alternatives are merged with RouxLab's existing, complete
42-case one-look CMLL bank.  The complete bank is a first-class part of the
scored candidate set, not a recovery path used only after another bank fails.

### Import validation

External case names and EO annotations are never trusted.  For every imported
algorithm RouxLab:

1. parses it with the same notation parser used by the solver;
2. applies its inverse to the solved cube to construct the exact case that the
   algorithm reverses;
3. calls the LSE boundary indexer on that case, which proves that both Roux
   blocks are still solved and that the remaining pieces are in the valid LSE
   orbit;
4. reapplies the algorithm and requires the result to be the solved cube.

Any malformed or block-breaking entry aborts validation with the offending
algorithm.  `Tools/TestCmllEoCandidates.fsx` currently validates all 92
entries.  The source and validation rule are documented beside the bank in
`Patterns/Roux/Experimental/CmllEoCandidates.md`.

### Runtime selection

For a concrete post-second-block cube, the solver constructs one explicit
candidate set:

- every matching algorithm from the complete built-in CMLL patterns, including
  the AUFs generated by `expandPatternsForAuf`;
- each of the 92 imported alternatives with each of the four U setups.

It removes duplicate move sequences, applies every candidate, and retains only
results that actually satisfy the CMLL corner goal.  For each surviving result
it evaluates the exact relative LSE distance from `lse-policy-v1.dat`, then
orders candidates by:

1. combined CMLL-plus-LSE length;
2. CMLL length as the deterministic tie-breaker.

The winning CMLL is emitted as the `CMLLEO` stage.  It is followed by the exact
`OptimalLSE` continuation that was used in its score.  The UI consequently
requires God-mode LSE while this corner mode is selected; it cannot display a
human LSE continuation that disagrees with the candidate comparison.

If the explicit union produces no valid candidate, the solver fails with the
exact uncovered cube state.  It never falls back to two-look CMLL, a generic
search, or a different corner method.

### Coverage lesson

The first implementation tried the 92 overview algorithms alone.  Regression
testing exposed this uncovered state:

```
OYOOOOBYBYRYWYYYOYRORGYGOBOBBBRRRGGGBBBRWRGGGWRWWWWWGW
```

This was treated as a bank-coverage bug, not patched with a hidden fallback.
The overview is a useful collection of alternatives, but it is not by itself
a complete executable CMLL bank under RouxLab's exact orientation and matching
conventions.  Making the already exhaustive 42-case bank part of the same
candidate union restored complete baseline coverage while still allowing any
imported alternative to win on combined distance.

This is the general rule for future imports: an algorithm collection's case
count or marketing description is not a coverage proof.  Coverage belongs to
the executable cube model and must be tested there.

### UI and persistence

`cmll = 2` selects the new corner mode.  It is mutually exclusive with
beginner corners, two-look CMLL, and ordinary one-look CMLL.  Selecting it also
sets `lse = 1`; the exact-LSE switch is shown as required.  Both values use the
existing `localStorage` configuration and therefore survive page reloads and
browser sessions.  The Optimal preset selects this mode and exact LSE.

The UI calls it **CMLL + EO influence**, not God CMLLEO.  Its help text states
that it tries 92 published alternatives and minimizes against exact remaining
LSE, but is not globally optimal CMLLEO.

### Verification completed

The integrated implementation has passed:

- .NET builds of both `BrowserSolver.fsproj` and `Solver.fsproj` with no
  warnings;
- validation of all 92 imported algorithms;
- six complete browser-solver regressions covering ordinary one-look CMLL and
  CMLL + EO influence, with every generated solution replayed to the solved
  cube;
- an assertion that the new mode emits exactly one `CMLLEO` stage and one
  exact `OptimalLSE` stage;
- the cancellation, direct-L4E, EOLR, and generated-browser-module tests;
- loading from a plain static server at `/site/lab/`, including all relative,
  versioned assets required by GitHub Pages.

### What this does not prove

This mode is exact only over its finite candidate union.  It does not prove
that its CMLL sequence is the shortest sequence that could solve the corners,
orient the edges, or minimize the combined finish.  It may miss:

- unpublished or unimported CMLL alternatives;
- longer corner algorithms whose unusually favorable LSE state wins overall;
- sequences that temporarily disturb more block pieces than the human library
  algorithms and restore them later;
- globally optimal paths in the full versioned `U/R/r/F` search alphabet.

Those gaps are exactly what the exhaustive 29,859,840-state policy is intended
to close.  Until that policy exists and passes exhaustive verification, the
human-library mode remains a strong measured approximation and a useful source
of data about how often EO-aware CMLL choice changes the best continuation.

## Phase-two measurements

The first complete 360-permutation abstract batch is now proven. It required
2,067,343,732 visited nodes and 297.82 seconds on the development machine. Its
combined optimum ranged from 11 through 16 STM, averaging 14.555556. A direct
extrapolation over 82,944 abstractions is therefore prohibited; it would take
years rather than producing a useful offline build.

A weighted reverse search seeded by all 2,880 CMLLEO terminals was also
measured. Unique discovered states grow as follows:

- through cost 4: 96,999;
- through cost 5: 769,872;
- through cost 6: 6,234,176;
- expanding cost 7 exceeds 16.8 million hash slots.

Keeping the 6.23-million-state cost-7 reverse half did not improve the hard
forward cases: random access across the reverse table, transposition table, and
three pruning tables became memory-latency bound. Do not scale either rejected
formulation unchanged.

The remaining scalable candidates are a symbolic stabilizer search that
propagates many abstract inputs together, or a checkpointed/distributed build.
Any implementation must first beat the complete 360-state benchmark by orders
of magnitude before an exhaustive run is authorized.
