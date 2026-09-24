module Lse

open System
open Cube

let moves = [| Move.M; Move.M'; Move.M2; Move.U; Move.U'; Move.U2 |]
let slotCount = 720 * 32 * 4 * 4
let unreachable = Byte.MaxValue

type Policy =
    { Distances: byte array
      OptimalMoves: byte array
      ReachableCount: int
      MaxDistance: int }

type private State =
    { Pieces: int array
      Flips: int array
      Center: int
      Auf: int }

let private lseEdges = [| Edge.UL; Edge.UR; Edge.UF; Edge.UB; Edge.DF; Edge.DB |]
let private otherEdges = [| Edge.DL; Edge.DR; Edge.FL; Edge.FR; Edge.BL; Edge.BR |]
let private topCorners = [| Corner.ULF; Corner.ULB; Corner.URB; Corner.URF |]
let private bottomCorners = [| Corner.DLF; Corner.DLB; Corner.DRB; Corner.DRF |]
let private allCenters = [| Center.U; Center.D; Center.L; Center.R; Center.F; Center.B |]

let private apply move cube = Cube.executeMove cube move

let private repeat count move =
    let mutable cube = Cube.solved
    for _ in 1 .. count do cube <- apply move cube
    cube

let private mReferences = Array.init 4 (fun count -> repeat count Move.M)
let private uReferences = Array.init 4 (fun count -> repeat count Move.U)

let private colorsAt locations cube =
    locations |> List.map (fun (face, sticker) -> Cube.look face sticker cube)

let private sameColors left right = List.sort left = List.sort right

let private edgeColors edge cube = colorsAt (Cube.edgeToFaceStickers edge) cube
let private cornerColors corner cube = colorsAt (Cube.cornerToFaceStickers corner) cube
let private centerColor center cube =
    let face, sticker = Cube.centerToFaceSticker center
    Cube.look face sticker cube

let private solvedLseEdgeColors = lseEdges |> Array.map (fun edge -> edgeColors edge Cube.solved)

let private permutationRank (permutation: int array) =
    let mutable rank = 0
    for index in 0 .. permutation.Length - 1 do
        let mutable smaller = 0
        for later in index + 1 .. permutation.Length - 1 do
            if permutation[later] < permutation[index] then smaller <- smaller + 1
        rank <- rank * (permutation.Length - index) + smaller
    rank

let private factorial = [| 1; 1; 2; 6; 24; 120; 720 |]

let private permutationOfRank rank =
    let available = ResizeArray<int>([0 .. 5])
    let permutation = Array.zeroCreate 6
    let mutable remaining = rank
    for index in 0 .. 5 do
        let block = factorial[5 - index]
        let digit = remaining / block
        remaining <- remaining % block
        permutation[index] <- available[digit]
        available.RemoveAt digit
    permutation

let private indexState state =
    let mutable flipRank = 0
    for index in 0 .. 4 do flipRank <- flipRank ||| (state.Flips[index] <<< index)
    (((permutationRank state.Pieces * 32 + flipRank) * 4 + state.Center) * 4 + state.Auf)

let private stateOfIndex index =
    let auf = index % 4
    let centerAndAbove = index / 4
    let center = centerAndAbove % 4
    let flipAndAbove = centerAndAbove / 4
    let flipRank = flipAndAbove % 32
    let permutation = permutationOfRank (flipAndAbove / 32)
    let flips = Array.zeroCreate 6
    let mutable parity = 0
    for bit in 0 .. 4 do
        flips[bit] <- (flipRank >>> bit) &&& 1
        parity <- parity ^^^ flips[bit]
    flips[5] <- parity
    { Pieces = permutation; Flips = flips; Center = center; Auf = auf }

let private stateOfCubeUnchecked cube =
    let pieces = Array.zeroCreate 6
    let flips = Array.zeroCreate 6
    for position in 0 .. 5 do
        let actual = edgeColors lseEdges[position] cube
        let piece = solvedLseEdgeColors |> Array.tryFindIndex (sameColors actual)
        match piece with
        | None -> invalidArg "cube" $"Non-LSE edge occupies {lseEdges[position]}."
        | Some pieceIndex ->
            pieces[position] <- pieceIndex
            flips[position] <- if actual[0] = solvedLseEdgeColors[pieceIndex][0] then 0 else 1

    let center =
        mReferences
        |> Array.tryFindIndex (fun reference ->
            allCenters |> Array.forall (fun position -> centerColor position cube = centerColor position reference))
        |> Option.defaultWith (fun () -> invalidArg "cube" "Centers are not in the M-slice orbit.")

    let auf =
        uReferences
        |> Array.tryFindIndex (fun reference ->
            topCorners |> Array.forall (fun position -> cornerColors position cube = cornerColors position reference))
        |> Option.defaultWith (fun () -> invalidArg "cube" "Top corners are not solved up to AUF.")

    { Pieces = pieces; Flips = flips; Center = center; Auf = auf }

