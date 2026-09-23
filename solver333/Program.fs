open System
open System.Text.Json

let args = Environment.GetCommandLineArgs() |> Array.skip 1

if args.Length > 0 && args[0] = "--patterns" then
    let cases patterns =
        patterns
        |> List.map (fun (_, _, (pattern, _, _, _), algs) ->
            {| pattern = pattern
               action = if List.isEmpty algs then "skip — already satisfied" else String.Join(" / ", algs) |})
    let sets = [
        {| id = "dl"; cases = cases Roux.dlEdgeBeginnerPatterns |}
        {| id = "lc"; cases = cases Roux.lCenterBeginnerPatterns |}
        {| id = "lb"; cases = cases Roux.lbPairBeginnerPatterns |}
        {| id = "lb2"; cases = cases Roux.lbPairIntermediatePatterns |}
        {| id = "lf"; cases = cases Roux.lfPairBeginnerPatterns |}
        {| id = "lf2"; cases = cases Roux.lfPairIntermediatePatterns |}
        {| id = "lfFirst2"; cases = cases Roux.lfPairFirstIntermediatePatterns |}
        {| id = "lbLast2"; cases = cases Roux.lbPairLastIntermediatePatterns |}
        {| id = "dr"; cases = cases Roux.drEdgeBeginnerPatterns |}
        {| id = "rb"; cases = cases Roux.rbPairBeginnerPatterns |}
        {| id = "rb2"; cases = cases Roux.rbPairIntermediatePatterns |}
        {| id = "rf"; cases = cases Roux.rfPairBeginnerPatterns |}
        {| id = "rf2"; cases = cases Roux.rfPairIntermediatePatterns |}
        {| id = "rfFirst2"; cases = cases Roux.rfPairFirstIntermediatePatterns |}
        {| id = "rbLast2"; cases = cases Roux.rbPairLastIntermediatePatterns |}
        {| id = "co"; cases = cases Roux.coBeginnerPatterns |}
        {| id = "co2"; cases = cases Roux.coIntermediatePatterns |}
        {| id = "cp"; cases = cases Roux.cpBeginnerPatterns |}
        {| id = "cp2"; cases = cases Roux.cpIntermediatePatterns |}
        {| id = "cmll"; cases = cases (Roux.cmllAdvancedPatterns |> List.filter (fun (_, stage, _, _) -> stage = "CornerOrientation")) |}
        {| id = "eo"; cases = cases Roux.edgeBeginnerOrientationPatters |}
        {| id = "eo2"; cases = cases Roux.edgeIntermediateOrientationPatters |}
        {| id = "lr"; cases = cases Roux.lrBeginnerPatterns |}
        {| id = "l4e"; cases = cases Roux.l4eBeginnerPatterns |}
    ]
    printfn "PATTERN_RESULT|%s" (JsonSerializer.Serialize(sets))
elif args.Length > 0 then
    try
        let scramble = args[0]
        Utility.cornerOrientationLevel <- if args |> Array.contains "--co=1" then 1 else 0
        Utility.cornerPermutationLevel <- if args |> Array.contains "--cp=1" then 1 else 0
        Utility.fullCmll <- args |> Array.contains "--cmll=1"
        Utility.edgeOrientationLevel <- if args |> Array.contains "--eo=1" then 1 else 0
        Utility.useEolr <- args |> Array.contains "--eo=2"
        Utility.lbPairLevel <- if args |> Array.contains "--lb=1" then 1 else 0
        Utility.lfPairLevel <- if args |> Array.contains "--lf=1" then 1 else 0
        Utility.rbPairLevel <- if args |> Array.contains "--rb=1" then 1 else 0
        Utility.rfPairLevel <- if args |> Array.contains "--rf=1" then 1 else 0
        Utility.orientCentersWithSecondBlock <- args |> Array.contains "--center-with-sb=1"
        Utility.chooseShortestSecondBlockPairOrder <- args |> Array.contains "--best-sb-order=1"
        Utility.chooseShortestFirstBlockPairOrder <- args |> Array.contains "--best-fb-order=1"
        Utility.x2yColorNeutral <- args |> Array.contains "--x2y-color-neutral=1"
        let scrambled = scramble |> Render.stringToSteps |> fun steps -> Cube.executeSteps steps Cube.solved
        let solveOne cube =
            Solver.solutionTrace <- []
            Roux.generateFrom [cube]
            Solver.solutionTrace
        let trace =
            if not Utility.x2yColorNeutral then solveOne scrambled
            else
                let orientations =
                    [ ""; "y"; "y2"; "y'"; "x2"; "x2 y"; "x2 y2"; "x2 y'" ]
                    |> List.map (fun algorithm -> if algorithm = "" then [] else Render.stringToSteps algorithm)
                let canonicalCenters =
                    [ Cube.Face.U, Cube.Color.W; Cube.Face.D, Cube.Color.Y
                      Cube.Face.L, Cube.Color.O; Cube.Face.R, Cube.Color.R
                      Cube.Face.F, Cube.Color.G; Cube.Face.B, Cube.Color.B ]
                let normalize cube =
                    let colorMap = canonicalCenters |> List.map (fun (face, canonical) -> Cube.look face Cube.Sticker.C cube, canonical) |> Map.ofList
                    cube |> Map.map (fun _ face -> face |> Map.map (fun _ color -> Map.find color colorMap))
                let firstBlockStages =
                    set [ "DLEdge"; "LCenter"; "TuckLBtoFD"; "BringDLBtoU"; "InsertLBPair"; "TuckLFtoBD"; "BringDLFtoURF"; "InsertLFPair"
                          "TuckLFFirsttoBD"; "BringDLFFirsttoURF"; "InsertLFFirstPair"; "TuckLBLasttoFD"; "BringDLBLasttoU"; "InsertLBLastPair" ]
                orientations
                |> List.map (fun orientation ->
                    let candidateTrace = scrambled |> Cube.executeSteps orientation |> normalize |> solveOne
                    let firstBlockMoves = candidateTrace |> List.filter (fst >> firstBlockStages.Contains) |> List.sumBy (snd >> List.length)
                    orientation, candidateTrace, firstBlockMoves)
                |> List.minBy (fun (_, _, moves) -> moves)
                |> fun (orientation, candidateTrace, _) ->
                    (if List.isEmpty orientation then [] else ["ColorNeutralOrientation", orientation]) @ candidateTrace
        Solver.solutionTrace <- trace
        let stages =
            Solver.solutionTrace
            |> List.map (fun (stage, steps) -> {| stage = stage; moves = Render.stepsToString steps |})
        let solution = Solver.solutionTrace |> List.collect snd |> Render.stepsToString
        printfn "ROUX_RESULT|%s" (JsonSerializer.Serialize({| solution = solution; stages = stages |}))
    with error ->
        printfn "ROUX_ERROR|%s" error.Message
        Environment.ExitCode <- 1
else
    let numCubes = 10
    Roux.generate numCubes
    let avgTwistCount = float Cube.twistCount / float numCubes
    printfn "Total Average Twists (STM): %f" avgTwistCount
