# Baseline exact LSE rule-mining results

This baseline uses both a categorical decision tree and overlapping conjunctions of human-oriented state features. Every counted rule selects a move that is optimal for every matched state. Impure groups are not counted as covered.

- Reachable non-solved states: 184,319
- Exact pure decision-tree leaves: 14,064
- Unresolved leaves after all current features: 14,002
- Exact one-, two-, and selected three-condition candidates: 2,548

## EOLR family coverage

The nine unsolved EO families from the EOLR guide plus solved EO classify every reachable LSE state:

| EO family | States |
| --- | ---: |
| 0/0 | 5,759 |
| 3/1 | 46,080 |
| 4/0 | 5,760 |
| 2o/2 | 11,520 |
| 2a/2 | 23,040 |
| 1/1 | 46,080 |
| 2o/0 | 11,520 |
| 2a/0 | 23,040 |
| 0/2 | 5,760 |
| 4/2 | 5,760 |

## Coverage by largest purity-first decision-tree leaves

| Rules | States | Coverage |
| ---: | ---: | ---: |
| 10 | 516 | 0.28% |
| 25 | 1,068 | 0.58% |
| 50 | 1,768 | 0.96% |
| 100 | 2,968 | 1.61% |
| 200 | 4,864 | 2.64% |
| 300 | 6,464 | 3.51% |
| 500 | 9,056 | 4.91% |
| 1,000 | 14,098 | 7.65% |

## Coverage by size-ranked overlapping conjunctions

| Rules | Newly covered states | Coverage |
| ---: | ---: | ---: |
| 10 | 816 | 0.44% |
| 25 | 1,302 | 0.71% |
| 50 | 1,558 | 0.85% |
| 100 | 1,738 | 0.94% |
| 200 | 1,895 | 1.03% |
| 300 | 1,895 | 1.03% |
| 500 | 1,895 | 1.03% |
| 1,000 | 1,895 | 1.03% |

## Thirty largest conjunction rules

1. **U** when EO shape = UL+UR+DF+DB flipped; center = quarter; LR layer relation = both D — 96 states
2. **U** when EO shape = UL+UR+DF+DB flipped; center = inverse-quarter; LR layer relation = both D — 96 states
3. **U** when EO shape = UF+UB+DF+DB flipped; center = half; LR layer relation = both D — 96 states
4. **M'** when EO shape = UF+UB flipped; center = quarter; LR role pair = flipped U-M + flipped U-M — 96 states
5. **M** when EO shape = UF+UB flipped; center = inverse-quarter; LR role pair = flipped U-M + flipped U-M — 96 states
6. **M** when EO shape = UL+UR+UF+UB+DF+DB flipped; center = solved; LR role pair = flipped U-M + flipped U-M — 96 states
7. **M** when EO shape = UL+UR+UF+UB+DF+DB flipped; center = half; LR role pair = flipped U-M + flipped U-M — 96 states
8. **M** when EO shape = UL+UR+UF+UB flipped; center = solved; oriented and solved edge count = 2 — 48 states
9. **M** when EO shape = UL+UR+UF+UB flipped; center = half; oriented and solved edge count = 2 — 48 states
10. **M** when EO shape = UL+UR+DF+DB flipped; center = solved; oriented and solved edge count = 2 — 48 states
11. **M** when EO shape = UL+UR+DF+DB flipped; center = half; oriented and solved edge count = 2 — 48 states
12. **U** when EO shape = UL+UR flipped; corner AUF = half; solved LR count = 2 — 48 states
13. **U** when EO shape = UB+DB flipped; corner AUF = half; solved LR count = 2 — 48 states
14. **U** when EO shape = UF+DF flipped; corner AUF = half; solved LR count = 2 — 48 states
15. **U** when EO shape = UL+UR+UF+UB+DF+DB flipped; corner AUF = half; solved LR count = 2 — 48 states
16. **M'** when EO shape = oriented; center = quarter; solved M-edge count = 3 — 32 states
17. **M** when EO shape = oriented; center = inverse-quarter; solved M-edge count = 3 — 32 states
18. **M** when EO shape = UL+UR+UF+UB+DF+DB flipped; center = solved; solved M-edge count = 3 — 32 states
19. **U** when flipped U-edge count = 1; solved M-edge count = 2; oriented and solved edge count = 4 — 32 states
20. **M'** when center = quarter; solved LR count = 1; oriented and solved edge count = 4 — 32 states
21. **M** when center = inverse-quarter; solved LR count = 1; oriented and solved edge count = 4 — 32 states
22. **U** when EO shape = UL+UR+UF+UB+DF+DB flipped; UL position = UR; permutation cycle type = 2+2+2 — 24 states
23. **U** when EO shape = UL+UR flipped; permutation cycle type = 2+2+1+1; solved LR count = 2 — 24 states
24. **U** when EO shape = UF+UB+DF+DB flipped; permutation cycle type = 2+2+1+1; solved LR count = 2 — 24 states
25. **M** when EO shape = UF+UB flipped; permutation cycle count = 4; oriented and solved edge count = 0 — 24 states
26. **U** when EO shape = UL+UR+UF+UB+DF+DB flipped; solved M-edge count = 4 — 16 states
27. **M** when EO shape = UF+UB flipped; LR role pair = flipped U-M + flipped U-M; permutation cycle type = 2+2+1+1 — 16 states
28. **M** when EO shape = UF+UB flipped; LR role pair = flipped U-M + flipped U-M; permutation cycle type = 2+2+2 — 16 states
29. **M2** when EO shape = UL+UR+DF+DB flipped; LR role pair = oriented U-M + oriented U-M; permutation cycle type = 2+2+2 — 16 states
30. **M2** when EO shape = UL+UR+DF+DB flipped; LR role pair = oriented U-M + oriented U-M; permutation cycle type = 4+1+1 — 16 states

These rules are a measurement baseline, not yet a polished teaching system. The next pass should merge mirrors, remove redundant conditions, and add relational features for bars, cycles, adjacency, and opposites.
