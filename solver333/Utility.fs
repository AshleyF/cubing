module Utility

#if !FABLE_COMPILER
open System.IO
#endif

let level = 0 // 0 = beginner, 1 = intermediate, 2 = advanced, 3 = god
let mutable cornerOrientationLevel = 0
let mutable cornerPermutationLevel = 0
let mutable fullCmll = false
let mutable edgeOrientationLevel = 0
let mutable lbPairLevel = 0
let mutable lfPairLevel = 0
let mutable rbPairLevel = 0
let mutable rfPairLevel = 0
let mutable orientCentersWithSecondBlock = false
let mutable chooseShortestSecondBlockPairOrder = false
let mutable chooseShortestFirstBlockPairOrder = false
let mutable x2yColorNeutral = false
let mutable useEolr = false

let readPatterns matchFn method level name cornerRotationNeutral cornerColorNeutral discoverAuf =
#if FABLE_COMPILER
    PatternData.read $"{method}/{level}/{name}"
#else
    File.ReadLines $"Patterns/{method}/{level}/{name}.txt"
#endif
    |> Seq.map (fun line -> line.Split ',')
    |> Seq.map (fun x ->
        let pattern = x[0]
        let algs = if x[1].Length = 0 then [] else x[1].Split ';' |> List.ofSeq
        matchFn, name, (pattern, cornerRotationNeutral, cornerColorNeutral, discoverAuf), algs)
    |> List.ofSeq
