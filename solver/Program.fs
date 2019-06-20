open System
open Cube
open Solver
open Render

let numCubes = 1000

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
    // Bring DLB corner to URB
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
    // Insert LB pair
    // "InsertLBPair", "O..O.......................BB.....G.BB..........W..W..", [] // skip TODO: this should be hierarchical (before TuckLBtoFD & BringDLBtoURB)
    "InsertLBPair", "........B..O..............W.B.....G..B..B.....O.W.....", ["R M B'"; "R2 r' B'"; "r M2 B'"; "r' R2 B'"; "M R B'"; "M2 r B'"]
    ]



let genRoux () =
    printfn "Scrambling %i cubes" numCubes
    let scrambled = List.init numCubes (fun _ -> printf "."; scramble 20 |> fst)
    printfn ""

    printfn "Solve DL edge (during inspection)"
    let caseDLD c = look Face.D Sticker.L c = Color.W && look Face.L Sticker.D c = Color.B
    let solutions = genCasesAndSolutions patterns true false false scrambled caseDLD "DLEdge"
    let solvedDL = solutions |> Map.toList |> List.map snd |> List.concat |> List.map (fun (_, _, c) -> c)
    distinctCases solutions

    printfn "Solve L center"
    let caseLC c = caseDLD c && look Face.L Sticker.C c = Color.B
    let solutions = genCasesAndSolutions patterns false true true solvedDL caseLC "LCenter"
    let solvedLC = solutions |> Map.toList |> List.map snd |> List.concat |> List.map (fun (_, _, c) -> c)
    distinctCases solutions

    printfn "Tuck LB to FD"
    let caseLBtoFD c = caseLC c && look Face.F Sticker.D c = Color.B && look Face.D Sticker.U c = Color.O
    let solutions = genCasesAndSolutions patterns false true true solvedLC caseLBtoFD "TuckLBtoFD"
    let solvedLBtoFD = solutions |> Map.toList |> List.map snd |> List.concat |> List.map (fun (_, _, c) -> c)
    distinctCases solutions

    printfn "Bring DLB corner to URB"
    let caseDLBtoURB c = caseLBtoFD c && look Face.U Sticker.UR c = Color.O && look Face.R Sticker.UR c = Color.W && look Face.B Sticker.DR c = Color.B
    let solutions = genCasesAndSolutions patterns false true true solvedLBtoFD caseDLBtoURB "BringDLBtoURB"
    let solvedDLBtoURB = solutions |> Map.toList |> List.map snd |> List.concat |> List.map (fun (_, _, c) -> c)
    distinctCases solutions

    printfn "Pair and insert LB pair"
    let caseInsertLBPair c = caseLC c && look Face.L Sticker.L c = Color.B && look Face.L Sticker.DL c = Color.B && look Face.B Sticker.UL c = Color.O && look Face.B Sticker.L c = Color.O && look Face.D Sticker.DL c = Color.W
    let solutions = genCasesAndSolutions patterns false true true solvedDLBtoURB caseInsertLBPair "InsertLBPair"
    distinctCases solutions

    // THIS TAKES TOO LONG!
    // printfn "Pairing R/B edge and corner in upper layer"
    // let caseBRPaired c = caseLC c && (look Face.U Sticker.D c = Color.R && look Face.U Sticker.DR c = Color.R && look Face.R Sticker.UL c = Color.W && look Face.F Sticker.U c = Color.B && look Face.F Sticker.UR c = Color.B)
    // let solutions = genCasesAndSolutions patterns false true true solvedLC caseBRPaired "R/BEdgeAndCorner"
    // let solvedBRPaired = solutions |> Map.toList |> List.map snd |> List.concat |> List.map (fun (_, _, c) -> c)
    // distinctCases solutions

    (*
    printfn "Placing FL edge in DF (R facing)"
    let caseFLtoDFR c = caseLC c && (look Face.D Sticker.U c = Color.B && look Face.F Sticker.D c = Color.R)
    let solutions = genCasesAndSolutions patterns false true true solvedLC caseFLtoDFR "FLEdgeInDF-R"
    let solvedFTtoDFR = solutions |> Map.toList |> List.map snd |> List.concat |> List.map (fun (_, _, c) -> c)
    distinctCases solutions

    printfn "Placing FL edge in DF (B facing)"
    let caseFLtoDFB c = caseLC c && (look Face.D Sticker.U c = Color.R && look Face.F Sticker.D c = Color.B)
    let solutions = genCasesAndSolutions patterns false true true solvedLC caseFLtoDFB "FLEdgeInDF-B"
    let solvedFTtoDFB = solutions |> Map.toList |> List.map snd |> List.concat |> List.map (fun (_, _, c) -> c)
    distinctCases solutions

    printfn "Placing FL edge in DB (R facing)"
    let caseFLtoDBR c = caseLC c && (look Face.D Sticker.D c = Color.B && look Face.B Sticker.U c = Color.R)
    let solutions = genCasesAndSolutions patterns false true true solvedLC caseFLtoDBR "FLEdgeInDB-R"
    let solvedFTtoDBR = solutions |> Map.toList |> List.map snd |> List.concat |> List.map (fun (_, _, c) -> c)
    distinctCases solutions

    printfn "Placing FL edge in DB (B facing)"
    let caseFLtoDBB c = caseLC c && (look Face.D Sticker.D c = Color.R && look Face.B Sticker.U c = Color.B)
    let solutions = genCasesAndSolutions patterns false true true solvedLC caseFLtoDBB "FLEdgeInDB-B"
    let solvedFTtoDBR = solutions |> Map.toList |> List.map snd |> List.concat |> List.map (fun (_, _, c) -> c)
    distinctCases solutions
    *)

    // printfn "Moving DLF corner to top with R/B up"
    // let caseNoWUp c0 c1 c2 = (c0 = Color.B && c1 = Color.R && c2 = Color.W) || (c0 = Color.R && c1 = Color.W && c2 = Color.B)
    // let caseDLFtoULF c = caseNoWUp (look Face.U Sticker.DL c) (look Face.F Sticker.UL c) (look Face.L Sticker.UR c)
    // let caseDLFtoURF c = caseNoWUp (look Face.U Sticker.DR c) (look Face.R Sticker.UL c) (look Face.F Sticker.UR c)
    // let caseDLFtoULB c = caseNoWUp (look Face.U Sticker.UL c) (look Face.L Sticker.UL c) (look Face.B Sticker.DL c)
    // let caseDLFtoURB c = caseNoWUp (look Face.U Sticker.UR c) (look Face.B Sticker.DR c) (look Face.R Sticker.UR c)
    // let caseDLFTopWithRBUp c = caseFLtoDF c && (caseDLFtoULF c || caseDLFtoURF c || caseDLFtoULB c ||caseDLFtoURB c)
    // let solutions = genCasesAndSolutions patterns false true true solvedFTtoDF caseDLFTopWithRBUp "DLFToTopWithR/BUp"
    // let solvedDLFTOpWithRBUp = solutions |> Map.toList |> List.map snd |> List.concat |> List.map (fun (_, _, c) -> c)
    // distinctCases solutions

    pause ()
