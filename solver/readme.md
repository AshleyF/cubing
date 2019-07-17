# Solver

The goal of this solver is to discover cases and corresponding patterns for each step. The steps are hand crafted and human-oriented. For example, the steps of a Roux solve.

Given thousands of scrambled cubes, the act of finding solutions to each step generates an exhaustive set of algs (up to the minimum length required) along with distinct patterns with which to recognize each case.

## Build Alg Sets

Given a partial goal state to be reached and a partially (or entirely) scrambled cube, we do an exhausive breadth-first search; terminating at the minimum depth at which one or more solutions are found.
This produces a set of algs which all solve the particular scramble. There may be many equally short solutions. Longer solutions are not considered.
Given a set of scrambled cubes, we get a set of many algorithm sets; one alg set for each distinct case encountered. In this way, we discover the cases given a sufficient (thousands) number of scrambles.

## Find Distinct Case Patterns

The set of scrambles and solutions are grouped by distinct alg set. Each group represents a distinct case.
Across all of the scrambles for a given case, we find the commonality. This becomes the pattern by which the case may be recognized.
Given a sufficent (at least hundreds per case) number of scrambles, the commonality settles to the true minimum.

## Optimize Solver

Given the set of patterns and known alg sets to solve each, these are used in place of exhaustive search in the future.
However, the process still begins with thousands of entirely scrambled cubes which are then solved up to the current "frontier" of known patterns and algs.

Even before all cases are known and patterns have been established, algs for thus far known cases can also be tried when no patterns match. With luck, one of them will work and is much faster than exhaustive search.

This process is iterated until a complete method (e.g. Roux, CFOP, ZZ, Petrus, ...) has been encoded to solve _any_ scramble.

# Roux

We've started with Roux.

## Beginner (10,000 solves)

- FB/SB tuck same orientation always in front, bring corner to top always same orientation, pair and insert.
- CMLL with sune only.
- EO with M' U' M' only.
- LR bring to DF/UB, then stack, bring to bottom, insert.
- L4E with M' U2' except bars and simple M2 case.

| Stage | Average | Worst |
| ----- | ------- | ----- |
| FB    | 15.5    |  22   |
| SB    | 21.1    |  32   |
| CMLL  | 35.0    |  66   |
| EO    | 10.9    |  20   |
| LR    |  7.4    |  10   |
| L4E   |  9.4    |  21   |
| Total | 99.3    | 171   |

## Intermediate (10,000 solves)

- Two-look CMLL (better by 15.7 avg, -40 worst).
- Full EO cases (better by 4.4 avg, -10 worst).
- Full LR cases (better by 1.9 avg, -3 worst).
- Full L4E cases (better by 5.3 avg, -16 worst).
- More flexible FB/SB (better by 0.1 avg, same worst).

| Stage | Average | Worst |
| ----- | ------- | ----- |
| FB    | 15.3    |  22   |
| SB    | 21.2    |  32   |
| CMLL  | 19.3    |  26   |
| EO    |  6.5    |  10   |
| LR    |  5.5    |   7   |
| L4E   |  4.1    |   5   |
| Total | 71.9    | 102   |

## Expert (10,000 solves)

- Full CMLL (better by 8.6 avg, -10 worst).

| Stage | Average | Worst |
| ----- | ------- | ----- |
| FB    | 15.5    | 22    |
| SB    | 21.1    | 32    |
| CMLL  | 10.7    | 16    |
| EO    |  6.5    | 10    |
| LR    |  5.5    |  7    |
| L4E   |  4.0    |  5    |
| Total | 63.4    | 92    |

# TODO

- Define goals in pattern language rather than `look Face.FOO Sticker.BAR cube` expressions
- Discover FB/SB cases by *deconstruction* from solved state
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