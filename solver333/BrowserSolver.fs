module BrowserSolver

open Cube

[<CLIMutable>]
type Config =
    { co: int; cp: int; cmll: int; eo: int
      lb: int; lf: int; rb: int; rf: int
      center: int; fbOrder: int; sbOrder: int; colorNeutral: int }

[<CLIMutable>]
type StageResult = { stage: string; moves: string }

[<CLIMutable>]
type SolveResult = { solution: string; stages: StageResult array }

let setPatterns (keys: string array) (values: string array array) = PatternData.setData keys values

let solve (scramble: string) (config: Config) =
    Utility.cornerOrientationLevel <- config.co
    Utility.cornerPermutationLevel <- config.cp
    Utility.fullCmll <- config.cmll = 1
    Utility.edgeOrientationLevel <- if config.eo = 1 then 1 else 0
    Utility.useEolr <- config.eo = 2
    Utility.lbPairLevel <- config.lb
    Utility.lfPairLevel <- config.lf
    Utility.rbPairLevel <- config.rb
    Utility.rfPairLevel <- config.rf
    Utility.orientCentersWithSecondBlock <- config.center = 1
    Utility.chooseShortestFirstBlockPairOrder <- config.fbOrder = 1
    Utility.chooseShortestSecondBlockPairOrder <- config.sbOrder = 1
    Utility.x2yColorNeutral <- config.colorNeutral = 1

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
                [ Face.U, Color.W; Face.D, Color.Y; Face.L, Color.O
                  Face.R, Color.R; Face.F, Color.G; Face.B, Color.B ]
            let normalize cube =
                let colorMap = canonicalCenters |> List.map (fun (face, canonical) -> Cube.look face Sticker.C cube, canonical) |> Map.ofList
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
    { solution = trace |> List.collect snd |> Render.stepsToString
      stages = trace |> List.map (fun (stage, steps) -> { stage = stage; moves = Render.stepsToString steps }) |> List.toArray }
