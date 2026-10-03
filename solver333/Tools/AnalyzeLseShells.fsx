#r "../../library/bin/Release/net8.0/Library.dll"

open System
open System.IO
open Lse

let policyPath = Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, "..", "Data", "lse-policy-v1.dat"))
let policy = loadPolicy policyPath

let axisMask mask firstBit =
    [ firstBit .. firstBit + 2 ]
    |> List.exists (fun bit -> mask &&& (1 <<< bit) <> 0)

let moveNames = [| "M"; "M'"; "M2'"; "U"; "U'"; "U2'" |]

let solveFrom firstMove index =
    let result = ResizeArray<int>()
    result.Add firstMove
    let mutable current = nextIndex firstMove index
    while policy.Distances[current] <> 0uy do
        let mask = int policy.OptimalMoves[current]
        let move = [ 0 .. 5 ] |> List.find (fun candidate -> mask &&& (1 <<< candidate) <> 0)
        result.Add move
        current <- nextIndex move current
    result |> Seq.map (fun move -> moveNames[move]) |> String.concat " "

for distance in 0 .. policy.MaxDistance do
    let states =
        [| for index in 0 .. policy.Distances.Length - 1 do
               if policy.Distances[index] = byte distance then yield index |]
    let mFirst = states |> Array.sumBy (fun index -> if axisMask (int policy.OptimalMoves[index]) 0 then 1 else 0)
    let uFirst = states |> Array.sumBy (fun index -> if axisMask (int policy.OptimalMoves[index]) 3 then 1 else 0)
    let both =
        states
        |> Array.sumBy (fun index ->
            let mask = int policy.OptimalMoves[index]
            if axisMask mask 0 && axisMask mask 3 then 1 else 0)
    let tiedMoves =
        states
        |> Array.countBy (fun index -> Convert.ToString(int policy.OptimalMoves[index], 2).Replace("0", "").Length)
        |> Array.sortBy fst
        |> Array.map (fun (count, states) -> $"{count}:{states}")
        |> String.concat " "
    printfn "d%i states=%i M-first=%i U-first=%i both=%i optimal-move-counts=%s" distance states.Length mFirst uFirst both tiedMoves

    if distance = 6 then
        let shared =
            states
            |> Array.find (fun index ->
                let mask = int policy.OptimalMoves[index]
                axisMask mask 0 && axisMask mask 3)
        let mask = int policy.OptimalMoves[shared]
        for firstMove in 0 .. 5 do
            if mask &&& (1 <<< firstMove) <> 0 then
                printfn "  shared example state=%i via %s" shared (solveFrom firstMove shared)
