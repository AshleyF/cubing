# CMLL + EO influence candidate bank

`CmllEoCandidates.txt` contains the 92 distinct algorithms displayed on the
SpeedcubeQuest CMLL overview on 2026-09-27:

https://speedcube.quest/algorithms/3x3/cmll

The source page identifies some entries as COLL (and therefore EO-preserving),
but RouxLab does not trust or encode those labels. It parses every sequence,
proves that it preserves both Roux blocks, applies every valid U setup to the
current state, and retains only actions that actually solve CMLL. These 92
alternatives are merged with RouxLab's exhaustive 42-case one-look CMLL bank;
all applicable actions are ranked by algorithm length plus the exact remaining
LSE distance. The complete baseline is part of the candidate set, not a hidden
fallback.

This finite human-algorithm bank is an approximation to CMLLEO, not a complete
or globally optimal CMLLEO table.
