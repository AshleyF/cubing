open System
open Cube
open Solver
open Render

let down, left, front = Color.W, Color.B, Color.R // TODO: color neutral (and defined by DL edge)

let genDLCases colorD colorL =
    let gen case =
        let rec gen' () =
            let cube, steps = scramble 20
            if case cube then cube, steps else gen' () // TODO: max iterations ("case never happens")
        gen' ()
    let genSolutions includeRotations includeMoves includeWideMoves includeSliceMoves depth caseIn caseOut name =
        let c, s = gen caseIn
        s |> movesToString |> printfn "%s: %s" name
        let solutions = solve includeRotations includeMoves includeWideMoves includeSliceMoves depth caseOut c
        for s in solutions do s |> stepsToString |> printfn "  Solution: %s"
    let caseULD c = look Face.U Sticker.L c = colorD && look Face.L Sticker.U c = colorL
    let caseULL c = look Face.U Sticker.L c = colorL && look Face.L Sticker.U c = colorD
    let caseURD c = look Face.U Sticker.R c = colorD && look Face.R Sticker.U c = colorL
    let caseURL c = look Face.U Sticker.R c = colorL && look Face.R Sticker.U c = colorD
    let caseUFD c = look Face.U Sticker.D c = colorD && look Face.F Sticker.U c = colorL
    let caseUFL c = look Face.U Sticker.D c = colorL && look Face.F Sticker.U c = colorD
    let caseUBD c = look Face.U Sticker.U c = colorD && look Face.B Sticker.D c = colorL
    let caseUBL c = look Face.U Sticker.U c = colorL && look Face.B Sticker.D c = colorD
    let caseDLD c = look Face.D Sticker.L c = colorD && look Face.L Sticker.D c = colorL
    let caseDLL c = look Face.D Sticker.L c = colorL && look Face.L Sticker.D c = colorD
    let caseDRD c = look Face.D Sticker.R c = colorD && look Face.R Sticker.D c = colorL
    let caseDRL c = look Face.D Sticker.R c = colorL && look Face.R Sticker.D c = colorD
    let caseDFD c = look Face.D Sticker.U c = colorD && look Face.F Sticker.D c = colorL
    let caseDFL c = look Face.D Sticker.U c = colorL && look Face.F Sticker.D c = colorD
    let caseDBD c = look Face.D Sticker.D c = colorD && look Face.B Sticker.U c = colorL
    let caseDBL c = look Face.D Sticker.D c = colorL && look Face.B Sticker.U c = colorD
    let caseFLD c = look Face.F Sticker.L c = colorD && look Face.L Sticker.R c = colorL
    let caseFLL c = look Face.F Sticker.L c = colorL && look Face.L Sticker.R c = colorD
    let caseFRD c = look Face.F Sticker.R c = colorD && look Face.R Sticker.L c = colorL
    let caseFRL c = look Face.F Sticker.R c = colorL && look Face.R Sticker.L c = colorD
    let caseBLD c = look Face.B Sticker.L c = colorD && look Face.L Sticker.L c = colorL
    let caseBLL c = look Face.B Sticker.L c = colorL && look Face.L Sticker.L c = colorD
    let caseBRD c = look Face.B Sticker.R c = colorD && look Face.R Sticker.R c = colorL
    let caseBRL c = look Face.B Sticker.R c = colorL && look Face.R Sticker.R c = colorD
    genSolutions true false false false 10 caseULD caseDLD "UL-D"
    genSolutions true false false false 10 caseULL caseDLD "UL-L"
    genSolutions true false false false 10 caseURD caseDLD "UR-D"
    genSolutions true false false false 10 caseURL caseDLD "UR-L"
    genSolutions true false false false 10 caseUFD caseDLD "UF-D"
    genSolutions true false false false 10 caseUFL caseDLD "UF-L"
    genSolutions true false false false 10 caseUBD caseDLD "UB-D"
    genSolutions true false false false 10 caseUBL caseDLD "UB-L"
    genSolutions true false false false 10 caseDLD caseDLD "DL-D"
    genSolutions true false false false 10 caseDLL caseDLD "DL-L"
    genSolutions true false false false 10 caseDRD caseDLD "DR-D"
    genSolutions true false false false 10 caseDRL caseDLD "DR-L"
    genSolutions true false false false 10 caseDFD caseDLD "DF-D"
    genSolutions true false false false 10 caseDFL caseDLD "DF-L"
    genSolutions true false false false 10 caseDBD caseDLD "DB-D"
    genSolutions true false false false 10 caseDBL caseDLD "DB-L"
    genSolutions true false false false 10 caseFLD caseDLD "FL-D"
    genSolutions true false false false 10 caseFLL caseDLD "FL-L"
    genSolutions true false false false 10 caseFRD caseDLD "FR-D"
    genSolutions true false false false 10 caseFRL caseDLD "FR-L"
    genSolutions true false false false 10 caseBLD caseDLD "BL-D"
    genSolutions true false false false 10 caseBLL caseDLD "BL-L"
    genSolutions true false false false 10 caseBRD caseDLD "BR-D"
    genSolutions true false false false 10 caseBRL caseDLD "BR-L"

genDLCases down left
pause ()

