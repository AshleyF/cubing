module Pairing

open Cube
open Solver

let ufEdge1stPatterns = [
    // Solving UF edge (during inspection) [24 cases]
    "UFEdge1st", ("................G.....R...............................", false, false, false), [] // skip
    "UFEdge1st", ("........................................G.....R.......", false, false, false), ["x"]
    "UFEdge1st", ("..............G..........R............................", false, false, false), ["y"]
    "UFEdge1st", (".............................GR.......................", false, false, false), ["z"]
    "UFEdge1st", (".......G..R...........................................", false, false, false), ["x'"]
    "UFEdge1st", ("............G......R..................................", false, false, false), ["y'"]
    "UFEdge1st", ("................................RG....................", false, false, false), ["z'"]
    "UFEdge1st", (".R..................................................G.", false, false, false), ["x2"]
    "UFEdge1st", (".......R..G...........................................", false, false, false), ["y2"]
    "UFEdge1st", ("........................................R.....G.......", false, false, false), ["z2"]
    "UFEdge1st", ("...........................................G......R...", false, false, false), ["x z'"; "y x"; "z' y"]
    "UFEdge1st", (".....G.............................R..................", false, false, false), ["x' y"; "y z'"; "z' x'"]
    "UFEdge1st", (".............................RG.......................", false, false, false), ["x y'"; "y' z'"; "z' x"]
    "UFEdge1st", ("..............R..........G............................", false, false, false), ["x' z'"; "y' x'"; "z' y'"]
    "UFEdge1st", ("............R......G..................................", false, false, false), ["x' z"; "y x'"; "z y"]
    "UFEdge1st", (".....................................G..........R.....", false, false, false), ["x z"; "y' x"; "z y'"]
    "UFEdge1st", ("...G.......................R..........................", false, false, false), ["x' y'"; "y' z"; "z x'"]
    "UFEdge1st", ("................................GR....................", false, false, false), ["x y"; "y z"; "z x"]
    "UFEdge1st", ("................R.....G...............................", false, false, false), ["x y2"; "x' z2"; "y2 x'"; "z2 x"]
    "UFEdge1st", (".G..................................................R.", false, false, false), ["x z2"; "x' y2"; "y2 x"; "z2 x'"]
    "UFEdge1st", ("...R.......................G..........................", false, false, false), ["x2 z"; "y2 z'"; "z y2"; "z' x2"]
    "UFEdge1st", (".....R.............................G..................", false, false, false), ["x2 z'"; "y2 z"; "z x2"; "z' y2"]
    "UFEdge1st", ("...........................................R......G...", false, false, false), ["x2 y"; "y z2"; "y' x2"; "z2 y'"]
    "UFEdge1st", (".....................................R..........G.....", false, false, false), ["x2 y'"; "y x2"; "y' z2"; "z2 y"]]

let ufrCorner1stPatterns = [
    // Solving UFR corner (during inspection) [24 cases]
    "UFRCorner1st", (".................G.....RW.............................", false, false, false), [] // skip
    "UFRCorner1st", (".........................................GW....R......", false, false, false), ["x"]
    "UFRCorner1st", ("........W..G..............R...........................", false, false, false), ["y"]
    "UFRCorner1st", ("...............W....GR................................", false, false, false), ["z"]
    "UFRCorner1st", ("........G..R..............W...........................", false, false, false), ["x'"]
    "UFRCorner1st", ("...............G....RW................................", false, false, false), ["y'"]
    "UFRCorner1st", (".........................................RG....W......", false, false, false), ["z'"]
    "UFRCorner1st", ("..R.........................................W........G", false, false, false), ["x2"]
    "UFRCorner1st", ("......R..G........W...................................", false, false, false), ["y2"]
    "UFRCorner1st", ("......................................WR.....G........", false, false, false), ["z2"]
    "UFRCorner1st", (".................W.....GR.............................", false, false, false), ["x y"; "y z"; "z x"]
    "UFRCorner1st", ("..G.........................................R........W", false, false, false), ["x' y"; "y z'"; "z' x'"]
    "UFRCorner1st", ("......................................GW.....R........", false, false, false), ["x z"; "y' x"; "z y'"]
    "UFRCorner1st", ("R...................................G..............W..", false, false, false), ["x2 z"; "y2 z'"; "z y2"; "z' x2"]
    "UFRCorner1st", (".................R.....WG.............................", false, false, false), ["x' z'"; "y' x'"; "z' y'"]
    "UFRCorner1st", ("G...................................W..............R..", false, false, false), ["x z2"; "x' y2"; "y2 x"; "z2 x'"]
    "UFRCorner1st", ("........R..W..............G...........................", false, false, false), ["x2 z'"; "y2 z"; "z x2"; "z' y2"]
    "UFRCorner1st", (".........................................WR....G......", false, false, false), ["x2 y"; "y z2"; "y' x2"; "z2 y'"]
    "UFRCorner1st", ("......W..R........G...................................", false, false, false), ["x' z"; "y x'"; "z y"]
    "UFRCorner1st", ("......................................RG.....W........", false, false, false), ["x y'"; "y' z'"; "z' x"]
    "UFRCorner1st", ("......G..W........R...................................", false, false, false), ["x' y'"; "y' z"; "z x'"]
    "UFRCorner1st", ("..W.........................................G........R", false, false, false), ["x z'"; "y x"; "z' y"]
    "UFRCorner1st", ("W...................................R..............G..", false, false, false), ["x2 y'"; "y x2"; "y' z2"; "z2 y"]
    "UFRCorner1st", ("...............R....WG................................", false, false, false), ["x y2"; "x' z2"; "y2 x'"; "z2 x"]]

