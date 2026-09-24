module BrowserSolver

open Cube

[<CLIMutable>]
type Config =
    { co: int; cp: int; cmll: int; eo: int
      lb: int; lf: int; rb: int; rf: int
      center: int; fbOrder: int; sbOrder: int; colorNeutral: int; lse: int }

[<CLIMutable>]
type StageResult = { stage: string; moves: string }

[<CLIMutable>]
type SolveResult = { solution: string; stages: StageResult array }

exception FirstBlockComplete

let setPatterns (keys: string array) (values: string array array) = PatternData.setData keys values
let setLsePolicy (distances: byte array) (optimalMoves: byte array) reachable maximum =
    Lse.installPolicy distances optimalMoves reachable maximum

let private resultFromTrace trace =
    { solution = trace |> List.collect snd |> Render.stepsToString
      stages = trace |> List.map (fun (stage, steps) -> { stage = stage; moves = Render.stepsToString steps }) |> List.toArray }

let solveWithProgress (scramble: string) (config: Config) (progress: string -> SolveResult -> unit) =
    Utility.cornerOrientationLevel <- config.co
    Utility.cornerPermutationLevel <- config.cp
    Utility.fullCmll <- config.cmll = 1
    Utility.edgeOrientationLevel <- if config.eo = 1 then 1 else 0
    Utility.useEolr <- config.eo = 2
    Utility.useOptimalLse <- config.lse = 1
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
        if not Utility.x2yColorNeutral then
            Roux.progressCallback <- fun milestone -> progress milestone (resultFromTrace Solver.solutionTrace)
            solveOne scrambled
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
                set [ "ColorNeutralOrientation"; "DLEdge"; "LCenter"; "TuckLBtoFD"; "BringDLBtoU"; "InsertLBPair"; "TuckLFtoBD"; "BringDLFtoURF"; "InsertLFPair"
                      "TuckLFFirsttoBD"; "BringDLFFirsttoURF"; "InsertLFFirstPair"; "TuckLBLasttoFD"; "BringDLBLasttoU"; "InsertLBLastPair" ]
            let solveFirstBlock cube =
                let mutable firstBlockTrace = []
                Solver.solutionTrace <- []
                Roux.progressCallback <- fun milestone ->
                    if milestone = "First block" then
                        firstBlockTrace <- Solver.solutionTrace
                        raise FirstBlockComplete
                try Roux.generateFrom [cube] with FirstBlockComplete -> ()
                firstBlockTrace
            let orientation, chosenCube, chosenFirstBlock, _ =
                orientations
                |> List.mapi (fun index orientation ->
                    let candidateCube = scrambled |> Cube.executeSteps orientation |> normalize
                    let candidateTrace = solveFirstBlock candidateCube
                    let displayTrace = (if List.isEmpty orientation then [] else ["ColorNeutralOrientation", orientation]) @ candidateTrace
                    progress $"Inspection {index + 1}/8" (resultFromTrace displayTrace)
                    let firstBlockMoves = candidateTrace |> List.filter (fst >> firstBlockStages.Contains) |> List.sumBy (snd >> List.length)
                    orientation, candidateCube, candidateTrace, firstBlockMoves)
                |> List.minBy (fun (_, _, _, moves) -> moves)
            let orientationTrace = if List.isEmpty orientation then [] else ["ColorNeutralOrientation", orientation]
            progress "First block" (resultFromTrace (orientationTrace @ chosenFirstBlock))
            Roux.progressCallback <- fun milestone ->
                if milestone <> "First block" then progress milestone (resultFromTrace (orientationTrace @ Solver.solutionTrace))
            orientationTrace @ solveOne chosenCube

    Solver.solutionTrace <- trace
    Roux.progressCallback <- ignore
    resultFromTrace trace

let solve (scramble: string) (config: Config) = solveWithProgress scramble config (fun _ _ -> ())
