module Solver

open System
open Cube

let solved =
    let u = faceOfStickers Color.W Color.W Color.W Color.W Color.W Color.W Color.W Color.W Color.W
    let d = faceOfStickers Color.Y Color.Y Color.Y Color.Y Color.Y Color.Y Color.Y Color.Y Color.Y
    let l = faceOfStickers Color.O Color.O Color.O Color.O Color.O Color.O Color.O Color.O Color.O
    let r = faceOfStickers Color.R Color.R Color.R Color.R Color.R Color.R Color.R Color.R Color.R
    let f = faceOfStickers Color.G Color.G Color.G Color.G Color.G Color.G Color.G Color.G Color.G
    let b = faceOfStickers Color.B Color.B Color.B Color.B Color.B Color.B Color.B Color.B Color.B
    cubeOfFaces u d l r f b

let scramble n =
    let moves = [Move.U; U'; U2; Move.D; D'; D2; Move.L; L'; L2; Move.R; R'; R2; Move.F; F'; F2; Move.B; B'; B2] @ [M; M'; M2] @ [S; S'; S2; E; E'; E2] // NOTE: centers don't move without slices
    let rand = Random()
    let rec scramble' cube sequence history n =
        let isRepeat c = List.contains c history
        let rec canUndoTwoInOneMove c = function
            | m :: t ->
                match history with
                    | _ :: previous :: _ -> if move m c = previous then true else canUndoTwoInOneMove c t
                    | _ -> false
            | _ -> false
        if n = 0 then (cube, Seq.rev sequence) else
            let m = List.item (rand.Next moves.Length) moves
            let cube' = move m cube
            if isRepeat cube' || canUndoTwoInOneMove cube' moves
            then scramble' cube sequence history n // try again
            else scramble' cube' (m :: sequence) (cube' :: history) (n - 1)
    scramble' solved [] [solved] n

let solve includeRotations includeMoves includeWideMoves includeSliceMoves depth check cube =
    let rec solve' max depth steps cube = seq {
        let recurse s = seq { yield! solve' max (depth + 1) (s :: steps) (step s cube) }
        if check cube then yield Seq.rev steps
        elif depth < max then
            if includeRotations then
                yield! recurse (Rotate X)
                yield! recurse (Rotate X')
                yield! recurse (Rotate X2)
                yield! recurse (Rotate Y)
                yield! recurse (Rotate Y')
                yield! recurse (Rotate Y2)
                yield! recurse (Rotate Z)
                yield! recurse (Rotate Z')
                yield! recurse (Rotate Z2)
            if includeMoves then
                yield! recurse (Move Move.U)
                yield! recurse (Move Move.U')
                yield! recurse (Move Move.U2)
                if includeWideMoves then
                    yield! recurse (Move Move.UW)
                    yield! recurse (Move Move.UW')
                    yield! recurse (Move Move.UW2)
                yield! recurse (Move Move.D)
                yield! recurse (Move Move.D')
                yield! recurse (Move Move.D2)
                if includeWideMoves then
                    yield! recurse (Move Move.DW)
                    yield! recurse (Move Move.DW')
                    yield! recurse (Move Move.DW2)
                yield! recurse (Move Move.L)
                yield! recurse (Move Move.L')
                yield! recurse (Move Move.L2)
                if includeWideMoves then
                    yield! recurse (Move Move.LW)
                    yield! recurse (Move Move.LW')
                    yield! recurse (Move Move.LW2)
                yield! recurse (Move Move.R)
                yield! recurse (Move Move.R')
                yield! recurse (Move Move.R2)
                if includeWideMoves then
                    yield! recurse (Move Move.RW)
                    yield! recurse (Move Move.RW')
                    yield! recurse (Move Move.RW2)
                yield! recurse (Move Move.F)
                yield! recurse (Move Move.F')
                yield! recurse (Move Move.F2)
                if includeWideMoves then
                    yield! recurse (Move Move.FW)
                    yield! recurse (Move Move.FW')
                    yield! recurse (Move Move.FW2)
                yield! recurse (Move Move.B)
                yield! recurse (Move Move.B')
                yield! recurse (Move Move.B2)
                if includeWideMoves then
                    yield! recurse (Move Move.BW)
                    yield! recurse (Move Move.BW')
                    yield! recurse (Move Move.BW2)
                if includeSliceMoves then
                    yield! recurse (Move Move.M)
                    yield! recurse (Move Move.M')
                    yield! recurse (Move Move.M2)
                    yield! recurse (Move Move.S)
                    yield! recurse (Move Move.S')
                    yield! recurse (Move Move.S2)
                    yield! recurse (Move Move.E)
                    yield! recurse (Move Move.E')
                    yield! recurse (Move Move.E2) }
    let rec iterativeDeepening depth = seq { // TODO: something more efficient (breadth-first)
        let solutions = solve' depth 0 [] cube |> List.ofSeq
        yield! solutions
        if Seq.length solutions = 0 then yield! iterativeDeepening (depth + 1) }
    iterativeDeepening 0