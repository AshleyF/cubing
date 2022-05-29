module Utility

open System.IO

let level = 2 // 0 = beginner, 1 = intermediate, 2 = advanced

let readPatterns matchFn method level name cornerRotationNeutral cornerColorNeutral discoverAuf =
    File.ReadLines $"Patterns/{method}/{level}/{name}.txt"
    |> Seq.map (fun line -> line.Split ',')
    |> Seq.map (fun x ->
        let pattern = x[0]
        let algs = if x[1].Length = 0 then [] else x[1].Split ';' |> List.ofSeq
        matchFn, name, (pattern, cornerRotationNeutral, cornerColorNeutral, discoverAuf), algs)
    |> List.ofSeq