let ufEdge2ndPatterns = [
    "UFEdge2nd", ("................GG....RRW.............................", false, false, false), [] // skip
    "UFEdge2nd", (".....G...........G.....RW..........R..................", false, false, false), ["B l"; "B M"; "b l"; "b M"]
    "UFEdge2nd", ("...G.............G.....RW..R..........................", false, false, false), ["B' l"; "B' M"; "b' l"; "b' M"]
    "UFEdge2nd", (".................G.....RW....GR.......................", false, false, false), ["d' B' l"; "d' B' M"; "d' b' l"; "d' b' M"; "d2 B' l2"; "d2 B' M2"; "d2 b' l2"; "d2 b' M2"; "L D l'"; "L D M'"; "L d l'"; "L d M'"; "L2 B l2"; "L2 B M2"; "L2 b l2"; "L2 b M2"; "l D l'"; "l D M'"; "l d l'"; "l d M'"; "l2 B l2"; "l2 B M2"; "l2 b l2"; "l2 b M2"; "R F R'"; "R f R'"; "R2 F R2"; "R2 f R2"; "r F R'"; "r f R'"; "r2 F R2"; "r2 f R2"; "E' B' l"; "E' B' M"; "E' b' l"; "E' b' M"; "E2 B' l2"; "E2 B' M2"; "E2 b' l2"; "E2 b' M2"]
    "UFEdge2nd", (".................G.....RW..................R......G...", false, false, false), ["D l2"; "D M2"; "d l2"; "d M2"]
    "UFEdge2nd", ("............R....G.G...RW.............................", false, false, false), ["L' B l2"; "L' B M2"; "L' b l2"; "L' b M2"; "L2 D l'"; "L2 D M'"; "L2 d l'"; "L2 d M'"; "l' B l2"; "l' B M2"; "l' b l2"; "l' b M2"; "l2 D l'"; "l2 D M'"; "l2 d l'"; "l2 d M'"; "b D' l2"; "b D' M2"; "b d' l2"; "b d' M2"; "b2 D' l'"; "b2 D' M'"; "b2 d' l'"; "b2 d' M'"; "S' D' l2"; "S' D' M2"; "S' d' l2"; "S' d' M2"; "S2 D' l'"; "S2 D' M'"; "S2 d' l'"; "S2 d' M'"]
    "UFEdge2nd", ("...R.............G.....RW..G..........................", false, false, false), ["B l2"; "B M2"; "b l2"; "b M2"]
    "UFEdge2nd", (".................G.....RW...............R.....G.......", false, false, false), ["D2 l2"; "D2 M2"; "d2 l2"; "d2 M2"]
    "UFEdge2nd", (".................G.....RW.......GR....................", false, false, false), ["d B' l2"; "d B' M2"; "d b' l2"; "d b' M2"; "d2 B' l"; "d2 B' M"; "d2 b' l"; "d2 b' M"; "E B' l2"; "E B' M2"; "E b' l2"; "E b' M2"; "E2 B' l"; "E2 B' M"; "E2 b' l"; "E2 b' M"]
    "UFEdge2nd", (".................G.....RW............G..........R.....", false, false, false), ["D l'"; "D M'"; "d l'"; "d M'"]
    "UFEdge2nd", ("................RG....GRW.............................", false, false, false), ["l D2 l2"; "l D2 M2"; "l d2 l2"; "l d2 M2"; "l' B2 l2"; "l' B2 M2"; "l' b2 l2"; "l' b2 M2"; "l2 D2 l'"; "l2 D2 M'"; "l2 d2 l'"; "l2 d2 M'"; "l2 B2 l"; "l2 B2 M"; "l2 b2 l"; "l2 b2 M"; "M D2 l2"; "M D2 M2"; "M d2 l2"; "M d2 M2"; "M' B2 l2"; "M' B2 M2"; "M' b2 l2"; "M' b2 M2"; "M2 D2 l'"; "M2 D2 M'"; "M2 d2 l'"; "M2 d2 M'"; "M2 B2 l"; "M2 B2 M"; "M2 b2 l"; "M2 b2 M"]
    "UFEdge2nd", (".....R...........G.....RW..........G..................", false, false, false), ["B' l2"; "B' M2"; "b' l2"; "b' M2"]
    "UFEdge2nd", (".................G.....RW...............G.....R.......", false, false, false), ["l'"; "M'"]
    "UFEdge2nd", (".G...............G.....RW...........................R.", false, false, false), ["D2 l'"; "D2 M'"; "d2 l'"; "d2 M'"; "B2 l"; "B2 M"; "b2 l"; "b2 M"]
    "UFEdge2nd", ("..............G..G.....RWR............................", false, false, false), ["b' D' l'"; "b' D' M'"; "b' d' l'"; "b' d' M'"; "b2 D' l2"; "b2 D' M2"; "b2 d' l2"; "b2 d' M2"; "S D' l'"; "S D' M'"; "S d' l'"; "S d' M'"; "S2 D' l2"; "S2 D' M2"; "S2 d' l2"; "S2 d' M2"]
    "UFEdge2nd", (".................G.....RW.......RG....................", false, false, false), ["d B l"; "d B M"; "d b l"; "d b M"; "d2 B l2"; "d2 B M2"; "d2 b l2"; "d2 b M2"; "E B l"; "E B M"; "E b l"; "E b M"; "E2 B l2"; "E2 B M2"; "E2 b l2"; "E2 b M2"]
    "UFEdge2nd", (".R...............G.....RW...........................G.", false, false, false), ["l2"; "M2"]
    "UFEdge2nd", (".......R..G......G.....RW.............................", false, false, false), ["B2 l2"; "B2 M2"; "b2 l2"; "b2 M2"]
    "UFEdge2nd", (".................G.....RW............R..........G.....", false, false, false), ["D' l2"; "D' M2"; "d' l2"; "d' M2"]
    "UFEdge2nd", ("..............R..G.....RWG............................", false, false, false), ["b' D l2"; "b' D M2"; "b' d l2"; "b' d M2"; "b2 D l'"; "b2 D M'"; "b2 d l'"; "b2 d M'"; "S D l2"; "S D M2"; "S d l2"; "S d M2"; "S2 D l'"; "S2 D M'"; "S2 d l'"; "S2 d M'"]
    "UFEdge2nd", (".................G.....RW..................G......R...", false, false, false), ["D' l'"; "D' M'"; "d' l'"; "d' M'"]
    "UFEdge2nd", (".......G..R......G.....RW.............................", false, false, false), ["l"; "M"]
    "UFEdge2nd", (".................G.....RW....RG.......................", false, false, false), ["d' B l2"; "d' B M2"; "d' b l2"; "d' b M2"; "d2 B l"; "d2 B M"; "d2 b l"; "d2 b M"; "L D' l2"; "L D' M2"; "L d' l2"; "L d' M2"; "L2 B' l"; "L2 B' M"; "L2 b' l"; "L2 b' M"; "l D' l2"; "l D' M2"; "l d' l2"; "l d' M2"; "l2 B' l"; "l2 B' M"; "l2 b' l"; "l2 b' M"; "E' B l2"; "E' B M2"; "E' b l2"; "E' b M2"; "E2 B l"; "E2 B M"; "E2 b l"; "E2 b M"]
    "UFEdge2nd", ("............G....G.R...RW.............................", false, false, false), ["L' B' l"; "L' B' M"; "L' b' l"; "L' b' M"; "L2 D' l2"; "L2 D' M2"; "L2 d' l2"; "L2 d' M2"; "l' B' l"; "l' B' M"; "l' b' l"; "l' b' M"; "l2 D' l2"; "l2 D' M2"; "l2 d' l2"; "l2 d' M2"; "R' U' R"; "R' u' R"; "R2 U' R2"; "R2 u' R2"; "r' U' R"; "r' u' R"; "r2 U' R2"; "r2 u' R2"; "b D l'"; "b D M'"; "b d l'"; "b d M'"; "b2 D l2"; "b2 D M2"; "b2 d l2"; "b2 d M2"; "S' D l'"; "S' D M'"; "S' d l'"; "S' d M'"; "S2 D l2"; "S2 D M2"; "S2 d l2"; "S2 d M2"]]

