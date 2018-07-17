open System
open Cube
open Solver
open Render

let numCubes = 1000
let down, left, front = Color.W, Color.B, Color.R // TODO: color neutral (and defined by DL edge)

let patterns = [
    // Solving DL edge (during inspection)
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
    "LCenter", "............................B.....G..B..........W.....", [] // skip
    "LCenter", "....B..........................G.....B..........W.....", ["E"; "u'"]
    "LCenter", "....G..........................B.....B..........W.....", ["u"; "E'"]
    "LCenter", "............................G.....B..B..........W.....", ["u2"; "E2"]
    "LCenter", ".............G.......................B..........WB....", ["r u"; "r E'"; "r' u'"; "r' E"; "M u'"; "M E"; "M' u"; "M' E'"]
    "LCenter", ".............B.......................B..........WG....", ["r u'"; "r E"; "r' u"; "r' E'"; "M u"; "M E'"; "M' u'"; "M' E"];
    ]

let hybridSolve includeRotations includeMoves includeWideMoves includeSliceMoves depth goal stage cube =
    let matches (c: string) (p: string) = Seq.forall2 (fun p c -> p = '.' || p = c) p c
    match Seq.tryFind (fun (s, p, _) -> s = stage && matches (cubeToString cube) p) patterns with
    | Some (_, _, algs) ->
        match algs with
        | a :: _ -> [a.Split(' ') |> Seq.map stringToStep] |> Seq.ofList
        | [] -> Seq.empty // skip
    | None -> solve includeRotations includeMoves includeWideMoves includeSliceMoves depth goal cube

let genCasesAndSolutions includeRotations includeMoves includeWideMoves includeSliceMoves depth cubes goal stage =
    printf "."
    let rec gen cases = function
        | cube :: remaining ->
            let solutions = hybridSolve includeRotations includeMoves includeWideMoves includeSliceMoves depth goal stage cube
            let algs = String.Join(" | ", Seq.map stepsToString solutions)
            // printfn "Algs: %s" algs
            let skip = Seq.length solutions = 0
            let key = if skip then "skip" else solutions |> Seq.map stepsToString |> Seq.sort |> Seq.head
            // printfn "Key: %s" key
            match Map.tryFind key cases with
            | Some case ->
                printf "!"
                gen (Map.add key ((cube, solutions, if skip then cube else executeSteps (Seq.head solutions) cube) :: case) cases) remaining
            | None ->
                printfn ""
                printfn "New case: %s (%s)" algs (cubeToString cube)
                gen (Map.add key [cube, solutions, if skip then cube else executeSteps (Seq.head solutions) cube] cases) remaining
        | _ -> cases
    gen Map.empty cubes

let distinctCases solutions =
    let common (cubes: string list) =
        let commonNth n =
            let distinct = cubes |> Seq.map (fun c -> c.[n]) |> Seq.distinct
            if Seq.length distinct = 1 then Seq.head distinct else '.'
        String.Concat(Seq.init (9 * 6) commonNth)
    let cases = solutions |> Map.toList |> List.map snd |> List.map (List.map (fun (c, _, _) -> cubeToString c)) |> List.map common
    let algs = solutions |> Map.toList |> List.map fst
    List.zip cases algs |> Seq.iter (fun (c, a) -> c |> stringToCube |> render; printfn "Algs: %s" a; printfn "Cube: %s" c)

