open System
open Cube
open Solver
open Render

// single-alg CO
let sune = "R U R' U R U2 R'"
let jperm = "R U R' F' R U R' U' R' F R2 U' R'" // without final AUF (U')
let mum = "M' U' M'"

let dlEdgePatterns = [
    // Solving DL edge (during inspection) [24 cases]
    "DLEdge",  (".....................................B..........W.....", false, false, false), [] // skip
    "DLEdge",  ("...W.......................B..........................", false, false, false), ["x"]
    "DLEdge",  (".W..................................................B.", false, false, false), ["x y"; "y z"; "z x"]
    "DLEdge",  (".......W..B...........................................", false, false, false), ["x y'"; "y' z'"; "z' x"]
    "DLEdge",  (".....W.............................B..................", false, false, false), ["x y2"; "x' z2"; "y2 x'"; "z2 x"]
    "DLEdge",  (".....B.............................W..................", false, false, false), ["x z"; "y' x"; "z y'"]
    "DLEdge",  (".............................WB.......................", false, false, false), ["x z'"; "y x"; "z' y"]
    "DLEdge",  ("................................WB....................", false, false, false), ["x z2"; "x' y2"; "y2 x"; "z2 x'"]
    "DLEdge",  (".............................BW.......................", false, false, false), ["x'"]
    "DLEdge",  ("................B.....W...............................", false, false, false), ["x' y"; "y z'"; "z' x'"]
    "DLEdge",  ("........................................W.....B.......", false, false, false), ["x' y'"; "y' z"; "z x'"]
    "DLEdge",  ("................................BW....................", false, false, false), ["x' z"; "y x'"; "z y"]
    "DLEdge",  ("...B.......................W..........................", false, false, false), ["x' z'"; "y' x'"; "z' y'"]
    "DLEdge",  ("............W......B..................................", false, false, false), ["x2"]
    "DLEdge",  (".......B..W...........................................", false, false, false), ["x2 y"; "y z2"; "y' x2"; "z2 y'"]
    "DLEdge",  ("................W.....B...............................", false, false, false), ["x2 y'"; "y x2"; "y' z2"; "z2 y"]
    "DLEdge",  ("..............B..........W............................", false, false, false), ["x2 z"; "y2 z'"; "z y2"; "z' x2"]
    "DLEdge",  (".....................................W..........B.....", false, false, false), ["x2 z'"; "y2 z"; "z x2"; "z' y2"]
    "DLEdge",  ("........................................B.....W.......", false, false, false), ["y"]
    "DLEdge",  (".B..................................................W.", false, false, false), ["y'"]
    "DLEdge",  ("...........................................B......W...", false, false, false), ["y2"]
    "DLEdge",  ("...........................................W......B...", false, false, false), ["z"]
    "DLEdge",  ("............B......W..................................", false, false, false), ["z'"]
    "DLEdge",  ("..............W..........B............................", false, false, false), ["z2"]]

let lCenterPatterns = [
    // Solve left center [6 cases]
    "LCenter", ("............................B.....G..B..........W.....", false, false, false), [] // skip
    "LCenter", ("....B..........................G.....B..........W.....", false, false, false), ["E"; "u'"]
    "LCenter", ("....G..........................B.....B..........W.....", false, false, false), ["u"; "E'"]
    "LCenter", ("............................G.....B..B..........W.....", false, false, false), ["u2"; "E2"]
    "LCenter", (".............G.......................B..........WB....", false, false, false), ["r u"; "r E'"; "r' u'"; "r' E"; "M u'"; "M E"; "M' u"; "M' E'"]
    "LCenter", (".............B.......................B..........WG....", false, false, false), ["r u'"; "r E"; "r' u"; "r' E'"; "M u"; "M E'"; "M' u'"; "M' E"]]

let lbPairPatterns = [
    // Tuck LB edge to FD [22 cases]
    "TuckLBtoFD", ("............................B.....G..B..B.....O.W.....", false, false, false), [] // skip
    "TuckLBtoFD", ("...O.......................BB.....G..B..........W.....", false, false, false), ["B r"; "B M'"]
    "TuckLBtoFD", (".....B......................B.....GO.B..........W.....", false, false, false), ["R2 F"; "r2 F"; "B r2"; "B M2"]
    "TuckLBtoFD", (".....O......................B.....GB.B..........W.....", false, false, false), ["B' r"; "B' M'"]
    "TuckLBtoFD", ("...B.......................OB.....G..B..........W.....", false, false, false), ["B' r2"; "B' M2"]
    "TuckLBtoFD", (".......O..B.................B.....G..B..........W.....", false, false, false), ["U2 r'"; "U2 M"; "r' F2"; "B2 r"; "B2 M'"; "M F2"]
    "TuckLBtoFD", (".B..........................B.....G..B..........W...O.", false, false, false), ["r2 F2"; "B2 r2"; "B2 M2"; "M2 F2"]
    "TuckLBtoFD", ("............................B...BOG..B..........W.....", false, false, false), ["F"]
    "TuckLBtoFD", ("............................BBO...G..B..........W.....", false, false, false), ["F r'"; "F M"]
    "TuckLBtoFD", ("............................BOB...G..B..........W.....", false, false, false), ["F'"]
    "TuckLBtoFD", ("............................B...OBG..B..........W.....", false, false, false), ["F' r'"; "F' M"]
    "TuckLBtoFD", ("................O.....B.....B.....G..B..........W.....", false, false, false), ["F2"]
    "TuckLBtoFD", ("............................B.....G..B..O.....B.W.....", false, false, false), ["r F2"; "F2 r'"; "F2 M"; "M' F2"]
    "TuckLBtoFD", ("............................B.....G..B.....B....W.O...", false, false, false), ["L D' L'"; "L' D' L"; "L2 D' L2"; "l D' L'"; "l' D' L"; "l2 D' L2"; "R F' r'"; "R F' M"; "R' B' r"; "R' B' M'"; "R2 U F2"; "R2 U' r2"; "R2 U' M2"; "r F' r'"; "r F' M"; "r' B' r"; "r' B' M'"; "r2 U F2"; "r2 U' r2"; "r2 U' M2"]
    "TuckLBtoFD", ("................B.....O.....B.....G..B..........W.....", false, false, false), ["r'"; "M"]
    "TuckLBtoFD", (".O..........................B.....G..B..........W...B.", false, false, false), ["r"; "M'"]
    "TuckLBtoFD", (".......B..O.................B.....G..B..........W.....", false, false, false), ["r2"; "M2"]
    "TuckLBtoFD", ("............................B.....G..B.....O....W.B...", false, false, false), ["R F"; "r F"]
    "TuckLBtoFD", ("..............B..........O..B.....G..B..........W.....", false, false, false), ["U r'"; "U M"; "R' F"; "r' F"]
    "TuckLBtoFD", ("..............O..........B..B.....G..B..........W.....", false, false, false), ["U F2"; "U' r2"; "U' M2"]
    "TuckLBtoFD", ("............O......B........B.....G..B..........W.....", false, false, false), ["U r2"; "U M2"; "U' F2"]
    "TuckLBtoFD", ("............B......O........B.....G..B..........W.....", false, false, false), ["U' r'"; "U' M"]
    // Bring DLB corner to URB [24 cases]
    "BringDLBtoURB", ("........B..O..............W.B.....G..B..B.....O.W.....", false, false, false), [] // skip
    "BringDLBtoURB", ("..B.........................B.....G..B..B...O.O.W....W", false, false, false), ["B"]
    "BringDLBtoURB", ("...............W....BO......B.....G..B..B.....O.W.....", false, false, false), ["U B'"]
    "BringDLBtoURB", (".................O.....WB...B.....G..B..B.....O.W.....", false, false, false), ["U'"]
    "BringDLBtoURB", ("...............O....WB......B.....G..B..B.....O.W.....", false, false, false), ["U2"]
    "BringDLBtoURB", ("............................B.....G..B..BBW...OOW.....", false, false, false), ["R2"]
    "BringDLBtoURB", ("...............B....OW......B.....G..B..B.....O.W.....", false, false, false), ["U' R"]
    "BringDLBtoURB", ("......B..W........O.........B.....G..B..B.....O.W.....", false, false, false), ["B'"]
    "BringDLBtoURB", ("............................B.....G..B..BOB...OWW.....", false, false, false), ["R U'"]
    "BringDLBtoURB", ("............................B.....G..B..BWO...OBW.....", false, false, false), ["R' B"]
    "BringDLBtoURB", (".................B.....OW...B.....G..B..B.....O.W.....", false, false, false), ["R"]
    "BringDLBtoURB", ("............................B.....G..BWOB....BO.W.....", false, false, false), ["L' U2 L"; "r' F2 r"; "F2 R F2"; "b2 L b2"; "M F2 r"; "S2 L b2"]
    "BringDLBtoURB", ("O...........................B.....G.BB..B.....O.W..W..", false, false, false), ["B R'"]
    "BringDLBtoURB", ("........W..B..............O.B.....G..B..B.....O.W.....", false, false, false), ["U R"; "R B"; "B U"]
    "BringDLBtoURB", ("B...........................B.....G.WB..B.....O.W..O..", false, false, false), ["B2"]
    "BringDLBtoURB", ("......W..O........B.........B.....G..B..B.....O.W.....", false, false, false), ["U"]
    "BringDLBtoURB", ("............................B.....G..BBWB....OO.W.....", false, false, false), ["L' U B' L"; "L2 u2 b2 U'"; "L2 u2 S2 U'"; "r' F U' r"; "r' F2 U' M'"; "r' F2 M' U'"; "r2 U2 F2 U'"; "r2 F2 M2 U'"; "F U' R F'"; "F U' r F'"; "F2 U2 r2 U"; "F2 U2 M2 U"; "b L' U b'"; "b L' u b'"; "b L2 U S"; "b L2 u S"; "b L2 S U"; "b2 u2 L2 U"; "b2 L2 S2 U"; "M F U' r"; "M F2 U' M'"; "M F2 M' U'"; "M2 U2 F2 U'"; "M2 F2 M2 U'"; "S' L' U b'"; "S' L' u b'"; "S' L2 U S"; "S' L2 u S"; "S' L2 S U"; "S2 u2 L2 U"; "S2 L2 S2 U"]
    "BringDLBtoURB", ("........O..W..............B.B.....G..B..B.....O.W.....", false, false, false), ["U' B'"; "R' U'"; "B' R'"]
    "BringDLBtoURB", ("..W.........................B.....G..B..B...B.O.W....O", false, false, false), ["R2 U'"; "B2 U"]
    "BringDLBtoURB", ("............................B.....G..BOBB....WO.W.....", false, false, false), ["L2 B' L2"; "r2 F' r2 | F U2 F' | b L2 b' | M2 F' r2 | S' L2 b'"]
    "BringDLBtoURB", (".................W.....BO...B.....G..B..B.....O.W.....", false, false, false), ["U2 B'"; "R2 B"]
    "BringDLBtoURB", ("W...........................B.....G.OB..B.....O.W..B..", false, false, false), ["B' U"]
    "BringDLBtoURB", ("..O.........................B.....G..B..B...W.O.W....B", false, false, false), ["R'"]
    "BringDLBtoURB", ("......O..B........W.........B.....G..B..B.....O.W.....", false, false, false), ["U2 R"; "B2 R'"]
    // Insert LB pair [1 case]
    // "InsertLBPair", ("O..O.......................BB.....G.BB..........W..W..", false, false), [] // skip TODO: this should be hierarchical (before TuckLBtoFD & BringDLBtoURB)
    "InsertLBPair", ("........B..O..............W.B.....G..B..B.....O.W.....", false, false, false), ["R M B'"; "R2 r' B'"; "r M2 B'"; "r' R2 B'"; "M R B'"; "M2 r B'"]]

