#r "../../library/bin/Release/net8.0/Library.dll"

open System
open System.IO
open Cube

let yellowUpRedFront = executeRotation (executeRotation solved Rotate.X2) Rotate.Y

let enumerate moves predicate renderer =
    let known = Collections.Generic.Dictionary<string, Move list * Cube>()
    let queue = Collections.Generic.Queue<Move list * Cube>()
    let initial = [], yellowUpRedFront
    known.Add(renderer yellowUpRedFront, initial)
    queue.Enqueue(initial)
    while queue.Count > 0 do
        let path, cube = queue.Dequeue()
        for move in moves do
            let next = executeMove cube move
            if predicate next then
                let key = renderer next
                if known.TryAdd(key, (move :: path, next)) then queue.Enqueue(move :: path, next)
    known.Values |> Seq.toList

let generate file colors traversalPredicate casePredicate pieces allowedMoves =
    let centerColor, downColor, pairColor = colors
    let stateKey cube = Render.piecesToString cube (pieces cube)
    let cases = enumerate allowedMoves traversalPredicate stateKey
    let lines =
        cases
        |> List.filter (snd >> casePredicate)
        |> List.map (fun (path, cube) ->
            let pattern = Render.piecesToString cube (pieces cube)
            let solution = path |> List.rev |> inverseMoves |> Render.movesToString
            pattern, solution)
        |> List.groupBy fst
        |> List.map (fun (pattern, values) ->
            let algorithms = values |> List.map snd |> List.distinct |> List.sortBy (fun value -> value.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length)
            $"{pattern},{String.Join(';', algorithms)}")
        |> List.sort
    File.WriteAllLines(file, lines)
    printfn "%s: %i exhaustive cases" (Path.GetFileNameWithoutExtension file) lines.Length

let solvedCDL cube =
    look Face.L Sticker.C cube = Color.B && look Face.L Sticker.D cube = Color.B && look Face.D Sticker.L cube = Color.W
let solvedLF cube =
    solvedCDL cube && look Face.L Sticker.R cube = Color.B && look Face.L Sticker.DR cube = Color.B &&
    look Face.D Sticker.UL cube = Color.W && look Face.F Sticker.L cube = Color.R && look Face.F Sticker.DL cube = Color.R
let solvedLB cube =
    solvedCDL cube && look Face.L Sticker.L cube = Color.B && look Face.L Sticker.DL cube = Color.B &&
    look Face.D Sticker.DL cube = Color.W && look Face.B Sticker.UL cube = Color.O && look Face.B Sticker.L cube = Color.O
let solvedFB cube = solvedLF cube && solvedLB cube
let solvedDR cube = solvedFB cube && look Face.R Sticker.C cube = Color.G && look Face.R Sticker.D cube = Color.G && look Face.D Sticker.R cube = Color.W
let solvedCDR cube =
    solvedFB cube &&
    ((look Face.R Sticker.D cube = Color.G && look Face.D Sticker.R cube = Color.W) ||
     (look Face.R Sticker.R cube = Color.G && look Face.B Sticker.R cube = Color.W) ||
     (look Face.R Sticker.L cube = Color.G && look Face.F Sticker.R cube = Color.W))
let solvedRF cube =
    solvedDR cube && look Face.R Sticker.L cube = Color.G && look Face.R Sticker.DL cube = Color.G &&
    look Face.D Sticker.UR cube = Color.W && look Face.F Sticker.R cube = Color.R && look Face.F Sticker.DR cube = Color.R
let solvedCDRAndRFPair cube =
    solvedFB cube &&
    ((look Face.R Sticker.L cube = Color.G && look Face.R Sticker.DL cube = Color.G &&
      look Face.F Sticker.UL cube = Color.R && look Face.F Sticker.L cube = Color.R &&
      look Face.D Sticker.UR cube = Color.W && look Face.R Sticker.D cube = Color.G && look Face.D Sticker.R cube = Color.W) ||
     (look Face.R Sticker.U cube = Color.G && look Face.R Sticker.UL cube = Color.G &&
      look Face.U Sticker.DR cube = Color.R && look Face.U Sticker.R cube = Color.R &&
      look Face.F Sticker.DL cube = Color.W && look Face.R Sticker.L cube = Color.G && look Face.F Sticker.R cube = Color.W) ||
     (look Face.R Sticker.D cube = Color.G && look Face.R Sticker.DR cube = Color.G &&
      look Face.D Sticker.DR cube = Color.R && look Face.D Sticker.R cube = Color.R &&
      look Face.B Sticker.DL cube = Color.W && look Face.R Sticker.R cube = Color.G && look Face.B Sticker.R cube = Color.W))

let piecesCDLAndLF cube =
    let c, _ = findCenter Color.B cube
    let dl, _ = findEdge Color.B Color.W cube
    let fl, _ = findEdge Color.B Color.R cube
    let dlf, _ = findCorner Color.B Color.R Color.W cube
    [Center c; Edge dl; Edge fl; Corner dlf]
let piecesFB cube =
    let bl, _ = findEdge Color.B Color.O cube
    let dlb, _ = findCorner Color.B Color.O Color.W cube
    piecesCDLAndLF cube @ [Edge bl; Corner dlb]
let piecesFBAndDRAndRF cube =
    let c, _ = findCenter Color.G cube
    let dr, _ = findEdge Color.G Color.W cube
    let fr, _ = findEdge Color.G Color.R cube
    let dfr, _ = findCorner Color.G Color.R Color.W cube
    piecesFB cube @ [Center c; Edge dr; Edge fr; Corner dfr]
let piecesSB cube =
    let br, _ = findEdge Color.G Color.O cube
    let dbr, _ = findCorner Color.G Color.O Color.W cube
    piecesFBAndDRAndRF cube @ [Edge br; Corner dbr]

let fbMoves = Cube.movesAll
let sbMoves = [Move.U; U'; U2; Move.R; R'; R2; Move.RW; RW'; RW2; Move.F; F'; Move.B; B'; Move.M; M'; M2]
let output name = Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, "..", "Patterns", "Roux", "Intermediate", name + ".txt"))

generate (output "InsertLFFirstPair") (Color.B, Color.W, Color.R) solvedCDL solvedCDL piecesCDLAndLF fbMoves
generate (output "InsertLBLastPair") (Color.B, Color.W, Color.O) solvedLF solvedLF piecesFB fbMoves
generate (output "InsertRFFirstPair") (Color.G, Color.W, Color.R) solvedFB solvedCDR piecesFBAndDRAndRF sbMoves

// InsertRBLastPair is generated by MirrorPairPatterns.fsx. That tool first
// proves the reflection mapping over every supported move, then mirrors the
// complete RF-last bank. A direct breadth-first traversal here is needlessly
// enormous and previously made this generator appear to hang.