let genRoux () =
    printfn "Scrambling %i cubes" numCubes
    let scrambled = List.init numCubes (fun _ -> printf "."; scramble 20 |> fst)
    printfn ""

    printfn "Solving DL edge (during inspection)"
    let caseDLD c = look Face.D Sticker.L c = Color.W && look Face.L Sticker.D c = Color.B
    let solutions = genCasesAndSolutions true false false false 10 scrambled caseDLD "DLEdge"
    let solvedDL = solutions |> Map.toList |> List.map snd |> List.concat |> List.map (fun (_, _, c) -> c)
    distinctCases solutions

    printfn "Solving L center"
    let caseLC c = caseDLD c && look Face.L Sticker.C c = Color.B
    let solutions = genCasesAndSolutions false true true true 10 solvedDL caseLC "LCenter"
    let solvedLC = solutions |> Map.toList |> List.map snd |> List.concat |> List.map (fun (_, _, c) -> c)
    distinctCases solutions

    // THIS TAKES TOO LONG!
    // printfn "Pairing R/B edge and corner in upper layer"
    // let caseBRPaired c = caseLC c && (look Face.U Sticker.D c = Color.R && look Face.U Sticker.DR c = Color.R && look Face.R Sticker.UL c = Color.W && look Face.F Sticker.U c = Color.B && look Face.F Sticker.UR c = Color.B)
    // let solutions = genCasesAndSolutions false true true true 10 solvedLC caseBRPaired "R/BEdgeAndCorner"
    // let solvedBRPaired = solutions |> Map.toList |> List.map snd |> List.concat |> List.map (fun (_, _, c) -> c)
    // distinctCases solutions

    (*
    printfn "Placing FL edge in DF (R facing)"
    let caseFLtoDFR c = caseLC c && (look Face.D Sticker.U c = Color.B && look Face.F Sticker.D c = Color.R)
    let solutions = genCasesAndSolutions false true true true 10 solvedLC caseFLtoDFR "FLEdgeInDF-R"
    let solvedFTtoDFR = solutions |> Map.toList |> List.map snd |> List.concat |> List.map (fun (_, _, c) -> c)
    distinctCases solutions

    printfn "Placing FL edge in DF (B facing)"
    let caseFLtoDFB c = caseLC c && (look Face.D Sticker.U c = Color.R && look Face.F Sticker.D c = Color.B)
    let solutions = genCasesAndSolutions false true true true 10 solvedLC caseFLtoDFB "FLEdgeInDF-B"
    let solvedFTtoDFB = solutions |> Map.toList |> List.map snd |> List.concat |> List.map (fun (_, _, c) -> c)
    distinctCases solutions

    printfn "Placing FL edge in DB (R facing)"
    let caseFLtoDBR c = caseLC c && (look Face.D Sticker.D c = Color.B && look Face.B Sticker.U c = Color.R)
    let solutions = genCasesAndSolutions false true true true 10 solvedLC caseFLtoDBR "FLEdgeInDB-R"
    let solvedFTtoDBR = solutions |> Map.toList |> List.map snd |> List.concat |> List.map (fun (_, _, c) -> c)
    distinctCases solutions

    printfn "Placing FL edge in DB (B facing)"
    let caseFLtoDBB c = caseLC c && (look Face.D Sticker.D c = Color.R && look Face.B Sticker.U c = Color.B)
    let solutions = genCasesAndSolutions false true true true 10 solvedLC caseFLtoDBB "FLEdgeInDB-B"
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
    // let solutions = genCasesAndSolutions false true true true 10 solvedFTtoDF caseDLFTopWithRBUp "DLFToTopWithR/BUp"
    // let solvedDLFTOpWithRBUp = solutions |> Map.toList |> List.map snd |> List.concat |> List.map (fun (_, _, c) -> c)
    // distinctCases solutions

    pause ()
// genRoux ()

let genPaired () =
    printfn "Scrambling %i cubes" numCubes
    let scrambled = List.init numCubes (fun _ -> printf "."; scramble 20 |> fst)
    printfn ""

    printfn "Solving DL edge (during inspection)"
    let caseDLD c = look Face.D Sticker.L c = Color.W && look Face.L Sticker.D c = Color.B
    let solutions = genCasesAndSolutions true false false false 10 scrambled caseDLD "DLEdge"
    let solvedDL = solutions |> Map.toList |> List.map snd |> List.concat |> List.map (fun (_, _, c) -> c)
    distinctCases solutions

    printfn "Solving L center"
    let caseLC c = caseDLD c && look Face.L Sticker.C c = Color.B
    let solutions = genCasesAndSolutions false true true true 10 solvedDL caseLC "LCenter"
    let solvedLC = solutions |> Map.toList |> List.map snd |> List.concat |> List.map (fun (_, _, c) -> c)
    distinctCases solutions

    // SLOW! try full slotting!
    printfn "Full slotting BL"
    let caseFullBL c = caseLC c && (look Face.B Sticker.L c = Color.O && look Face.B Sticker.UL c = Color.O && look Face.L Sticker.L c = Color.B && look Face.L Sticker.DL c = Color.B && look Face.D Sticker.DL c = Color.W)
    let solutions = genCasesAndSolutions false true true true 20 solvedLC caseFullBL "SlotBL"
    let solvedFullBL = solutions |> Map.toList |> List.map snd |> List.concat |> List.map (fun (_, _, c) -> c)
    distinctCases solutions

    // printfn "Pairing BWR"
    // let casePairedBWR c = (look Face.F Sticker.DL c = Color.R && look Face.L Sticker.D c = Color.B && look Face.L Sticker.DR c = Color.B && look Face.D Sticker.UL c = Color.W && look Face.D Sticker.L c = Color.W)
    // let solutions = genCasesAndSolutions false true true true 10 solvedDL casePairedBWR "PairBWR"
    // let solvedPairedBWR = solutions |> Map.toList |> List.map snd |> List.concat |> List.map (fun (_, _, c) -> c)
    // distinctCases solutions
// genPaired ()

let genRouxL4E () =
    printfn "Scrambling %i cubes" numCubes
    let scrambled = List.init numCubes (fun _ -> printf "."; scrambleRouxL4E 20 |> fst)
    printfn ""

    printfn "Solving L4E"
    let caseSolved c = c = solved
    let solutions = genCasesAndSolutions false true true true 10 scrambled caseSolved "L4ESolved"
    distinctCases solutions
genRouxL4E ()

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
    let solutions = solve true false false false 3 checkDLEdge c |> List.ofSeq
    printfn "Number of solutions (DL edge): %i" (Seq.length solutions)
    pause ()
    for s in solutions do
        let c = executeSteps s c
        render c
        printfn "Solution (DL edge): %s" (stepsToString s)
        let sols = solve false true true true 3 checkLC c |> List.ofSeq
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