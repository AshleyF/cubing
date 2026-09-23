#r "../../library/bin/Release/net8.0/Library.dll"
#r "../bin/Release/net8.0/Solver.dll"

open System
open System.IO
open Cube

type Settings =
    { name: string
      note: string
      cancel: bool
      lb: int; lf: int; fbOrder: bool
      rb: int; rf: int; sbOrder: bool
      co: int; cp: int; cmll: bool
      eo: int; eolr: bool; centers: bool
      colorNeutral: bool }

let beginner =
    { name = "Beginner"; note = "Beginner pairs, Sune/J-perm corners, M/U edge orientation"
      cancel = false; lb = 0; lf = 0; fbOrder = false; rb = 0; rf = 0; sbOrder = false
      co = 0; cp = 0; cmll = false; eo = 0; eolr = false; centers = false; colorNeutral = false }

let configurations =
    [ beginner
      { beginner with name = "+ cancellations"; note = "Adjacent same-slice moves simplified"; cancel = true }
      { beginner with name = "+ direct first-block pairs"; note = "Direct back and front pair cases"; cancel = true; lb = 1; lf = 1 }
      { beginner with name = "+ first-block pair order"; note = "Try both pair orders; choose the shorter pair solution"; cancel = true; lb = 1; lf = 1; fbOrder = true }
      { beginner with name = "+ direct second-block pairs"; note = "Direct back and front pair cases in both blocks"; cancel = true; lb = 1; lf = 1; fbOrder = true; rb = 1; rf = 1 }
      { beginner with name = "+ second-block pair order"; note = "Try both second-block pair orders"; cancel = true; lb = 1; lf = 1; fbOrder = true; rb = 1; rf = 1; sbOrder = true }
      { beginner with name = "+ two-look CMLL"; note = "Direct corner orientation, then direct permutation"; cancel = true; lb = 1; lf = 1; fbOrder = true; rb = 1; rf = 1; sbOrder = true; co = 1; cp = 1 }
      { beginner with name = "+ one-look CMLL"; note = "Full CMLL"; cancel = true; lb = 1; lf = 1; fbOrder = true; rb = 1; rf = 1; sbOrder = true; cmll = true }
      { beginner with name = "+ direct EO"; note = "Direct edge-orientation cases"; cancel = true; lb = 1; lf = 1; fbOrder = true; rb = 1; rf = 1; sbOrder = true; cmll = true; eo = 1 }
      { beginner with name = "+ centers with SB"; note = "Orient centers while finishing second block"; cancel = true; lb = 1; lf = 1; fbOrder = true; rb = 1; rf = 1; sbOrder = true; cmll = true; eo = 1; centers = true }
      { beginner with name = "+ EOLR"; note = "Combined EO/LR, without center optimization"; cancel = true; lb = 1; lf = 1; fbOrder = true; rb = 1; rf = 1; sbOrder = true; cmll = true; eolr = true }
      { beginner with name = "+ EOLR + centers"; note = "Combined EO/LR with centers oriented during SB"; cancel = true; lb = 1; lf = 1; fbOrder = true; rb = 1; rf = 1; sbOrder = true; cmll = true; eolr = true; centers = true }
      { beginner with name = "+ x2/y color neutrality"; note = "Choose the shortest first block among eight orientations"; cancel = true; lb = 1; lf = 1; fbOrder = true; rb = 1; rf = 1; sbOrder = true; cmll = true; eolr = true; centers = true; colorNeutral = true } ]

let moves = [Move.U; U'; U2; Move.D; D'; D2; Move.L; L'; L2; Move.R; R'; R2; Move.F; F'; F2; Move.B; B'; B2]
let random = Random(3332026)
let scrambles =
    List.init 1000 (fun _ ->
        let rec build previous remaining cube =
            if remaining = 0 then cube
            else
                let candidates = moves |> List.filter (fun move -> previous |> Option.forall (fun p -> Render.moveToString p |> Seq.head <> (Render.moveToString move |> Seq.head)))
                let move = candidates[random.Next(candidates.Length)]
                build (Some move) (remaining - 1) (Cube.executeMove cube move)
        build None 25 Cube.solved)

let configure settings =
    Utility.lbPairLevel <- settings.lb; Utility.lfPairLevel <- settings.lf
    Utility.rbPairLevel <- settings.rb; Utility.rfPairLevel <- settings.rf
    Utility.chooseShortestFirstBlockPairOrder <- settings.fbOrder
    Utility.chooseShortestSecondBlockPairOrder <- settings.sbOrder
    Utility.cornerOrientationLevel <- settings.co; Utility.cornerPermutationLevel <- settings.cp
    Utility.fullCmll <- settings.cmll; Utility.edgeOrientationLevel <- settings.eo
    Utility.useEolr <- settings.eolr; Utility.orientCentersWithSecondBlock <- settings.centers
    Utility.x2yColorNeutral <- settings.colorNeutral

