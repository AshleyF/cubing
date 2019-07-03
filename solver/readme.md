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

We've started with Roux. The following stages are used:

- FB - First block (left 1x2x3 block)
    - DL edge in place (using rotations; during inspection) [24 cases]
	- L center (also R center of course) [6 cases]
	- BL pair
	    - BL edge to DF (very meticulous for now) [22 cases]
		- BL corner to UBR
		- Pair and insert
	- FL pair
	    - FL edge to DF
		- FL corner to UFR
		- Pair and insert
- SB - Second block (right 1x2x3 block)
    - DR edge
    - BR pair
	- FR pair
- EO - Edge orientation (orient 6 remaining edges)
- LR - Left/Right upper edges
- L4E - Last four edges

# TODO

- Simplify patterns by discovering AUF
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