let lfPairPatterns = [
    // Tuck LF to BD [20 cases]
    "TuckLFtoBD", ("OB.O.......................BB.....G.BB..........W..WR.", false, false, false), [] // skip
    "TuckLFtoBD", ("O..O............R.....B....BB.....G.BB..........W..W..", false, false, false), ["r2"; "M2"]
    "TuckLFtoBD", ("O..O.......................BB...RBG.BB..........W..W..", false, false, false), ["F r'"; "F M"]
    "TuckLFtoBD", ("O..O.......................BBBR...G.BB..........W..W..", false, false, false), ["F' r'"; "F' M"]
    "TuckLFtoBD", ("O..O........R......B.......BB.....G.BB..........W..W..", false, false, false), ["U' r2"; "U' M2"]
    "TuckLFtoBD", ("O..O...R..B................BB.....G.BB..........W..W..", false, false, false), ["r"; "M'"]
    "TuckLFtoBD", ("O..O.......................BB.....G.BB.....B....W.RW..", false, false, false), ["L D L'"; "L2 D L2"; "l D L'"; "l2 D L2"; "R F r'"; "R F M"; "R2 U r2"; "R2 U M2"; "r F r'"; "r F M"; "r2 U r2"; "r2 U M2"]
    "TuckLFtoBD", ("OR.O.......................BB.....G.BB..........W..WB.", false, false, false), ["r F2 r2"; "r F2 M2"; "r' U2 r2"; "r' U2 M2"; "r2 U2 r"; "r2 U2 M'"; "r2 F2 r'"; "r2 F2 M"; "M U2 r2"; "M U2 M2"; "M' F2 r2"; "M' F2 M2"; "M2 U2 r"; "M2 U2 M'"; "M2 F2 r'"; "M2 F2 M"]
    "TuckLFtoBD", ("O..O.......................BB...BRG.BB..........W..W..", false, false, false), ["F' r2"; "F' M2"]
    "TuckLFtoBD", ("O..O.......................BB.....G.BB.....R....W.BW..", false, false, false), ["R F' r2"; "R F' M2"; "R2 U' r"; "R2 U' M'"; "r F' r2"; "r F' M2"; "r2 U' r"; "r2 U' M'"]
    "TuckLFtoBD", ("O..O............B.....R....BB.....G.BB..........W..W..", false, false, false), ["U2 r"; "U2 M'"; "F2 r'"; "F2 M"]
    "TuckLFtoBD", ("O..O.......................BB.....G.BB..B.....R.W..W..", false, false, false), ["F2 r2"; "F2 M2"]
    "TuckLFtoBD", ("O..O..........B..........R.BB.....G.BB..........W..W..", false, false, false), ["U' r"; "U' M'"]
    "TuckLFtoBD", ("O..O.......................BBRB...G.BB..........W..W..", false, false, false), ["F r2"; "F M2"]
    "TuckLFtoBD", ("O..O.B.....................BB.....GRBB..........W..W..", false, false, false), ["L' B' L"; "L2 B' L2"; "l' B' L"; "l2 B' L2"; "R' U' r"; "R' U' M'"; "R2 F' r2"; "R2 F' M2"; "r' U' r"; "r' U' M'"; "r2 F' r2"; "r2 F' M2"]
    "TuckLFtoBD", ("O..O........B......R.......BB.....G.BB..........W..W..", false, false, false), ["U r"; "U M'"]
    "TuckLFtoBD", ("O..O..........R..........B.BB.....G.BB..........W..W..", false, false, false), ["U r2"; "U M2"]
    "TuckLFtoBD", ("O..O.......................BB.....G.BB..R.....B.W..W..", false, false, false), ["r'"; "M"]
    "TuckLFtoBD", ("O..O.R.....................BB.....GBBB..........W..W..", false, false, false), ["R' U r2"; "R' U M2"; "R2 F r'"; "R2 F M"; "r' U r2"; "r' U M2"; "r2 F r'"; "r2 F M"]
    "TuckLFtoBD", ("O..O...B..R................BB.....G.BB..........W..W..", false, false, false), ["U2 r2"; "U2 M2"]
    // Bring DLF corner to URF [21 cases]
    "BringDLFtoURF", ("OB.O.............R.....BW..BB.....G.BB..........W..WR.", false, false, false), [] // skip
    "BringDLFtoURF", ("OBRO.......................BB.....G.BB......B...W..WRW", false, false, false), ["R' U"]
    "BringDLFtoURF", ("OB.O..R..W........B........BB.....G.BB..........W..WR.", false, false, false), ["U' F"]
    "BringDLFtoURF", ("OB.O...........W....RB.....BB.....G.BB..........W..WR.", false, false, false), ["F"]
    "BringDLFtoURF", ("OB.O.......................BB.....G.BBBR.....W..W..WR.", false, false, false), ["F' R"]
    "BringDLFtoURF", ("OB.O.............B.....WR..BB.....G.BB..........W..WR.", false, false, false), ["U' R'"; "R' F'"; "F' U'"]
    "BringDLFtoURF", ("OBBO.......................BB.....G.BB......W...W..WRR", false, false, false), ["R2"]
    "BringDLFtoURF", ("OB.O.......................BB.....G.BB...WB....RW..WR.", false, false, false), ["R2 U"; "F2 U'"]
    "BringDLFtoURF", ("OB.O.............W.....RB..BB.....G.BB..........W..WR.", false, false, false), ["U F"; "R U"; "F R"]
    "BringDLFtoURF", ("OB.O.......................BB.....G.BB...BR....WW..WR.", false, false, false), ["F'"]
    "BringDLFtoURF", ("OB.O.......................BB.....G.BBRW.....B..W..WR.", false, false, false), ["F U'"]
    "BringDLFtoURF", ("OB.O..B..R........W........BB.....G.BB..........W..WR.", false, false, false), ["U2"]
    "BringDLFtoURF", ("OB.O...........R....BW.....BB.....G.BB..........W..WR.", false, false, false), ["U'"]
    "BringDLFtoURF", ("OBWO.......................BB.....G.BB......R...W..WRB", false, false, false), ["R F'"]
    "BringDLFtoURF", ("OB.O...........B....WR.....BB.....G.BB..........W..WR.", false, false, false), ["U2 R'"; "F2 R"]
    "BringDLFtoURF", ("OB.O..W..B........R........BB.....G.BB..........W..WR.", false, false, false), ["U R'"]
    "BringDLFtoURF", ("OB.O....R..B..............WBB.....G.BB..........W..WR.", false, false, false), ["R'"]
    "BringDLFtoURF", ("OB.O....W..R..............BBB.....G.BB..........W..WR.", false, false, false), ["U"]
    "BringDLFtoURF", ("OB.O....B..W..............RBB.....G.BB..........W..WR.", false, false, false), ["U2 F"; "R2 F'"]
    "BringDLFtoURF", ("OB.O.......................BB.....G.BB...RW....BW..WR.", false, false, false), ["R"]
    "BringDLFtoURF", ("OB.O.......................BB.....G.BBWB.....R..W..WR.", false, false, false), ["F2"]
    // Pair and insert LF pair (complete FB) [1 case]
    "InsertLFPair", ("OB.O.............R.....BW..BB.....G.BB..........W..WR.", false, false, false), ["R' M' F"; "R2 r F"; "r R2 F"; "r' M2 F"; "M' R' F"; "M2 r' F"]]

