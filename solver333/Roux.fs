module Roux

open Cube
open Solver
open Utility

let sune = "R U R' U R U2 R'"
let jperm = "R U R' F' R U R' U' R' F R2 U' R'" // without final AUF (U')
let mum = "M' U' M'"
let diagSwap = "r2 D r' U r D' R2 U' F' U' F" // fancy!
let sexy = "R U R' U'"
let sledge = "R' F R F'"

let matchesPieces (pieces: Piece list) (cube: Cube) (pattern: string, _: bool, _: bool) =
    pattern = Render.piecesToString cube pieces

let dlEdgeBeginnerPatterns = readPatterns matchesGeneric "Roux" "Beginner" "DLEdge" false false false // Solving DL edge (during inspection) [24 cases]
let lCenterBeginnerPatterns = readPatterns matchesGeneric "Roux" "Beginner" "LCenter" false false false // Solve left center [6 cases]

let lbPairBeginnerPatterns =
    let tuckLBtoFD = readPatterns matchesGeneric "Roux" "Beginner" "TuckLBtoFD" false false false // Tuck LB edge to FD [22 cases]
    let bringDLBtoU = readPatterns matchesGeneric "Roux" "Beginner" "BringDLBtoU" false false false // Bring DLB corner to URB [24 cases]
    let insertLBPair = readPatterns matchesGeneric "Roux" "Beginner" "InsertLBPair" false false false // Insert LB pair [1 case]
    tuckLBtoFD @ bringDLBtoU @ insertLBPair

let lbPairIntermediatePatterns = readPatterns matchesGeneric "Roux" "Intermediate" "InsertLBPair" false false false

let matchesLBSquare (cube: Cube) =
    let pieces =
        let (c, _) = Cube.findCenter Color.B cube
        let (dl, _) = Cube.findEdge Color.B Color.W cube
        let (bl, _) = Cube.findEdge Color.B Color.O cube
        let (dlb, _) = Cube.findCorner Color.B Color.O Color.W cube
        [Center c; Edge dl; Edge bl; Corner dlb]
    matchesPieces pieces cube
let lbSquareGodPattern = readPatterns matchesLBSquare "Roux" "God" "BuildLBSquare" false false false

let lfPairBeginnerPatterns =
    let tuckLFtoBD = readPatterns matchesGeneric "Roux" "Beginner" "TuckLFtoBD" false false false // Tuck LF to BD [20 cases]
    let bringDLFtoURF = readPatterns matchesGeneric "Roux" "Beginner" "BringDLFtoURF" false false false // Bring DLB corner to URB [24 cases]
    let insertLFPair = readPatterns matchesGeneric "Roux" "Beginner" "InsertLFPair" false false false // Pair and insert LF pair (complete FB) [1 case]
    tuckLFtoBD @ bringDLFtoURF @ insertLFPair

let lfPairIntermediatePatterns = readPatterns matchesGeneric "Roux" "Intermediate" "InsertLFPair" false false false

let fbBeginnerPatterns = dlEdgeBeginnerPatterns @ lCenterBeginnerPatterns @ lbPairBeginnerPatterns @ lfPairBeginnerPatterns
let fbIntermediatePatterns = dlEdgeBeginnerPatterns @ lCenterBeginnerPatterns @ lbPairIntermediatePatterns @ lfPairIntermediatePatterns
let fbAdvancedPatterns = dlEdgeBeginnerPatterns @ lCenterBeginnerPatterns @ lbPairIntermediatePatterns @ lfPairIntermediatePatterns
let fbGodPatterns = lbSquareGodPattern @ lfPairIntermediatePatterns

let drEdgeBeginnerPatterns = readPatterns matchesGeneric "Roux" "Beginner" "DREdge" false false false // Solve DR edge [18 cases] (restricted to U, R/r, F, B, M)

let rbPairBeginnerPatterns =
    let tuckRBtoFD = readPatterns matchesGeneric "Roux" "Beginner" "TuckRBtoFD" false false false // Tuck RB to FD [16 cases] (restricted to U, R/r, F, B, M)
    let bringDRBtoULB = readPatterns matchesGeneric "Roux" "Beginner" "BringDRBtoULB" false false false // Bring DRB to ULB [18 cases] (restricted to U, R/r, F, B, M)
    let insertRBPair = readPatterns matchesGeneric "Roux" "Beginner" "InsertRBPair" false false false // Pair and insert RB pair [1 case] (restricted to U, R/r, F, B, M)
    tuckRBtoFD @ bringDRBtoULB @ insertRBPair

let rbPairIntermediatePatterns = readPatterns matchesGeneric "Roux" "Intermediate" "InsertRBPair" false false false

