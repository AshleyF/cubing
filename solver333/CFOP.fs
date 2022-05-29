module CFOP

open Cube
open Solver
open Utility

let downCenterPatterns = [
    matchesGeneric, "DCenter", (".............Y...................................W....", false, false, false), [] // skip
    matchesGeneric, "DCenter", ("....W..........................Y......................", false, false, false), ["x"]
    matchesGeneric, "DCenter", ("....Y..........................W......................", false, false, false), ["x'"]
    matchesGeneric, "DCenter", (".............W...................................Y....", false, false, false), ["x2"]
    matchesGeneric, "DCenter", ("............................Y.....W...................", false, false, false), ["z"]
    matchesGeneric, "DCenter", ("............................W.....Y...................", false, false, false), ["z'"]]

let cfopBeginnerPatterns = downCenterPatterns
let cfopIntermediatePatterns = [] // TODO
let cfopAdvancedPatterns = [] // TODO

let solve =
    let patterns =
        match level with 
        | 0 -> cfopBeginnerPatterns
        | 1 -> cfopIntermediatePatterns
        | 2 -> cfopAdvancedPatterns
        | _ -> failwith "Unknown level"
    patterns |> expandPatternsForAuf |> solveCase

let generate numCubes =
    let scrambled = initScrambledCubes numCubes

    // down-center (assume white cross)
    let caseDC c = look Face.D Sticker.C c = Color.W

    let rotations = [Rotate X; Rotate X'; Rotate X2; Rotate Y; Rotate Y'; Rotate Y2; Rotate Z; Rotate Z'; Rotate Z2]
    let solvedDC = solve rotations "Solve down-center (during inspection)" "DCenter" caseDC scrambled false

    Solver.stageStats "Cross" numCubes