let fbPatterns = dlEdgePatterns @ lCenterPatterns @ lbPairPatterns @ lfPairPatterns

let drEdgePatterns = [
    // Solve DR edge [18 cases] (restricted to U, R/r, F, B, M)
    "DREdge", ("O..O.......................BBBR...G.BBBR...G.W..W.WW..", false, false, false), [] // skip
    "DREdge", ("O..O...W..G................BBBR...G.BBBR.....W..W..W..", false, false, false), ["r' U' R2"; "r' U' r2"; "B' R B"; "M U' R2"; "M U' r2"]
    "DREdge", ("O..O..........W..........G.BBBR...G.BBBR.....W..W..W..", false, false, false), ["R2"; "r2"]
    "DREdge", ("O..O............W.....G....BBBR...G.BBBR.....W..W..W..", false, false, false), ["U' R2"; "U' r2"]
    "DREdge", ("O..O........G......W.......BBBR...G.BBBR.....W..W..W..", false, false, false), ["U r' U' R2"; "U r' U' r2"; "U B' R B"; "U M U' R2"; "U M U' r2"; "U' r U R2"; "U' r U r2"; "U' F R' F'"; "U' M' U R2"; "U' M' U r2"; "F' U' F R'"; "F' U' F r'"; "B U B' R"; "B U B' r"]
    "DREdge", ("O..O.......................BBBR...G.BBBRW....WG.W..W..", false, false, false), ["r U' R2"; "r U' r2"; "M' U' R2"; "M' U' r2"]
    "DREdge", ("O..O.W.....................BBBR...GGBBBR.....W..W..W..", false, false, false), ["R"; "r"]
    "DREdge", ("O..O.......................BBBR...G.BBBRG....WW.W..W..", false, false, false), ["r2 U R2"; "r2 U r2"; "M2 U R2"; "M2 U r2"]
    "DREdge", ("O..O.G.....................BBBR...GWBBBR.....W..W..W..", false, false, false), ["B U B' R2"; "B U B' r2"]
    "DREdge", ("OG.O.......................BBBR...G.BBBR.....W..W..WW.", false, false, false), ["r2 U' R2"; "r2 U' r2"; "M2 U' R2"; "M2 U' r2"]
    "DREdge", ("O..O............G.....W....BBBR...G.BBBR.....W..W..W..", false, false, false), ["r U R2"; "r U r2"; "F R' F'"; "M' U R2"; "M' U r2"]
    "DREdge", ("O..O.......................BBBR.GWG.BBBR.....W..W..W..", false, false, false), ["F' U' F R2"; "F' U' F r2"]
    "DREdge", ("O..O..........G..........W.BBBR...G.BBBR.....W..W..W..", false, false, false), ["U r U R2"; "U r U r2"; "U F R' F'"; "U M' U R2"; "U M' U r2"; "U' r' U' R2"; "U' r' U' r2"; "U' B' R B"; "U' M U' R2"; "U' M U' r2"; "F' U F R'"; "F' U F r'"; "B U' B' R"; "B U' B' r"]
    "DREdge", ("O..O........W......G.......BBBR...G.BBBR.....W..W..W..", false, false, false), ["U2 R2"; "U2 r2"]
    "DREdge", ("O..O.......................BBBR...G.BBBR...W.W..W.GW..", false, false, false), ["R F' U' F R2"; "R F' U' F r2"; "R' B U B' R2"; "R' B U B' r2"; "R2 U r U R2"; "R2 U r U r2"; "R2 U F R' F'"; "R2 U M' U R2"; "R2 U M' U r2"; "R2 U' r' U' R2"; "R2 U' r' U' r2"; "R2 U' B' R B"; "R2 U' M U' R2"; "R2 U' M U' r2"; "R2 F' U F R'"; "R2 F' U F r'"; "R2 B U' B' R"; "R2 B U' B' r"; "r F' U' F R2"; "r F' U' F r2"; "r' B U B' R2"; "r' B U B' r2"; "r2 U r U R2"; "r2 U r U r2"; "r2 U F R' F'"; "r2 U M' U R2"; "r2 U M' U r2"; "r2 U' r' U' R2"; "r2 U' r' U' r2"; "r2 U' B' R B"; "r2 U' M U' R2"; "r2 U' M U' r2"; "r2 F' U F R'"; "r2 F' U F r'"; "r2 B U' B' R"; "r2 B U' B' r"; "F R F' U' R2"; "F R F' U' r2"; "B' R' B U R2"; "B' R' B U r2"]
    "DREdge", ("OW.O.......................BBBR...G.BBBR.....W..W..WG.", false, false, false), ["r' U R2"; "r' U r2"; "M U R2"; "M U r2"]
    "DREdge", ("O..O.......................BBBR.WGG.BBBR.....W..W..W..", false, false, false), ["R'"; "r'"]
    "DREdge", ("O..O...G..W................BBBR...G.BBBR.....W..W..W..", false, false, false), ["U R2"; "U r2"]]