let rfPairBeginnerPatterns = [
    // Tuck RF to BD [14 cases] (restricted to U, R/r, F, B, M)
    matchesGeneric, "TuckRFtoBD", ("OGOO.O.....................BBBR...GGBBBR...GGW..W.WWRW", false, false, false), [] // skip
    matchesGeneric, "TuckRFtoBD", ("O.OO.O.G..R................BBBR...GGBBBR...GGW..W.WW.W", false, false, false), ["U2 M2"]
    matchesGeneric, "TuckRFtoBD", ("O.OO.O.....................BBBR.RGGGBBBR...GGW..W.WW.W", false, false, false), ["R U R r2"; "R U R' M2"; "R U r' M"; "R U r2 R"; "R U M r'"; "R U M2 R'"; "r U R r2"; "r U R' M2"; "r U r' M"; "r U r2 R"; "r U M r'"; "r U M2 R'"; "F' U2 F M'"]
    matchesGeneric, "TuckRFtoBD", ("O.OO.O.R..G................BBBR...GGBBBR...GGW..W.WW.W", false, false, false), ["M'"]
    matchesGeneric, "TuckRFtoBD", ("O.OO.O..........R.....G....BBBR...GGBBBR...GGW..W.WW.W", false, false, false), ["M2"]
    matchesGeneric, "TuckRFtoBD", ("OROO.O.....................BBBR...GGBBBR...GGW..W.WWGW", false, false, false), ["M U2 M2"; "M2 U2 M'"]
    matchesGeneric, "TuckRFtoBD", ("O.OO.O........R..........G.BBBR...GGBBBR...GGW..W.WW.W", false, false, false), ["U M2"]
    matchesGeneric, "TuckRFtoBD", ("O.OO.O......G......R.......BBBR...GGBBBR...GGW..W.WW.W", false, false, false), ["U M'"]
    matchesGeneric, "TuckRFtoBD", ("O.OO.O......R......G.......BBBR...GGBBBR...GGW..W.WW.W", false, false, false), ["U' M2"]
    matchesGeneric, "TuckRFtoBD", ("O.OO.O........G..........R.BBBR...GGBBBR...GGW..W.WW.W", false, false, false), ["U' M'"]
    matchesGeneric, "TuckRFtoBD", ("O.OO.O.....................BBBR.GRGGBBBR...GGW..W.WW.W", false, false, false), ["R U' R' M'"; "R U' R2 r"; "R U' r R2"; "R U' r' M2"; "R U' M' R'"; "R U' M2 r'"; "r U' R' M'"; "r U' R2 r"; "r U' r R2"; "r U' r' M2"; "r U' M' R'"; "r U' M2 r'"]
    matchesGeneric, "TuckRFtoBD", ("O.OO.O..........G.....R....BBBR...GGBBBR...GGW..W.WW.W", false, false, false), ["U2 M'"]
    matchesGeneric, "TuckRFtoBD", ("O.OO.O.....................BBBR...GGBBBRR..GGWG.W.WW.W", false, false, false), ["M"]
    matchesGeneric, "TuckRFtoBD", ("O.OO.O.....................BBBR...GGBBBRG..GGWR.W.WW.W", false, false, false), ["M' U2 M'"; "M2 U2 M2"]
    // Bring DRF to ULF [15 cases] (restricted to U, R/r, F, B, M)
    matchesGeneric, "BringDRFtoULF", ("OGOO.O.........R....WG.....BBBR...GGBBBR...GGW..W.WWRW", false, false, false), [] // skip
    matchesGeneric, "BringDRFtoULF", ("OGOO.OR..G........W........BBBR...GGBBBR...GGW..W.WWRW", false, false, false), ["U2 R U2 R'"; "U2 r U2 r'"; "F' U' F U"]
    matchesGeneric, "BringDRFtoULF", ("OGOO.O..R..W..............GBBBR...GGBBBR...GGW..W.WWRW", false, false, false), ["R' F R F'"]
    matchesGeneric, "BringDRFtoULF", ("OGOO.O.....................BBBR...GGBBBR.WRGGW.GW.WWRW", false, false, false), ["R U' R' U"; "r U' r' U"; "F' U' F U2"; "F' U2 F U'"]
    matchesGeneric, "BringDRFtoULF", ("OGOO.O...........R.....WG..BBBR...GGBBBR...GGW..W.WWRW", false, false, false), ["U"]
    matchesGeneric, "BringDRFtoULF", ("OGOO.O..G..R..............WBBBR...GGBBBR...GGW..W.WWRW", false, false, false), ["U2"]
    matchesGeneric, "BringDRFtoULF", ("OGOO.O..W..G..............RBBBR...GGBBBR...GGW..W.WWRW", false, false, false), ["U R U2 R'"; "U r U2 r'"; "F R' F' R"; "F' U2 F U"]
    matchesGeneric, "BringDRFtoULF", ("OGOO.O.........G....RW.....BBBR...GGBBBR...GGW..W.WWRW", false, false, false), ["U' R U2 R'"; "U' r U2 r'"]
    matchesGeneric, "BringDRFtoULF", ("OGOO.O.....................BBBR...GGBBBR.RGGGW.WW.WWRW", false, false, false), ["R U R'"; "r U r'"]
    matchesGeneric, "BringDRFtoULF", ("OGOO.OW..R........G........BBBR...GGBBBR...GGW..W.WWRW", false, false, false), ["U'"]
    matchesGeneric, "BringDRFtoULF", ("OGOO.O.........W....GR.....BBBR...GGBBBR...GGW..W.WWRW", false, false, false), ["R U2 R' U"; "r U2 r' U"]
    matchesGeneric, "BringDRFtoULF", ("OGOO.O...........W.....GR..BBBR...GGBBBR...GGW..W.WWRW", false, false, false), ["F' U F U'"; "F' U2 F U2"]
    matchesGeneric, "BringDRFtoULF", ("OGOO.OG..W........R........BBBR...GGBBBR...GGW..W.WWRW", false, false, false), ["R U R' U"; "r U r' U"]
    matchesGeneric, "BringDRFtoULF", ("OGOO.O.....................BBBR...GGBBBR.GWGGW.RW.WWRW", false, false, false), ["R U' B U' B' R'"; "R B U2 B' R' U"; "R2 B' R' B U' R'"; "R2 B' R' B R' U'"; "r U' B U' B' r'"; "r B U2 B' r' U"; "r2 B r2 B' U2 B"; "F U2 F U2 F' U2"; "F R' F' R2 U2 R'"; "F' U F R U2 R'"; "F' U F r U2 r'"; "B r2 B r2 B' U2"]
    matchesGeneric, "BringDRFtoULF", ("OGOO.O...........G.....RW..BBBR...GGBBBR...GGW..W.WWRW", false, false, false), ["R U2 R'"; "r U2 r'"]
    // Pair and insert RF pair (complete SB) [1 case] (restricted to U, R/r, F, B, M)
    matchesGeneric, "InsertRFPair", ("OGOO.O.........R....WG.....BBBR...GGBBBR...GGW..W.WWRW", false, false, false), ["R M2 U' R'"; "R M2 U' r'"; "R' r2 U' R'"; "R' r2 U' r'"; "r M' U' R'"; "r M' U' r'"; "r2 R' U' R'"; "r2 R' U' r'"; "M' r U' R'"; "M' r U' r'"; "M2 R U' R'"; "M2 R U' r'"]]

let rfPairIntermediatePatterns = readPatterns matchesGeneric "Roux" "Intermediate" "InsertRFPair" false false false

let sbBeginnerPatterns = drEdgeBeginnerPatterns @ rbPairBeginnerPatterns @ rfPairBeginnerPatterns

let centerOrientationPatterns = [
    // Orient center - hand authored patterns [2 cases]
    matchesGeneric, "CenterOrientation", ("O.OO.O.......E.............BBBR.RGGGBBBR.RGGGW.WWEWW.W", true, true, false), [] // skip
    matchesGeneric, "CenterOrientation", ("O.OO.O.......P.............BBBR.RGGGBBBR.RGGGW.WWPWW.W", true, true, false), ["M"]] // or M'

let sbIntermediatePatterns = centerOrientationPatterns @ drEdgeBeginnerPatterns @ rbPairIntermediatePatterns @ rfPairIntermediatePatterns

