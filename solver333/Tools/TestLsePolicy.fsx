#r "../bin/Release/net8.0/Library.dll"

open System
open Cube

let tablePath = System.IO.Path.Combine(__SOURCE_DIRECTORY__, "..", "Data", "lse-policy-v1.dat")
let policy = Lse.loadPolicy tablePath
let reachable, maximum = Lse.validatePolicy policy

if reachable <> 184320 then failwith $"Expected 184320 reachable LSE states, found {reachable}."

for index in 0 .. Lse.slotCount - 1 do
    if policy.Distances[index] <> Lse.unreachable then
        let solution = Lse.solveIndex policy index
        if solution.Length <> int policy.Distances[index] then
            failwith $"State {index} has distance {policy.Distances[index]} but a {solution.Length}-move solution."

let random = Random 3332026
for trial in 1 .. 2000 do
    let length = random.Next(1, 101)
    let mutable cube = Cube.solved
    let mutable index = Lse.indexCube cube
    for _ in 1 .. length do
        let moveIndex = random.Next Lse.moves.Length
        cube <- Cube.executeMove cube Lse.moves[moveIndex]
        index <- Lse.nextIndex moveIndex index
        let cubeIndex = Lse.indexCube cube
        if index <> cubeIndex then
            failwith $"Compact transition disagrees with the cube model in trial {trial}: {index} <> {cubeIndex}."
    let solution = Lse.solveCube policy cube
    if solution.Length <> int policy.Distances[index] then
        failwith $"Trial {trial} did not return a shortest solution."
    let solved = Cube.executeMoves solution cube
    if Render.cubeToString solved <> Render.cubeToString Cube.solved then
        failwith $"Trial {trial} did not solve the full cube."

printfn "Verified %i reachable LSE states through depth %i and 2,000 full-cube transition/solution trials." reachable maximum