let ufrCorner2ndPatterns = [
    "URFCorner2nd", ("................GG....RRW.............................", false, false, false), [] // skip
    "URFCorner2nd", ("................G.....R..................GW....R......", false, false, false), ["R"]
    "URFCorner2nd", ("........G..R....G.....R...W...........................", false, false, false), ["R'"]
    "URFCorner2nd", ("..R.............G.....R.....................W........G", false, false, false), ["R2"]
    "URFCorner2nd", ("..G.............G.....R.....................R........W", false, false, false), ["B R'"; "b R'"]
    "URFCorner2nd", ("......G..W......G.R...R...............................", false, false, false), ["B' R'"; "b' R'"]
    "URFCorner2nd", ("................G.....R...............GW.....R........", false, false, false), ["D R"; "d R"]
    "URFCorner2nd", ("..W.............G.....R.....................G........R", false, false, false), ["D' R"; "d' R"]
    "URFCorner2nd", ("R...............G.....R.............G..............W..", false, false, false), ["B R2"; "b R2"]
    "URFCorner2nd", ("......R..G......G.W...R...............................", false, false, false), ["B2 R2"; "b2 R2"]
    "URFCorner2nd", ("........R..W....G.....R...G...........................", false, false, false), ["B' R2"; "b' R2"]
    "URFCorner2nd", ("................G.....R...............WR.....G........", false, false, false), ["D2 R2"; "d2 R2"]
    "URFCorner2nd", ("................G.....R..................WR....G......", false, false, false), ["D R2"; "d R2"]
    "URFCorner2nd", ("W...............G.....R.............R..............G..", false, false, false), ["D' R2"; "d' R2"]
    "URFCorner2nd", ("................G.....R..................RG....W......", false, false, false), ["D B R'"; "D b R'"; "D2 B R2"; "D2 b R2"; "d B R'"; "d b R'"; "d2 B R2"; "d2 b R2"; "l' F' l"; "l' F' M"; "l' f' l"; "l' f' M"; "l2 F' l2"; "l2 F' M2"; "l2 f' l2"; "l2 f' M2"; "R' D' R"; "R' d' R"; "R2 B' R2"; "R2 b' R2"; "M' F' l"; "M' F' M"; "M' f' l"; "M' f' M"; "M2 F' l2"; "M2 F' M2"; "M2 f' l2"; "M2 f' M2"]
    "URFCorner2nd", ("...............GG...RWR...............................", false, false, false), ["L' B' R'"; "L' b' R'"; "L2 D' R2"; "L2 d' R2"; "r' U' l'"; "r' U' M'"; "r' u' l'"; "r' u' M'"; "r2 U' l2"; "r2 U' M2"; "r2 u' l2"; "r2 u' M2"; "M U' l'"; "M U' M'"; "M u' l'"; "M u' M'"; "M2 U' l2"; "M2 U' M2"; "M2 u' l2"; "M2 u' M2"]
    "URFCorner2nd", ("........W..G....G.....R...R...........................", false, false, false), ["l U l'"; "l U M'"; "l u l'"; "l u M'"; "l2 U l2"; "l2 U M2"; "l2 u l2"; "l2 u M2"; "R B R'"; "R b R'"; "R2 D R2"; "R2 d R2"; "B' D' R"; "B' d' R"; "B2 D' R2"; "B2 d' R2"; "b' D' R"; "b' d' R"; "b2 D' R2"; "b2 d' R2"; "M U l'"; "M U M'"; "M u l'"; "M u M'"; "M2 U l2"; "M2 U M2"; "M2 u l2"; "M2 u M2"]
    "URFCorner2nd", ("...............WG...GRR...............................", false, false, false), ["L D R"; "L d R"; "L2 B R2"; "L2 b R2"; "r F l"; "r F M"; "r f l"; "r f M"; "r2 F l2"; "r2 F M2"; "r2 f l2"; "r2 f M2"; "M' F l"; "M' F M"; "M' f l"; "M' f M"; "M2 F l2"; "M2 F M2"; "M2 f l2"; "M2 f M2"]
    "URFCorner2nd", ("................G.....R...............RG.....W........", false, false, false), ["D' B R2"; "D' b R2"; "D2 B R'"; "D2 b R'"; "d' B R2"; "d' b R2"; "d2 B R'"; "d2 b R'"; "L D' R2"; "L d' R2"; "L2 B' R'"; "L2 b' R'"]
    "URFCorner2nd", ("................GW....RGR.............................", false, false, false), ["R' D R2"; "R' d R2"; "R2 B R'"; "R2 b R'"]
    "URFCorner2nd", ("................GR....RWG.............................", false, false, false), ["R B' R2"; "R b' R2"; "R2 D' R"; "R2 d' R"]
    "URFCorner2nd", ("......W..R......G.G...R...............................", false, false, false), ["L' B R2"; "L' b R2"; "L2 D R"; "L2 d R"; "B D' R2"; "B d' R2"; "B2 D' R"; "B2 d' R"; "b D' R2"; "b d' R2"; "b2 D' R"; "b2 d' R"]
    "URFCorner2nd", ("G...............G.....R.............W..............R..", false, false, false), ["D2 R"; "d2 R"; "B2 R'"; "b2 R'"]
    "URFCorner2nd", ("...............RG...WGR...............................", false, false, false), ["L D2 R2"; "L d2 R2"; "L' B2 R2"; "L' b2 R2"; "L2 D2 R"; "L2 d2 R"; "L2 B2 R'"; "L2 b2 R'"]]

