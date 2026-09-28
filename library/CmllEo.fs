module CmllEo

open Cube

let cornerSlotCount = 24 * 27
let reachableEdgeCenterCount = 46080
let boundaryStateCount = cornerSlotCount * reachableEdgeCenterCount

let private topCorners = [| Corner.ULF; Corner.ULB; Corner.URB; Corner.URF |]

let private colorsAt (locations: (Face * Sticker) list) (cube: Cube) : Color list =
    locations |> List.map (fun (face, sticker) -> Cube.look face sticker cube)

let private cornerColors (corner: Corner) (cube: Cube) = colorsAt (Cube.cornerToFaceStickers corner) cube
let private sameColors (left: Color list) (right: Color list) = List.sort left = List.sort right
let private solvedCornerColors = topCorners |> Array.map (fun corner -> cornerColors corner Cube.solved)

let private permutationRank (permutation: int array) =
    let mutable rank = 0
    for index in 0 .. permutation.Length - 1 do
        let mutable smaller = 0
        for later in index + 1 .. permutation.Length - 1 do
            if permutation[later] < permutation[index] then smaller <- smaller + 1
        rank <- rank * (permutation.Length - index) + smaller
    rank

let private isUpDown = function Color.W | Color.Y -> true | _ -> false

/// Ranks the concrete four-top-corner state in 0..647.  Both Roux blocks must
/// already be solved, so only top-layer corner pieces are admitted.  The first
/// three twists are stored; legality determines the fourth.
let indexCornersCube (cube: Cube) =
    // Reuse the LSE boundary validator.  Unlike indexCube, this intentionally
    // does not require the top corners to be solved up to AUF.
    Lse.indexEdgeCenterCube cube |> ignore
    let pieces = Array.zeroCreate 4
    let twists = Array.zeroCreate 4
    for position in 0 .. 3 do
        let actual = cornerColors topCorners[position] cube
        pieces[position] <-
            solvedCornerColors
            |> Array.tryFindIndex (sameColors actual)
            |> Option.defaultWith (fun () -> invalidArg "cube" $"Non-top corner occupies {topCorners[position]}.")
        twists[position] <-
            actual
            |> List.tryFindIndex isUpDown
            |> Option.defaultWith (fun () -> invalidArg "cube" $"Corner at {topCorners[position]} has no U/D sticker.")
    if (pieces |> Array.distinct |> Array.length) <> 4 then invalidArg "cube" "Top-corner permutation contains duplicate pieces."
    let twistRank = twists[0] + 3 * twists[1] + 9 * twists[2]
    permutationRank pieces * 27 + twistRank

/// A stable sparse coordinate useful to native generators before parity
/// compression.  Exactly 29,859,840 of its 59,719,680 slots are reachable.
let indexSparseBoundaryCube (cube: Cube) =
    indexCornersCube cube * Lse.edgeCenterSlotCount + Lse.indexEdgeCenterCube cube
