open System
open Cube
open Solver
open Render

// single-alg CO
let sune = "R U R' U R U2 R'"
let coH = sune + " U' " + sune + " " + sune + " U' " + sune + " U2 " + sune // 38 twists (5 sunes)
let coPi = sune + " U2 " + sune + " U' " + sune + " U2 " + sune // 31 twists (4 sunes)
let coU = sune + " U' " + sune + " U2 " + sune // 23 twists (3 sunes)
let coT = sune + " U " + sune + " U2 " + sune // 23 twists (3 sunes)
let coS = sune // 7 twists (1 sune)
let coAs = sune + " U2 " + sune // 15 twists (2 sunes)
let coL = sune + " " + sune + " U2 " + sune // 22 twists (3 sunes)

// single-alg CP
let jperm = "R U R' F' R U R' U' R' F R2 U' R'" // without final AUF (U')
// *)

(* seven-alg CO
let sexy = "R U R' U'"
let coH = "F " + sexy + " " + sexy + " " + sexy + " F'" // 14 twists
// *)

let patterns = [
    // Solving DL edge (during inspection) [24 cases]
    "DLEdge",  ".....................................B..........W.....", [] // skip
    "DLEdge",  "...W.......................B..........................", ["x"]
    "DLEdge",  ".W..................................................B.", ["x y"; "y z"; "z x"]
    "DLEdge",  ".......W..B...........................................", ["x y'"; "y' z'"; "z' x"]
    "DLEdge",  ".....W.............................B..................", ["x y2"; "x' z2"; "y2 x'"; "z2 x"]
    "DLEdge",  ".....B.............................W..................", ["x z"; "y' x"; "z y'"]
    "DLEdge",  ".............................WB.......................", ["x z'"; "y x"; "z' y"]
    "DLEdge",  "................................WB....................", ["x z2"; "x' y2"; "y2 x"; "z2 x'"]
    "DLEdge",  ".............................BW.......................", ["x'"]
    "DLEdge",  "................B.....W...............................", ["x' y"; "y z'"; "z' x'"]
    "DLEdge",  "........................................W.....B.......", ["x' y'"; "y' z"; "z x'"]
    "DLEdge",  "................................BW....................", ["x' z"; "y x'"; "z y"]
    "DLEdge",  "...B.......................W..........................", ["x' z'"; "y' x'"; "z' y'"]
    "DLEdge",  "............W......B..................................", ["x2"]
    "DLEdge",  ".......B..W...........................................", ["x2 y"; "y z2"; "y' x2"; "z2 y'"]
    "DLEdge",  "................W.....B...............................", ["x2 y'"; "y x2"; "y' z2"; "z2 y"]
    "DLEdge",  "..............B..........W............................", ["x2 z"; "y2 z'"; "z y2"; "z' x2"]
    "DLEdge",  ".....................................W..........B.....", ["x2 z'"; "y2 z"; "z x2"; "z' y2"]
    "DLEdge",  "........................................B.....W.......", ["y"]
    "DLEdge",  ".B..................................................W.", ["y'"]
    "DLEdge",  "...........................................B......W...", ["y2"]
    "DLEdge",  "...........................................W......B...", ["z"]
    "DLEdge",  "............B......W..................................", ["z'"]
    "DLEdge",  "..............W..........B............................", ["z2"]
    // Solve left center [6 cases]
    "LCenter", "............................B.....G..B..........W.....", [] // skip
    "LCenter", "....B..........................G.....B..........W.....", ["E"; "u'"]
    "LCenter", "....G..........................B.....B..........W.....", ["u"; "E'"]
    "LCenter", "............................G.....B..B..........W.....", ["u2"; "E2"]
    "LCenter", ".............G.......................B..........WB....", ["r u"; "r E'"; "r' u'"; "r' E"; "M u'"; "M E"; "M' u"; "M' E'"]
    "LCenter", ".............B.......................B..........WG....", ["r u'"; "r E"; "r' u"; "r' E'"; "M u"; "M E'"; "M' u'"; "M' E"];
    // Tuck LB edge to FD [22 cases]
    "TuckLBtoFD", "............................B.....G..B..B.....O.W.....", [] // skip
    "TuckLBtoFD", "...O.......................BB.....G..B..........W.....", ["B r"; "B M'"]
    "TuckLBtoFD", ".....B......................B.....GO.B..........W.....", ["R2 F"; "r2 F"; "B r2"; "B M2"]
    "TuckLBtoFD", ".....O......................B.....GB.B..........W.....", ["B' r"; "B' M'"]
    "TuckLBtoFD", "...B.......................OB.....G..B..........W.....", ["B' r2"; "B' M2"]
    "TuckLBtoFD", ".......O..B.................B.....G..B..........W.....", ["U2 r'"; "U2 M"; "r' F2"; "B2 r"; "B2 M'"; "M F2"]
    "TuckLBtoFD", ".B..........................B.....G..B..........W...O.", ["r2 F2"; "B2 r2"; "B2 M2"; "M2 F2"]
    "TuckLBtoFD", "............................B...BOG..B..........W.....", ["F"]
    "TuckLBtoFD", "............................BBO...G..B..........W.....", ["F r'"; "F M"]
    "TuckLBtoFD", "............................BOB...G..B..........W.....", ["F'"]
    "TuckLBtoFD", "............................B...OBG..B..........W.....", ["F' r'"; "F' M"]
    "TuckLBtoFD", "................O.....B.....B.....G..B..........W.....", ["F2"]
    "TuckLBtoFD", "............................B.....G..B..O.....B.W.....", ["r F2"; "F2 r'"; "F2 M"; "M' F2"]
    "TuckLBtoFD", "............................B.....G..B.....B....W.O...", ["L D' L'"; "L' D' L"; "L2 D' L2"; "l D' L'"; "l' D' L"; "l2 D' L2"; "R F' r'"; "R F' M"; "R' B' r"; "R' B' M'"; "R2 U F2"; "R2 U' r2"; "R2 U' M2"; "r F' r'"; "r F' M"; "r' B' r"; "r' B' M'"; "r2 U F2"; "r2 U' r2"; "r2 U' M2"]
    "TuckLBtoFD", "................B.....O.....B.....G..B..........W.....", ["r'"; "M"]
    "TuckLBtoFD", ".O..........................B.....G..B..........W...B.", ["r"; "M'"]
    "TuckLBtoFD", ".......B..O.................B.....G..B..........W.....", ["r2"; "M2"]
    "TuckLBtoFD", "............................B.....G..B.....O....W.B...", ["R F"; "r F"]
    "TuckLBtoFD", "..............B..........O..B.....G..B..........W.....", ["U r'"; "U M"; "R' F"; "r' F"]
    "TuckLBtoFD", "..............O..........B..B.....G..B..........W.....", ["U F2"; "U' r2"; "U' M2"]
    "TuckLBtoFD", "............O......B........B.....G..B..........W.....", ["U r2"; "U M2"; "U' F2"]
    "TuckLBtoFD", "............B......O........B.....G..B..........W.....", ["U' r'"; "U' M"]
    // Bring DLB corner to URB [24 cases]
    "BringDLBtoURB", "........B..O..............W.B.....G..B..B.....O.W.....", [] // skip
    "BringDLBtoURB", "..B.........................B.....G..B..B...O.O.W....W", ["B"]
    "BringDLBtoURB", "...............W....BO......B.....G..B..B.....O.W.....", ["U B'"]
    "BringDLBtoURB", ".................O.....WB...B.....G..B..B.....O.W.....", ["U'"]
    "BringDLBtoURB", "...............O....WB......B.....G..B..B.....O.W.....", ["U2"]
    "BringDLBtoURB", "............................B.....G..B..BBW...OOW.....", ["R2"]
    "BringDLBtoURB", "...............B....OW......B.....G..B..B.....O.W.....", ["U' R"]
    "BringDLBtoURB", "......B..W........O.........B.....G..B..B.....O.W.....", ["B'"]
    "BringDLBtoURB", "............................B.....G..B..BOB...OWW.....", ["R U'"]
    "BringDLBtoURB", "............................B.....G..B..BWO...OBW.....", ["R' B"]
    "BringDLBtoURB", ".................B.....OW...B.....G..B..B.....O.W.....", ["R"]
    "BringDLBtoURB", "............................B.....G..BWOB....BO.W.....", ["L' U2 L"; "r' F2 r"; "F2 R F2"; "b2 L b2"; "M F2 r"; "S2 L b2"]
    "BringDLBtoURB", "O...........................B.....G.BB..B.....O.W..W..", ["B R'"]
    "BringDLBtoURB", "........W..B..............O.B.....G..B..B.....O.W.....", ["U R"; "R B"; "B U"]
    "BringDLBtoURB", "B...........................B.....G.WB..B.....O.W..O..", ["B2"]
    "BringDLBtoURB", "......W..O........B.........B.....G..B..B.....O.W.....", ["U"]
    "BringDLBtoURB", "............................B.....G..BBWB....OO.W.....", ["L' U B' L"; "L2 u2 b2 U'"; "L2 u2 S2 U'"; "r' F U' r"; "r' F2 U' M'"; "r' F2 M' U'"; "r2 U2 F2 U'"; "r2 F2 M2 U'"; "F U' R F'"; "F U' r F'"; "F2 U2 r2 U"; "F2 U2 M2 U"; "b L' U b'"; "b L' u b'"; "b L2 U S"; "b L2 u S"; "b L2 S U"; "b2 u2 L2 U"; "b2 L2 S2 U"; "M F U' r"; "M F2 U' M'"; "M F2 M' U'"; "M2 U2 F2 U'"; "M2 F2 M2 U'"; "S' L' U b'"; "S' L' u b'"; "S' L2 U S"; "S' L2 u S"; "S' L2 S U"; "S2 u2 L2 U"; "S2 L2 S2 U"]
    "BringDLBtoURB", "........O..W..............B.B.....G..B..B.....O.W.....", ["U' B'"; "R' U'"; "B' R'"]
    "BringDLBtoURB", "..W.........................B.....G..B..B...B.O.W....O", ["R2 U'"; "B2 U"]
    "BringDLBtoURB", "............................B.....G..BOBB....WO.W.....", ["L2 B' L2"; "r2 F' r2 | F U2 F' | b L2 b' | M2 F' r2 | S' L2 b'"]
    "BringDLBtoURB", ".................W.....BO...B.....G..B..B.....O.W.....", ["U2 B'"; "R2 B"]
    "BringDLBtoURB", "W...........................B.....G.OB..B.....O.W..B..", ["B' U"]
    "BringDLBtoURB", "..O.........................B.....G..B..B...W.O.W....B", ["R'"]
    "BringDLBtoURB", "......O..B........W.........B.....G..B..B.....O.W.....", ["U2 R"; "B2 R'"]
    // Insert LB pair [1 case]
    // "InsertLBPair", "O..O.......................BB.....G.BB..........W..W..", [] // skip TODO: this should be hierarchical (before TuckLBtoFD & BringDLBtoURB)
    "InsertLBPair", "........B..O..............W.B.....G..B..B.....O.W.....", ["R M B'"; "R2 r' B'"; "r M2 B'"; "r' R2 B'"; "M R B'"; "M2 r B'"]
    // Tuck LF to BD [20 cases]
    "TuckLFtoBD", "OB.O.......................BB.....G.BB..........W..WR.", [] // skip
    "TuckLFtoBD", "O..O............R.....B....BB.....G.BB..........W..W..", ["r2"; "M2"]
    "TuckLFtoBD", "O..O.......................BB...RBG.BB..........W..W..", ["F r'"; "F M"]
    "TuckLFtoBD", "O..O.......................BBBR...G.BB..........W..W..", ["F' r'"; "F' M"]
    "TuckLFtoBD", "O..O........R......B.......BB.....G.BB..........W..W..", ["U' r2"; "U' M2"]
    "TuckLFtoBD", "O..O...R..B................BB.....G.BB..........W..W..", ["r"; "M'"]
    "TuckLFtoBD", "O..O.......................BB.....G.BB.....B....W.RW..", ["L D L'"; "L2 D L2"; "l D L'"; "l2 D L2"; "R F r'"; "R F M"; "R2 U r2"; "R2 U M2"; "r F r'"; "r F M"; "r2 U r2"; "r2 U M2"]
    "TuckLFtoBD", "OR.O.......................BB.....G.BB..........W..WB.", ["r F2 r2"; "r F2 M2"; "r' U2 r2"; "r' U2 M2"; "r2 U2 r"; "r2 U2 M'"; "r2 F2 r'"; "r2 F2 M"; "M U2 r2"; "M U2 M2"; "M' F2 r2"; "M' F2 M2"; "M2 U2 r"; "M2 U2 M'"; "M2 F2 r'"; "M2 F2 M"]
    "TuckLFtoBD", "O..O.......................BB...BRG.BB..........W..W..", ["F' r2"; "F' M2"]
    "TuckLFtoBD", "O..O.......................BB.....G.BB.....R....W.BW..", ["R F' r2"; "R F' M2"; "R2 U' r"; "R2 U' M'"; "r F' r2"; "r F' M2"; "r2 U' r"; "r2 U' M'"]
    "TuckLFtoBD", "O..O............B.....R....BB.....G.BB..........W..W..", ["U2 r"; "U2 M'"; "F2 r'"; "F2 M"]
    "TuckLFtoBD", "O..O.......................BB.....G.BB..B.....R.W..W..", ["F2 r2"; "F2 M2"]
    "TuckLFtoBD", "O..O..........B..........R.BB.....G.BB..........W..W..", ["U' r"; "U' M'"]
    "TuckLFtoBD", "O..O.......................BBRB...G.BB..........W..W..", ["F r2"; "F M2"]
    "TuckLFtoBD", "O..O.B.....................BB.....GRBB..........W..W..", ["L' B' L"; "L2 B' L2"; "l' B' L"; "l2 B' L2"; "R' U' r"; "R' U' M'"; "R2 F' r2"; "R2 F' M2"; "r' U' r"; "r' U' M'"; "r2 F' r2"; "r2 F' M2"]
    "TuckLFtoBD", "O..O........B......R.......BB.....G.BB..........W..W..", ["U r"; "U M'"]
    "TuckLFtoBD", "O..O..........R..........B.BB.....G.BB..........W..W..", ["U r2"; "U M2"]
    "TuckLFtoBD", "O..O.......................BB.....G.BB..R.....B.W..W..", ["r'"; "M"]
    "TuckLFtoBD", "O..O.R.....................BB.....GBBB..........W..W..", ["R' U r2"; "R' U M2"; "R2 F r'"; "R2 F M"; "r' U r2"; "r' U M2"; "r2 F r'"; "r2 F M"]
    "TuckLFtoBD", "O..O...B..R................BB.....G.BB..........W..W..", ["U2 r2"; "U2 M2"]
    // Bring DLF corner to URF [21 cases]
    "BringDLFtoURF", "OB.O.............R.....BW..BB.....G.BB..........W..WR.", [] // skip
    "BringDLFtoURF", "OBRO.......................BB.....G.BB......B...W..WRW", ["R' U"]
    "BringDLFtoURF", "OB.O..R..W........B........BB.....G.BB..........W..WR.", ["U' F"]
    "BringDLFtoURF", "OB.O...........W....RB.....BB.....G.BB..........W..WR.", ["F"]
    "BringDLFtoURF", "OB.O.......................BB.....G.BBBR.....W..W..WR.", ["F' R"]
    "BringDLFtoURF", "OB.O.............B.....WR..BB.....G.BB..........W..WR.", ["U' R'"; "R' F'"; "F' U'"]
    "BringDLFtoURF", "OBBO.......................BB.....G.BB......W...W..WRR", ["R2"]
    "BringDLFtoURF", "OB.O.......................BB.....G.BB...WB....RW..WR.", ["R2 U"; "F2 U'"]
    "BringDLFtoURF", "OB.O.............W.....RB..BB.....G.BB..........W..WR.", ["U F"; "R U"; "F R"]
    "BringDLFtoURF", "OB.O.......................BB.....G.BB...BR....WW..WR.", ["F'"]
    "BringDLFtoURF", "OB.O.......................BB.....G.BBRW.....B..W..WR.", ["F U'"]
    "BringDLFtoURF", "OB.O..B..R........W........BB.....G.BB..........W..WR.", ["U2"]
    "BringDLFtoURF", "OB.O...........R....BW.....BB.....G.BB..........W..WR.", ["U'"]
    "BringDLFtoURF", "OBWO.......................BB.....G.BB......R...W..WRB", ["R F'"]
    "BringDLFtoURF", "OB.O...........B....WR.....BB.....G.BB..........W..WR.", ["U2 R'"; "F2 R"]
    "BringDLFtoURF", "OB.O..W..B........R........BB.....G.BB..........W..WR.", ["U R'"]
    "BringDLFtoURF", "OB.O....R..B..............WBB.....G.BB..........W..WR.", ["R'"]
    "BringDLFtoURF", "OB.O....W..R..............BBB.....G.BB..........W..WR.", ["U"]
    "BringDLFtoURF", "OB.O....B..W..............RBB.....G.BB..........W..WR.", ["U2 F"; "R2 F'"]
    "BringDLFtoURF", "OB.O.......................BB.....G.BB...RW....BW..WR.", ["R"]
    "BringDLFtoURF", "OB.O.......................BB.....G.BBWB.....R..W..WR.", ["F2"]
    // Pair and insert LF pair (complete FB) [1 case]
    "InsertLFPair", "OB.O.............R.....BW..BB.....G.BB..........W..WR.", ["R' M' F"; "R2 r F"; "r R2 F"; "r' M2 F"; "M' R' F"; "M2 r' F"]
    // Solve DR edge [18 cases] (restricted to U, R/r, F, B, M)
    "DREdge", "O..O.......................BBBR...G.BBBR...G.W..W.WW..", [] // skip
    "DREdge", "O..O...W..G................BBBR...G.BBBR.....W..W..W..", ["r' U' R2"; "r' U' r2"; "B' R B"; "M U' R2"; "M U' r2"]
    "DREdge", "O..O..........W..........G.BBBR...G.BBBR.....W..W..W..", ["R2"; "r2"]
    "DREdge", "O..O............W.....G....BBBR...G.BBBR.....W..W..W..", ["U' R2"; "U' r2"]
    "DREdge", "O..O........G......W.......BBBR...G.BBBR.....W..W..W..", ["U r' U' R2"; "U r' U' r2"; "U B' R B"; "U M U' R2"; "U M U' r2"; "U' r U R2"; "U' r U r2"; "U' F R' F'"; "U' M' U R2"; "U' M' U r2"; "F' U' F R'"; "F' U' F r'"; "B U B' R"; "B U B' r"]
    "DREdge", "O..O.......................BBBR...G.BBBRW....WG.W..W..", ["r U' R2"; "r U' r2"; "M' U' R2"; "M' U' r2"]
    "DREdge", "O..O.W.....................BBBR...GGBBBR.....W..W..W..", ["R"; "r"]
    "DREdge", "O..O.......................BBBR...G.BBBRG....WW.W..W..", ["r2 U R2"; "r2 U r2"; "M2 U R2"; "M2 U r2"]
    "DREdge", "O..O.G.....................BBBR...GWBBBR.....W..W..W..", ["B U B' R2"; "B U B' r2"]
    "DREdge", "OG.O.......................BBBR...G.BBBR.....W..W..WW.", ["r2 U' R2"; "r2 U' r2"; "M2 U' R2"; "M2 U' r2"]
    "DREdge", "O..O............G.....W....BBBR...G.BBBR.....W..W..W..", ["r U R2"; "r U r2"; "F R' F'"; "M' U R2"; "M' U r2"]
    "DREdge", "O..O.......................BBBR.GWG.BBBR.....W..W..W..", ["F' U' F R2"; "F' U' F r2"]
    "DREdge", "O..O..........G..........W.BBBR...G.BBBR.....W..W..W..", ["U r U R2"; "U r U r2"; "U F R' F'"; "U M' U R2"; "U M' U r2"; "U' r' U' R2"; "U' r' U' r2"; "U' B' R B"; "U' M U' R2"; "U' M U' r2"; "F' U F R'"; "F' U F r'"; "B U' B' R"; "B U' B' r"]
    "DREdge", "O..O........W......G.......BBBR...G.BBBR.....W..W..W..", ["U2 R2"; "U2 r2"]
    "DREdge", "O..O.......................BBBR...G.BBBR...W.W..W.GW..", ["R F' U' F R2"; "R F' U' F r2"; "R' B U B' R2"; "R' B U B' r2"; "R2 U r U R2"; "R2 U r U r2"; "R2 U F R' F'"; "R2 U M' U R2"; "R2 U M' U r2"; "R2 U' r' U' R2"; "R2 U' r' U' r2"; "R2 U' B' R B"; "R2 U' M U' R2"; "R2 U' M U' r2"; "R2 F' U F R'"; "R2 F' U F r'"; "R2 B U' B' R"; "R2 B U' B' r"; "r F' U' F R2"; "r F' U' F r2"; "r' B U B' R2"; "r' B U B' r2"; "r2 U r U R2"; "r2 U r U r2"; "r2 U F R' F'"; "r2 U M' U R2"; "r2 U M' U r2"; "r2 U' r' U' R2"; "r2 U' r' U' r2"; "r2 U' B' R B"; "r2 U' M U' R2"; "r2 U' M U' r2"; "r2 F' U F R'"; "r2 F' U F r'"; "r2 B U' B' R"; "r2 B U' B' r"; "F R F' U' R2"; "F R F' U' r2"; "B' R' B U R2"; "B' R' B U r2"]
    "DREdge", "OW.O.......................BBBR...G.BBBR.....W..W..WG.", ["r' U R2"; "r' U r2"; "M U R2"; "M U r2"]
    "DREdge", "O..O.......................BBBR.WGG.BBBR.....W..W..W..", ["R'"; "r'"]
    "DREdge", "O..O...G..W................BBBR...G.BBBR.....W..W..W..", ["U R2"; "U r2"]
    // Tuck RB to FD [16 cases] (restricted to U, R/r, F, B, M)
    "TuckRBtoFD", "O..O.......................BBBR...G.BBBRG..G.WO.W.WW..", [] // skip
    "TuckRBtoFD", "O..O.......................BBBR...G.BBBRO..G.WG.W.WW..", ["M' U2 M2"; "M2 U2 M"]
    "TuckRBtoFD", "O..O...O..G................BBBR...G.BBBR...G.W..W.WW..", ["U2 M"]
    "TuckRBtoFD", "O..O.G.....................BBBR...GOBBBR...G.W..W.WW..", ["R' U R M"; "R' U R2 r'"; "R' U r M2"; "R' U r' R2"; "R' U M R"; "R' U M2 r"; "r' U R M"; "r' U R2 r'"; "r' U r M2"; "r' U r' R2"; "r' U M R"; "r' U M2 r"]
    "TuckRBtoFD", "OO.O.......................BBBR...G.BBBR...G.W..W.WWG.", ["M'"]
    "TuckRBtoFD", "O..O...G..O................BBBR...G.BBBR...G.W..W.WW..", ["M2"]
    "TuckRBtoFD", "O..O............G.....O....BBBR...G.BBBR...G.W..W.WW..", ["M"]
    "TuckRBtoFD", "O..O.......................BBBR.OGG.BBBR...G.W..W.WW..", ["R U' R r2"; "R U' R' M2"; "R U' r' M"; "R U' r2 R"; "R U' M r'"; "R U' M2 R'"; "r U' R r2"; "r U' R' M2"; "r U' r' M"; "r U' r2 R"; "r U' M r'"; "r U' M2 R'"]
    "TuckRBtoFD", "O..O........O......G.......BBBR...G.BBBR...G.W..W.WW..", ["U M2"]
    "TuckRBtoFD", "O..O........G......O.......BBBR...G.BBBR...G.W..W.WW..", ["U' M"]
    "TuckRBtoFD", "O..O.O.....................BBBR...GGBBBR...G.W..W.WW..", ["R' U' R M2"; "R' U' R' r2"; "R' U' r M'"; "R' U' r2 R'"; "R' U' M' r"; "R' U' M2 R"; "r' U' R M2"; "r' U' R' r2"; "r' U' r M'"; "r' U' r2 R'"; "r' U' M' r"; "r' U' M2 R"; "B U2 B' M"]
    "TuckRBtoFD", "O..O.......................BBBR.GOG.BBBR...G.W..W.WW..", ["R U r'"; "r U r'"]
    "TuckRBtoFD", "O..O..........O..........G.BBBR...G.BBBR...G.W..W.WW..", ["U' M2"]
    "TuckRBtoFD", "OG.O.......................BBBR...G.BBBR...G.W..W.WWO.", ["M U2 M"; "M2 U2 M2"]
    "TuckRBtoFD", "O..O..........G..........O.BBBR...G.BBBR...G.W..W.WW..", ["U M"]
    "TuckRBtoFD", "O..O............O.....G....BBBR...G.BBBR...G.W..W.WW..", ["U2 M2"]
    // Bring DRB to ULB [18 cases] (restricted to U, R/r, F, B, M)
    "BringDRBtoULB", "O..O..G..O........W........BBBR...G.BBBRG..G.WO.W.WW..", [] // skip
    "BringDRBtoULB", "O..O.......................BBBR...G.BBBRGWGG.WOOW.WW..", ["R2 B' R' B R'"; "F R2 F' R2 U'"]
    "BringDRBtoULB", "O.WO.......................BBBR...G.BBBRG..GOWO.W.WW.G", ["R' U R U'"; "r' U r U'"; "B U B' U2"; "B U2 B' U"]
    "BringDRBtoULB", "O..O.............G.....WO..BBBR...G.BBBRG..G.WO.W.WW..", ["F' U F"]
    "BringDRBtoULB", "O..O.......................BBBR...G.BBBRGOWG.WOGW.WW..", ["R U2 R'"]
    "BringDRBtoULB", "O..O....O..G..............WBBBR...G.BBBRG..G.WO.W.WW..", ["R' U2 R"; "r' U2 r"]
    "BringDRBtoULB", "O..O...........O....GW.....BBBR...G.BBBRG..G.WO.W.WW..", ["U"]
    "BringDRBtoULB", "O..O...........G....WO.....BBBR...G.BBBRG..G.WO.W.WW..", ["U' F' U F"; "U2 R' U2 R"; "U2 r' U2 r"; "R U2 R' U2"; "B U B' U'"]
    "BringDRBtoULB", "O..O....G..W..............OBBBR...G.BBBRG..G.WO.W.WW..", ["U R U' R'"; "R2 B' R2 B"; "F' U2 F U2"; "B U' B' U"; "B U2 B' U2"]
    "BringDRBtoULB", "O.OO.......................BBBR...G.BBBRG..GGWO.W.WW.W", ["R' U' R"; "r' U' r"]
    "BringDRBtoULB", "O..O..W..G........O........BBBR...G.BBBRG..G.WO.W.WW..", ["U R' U2 R"; "U r' U2 r"; "U2 F' U F"; "R U R' U2"]
    "BringDRBtoULB", "O..O...........W....OG.....BBBR...G.BBBRG..G.WO.W.WW..", ["U' R U' R'"; "R' U' R U'"; "r' U' r U'"]
    "BringDRBtoULB", "O..O..O..W........G........BBBR...G.BBBRG..G.WO.W.WW..", ["U2 R U' R'"; "R' U2 R U'"; "r' U2 r U'"; "F' U' F U2"]
    "BringDRBtoULB", "O.GO.......................BBBR...G.BBBRG..GWWO.W.WW.O", ["B' R2 B R2 U2"]
    "BringDRBtoULB", "O..O.............W.....OG..BBBR...G.BBBRG..G.WO.W.WW..", ["R U' R'"]
    "BringDRBtoULB", "O..O....W..O..............GBBBR...G.BBBRG..G.WO.W.WW..", ["U'"]
    "BringDRBtoULB", "O..O.............O.....GW..BBBR...G.BBBRG..G.WO.W.WW..", ["U2"]
    "BringDRBtoULB", "O..O.......................BBBR...G.BBBRGGOG.WOWW.WW..", ["F' U2 F"]
    // Pair and insert RB pair [1 case] (restricted to U, R/r, F, B, M)
    "InsertRBPair", "O..O..G..O........W........BBBR...G.BBBRG..G.WO.W.WW..", ["R r2 U R"; "R r2 U r"; "R' M2 U R"; "R' M2 U r"; "r' M U R"; "r' M U r"; "r2 R U R"; "r2 R U r"; "M r' U R"; "M r' U r"; "M2 R' U R"; "M2 R' U r"]
    // Tuck RF to BD [14 cases] (restricted to U, R/r, F, B, M)
    "TuckRFtoBD", "OGOO.O.....................BBBR...GGBBBR...GGW..W.WWRW", [] // skip
    "TuckRFtoBD", "O.OO.O.G..R................BBBR...GGBBBR...GGW..W.WW.W", ["U2 M2"]
    "TuckRFtoBD", "O.OO.O.....................BBBR.RGGGBBBR...GGW..W.WW.W", ["R U R r2"; "R U R' M2"; "R U r' M"; "R U r2 R"; "R U M r'"; "R U M2 R'"; "r U R r2"; "r U R' M2"; "r U r' M"; "r U r2 R"; "r U M r'"; "r U M2 R'"; "F' U2 F M'"]
    "TuckRFtoBD", "O.OO.O.R..G................BBBR...GGBBBR...GGW..W.WW.W", ["M'"]
    "TuckRFtoBD", "O.OO.O..........R.....G....BBBR...GGBBBR...GGW..W.WW.W", ["M2"]
    "TuckRFtoBD", "OROO.O.....................BBBR...GGBBBR...GGW..W.WWGW", ["M U2 M2"; "M2 U2 M'"]
    "TuckRFtoBD", "O.OO.O........R..........G.BBBR...GGBBBR...GGW..W.WW.W", ["U M2"]
    "TuckRFtoBD", "O.OO.O......G......R.......BBBR...GGBBBR...GGW..W.WW.W", ["U M'"]
    "TuckRFtoBD", "O.OO.O......R......G.......BBBR...GGBBBR...GGW..W.WW.W", ["U' M2"]
    "TuckRFtoBD", "O.OO.O........G..........R.BBBR...GGBBBR...GGW..W.WW.W", ["U' M'"]
    "TuckRFtoBD", "O.OO.O.....................BBBR.GRGGBBBR...GGW..W.WW.W", ["R U' R' M'"; "R U' R2 r"; "R U' r R2"; "R U' r' M2"; "R U' M' R'"; "R U' M2 r'"; "r U' R' M'"; "r U' R2 r"; "r U' r R2"; "r U' r' M2"; "r U' M' R'"; "r U' M2 r'"]
    "TuckRFtoBD", "O.OO.O..........G.....R....BBBR...GGBBBR...GGW..W.WW.W", ["U2 M'"]
    "TuckRFtoBD", "O.OO.O.....................BBBR...GGBBBRR..GGWG.W.WW.W", ["M"]
    "TuckRFtoBD", "O.OO.O.....................BBBR...GGBBBRG..GGWR.W.WW.W", ["M' U2 M'"; "M2 U2 M2"]
    // Bring DRF to ULF [15 cases] (restricted to U, R/r, F, B, M)
    "BringDRFtoULF", "OGOO.O.........R....WG.....BBBR...GGBBBR...GGW..W.WWRW", [] // skip
    "BringDRFtoULF", "OGOO.OR..G........W........BBBR...GGBBBR...GGW..W.WWRW", ["U2 R U2 R'"; "U2 r U2 r'"; "F' U' F U"]
    "BringDRFtoULF", "OGOO.O..R..W..............GBBBR...GGBBBR...GGW..W.WWRW", ["R' F R F'"]
    "BringDRFtoULF", "OGOO.O.....................BBBR...GGBBBR.WRGGW.GW.WWRW", ["R U' R' U"; "r U' r' U"; "F' U' F U2"; "F' U2 F U'"]
    "BringDRFtoULF", "OGOO.O...........R.....WG..BBBR...GGBBBR...GGW..W.WWRW", ["U"]
    "BringDRFtoULF", "OGOO.O..G..R..............WBBBR...GGBBBR...GGW..W.WWRW", ["U2"]
    "BringDRFtoULF", "OGOO.O..W..G..............RBBBR...GGBBBR...GGW..W.WWRW", ["U R U2 R'"; "U r U2 r'"; "F R' F' R"; "F' U2 F U"]
    "BringDRFtoULF", "OGOO.O.........G....RW.....BBBR...GGBBBR...GGW..W.WWRW", ["U' R U2 R'"; "U' r U2 r'"]
    "BringDRFtoULF", "OGOO.O.....................BBBR...GGBBBR.RGGGW.WW.WWRW", ["R U R'"; "r U r'"]
    "BringDRFtoULF", "OGOO.OW..R........G........BBBR...GGBBBR...GGW..W.WWRW", ["U'"]
    "BringDRFtoULF", "OGOO.O.........W....GR.....BBBR...GGBBBR...GGW..W.WWRW", ["R U2 R' U"; "r U2 r' U"]
    "BringDRFtoULF", "OGOO.O...........W.....GR..BBBR...GGBBBR...GGW..W.WWRW", ["F' U F U'"; "F' U2 F U2"]
    "BringDRFtoULF", "OGOO.OG..W........R........BBBR...GGBBBR...GGW..W.WWRW", ["R U R' U"; "r U r' U"]
    "BringDRFtoULF", "OGOO.O.....................BBBR...GGBBBR.GWGGW.RW.WWRW", ["R U' B U' B' R'"; "R B U2 B' R' U"; "R2 B' R' B U' R'"; "R2 B' R' B R' U'"; "r U' B U' B' r'"; "r B U2 B' r' U"; "r2 B r2 B' U2 B"; "F U2 F U2 F' U2"; "F R' F' R2 U2 R'"; "F' U F R U2 R'"; "F' U F r U2 r'"; "B r2 B r2 B' U2"]
    "BringDRFtoULF", "OGOO.O...........G.....RW..BBBR...GGBBBR...GGW..W.WWRW", ["R U2 R'"; "r U2 r'"]
    // Pair and insert RF pair (complete SB) [1 case] (restricted to U, R/r, F, B, M)
    "InsertRFPair", "OGOO.O.........R....WG.....BBBR...GGBBBR...GGW..W.WWRW", ["R M2 U' R'"; "R M2 U' r'"; "R' r2 U' R'"; "R' r2 U' r'"; "r M' U' R'"; "r M' U' r'"; "r2 R' U' R'"; "r2 R' U' r'"; "M' r U' R'"; "M' r U' r'"; "M2 R U' R'"; "M2 R U' r'"]
    // Orient corners (CO) - hand authored patterns [27 cases] - single-alg (sune)
    "CornerOrientation", "O.OO.O...Y.Y...Y.Y.........BBBR.RGGGBBBR.RGGGW.WW.WW.W", [] // skip
    "CornerOrientation", "O.OO.OY.Y............Y.Y...BBBR.RGGGBBBR.RGGGW.WW.WW.W", [coH] // H
    "CornerOrientation", "O.OO.O............Y.Y...Y.YBBBR.RGGGBBBR.RGGGW.WW.WW.W", ["U " + coH] // (AUF U/U')
    "CornerOrientation", "O.OO.OY..............Y..Y.YBBBR.RGGGBBBR.RGGGW.WW.WW.W", [coPi] // Pi
    "CornerOrientation", "O.OO.OY.Y...........Y...Y..BBBR.RGGGBBBR.RGGGW.WW.WW.W", ["U " + coPi]
    "CornerOrientation", "O.OO.O..Y.........Y.Y..Y...BBBR.RGGGBBBR.RGGGW.WW.WW.W", ["U2 " + coPi]
    "CornerOrientation", "O.OO.O............Y..Y.Y..YBBBR.RGGGBBBR.RGGGW.WW.WW.W", ["U' " + coPi]
    "CornerOrientation", "O.OO.O...Y.Y.........Y.Y...BBBR.RGGGBBBR.RGGGW.WW.WW.W", [coU] // U
    "CornerOrientation", "O.OO.O...Y.....Y........Y.YBBBR.RGGGBBBR.RGGGW.WW.WW.W", ["U " + coU]
    "CornerOrientation", "O.OO.OY.Y......Y.Y.........BBBR.RGGGBBBR.RGGGW.WW.WW.W", ["U2 " + coU]
    "CornerOrientation", "O.OO.O.....Y.....YY.Y......BBBR.RGGGBBBR.RGGGW.WW.WW.W", ["U' " + coU]
    "CornerOrientation", "O.OO.OY....Y.....Y...Y.....BBBR.RGGGBBBR.RGGGW.WW.WW.W", [coT] // T
    "CornerOrientation", "O.OO.O...Y.Y........Y...Y..BBBR.RGGGBBBR.RGGGW.WW.WW.W", ["U " + coT]
    "CornerOrientation", "O.OO.O..YY.....Y.......Y...BBBR.RGGGBBBR.RGGGW.WW.WW.W", ["U2 " + coT]
    "CornerOrientation", "O.OO.O.........Y.YY.......YBBBR.RGGGBBBR.RGGGW.WW.WW.W", ["U' " + coT]
    "CornerOrientation", "O.OO.OY........Y.......Y..YBBBR.RGGGBBBR.RGGGW.WW.WW.W", [coS] // sune
    "CornerOrientation", "O.OO.OY..........Y..Y.....YBBBR.RGGGBBBR.RGGGW.WW.WW.W", ["U " + coS]
    "CornerOrientation", "O.OO.OY....Y........Y..Y...BBBR.RGGGBBBR.RGGGW.WW.WW.W", ["U2 " + coS]
    "CornerOrientation", "O.OO.O...Y..........Y..Y..YBBBR.RGGGBBBR.RGGGW.WW.WW.W", ["U' " + coS]
    "CornerOrientation", "O.OO.O..Y......Y..Y.....Y..BBBR.RGGGBBBR.RGGGW.WW.WW.W", [coAs] // anti-sune
    "CornerOrientation", "O.OO.O..Y........YY..Y.....BBBR.RGGGBBBR.RGGGW.WW.WW.W", ["U " + coAs]
    "CornerOrientation", "O.OO.O.....Y......Y..Y..Y..BBBR.RGGGBBBR.RGGGW.WW.WW.W", ["U2 " + coAs]
    "CornerOrientation", "O.OO.O..YY...........Y..Y..BBBR.RGGGBBBR.RGGGW.WW.WW.W", ["U' " + coAs]
    "CornerOrientation", "O.OO.O...Y.......Y...Y....YBBBR.RGGGBBBR.RGGGW.WW.WW.W", [coL] // L
    "CornerOrientation", "O.OO.OY....Y...Y........Y..BBBR.RGGGBBBR.RGGGW.WW.WW.W", ["U " + coL]
    "CornerOrientation", "O.OO.O..YY.......Y..Y......BBBR.RGGGBBBR.RGGGW.WW.WW.W", ["U2 " + coL]
    "CornerOrientation", "O.OO.O.....Y...Y..Y....Y...BBBR.RGGGBBBR.RGGGW.WW.WW.W", ["U' " + coL]
    // Permute corners (CP) - hand authored patterns [20 + 4 cases] - single-alg (jperm) [final AUF is unnecessary]
    "CornerPermutation", "O.OO.OO.OY.Y...Y.YB.BR.RG.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", [] // skip
    "CornerPermutation", "O.OO.OG.GY.Y...Y.YO.OB.BR.RBBBR.RGGGBBBR.RGGGW.WW.WW.W", ["U"] // skip
    "CornerPermutation", "O.OO.OR.RY.Y...Y.YG.GO.OB.BBBBR.RGGGBBBR.RGGGW.WW.WW.W", ["U2"] // skip
    "CornerPermutation", "O.OO.OB.BY.Y...Y.YR.RG.GO.OBBBR.RGGGBBBR.RGGGW.WW.WW.W", ["U'"] // skip
    "CornerPermutation", "O.OO.OG.RY.Y...Y.YO.OB.RG.BBBBR.RGGGBBBR.RGGGW.WW.WW.W", [jperm] // O
    "CornerPermutation", "O.OO.OB.GY.Y...Y.YR.GO.OB.RBBBR.RGGGBBBR.RGGGW.WW.WW.W", ["U " + jperm]
    "CornerPermutation", "O.OO.OR.BY.Y...Y.YG.BR.GO.OBBBR.RGGGBBBR.RGGGW.WW.WW.W", ["U2 " + jperm]
    "CornerPermutation", "O.OO.OO.OY.Y...Y.YB.RG.BR.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", ["U' " + jperm]
    "CornerPermutation", "O.OO.OR.BY.Y...Y.YG.GO.BR.OBBBR.RGGGBBBR.RGGGW.WW.WW.W", [jperm + " U"] // G
    "CornerPermutation", "O.OO.OO.RY.Y...Y.YB.RG.GO.BBBBR.RGGGBBBR.RGGGW.WW.WW.W", ["U " + jperm + " U"]
    "CornerPermutation", "O.OO.OB.OY.Y...Y.YR.OB.RG.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", ["U2 " + jperm + " U"]
    "CornerPermutation", "O.OO.OG.GY.Y...Y.YO.BR.OB.RBBBR.RGGGBBBR.RGGGW.WW.WW.W", ["U' " + jperm + " U"]
    "CornerPermutation", "O.OO.OB.OY.Y...Y.YR.RG.OB.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", [jperm + " U2"] // R
    "CornerPermutation", "O.OO.OG.BY.Y...Y.YO.BR.RG.OBBBR.RGGGBBBR.RGGGW.WW.WW.W", ["U " + jperm + " U2"]
    "CornerPermutation", "O.OO.OO.GY.Y...Y.YB.GO.BR.RBBBR.RGGGBBBR.RGGGW.WW.WW.W", ["U2 " + jperm + " U2"]
    "CornerPermutation", "O.OO.OR.RY.Y...Y.YG.OB.GO.BBBBR.RGGGBBBR.RGGGW.WW.WW.W", ["U' " + jperm + " U2"]
    "CornerPermutation", "O.OO.OO.GY.Y...Y.YB.BR.GO.RBBBR.RGGGBBBR.RGGGW.WW.WW.W", [jperm + " U'"] // B
    "CornerPermutation", "O.OO.OR.OY.Y...Y.YG.OB.BR.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", ["U " + jperm + " U'"]
    "CornerPermutation", "O.OO.OG.RY.Y...Y.YO.RG.OB.BBBBR.RGGGBBBR.RGGGW.WW.WW.W", ["U2 " + jperm + " U'"]
    "CornerPermutation", "O.OO.OB.BY.Y...Y.YR.GO.RG.OBBBR.RGGGBBBR.RGGGW.WW.WW.W", ["U' " + jperm + " U'"]
    "CornerPermutation", "O.OO.OR.OY.Y...Y.YG.BR.OB.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", [jperm + " U " + jperm] // diag
    "CornerPermutation", "O.OO.OB.GY.Y...Y.YR.OB.GO.RBBBR.RGGGBBBR.RGGGW.WW.WW.W", [jperm + " U " + jperm + " U"]
    "CornerPermutation", "O.OO.OO.RY.Y...Y.YB.GO.RG.BBBBR.RGGGBBBR.RGGGW.WW.WW.W", [jperm + " U " + jperm + " U2"]
    "CornerPermutation", "O.OO.OG.BY.Y...Y.YO.RG.BR.OBBBR.RGGGBBBR.RGGGW.WW.WW.W", [jperm + " U " + jperm + " U'"]
    // Orient center - hand authored patterns [4 cases]
    "CenterOrientation", "O.OO.OO.OY.Y.Y.Y.YB.BR.RG.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", [] // skip
    "CenterOrientation", "O.OO.OO.OY.Y.W.Y.YB.BR.RG.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", [] // skip
    "CenterOrientation", "O.OO.OO.OY.Y.R.Y.YB.BR.RG.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", ["M"] // or M'
    "CenterOrientation", "O.OO.OO.OY.Y.O.Y.YB.BR.RG.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", ["M"] // or M'
]

let solve = solveCase patterns

let genRoux () =
    let numCubes = 1000
    printfn "Scrambling %i cubes" numCubes
    let scrambled = List.init numCubes (fun _ -> printf "."; scramble 20 |> fst)
    printfn ""

    // First Block (FB)

    // DL
    let caseDL c = look Face.D Sticker.L c = Color.W && look Face.L Sticker.D c = Color.B
    let rotations = [Rotate X; Rotate X'; Rotate X2; Rotate Y; Rotate Y'; Rotate Y2; Rotate Z; Rotate Z'; Rotate Z2]
    let solvedDL = solve rotations "Solve DL edge (during inspection)" "DLEdge" caseDL scrambled

    // center
    let caseLC c = caseDL c && look Face.L Sticker.C c = Color.B
    let moves = [Move Move.U; Move Move.U'; Move Move.U2; Move Move.UW; Move Move.UW'; Move Move.UW2
                 Move Move.D; Move Move.D'; Move Move.D2; Move Move.DW; Move Move.DW'; Move Move.DW2
                 Move Move.L; Move Move.L'; Move Move.L2; Move Move.LW; Move Move.LW'; Move Move.LW2
                 Move Move.R; Move Move.R'; Move Move.R2; Move Move.RW; Move Move.RW'; Move Move.RW2
                 Move Move.F; Move Move.F'; Move Move.F2; Move Move.FW; Move Move.FW'; Move Move.FW2
                 Move Move.B; Move Move.B'; Move Move.B2; Move Move.BW; Move Move.BW'; Move Move.BW2
                 Move Move.M; Move Move.M'; Move Move.M2;
                 Move Move.S; Move Move.S'; Move Move.S2;
                 Move Move.E; Move Move.E'; Move Move.E2]
    let solvedLC = solve moves "Solve L center" "LCenter" caseLC solvedDL

    // LB pair
    let caseLBtoFD c = caseLC c && look Face.F Sticker.D c = Color.B && look Face.D Sticker.U c = Color.O
    let solvedLBtoFD = solve moves "Tuck LB to FD" "TuckLBtoFD" caseLBtoFD solvedLC

    let caseDLBtoURB c = caseLBtoFD c && look Face.U Sticker.UR c = Color.O && look Face.R Sticker.UR c = Color.W && look Face.B Sticker.DR c = Color.B
    let solvedDLBtoURB = solve moves "Bring DLB corner to URB" "BringDLBtoURB" caseDLBtoURB solvedLBtoFD

    let caseLBPair c = caseLC c && look Face.L Sticker.L c = Color.B && look Face.L Sticker.DL c = Color.B && look Face.B Sticker.UL c = Color.O && look Face.B Sticker.L c = Color.O && look Face.D Sticker.DL c = Color.W
    let solvedLBPair = solve moves "Pair and insert LB pair" "InsertLBPair" caseLBPair solvedDLBtoURB

    // LF pair
    let caseLFtoBD c = caseLBPair c && look Face.B Sticker.U c = Color.B && look Face.D Sticker.D c = Color.R
    let solvedLFtoBD = solve moves "Tuck LF to BD" "TuckLFtoBD" caseLFtoBD solvedLBPair

    let caseDLFtoURF c = caseLFtoBD c && look Face.U Sticker.DR c = Color.R && look Face.R Sticker.UL c = Color.W && look Face.F Sticker.UR c = Color.B
    let solvedDLFtoURF = solve moves "Bring DLF corner to URF" "BringDLFtoURF" caseDLFtoURF solvedLFtoBD

    let caseSolvedFB c = caseLBPair c && look Face.L Sticker.R c = Color.B && look Face.L Sticker.DR c = Color.B && look Face.F Sticker.DL c = Color.R && look Face.F Sticker.L c = Color.R && look Face.D Sticker.UL c = Color.W
    let solvedFB = solve moves "Pair and insert LF pair (complete FB)" "InsertLFPair" caseSolvedFB solvedDLFtoURF

    // Second Block (SB)

    // DR (center done)
    let caseDR c = caseSolvedFB c && look Face.R Sticker.D c = Color.G && look Face.D Sticker.R c = Color.W
    let sbMoves = [Move Move.U; Move Move.U'; Move Move.U2
                   Move Move.R; Move Move.R'; Move Move.R2; Move Move.RW; Move Move.RW'; Move Move.RW2
                   Move Move.F; Move Move.F'
                   Move Move.B; Move Move.B'
                   Move Move.M; Move Move.M'; Move Move.M2]
    let solvedDR = solve sbMoves "Solve DR edge" "DREdge" caseDR solvedFB

    // RB pair
    let caseRBtoFD c = caseDR c && look Face.F Sticker.D c = Color.G && look Face.D Sticker.U c = Color.O
    let solvedRBtoFD = solve sbMoves "Tuck RB to FD" "TuckRBtoFD" caseRBtoFD solvedDR

    let caseDRBtoULB c = caseRBtoFD c && look Face.U Sticker.UL c = Color.O && look Face.L Sticker.UL c = Color.W && look Face.B Sticker.DL c = Color.G
    let solvedDRBtoULB = solve sbMoves "Bring DRB to ULB" "BringDRBtoULB" caseDRBtoULB solvedRBtoFD

    let caseRBPair c = caseDR c && look Face.R Sticker.R c = Color.G && look Face.R Sticker.DR c = Color.G && look Face.B Sticker.UR c = Color.O && look Face.B Sticker.R c = Color.O && look Face.D Sticker.DR c = Color.W
    let solvedRBPair = solve sbMoves "Pair and insert RB pair" "InsertRBPair" caseRBPair solvedDRBtoULB

    // RF pair
    let caseRFtoBD c = caseRBPair c && look Face.B Sticker.U c = Color.G && look Face.D Sticker.D c = Color.R
    let solvedRFtoBD = solve sbMoves "Tuck RF to BD" "TuckRFtoBD" caseRFtoBD solvedRBPair

    let caseDRFtoULF c = caseRFtoBD c && look Face.U Sticker.DL c = Color.R && look Face.L Sticker.UR c = Color.W && look Face.F Sticker.UL c = Color.G
    let solvedDRFtoULF = solve sbMoves "Bring DRF to ULF" "BringDRFtoULF" caseDRFtoULF solvedRFtoBD

    let caseSolvedSB c = caseRBPair c && look Face.R Sticker.L c = Color.G && look Face.R Sticker.DL c = Color.G && look Face.F Sticker.DR c = Color.R && look Face.F Sticker.R c = Color.R && look Face.D Sticker.UR c = Color.W
    let solvedSB = solve sbMoves "Pair and insert RF pair (complete SB)" "InsertRFPair" caseSolvedSB solvedDRFtoULF

    // Orient corners (CO) - hand authored patterns
    let caseCO c = caseSolvedSB c && look Face.U Sticker.UL c = Color.Y && look Face.U Sticker.UR c = Color.Y && look Face.U Sticker.DL c = Color.Y && look Face.U Sticker.DR c = Color.Y
    let solvedCO = solve moves "Orient corners (CO)" "CornerOrientation" caseCO solvedSB

    // Permute corners (CP) - hand authored patterns
    let rufMoves = [Move Move.U; Move Move.U'; Move Move.U2; Move Move.R; Move Move.R'; Move Move.R2; Move Move.F; Move Move.F'; Move Move.F2]
    let lookCornerFaceStickers sticker0 sticker1 face col c = look face sticker0 c = col && look face sticker1 c = col
    let lookCornerFace = lookCornerFaceStickers Sticker.UL Sticker.UR
    let lookB = lookCornerFaceStickers Sticker.DL Sticker.DR Face.B
    let lookL = lookCornerFace Face.L
    let lookR = lookCornerFace Face.R
    let lookF = lookCornerFace Face.F
    let caseCP c =
        let caseCP0 = lookB Color.O c && lookF Color.R c && lookL Color.B c && lookR Color.G c
        let caseCP1 = lookB Color.B c && lookF Color.G c && lookL Color.R c && lookR Color.O c
        let caseCP2 = lookB Color.R c && lookF Color.O c && lookL Color.G c && lookR Color.B c
        let caseCP3 = lookB Color.G c && lookF Color.B c && lookL Color.O c && lookR Color.R c
        caseCO c && (caseCP0 || caseCP1 || caseCP2 || caseCP3)
    let solvedCP = solve rufMoves "Permute corners (CP)" "CornerPermutation" caseCP solvedCO

    // Orient center (note: generated patterns and algs are not distinct because goal is flexible U/D colors)
    let mMoves = [Move Move.M; Move Move.M'; Move Move.M2]
    let caseCenterO c = caseCP c && (look Face.U Sticker.C c = Color.W || look Face.U Sticker.C c = Color.Y)
    let solvedCenterO = solve mMoves "Orient center" "CenterOrientation" caseCenterO solvedCP

    // Orient edges (EO) (note: generated patterns and algs are not distinct because goal is flexible U/D colors)
    let mud2Moves = mMoves @ [Move Move.U; Move Move.U'; Move Move.U2; Move Move.D2]
    let caseEO c = caseCenterO c &&
                   (look Face.U Sticker.L c = Color.W || look Face.U Sticker.L c = Color.Y) &&
                   (look Face.U Sticker.U c = Color.W || look Face.U Sticker.U c = Color.Y) &&
                   (look Face.U Sticker.R c = Color.W || look Face.U Sticker.R c = Color.Y) &&
                   (look Face.U Sticker.D c = Color.W || look Face.U Sticker.D c = Color.Y) &&
                   (look Face.D Sticker.U c = Color.W || look Face.D Sticker.U c = Color.Y) &&
                   (look Face.D Sticker.D c = Color.W || look Face.D Sticker.D c = Color.Y)
    let solvedEO = solve mud2Moves "Orient edges (EO)" "EdgeOrientation" caseEO solvedCenterO

    pause ()
genRoux ()

printfn "DONE!"
Console.ReadLine() |> ignore