let rbPairPatterns = [
    // Tuck RB to FD [16 cases] (restricted to U, R/r, F, B, M)
    "TuckRBtoFD", ("O..O.......................BBBR...G.BBBRG..G.WO.W.WW..", false, false, false), [] // skip
    "TuckRBtoFD", ("O..O.......................BBBR...G.BBBRO..G.WG.W.WW..", false, false, false), ["M' U2 M2"; "M2 U2 M"]
    "TuckRBtoFD", ("O..O...O..G................BBBR...G.BBBR...G.W..W.WW..", false, false, false), ["U2 M"]
    "TuckRBtoFD", ("O..O.G.....................BBBR...GOBBBR...G.W..W.WW..", false, false, false), ["R' U R M"; "R' U R2 r'"; "R' U r M2"; "R' U r' R2"; "R' U M R"; "R' U M2 r"; "r' U R M"; "r' U R2 r'"; "r' U r M2"; "r' U r' R2"; "r' U M R"; "r' U M2 r"]
    "TuckRBtoFD", ("OO.O.......................BBBR...G.BBBR...G.W..W.WWG.", false, false, false), ["M'"]
    "TuckRBtoFD", ("O..O...G..O................BBBR...G.BBBR...G.W..W.WW..", false, false, false), ["M2"]
    "TuckRBtoFD", ("O..O............G.....O....BBBR...G.BBBR...G.W..W.WW..", false, false, false), ["M"]
    "TuckRBtoFD", ("O..O.......................BBBR.OGG.BBBR...G.W..W.WW..", false, false, false), ["R U' R r2"; "R U' R' M2"; "R U' r' M"; "R U' r2 R"; "R U' M r'"; "R U' M2 R'"; "r U' R r2"; "r U' R' M2"; "r U' r' M"; "r U' r2 R"; "r U' M r'"; "r U' M2 R'"]
    "TuckRBtoFD", ("O..O........O......G.......BBBR...G.BBBR...G.W..W.WW..", false, false, false), ["U M2"]
    "TuckRBtoFD", ("O..O........G......O.......BBBR...G.BBBR...G.W..W.WW..", false, false, false), ["U' M"]
    "TuckRBtoFD", ("O..O.O.....................BBBR...GGBBBR...G.W..W.WW..", false, false, false), ["R' U' R M2"; "R' U' R' r2"; "R' U' r M'"; "R' U' r2 R'"; "R' U' M' r"; "R' U' M2 R"; "r' U' R M2"; "r' U' R' r2"; "r' U' r M'"; "r' U' r2 R'"; "r' U' M' r"; "r' U' M2 R"; "B U2 B' M"]
    "TuckRBtoFD", ("O..O.......................BBBR.GOG.BBBR...G.W..W.WW..", false, false, false), ["R U r'"; "r U r'"]
    "TuckRBtoFD", ("O..O..........O..........G.BBBR...G.BBBR...G.W..W.WW..", false, false, false), ["U' M2"]
    "TuckRBtoFD", ("OG.O.......................BBBR...G.BBBR...G.W..W.WWO.", false, false, false), ["M U2 M"; "M2 U2 M2"]
    "TuckRBtoFD", ("O..O..........G..........O.BBBR...G.BBBR...G.W..W.WW..", false, false, false), ["U M"]
    "TuckRBtoFD", ("O..O............O.....G....BBBR...G.BBBR...G.W..W.WW..", false, false, false), ["U2 M2"]
    // Bring DRB to ULB [18 cases] (restricted to U, R/r, F, B, M)
    "BringDRBtoULB", ("O..O..G..O........W........BBBR...G.BBBRG..G.WO.W.WW..", false, false, false), [] // skip
    "BringDRBtoULB", ("O..O.......................BBBR...G.BBBRGWGG.WOOW.WW..", false, false, false), ["R2 B' R' B R'"; "F R2 F' R2 U'"]
    "BringDRBtoULB", ("O.WO.......................BBBR...G.BBBRG..GOWO.W.WW.G", false, false, false), ["R' U R U'"; "r' U r U'"; "B U B' U2"; "B U2 B' U"]
    "BringDRBtoULB", ("O..O.............G.....WO..BBBR...G.BBBRG..G.WO.W.WW..", false, false, false), ["F' U F"]
    "BringDRBtoULB", ("O..O.......................BBBR...G.BBBRGOWG.WOGW.WW..", false, false, false), ["R U2 R'"]
    "BringDRBtoULB", ("O..O....O..G..............WBBBR...G.BBBRG..G.WO.W.WW..", false, false, false), ["R' U2 R"; "r' U2 r"]
    "BringDRBtoULB", ("O..O...........O....GW.....BBBR...G.BBBRG..G.WO.W.WW..", false, false, false), ["U"]
    "BringDRBtoULB", ("O..O...........G....WO.....BBBR...G.BBBRG..G.WO.W.WW..", false, false, false), ["U' F' U F"; "U2 R' U2 R"; "U2 r' U2 r"; "R U2 R' U2"; "B U B' U'"]
    "BringDRBtoULB", ("O..O....G..W..............OBBBR...G.BBBRG..G.WO.W.WW..", false, false, false), ["U R U' R'"; "R2 B' R2 B"; "F' U2 F U2"; "B U' B' U"; "B U2 B' U2"]
    "BringDRBtoULB", ("O.OO.......................BBBR...G.BBBRG..GGWO.W.WW.W", false, false, false), ["R' U' R"; "r' U' r"]
    "BringDRBtoULB", ("O..O..W..G........O........BBBR...G.BBBRG..G.WO.W.WW..", false, false, false), ["U R' U2 R"; "U r' U2 r"; "U2 F' U F"; "R U R' U2"]
    "BringDRBtoULB", ("O..O...........W....OG.....BBBR...G.BBBRG..G.WO.W.WW..", false, false, false), ["U' R U' R'"; "R' U' R U'"; "r' U' r U'"]
    "BringDRBtoULB", ("O..O..O..W........G........BBBR...G.BBBRG..G.WO.W.WW..", false, false, false), ["U2 R U' R'"; "R' U2 R U'"; "r' U2 r U'"; "F' U' F U2"]
    "BringDRBtoULB", ("O.GO.......................BBBR...G.BBBRG..GWWO.W.WW.O", false, false, false), ["B' R2 B R2 U2"]
    "BringDRBtoULB", ("O..O.............W.....OG..BBBR...G.BBBRG..G.WO.W.WW..", false, false, false), ["R U' R'"]
    "BringDRBtoULB", ("O..O....W..O..............GBBBR...G.BBBRG..G.WO.W.WW..", false, false, false), ["U'"]
    "BringDRBtoULB", ("O..O.............O.....GW..BBBR...G.BBBRG..G.WO.W.WW..", false, false, false), ["U2"]
    "BringDRBtoULB", ("O..O.......................BBBR...G.BBBRGGOG.WOWW.WW..", false, false, false), ["F' U2 F"]
    // Pair and insert RB pair [1 case] (restricted to U, R/r, F, B, M)
    "InsertRBPair", ("O..O..G..O........W........BBBR...G.BBBRG..G.WO.W.WW..", false, false, false), ["R r2 U R"; "R r2 U r"; "R' M2 U R"; "R' M2 U r"; "r' M U R"; "r' M U r"; "r2 R U R"; "r2 R U r"; "M r' U R"; "M r' U r"; "M2 R' U R"; "M2 R' U r"]]

