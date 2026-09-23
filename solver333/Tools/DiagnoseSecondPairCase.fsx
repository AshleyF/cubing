#r "../../library/bin/Release/net8.0/Library.dll"
#r "../bin/Release/net8.0/Solver.dll"

open Cube

let cube = Render.stringToCube "OYOOROGYYROORWRYGGYWBRROYYBBBBROWOGGBBBRYRGGGWBWWYWWGW"
let caseRF = Solver.lookPattern "O.OO.O.....................BBBR.RG..BBBR.RGG.W.WW.WW.."
let caseSB = Solver.lookPattern "O.OO.O.....................BBBR.RG.GBBBR.RGGGW.WW.WW.W"
let centerOriented c = caseSB c && (look Face.U Sticker.C c = Color.W || look Face.U Sticker.C c = Color.Y)
let matching patterns state = patterns |> List.filter (fun (matcher, _, (pattern, auf, colors, _), _) -> matcher state (pattern, auf, colors))
let parse values = if List.isEmpty values then [[]] else values |> List.map Render.stringToSteps
let rfMatches = matching Roux.rfPairFirstIntermediatePatterns cube
printfn "RF-first matches: %i" rfMatches.Length
let rfSolutions = rfMatches |> List.collect (fun (_,_,_,values) -> parse values) |> List.filter (fun alg -> executeSteps alg cube |> caseRF)
printfn "RF-first solving algorithms: %i" rfSolutions.Length
for rf in rfSolutions do
    let state = executeSteps rf cube
    let rbMatches = matching Roux.rbPairLastIntermediatePatterns state
    printfn "RF %s; RB-last matches: %i" (Render.stepsToString rf) rbMatches.Length
    for _,_,_,values in rbMatches do
        for rb in parse values do
            printfn "  RB: %s solved=%b centered=%b" (Render.stepsToString rb) (executeSteps rb state |> caseSB) (executeSteps rb state |> centerOriented)
            let variants =
                rb :: (rb |> List.mapi (fun index step ->
                    let replacement = match step with | Move Move.R -> Some (Move Move.RW) | Move Move.R' -> Some (Move Move.RW') | Move Move.R2 -> Some (Move Move.RW2) | Move Move.RW -> Some (Move Move.R) | Move Move.RW' -> Some (Move Move.R') | Move Move.RW2 -> Some (Move Move.R2) | _ -> None
                    replacement |> Option.map (fun replacement -> rb |> List.mapi (fun i candidate -> if i=index then replacement else candidate))) |> List.choose id)
            for candidate in variants do
                if executeSteps candidate state |> centerOriented then printfn "  CENTERED: %s" (Render.stepsToString candidate)
