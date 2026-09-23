#r "../../library/bin/Release/net8.0/Library.dll"

open System
open System.IO
open Cube

let horizontal face =
    faceOfStickers (stickerUR face) (stickerU face) (stickerUL face)
                   (stickerR face) (stickerC face) (stickerL face)
                   (stickerDR face) (stickerD face) (stickerDL face)
let vertical face =
    faceOfStickers (stickerDL face) (stickerD face) (stickerDR face)
                   (stickerL face) (stickerC face) (stickerR face)
                   (stickerUL face) (stickerU face) (stickerUR face)
let identity face = face
let diagonal face = face |> horizontal |> faceRotateCW
let antiDiagonal face = face |> horizontal |> faceRotateCCW
let transforms =
    [ "identity", identity; "cw", faceRotateCW; "half", faceRotate180; "ccw", faceRotateCCW
      "horizontal", horizontal; "vertical", vertical; "diagonal", diagonal; "anti-diagonal", antiDiagonal ]
let recolor = function Color.R -> Color.O | Color.O -> Color.R | color -> color
let reflectWith ud lr fb cube =
    cubeOfFaces (faceU cube |> ud) (faceD cube |> ud)
                (faceL cube |> lr) (faceR cube |> lr)
                (faceB cube |> fb) (faceF cube |> fb)
    |> Map.map (fun _ face -> face |> Map.map (fun _ color -> recolor color))

let mirrorMove = function
    | Move.U -> U' | U' -> Move.U | U2 -> U2 | Move.D -> D' | D' -> Move.D | D2 -> D2
    | Move.L -> L' | L' -> Move.L | L2 -> L2 | Move.R -> R' | R' -> Move.R | R2 -> R2
    | Move.F -> B' | F' -> Move.B | F2 -> B2 | Move.B -> F' | B' -> Move.F | B2 -> F2
    | Move.M -> M' | M' -> Move.M | M2 -> M2
    | RW -> RW' | RW' -> RW | RW2 -> RW2
    | move -> failwith $"Unsupported mirror move: {move}"

let validationMoves = [Move.U; U'; U2; Move.R; R'; R2; RW; RW'; RW2; Move.F; F'; F2; Move.B; B'; B2; Move.M; M'; M2]
let samples =
    [ Cube.solved
      Cube.executeMoves [Move.R; Move.U; F'; M2; B2] Cube.solved
      Cube.executeMoves [B'; RW; U2; Move.F; M'] Cube.solved ]
let choice =
    [ for udName, ud in transforms do
      for lrName, lr in transforms do
      for fbName, fb in transforms do
        let candidate = reflectWith ud lr fb
        if samples |> List.forall (fun cube -> validationMoves |> List.forall (fun move -> executeMove cube move |> candidate = (candidate cube |> fun mirrored -> executeMove mirrored (mirrorMove move)))) then
            yield udName, lrName, fbName, candidate ]
    |> List.tryHead
let udName, lrName, fbName, reflect = choice |> Option.defaultWith (fun () -> failwith "No valid reflection mapping found")
printfn "Reflection transforms: U/D=%s L/R=%s F/B=%s" udName lrName fbName

let source = Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, "..", "Patterns", "Roux", "Intermediate", "InsertRFPair.txt"))
let output = Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, "..", "Patterns", "Roux", "Intermediate", "InsertRBLastPair.txt"))
let lines =
    File.ReadLines(source)
    |> Seq.map (fun line ->
        let comma = line.IndexOf(',')
        let pattern = line.Substring(0, comma) |> Render.stringToCube |> reflect |> Render.cubeToString
        let algorithms =
            line.Substring(comma + 1).Split(';', StringSplitOptions.RemoveEmptyEntries)
            |> Array.map (fun algorithm ->
                algorithm |> Render.stringToSteps |> List.map (function Step.Move move -> Step.Move (mirrorMove move) | step -> step) |> Render.stepsToString)
        $"{pattern},{String.Join(';', algorithms)}")
    |> Seq.toArray
File.WriteAllLines(output, lines)
printfn "Mirrored %i RF-last cases into exhaustive RB-last cases" lines.Length