let rfPairPatterns = [
    // Tuck RF to BD [14 cases] (restricted to U, R/r, F, B, M)
    "TuckRFtoBD", ("OGOO.O.....................BBBR...GGBBBR...GGW..W.WWRW", false, false, false), [] // skip
    "TuckRFtoBD", ("O.OO.O.G..R................BBBR...GGBBBR...GGW..W.WW.W", false, false, false), ["U2 M2"]
    "TuckRFtoBD", ("O.OO.O.....................BBBR.RGGGBBBR...GGW..W.WW.W", false, false, false), ["R U R r2"; "R U R' M2"; "R U r' M"; "R U r2 R"; "R U M r'"; "R U M2 R'"; "r U R r2"; "r U R' M2"; "r U r' M"; "r U r2 R"; "r U M r'"; "r U M2 R'"; "F' U2 F M'"]
    "TuckRFtoBD", ("O.OO.O.R..G................BBBR...GGBBBR...GGW..W.WW.W", false, false, false), ["M'"]
    "TuckRFtoBD", ("O.OO.O..........R.....G....BBBR...GGBBBR...GGW..W.WW.W", false, false, false), ["M2"]
    "TuckRFtoBD", ("OROO.O.....................BBBR...GGBBBR...GGW..W.WWGW", false, false, false), ["M U2 M2"; "M2 U2 M'"]
    "TuckRFtoBD", ("O.OO.O........R..........G.BBBR...GGBBBR...GGW..W.WW.W", false, false, false), ["U M2"]
    "TuckRFtoBD", ("O.OO.O......G......R.......BBBR...GGBBBR...GGW..W.WW.W", false, false, false), ["U M'"]
    "TuckRFtoBD", ("O.OO.O......R......G.......BBBR...GGBBBR...GGW..W.WW.W", false, false, false), ["U' M2"]
    "TuckRFtoBD", ("O.OO.O........G..........R.BBBR...GGBBBR...GGW..W.WW.W", false, false, false), ["U' M'"]
    "TuckRFtoBD", ("O.OO.O.....................BBBR.GRGGBBBR...GGW..W.WW.W", false, false, false), ["R U' R' M'"; "R U' R2 r"; "R U' r R2"; "R U' r' M2"; "R U' M' R'"; "R U' M2 r'"; "r U' R' M'"; "r U' R2 r"; "r U' r R2"; "r U' r' M2"; "r U' M' R'"; "r U' M2 r'"]
    "TuckRFtoBD", ("O.OO.O..........G.....R....BBBR...GGBBBR...GGW..W.WW.W", false, false, false), ["U2 M'"]
    "TuckRFtoBD", ("O.OO.O.....................BBBR...GGBBBRR..GGWG.W.WW.W", false, false, false), ["M"]
    "TuckRFtoBD", ("O.OO.O.....................BBBR...GGBBBRG..GGWR.W.WW.W", false, false, false), ["M' U2 M'"; "M2 U2 M2"]
    // Bring DRF to ULF [15 cases] (restricted to U, R/r, F, B, M)
    "BringDRFtoULF", ("OGOO.O.........R....WG.....BBBR...GGBBBR...GGW..W.WWRW", false, false, false), [] // skip
    "BringDRFtoULF", ("OGOO.OR..G........W........BBBR...GGBBBR...GGW..W.WWRW", false, false, false), ["U2 R U2 R'"; "U2 r U2 r'"; "F' U' F U"]
    "BringDRFtoULF", ("OGOO.O..R..W..............GBBBR...GGBBBR...GGW..W.WWRW", false, false, false), ["R' F R F'"]
    "BringDRFtoULF", ("OGOO.O.....................BBBR...GGBBBR.WRGGW.GW.WWRW", false, false, false), ["R U' R' U"; "r U' r' U"; "F' U' F U2"; "F' U2 F U'"]
    "BringDRFtoULF", ("OGOO.O...........R.....WG..BBBR...GGBBBR...GGW..W.WWRW", false, false, false), ["U"]
    "BringDRFtoULF", ("OGOO.O..G..R..............WBBBR...GGBBBR...GGW..W.WWRW", false, false, false), ["U2"]
    "BringDRFtoULF", ("OGOO.O..W..G..............RBBBR...GGBBBR...GGW..W.WWRW", false, false, false), ["U R U2 R'"; "U r U2 r'"; "F R' F' R"; "F' U2 F U"]
    "BringDRFtoULF", ("OGOO.O.........G....RW.....BBBR...GGBBBR...GGW..W.WWRW", false, false, false), ["U' R U2 R'"; "U' r U2 r'"]
    "BringDRFtoULF", ("OGOO.O.....................BBBR...GGBBBR.RGGGW.WW.WWRW", false, false, false), ["R U R'"; "r U r'"]
    "BringDRFtoULF", ("OGOO.OW..R........G........BBBR...GGBBBR...GGW..W.WWRW", false, false, false), ["U'"]
    "BringDRFtoULF", ("OGOO.O.........W....GR.....BBBR...GGBBBR...GGW..W.WWRW", false, false, false), ["R U2 R' U"; "r U2 r' U"]
    "BringDRFtoULF", ("OGOO.O...........W.....GR..BBBR...GGBBBR...GGW..W.WWRW", false, false, false), ["F' U F U'"; "F' U2 F U2"]
    "BringDRFtoULF", ("OGOO.OG..W........R........BBBR...GGBBBR...GGW..W.WWRW", false, false, false), ["R U R' U"; "r U r' U"]
    "BringDRFtoULF", ("OGOO.O.....................BBBR...GGBBBR.GWGGW.RW.WWRW", false, false, false), ["R U' B U' B' R'"; "R B U2 B' R' U"; "R2 B' R' B U' R'"; "R2 B' R' B R' U'"; "r U' B U' B' r'"; "r B U2 B' r' U"; "r2 B r2 B' U2 B"; "F U2 F U2 F' U2"; "F R' F' R2 U2 R'"; "F' U F R U2 R'"; "F' U F r U2 r'"; "B r2 B r2 B' U2"]
    "BringDRFtoULF", ("OGOO.O...........G.....RW..BBBR...GGBBBR...GGW..W.WWRW", false, false, false), ["R U2 R'"; "r U2 r'"]
    // Pair and insert RF pair (complete SB) [1 case] (restricted to U, R/r, F, B, M)
    "InsertRFPair", ("OGOO.O.........R....WG.....BBBR...GGBBBR...GGW..W.WWRW", false, false, false), ["R M2 U' R'"; "R M2 U' r'"; "R' r2 U' R'"; "R' r2 U' r'"; "r M' U' R'"; "r M' U' r'"; "r2 R' U' R'"; "r2 R' U' r'"; "M' r U' R'"; "M' r U' r'"; "M2 R U' R'"; "M2 R U' r'"]]

let sbPatterns = drEdgePatterns @ rbPairPatterns @ rfPairPatterns

