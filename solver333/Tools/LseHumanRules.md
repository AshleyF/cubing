# Mining human-readable rules from the exact LSE policy

RouxLab has an exact policy for the Roux last six edges under the move set
`M`, `M'`, `M2`, `U`, `U'`, and `U2`. The policy is an excellent solver, but
its 184,320 reachable states are not a useful human learning system.

The objective of this project is **not** to replace the table with a small
rule set that solves every state suboptimally. It is to find a compact set of
rules that is provably optimal wherever it applies, measure the fraction of
states those rules cover, and leave the remaining long tail explicitly
uncovered.

A result such as “100 rules cover 70% of LSE states optimally” is useful even
if the other 30% still require ordinary EOLR/L4E or lookup-table treatment.

The executable baseline miner is
[`AnalyzeLseHumanRules.fsx`](AnalyzeLseHumanRules.fsx), and its generated
measurements are in
[`LseHumanRulesResults.md`](LseHumanRulesResults.md).

The follow-up reduction miner is
[`AnalyzeLseReductions.fsx`](AnalyzeLseReductions.fsx). It treats forms such as
`U-M-U` as parameterized instructions—“adjust, use the M direction that makes
the named relationship, then adjust”—and exhaustively proves the whole prefix
optimal before emitting a source-family to target-family reduction. Its output
is [`LseReductionResults.md`](LseReductionResults.md).

## Why EOLR is the model

EOLR combines edge orientation with handling the UL and UR edges. A concrete
cube state is described using a small vocabulary:

- an EO shape such as Arrow, 4/0, 2-opposite/2, or 2-adjacent/2;
- the relative positions of UL and UR;
- whether the relevant edges are oriented;
- front/back reflection and AUF symmetry;
- a short action or continuation rule.

That vocabulary lets a modest number of descriptions stand for many concrete
color assignments and positions. Kian Mansour's guide describes roughly 60
EOLR cases after symmetry and intuitive grouping rather than presenting every
raw cube state independently.

The source document is especially important because it is not really a flat
algorithm sheet. It first divides EO into nine unsolved families (`3/1`,
`4/0`, `2o/2`, `2a/2`, `1/1`, `2o/0`, `2a/0`, `0/2`, and `4/2`). Within a
family it describes the two LR edges relationally: stacked, bottom, adjacent,
opposite, oriented, misoriented, good, bad, and so on.

Most instructions are reductions rather than terminal algorithms. They say,
in effect, “place this LR edge over that one, perform the appropriate M move,
and continue from Stacked 3/1.” Several offer a misoriented-center alternative.
Words such as “adjust,” “either direction,” and “the M move that keeps this
edge on D” parameterize several concrete sequences with one description.

That is the compression mechanism we need to imitate. A human rule should be
allowed to:

- choose a direction from the observed relationship;
- perform a short setup/commutator-like trigger;
- transform the state into another named family;
- recurse from that simpler family;
- optionally preserve or deliberately misorient centers.

A fixed predicate followed by one literal move is therefore only a baseline,
not an adequate model of a human LSE rule.

Useful EOLR references:

- [The EOLR document used for the original study](https://docs.google.com/document/d/1dvGERLfN-0rVwN914HH1zRPHLfdM6d5rPfe0HxOMK08/edit?tab=t.0#heading=h.phjfa966vtn)
- [Kian Mansour's EOLR guide](https://sites.google.com/view/kianroux/eolr)
- [Kian Mansour's EOLR algorithm sheet](https://cuberoot.me/wp-content/uploads/2019/11/455-Roux-EOLR.pdf)
- [SpeedCubingTips EOLR case reference](https://www.speedcubingtips.eu/blog/2019/07/22/lse-eolr-methode-roux/)
- [Method Library's Roux/EOLR history and resource index](https://sites.google.com/view/methodlibrary/3x3-methods/roux)
- [The generated EOLR material in this repository](../../lse/EOLR.md)

## What the 184,320 states represent

The exact LSE coordinate contains:

- `6! = 720` permutations of the six LSE edges;
- `2^5 = 32` legal edge-orientation assignments (the sixth flip is fixed by
  parity);
- 4 M-slice center offsets;
- 4 top-corner AUFs.

That produces 368,640 raw coordinate slots. Exactly half are reachable in the
LSE orbit because edge permutation, centers, and corners must satisfy the
cube's parity constraints, leaving 184,320 states.

These are concrete coordinates, not 184,320 unrelated ideas. The policy is a
regular graph with only six possible actions, and many states share both
recognition features and optimal next moves.

## The partial-but-exact rule objective

A proposed rule has two parts:

1. A predicate expressed in recognizable features.
2. One move from the LSE move set.

For example:

```text
When EO is Arrow,
UL and UR are adjacent on U,
and the oriented LR edge matches the front corner pair:
    play U M
```

For mining purposes, the action may initially be a single move. Applying that
move returns to the rule classifier, so a sequence emerges from repeated rule
application. Later, several consecutive rules can be folded into a familiar
trigger.

A rule is accepted as **exact** only when its selected move reduces the exact
policy distance by one for every reachable state matching the predicate. The
policy stores all tied optimal next moves, so a rule can use any move shared by
the complete matched set.

States not covered by a proven rule remain uncovered. There is no generic or
suboptimal fallback hidden inside the coverage number.

## Candidate human features

The first mining pass should expose features that are cheap for a human to
recognize:

- EO shape and total flipped-edge count;
- flipped edges on U versus D;
- UL and UR positions and orientations;
- whether UL/UR are together, split, adjacent, opposite, or on D;
- center orientation;
- corner AUF;
- solved edges and solved/opposite bars;
- cycles among the remaining four edges;
- front/back mirrors;
- whether a move creates an Arrow, bar, LR insertion, or favorable L4E case.

Raw facts such as “the UF piece occupies DB” are useful to the miner, but the
finished rules should prefer relational descriptions such as “the two opposite
edges are exchanged.”

## Proposed analysis

1. Enumerate every reachable, non-solved LSE state from `lse-policy-v1.dat`.
2. Record the complete set of optimal next moves for each state.
3. Compute the human-recognizable features above.
4. Search for conjunctions of features whose matched states share at least one
   optimal next move.
5. Rank exact rules by newly covered states, not merely by total matches.
6. Report cumulative optimal coverage for 10, 25, 50, 100, 200, and 300 rules.
7. Validate each rule exhaustively against the policy.
8. Keep all unmatched states as the explicit long tail.

The first result should be treated as a baseline decision tree. Later passes
can compress it by:

- merging mirrored rules into one rule with a direction qualifier;
- removing redundant predicates;
- replacing absolute piece locations with relations;
- combining repeated one-move rules into triggers;
- clustering the uncovered states to discover new vocabulary;
- allowing a small number of named exceptional cases without weakening the
  guarantees of the broad rules.

## Measurements that matter

For each rule budget, report:

- percentage of all reachable states covered optimally;
- percentage under a representative scramble distribution;
- average and maximum recognition depth;
- average remaining exact distance after the rule fires;
- number of mirrored or color-relative variants represented by one schema;
- distribution of uncovered states by EO shape, LR relation, and exact
  distance.

Uniform state coverage and scramble-weighted coverage answer different
questions. Uniform coverage measures compression of the mathematical state
space. Scramble-weighted coverage estimates how often a solver would actually
recognize one of the rules in practice.

## Expected outcome

Exact optimality will probably have a long tail: states that look similar
under simple features but require different first moves because of the future
L4E permutation. That does not invalidate the project. The useful deliverable
is a growing, auditable library of broad rules with exact coverage numbers,
followed by increasingly specific exceptions.

The exact policy remains the oracle and the proof mechanism. The human rule
set is a compressed, progressively learned view of that policy.

## Initial baseline result

The first pass used EO shape, flip counts, UL/UR position and orientation,
center state, corner AUF, slot contents, and solved-edge count. It tested two
simple representations:

- a purity-first categorical decision tree limited to six conditions;
- overlapping conjunctions containing one, two, or selected three conditions.

Every accepted move was exhaustively checked against the exact optimal-move
mask. The decision tree's largest 100 exact leaves cover 2,172 of the 184,319
non-solved states (1.18%); its largest 300 cover 4,618 states (2.51%). Simple
overlapping conjunctions perform worse.

This is a useful negative baseline. Absolute locations and counts do not yet
capture the transformations humans recognize in EOLR. The next iteration
should prioritize relational and symmetry-aware features—mirrors, adjacency,
opposites, bars, permutation cycles, and parameterized directions—before
trying a more sophisticated learning algorithm.

The second pass added LR adjacency/opposition, permutation-cycle type and
count, solved LR/M-edge counts, M-slice occupancy, and oriented-solved edge
count. It improved the largest 100 decision-tree rules from 1.18% to 1.35%
coverage, the largest 300 from 2.51% to 2.86%, and the largest 1,000 from
5.54% to 6.25%. The improvement is real but small. Merely adding more static
classifiers is unlikely to produce the hoped-for compression; the next useful
experiment should represent parameterized transformations and short optimal
triggers rather than only one fixed next move per conjunction.

The third pass encoded the guide's EO-family classification and LR roles
directly. This raised decision-tree coverage to 1.61% for 100 rules, 3.51% for
300, and 7.65% for 1,000. That is another measurable improvement, but it also
reinforces the same conclusion: the guide's power comes from directional,
recursive transformations, not merely better static labels.

## First parameterized-reduction result

The next experiment stopped asking for one literal move per predicate. It
tested eight short axis skeletons—from `U-M` through `M-U-M-U`—and allowed the
power of each move to be chosen by the target relationship. A reduction was
accepted only if every concrete state in its source family had an
instantiation that decreased exact LSE distance on every move and reached the
same named target family.

The coarsest 100 EO/LR families produced no universal reductions at all. This
is strong evidence that optimal full LSE cannot choose an EOLR continuation
without observing some of the future L4E. Adding relative center/corner
alignment, edge-permutation cycle type, and solved M-edge count produced 361
universal reductions across 131 of 2,076 relational families. Those families
contain 2,111 concrete states, or 1.15% of the non-solved state space.

That percentage is not yet impressive, but the representation is finally
behaving like the source guide: broad rules now say “choose a U adjustment,”
“apply the M direction that creates this family,” and recurse. The most useful
next vocabulary is visual rather than more raw coordinates: good/bad arrows,
oriented and misoriented bars, whether an LR edge matches the adjacent corner,
and the relative order of the four non-LR edges. See the exhaustive rows in
[`LseReductionResults.md`](LseReductionResults.md).