genRoux ()

(*
let genPaired () =
    printfn "Scrambling %i cubes" numCubes
    let scrambled = List.init numCubes (fun _ -> printf "."; scramble 20 |> fst)
    printfn ""

    printfn "Solving DL edge (during inspection)"
    let caseDLD c = look Face.D Sticker.L c = Color.W && look Face.L Sticker.D c = Color.B
    let solutions = genCasesAndSolutions patterns true false false scrambled caseDLD "DLEdge"
    let solvedDL = solutions |> Map.toList |> List.map snd |> List.concat |> List.map (fun (_, _, c) -> c)
    distinctCases solutions

    printfn "Solving L center"
    let caseLC c = caseDLD c && look Face.L Sticker.C c = Color.B
    let solutions = genCasesAndSolutions patterns false true true solvedDL caseLC "LCenter"
    let solvedLC = solutions |> Map.toList |> List.map snd |> List.concat |> List.map (fun (_, _, c) -> c)
    distinctCases solutions

    // SLOW! try full slotting!
    printfn "Full slotting BL"
    let caseFullBL c = caseLC c && (look Face.B Sticker.L c = Color.O && look Face.B Sticker.UL c = Color.O && look Face.L Sticker.L c = Color.B && look Face.L Sticker.DL c = Color.B && look Face.D Sticker.DL c = Color.W)
    let solutions = genCasesAndSolutions patterns false true true 20 solvedLC caseFullBL "SlotBL"
    let solvedFullBL = solutions |> Map.toList |> List.map snd |> List.concat |> List.map (fun (_, _, c) -> c)
    distinctCases solutions

    // printfn "Pairing BWR"
    // let casePairedBWR c = (look Face.F Sticker.DL c = Color.R && look Face.L Sticker.D c = Color.B && look Face.L Sticker.DR c = Color.B && look Face.D Sticker.UL c = Color.W && look Face.D Sticker.L c = Color.W)
    // let solutions = genCasesAndSolutions patterns false true true solvedDL casePairedBWR "PairBWR"
    // let solvedPairedBWR = solutions |> Map.toList |> List.map snd |> List.concat |> List.map (fun (_, _, c) -> c)
    // distinctCases solutions
genPaired ()
*)

