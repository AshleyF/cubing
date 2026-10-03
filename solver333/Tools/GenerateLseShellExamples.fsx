#r "../../library/bin/Release/net8.0/Library.dll"

open System
open System.IO
open System.Text.Json
open Lse
open Cube

type DiagramState =
    { pieces: int array
      flips: int array
      center: int
      auf: int
      distance: int
      setup: string
      solution: string
      optimalFirstMoves: string array }

let moveNames = [| "M"; "M'"; "M2'"; "U"; "U'"; "U2'" |]

let permutationOfRank rank =
    let factorial = [| 1; 1; 2; 6; 24; 120; 720 |]
    let available = ResizeArray<int>([0 .. 5])
    let permutation = Array.zeroCreate 6
    let mutable remaining = rank
    for position in 0 .. 5 do
        let block = factorial[5 - position]
        let digit = remaining / block
        remaining <- remaining % block
        permutation[position] <- available[digit]
        available.RemoveAt digit
    permutation

let decode index distance setup solution optimalFirstMoves =
    let auf = index % 4
    let centerAndAbove = index / 4
    let center = centerAndAbove % 4
    let flipAndAbove = centerAndAbove / 4
    let flipRank = flipAndAbove % 32
    let flips = Array.zeroCreate 6
    let mutable parity = 0
    for bit in 0 .. 4 do
        flips[bit] <- (flipRank >>> bit) &&& 1
        parity <- parity ^^^ flips[bit]
    flips[5] <- parity
    { pieces = permutationOfRank (flipAndAbove / 32)
      flips = flips
      center = center
      auf = auf
      distance = distance
      setup = setup
      solution = solution
      optimalFirstMoves = optimalFirstMoves }

let policyPath = Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, "..", "Data", "lse-policy-v1.dat"))
let policy = loadPolicy policyPath

let solve index =
    let moves = ResizeArray<int>()
    let mutable current = index
    while policy.Distances[current] <> 0uy do
        let mask = int policy.OptimalMoves[current]
        let move = [0 .. 5] |> List.find (fun candidate -> mask &&& (1 <<< candidate) <> 0)
        moves.Add move
        current <- nextIndex move current
    List.ofSeq moves

let inverseMove = function
    | 0 -> 1 | 1 -> 0 | 2 -> 2 | 3 -> 4 | 4 -> 3 | 5 -> 5
    | move -> failwith $"Invalid move {move}."

let render moves = moves |> List.map (fun move -> moveNames[move]) |> String.concat " "

let states =
    [| for index in 0 .. policy.Distances.Length - 1 do
           let distance = policy.Distances[index]
           if distance <> unreachable && distance <= 20uy then
               let solutionMoves = solve index
               let setupMoves = solutionMoves |> List.rev |> List.map inverseMove
               let reconstructed = Cube.executeMoves (setupMoves |> List.map (fun move -> Lse.moves[move])) Cube.solved
               let reconstructedIndex = Lse.indexCube reconstructed
               if reconstructedIndex <> index then
                   failwith $"Setup reconstruction for state {index} produced {reconstructedIndex}."
               let optimalFirstMoves =
                   [| for move in 0 .. 5 do
                          if int policy.OptimalMoves[index] &&& (1 <<< move) <> 0 then
                              yield moveNames[move] |]
               yield decode index (int distance) (render setupMoves) (render solutionMoves) optimalFirstMoves |]
    |> Array.sortBy (fun state -> state.distance, state.solution)

let expectedCounts =
    [| 1; 6; 18; 54; 161; 472; 1346; 3523; 8374; 17254; 31575
       40622; 40200; 25959; 10643; 2816; 882; 320; 80; 12; 2 |]

for distance in 0 .. 20 do
    let count = states |> Array.sumBy (fun state -> if state.distance = distance then 1 else 0)
    let expected = expectedCounts[distance]
    if count <> expected then failwith $"Distance {distance} produced {count} states; expected {expected}."

let outputPath = Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, "..", "..", "site", "lab", "lse-shells.json"))
let outputDirectory = Path.GetDirectoryName(outputPath)
let compactOptions = JsonSerializerOptions(WriteIndented = false)
let index =
    [| for distance in 0 .. 20 ->
           {| distance = distance
              count = expectedCounts[distance] |} |]
let indexPath = Path.Combine(outputDirectory, "lse-shells-index.json")
File.WriteAllText(indexPath, JsonSerializer.Serialize(index, compactOptions) + Environment.NewLine)

for distance in 0 .. 20 do
    let shell = states |> Array.filter (fun state -> state.distance = distance)
    let shellPath = Path.Combine(outputDirectory, $"lse-shells-{distance}.json")
    File.WriteAllText(shellPath, JsonSerializer.Serialize(shell, compactOptions) + Environment.NewLine)
    printfn "Wrote distance %i: %i states to %s" distance shell.Length shellPath

printfn "Wrote %i exact states in distance-partitioned files." states.Length