let roux cube =
    let rotateDLEdgeIntoPosition colorD colorL cube =
        let e, (c0, c1) = findEdge colorD colorL cube
        renderWithHighlights (edgeToFaceStickers e) cube
        printfn "Found %s/%s edge (%s/%s)" (colorToString colorL) (colorToString colorD) (colorToString c0) (colorToString c1)
        pause ()
        let cube' =
            let task = "Rotate it to DL position"
            match e, c0, c1 with
            | UL, u, _ when u = colorD -> execute [Rotate X2] task cube
            | UL, u, _ when u = colorL -> execute [Rotate Z'] task cube
            | UR, u, _ when u = colorD -> execute [Rotate Z2] task cube
            | UR, u, _ when u = colorL -> execute [Rotate X2; Rotate Z] task cube
            | UF, u, _ when u = colorD -> execute [Rotate Z2; Rotate Y] task cube
            | UF, u, _ when u = colorL -> execute [Rotate X'; Rotate Y] task cube
            | UB, u, _ when u = colorD -> execute [Rotate X2; Rotate Y] task cube
            | UB, u, _ when u = colorL -> execute [Rotate X; Rotate Y'] task cube
            | DL, d, _ when d = colorD -> execute [] "Already in place!" cube
            | DL, d, _ when d = colorL -> execute [Rotate X2; Rotate Z'] task cube
            | DR, d, _ when d = colorD -> execute [Rotate Y2] task cube
            | DR, d, _ when d = colorL -> execute [Rotate Z] task cube
            | DF, d, _ when d = colorD -> execute [Rotate Y] task cube
            | DF, d, _ when d = colorL -> execute [Rotate X2; Rotate Y'; Rotate Z'] task cube
            | DB, d, _ when d = colorD -> execute [Rotate Y'] task cube
            | DB, d, _ when d = colorL -> execute [Rotate X; Rotate Y] task cube
            | FL, f, _ when f = colorD -> execute [Rotate X'] task cube
            | FL, f, _ when f = colorL -> execute [Rotate Z'; Rotate Y] task cube
            | FR, f, _ when f = colorD -> execute [Rotate X'; Rotate Y2] task cube
            | FR, f, _ when f = colorL -> execute [Rotate Z; Rotate Y] task cube
            | BL, b, _ when b = colorD -> execute [Rotate X] task cube
            | BL, b, _ when b = colorL -> execute [Rotate X'; Rotate Z'] task cube
            | BR, b, _ when b = colorD -> execute [Rotate X; Rotate Y2] task cube
            | BR, b, _ when b = colorL -> execute [Rotate X; Rotate Z] task cube
            | _ -> failwith "Unexpected edge position"
        cube'
    let moveCenterToLeftSide color cube =
        let c = findCenter color cube
        renderWithHighlights [centerToFaceSticker c] cube
        printfn "Found %s center" (colorToString color)
        pause ()
        let cube' =
            let task = "Rotate it to left side"
            match c with
            | Center.U -> execute [Move RW'; Move UW] task cube
            | Center.D -> execute [Move RW; Move UW] task cube
            | Center.L -> execute [] "Already in place!" cube
            | Center.R -> execute [Move UW2] task cube // or Y2
            | Center.F -> execute [Move UW] task cube
            | Center.B -> execute [Move UW'] task cube
        cube'
    let solveFLPair colorF colorL colorD cube =
        let moveFLEdgeToDF cube =
            let e, (c0, c1) = findEdge colorF colorL cube // TODO: No need to search DL
            renderWithHighlights (edgeToFaceStickers e) cube
            printfn "Found %s/%s edge" (colorToString c0) (colorToString c1)
            let task = "Move it to DF position"
            match e, c0, c1 with // NOTE: not worrying about orientation
            | UL, _, _ -> execute [Move U'; Move RW'] task cube // RW' easier M
            | UR, _, _ -> execute [Move Move.U; Move RW'] task cube
            | UF, _, _ -> execute [Move RW'] task cube
            | UB, _, _ -> execute [Move RW2] task cube
            | DL, _, _ -> failwith "Unexpected edge position (DL)"
            | DR, _, _ -> execute [Move L'; Move D'; Move Move.L] task cube // tricky: maybe better than R2 U M
            | DF, _, _ -> execute [] "Already in place!" cube
            | DB, _, _ -> execute [Move RW] task cube // or L D L'
            | FL, _, _ -> execute [Move F'] task cube
            | FR, _, _ -> execute [Move Move.F] task cube
            | BL, _, _ -> execute [Move Move.B; Move RW] task cube
            | BR, _, _ -> execute [Move Move.B'; Move RW] task cube
            | _ -> failwith "Unexpected edge position"
        let cube = moveFLEdgeToDF cube
        cube
    let cube = rotateDLEdgeIntoPosition down left cube
    let cube = moveCenterToLeftSide left cube
    let cube = solveFLPair front left down cube
    cube

Console.BackgroundColor <- ConsoleColor.Black
Console.ForegroundColor <- ConsoleColor.Gray
Console.Clear()

let hiligePieces () =
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

let testRoux () =
    let c, s = scramble 20
    render c
    printfn "Scramble: %s                  " (movesToString s)
    pause ()
    let c = roux c
    if look Face.L Sticker.C c <> Color.B then failwith "Not solved CL"
    if look Face.L Sticker.D c <> Color.B then failwith "Not solved DL"
    if look Face.D Sticker.L c <> Color.W then failwith "Not solved DL"
    let du = look Face.D Sticker.U c
    let fd = look Face.F Sticker.D c
    if not ((du = Color.R && fd = Color.B) || (du = Color.B && fd = Color.R)) then failwith "Not solved DF"
    ignore

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

while true do testSolver ()

while true do testRoux () |> ignore

render solved
pause ()

let test = solved |> moveM' |> moveD' |> moveU' |> moveL2 |> moveLW
render test
pause ()

renderWithHighlights [Face.U, Sticker.C; Face.U, Sticker.L; Face.U, Sticker.R] test
pause ()

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

Long term:
- Video analysis
- Program synthesis for block-building steps
*)