let private validateLsePreconditions cube =
    otherEdges
    |> Array.iter (fun position ->
        if edgeColors position cube <> edgeColors position Cube.solved then
            invalidArg "cube" $"Block edge {position} is not solved.")
    bottomCorners
    |> Array.iter (fun position ->
        if cornerColors position cube <> cornerColors position Cube.solved then
            invalidArg "cube" $"Bottom corner {position} is not solved.")
    if centerColor Center.L cube <> centerColor Center.L Cube.solved ||
       centerColor Center.R cube <> centerColor Center.R Cube.solved then
        invalidArg "cube" "Left or right center is not solved."

let indexCube cube =
    validateLsePreconditions cube
    stateOfCubeUnchecked cube |> indexState

let private solvedState = stateOfCubeUnchecked Cube.solved

let private transforms =
    moves
    |> Array.map (fun move -> Cube.solved |> apply move |> stateOfCubeUnchecked)

let private moveState moveIndex state =
    let transform = transforms[moveIndex]
    let pieces = Array.zeroCreate 6
    let flips = Array.zeroCreate 6
    for destination in 0 .. 5 do
        let source = transform.Pieces[destination]
        pieces[destination] <- state.Pieces[source]
        flips[destination] <- state.Flips[source] ^^^ transform.Flips[destination]
    { Pieces = pieces
      Flips = flips
      Center = (state.Center + transform.Center) % 4
      Auf = (state.Auf + transform.Auf) % 4 }

let nextIndex moveIndex index = stateOfIndex index |> moveState moveIndex |> indexState

let distance policy index =
    if index < 0 || index >= slotCount || policy.Distances[index] = unreachable then None
    else Some (int policy.Distances[index])

let optimalNextMoves policy index =
    if index < 0 || index >= slotCount || policy.Distances[index] = unreachable then
        invalidArg "index" $"State {index} is not in the reachable LSE orbit."
    let mask = int policy.OptimalMoves[index]
    moves |> Array.mapi (fun bit move -> bit, move) |> Array.choose (fun (bit, move) -> if mask &&& (1 <<< bit) <> 0 then Some move else None) |> List.ofArray

let buildPolicy () =
    let distances = Array.create slotCount unreachable
    let optimalMoves = Array.zeroCreate<byte> slotCount
    let queue = Array.zeroCreate<int> slotCount
    let solvedIndex = indexState solvedState
    let mutable head = 0
    let mutable tail = 1
    let mutable maxDistance = 0
    queue[0] <- solvedIndex
    distances[solvedIndex] <- 0uy

    while head < tail do
        let current = queue[head]
        head <- head + 1
        let nextDistance = int distances[current] + 1
        for moveIndex in 0 .. moves.Length - 1 do
            let neighbor = nextIndex moveIndex current
            if distances[neighbor] = unreachable then
                distances[neighbor] <- byte nextDistance
                maxDistance <- max maxDistance nextDistance
                queue[tail] <- neighbor
                tail <- tail + 1

    for index in 0 .. slotCount - 1 do
        if distances[index] <> unreachable && distances[index] <> 0uy then
            let targetDistance = distances[index] - 1uy
            let mutable mask = 0
            for moveIndex in 0 .. moves.Length - 1 do
                if distances[nextIndex moveIndex index] = targetDistance then
                    mask <- mask ||| (1 <<< moveIndex)
            if mask = 0 then failwith $"Reachable LSE state {index} has no optimal next move."
            optimalMoves[index] <- byte mask

    { Distances = distances
      OptimalMoves = optimalMoves
      ReachableCount = tail
      MaxDistance = maxDistance }