let patterns = ufEdge1stPatterns @ ufEdge2ndPatterns @ ufrCorner1stPatterns @ ufrCorner2ndPatterns

let solve = patterns |> expandPatternsForAuf |> solveCase

let rotations = [Rotate X; Rotate X'; Rotate X2; Rotate Y; Rotate Y'; Rotate Y2; Rotate Z; Rotate Z'; Rotate Z2]
let moves = [Move Move.U; Move Move.U'; Move Move.U2; Move Move.UW; Move Move.UW'; Move Move.UW2
             Move Move.D; Move Move.D'; Move Move.D2; Move Move.DW; Move Move.DW'; Move Move.DW2
             Move Move.L; Move Move.L'; Move Move.L2; Move Move.LW; Move Move.LW'; Move Move.LW2
             Move Move.R; Move Move.R'; Move Move.R2; Move Move.RW; Move Move.RW'; Move Move.RW2
             Move Move.F; Move Move.F'; Move Move.F2; Move Move.FW; Move Move.FW'; Move Move.FW2
             Move Move.B; Move Move.B'; Move Move.B2; Move Move.BW; Move Move.BW'; Move Move.BW2
             Move Move.M; Move Move.M'; Move Move.M2;
             Move Move.S; Move Move.S'; Move Move.S2;
             Move Move.E; Move Move.E'; Move Move.E2]

let generateEdgeFirst numCubes =
    let scrambled = initScrambledCubes numCubes

    // UF
    let caseUF c = look Face.U Sticker.D c = Color.G && look Face.F Sticker.U c = Color.R
    let solvedUF = solve rotations "Solve UF edge (rotations only)" "UFEdge1st" caseUF scrambled false

    // DLF
    let caseUFR c = caseUF c && look Face.U Sticker.DR c = Color.G && look Face.F Sticker.UR c = Color.R && look Face.R Sticker.UL c = Color.W
    let solvedUFR = solve moves "Pair UFR corner with UF edge" "URFCorner2nd" caseUFR solvedUF true

    Solver.stageStats "Pair" numCubes

let generateCornerFirst numCubes =
    let scrambled = initScrambledCubes numCubes

    // DLF
    let caseUFR c = look Face.U Sticker.DR c = Color.G && look Face.F Sticker.UR c = Color.R && look Face.R Sticker.UL c = Color.W
    let solvedUFR = solve rotations "Solve UFR corner (rotations only)" "UFRCorner1st" caseUFR scrambled false

    // UF
    let caseUF c = caseUFR c && look Face.U Sticker.D c = Color.G && look Face.F Sticker.U c = Color.R
    let solvedUF = solve moves "Pair UF edge with UFR corner" "UFEdge2nd" caseUF solvedUFR true

    Solver.stageStats "Pair" numCubes

let generate = generateCornerFirst
