# Exact parameterized LSE reductions

This experiment treats a sequence shape such as `U-M-U` as one rule schema. Each U or M power is selected by the visible relationship that the rule promises to create. A schema is reported only when every concrete state in its normalized source family has at least one instantiation that follows the exact optimal policy at every move and reaches the same normalized target family.

- Concrete non-solved states: 184,319
- Relational source families: 2,076
- Universal exact reductions found: 361
- Source families with at least one reduction: 131
- Concrete states in those families: 2,111 (1.15%)
- Sequence skeletons tested: U, M, U-M, M-U, U-M-U, M-U-M, U-M-U-M, M-U-M-U

This is stricter than an ordinary EOLR description. Recognition uses EO family, the two color-neutral LR roles, their coarse geometric relation, relative center/corner alignment, permutation cycle type, and solved M-edge count. It does not yet use guide-specific ideas such as good/bad arrows, bars, or matching a particular corner color.

## Largest universal reductions

| States | Schema | Source family | Target family | Mean reachable targets |
| ---: | --- | --- | --- | ---: |
| 48 | U-adjust | 1/1, LR oriented U-side + oriented U-side, opposite U; center/corners delta 0; cycles 2+2+1+1; 0 solved M edges | 1/1, LR oriented U-M + oriented U-M, opposite U | 1.29 |
| 48 | U-adjust | 1/1, LR oriented U-side + oriented U-side, opposite U; center/corners delta 2; cycles 2+2+1+1; 0 solved M edges | 1/1, LR oriented U-M + oriented U-M, opposite U | 1.25 |
| 48 | U-adjust | 2a/2, LR oriented U-side + flipped U-side, opposite U; center/corners delta 0; cycles 2+2+1+1; 0 solved M edges | 2a/2, LR oriented U-M + flipped U-M, opposite U | 1.12 |
| 48 | U-adjust | 2a/2, LR oriented U-side + flipped U-side, opposite U; center/corners delta 2; cycles 2+2+1+1; 0 solved M edges | 2a/2, LR oriented U-M + flipped U-M, opposite U | 1.12 |
| 48 | U-adjust | 2a/2, LR oriented U-side + oriented U-M, adjacent U; center/corners delta 2; cycles 2+2+1+1; 1 solved M edges | 2a/2, LR oriented U-side + oriented U-M, adjacent U | 1.00 |
| 48 | U-adjust | 0/0, LR oriented U-side + oriented D, split; center/corners delta 0; cycles 4+2; 0 solved M edges | 0/0, LR oriented U-M + oriented D, split | 1.33 |
| 48 | U-adjust | 0/0, LR oriented U-side + oriented D, split; center/corners delta 2; cycles 4+2; 0 solved M edges | 0/0, LR oriented U-M + oriented D, split | 1.33 |
| 48 | U-adjust | 2o/0, LR flipped U-side + oriented D, split; center/corners delta 0; cycles 4+2; 0 solved M edges | 2o/0, LR flipped U-M + oriented D, split | 1.12 |
| 48 | U-adjust | 2o/0, LR flipped U-side + oriented D, split; center/corners delta 2; cycles 4+2; 0 solved M edges | 2o/0, LR flipped U-M + oriented D, split | 1.21 |
| 48 | M | 2o/0, LR flipped U-M + flipped U-M, opposite U; center/corners delta 3; cycles 6; 0 solved M edges | 1/1, LR oriented U-M + oriented D, split | 1.71 |
| 48 | M | 2o/0, LR flipped U-M + flipped U-M, opposite U; center/corners delta 1; cycles 6; 0 solved M edges | 1/1, LR oriented U-M + oriented D, split | 1.71 |
| 48 | U-M | 1/1, LR oriented U-side + oriented U-side, opposite U; center/corners delta 0; cycles 2+2+1+1; 0 solved M edges | 3/1, LR flipped U-M + flipped D, split | 2.33 |
| 48 | U-M | 1/1, LR oriented U-side + oriented U-side, opposite U; center/corners delta 2; cycles 2+2+1+1; 0 solved M edges | 3/1, LR flipped U-M + flipped D, split | 2.04 |
| 48 | M-U | 2o/0, LR flipped U-M + flipped U-M, opposite U; center/corners delta 3; cycles 6; 0 solved M edges | 1/1, LR oriented U-side + oriented D, split | 1.88 |
| 48 | M-U | 2o/0, LR flipped U-M + flipped U-M, opposite U; center/corners delta 1; cycles 6; 0 solved M edges | 1/1, LR oriented U-side + oriented D, split | 1.88 |
| 40 | M | 4/2, LR flipped U-M + flipped U-M, opposite U; center/corners delta 0; cycles 4+2; 0 solved M edges | 2o/0, LR oriented U-M + oriented D, split | 1.60 |
| 40 | M | 4/2, LR flipped U-M + flipped U-M, opposite U; center/corners delta 2; cycles 4+2; 0 solved M edges | 2o/0, LR oriented U-M + oriented D, split | 1.60 |
| 40 | M-U | 4/2, LR flipped U-M + flipped U-M, opposite U; center/corners delta 0; cycles 4+2; 0 solved M edges | 2o/0, LR oriented U-side + oriented D, split | 1.75 |
| 40 | M-U | 4/2, LR flipped U-M + flipped U-M, opposite U; center/corners delta 2; cycles 4+2; 0 solved M edges | 2o/0, LR oriented U-side + oriented D, split | 1.75 |
| 32 | U-adjust | 2o/2, LR flipped U-side + flipped U-side, opposite U; center/corners delta 3; cycles 3+2+1; 1 solved M edges | 2o/2, LR flipped U-M + flipped U-M, opposite U | 1.00 |
| 32 | U-adjust | 2o/2, LR flipped U-side + flipped U-side, opposite U; center/corners delta 1; cycles 3+2+1; 1 solved M edges | 2o/2, LR flipped U-M + flipped U-M, opposite U | 1.00 |
| 32 | U-adjust | 2o/0, LR flipped U-side + oriented D, split; center/corners delta 0; cycles 3+3; 0 solved M edges | 2o/0, LR flipped U-M + oriented D, split | 1.00 |
| 32 | U-adjust | 2o/0, LR flipped U-side + oriented D, split; center/corners delta 2; cycles 3+3; 0 solved M edges | 2o/0, LR flipped U-M + oriented D, split | 1.00 |
| 32 | M | 1/1, LR oriented U-M + oriented U-M, opposite U; center/corners delta 2; cycles 2+2+1+1; 2 solved M edges | 2a/2, LR flipped U-M + flipped D, split | 1.88 |
| 32 | M | 2o/0, LR flipped U-M + flipped U-M, opposite U; center/corners delta 3; cycles 3+2+1; 1 solved M edges | 1/1, LR oriented U-M + oriented D, split | 1.81 |
| 32 | M | 2o/0, LR flipped U-M + flipped U-M, opposite U; center/corners delta 1; cycles 3+2+1; 1 solved M edges | 1/1, LR oriented U-M + oriented D, split | 1.81 |
| 32 | U-adjust | 2o/0, LR oriented U-M + oriented D, split; center/corners delta 3; cycles 2+2+2; 0 solved M edges | 2o/0, LR oriented U-side + oriented D, split | 1.12 |
| 32 | U-adjust | 2o/0, LR oriented U-M + oriented D, split; center/corners delta 1; cycles 2+2+2; 0 solved M edges | 2o/0, LR oriented U-side + oriented D, split | 1.12 |
| 32 | M | 4/2, LR flipped U-M + flipped U-M, opposite U; center/corners delta 0; cycles 5+1; 1 solved M edges | 2o/0, LR oriented U-M + oriented D, split | 1.50 |
| 32 | M | 4/2, LR flipped U-M + flipped U-M, opposite U; center/corners delta 2; cycles 5+1; 1 solved M edges | 2o/0, LR oriented U-M + oriented D, split | 1.50 |
| 32 | U-adjust | 2o/0, LR oriented U-M + oriented D, split; center/corners delta 3; cycles 4+1+1; 2 solved M edges | 2o/0, LR oriented U-side + oriented D, split | 1.12 |
| 32 | U-adjust | 2o/0, LR oriented U-M + oriented D, split; center/corners delta 1; cycles 4+1+1; 2 solved M edges | 2o/0, LR oriented U-side + oriented D, split | 1.12 |
| 32 | U-adjust | 0/0, LR oriented D + oriented D, both D; center/corners delta 0; cycles 5+1; 1 solved M edges | 0/0, LR oriented D + oriented D, both D | 1.00 |
| 32 | U-adjust | 0/0, LR oriented D + oriented D, both D; center/corners delta 2; cycles 5+1; 1 solved M edges | 0/0, LR oriented D + oriented D, both D | 1.00 |
| 32 | M-U | 1/1, LR oriented U-M + oriented U-M, opposite U; center/corners delta 2; cycles 2+2+1+1; 2 solved M edges | 2a/2, LR flipped U-side + flipped D, split | 2.12 |
| 32 | M-U | 2o/0, LR flipped U-M + flipped U-M, opposite U; center/corners delta 3; cycles 3+2+1; 1 solved M edges | 1/1, LR oriented U-side + oriented D, split | 2.12 |
| 32 | M-U | 2o/0, LR flipped U-M + flipped U-M, opposite U; center/corners delta 1; cycles 3+2+1; 1 solved M edges | 1/1, LR oriented U-side + oriented D, split | 2.12 |
| 32 | M-U | 4/2, LR flipped U-M + flipped U-M, opposite U; center/corners delta 0; cycles 5+1; 1 solved M edges | 2o/0, LR oriented U-side + oriented D, split | 1.50 |
| 32 | M-U | 4/2, LR flipped U-M + flipped U-M, opposite U; center/corners delta 2; cycles 5+1; 1 solved M edges | 2o/0, LR oriented U-side + oriented D, split | 1.50 |
| 16 | M | 2a/0, LR oriented U-side + flipped U-side, opposite U; center/corners delta 0; cycles 1+1+1+1+1+1; 4 solved M edges | 3/1, LR oriented U-side + flipped U-side, opposite U | 2.38 |
| 16 | U-adjust | 3/1, LR oriented U-side + flipped U-side, opposite U; center/corners delta 2; cycles 1+1+1+1+1+1; 4 solved M edges | 3/1, LR oriented U-M + flipped U-M, opposite U | 1.50 |
| 16 | U-adjust | 2a/2, LR oriented U-side + flipped U-side, opposite U; center/corners delta 0; cycles 1+1+1+1+1+1; 4 solved M edges | 2a/2, LR oriented U-M + flipped U-M, opposite U | 1.25 |
| 16 | U-adjust | 2a/2, LR oriented U-side + flipped U-side, opposite U; center/corners delta 2; cycles 1+1+1+1+1+1; 4 solved M edges | 2a/2, LR oriented U-M + flipped U-M, opposite U | 1.50 |
| 16 | U-adjust | 2o/0, LR flipped U-side + oriented U-M, adjacent U; center/corners delta 3; cycles 2+1+1+1+1; 3 solved M edges | 2o/0, LR oriented U-side + flipped U-M, adjacent U | 1.38 |
| 16 | U-adjust | 2o/0, LR flipped U-side + oriented U-M, adjacent U; center/corners delta 1; cycles 2+1+1+1+1; 3 solved M edges | 2o/0, LR oriented U-side + flipped U-M, adjacent U | 1.38 |
| 16 | U-adjust | 2o/0, LR flipped U-side + oriented D, split; center/corners delta 3; cycles 2+1+1+1+1; 3 solved M edges | 2o/0, LR flipped U-M + oriented D, split | 1.25 |
| 16 | U-adjust | 2o/0, LR flipped U-side + oriented D, split; center/corners delta 1; cycles 2+1+1+1+1; 3 solved M edges | 2o/0, LR flipped U-M + oriented D, split | 1.25 |
| 16 | U-adjust | 3/1, LR oriented U-side + oriented D, split; center/corners delta 3; cycles 2+1+1+1+1; 3 solved M edges | 3/1, LR oriented U-M + oriented D, split | 1.25 |
| 16 | U-adjust | 3/1, LR oriented U-side + oriented D, split; center/corners delta 1; cycles 2+1+1+1+1; 3 solved M edges | 3/1, LR oriented U-M + oriented D, split | 1.25 |
| 16 | U-adjust | 3/1, LR oriented U-side + flipped U-side, opposite U; center/corners delta 3; cycles 2+1+1+1+1; 4 solved M edges | 3/1, LR oriented U-M + flipped U-M, opposite U | 1.38 |
| 16 | U-adjust | 3/1, LR oriented U-side + flipped U-side, opposite U; center/corners delta 1; cycles 2+1+1+1+1; 4 solved M edges | 3/1, LR oriented U-M + flipped U-M, opposite U | 1.38 |
| 16 | U-adjust | 2o/0, LR flipped U-side + oriented U-M, adjacent U; center/corners delta 2; cycles 3+1+1+1; 3 solved M edges | 2o/0, LR oriented U-side + flipped U-M, adjacent U | 1.25 |
| 16 | M | 2a/0, LR oriented U-side + flipped U-M, adjacent U; center/corners delta 0; cycles 3+1+1+1; 3 solved M edges | 3/1, LR oriented U-side + oriented D, split | 1.50 |
| 16 | M | 2a/0, LR flipped U-side + oriented U-M, adjacent U; center/corners delta 0; cycles 3+1+1+1; 3 solved M edges | 2a/2, LR flipped U-side + flipped D, split | 1.62 |
| 16 | M | 2a/0, LR oriented U-side + oriented U-M, adjacent U; center/corners delta 0; cycles 3+1+1+1; 3 solved M edges | 3/1, LR oriented U-side + flipped U-M, adjacent U | 1.12 |
| 16 | M | 2a/0, LR oriented U-side + oriented U-M, adjacent U; center/corners delta 2; cycles 3+1+1+1; 3 solved M edges | 3/1, LR oriented U-side + flipped U-M, adjacent U | 1.38 |
| 16 | U-adjust | 0/2, LR oriented U-side + oriented U-M, adjacent U; center/corners delta 0; cycles 3+1+1+1; 3 solved M edges | 0/2, LR oriented U-side + oriented U-M, adjacent U | 1.00 |
| 16 | M | 0/2, LR oriented U-side + oriented U-M, adjacent U; center/corners delta 0; cycles 3+1+1+1; 3 solved M edges | 1/1, LR oriented U-side + flipped D, split | 1.75 |
| 16 | U-adjust | 0/2, LR oriented U-side + oriented U-M, adjacent U; center/corners delta 2; cycles 3+1+1+1; 3 solved M edges | 0/2, LR oriented U-side + oriented U-M, adjacent U | 1.00 |
| 16 | M | 0/2, LR oriented U-side + oriented U-M, adjacent U; center/corners delta 2; cycles 3+1+1+1; 3 solved M edges | 1/1, LR oriented U-side + flipped D, split | 1.50 |

## Interpretation

A row is a genuine recursive rule candidate: recognize the source family, choose powers in the named skeleton that produce the target relationship, then continue using the rule for that target family. The next pass should add the guide's missing relational vocabulary, choose a small acyclic set of reductions, and test how many states can reach solved by recursive application without consulting the raw table.