let validatePolicy policy =
    if policy.Distances.Length <> slotCount || policy.OptimalMoves.Length <> slotCount then
        invalidArg "policy" $"Expected {slotCount} LSE policy slots."
    let mutable reachable = 0
    let mutable maximum = 0
    for index in 0 .. slotCount - 1 do
        let distance = policy.Distances[index]
        if distance <> unreachable then
            reachable <- reachable + 1
            maximum <- max maximum (int distance)
            if distance = 0uy then
                if index <> indexState solvedState then failwith $"Unexpected zero-distance LSE state {index}."
            else
                let mask = int policy.OptimalMoves[index]
                if mask &&& ~~~0x3F <> 0 then failwith $"LSE state {index} contains invalid optimal-move bits."
                if mask = 0 then failwith $"LSE state {index} has no stored optimal move."
                for moveIndex in 0 .. moves.Length - 1 do
                    if mask &&& (1 <<< moveIndex) <> 0 && policy.Distances[nextIndex moveIndex index] <> distance - 1uy then
                        failwith $"LSE state {index} marks a non-optimal move {moves[moveIndex]}."
    if reachable <> policy.ReachableCount then failwith "LSE reachable-count metadata is incorrect."
    if maximum <> policy.MaxDistance then failwith "LSE maximum-distance metadata is incorrect."
    reachable, maximum

let solveIndex policy index =
    if index < 0 || index >= slotCount || policy.Distances[index] = unreachable then
        invalidArg "index" $"State {index} is not in the reachable LSE orbit."
    let solution = ResizeArray<Move>()
    let mutable current = index
    while policy.Distances[current] <> 0uy do
        let mask = int policy.OptimalMoves[current]
        let moveIndex = [0 .. moves.Length - 1] |> List.find (fun bit -> mask &&& (1 <<< bit) <> 0)
        solution.Add moves[moveIndex]
        current <- nextIndex moveIndex current
    List.ofSeq solution

let solveCube policy cube = indexCube cube |> solveIndex policy
let distanceCube policy cube = indexCube cube |> distance policy |> Option.get
let optimalNextMovesCube policy cube = indexCube cube |> optimalNextMoves policy

let private recolorToSolvedFrame goal cube =
    let colorMap =
        allCenters
        |> Array.map (fun center -> centerColor center goal, centerColor center Cube.solved)
        |> Map.ofArray
    cube |> Map.map (fun _ face -> face |> Map.map (fun _ color -> Map.find color colorMap))

let indexCubeRelative goal cube = cube |> recolorToSolvedFrame goal |> indexCube
let solveCubeRelative policy goal cube = indexCubeRelative goal cube |> solveIndex policy

let mutable private installedPolicy: Policy option = None

let installPolicy distances optimalMoves reachable maximum =
    let policy =
        { Distances = Array.copy distances
          OptimalMoves = Array.copy optimalMoves
          ReachableCount = reachable
          MaxDistance = maximum }
    validatePolicy policy |> ignore
    installedPolicy <- Some policy

let requirePolicy () =
    installedPolicy |> Option.defaultWith (fun () -> failwith "The exact LSE policy has not been loaded.")

#if !FABLE_COMPILER
open System.IO

let savePolicy path policy =
    validatePolicy policy |> ignore
    let directory = Path.GetDirectoryName(Path.GetFullPath path)
    Directory.CreateDirectory directory |> ignore
    use stream = File.Create path
    use writer = new BinaryWriter(stream)
    writer.Write [| byte 'R'; byte 'L'; byte 'S'; byte 'E' |]
    writer.Write 1
    writer.Write slotCount
    writer.Write policy.ReachableCount
    writer.Write policy.MaxDistance
    for index in 0 .. slotCount - 1 do
        writer.Write policy.Distances[index]
        writer.Write policy.OptimalMoves[index]

let loadPolicy path =
    use stream = File.OpenRead path
    use reader = new BinaryReader(stream)
    let magic = reader.ReadBytes 4
    if magic <> [| byte 'R'; byte 'L'; byte 'S'; byte 'E' |] then invalidArg "path" "Not an LSE policy file."
    let version = reader.ReadInt32()
    if version <> 1 then invalidArg "path" $"Unsupported LSE policy version {version}."
    let slots = reader.ReadInt32()
    if slots <> slotCount then invalidArg "path" $"Expected {slotCount} slots, found {slots}."
    let reachable = reader.ReadInt32()
    let maximum = reader.ReadInt32()
    let distances = Array.zeroCreate<byte> slotCount
    let optimalMoves = Array.zeroCreate<byte> slotCount
    for index in 0 .. slotCount - 1 do
        distances[index] <- reader.ReadByte()
        optimalMoves[index] <- reader.ReadByte()
    if stream.Position <> stream.Length then invalidArg "path" "Unexpected trailing data in LSE policy file."
    let policy = { Distances = distances; OptimalMoves = optimalMoves; ReachableCount = reachable; MaxDistance = maximum }
    validatePolicy policy |> ignore
    policy
#endif