let firstBlockStages =
    set [ "DLEdge"; "LCenter"; "TuckLBtoFD"; "BringDLBtoU"; "InsertLBPair"; "TuckLFtoBD"; "BringDLFtoURF"; "InsertLFPair"
          "TuckLFFirsttoBD"; "BringDLFFirsttoURF"; "InsertLFFirstPair"; "TuckLBLasttoFD"; "BringDLBLasttoU"; "InsertLBLastPair" ]

let perCubeTrace count trace =
    [0 .. count - 1]
    |> List.map (fun cubeIndex ->
        trace
        |> List.mapi (fun index item -> index, item)
        |> List.choose (fun (index, item) -> if index % count = cubeIndex then Some item else None))

let solveBatch settings cubes =
    configure settings
    let run candidates =
        Solver.solutionTrace <- []
        Roux.generateFrom candidates |> ignore
        perCubeTrace candidates.Length Solver.solutionTrace
    if not settings.colorNeutral then run cubes
    else
        let orientations = [ ""; "y"; "y2"; "y'"; "x2"; "x2 y"; "x2 y2"; "x2 y'" ] |> List.map (fun a -> if a = "" then [] else Render.stringToSteps a)
        let canonical = [ Face.U, Color.W; Face.D, Color.Y; Face.L, Color.O; Face.R, Color.R; Face.F, Color.G; Face.B, Color.B ]
        let normalize candidate =
            let colorMap = canonical |> List.map (fun (face, color) -> Cube.look face Sticker.C candidate, color) |> Map.ofList
            candidate |> Map.map (fun _ face -> face |> Map.map (fun _ color -> Map.find color colorMap))
        let candidates =
            orientations
            |> List.map (fun orientation ->
                let oriented = cubes |> List.map (fun cube -> cube |> Cube.executeSteps orientation |> normalize)
                orientation, run oriented)
        [0 .. cubes.Length - 1]
        |> List.map (fun cubeIndex ->
            candidates
            |> List.map (fun (orientation, traces) ->
                let trace = traces[cubeIndex]
                let fb = trace |> List.filter (fst >> firstBlockStages.Contains) |> List.sumBy (snd >> List.length)
                orientation, trace, fb)
            |> List.minBy (fun (_, _, fb) -> fb)
            |> fun (orientation, trace, _) -> (if List.isEmpty orientation then [] else ["ColorNeutralOrientation", orientation]) @ trace)

let token (step: Step) = Render.stepsToString [step]
let turn (token: string) =
    let amount = if token.EndsWith("2") then 2 elif token.EndsWith("'") then 3 else 1
    let face = if amount = 1 then token else token.Substring(0, token.Length - 1)
    face, amount
let cancelledCount steps =
    let folder stack step =
        let face, amount = turn (token step)
        match stack with
        | (previousFace, previousAmount) :: tail when previousFace = face ->
            let combined = (previousAmount + amount) % 4
            if combined = 0 then tail else (face, combined) :: tail
        | _ -> (face, amount) :: stack
    steps |> List.fold folder [] |> List.length

let originalOut = Console.Out
let quiet = TextWriter.Null
let requested = Environment.GetEnvironmentVariable("BENCHMARK_ONLY")
let selectedConfigurations = if String.IsNullOrWhiteSpace requested then configurations else configurations |> List.filter (fun settings -> settings.name = requested)
let results =
    selectedConfigurations
    |> List.map (fun settings ->
        originalOut.WriteLine($"Running {settings.name}…")
        Console.SetOut(quiet)
        let counts =
            solveBatch settings scrambles
            |> List.map (fun trace ->
                let steps = trace |> List.collect snd
                if settings.cancel then cancelledCount steps else steps.Length)
        Console.SetOut(originalOut)
        let average = counts |> List.averageBy float
        let ordered = counts |> List.sort
        {| name = settings.name; note = settings.note; average = average
           median = float ordered[ordered.Length / 2]; minimum = List.min counts; maximum = List.max counts |})

let rows =
    results
    |> List.mapi (fun index result ->
        let previous = if index = 0 then result.average else results[index - 1].average
        {| name = result.name; note = result.note; average = result.average; median = result.median
           minimum = result.minimum; maximum = result.maximum; saved = previous - result.average
           totalSaved = results[0].average - result.average |})

let output = Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, "..", "..", "site", "lab", "benchmark-results.json"))
File.WriteAllText(output, System.Text.Json.JsonSerializer.Serialize({| sampleSize = scrambles.Length; seed = 3332026; rows = rows |}))
printfn "Wrote %s" output
