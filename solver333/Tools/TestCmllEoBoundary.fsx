#r "../../library/bin/Debug/net8.0/Library.dll"

open Cube

let solvedEdgeCenter = Lse.indexEdgeCenterCube Cube.solved
let solvedAuf = Lse.cornerAufCube Cube.solved
let solvedLse = Lse.indexCube Cube.solved
if Lse.indexFromEdgeCenterAndAuf solvedEdgeCenter solvedAuf <> solvedLse then
    failwith "Split CMLLEO boundary coordinate does not reproduce the solved LSE index."

let lseAlgorithms =
    [ "M"; "M'"; "M2"; "U"; "U'"; "U2"
      "M U M' U2 M2 U'"; "U2 M' U M2 U' M" ]

for algorithm in lseAlgorithms do
    let cube = Cube.solved |> Cube.executeSteps (Render.stringToSteps algorithm)
    let split = Lse.indexFromEdgeCenterAndAuf (Lse.indexEdgeCenterCube cube) (Lse.cornerAufCube cube)
    let direct = Lse.indexCube cube
    if split <> direct then failwith $"Split coordinate mismatch after {algorithm}: {split} <> {direct}."

let unsolvedCorners = Cube.solved |> Cube.executeSteps (Render.stringToSteps "R U R' U R U2 R'")
// The edge/center half is deliberately usable before CMLL is solved.
Lse.indexEdgeCenterCube unsolvedCorners |> ignore
let cornerIndex = CmllEo.indexCornersCube unsolvedCorners
if cornerIndex < 0 || cornerIndex >= CmllEo.cornerSlotCount then failwith "Corner coordinate is out of range."
CmllEo.indexSparseBoundaryCube unsolvedCorners |> ignore
try
    Lse.cornerAufCube unsolvedCorners |> ignore
    failwith "Expected an unsolved corner state to be rejected as a CMLLEO boundary."
with :? System.ArgumentException -> ()

printfn "CMLLEO_BOUNDARY|ok|edge_center_slots=%d|combined_states=%d" Lse.edgeCenterSlotCount (648 * 46080)