(*
let genRouxL4E () =
    printfn "Scrambling %i cubes" numCubes
    let scrambled = List.init numCubes (fun _ -> printf "."; scrambleRouxL4E 20 |> fst) |> List.map (fun c -> if look Face.R Sticker.U c = Color.B then moveU2 c else c)
    printfn ""

    printfn "Solving L4E"
    let caseSolved c = c = solved
    let solutions = genCasesAndSolutions patterns false true true scrambled caseSolved "L4ESolved"
    distinctCases solutions
genRouxL4E ()
*)

(*
Console.BackgroundColor <- ConsoleColor.Black
Console.ForegroundColor <- ConsoleColor.Gray
Console.Clear()

let hilitePieces () =
    for c in [Center.U; D; L; R; F; B] do
        let stickers = centerToFaceSticker c
        renderWithHighlights [stickers] solved
        printfn "Center: %A" c
        pause ()
    for e in [UL; UR; UF; UB; DL; DR; DF; DB; FL; FR; BL; BR] do
        let stickers = edgeToFaceStickers e
        renderWithHighlights stickers solved
        printfn "Edge: %A" e
        pause ()
    for c in [ULF; ULB; URF; URB; DLF; DLB; DRF; DRB] do
        let stickers = cornerToFaceStickers c
        renderWithHighlights stickers solved
        printfn "Corner: %A" c
        pause ()

let testSolver () =
    let checkDLEdge cube = look Face.L Sticker.D cube = Color.B && look Face.D Sticker.L cube = Color.W
    let checkLC cube = checkDLEdge cube && look Face.L Sticker.C cube = Color.B
    let c, s = scramble 20
    render c
    printfn "Scramble: %s                  " (movesToString s)
    let solutions = solve true false false 3 checkDLEdge c |> List.ofSeq
    printfn "Number of solutions (DL edge): %i" (Seq.length solutions)
    pause ()
    for s in solutions do
        let c = executeSteps s c
        render c
        printfn "Solution (DL edge): %s" (stepsToString s)
        let sols = solve false true true 3 checkLC c |> List.ofSeq
        printfn "Number of solutions (LC): %i" (Seq.length solutions)
        pause ()
        for s in sols do
            let c = executeSteps s c
            render c
            printfn "Solution (LC): %s" (stepsToString s)
            pause ()

// while true do testSolver ()

render solved
pause ()

let test = solved |> moveM' |> moveD' |> moveU' |> moveL2 |> moveLW
render test
pause ()

renderWithHighlights [Face.U, Sticker.C; Face.U, Sticker.L; Face.U, Sticker.R] test
pause ()
*)

(*

TODO:
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
*)

printfn "DONE!"
Console.ReadLine() |> ignore

printfn "DONE!!"
Console.ReadLine() |> ignore

printfn "DONE!!!"
Console.ReadLine() |> ignore