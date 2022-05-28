module LSE

open Cube
open System

printfn "Generating Block Cases"

let mutable count = 0

let gen known cases predicate renderer move = seq {
    let newCases =
        cases
        |> Map.toSeq
        |> Seq.map (fun (_, (moves, cube)) -> (move :: moves), Cube.executeMove cube move) // execute move
        |> Seq.filter (fun (_, cube) -> predicate cube)
        |> Seq.map (fun (moves, cube) -> (renderer cube), (moves, cube)) // render case
        |> Seq.filter (fun (case, _) -> not (Map.containsKey case known)) // filter existing
    newCases |> Seq.iter (fun _ ->
        count <- count + 1
        if count % 1000 = 0 then printf ".")
    yield! newCases }

let yellowUpRedFront = Cube.executeRotation (Cube.executeRotation Cube.solved Rotate.X2) Rotate.Y
let init = [Render.cubeToString yellowUpRedFront, ([], yellowUpRedFront)]

let rec iter cases known predicate renderer =
    let rec genCases known cases =
        printfn "Count: %i" (Map.count cases)
        Cube.movesCommonRoux |> Seq.map (gen known cases predicate renderer) |> Seq.concat
    let initialCount = count
    let cases' = cases |> Map.ofSeq |> genCases (Map.ofSeq known) |> List.ofSeq
    if count > initialCount
    then iter cases' (known @ cases') predicate renderer
    else known

let reportCases name pieces cases =
    let renderMoveSet (m: string list) = String.Join("; ", List.map (sprintf "\"%s\"") m)
    let keyMaker cube = cube |> pieces |> Render.piecesToString cube
    cases
    |> List.groupBy (fun (c, (_, _)) -> c) // group by case
    |> List.map (fun (c, x) -> (c, List.map (fun (_, (m, s)) -> m |> List.rev |> Cube.inverseMoves |> Render.movesToString, s) x))
    |> List.map (fun (_, x) -> $"    \"{name}\", (\"{x |> List.head |> snd |> keyMaker}\", false, false, false), [{x |> List.map fst |> renderMoveSet}]")
    |> List.iter (printfn "%s")

let genPairInsertCases name colorC colorD colorFB predicate pieces =
    let renderPairPositions cube =
        let downEdge = Cube.findEdge colorC colorD cube
        let pairEdge = Cube.findEdge colorC colorFB cube
        let corner = Cube.findCorner colorC colorFB colorD cube
        $"{downEdge} {pairEdge} {corner}"
    iter init [] predicate renderPairPositions
    |> reportCases $"Insert{name}Pair" pieces

let solvedCDL cube =
    Cube.look Face.L Sticker.C cube = Color.B &&
    Cube.look Face.L Sticker.D cube = Color.B &&
    Cube.look Face.D Sticker.L cube = Color.W
let piecesCDLAndBL (cube: Cube) =
    let (c, _) = Cube.findCenter Color.B cube
    let (dl, _) = Cube.findEdge Color.B Color.W cube
    let (bl, _) = Cube.findEdge Color.B Color.O cube
    let (dlb, _) = Cube.findCorner Color.B Color.O Color.W cube
    [Center c; Edge dl; Edge bl; Corner dlb]
//genPairInsertCases "LB" Color.B Color.W Color.O solvedCDL piecesCDLAndBL

let solvedCDLAndBLPair cube =
    solvedCDL cube &&
    Cube.look Face.L Sticker.L cube = Color.B &&
    Cube.look Face.L Sticker.DL cube = Color.B &&
    Cube.look Face.D Sticker.DL cube = Color.W &&
    Cube.look Face.B Sticker.UL cube = Color.O &&
    Cube.look Face.B Sticker.L cube = Color.O
let piecesFB (cube: Cube) =
    let (fl, _) = Cube.findEdge Color.B Color.R cube
    let (dbf, _) = Cube.findCorner Color.B Color.R Color.W cube
    piecesCDLAndBL cube @ [Edge fl; Corner dbf]
//genPairInsertCases "LF" Color.B Color.W Color.R solvedCDLAndBLPair piecesFB

let solvedFB cube =
    solvedCDLAndBLPair cube &&
    Cube.look Face.L Sticker.R cube = Color.B && // FL pair
    Cube.look Face.L Sticker.DR cube = Color.B &&
    Cube.look Face.D Sticker.UL cube = Color.W &&
    Cube.look Face.F Sticker.L cube = Color.R &&
    Cube.look Face.F Sticker.DL cube = Color.R &&
    Cube.look Face.R Sticker.C cube = Color.G

let solvedFBAndCDR cube =
    solvedFB cube &&
    ((Cube.look Face.R Sticker.D cube = Color.G && // DR down
      Cube.look Face.D Sticker.R cube = Color.W) ||
     (Cube.look Face.R Sticker.R cube = Color.G && // DR back
      Cube.look Face.B Sticker.R cube = Color.W) ||
     (Cube.look Face.R Sticker.L cube = Color.G && // DR front
      Cube.look Face.F Sticker.R cube = Color.W))
let piecesFBAndCDRAndBR (cube: Cube) =
    let (c, _) = Cube.findCenter Color.G cube
    let (dr, _) = Cube.findEdge Color.G Color.W cube
    let (br, _) = Cube.findEdge Color.G Color.O cube
    let (dbr, _) = Cube.findCorner Color.G Color.O Color.W cube
    piecesFB cube @ [Center c; Edge dr; Edge br; Corner dbr]
//genPairInsertCases "BR" Color.G Color.W Color.O solvedFBAndCDR piecesFBAndCDRAndBR

let solvedFBAndCDRAndBRPair cube =
    solvedFB cube &&
    ((Cube.look Face.R Sticker.R cube = Color.G && // BR pair (down)
      Cube.look Face.R Sticker.DR cube = Color.G &&
      Cube.look Face.B Sticker.UR cube = Color.O &&
      Cube.look Face.B Sticker.R cube = Color.O &&
      Cube.look Face.D Sticker.DR cube = Color.W &&
      Cube.look Face.R Sticker.D cube = Color.G && // DR down
      Cube.look Face.D Sticker.R cube = Color.W) ||
     (Cube.look Face.R Sticker.U cube = Color.G && // BR pair (back)
      Cube.look Face.R Sticker.UR cube = Color.G &&
      Cube.look Face.U Sticker.UR cube = Color.O &&
      Cube.look Face.U Sticker.R cube = Color.O &&
      Cube.look Face.B Sticker.DR cube = Color.W &&
      Cube.look Face.R Sticker.R cube = Color.G && // DR back
      Cube.look Face.B Sticker.R cube = Color.W) ||
     (Cube.look Face.R Sticker.D cube = Color.G && // BR pair (front)
      Cube.look Face.R Sticker.DL cube = Color.G &&
      Cube.look Face.D Sticker.UR cube = Color.O &&
      Cube.look Face.D Sticker.R cube = Color.O &&
      Cube.look Face.F Sticker.DR cube = Color.W &&
      Cube.look Face.R Sticker.L cube = Color.G && // DR front
      Cube.look Face.F Sticker.R cube = Color.W))
let piecesFBAndCDRAndBRAndFR (cube: Cube) =
    let (fr, _) = Cube.findEdge Color.G Color.R cube
    let (dfr, _) = Cube.findCorner Color.G Color.R Color.W cube
    piecesFBAndCDRAndBR cube @ [Edge fr; Corner dfr]
genPairInsertCases "FR" Color.G Color.W Color.R solvedFBAndCDRAndBRPair piecesFBAndCDRAndBRAndFR