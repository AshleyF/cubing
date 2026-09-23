#r "../../library/bin/Release/net8.0/Library.dll"
#r "../bin/Release/net8.0/Solver.dll"

open Cube

let cube = Render.stringToCube "YWBWROWRBRBORWYGGYGWYRYBROWGBRYOYBGBOBOWOOYRWGGBWYGGOR"
let caseLFPair = Solver.lookPattern "............................BBR......BBR.....W..W....."
let matches = Roux.lfPairFirstIntermediatePatterns |> List.filter (fun (_, _, (pattern, auf, colors, _), _) -> Solver.matchesGeneric cube (pattern, auf, colors))
printfn "Matched patterns: %i" matches.Length
for _, _, (pattern, _, _, _), algorithms in matches do
    printfn "%s" pattern
    for algorithm in algorithms do
        let solved = Cube.executeSteps (Render.stringToSteps algorithm) cube
        printfn "%s -> %b" algorithm (caseLFPair solved)

let lfSolved = Cube.executeSteps (Render.stringToSteps "M' r F") cube
let caseFB = Solver.lookPattern "O..O.......................BBBR.....BBBR.....W..W..W.."
let lastMatches = Roux.lbPairLastIntermediatePatterns |> List.filter (fun (_, _, (pattern, auf, colors, _), _) -> Solver.matchesGeneric lfSolved (pattern, auf, colors))
printfn "LB-last matched patterns: %i" lastMatches.Length
for _, _, (pattern, _, _, _), algorithms in lastMatches do
    printfn "%s" pattern
    for algorithm in algorithms do
        printfn "%s -> %b" algorithm (Cube.executeSteps (Render.stringToSteps algorithm) lfSolved |> caseFB)
