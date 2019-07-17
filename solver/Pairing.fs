module Pairing

open Cube
open Solver
open Render

let ufEdgePatterns = [
    // Solving UF edge (during inspection) [24 cases]
    "UFEdge", ("................G.....R...............................", false, false, false), [] // skip
    "UFEdge", ("............G......R..................................", false, false, false), ["y'"]
    "UFEdge", (".......G..R...........................................", false, false, false), ["x'"]
    "UFEdge", (".............................GR.......................", false, false, false), ["z"]
    "UFEdge", ("........................................G.....R.......", false, false, false), ["x"]
    "UFEdge", ("..............G..........R............................", false, false, false), ["y"]
    "UFEdge", ("................................RG....................", false, false, false), ["z'"]
    "UFEdge", ("........................................R.....G.......", false, false, false), ["z2"]
    "UFEdge", (".......R..G...........................................", false, false, false), ["y2"]
    "UFEdge", (".R..................................................G.", false, false, false), ["x2"]
    "UFEdge", ("...........................................G......R...", false, false, false), ["x z'"; "y x"; "z' y"]
    "UFEdge", (".....G.............................R..................", false, false, false), ["x' y"; "y z'"; "z' x'"]
    "UFEdge", (".............................RG.......................", false, false, false), ["x y'"; "y' z'"; "z' x"]
    "UFEdge", ("..............R..........G............................", false, false, false), ["x' z'"; "y' x'"; "z' y'"]
    "UFEdge", ("............R......G..................................", false, false, false), ["x' z"; "y x'"; "z y"]
    "UFEdge", (".....................................G..........R.....", false, false, false), ["x z"; "y' x"; "z y'"]
    "UFEdge", ("...G.......................R..........................", false, false, false), ["x' y'"; "y' z"; "z x'"]
    "UFEdge", ("................................GR....................", false, false, false), ["x y"; "y z"; "z x"]
    "UFEdge", ("................R.....G...............................", false, false, false), ["x y2"; "x' z2"; "y2 x'"; "z2 x"]
    "UFEdge", (".G..................................................R.", false, false, false), ["x z2"; "x' y2"; "y2 x"; "z2 x'"]
    "UFEdge", ("...R.......................G..........................", false, false, false), ["x2 z"; "y2 z'"; "z y2"; "z' x2"]
    "UFEdge", (".....R.............................G..................", false, false, false), ["x2 z'"; "y2 z"; "z x2"; "z' y2"]
    "UFEdge", ("...........................................R......G...", false, false, false), ["x2 y"; "y z2"; "y' x2"; "z2 y'"]
    "UFEdge", (".....................................R..........G.....", false, false, false), ["x2 y'"; "y x2"; "y' z2"; "z2 y"]]