let coBeginnerPatterns =
    // Orient corners (CO) - hand authored patterns [8 cases] - single-alg (sune)
    ["CornerOrientation", ("O.OO.O...Y.Y...Y.Y.........BBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, false), [] // skip
     "CornerOrientation", ("O.OO.OY.Y............Y.Y...BBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true), [sune + " U' " + sune + " " + sune + " U' " + sune + " U2 " + sune] // H - 38 twists (5 sunes) - easy to learn, but not bestcoH]
     "CornerOrientation", ("O.OO.OY..............Y..Y.YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true), [sune + " U2 " + sune + " U' " + sune + " U2 " + sune] // Pi - 31 twists (4 sunes)
     "CornerOrientation", ("O.OO.O...Y.Y.........Y.Y...BBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true), [sune + " U' " + sune + " U2 " + sune] // U - 23 twists (3 sunes)
     "CornerOrientation", ("O.OO.OY....Y.....Y...Y.....BBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true), [sune + " U " + sune + " U2 " + sune] // T - 23 twists (3 sunes)
     "CornerOrientation", ("O.OO.OY........Y.......Y..YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true), [sune] // sune - 7 twists (1 sune)
     "CornerOrientation", ("O.OO.O..Y......Y..Y.....Y..BBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true), [sune + " U2 " + sune] // anti-sune - 15 twists (2 sunes)
     "CornerOrientation", ("O.OO.O...Y.......Y...Y....YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true), [sune + " " + sune + " U2 " + sune]] // L - 22 twists (3 sunes)

let coIntermediatePatterns =
    let sexy = "R U R' U'"
    let sledge = "R' F R F'"
    // Orient corners (CO) - hand authored patterns [8 cases] - 7 algs (~13 STM better than beginner)
    ["CornerOrientation", ("O.OO.O...Y.Y...Y.Y.........BBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, false), [] // skip
     "CornerOrientation", ("O.OO.OY.Y............Y.Y...BBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true), ["R U2 R' U' " + sexy + " R U' R'"] // H
     "CornerOrientation", ("O.OO.O..Y.........Y.Y..Y...BBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true), ["F " + sexy + " " + sexy + " F'"] // Pi
     "CornerOrientation", ("O.OO.O.....Y.....YY.Y......BBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true), ["F " + sexy + " F'"] // U
     "CornerOrientation", ("O.OO.OY....Y.....Y...Y.....BBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true), [sexy + " " + sledge] // T
     "CornerOrientation", ("O.OO.OY........Y.......Y..YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true), [sune] // sune
     "CornerOrientation", ("O.OO.O..Y........YY..Y.....BBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true), ["L' U' L U' L' U2 L"] // anti-sune
     "CornerOrientation", ("O.OO.O...Y.......Y...Y....YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true), ["F R' F' R U R U' R'"]] // L

let cpBeginnerPatterns = [
    // Permute corners (CP) - hand authored patterns [3 cases] - single-alg (jperm)
    "CornerPermutation", ("O.OO.OO.OY.Y...Y.YB.BR.RG.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, false), [] // skip (color neutral)
    "CornerPermutation", ("O.OO.OG.RY.Y...Y.YO.OB.RG.BBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), [jperm] // adjacent (color neutral)
    "CornerPermutation", ("O.OO.OR.OY.Y...Y.YG.BR.OB.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, false), [jperm + " U " + jperm]] // diag (color neutral)

let cpIntermediatePatterns =
    let diagSwap = "r2 D r' U r D' R2 U' F' U' F" // fancy!
    // Permute corners (CP) - hand authored patterns [3 cases] - (two algs: jperm [same as beginner] and diagSwap) (just diag swap, ~3 STM better than beginner)
    ["CornerPermutation", ("O.OO.OO.OY.Y...Y.YB.BR.RG.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, false), [] // skip (color neutral)
     "CornerPermutation", ("O.OO.OG.RY.Y...Y.YO.OB.RG.BBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), [jperm] // adjacent (color neutral)
     "CornerPermutation", ("O.OO.OR.OY.Y...Y.YG.BR.OB.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, false), [diagSwap]] // diag (color neutral)

let cmllBeginnerPatterns = coBeginnerPatterns @ cpBeginnerPatterns
let cmllIntermediatePatterns = coIntermediatePatterns @ cpIntermediatePatterns // two-look

let centerOrientationPatterns = [
    // Orient center - hand authored patterns [2 cases]
    "CenterOrientation", ("O.OO.OO.OY.Y.E.Y.YB.BR.RG.GBBBR.RGGGBBBR.RGGGW.WWEWW.W", true, true, false), [] // skip
    "CenterOrientation", ("O.OO.OO.OY.Y.P.Y.YB.BR.RG.GBBBR.RGGGBBBR.RGGGW.WWPWW.W", true, true, false), ["M"]] // or M'

let edgeBeginnerOrientationPatters =
    // Orient edges (EO) - hand authored patterns [12 cases]
    ["EdgeOrientation", ("O.OO.OO.OYEYEEEYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWEW", true, true, false), [] // skip
     "EdgeOrientation", ("O.OO.OO.OYPYEEEYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWEW", true, true, true),  [mum + " " + mum + " U' " + mum] // 2-up vertical
     "EdgeOrientation", ("O.OO.OO.OYEYEEEYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWPW", true, true, false), ["M2 " +  mum + " " + mum + " U' " + mum] // 2-down
     "EdgeOrientation", ("O.OO.OO.OYPYEEPYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWEW", true, true, true),  [mum + " U2 " + mum] // 2-up L (NE)
     "EdgeOrientation", ("O.OO.OO.OYPYEEEYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWPW", true, true, true),  [mum + " " + mum + " U2 " + mum + " U " + mum + " U' " + mum] // 2 B/B
     "EdgeOrientation", ("O.OO.OO.OYPYEEEYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWEW", true, true, true),  [mum + " U " + mum + " U' " + mum] // 2 B/F
     "EdgeOrientation", ("O.OO.OO.OYPYPEPYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWEW", true, true, false), [mum + " U' " + mum + " U " + mum + " U' " + mum] // 4 up
     "EdgeOrientation", ("O.OO.OO.OYEYPEPYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWEW", true, true, true),  [mum] // arrow F/F
     "EdgeOrientation", ("O.OO.OO.OYEYPEPYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWPW", true, true, true),  [mum + " U' " + mum + " U2 " + mum] // arrow F/B
     "EdgeOrientation", ("O.OO.OO.OYEYPEPYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWPW", true, true, true),  [mum + " U2 " + mum + " U2 " + mum] // 2-up/2-down horizontal (special case - leave horizontal)
     "EdgeOrientation", ("O.OO.OO.OYPYEEPYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWPW", true, true, true),  [mum + " U' " + mum] // 2-up L (NE)/2-down
     "EdgeOrientation", ("O.OO.OO.OYPYPEPYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWPW", true, true, false), [mum + " U2 " + mum + " U " + mum + " U' " + mum]] // 6-flip

let edgeIntermediateOrientationPatters =
    // Orient edges (EO) - hand authored patterns (~4 STM better than beginner) [19 cases]
    ["EdgeOrientation", ("O.OO.OO.OYEYEEEYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWEW", true, true, false), [] // skip
     "EdgeOrientation", ("O.OO.OO.OYPYEEEYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWEW", true, true, true),  ["M' U M U' " + mum] // 2-up vertical
     "EdgeOrientation", ("O.OO.OO.OYEYEEEYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWPW", true, true, false), [mum + " U M U' M'"] // 2-down
     "EdgeOrientation", ("O.OO.OO.OYEYEEPYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWEW", true, true, false), ["M U M' U2 M U' M'"] // 2-up L (SE)
     "EdgeOrientation", ("O.OO.OO.OYEYPEEYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWEW", true, true, false), ["M U' M' U2 M U' M'"] // 2-up L (SW)
     "EdgeOrientation", ("O.OO.OO.OYPYPEEYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWEW", true, true, false), ["M' U M' U2 " + mum] // 2-up L (NW)
     "EdgeOrientation", ("O.OO.OO.OYPYEEPYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWEW", true, true, false), ["M' U' M U2 " + mum] // 2-up L (NE)
     "EdgeOrientation", ("O.OO.OO.OYEYEEEYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWPW", true, true, true),  [mum + " U' M U' M'"] // 2 F/B
     "EdgeOrientation", ("O.OO.OO.OYPYEEEYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWEW", true, true, true),  ["M' U' M U' " + mum] // 2 B/F
     "EdgeOrientation", ("O.OO.OO.OYPYPEPYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWEW", true, true, false), ["M' U2 M' U2 " + mum] // 4 up
     "EdgeOrientation", ("O.OO.OO.OYPYPEPYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWEW", true, true, false), [mum + " U' " + mum + " U " + mum + " U' " + mum] // 4 up
     "EdgeOrientation", ("O.OO.OO.OYEYPEPYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWEW", true, true, true),  [mum] // arrow F/F
     "EdgeOrientation", ("O.OO.OO.OYPYPEPYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWPW", true, true, true),  ["M U' M'"] // arrow B/B (easy)
     "EdgeOrientation", ("O.OO.OO.OYEYPEPYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWPW", true, true, true),  ["M' U2 M' U2 M U' M'"] // 2-up/2-down horizontal (special case - leave horizontal)
     "EdgeOrientation", ("O.OO.OO.OYEYEEPYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWPW", true, true, false), ["M2 U' M U' M'"] // 2-up L (SE)/2-down
     "EdgeOrientation", ("O.OO.OO.OYEYPEEYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWPW", true, true, false), ["M2 U M U' M'"] // 2-up L (SW)/2-down
     "EdgeOrientation", ("O.OO.OO.OYPYPEEYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWPW", true, true, false), ["M2 U' " + mum] // 2-up L (NW)/2-down
     "EdgeOrientation", ("O.OO.OO.OYPYEEPYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWPW", true, true, false), ["M2 U " + mum] // 2-up L (NE)/2-down
     "EdgeOrientation", ("O.OO.OO.OYPYPEPYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWPW", true, true, false), ["R U' r' U' M' U r U r'"]] // 6-flip (fancy!)

let eoBeginnerPatterns = centerOrientationPatterns @ edgeBeginnerOrientationPatters
let eoIntermediatePatterns = centerOrientationPatterns @ edgeIntermediateOrientationPatters

let lrPatterns = [
    // L edge to DF
    "LToDF", ("OPOO.OOPOYEYEEEYEYBPBRPRGPGBBBR.RGGGBBBRBRGGGWEWWEWWEW", true, true, false), [] // skip
    "LToDF", ("OPOO.OOBOYEYEEEYEYBPBRPRGPGBBBR.RGGGBBBRPRGGGWEWWEWWEW", true, true, true),  ["M2"]
    "LToDF", ("OBOO.OOPOYEYEEEYEYBPBRPRGPGBBBR.RGGGBBBRPRGGGWEWWEWWEW", true, true, false), ["M U2 M"; "M2 U2 M2"]
    // LR edges to bottom
    "LREdgesBottom", ("OGOO.OOPOYEYEEEYEYBPBR.RGPGBBBRPRGGGBBBRBRGGGWEWWEWWEW", true, true, false), [] // skip
    "LREdgesBottom", ("OPOO.OOGOYEYEEEYEYBPBR.RGPGBBBRPRGGGBBBRBRGGGWEWWEWWEW", true, true, true), ["M U2 M'"]
    // LR solved (AUF and up corner colors matter again)
    "LREdges", ("OGOO.OBPBYEYEEEYEYRPRG.GOPOBBBRPRGGGBBBRBRGGGWEWWEWWEW", false, false, true), ["M2 U'"]]

let eolrBeginnerPatterns = eoBeginnerPatterns @ lrPatterns
let eolrIntermediatePatterns = eoIntermediatePatterns @ lrPatterns

let l4eBeginnerPatterns =
    let mu2 = "M' U2"
    // Last 4 edges -> Solved! - hand authored
    ["L4E", ("OOOOOOOOOYYYYYYYYYBBBRRRGGGBBBRRRGGGBBBRRRGGGWWWWWWWWW", false, false, false), [] // skip (solved!)
     "L4E", ("OROOROOROYWYYWYYWYBBBRORGGGBBBRORGGGBBBRORGGGWYWWYWWYW", false, false, false), ["M2"]
     "L4E", ("OOOOROOROYWYYWYYYYBBBRRRGGGBBBRORGGGBBBRORGGGWWWWYWWYW", false, false, false), [mu2 + " " + mu2]
     "L4E", ("OROOROOOOYWYYWYYYYBBBRRRGGGBBBRORGGGBBBRORGGGWYWWYWWWW", false, false, false), [mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2]
     "L4E", ("OROOOOOOOYYYYYYYWYBBBRORGGGBBBRRRGGGBBBRRRGGGWYWWWWWWW", false, false, false), [mu2 + " " + mu2 + " M2"]
     "L4E", ("OOOOOOOROYYYYYYYWYBBBRORGGGBBBRRRGGGBBBRRRGGGWWWWWWWYW", false, false, false), [mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " M2"]
     "L4E", ("OROOOOOOOYWYYYYYYYBBBRORGGGBBBRRRGGGBBBRRRGGGWWWWWWWYW", false, false, false), [mu2 + " " + mu2 + (* vertical bars *) " M' U2 M2 U2 M"]
     "L4E", ("OOOOOOOROYWYYYYYYYBBBRORGGGBBBRRRGGGBBBRRRGGGWYWWWWWWW", false, false, false), [mu2 + " " + mu2 + " " + mu2 + " " + mu2 + (* horizontal bars *) " U2 M2 U2 M2"] // (could spot, bar earlier, but...)
     "L4E", ("OOOOROOROYYYYWYYWYBBBRRRGGGBBBRORGGGBBBRORGGGWYWWYWWWW", false, false, false), [mu2 + " " + mu2 + (* vertical bars *) " M' U2 M2 U2 M'"]
     "L4E", ("OROOROOOOYYYYWYYWYBBBRRRGGGBBBRORGGGBBBRORGGGWWWWYWWYW", false, false, false), [mu2 + " " + mu2 + " " + mu2 + " " + mu2 + (* horizontal bars *) " U2 M2 U2"]
     "L4E", ("OOOOOOOROYYYYYYYWYBBBRRRGGGBBBRRRGGGBBBRORGGGWYWWWWWWW", false, false, false), [mu2 + " " + mu2 + " U2 M2 U2"]
     "L4E", ("OROOOOOOOYYYYYYYWYBBBRRRGGGBBBRRRGGGBBBRORGGGWWWWWWWYW", false, false, false), [mu2 + " " + mu2 + " " + mu2 + " " + mu2 + (* vertical bars *) " M' U2 M2 U2 M'"]
     "L4E", ("OROOROOOOYWYYWYYYYBBBRORGGGBBBRORGGGBBBRRRGGGWWWWYWWYW", false, false, false), [mu2 + " " + mu2 + (* horizontal bars *) " U2 M2 U2 M2"]
     "L4E", ("OOOOROOROYWYYWYYYYBBBRORGGGBBBRORGGGBBBRRRGGGWYWWYWWWW", false, false, false), [mu2 + " " + mu2 + " " + mu2 + " " + mu2 + (* vertical bars *) " M' U2 M2 U2 M"]
     "L4E", ("OROOROOOOYYYYWYYWYBBBRORGGGBBBRORGGGBBBRRRGGGWYWWYWWWW", false, false, false), [mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " M2"]
     "L4E", ("OOOOROOROYYYYWYYWYBBBRORGGGBBBRORGGGBBBRRRGGGWWWWYWWYW", false, false, false), [mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " M2"]
     "L4E", ("OOOOOOOROYWYYYYYYYBBBRRRGGGBBBRRRGGGBBBRORGGGWWWWWWWYW", false, false, false), [mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2]
     "L4E", ("OROOOOOOOYWYYYYYYYBBBRRRGGGBBBRRRGGGBBBRORGGGWYWWWWWWW", false, false, false), [mu2 + " " + mu2 + " " + mu2 + " " + mu2]
     "L4E", ("OOOOROOOOYWYYWYYWYBBBRRRGGGBBBRORGGGBBBRRRGGGWYWWYWWYW", false, false, false), ["U2 M2 U2"] // horizontal bars
     "L4E", ("OROOOOOROYYYYYYYYYBBBRORGGGBBBRRRGGGBBBRORGGGWWWWWWWWW", false, false, false), ["U2 M2 U2 M2"] // horizontal bars
     "L4E", ("OROOROOROYYYYWYYYYBBBRORGGGBBBRORGGGBBBRORGGGWWWWYWWWW", false, false, false), ["M U2 M2 U2 M'"] // vertical bars
     "L4E", ("OOOOOOOOOYWYYYYYWYBBBRRRGGGBBBRRRGGGBBBRRRGGGWYWWWWWYW", false, false, false), ["M' U2 M2 U2 M'"] // vertical bars
     "L4E", ("OROOOOOROYWYYYYYWYBBBRORGGGBBBRRRGGGBBBRORGGGWYWWWWWYW", false, false, false), [mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " M2"] // dots
     "L4E", ("OOOOROOOOYYYYWYYYYBBBRRRGGGBBBRORGGGBBBRRRGGGWWWWYWWWW", false, false, false), [mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2]] // dots

let l4eIntermediatePatterns = [
    // Last 4 edges -> Solved! - hand authored (~5 STM better than beginner)
    "L4E", ("OOOOOOOOOYYYYYYYYYBBBRRRGGGBBBRRRGGGBBBRRRGGGWWWWWWWWW", false, false, false), [] // skip (solved!)
    "L4E", ("OROOROOROYWYYWYYWYBBBRORGGGBBBRORGGGBBBRORGGGWYWWYWWYW", false, false, false), ["M2"]
    "L4E", ("OOOOROOROYWYYWYYYYBBBRRRGGGBBBRORGGGBBBRORGGGWWWWYWWYW", false, false, false), ["M' U2 M' U2"]
    "L4E", ("OROOROOOOYWYYWYYYYBBBRRRGGGBBBRORGGGBBBRORGGGWYWWYWWWW", false, false, false), ["U2 M U2 M"]
    "L4E", ("OROOOOOOOYYYYYYYWYBBBRORGGGBBBRRRGGGBBBRRRGGGWYWWWWWWW", false, false, false), ["M' U2 M' U2 M2"]
    "L4E", ("OOOOOOOROYYYYYYYWYBBBRORGGGBBBRRRGGGBBBRRRGGGWWWWWWWYW", false, false, false), ["U2 M U2 M'"]
    "L4E", ("OROOOOOOOYWYYYYYYYBBBRORGGGBBBRRRGGGBBBRRRGGGWWWWWWWYW", false, false, false), ["M U2 M' U2"]
    "L4E", ("OOOOOOOROYWYYYYYYYBBBRORGGGBBBRRRGGGBBBRRRGGGWYWWWWWWW", false, false, false), ["U2 M' U2 M"]
    "L4E", ("OOOOROOROYYYYWYYWYBBBRRRGGGBBBRORGGGBBBRORGGGWYWWYWWWW", false, false, false), ["M U2 M' U2 M2"]
    "L4E", ("OROOROOOOYYYYWYYWYBBBRRRGGGBBBRORGGGBBBRORGGGWWWWYWWYW", false, false, false), ["U2 M' U2 M'"]
    "L4E", ("OOOOOOOROYYYYYYYWYBBBRRRGGGBBBRRRGGGBBBRORGGGWYWWWWWWW", false, false, false), ["M' U2 M U2"]
    "L4E", ("OROOOOOOOYYYYYYYWYBBBRRRGGGBBBRRRGGGBBBRORGGGWWWWWWWYW", false, false, false), ["M2 U2 M U2 M"]
    "L4E", ("OROOROOOOYWYYWYYYYBBBRORGGGBBBRORGGGBBBRRRGGGWWWWYWWYW", false, false, false), ["M' U2 M U2 M2"]
    "L4E", ("OOOOROOROYWYYWYYYYBBBRORGGGBBBRORGGGBBBRRRGGGWYWWYWWWW", false, false, false), ["M2 U2 M U2 M'"]
    "L4E", ("OROOROOOOYYYYWYYWYBBBRORGGGBBBRORGGGBBBRRRGGGWYWWYWWWW", false, false, false), ["M U2 M U2"]
    "L4E", ("OOOOROOROYYYYWYYWYBBBRORGGGBBBRORGGGBBBRRRGGGWWWWYWWYW", false, false, false), ["M2 U2 M' U2 M"]
    "L4E", ("OOOOOOOROYWYYYYYYYBBBRRRGGGBBBRRRGGGBBBRORGGGWWWWWWWYW", false, false, false), ["M U2 M U2 M2"]
    "L4E", ("OROOOOOOOYWYYYYYYYBBBRRRGGGBBBRRRGGGBBBRORGGGWYWWWWWWW", false, false, false), ["M2 U2 M' U2 M'"]
    "L4E", ("OOOOROOOOYWYYWYYWYBBBRRRGGGBBBRORGGGBBBRRRGGGWYWWYWWYW", false, false, false), ["U2 M2 U2"] // horizontal bars
    "L4E", ("OROOOOOROYYYYYYYYYBBBRORGGGBBBRRRGGGBBBRORGGGWWWWWWWWW", false, false, false), ["U2 M2 U2 M2"] // horizontal bars
    "L4E", ("OROOROOROYYYYWYYYYBBBRORGGGBBBRORGGGBBBRORGGGWWWWYWWWW", false, false, false), ["M U2 M2 U2 M'"] // vertical bars
    "L4E", ("OOOOOOOOOYWYYYYYWYBBBRRRGGGBBBRRRGGGBBBRRRGGGWYWWWWWYW", false, false, false), ["M' U2 M2 U2 M'"] // vertical bars
    "L4E", ("OROOOOOROYWYYYYYWYBBBRORGGGBBBRRRGGGBBBRORGGGWYWWWWWYW", false, false, false), ["E2 M E2 M"] // dots
    "L4E", ("OOOOROOOOYYYYWYYYYBBBRRRGGGBBBRORGGGBBBRRRGGGWWWWYWWWW", false, false, false), ["E2 M E2 M'"]] // dots

let lseBeginnerPatterns = eolrBeginnerPatterns @ l4eBeginnerPatterns
let lseIntermediatePatterns = eolrIntermediatePatterns @ l4eIntermediatePatterns

let rouxBeginnerPatterns = fbPatterns @ sbPatterns @ cmllBeginnerPatterns @ lseBeginnerPatterns // 102 STM, 99 with ignored AUF
let rouxIntermediatePatterns = fbPatterns @ sbPatterns @ cmllIntermediatePatterns @ lseIntermediatePatterns // 97 STM with LSE, 84 with 1L CO, 81 with 1L CP, 77 with EO, 74 with ignored AUF

let solve =
    // rouxBeginnerPatterns
    rouxIntermediatePatterns
    |> expandPatternsForAuf |> solveCase

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
    let caseCP c = caseCO c && (look Face.L Sticker.UL c = look Face.L Sticker.UR c) && (look Face.R Sticker.UL c = look Face.R Sticker.UR c) // check left/right "pairs"
    let solvedCP = solve rufMoves "Permute corners (CP)" "CornerPermutation" caseCP solvedCO

    // Orient center (note: generated patterns and algs are not distinct because goal is flexible U/D colors)
    let mMoves = [Move Move.M; Move Move.M'; Move Move.M2]
    let caseCenterO c = caseCP c && (look Face.U Sticker.C c = Color.W || look Face.U Sticker.C c = Color.Y)
    let solvedCenterO = solve mMoves "Orient center" "CenterOrientation" caseCenterO solvedCP

    // Orient edges (EO) (note: generated patterns and algs are not distinct because goal is flexible U/D colors)
    let muMoves = mMoves @ [Move Move.U; Move Move.U'; Move Move.U2]
    let caseEO c = caseCenterO c &&
                   (look Face.U Sticker.L c = Color.W || look Face.U Sticker.L c = Color.Y) &&
                   (look Face.U Sticker.U c = Color.W || look Face.U Sticker.U c = Color.Y) &&
                   (look Face.U Sticker.R c = Color.W || look Face.U Sticker.R c = Color.Y) &&
                   (look Face.U Sticker.D c = Color.W || look Face.U Sticker.D c = Color.Y) &&
                   (look Face.D Sticker.U c = Color.W || look Face.D Sticker.U c = Color.Y) &&
                   (look Face.D Sticker.D c = Color.W || look Face.D Sticker.D c = Color.Y)
    let solvedEO = solve muMoves "Orient edges (EO)" "EdgeOrientation" caseEO solvedCenterO

    // Left/right edges (LR)

    // L edge to DF
    let caseLtoDF c = caseEO c && look Face.F Sticker.D c = Color.B
    let solvedLtoDF = solve muMoves "L edge to DF" "LToDF" caseLtoDF solvedEO

    // LR edges to bottom
    let caseLRBottom c = caseLtoDF c && look Face.F Sticker.D c = Color.B && look Face.B Sticker.U c = Color.G
    let solvedLRBottom = solve muMoves "LR edges to bottom" "LREdgesBottom" caseLRBottom solvedLtoDF

    // LR edges solved
    let caseLRSolved c = caseEO c && look Face.L Sticker.U c = Color.B && look Face.R Sticker.U c = Color.G
    let solvedLR = solve muMoves "LR edges solved" "LREdges" caseLRSolved solvedLRBottom

    // Last 4 edges (L4E)
    let mud2Moves = [Move Move.M; Move Move.M'; Move Move.M2; Move Move.U2; Move Move.D2]
    let caseSolved cube =
        let solved face color = Map.find face cube |> Map.forall (fun _ col -> col = color)
        solved Face.F Color.R &&
        solved Face.B Color.O &&
        solved Face.L Color.B &&
        solved Face.R Color.G &&
        solved Face.U Color.Y &&
        solved Face.D Color.W
    let solved = solve mud2Moves "Last 4 edges -> Solved!" "L4E" caseSolved solvedLR

    let avgTwistCount = float Cube.twistCount / float numCubes;
    printfn "Average Twists (STM): %f" avgTwistCount

    pause ()
genRoux ()

printfn "DONE!"
Console.ReadLine() |> ignore