let coBeginnerPatterns = [
    // Orient corners (CO) - hand authored patterns [8 cases] - single-alg (sune)
    matchesGeneric, "CornerOrientation", ("O.OO.O...Y.Y...Y.Y.........BBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, false), [] // skip
    matchesGeneric, "CornerOrientation", ("O.OO.OY.Y............Y.Y...BBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true), [sune + " U2 " + sune + " " + sune + " U2 " + sune] // H - 31 twists (4 sunes) - easy to learn, but not bestcoH]
    matchesGeneric, "CornerOrientation", ("O.OO.OY..............Y..Y.YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true), [sune + " U2 " + sune + " U' " + sune + " U2 " + sune] // Pi - 31 twists (4 sunes)
    matchesGeneric, "CornerOrientation", ("O.OO.O...Y.Y.........Y.Y...BBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true), [sune + " U' " + sune + " U2 " + sune] // U - 23 twists (3 sunes)
    matchesGeneric, "CornerOrientation", ("O.OO.OY....Y.....Y...Y.....BBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true), [sune + " U " + sune + " U2 " + sune] // T - 23 twists (3 sunes)
    matchesGeneric, "CornerOrientation", ("O.OO.OY........Y.......Y..YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true), [sune] // sune - 7 twists (1 sune)
    matchesGeneric, "CornerOrientation", ("O.OO.O..Y......Y..Y.....Y..BBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true), [sune + " U2 " + sune] // anti-sune - 15 twists (2 sunes)
    matchesGeneric, "CornerOrientation", ("O.OO.O...Y.......Y...Y....YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true), [sune + " " + sune + " U2 " + sune]] // L - 22 twists (3 sunes)

let coIntermediatePatterns = [
    // Orient corners (CO) - hand authored patterns [8 cases] - 7 algs (~13 STM better than beginner)
    matchesGeneric, "CornerOrientation", ("O.OO.O...Y.Y...Y.Y.........BBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, false), [] // skip
    matchesGeneric, "CornerOrientation", ("O.OO.OY.Y............Y.Y...BBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true), ["R U2 R' U' " + sexy + " R U' R'"] // H
    matchesGeneric, "CornerOrientation", ("O.OO.O..Y.........Y.Y..Y...BBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true), ["F " + sexy + " " + sexy + " F'"] // Pi
    matchesGeneric, "CornerOrientation", ("O.OO.O.....Y.....YY.Y......BBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true), ["F " + sexy + " F'"] // U
    matchesGeneric, "CornerOrientation", ("O.OO.OY....Y.....Y...Y.....BBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true), [sexy + " " + sledge] // T
    matchesGeneric, "CornerOrientation", ("O.OO.OY........Y.......Y..YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true), [sune] // sune
    matchesGeneric, "CornerOrientation", ("O.OO.O..Y........YY..Y.....BBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true), ["L' U' L U' L' U2 L"] // anti-sune
    matchesGeneric, "CornerOrientation", ("O.OO.O...Y.......Y...Y....YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true), ["F R' F' R U R U' R'"]] // L

let cpBeginnerPatterns = [
    // Permute corners (CP) - hand authored patterns [3 cases] - single-alg (jperm)
    matchesGeneric, "CornerPermutation", ("O.OO.OO.OY.Y...Y.YB.BR.RG.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, false), [] // skip (color neutral)
    matchesGeneric, "CornerPermutation", ("O.OO.OG.RY.Y...Y.YO.OB.RG.BBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), [jperm] // adjacent (color neutral)
    matchesGeneric, "CornerPermutation", ("O.OO.OR.OY.Y...Y.YG.BR.OB.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, false), [jperm + " U " + jperm]] // diag (color neutral)

let cpIntermediatePatterns = [
    // Permute corners (CP) - hand authored patterns [3 cases] - (two algs: jperm [same as beginner] and diagSwap) (just diag swap, ~3 STM better than beginner)
    matchesGeneric, "CornerPermutation", ("O.OO.OO.OY.Y...Y.YB.BR.RG.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, false), [] // skip (color neutral)
    matchesGeneric, "CornerPermutation", ("O.OO.OG.RY.Y...Y.YO.OB.RG.BBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), [jperm] // adjacent (color neutral)
    matchesGeneric, "CornerPermutation", ("O.OO.OR.OY.Y...Y.YG.BR.OB.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, false), [diagSwap]] // diag (color neutral)

let cmllBeginnerPatterns = coBeginnerPatterns @ cpBeginnerPatterns
let cmllIntermediatePatterns = coIntermediatePatterns @ cpIntermediatePatterns // two-look

let cmllAdvancedPatterns = [
    // Full CMLL - hand authored patterns [?? cases] (~10 STM better than intermediate)
    matchesGeneric, "CornerOrientation", ("O.OO.OO.OY.Y...Y.YB.BR.RG.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, false), [] // skip (color neutral)
    matchesGeneric, "CornerOrientation", ("O.OO.OG.RY.Y...Y.YO.OB.RG.BBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), [jperm] // O adjacent (color neutral)
    matchesGeneric, "CornerOrientation", ("O.OO.OR.OY.Y...Y.YG.BR.OB.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, false), [diagSwap] // O diag (color neutral)
    matchesGeneric, "CornerOrientation", ("O.OO.OY.YO.O...R.RG.GY.YB.BBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), ["F " + sexy + " " + sexy + " " + sexy + " F'"] // H rows
    matchesGeneric, "CornerOrientation", ("O.OO.OR.OB.B...G.GY.YR.OY.YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), ["R U R' U R U' R' U R U2 R'"] // H columns
    matchesGeneric, "CornerOrientation", ("O.OO.OR.RB.G...O.OY.YG.BY.YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), ["R U2' R2' F R F' U2 R' F R F'"] // H column
    matchesGeneric, "CornerOrientation", ("O.OO.OY.YB.B...R.OO.GY.YG.RBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), ["r U' r2' D' r U' r' D r2 U r'"] // H row (top)
    matchesGeneric, "CornerOrientation", ("O.OO.OB.YO.G...R.GY.YB.YR.OBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), ["F R U R' U' R U R' U' F'"] // Pi right bar
    matchesGeneric, "CornerOrientation", ("O.OO.OB.BO.R...G.GY.OY.YR.YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), ["R' U' R' F R F' R U' R' U2 R"] // Pi left bar
    matchesGeneric, "CornerOrientation", ("O.OO.OY.YG.O...O.BR.YG.RY.BBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), ["F R' F' R U2 R U' R' U R U2' R'"] // Pi back slash
    matchesGeneric, "CornerOrientation", ("O.OO.OG.YR.B...B.OY.YO.YG.RBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), ["R U2 R' U' R U R' U2' R' F R F'"] // Pi forward slash
    matchesGeneric, "CornerOrientation", ("O.OO.OB.BO.R...R.OY.GY.YG.YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), ["R' F R U F U' R U R' U' F'"] // Pi checkerboard
    matchesGeneric, "CornerOrientation", ("O.OO.OG.BR.R...O.OY.BY.YG.YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), ["r U' r2' D' r U r' D r2 U r'"] // Pi rows
    matchesGeneric, "CornerOrientation", ("O.OO.OG.OR.Y...R.YY.YB.OB.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), ["F R U R' U' F'"] // U column
    matchesGeneric, "CornerOrientation", ("O.OO.OO.GY.Y...G.RB.OY.YB.RBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), ["R2 D R' U2 R D' R' U2 R'"] // U forward slash
    matchesGeneric, "CornerOrientation", ("O.OO.OY.YR.G...Y.YB.OB.RG.OBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), ["R2' D' R U2 R' D R U2 R"] // U back slash
    matchesGeneric, "CornerOrientation", ("O.OO.OY.YG.B...Y.YR.GO.OB.RBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), ["R2' F U' F U F2 R2 U' R' F R"] // U back row
    matchesGeneric, "CornerOrientation", ("O.OO.OG.BR.Y...R.YY.YB.GO.OBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), ["F R2 D R' U R D' R2' U' F'"] // U columns
    matchesGeneric, "CornerOrientation", ("O.OO.OG.BY.Y...B.GO.RY.YR.OBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), ["r U' r' U r' D' r U' r' D r"] // U checkerboard
    matchesGeneric, "CornerOrientation", ("O.OO.OY.BR.Y...G.YB.OY.RG.OBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), ["R U R' U' R' F R F'"] // T row (to left)
    matchesGeneric, "CornerOrientation", ("O.OO.OG.YY.R...Y.BO.BR.YO.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), ["L' U' L U L F' L' F"] // T row (to right)
    matchesGeneric, "CornerOrientation", ("O.OO.OG.BR.R...Y.YY.GO.OB.YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), ["F R' F R2 U' R' U' R U R' F2"] // T rows
    matchesGeneric, "CornerOrientation", ("O.OO.OR.RB.G...Y.YY.GO.OB.YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), ["r' U r U2' R2' F R F' R"] // T front row
    matchesGeneric, "CornerOrientation", ("O.OO.OR.OB.B...Y.YY.GO.RG.YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), ["r' D' r U r' D r U' r U r'"] // T back row
    matchesGeneric, "CornerOrientation", ("O.OO.OG.BY.Y...G.BO.YR.RY.OBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), ["r2' D' r U r' D r2 U' r' U' r"] // T columns
    matchesGeneric, "CornerOrientation", ("O.OO.OY.GB.O...Y.GO.BR.YR.YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), [sune] // S top row
    matchesGeneric, "CornerOrientation", ("O.OO.OY.GG.O...Y.RR.OB.YB.YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), ["L' U2 L U2' L F' L' F"] // S checkerboard
    matchesGeneric, "CornerOrientation", ("O.OO.OY.GR.O...Y.BB.RG.YO.YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), ["F R' F' R U2 R U2' R'"] // S forward slash
    matchesGeneric, "CornerOrientation", ("O.OO.OY.GB.O...Y.RO.RG.YB.YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), ["R U R' U' R' F R F' R U R' U R U2 R'"] // S rows (not best alg, but what I use)
    matchesGeneric, "CornerOrientation", ("O.OO.OY.BO.Y...G.RG.YR.YB.OBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), ["R U R' U R' F R F' R U2' R'"] // S bottom row
    matchesGeneric, "CornerOrientation", ("O.OO.OY.BO.R...Y.BG.RG.YO.YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), ["R U' L' U R' U' L"] // S back slash
    matchesGeneric, "CornerOrientation", ("O.OO.OR.YB.O...R.YY.GY.GO.BBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), ["L' U' L U' L' U2 L"] // AS top row
    matchesGeneric, "CornerOrientation", ("O.OO.OB.YO.G...R.YY.GY.BR.OBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), ["R2 D R' U R D' R' U R' U' R U' R'"] // AS rows
    matchesGeneric, "CornerOrientation", ("O.OO.OR.YB.G...O.YY.BY.RG.OBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), ["F' L F L' U2' L' U2 L"] // AS back slash
    matchesGeneric, "CornerOrientation", ("O.OO.OG.YR.G...O.YY.BY.BR.OBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), ["R U2' R' U2 R' F R F'"] // AS checkerboard
    matchesGeneric, "CornerOrientation", ("O.OO.OR.YB.G...R.YY.GY.OB.OBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), ["L' U R U' L U R'"] // AS back slash
    matchesGeneric, "CornerOrientation", ("O.OO.OB.YO.B...G.YY.OY.RG.RBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), ["R' U' R U' L U' R' U L' U2 R"] // AS bottom row
    matchesGeneric, "CornerOrientation", ("O.OO.OO.YY.B...G.YB.YR.GO.RBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), ["R2' D' R U' R' D R U R"] // L mirror
    matchesGeneric, "CornerOrientation", ("O.OO.OO.RY.G...B.YB.RY.GO.YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), ["F R' F' R U R U' R'"] // L inverse
    matchesGeneric, "CornerOrientation", ("O.OO.OR.YY.B...O.YG.YG.OB.RBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), ["R U2 R' U' R U R' U' R U R' U' R U' R'"] // L pure
    matchesGeneric, "CornerOrientation", ("O.OO.OO.BY.R...R.YB.GY.GO.YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), ["R U2 R D R' U2 R D' R2'"] // L front commutator
    matchesGeneric, "CornerOrientation", ("O.OO.OG.YY.O...G.YO.YR.BR.BBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), ["R' U' R U R' F' R U R' U' R' F R2"] // L diag
    matchesGeneric, "CornerOrientation", ("O.OO.OR.YY.O...O.YG.YG.BR.BBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true), ["U R' U2 R' D' R U2 R' D R2"] // L back commutator
    // Permute corners (CP) - always skip!
    matchesGeneric, "CornerPermutation", ("O.OO.OO.OY.Y...Y.YB.BR.RG.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, false), []] // always skip! (color neutral)

let edgeBeginnerOrientationPatters = [
    // Orient edges (EO) - hand authored patterns [12 cases]
    matchesGeneric, "EdgeOrientation", ("O.OO.OO.OYEYEEEYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWEW", true, true, false), [] // skip
    matchesGeneric, "EdgeOrientation", ("O.OO.OO.OYPYEEEYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWEW", true, true, true),  [mum + " " + mum + " U' " + mum] // 2-up vertical
    matchesGeneric, "EdgeOrientation", ("O.OO.OO.OYEYEEEYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWPW", true, true, false), ["M2 " +  mum + " " + mum + " U' " + mum] // 2-down
    matchesGeneric, "EdgeOrientation", ("O.OO.OO.OYPYEEPYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWEW", true, true, true),  [mum + " U2 " + mum] // 2-up L (NE)
    matchesGeneric, "EdgeOrientation", ("O.OO.OO.OYPYEEEYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWPW", true, true, true),  [mum + " " + mum + " U2 " + mum + " U " + mum + " U' " + mum] // 2 B/B
    matchesGeneric, "EdgeOrientation", ("O.OO.OO.OYPYEEEYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWEW", true, true, true),  [mum + " U " + mum + " U' " + mum] // 2 B/F
    matchesGeneric, "EdgeOrientation", ("O.OO.OO.OYPYPEPYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWEW", true, true, false), [mum + " U' " + mum + " U " + mum + " U' " + mum] // 4 up
    matchesGeneric, "EdgeOrientation", ("O.OO.OO.OYEYPEPYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWEW", true, true, true),  [mum] // arrow F/F
    matchesGeneric, "EdgeOrientation", ("O.OO.OO.OYEYPEPYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWPW", true, true, true),  [mum + " U' " + mum + " U2 " + mum] // arrow F/B
    matchesGeneric, "EdgeOrientation", ("O.OO.OO.OYEYPEPYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWPW", true, true, true),  [mum + " U2 " + mum + " U2 " + mum] // 2-up/2-down horizontal (special case - leave horizontal)
    matchesGeneric, "EdgeOrientation", ("O.OO.OO.OYPYEEPYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWPW", true, true, true),  [mum + " U' " + mum] // 2-up L (NE)/2-down
    matchesGeneric, "EdgeOrientation", ("O.OO.OO.OYPYPEPYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWPW", true, true, false), [mum + " U2 " + mum + " U " + mum + " U' " + mum]] // 6-flip

let edgeIntermediateOrientationPatters = [
    // Orient edges (EO) - hand authored patterns (~4 STM better than beginner) [19 cases]
    matchesGeneric, "EdgeOrientation", ("O.OO.OO.OYEYEEEYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWEW", true, true, false), [] // skip
    matchesGeneric, "EdgeOrientation", ("O.OO.OO.OYPYEEEYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWEW", true, true, true),  ["M' U M U' " + mum] // 2-up vertical
    matchesGeneric, "EdgeOrientation", ("O.OO.OO.OYEYEEEYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWPW", true, true, false), [mum + " U M U' M'"] // 2-down
    matchesGeneric, "EdgeOrientation", ("O.OO.OO.OYEYEEPYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWEW", true, true, false), ["M U M' U2 M U' M'"] // 2-up L (SE)
    matchesGeneric, "EdgeOrientation", ("O.OO.OO.OYEYPEEYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWEW", true, true, false), ["M U' M' U2 M U' M'"] // 2-up L (SW)
    matchesGeneric, "EdgeOrientation", ("O.OO.OO.OYPYPEEYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWEW", true, true, false), ["M' U M' U2 " + mum] // 2-up L (NW)
    matchesGeneric, "EdgeOrientation", ("O.OO.OO.OYPYEEPYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWEW", true, true, false), ["M' U' M U2 " + mum] // 2-up L (NE)
    matchesGeneric, "EdgeOrientation", ("O.OO.OO.OYEYEEEYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWPW", true, true, true),  [mum + " U' M U' M'"] // 2 F/B
    matchesGeneric, "EdgeOrientation", ("O.OO.OO.OYPYEEEYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWEW", true, true, true),  ["M' U' M U' " + mum] // 2 B/F
    matchesGeneric, "EdgeOrientation", ("O.OO.OO.OYPYPEPYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWEW", true, true, false), ["M' U2 M' U2 " + mum] // 4 up
    matchesGeneric, "EdgeOrientation", ("O.OO.OO.OYPYPEPYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWEW", true, true, false), [mum + " U' " + mum + " U " + mum + " U' " + mum] // 4 up
    matchesGeneric, "EdgeOrientation", ("O.OO.OO.OYEYPEPYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWEW", true, true, true),  [mum] // arrow F/F
    matchesGeneric, "EdgeOrientation", ("O.OO.OO.OYPYPEPYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWPW", true, true, true),  ["M U' M'"] // arrow B/B (easy)
    matchesGeneric, "EdgeOrientation", ("O.OO.OO.OYEYPEPYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWPW", true, true, true),  ["M' U2 M' U2 M U' M'"] // 2-up/2-down horizontal (special case - leave horizontal)
    matchesGeneric, "EdgeOrientation", ("O.OO.OO.OYEYEEPYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWPW", true, true, false), ["M2 U' M U' M'"] // 2-up L (SE)/2-down
    matchesGeneric, "EdgeOrientation", ("O.OO.OO.OYEYPEEYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWPW", true, true, false), ["M2 U M U' M'"] // 2-up L (SW)/2-down
    matchesGeneric, "EdgeOrientation", ("O.OO.OO.OYPYPEEYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWPW", true, true, false), ["M2 U' " + mum] // 2-up L (NW)/2-down
    matchesGeneric, "EdgeOrientation", ("O.OO.OO.OYPYEEPYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWPW", true, true, false), ["M2 U " + mum] // 2-up L (NE)/2-down
    matchesGeneric, "EdgeOrientation", ("O.OO.OO.OYPYPEPYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWPW", true, true, false), ["R U' r' U' M' U r U r'"]] // 6-flip (fancy!)

let eoBeginnerPatterns = centerOrientationPatterns @ edgeBeginnerOrientationPatters
let eoIntermediatePatterns = edgeIntermediateOrientationPatters

let lrBeginnerPatterns = [
    // L edge to DF
    matchesGeneric, "LToDF", ("OPOOPOOPOYEYEEEYEYBPBRPRGPGBBBRPRGGGBBBRBRGGGWEWWEWWEW", true, true, false), [] // skip
    matchesGeneric, "LToDF", ("OPOOPOOBOYEYEEEYEYBPBRPRGPGBBBRPRGGGBBBRPRGGGWEWWEWWEW", true, true, true),  ["M2"]
    matchesGeneric, "LToDF", ("OBOOPOOPOYEYEEEYEYBPBRPRGPGBBBRPRGGGBBBRPRGGGWEWWEWWEW", true, true, false), ["M U2 M"; "M2 U2 M2"]
    // LR edges to bottom
    matchesGeneric, "LREdgesBottom", ("OGOO.OOPOYEYEEEYEYBPBR.RGPGBBBRPRGGGBBBRBRGGGWEWWEWWEW", true, true, false), [] // skip
    matchesGeneric, "LREdgesBottom", ("OPOO.OOGOYEYEEEYEYBPBR.RGPGBBBRPRGGGBBBRBRGGGWEWWEWWEW", true, true, true), ["M U2 M'"]
    // LR solved (AUF and up corner colors matter again)
    matchesGeneric,"LREdges", ("OGOO.OBPBYEYEEEYEYRPRG.GOPOBBBRPRGGGBBBRBRGGGWEWWEWWEW", false, false, true), ["M2 U'"]]

let lrIntermediatePatterns = [
    // LR edges to bottom *directly* after EO (incomplete patterns; not restrictive enough)
    matchesGeneric, "LREdges", ("O.OO.OO.OY.Y...Y.YBBBR.RGGGBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true),  [] // LR
    matchesGeneric, "LREdges", ("OGOO.OG.GY.Y...Y.YO.OBBBR.RBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true),  ["M' U2 M' U'"] // L0
    matchesGeneric, "LREdges", ("O.OO.OOGOY.Y...Y.YBBBR.RG.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, false), ["M2 U M U2 M U"] // L1
    matchesGeneric, "LREdges", ("O.OO.OR.RY.Y...Y.YG.GOGOBBBBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true),  ["M2 U M' U2 M' U'"] // L1
    matchesGeneric, "LREdges", ("O.OO.ORGRY.Y...Y.YG.GO.OBBBBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, false), ["M2 U' M U2 M U"] // L2
    matchesGeneric, "LREdges", ("O.OO.OO.OY.Y...Y.YBBBRGRG.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true),  ["M2 U' M' U2 M' U'"] // L2
    matchesGeneric, "LREdges", ("O.OO.OBBBY.Y...Y.YR.RG.GO.OBBBR.RGGGBBBRGRGGGW.WW.WW.W", false, false, true),  ["M U2 M U"] // L3
    matchesGeneric, "LREdges", ("O.OO.OBGBY.Y...Y.YR.RGBGO.OBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, false), ["M2 U2 M2 U"] // RL
    matchesGeneric, "LREdges", ("O.OO.OGBGY.Y...Y.YO.OBGBR.RBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true),  ["M2 U2 M2 U'"] // RL
    matchesGeneric, "LREdges", ("OGOO.OB.BY.Y...Y.YR.RGBGO.OBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true),  ["M U2 M' U"] // R0
    matchesGeneric, "LREdges", ("O.OO.OR.RY.Y...Y.YGBGOGOB.BBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, false), ["M2 U' M U2 M' U"] // R1
    matchesGeneric, "LREdges", ("O.OO.OOGOY.Y...Y.YB.BR.RGBGBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true),  ["M2 U' M' U2 M U'"] // R1
    matchesGeneric, "LREdges", ("O.OO.OO.OY.Y...Y.YB.BRGRGBGBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, false), ["M2 U M U2 M' U"] // R2
    matchesGeneric, "LREdges", ("O.OO.ORGRY.Y...Y.YGBGO.OB.BBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true),  ["M2 U M' U2 M U'"] // R2
    matchesGeneric, "LREdges", ("O.OO.OGBGY.Y...Y.YO.OB.BR.RBBBR.RGGGBBBRGRGGGW.WW.WW.W", false, false, true),  ["M' U2 M U'"] // R3
    matchesGeneric, "LREdges", ("OBOO.OG.GY.Y...Y.YO.OBGBR.RBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true),  ["M U2 M' U'"] // 0L
    matchesGeneric, "LREdges", ("OBOO.OB.BY.Y...Y.YR.RGGGO.OBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true),  ["M' U2 M' U"] // 0R
    matchesGeneric, "LREdges", ("OBOO.OR.RY.Y...Y.YG.GOGOB.BBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true),  ["M' U2 M U' M2 U"; "M U2 M U M2 U'"] // 01
    matchesGeneric, "LREdges", ("OBOO.OO.OY.Y...Y.YB.BRGRG.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true),  ["M' U2 M U M2 U"; "M U2 M U' M2 U'"] // 02
    matchesGeneric, "LREdges", ("OBOO.OG.GY.Y...Y.YO.OB.BR.RBBBR.RGGGBBBRGRGGGW.WW.WW.W", false, false, true),  ["M2 U"] // 03
    matchesGeneric, "LREdges", ("O.OO.OOBOY.Y...Y.YBGBR.RG.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true),  ["M2 U M' U2 M U"] // 1L
    matchesGeneric, "LREdges", ("O.OO.OR.RY.Y...Y.YG.GOBOBGBBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true),  ["M2 U M U2 M' U'"] // 1L
    matchesGeneric, "LREdges", ("O.OO.OR.RY.Y...Y.YGGGOBOB.BBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true),  ["M2 U' M' U2 M' U"] // 1R
    matchesGeneric, "LREdges", ("OGOO.OR.RY.Y...Y.YG.GOBOB.BBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true),  ["M' U2 M U M2 U'"; "M U2 M U' M2 U"] // 10
    matchesGeneric, "LREdges", ("O.OO.ORGRY.Y...Y.YG.GOBOB.BBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, false), ["M2 U M2 U"]  // 12
    matchesGeneric, "LREdges", ("O.OO.OOBOY.Y...Y.YB.BRGRG.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true),  ["M2 U M2 U'"] // 12
    matchesGeneric, "LREdges", ("O.OO.OOBOY.Y...Y.YB.BR.RG.GBBBR.RGGGBBBRGRGGGW.WW.WW.W", false, false, true),  ["M' U2 M' U' M2 U'"; "M U2 M' U M2 U"] // 13
    matchesGeneric, "LREdges", ("O.OO.OO.OY.Y...Y.YBGBRBRG.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, false), ["M2 U' M U2 M' U'"] // 2L
    matchesGeneric, "LREdges", ("O.OO.ORBRY.Y...Y.YG.GO.OBGBBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true),  ["M2 U' M' U2 M U"] // 2L
    matchesGeneric, "LREdges", ("O.OO.ORBRY.Y...Y.YGGGO.OB.BBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, false), ["M2 U M U2 M U'"]  // 2R
    matchesGeneric, "LREdges", ("O.OO.OO.OY.Y...Y.YB.BRBRGGGBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true),  ["M2 U M' U2 M' U"] // 2R
    matchesGeneric, "LREdges", ("OGOO.OO.OY.Y...Y.YB.BRBRG.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true),  ["M' U2 M U' M2 U'"; "M U2 M U M2 U"] // 20
    matchesGeneric, "LREdges", ("O.OO.OOGOY.Y...Y.YB.BRBRG.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, false), ["M2 U' M2 U"] // 21
    matchesGeneric, "LREdges", ("O.OO.ORBRY.Y...Y.YG.GOGOB.BBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true),  ["M2 U' M2 U'"] // 21
    matchesGeneric, "LREdges", ("O.OO.ORBRY.Y...Y.YG.GO.OB.BBBBR.RGGGBBBRGRGGGW.WW.WW.W", false, false, true),  ["M' U2 M' U M2 U'"; "M U2 M' U' M2 U"] // 23
    matchesGeneric, "LREdges", ("O.OO.OBGBY.Y...Y.YR.RG.GO.OBBBR.RGGGBBBRBRGGGW.WW.WW.W", false, false, true),  ["M' U2 M U"] // 3L
    matchesGeneric, "LREdges", ("O.OO.OGGGY.Y...Y.YO.OB.BR.RBBBR.RGGGBBBRBRGGGW.WW.WW.W", false, false, true),  ["M U2 M U'"] // 3R
    matchesGeneric, "LREdges", ("OGOO.OB.BY.Y...Y.YR.RG.GO.OBBBR.RGGGBBBRBRGGGW.WW.WW.W", false, false, true),  ["M2 U'"] // 30
    matchesGeneric, "LREdges", ("O.OO.OOGOY.Y...Y.YB.BR.RG.GBBBR.RGGGBBBRBRGGGW.WW.WW.W", false, false, true),  ["M' U2 M' U M2 U"; "M U2 M' U' M2 U'"] // 31
    matchesGeneric, "LREdges", ("O.OO.ORGRY.Y...Y.YG.GO.OB.BBBBR.RGGGBBBRBRGGGW.WW.WW.W", false, false, true),  ["M' U2 M' U' M2 U"; "M U2 M' U M2 U'"]] // 32

let eolrBeginnerPatterns = eoBeginnerPatterns @ lrBeginnerPatterns
let eolrIntermediatePatterns = eoIntermediatePatterns @ lrIntermediatePatterns

let l4eBeginnerPatterns =
    let mu2 = "M' U2"
    // Last 4 edges -> Solved! - hand authored
    [matchesGeneric, "L4E", ("OOOOOOOOOYYYYYYYYYBBBRRRGGGBBBRRRGGGBBBRRRGGGWWWWWWWWW", false, false, false), [] // skip (solved!)
     matchesGeneric, "L4E", ("OROOROOROYWYYWYYWYBBBRORGGGBBBRORGGGBBBRORGGGWYWWYWWYW", false, false, false), ["M2"]
     matchesGeneric, "L4E", ("OOOOROOROYWYYWYYYYBBBRRRGGGBBBRORGGGBBBRORGGGWWWWYWWYW", false, false, false), [mu2 + " " + mu2]
     matchesGeneric, "L4E", ("OROOROOOOYWYYWYYYYBBBRRRGGGBBBRORGGGBBBRORGGGWYWWYWWWW", false, false, false), [mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2]
     matchesGeneric, "L4E", ("OROOOOOOOYYYYYYYWYBBBRORGGGBBBRRRGGGBBBRRRGGGWYWWWWWWW", false, false, false), [mu2 + " " + mu2 + " M2"]
     matchesGeneric, "L4E", ("OOOOOOOROYYYYYYYWYBBBRORGGGBBBRRRGGGBBBRRRGGGWWWWWWWYW", false, false, false), [mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " M2"]
     matchesGeneric, "L4E", ("OROOOOOOOYWYYYYYYYBBBRORGGGBBBRRRGGGBBBRRRGGGWWWWWWWYW", false, false, false), [mu2 + " " + mu2 + (* vertical bars *) " M' U2 M2 U2 M"]
     matchesGeneric, "L4E", ("OOOOOOOROYWYYYYYYYBBBRORGGGBBBRRRGGGBBBRRRGGGWYWWWWWWW", false, false, false), [mu2 + " " + mu2 + " " + mu2 + " " + mu2 + (* horizontal bars *) " U2 M2 U2 M2"] // (could spot, bar earlier, but...)
     matchesGeneric, "L4E", ("OOOOROOROYYYYWYYWYBBBRRRGGGBBBRORGGGBBBRORGGGWYWWYWWWW", false, false, false), [mu2 + " " + mu2 + (* vertical bars *) " M' U2 M2 U2 M'"]
     matchesGeneric, "L4E", ("OROOROOOOYYYYWYYWYBBBRRRGGGBBBRORGGGBBBRORGGGWWWWYWWYW", false, false, false), [mu2 + " " + mu2 + " " + mu2 + " " + mu2 + (* horizontal bars *) " U2 M2 U2"]
     matchesGeneric, "L4E", ("OOOOOOOROYYYYYYYWYBBBRRRGGGBBBRRRGGGBBBRORGGGWYWWWWWWW", false, false, false), [mu2 + " " + mu2 + " U2 M2 U2"]
     matchesGeneric, "L4E", ("OROOOOOOOYYYYYYYWYBBBRRRGGGBBBRRRGGGBBBRORGGGWWWWWWWYW", false, false, false), [mu2 + " " + mu2 + " " + mu2 + " " + mu2 + (* vertical bars *) " M' U2 M2 U2 M'"]
     matchesGeneric, "L4E", ("OROOROOOOYWYYWYYYYBBBRORGGGBBBRORGGGBBBRRRGGGWWWWYWWYW", false, false, false), [mu2 + " " + mu2 + (* horizontal bars *) " U2 M2 U2 M2"]
     matchesGeneric, "L4E", ("OOOOROOROYWYYWYYYYBBBRORGGGBBBRORGGGBBBRRRGGGWYWWYWWWW", false, false, false), [mu2 + " " + mu2 + " " + mu2 + " " + mu2 + (* vertical bars *) " M' U2 M2 U2 M"]
     matchesGeneric, "L4E", ("OROOROOOOYYYYWYYWYBBBRORGGGBBBRORGGGBBBRRRGGGWYWWYWWWW", false, false, false), [mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " M2"]
     matchesGeneric, "L4E", ("OOOOROOROYYYYWYYWYBBBRORGGGBBBRORGGGBBBRRRGGGWWWWYWWYW", false, false, false), [mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " M2"]
     matchesGeneric, "L4E", ("OOOOOOOROYWYYYYYYYBBBRRRGGGBBBRRRGGGBBBRORGGGWWWWWWWYW", false, false, false), [mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2]
     matchesGeneric, "L4E", ("OROOOOOOOYWYYYYYYYBBBRRRGGGBBBRRRGGGBBBRORGGGWYWWWWWWW", false, false, false), [mu2 + " " + mu2 + " " + mu2 + " " + mu2]
     matchesGeneric, "L4E", ("OOOOROOOOYWYYWYYWYBBBRRRGGGBBBRORGGGBBBRRRGGGWYWWYWWYW", false, false, false), ["U2 M2 U2"] // horizontal bars
     matchesGeneric, "L4E", ("OROOOOOROYYYYYYYYYBBBRORGGGBBBRRRGGGBBBRORGGGWWWWWWWWW", false, false, false), ["U2 M2 U2 M2"] // horizontal bars
     matchesGeneric, "L4E", ("OROOROOROYYYYWYYYYBBBRORGGGBBBRORGGGBBBRORGGGWWWWYWWWW", false, false, false), ["M U2 M2 U2 M'"] // vertical bars
     matchesGeneric, "L4E", ("OOOOOOOOOYWYYYYYWYBBBRRRGGGBBBRRRGGGBBBRRRGGGWYWWWWWYW", false, false, false), ["M' U2 M2 U2 M'"] // vertical bars
     matchesGeneric, "L4E", ("OROOOOOROYWYYYYYWYBBBRORGGGBBBRRRGGGBBBRORGGGWYWWWWWYW", false, false, false), [mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " M2"] // dots
     matchesGeneric, "L4E", ("OOOOROOOOYYYYWYYYYBBBRRRGGGBBBRORGGGBBBRRRGGGWWWWYWWWW", false, false, false), [mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2 + " " + mu2]] // dots

let l4eIntermediatePatterns = [
    // Last 4 edges -> Solved! - hand authored (~5 STM better than beginner)
    matchesGeneric, "L4E", ("OOOOOOOOOYYYYYYYYYBBBRRRGGGBBBRRRGGGBBBRRRGGGWWWWWWWWW", false, false, false), [] // skip (solved!)
    matchesGeneric, "L4E", ("OROOROOROYWYYWYYWYBBBRORGGGBBBRORGGGBBBRORGGGWYWWYWWYW", false, false, false), ["M2"]
    matchesGeneric, "L4E", ("OOOOROOROYWYYWYYYYBBBRRRGGGBBBRORGGGBBBRORGGGWWWWYWWYW", false, false, false), ["M' U2 M' U2"]
    matchesGeneric, "L4E", ("OROOROOOOYWYYWYYYYBBBRRRGGGBBBRORGGGBBBRORGGGWYWWYWWWW", false, false, false), ["U2 M U2 M"]
    matchesGeneric, "L4E", ("OROOOOOOOYYYYYYYWYBBBRORGGGBBBRRRGGGBBBRRRGGGWYWWWWWWW", false, false, false), ["M' U2 M' U2 M2"]
    matchesGeneric, "L4E", ("OOOOOOOROYYYYYYYWYBBBRORGGGBBBRRRGGGBBBRRRGGGWWWWWWWYW", false, false, false), ["U2 M U2 M'"]
    matchesGeneric, "L4E", ("OROOOOOOOYWYYYYYYYBBBRORGGGBBBRRRGGGBBBRRRGGGWWWWWWWYW", false, false, false), ["M U2 M' U2"]
    matchesGeneric, "L4E", ("OOOOOOOROYWYYYYYYYBBBRORGGGBBBRRRGGGBBBRRRGGGWYWWWWWWW", false, false, false), ["U2 M' U2 M"]
    matchesGeneric, "L4E", ("OOOOROOROYYYYWYYWYBBBRRRGGGBBBRORGGGBBBRORGGGWYWWYWWWW", false, false, false), ["M U2 M' U2 M2"]
    matchesGeneric, "L4E", ("OROOROOOOYYYYWYYWYBBBRRRGGGBBBRORGGGBBBRORGGGWWWWYWWYW", false, false, false), ["U2 M' U2 M'"]
    matchesGeneric, "L4E", ("OOOOOOOROYYYYYYYWYBBBRRRGGGBBBRRRGGGBBBRORGGGWYWWWWWWW", false, false, false), ["M' U2 M U2"]
    matchesGeneric, "L4E", ("OROOOOOOOYYYYYYYWYBBBRRRGGGBBBRRRGGGBBBRORGGGWWWWWWWYW", false, false, false), ["M2 U2 M U2 M"]
    matchesGeneric, "L4E", ("OROOROOOOYWYYWYYYYBBBRORGGGBBBRORGGGBBBRRRGGGWWWWYWWYW", false, false, false), ["M' U2 M U2 M2"]
    matchesGeneric, "L4E", ("OOOOROOROYWYYWYYYYBBBRORGGGBBBRORGGGBBBRRRGGGWYWWYWWWW", false, false, false), ["M2 U2 M U2 M'"]
    matchesGeneric, "L4E", ("OROOROOOOYYYYWYYWYBBBRORGGGBBBRORGGGBBBRRRGGGWYWWYWWWW", false, false, false), ["M U2 M U2"]
    matchesGeneric, "L4E", ("OOOOROOROYYYYWYYWYBBBRORGGGBBBRORGGGBBBRRRGGGWWWWYWWYW", false, false, false), ["M2 U2 M' U2 M"]
    matchesGeneric, "L4E", ("OOOOOOOROYWYYYYYYYBBBRRRGGGBBBRRRGGGBBBRORGGGWWWWWWWYW", false, false, false), ["M U2 M U2 M2"]
    matchesGeneric, "L4E", ("OROOOOOOOYWYYYYYYYBBBRRRGGGBBBRRRGGGBBBRORGGGWYWWWWWWW", false, false, false), ["M2 U2 M' U2 M'"]
    matchesGeneric, "L4E", ("OOOOROOOOYWYYWYYWYBBBRRRGGGBBBRORGGGBBBRRRGGGWYWWYWWYW", false, false, false), ["U2 M2 U2"] // horizontal bars
    matchesGeneric, "L4E", ("OROOOOOROYYYYYYYYYBBBRORGGGBBBRRRGGGBBBRORGGGWWWWWWWWW", false, false, false), ["U2 M2 U2 M2"] // horizontal bars
    matchesGeneric, "L4E", ("OROOROOROYYYYWYYYYBBBRORGGGBBBRORGGGBBBRORGGGWWWWYWWWW", false, false, false), ["M U2 M2 U2 M'"] // vertical bars
    matchesGeneric, "L4E", ("OOOOOOOOOYWYYYYYWYBBBRRRGGGBBBRRRGGGBBBRRRGGGWYWWWWWYW", false, false, false), ["M' U2 M2 U2 M'"] // vertical bars
    matchesGeneric, "L4E", ("OROOOOOROYWYYYYYWYBBBRORGGGBBBRRRGGGBBBRORGGGWYWWWWWYW", false, false, false), ["E2 M E2 M"] // dots
    matchesGeneric, "L4E", ("OOOOROOOOYYYYWYYYYBBBRRRGGGBBBRORGGGBBBRRRGGGWWWWYWWWW", false, false, false), ["E2 M E2 M'"]] // dots

let lseBeginnerPatterns = eolrBeginnerPatterns @ l4eBeginnerPatterns
let lseIntermediatePatterns = eolrIntermediatePatterns @ l4eIntermediatePatterns

let rouxBeginnerPatterns = fbBeginnerPatterns @ sbBeginnerPatterns @ cmllBeginnerPatterns @ lseBeginnerPatterns // 102 STM, 99 with ignored AUF
let rouxIntermediatePatterns = fbIntermediatePatterns @ sbIntermediatePatterns @ cmllIntermediatePatterns @ lseIntermediatePatterns // 97 STM with LSE, 84 with 1L CO, 81 with 1L CP, 77 with EO, 74 with ignored AUF
let rouxAdvancedPatterns = fbIntermediatePatterns @ sbIntermediatePatterns @ cmllAdvancedPatterns @ lseIntermediatePatterns // 65 STM with CMLL
let rouxGodPatterns = fbGodPatterns @ sbIntermediatePatterns @ cmllAdvancedPatterns @ lseIntermediatePatterns // 65 STM with CMLL

let solve =
    let patterns =
        match level with 
        | 0 -> rouxBeginnerPatterns
        | 1 -> rouxIntermediatePatterns
        | 2 -> rouxAdvancedPatterns
        | 3 -> rouxGodPatterns
        | _ -> failwith "Unknown level"
    patterns |> expandPatternsForAuf |> solveCase

let generate numCubes =
    let scrambled = initScrambledCubes numCubes
    //let scrambled = [
    //    ("R' U' F B L' U R2 B2 F2 D R2 F2 L2 R2 U F2 D' R' F' L2 F2 L' B' F' R' U' F"
    //    |> Render.stringToSteps |> Cube.executeSteps) Cube.solved ]

    // First Block (FB)
    let movesU = [Move Move.U; Move Move.U'; Move Move.U2; Move Move.UW; Move Move.UW'; Move Move.UW2]
    let movesD = [Move Move.D; Move Move.D'; Move Move.D2; Move Move.DW; Move Move.DW'; Move Move.DW2]
    let movesL = [Move Move.L; Move Move.L'; Move Move.L2; Move Move.LW; Move Move.LW'; Move Move.LW2]
    let movesR = [Move Move.R; Move Move.R'; Move Move.R2; Move Move.RW; Move Move.RW'; Move Move.RW2]
    let movesF = [Move Move.F; Move Move.F'; Move Move.F2; Move Move.FW; Move Move.FW'; Move Move.FW2]
    let movesB = [Move Move.B; Move Move.B'; Move Move.B2; Move Move.BW; Move Move.BW'; Move Move.BW2]
    let movesM = [Move Move.M; Move Move.M'; Move Move.M2]
    let movesS = [Move Move.S; Move Move.S'; Move Move.S2]
    let movesE = [Move Move.E; Move Move.E'; Move Move.E2]
    let moves = movesU @ movesD @ movesL @ movesR @ movesF @ movesB @ movesM @ movesS @ movesE
    let solvedLBPair =
        let caseLBPair = Solver.lookPattern "O..O.......................BB.......BB..........W..W.." // LB pair inserted
        if level < 3 then
            // DL
            let caseDL = Solver.lookPattern ".....................................B..........W....."
            let rotations = [Rotate X; Rotate X'; Rotate X2; Rotate Y; Rotate Y'; Rotate Y2; Rotate Z; Rotate Z'; Rotate Z2]
            let solvedDL = solve rotations "Solve DL edge (during inspection)" "DLEdge" caseDL scrambled false
            Solver.stageStats "Inspection" numCubes

            // center
            let caseLC = Solver.lookPattern "............................B........B..........W....."
            let solvedLC = solve moves "Solve L center" "LCenter" caseLC solvedDL false

            // LB pair
            if level = 0 then
                let caseLBtoFD = Solver.lookPattern "............................B........B..B.....O.W....." // OB edge in DF position
                let solvedLBtoFD = solve moves "Tuck LB to FD" "TuckLBtoFD" caseLBtoFD solvedLC false
                let caseDLBtoUFR = Solver.lookPattern ".................B.....OW............................."
                let caseDLBtoUBR = Solver.lookPattern "........B..O..............W..........................."
                let caseDLBtoU c = caseLBtoFD c && (caseDLBtoUBR c || (if level > 0 then caseDLBtoUFR c else false))
                let solvedDLBtoURB = solve moves "Bring DLB corner to URB" "BringDLBtoU" caseDLBtoU solvedLBtoFD false
                solve moves "Pair and insert LB pair" "InsertLBPair" caseLBPair solvedDLBtoURB false
            else // level > 0 : intermediate+ (solve LB pair directly)
                solve moves "Pair and insert LB pair" "InsertLBPair" caseLBPair solvedLC true
        else // god
            solve moves "Build LB square" "BuildLBSquare" caseLBPair scrambled true

    // LF pair
    let caseSolvedFB = Solver.lookPattern "O..O.......................BBBR.....BBBR.....W..W..W.."
    let solvedFB =
        if level = 0 then
            let caseLFtoBD = Solver.lookPattern "OB.O.......................BB.......BB..........W..WR."
            let solvedLFtoBD = solve moves "Tuck LF to BD" "TuckLFtoBD" caseLFtoBD solvedLBPair false
            let caseDLFtoURF = Solver.lookPattern "OB.O.............R.....BW..BB.......BB..........W..WR."
            let solvedDLFtoURF = solve moves "Bring DLF corner to URF" "BringDLFtoURF" caseDLFtoURF solvedLFtoBD false
            solve moves "Pair and insert LF pair (complete FB)" "InsertLFPair" caseSolvedFB solvedDLFtoURF true
        else
            solve moves "Pair and insert LF pair (complete FB)" "InsertLFPair" caseSolvedFB solvedLBPair true

    Solver.stageStats "FB" numCubes

    // Second Block (SB)

    // DR (center done)
    let caseDR = Solver.lookPattern "O..O.......................BBBR.....BBBR...G.W..W.WW.."
    let sbMoves = [Move Move.U; Move Move.U'; Move Move.U2
                   Move Move.R; Move Move.R'; Move Move.R2; Move Move.RW; Move Move.RW'; Move Move.RW2
                   Move Move.F; Move Move.F'
                   Move Move.B; Move Move.B'
                   Move Move.M; Move Move.M'; Move Move.M2]
    let solvedDR = solve sbMoves "Solve DR edge" "DREdge" caseDR solvedFB false

    // RB pair
    let caseRBPair = Solver.lookPattern "O.OO.O.....................BBBR....GBBBR...GGW..W.WW.W"
    let solvedRBPair =
        if level = 0 then
            let caseRBtoFD = Solver.lookPattern "O..O.......................BBBR.....BBBRG..G.WO.W.WW.."
            let solvedRBtoFD = solve sbMoves "Tuck RB to FD" "TuckRBtoFD" caseRBtoFD solvedDR false
            let caseDRBtoULB = Solver.lookPattern "O..O..G.....O.....W........BBBR.....BBBRG..G.WO.W.WW.."
            let solvedDRBtoULB = solve sbMoves "Bring DRB to ULB" "BringDRBtoULB" caseDRBtoULB solvedRBtoFD false
            solve sbMoves "Pair and insert RB pair" "InsertRBPair" caseRBPair solvedDRBtoULB false
        else
            solve sbMoves "Pair and insert RB pair" "InsertRBPair" caseRBPair solvedDR true

    // RF pair
    let mMoves = [Move Move.M; Move Move.M'; Move Move.M2]
    let caseCO = Solver.lookPattern "O.OO.O...Y.Y...Y.Y.........BBBR.RG.GBBBR.RGGGW.WW.WW.W"
    let caseCP c = caseCO c && (look Face.L Sticker.UL c = look Face.L Sticker.UR c) && (look Face.R Sticker.UL c = look Face.R Sticker.UR c) // check left/right "pairs"
    let caseCenterO c = caseCP c && (look Face.U Sticker.C c = Color.W || look Face.U Sticker.C c = Color.Y)

    let caseSolvedSB = Solver.lookPattern "O.OO.O.....................BBBR.RG.GBBBR.RGGGW.WW.WW.W" // F2B complete
    let solvedSB =
        if level = 0 then
            let caseRFtoBD = Solver.lookPattern "OGOO.O.....................BBBR....GBBBR...GGW..W.WWRW"
            let solvedRFtoBD = solve sbMoves "Tuck RF to BD" "TuckRFtoBD" caseRFtoBD solvedRBPair false
            let caseDRFtoULF = Solver.lookPattern "OGOO.O.........R....WG.....BBBR....GBBBR...GGW..W.WWRW"
            let solvedDRFtoULF = solve sbMoves "Bring DRF to ULF" "BringDRFtoULF" caseDRFtoULF solvedRFtoBD false
            solve sbMoves "Pair and insert RF pair (complete SB)" "InsertRFPair" caseSolvedSB solvedDRFtoULF true
        else
            let solvedRF = solve sbMoves "Pair and insert RF pair (complete SB)" "InsertRFPair" caseSolvedSB solvedRBPair true
            solve mMoves "Orient center" "CenterOrientation" caseCenterO solvedRF false

    Solver.stageStats "SB" numCubes

    // Orient corners (CO) - hand authored patterns
    let solvedCO = solve moves "Orient corners (CO)" "CornerOrientation" caseCO solvedSB false

    // Permute corners (CP) - hand authored patterns
    let rufMoves = [Move Move.U; Move Move.U'; Move Move.U2; Move Move.R; Move Move.R'; Move Move.R2; Move Move.F; Move Move.F'; Move Move.F2]
    let solvedCP = solve rufMoves "Permute corners (CP)" "CornerPermutation" caseCP solvedCO true

    Solver.stageStats "CMLL" numCubes

    // Orient center (note: generated patterns and algs are not distinct because goal is flexible U/D colors)
    let solvedCenterO =
        if level = 0
        then solve mMoves "Orient center" "CenterOrientation" caseCenterO solvedCP false
        else solvedCP // center already solved with SB

    // Orient edges (EO) (note: generated patterns and algs are not distinct because goal is flexible U/D colors)
    let muMoves = mMoves @ [Move Move.U; Move Move.U'; Move Move.U2]
    let caseEO c = caseCenterO c &&
                   (look Face.U Sticker.L c = Color.W || look Face.U Sticker.L c = Color.Y) &&
                   (look Face.U Sticker.U c = Color.W || look Face.U Sticker.U c = Color.Y) &&
                   (look Face.U Sticker.R c = Color.W || look Face.U Sticker.R c = Color.Y) &&
                   (look Face.U Sticker.D c = Color.W || look Face.U Sticker.D c = Color.Y) &&
                   (look Face.D Sticker.U c = Color.W || look Face.D Sticker.U c = Color.Y) &&
                   (look Face.D Sticker.D c = Color.W || look Face.D Sticker.D c = Color.Y)
    let solvedEO = solve muMoves "Orient edges (EO)" "EdgeOrientation" caseEO solvedCenterO true

    Solver.stageStats "EO" numCubes

    // Left/right edges (LR)

    let caseLRSolved c = caseEO c && look Face.L Sticker.U c = Color.B && look Face.R Sticker.U c = Color.G &&
                                     look Face.L Sticker.UL c = Color.B && look Face.R Sticker.UR c = Color.G &&
                                     look Face.L Sticker.UR c = Color.B && look Face.R Sticker.UL c = Color.G
    let solvedLR =
        if level = 0 then 
            // L edge to DF*)
            let caseLtoDF c = caseEO c && look Face.F Sticker.D c = Color.B
            let solvedLtoDF = solve muMoves "L edge to DF" "LToDF" caseLtoDF solvedEO false

            // LR edges to bottom
            let caseLRBottom c = caseLtoDF c && look Face.F Sticker.D c = Color.B && look Face.B Sticker.U c = Color.G
            let solvedLRBottom = solve muMoves "LR edges to bottom" "LREdgesBottom" caseLRBottom solvedLtoDF false

            // LR edges solved
            solve muMoves "LR edges solved" "LREdges" caseLRSolved solvedLRBottom true
        else
            // LR edges solved (directly from EO)
            solve muMoves "LR edges solved" "LREdges" caseLRSolved solvedEO true

    Solver.stageStats "LR" numCubes

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
    let solved = solve mud2Moves "Last 4 edges -> Solved!" "L4E" caseSolved solvedLR true

    Solver.stageStats "L4E" numCubes