# Solver

The goal of this solver is to discover cases and corresponding patterns for each step. The steps are hand crafted and human-oriented. For example, the steps of a Roux solve.

Given thousands of scrambled cubes, the act of finding solutions to each step generates an exhaustive set of algs (up to the minimum length required) along with distinct patterns with which to recognize each case.

## Build Alg Sets

Given a partial goal state to be reached and a partially (or entirely) scrambled cube, we do an exhaustive breadth-first search; terminating at the minimum depth at which one or more solutions are found.
This produces a set of algs which all solve the particular scramble. There may be many equally short solutions. Longer solutions are not considered.
Given a set of scrambled cubes, we get a set of many algorithm sets; one alg set for each distinct case encountered. In this way, we discover the cases given a sufficient number (thousands) of scrambles.

## Find Distinct Case Patterns

The set of scrambles and solutions are grouped by distinct alg set. Each group represents a distinct case.
Across all of the scrambles for a given case, we find the commonality. This becomes the pattern by which the case may be recognized.
Given a sufficient (at least hundreds per case) number of scrambles, the commonality settles to the true minimum.

## Optimize Solver

Given the set of patterns and known alg sets to solve each, these are used in place of exhaustive search in the future.
However, the process still begins with thousands of entirely scrambled cubes which are then solved up to the current "frontier" of known patterns and algs.

Even before all cases are known and patterns have been established, algs for thus far known cases can also be tried when no patterns match. With luck, one of them will work and is much faster than exhaustive search.

This process is iterated until a complete method (e.g. Roux, CFOP, ZZ, Petrus, ...) has been encoded to solve _any_ scramble.

# Roux

We've started with Roux. Note that the following include first rotating the DL edge into place. These rotations (1.5 average, 2 worst case) have been subtracted from FB.

## Beginner (10,000 solves)

- FB/SB tuck same orientation always in front, bring corner to top always same orientation, pair and insert.
- CMLL with sune only.
- EO with M' U' M' only.
- LR bring to DF/UB, then stack, bring to bottom, insert.
- L4E with M' U2' except bars and simple M2 case.

| Stage | Average | Worst |
| ----- | ------- | ----- |
| FB    | 14.0    |  20   |
| SB    | 21.2    |  32   |
| CMLL  | 34.4    |  59   |
| EO    | 10.9    |  20   |
| LR    |  7.4    |  10   |
| L4E   |  9.4    |  21   |
| Total | 97.3    | 162   |

## Intermediate (10,000 solves)

- Two-look CMLL (better by 15.7 avg, -40 worst).
- Full EO cases (better by 4.4 avg, -10 worst).
- Full LR cases (better by 1.9 avg, -3 worst).
- Full L4E cases (better by 5.3 avg, -16 worst).
- F2L-style FB/SB (better by 3.3/5.5 avg, -4/-8 worst, produced by (blocks solver)[../blocks])

| Stage | Average | Worst |
| ----- | ------- | ----- |
| FB    | 10.7    | 16    |
| SB    | 15.7    | 24    |
| CMLL  | 19.3    | 26    |
| EO    |  6.5    | 10    |
| LR    |  5.5    |  7    |
| L4E   |  4.1    |  5    |
| Total | 61.9    | 88    |

## Advanced (10,000 solves)

- Full CMLL (better by 8.6 avg, -10 worst).

| Stage | Average | Worst |
| ----- | ------- | ----- |
| FB    | 10.7    | 16    |
| SB    | 15.7    | 24    |
| CMLL  | 10.7    | 16    |
| EO    |  6.5    | 10    |
| LR    |  5.5    |  7    |
| L4E   |  4.1    |  5    |
| Total | 53.2    | 78    |

# TODO

- Intermediate FB/SB pairs with edge in DF/DB flipped either way (multi-strategy)
- Formalize edge and corner orientations in search
- Search centers/corners/edges by "ease" (T/F then L/R then D then B
- Gather metrics (moves, rotations, turns [half/quarter], looks [batch and individual stickers])
    - By phase (inspections rotations, cross/LR blocks, PLL, ...)
- Algorithm search
- Rank algorithms by "ease" (R, L, U, F, D, B, ... combinations, finger tricks, ...)
    - Maybe with user input or video analysis
- Better scramble algorithm (reducing "useless" moves)
- Idea: Sub-steps defined by steps to completed state (skip all sub-steps if complete)
- Function to rotate/mirror algorithms (also relative to fixed points? e.g. pair edge with corner vs. corner with edge)

Long term:
- Video analysis
- Program synthesis for block-building steps

# Ideas

It may be possible to use A* to solve cubes, with a strong heuristic guide. As a heuristic, I'm thinking to use some measure of "entropy"; zero being solved and something like `ln(states)` otherwise.

Aside from being solved. Pieces may have other properties and relationships making them more or less closer to solved.

* Edge/center pairs
* Edge/corner pairs
* 1x2x2 blocks
* 1x2x3 blocks
* 2x2x3 blocks (Petrus)
* 1x3x3 faces (beginner)
* 2x3x3 layers (F2L)
* Oriented
* Permuted
* Edges oriented (in Roux/ZZ sense)
* Edge/center in line (single twist to solve)
* Edge/corner in line (single twist to solve)

I'm hoping that such an approach will cause the system to do block building or could be influenced by weights to tend to use layer-by-layer, Roux-style, Petrus-style, ZZ-style EO-line, corners first, etc. It may automatically do interesting things like influencing edges during F2L/SB, etc. The hope is to have a solver for first/second block in Roux for example and use pattern matching for the rest and to have solves from which humans can learn (human-like to some degree).

Interesting that with block building, having individual pieces in place can be _bad_. For example, F2L cases where a corner or edge is already in place are _harder_. They need to be paired outside of the "slot" in which they belong and then inserted as a unit.