let ufrCornerPatterns = [
    "UFRCorner", ("................GG....RRW.............................", false, false, false), [] // skip
    "UFRCorner", ("................G.....R..................GW....R......", false, false, false), ["R"]
    "UFRCorner", ("........G..R....G.....R...W...........................", false, false, false), ["R'"]
    "UFRCorner", ("..R.............G.....R.....................W........G", false, false, false), ["R2"]
    "UFRCorner", ("..G.............G.....R.....................R........W", false, false, false), ["B R'"; "b R'"]
    "UFRCorner", ("......G..W......G.R...R...............................", false, false, false), ["B' R'"; "b' R'"]
    "UFRCorner", ("................G.....R...............GW.....R........", false, false, false), ["D R"; "d R"]
    "UFRCorner", ("..W.............G.....R.....................G........R", false, false, false), ["D' R"; "d' R"]
    "UFRCorner", ("R...............G.....R.............G..............W..", false, false, false), ["B R2"; "b R2"]
    "UFRCorner", ("......R..G......G.W...R...............................", false, false, false), ["B2 R2"; "b2 R2"]
    "UFRCorner", ("........R..W....G.....R...G...........................", false, false, false), ["B' R2"; "b' R2"]
    "UFRCorner", ("................G.....R...............WR.....G........", false, false, false), ["D2 R2"; "d2 R2"]
    "UFRCorner", ("................G.....R..................WR....G......", false, false, false), ["D R2"; "d R2"]
    "UFRCorner", ("W...............G.....R.............R..............G..", false, false, false), ["D' R2"; "d' R2"]
    "UFRCorner", ("................G.....R..................RG....W......", false, false, false), ["D B R'"; "D b R'"; "D2 B R2"; "D2 b R2"; "d B R'"; "d b R'"; "d2 B R2"; "d2 b R2"; "l' F' l"; "l' F' M"; "l' f' l"; "l' f' M"; "l2 F' l2"; "l2 F' M2"; "l2 f' l2"; "l2 f' M2"; "R' D' R"; "R' d' R"; "R2 B' R2"; "R2 b' R2"; "M' F' l"; "M' F' M"; "M' f' l"; "M' f' M"; "M2 F' l2"; "M2 F' M2"; "M2 f' l2"; "M2 f' M2"]
    "UFRCorner", ("...............GG...RWR...............................", false, false, false), ["L' B' R'"; "L' b' R'"; "L2 D' R2"; "L2 d' R2"; "r' U' l'"; "r' U' M'"; "r' u' l'"; "r' u' M'"; "r2 U' l2"; "r2 U' M2"; "r2 u' l2"; "r2 u' M2"; "M U' l'"; "M U' M'"; "M u' l'"; "M u' M'"; "M2 U' l2"; "M2 U' M2"; "M2 u' l2"; "M2 u' M2"]
    "UFRCorner", ("........W..G....G.....R...R...........................", false, false, false), ["l U l'"; "l U M'"; "l u l'"; "l u M'"; "l2 U l2"; "l2 U M2"; "l2 u l2"; "l2 u M2"; "R B R'"; "R b R'"; "R2 D R2"; "R2 d R2"; "B' D' R"; "B' d' R"; "B2 D' R2"; "B2 d' R2"; "b' D' R"; "b' d' R"; "b2 D' R2"; "b2 d' R2"; "M U l'"; "M U M'"; "M u l'"; "M u M'"; "M2 U l2"; "M2 U M2"; "M2 u l2"; "M2 u M2"]
    "UFRCorner", ("...............WG...GRR...............................", false, false, false), ["L D R"; "L d R"; "L2 B R2"; "L2 b R2"; "r F l"; "r F M"; "r f l"; "r f M"; "r2 F l2"; "r2 F M2"; "r2 f l2"; "r2 f M2"; "M' F l"; "M' F M"; "M' f l"; "M' f M"; "M2 F l2"; "M2 F M2"; "M2 f l2"; "M2 f M2"]
    "UFRCorner", ("................G.....R...............RG.....W........", false, false, false), ["D' B R2"; "D' b R2"; "D2 B R'"; "D2 b R'"; "d' B R2"; "d' b R2"; "d2 B R'"; "d2 b R'"; "L D' R2"; "L d' R2"; "L2 B' R'"; "L2 b' R'"]
    "UFRCorner", ("................GW....RGR.............................", false, false, false), ["R' D R2"; "R' d R2"; "R2 B R'"; "R2 b R'"]
    "UFRCorner", ("................GR....RWG.............................", false, false, false), ["R B' R2"; "R b' R2"; "R2 D' R"; "R2 d' R"]
    "UFRCorner", ("......W..R......G.G...R...............................", false, false, false), ["L' B R2"; "L' b R2"; "L2 D R"; "L2 d R"; "B D' R2"; "B d' R2"; "B2 D' R"; "B2 d' R"; "b D' R2"; "b d' R2"; "b2 D' R"; "b2 d' R"]
    "UFRCorner", ("G...............G.....R.............W..............R..", false, false, false), ["D2 R"; "d2 R"; "B2 R'"; "b2 R'"]
    "UFRCorner", ("...............RG...WGR...............................", false, false, false), ["L D2 R2"; "L d2 R2"; "L' B2 R2"; "L' b2 R2"; "L2 D2 R"; "L2 d2 R"; "L2 B2 R'"; "L2 b2 R'"]]

let patterns = ufEdgePatterns @ ufrCornerPatterns

let solve = patterns |> expandPatternsForAuf |> solveCase

let generate numCubes =
    printfn "Scrambling %i cubes" numCubes
    let scrambled = List.init numCubes (fun _ -> printf "."; scramble 20 |> fst)
    printfn ""

    // UF
    let caseUF c = look Face.U Sticker.D c = Color.G && look Face.F Sticker.U c = Color.R
    let rotations = [Rotate X; Rotate X'; Rotate X2; Rotate Y; Rotate Y'; Rotate Y2; Rotate Z; Rotate Z'; Rotate Z2]
    let solvedUF = solve rotations "Solve UF edge (rotations only)" "UFEdge" caseUF scrambled false

    // DLF
    let caseUFR c = caseUF c && look Face.U Sticker.DR c = Color.G && look Face.F Sticker.UR c = Color.R && look Face.R Sticker.UL c = Color.W
    let moves = [Move Move.U; Move Move.U'; Move Move.U2; Move Move.UW; Move Move.UW'; Move Move.UW2
                 Move Move.D; Move Move.D'; Move Move.D2; Move Move.DW; Move Move.DW'; Move Move.DW2
                 Move Move.L; Move Move.L'; Move Move.L2; Move Move.LW; Move Move.LW'; Move Move.LW2
                 Move Move.R; Move Move.R'; Move Move.R2; Move Move.RW; Move Move.RW'; Move Move.RW2
                 Move Move.F; Move Move.F'; Move Move.F2; Move Move.FW; Move Move.FW'; Move Move.FW2
                 Move Move.B; Move Move.B'; Move Move.B2; Move Move.BW; Move Move.BW'; Move Move.BW2
                 Move Move.M; Move Move.M'; Move Move.M2;
                 Move Move.S; Move Move.S'; Move Move.S2;
                 Move Move.E; Move Move.E'; Move Move.E2]
    let solvedUFR = solve moves "Pair UFR corner with UF edge" "UFRCorner" caseUFR solvedUF true

    Solver.stageStats "Pair" numCubes

    let avgTwistCount = float Cube.twistCount / float numCubes;
    printfn "Total Average Twists (STM): %f" avgTwistCount

    pause ()