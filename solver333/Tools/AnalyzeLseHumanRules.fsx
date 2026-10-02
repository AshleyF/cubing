#r "../../library/bin/Release/net8.0/Library.dll"

open System
open System.IO
open System.Collections.Generic
open Lse

type State =
    { Pieces: int array
      Flips: int array
      Center: int
      Auf: int
      Optimal: int }

type Feature =
    { Name: string
      Value: State -> int
      Describe: int -> string }

type Rule =
    { Conditions: (int * int) list
      Move: int
      Members: int array }

let pieceNames = [| "UL"; "UR"; "UF"; "UB"; "DF"; "DB" |]
let moveNames = [| "M"; "M'"; "M2"; "U"; "U'"; "U2" |]
let offsetNames = [| "solved"; "quarter"; "half"; "inverse-quarter" |]

let permutationOfRank rank =
    let factorial = [| 1; 1; 2; 6; 24; 120; 720 |]
    let available = ResizeArray<int>([0 .. 5])
    let permutation = Array.zeroCreate 6
    let mutable remaining = rank
    for position in 0 .. 5 do
        let block = factorial[5 - position]
        let digit = remaining / block
        remaining <- remaining % block
        permutation[position] <- available[digit]
        available.RemoveAt digit
    permutation

let decode index optimal =
    let auf = index % 4
    let centerAndAbove = index / 4
    let center = centerAndAbove % 4
    let flipAndAbove = centerAndAbove / 4
    let flipRank = flipAndAbove % 32
    let flips = Array.zeroCreate 6
    let mutable parity = 0
    for bit in 0 .. 4 do
        flips[bit] <- (flipRank >>> bit) &&& 1
        parity <- parity ^^^ flips[bit]
    flips[5] <- parity
    { Pieces = permutationOfRank (flipAndAbove / 32)
      Flips = flips
      Center = center
      Auf = auf
      Optimal = optimal }

let positionOf piece (state: State) = state.Pieces |> Array.findIndex ((=) piece)
let orientationOf piece state = state.Flips[positionOf piece state]
let eoMask state = state.Flips |> Array.mapi (fun position flip -> flip <<< position) |> Array.sum
let solvedCount state = state.Pieces |> Array.mapi (fun position piece -> if position = piece then 1 else 0) |> Array.sum
let permutationCycles state =
    let seen = Array.zeroCreate<bool> 6
    [ for start in 0 .. 5 do
          if not seen[start] then
              let mutable length = 0
              let mutable position = start
              while not seen[position] do
                  seen[position] <- true
                  length <- length + 1
                  position <- state.Pieces[position]
              yield length ]
    |> List.sortDescending
let cycleType state = permutationCycles state |> List.fold (fun value length -> value * 7 + length) 0
let cycleTypeName value =
    let rec decode remaining lengths =
        if remaining = 0 then lengths else decode (remaining / 7) ((remaining % 7) :: lengths)
    decode value [] |> List.map string |> String.concat "+"
let uRing = [| 0; 3; 1; 2 |] // UL, UB, UR, UF
let uRingIndex position = uRing |> Array.tryFindIndex ((=) position)
let lrRelation state =
    let left, right = positionOf 0 state, positionOf 1 state
    match uRingIndex left, uRingIndex right with
    | Some l, Some r when (abs (l - r) = 2) -> 3
    | Some _, Some _ -> 2
    | None, None -> 1
    | _ -> 0
let eoFamily state =
    let top = state.Flips[0..3] |> Array.sum
    let bottom = state.Flips[4..5] |> Array.sum
    let flippedTop = [0..3] |> List.filter (fun position -> state.Flips[position] = 1)
    let opposite =
        match flippedTop |> List.choose uRingIndex with
        | [left; right] -> abs (left - right) = 2
        | _ -> false
    match top, bottom with
    | 0, 0 -> 0 // solved EO
    | 3, 1 -> 1 // 3/1
    | 4, 0 -> 2 // 4/0
    | 2, 2 when opposite -> 3 // 2o/2
    | 2, 2 -> 4 // 2a/2
    | 1, 1 -> 5 // 1/1
    | 2, 0 when opposite -> 6 // 2o/0
    | 2, 0 -> 7 // 2a/0
    | 0, 2 -> 8 // 0/2
    | 4, 2 -> 9 // 4/2
    | _ -> failwith $"Unclassified legal EO shape: {state.Flips}"
let lrRole position flip =
    match position with
    | 0 | 1 -> flip // oriented/flipped U-side
    | 2 | 3 -> 2 + flip // oriented/flipped U M-slice
    | 4 | 5 -> 4 + flip // oriented/flipped D
    | _ -> failwith "Invalid LSE edge position."
let lrRolePair state =
    let roles =
        [0; 1]
        |> List.map (fun piece ->
            let position = positionOf piece state
            lrRole position state.Flips[position])
        |> List.sort
    roles[0] * 6 + roles[1]
let lrRolePairName value =
    let names = [| "oriented U-side"; "flipped U-side"; "oriented U-M"; "flipped U-M"; "oriented D"; "flipped D" |]
    $"{names[value / 6]} + {names[value % 6]}"
let solvedLrCount state = [0; 1] |> List.sumBy (fun piece -> if positionOf piece state = piece then 1 else 0)
let solvedMCount state = [2; 3; 4; 5] |> List.sumBy (fun piece -> if positionOf piece state = piece then 1 else 0)
let mSliceOccupancy state =
    [2; 3; 4; 5]
    |> List.sumBy (fun piece -> if positionOf piece state >= 2 then 1 else 0)
let orientedSolvedCount state =
    [0 .. 5]
    |> List.sumBy (fun piece ->
        let position = positionOf piece state
        if position = piece && state.Flips[position] = 0 then 1 else 0)
let nameFrom (names: string array) (value: int) = names[value]
let binary (zero: string) (one: string) (value: int) = if value = 0 then zero else one
let feature (name: string) (value: State -> int) (describe: int -> string) =
    { Name = name; Value = value; Describe = describe }

let features =
    [ feature "EO shape" eoMask (fun mask ->
          [ for position in 0 .. 5 do
                if mask &&& (1 <<< position) <> 0 then yield pieceNames[position] ]
          |> function
             | [] -> "oriented"
             | positions -> String.Join("+", positions) + " flipped")
      feature "flipped edge count" (fun state -> Array.sum state.Flips) string
      feature "flipped U-edge count" (fun state -> state.Flips[0..3] |> Array.sum) string
      feature "flipped D-edge count" (fun state -> state.Flips[4..5] |> Array.sum) string
      feature "center" (fun state -> state.Center) (nameFrom offsetNames)
      feature "corner AUF" (fun state -> state.Auf) (nameFrom offsetNames)
      feature "UL position" (positionOf 0) (nameFrom pieceNames)
      feature "UR position" (positionOf 1) (nameFrom pieceNames)
      feature "UL orientation" (orientationOf 0) (binary "oriented" "flipped")
      feature "UR orientation" (orientationOf 1) (binary "oriented" "flipped")
      feature "LR layer relation" (fun state ->
          let left = positionOf 0 state
          let right = positionOf 1 state
          if (left < 4) <> (right < 4) then 0
          elif left >= 4 then 1
          else 2) (nameFrom [| "split"; "both D"; "both U" |])
      feature "LR geometric relation" lrRelation (nameFrom [| "split"; "both D"; "adjacent on U"; "opposite on U" |])
      feature "EOLR EO family" eoFamily (nameFrom [| "0/0"; "3/1"; "4/0"; "2o/2"; "2a/2"; "1/1"; "2o/0"; "2a/0"; "0/2"; "4/2" |])
      feature "LR role pair" lrRolePair lrRolePairName
      feature "permutation cycle type" cycleType cycleTypeName
      feature "permutation cycle count" (permutationCycles >> List.length) string
      feature "solved LR count" solvedLrCount string
      feature "solved M-edge count" solvedMCount string
      feature "L4 pieces in M-slice slots" mSliceOccupancy string
      feature "oriented and solved edge count" orientedSolvedCount string
      feature "piece at UL" (fun state -> state.Pieces[0]) (nameFrom pieceNames)
      feature "piece at UR" (fun state -> state.Pieces[1]) (nameFrom pieceNames)
      feature "piece at UF" (fun state -> state.Pieces[2]) (nameFrom pieceNames)
      feature "piece at UB" (fun state -> state.Pieces[3]) (nameFrom pieceNames)
      feature "piece at DF" (fun state -> state.Pieces[4]) (nameFrom pieceNames)
      feature "piece at DB" (fun state -> state.Pieces[5]) (nameFrom pieceNames)
      feature "UL slot orientation" (fun state -> state.Flips[0]) (binary "oriented" "flipped")
      feature "UR slot orientation" (fun state -> state.Flips[1]) (binary "oriented" "flipped")
      feature "UF slot orientation" (fun state -> state.Flips[2]) (binary "oriented" "flipped")
      feature "UB slot orientation" (fun state -> state.Flips[3]) (binary "oriented" "flipped")
      feature "DF slot orientation" (fun state -> state.Flips[4]) (binary "oriented" "flipped")
      feature "solved-edge count" solvedCount string ]
    |> List.toArray

let policyPath = Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, "..", "Data", "lse-policy-v1.dat"))
let policy = loadPolicy policyPath
let states =
    [| for index in 0 .. policy.Distances.Length - 1 do
           if policy.Distances[index] <> unreachable && policy.Distances[index] <> 0uy then
               yield decode index (int policy.OptimalMoves[index]) |]

let commonMove (members: int array) =
    let mutable mask = 0x3F
    for stateIndex in members do mask <- mask &&& states[stateIndex].Optimal
    if mask = 0 then None
    else Some ([0 .. 5] |> List.find (fun move -> mask &&& (1 <<< move) <> 0))

let support (members: int array) =
    let counts = Array.zeroCreate 6
    for stateIndex in members do
        for move in 0 .. 5 do
            if states[stateIndex].Optimal &&& (1 <<< move) <> 0 then counts[move] <- counts[move] + 1
    Array.max counts

let partition featureIndex (members: int array) =
    let groups = Dictionary<int, ResizeArray<int>>()
    for stateIndex in members do
        let value = features[featureIndex].Value states[stateIndex]
        match groups.TryGetValue value with
        | true, group -> group.Add stateIndex
        | _ ->
            let group = ResizeArray<int>()
            group.Add stateIndex
            groups[value] <- group
    groups |> Seq.map (fun pair -> pair.Key, pair.Value.ToArray()) |> Seq.toArray

let rules = ResizeArray<Rule>()
let deadEnds = ResizeArray<int array>()
let maximumConditions = 6

let rec build conditions available (members: int array) =
    match commonMove members with
    | Some move -> rules.Add { Conditions = List.rev conditions; Move = move; Members = members }
    | None when List.isEmpty available || conditions.Length >= maximumConditions -> deadEnds.Add members
    | None ->
        let featureIndex, groups, _, _ =
            available
            |> List.map (fun featureIndex ->
                let groups = partition featureIndex members
                let score = groups |> Array.sumBy (snd >> support)
                let pureCoverage =
                    groups
                    |> Array.sumBy (fun (_, group) -> if commonMove group |> Option.isSome then group.Length else 0)
                featureIndex, groups, pureCoverage, score)
            |> List.maxBy (fun (_, groups, pureCoverage, score) -> pureCoverage, score, -groups.Length)
        let remaining = available |> List.filter ((<>) featureIndex)
        for value, group in groups do
            build ((featureIndex, value) :: conditions) remaining group

build [] [0 .. features.Length - 1] [| 0 .. states.Length - 1 |]

let treeRules = rules |> Seq.sortByDescending (fun rule -> rule.Members.Length) |> Seq.toArray

let rec combinations count source =
    match count, source with
    | 0, _ -> [ [] ]
    | _, [] -> []
    | count, head :: tail ->
        (combinations (count - 1) tail |> List.map (fun rest -> head :: rest)) @ combinations count tail

let candidateFeatureSets =
    let all = [0 .. features.Length - 1]
    let highLevel = [0 .. 19] @ [features.Length - 1]
    (combinations 1 all) @ (combinations 2 all) @ (combinations 3 highLevel)

let mine featureIndices =
    let groups = Dictionary<int64, ResizeArray<int>>()
    let values = Dictionary<int64, int list>()
    for stateIndex in 0 .. states.Length - 1 do
        let featureValues = featureIndices |> List.map (fun featureIndex -> features[featureIndex].Value states[stateIndex])
        let key = featureValues |> List.fold (fun key value -> key * 64L + int64 value) 0L
        match groups.TryGetValue key with
        | true, group -> group.Add stateIndex
        | _ ->
            let group = ResizeArray<int>()
            group.Add stateIndex
            groups[key] <- group
            values[key] <- featureValues
    [| for pair in groups do
           let members = pair.Value.ToArray()
           match commonMove members with
           | Some move ->
               let conditions = List.zip featureIndices values[pair.Key]
               yield { Conditions = conditions; Move = move; Members = members }
           | None -> () |]

let candidates =
    candidateFeatureSets
    |> List.collect (mine >> Array.toList)
    |> List.sortByDescending (fun rule -> rule.Members.Length)

// Keep the ranking deliberately simple and auditable: consider broader rules
// first, retain a rule only when it adds at least one previously uncovered
// state, and report actual union coverage rather than summing overlaps.
let covered = Array.zeroCreate<bool> states.Length
let selected = ResizeArray<Rule * int>()
for candidate in candidates do
    if selected.Count < 1000 then
        let gain = candidate.Members |> Array.sumBy (fun stateIndex -> if covered[stateIndex] then 0 else 1)
        if gain > 0 then
            selected.Add(candidate, gain)
            for stateIndex in candidate.Members do covered[stateIndex] <- true

let checkpoints = [| 10; 25; 50; 100; 200; 300; 500; 1000 |]
let coverage count = selected |> Seq.truncate count |> Seq.sumBy snd
let treeCoverage count = treeRules |> Seq.truncate count |> Seq.sumBy (fun rule -> rule.Members.Length)

for rule, _ in selected do
    for stateIndex in rule.Members do
        if states[stateIndex].Optimal &&& (1 <<< rule.Move) = 0 then
            failwith $"Rule validation failed for state {stateIndex}."

let describeRule number rule =
    let conditions =
        rule.Conditions
        |> List.map (fun (featureIndex, value) ->
            $"{features[featureIndex].Name} = {features[featureIndex].Describe value}")
        |> String.concat "; "
    $"{number}. **{moveNames[rule.Move]}** when {conditions} — {rule.Members.Length:N0} states"

let reportPath = Path.Combine(__SOURCE_DIRECTORY__, "LseHumanRulesResults.md")
let report = ResizeArray<string>()
let eoFamilyNames = [| "0/0"; "3/1"; "4/0"; "2o/2"; "2a/2"; "1/1"; "2o/0"; "2a/0"; "0/2"; "4/2" |]
report.Add "# Baseline exact LSE rule-mining results"
report.Add ""
report.Add "This baseline uses both a categorical decision tree and overlapping conjunctions of human-oriented state features. Every counted rule selects a move that is optimal for every matched state. Impure groups are not counted as covered."
report.Add ""
report.Add $"- Reachable non-solved states: {states.Length:N0}"
report.Add $"- Exact pure decision-tree leaves: {treeRules.Length:N0}"
report.Add $"- Unresolved leaves after all current features: {deadEnds.Count:N0}"
report.Add $"- Exact one-, two-, and selected three-condition candidates: {candidates.Length:N0}"
report.Add ""
report.Add "## EOLR family coverage"
report.Add ""
report.Add "The nine unsolved EO families from the EOLR guide plus solved EO classify every reachable LSE state:"
report.Add ""
report.Add "| EO family | States |"
report.Add "| --- | ---: |"
states
|> Array.countBy eoFamily
|> Array.sortBy fst
|> Array.iter (fun (family, count) -> report.Add(sprintf "| %s | %s |" eoFamilyNames[family] (count.ToString("N0"))))
report.Add ""
report.Add "## Coverage by largest purity-first decision-tree leaves"
report.Add ""
report.Add "| Rules | States | Coverage |"
report.Add "| ---: | ---: | ---: |"
for count in checkpoints do
    let covered = treeCoverage count
    let percentage = 100.0 * float covered / float states.Length
    report.Add(sprintf "| %s | %s | %.2f%% |" (count.ToString("N0")) (covered.ToString("N0")) percentage)
report.Add ""
report.Add "## Coverage by size-ranked overlapping conjunctions"
report.Add ""
report.Add "| Rules | Newly covered states | Coverage |"
report.Add "| ---: | ---: | ---: |"
for count in checkpoints do
    let covered = coverage count
    let percentage = 100.0 * float covered / float states.Length
    report.Add(sprintf "| %s | %s | %.2f%% |" (count.ToString("N0")) (covered.ToString("N0")) percentage)
report.Add ""
report.Add "## Thirty largest conjunction rules"
report.Add ""
selected |> Seq.truncate 30 |> Seq.iteri (fun index (rule, _) -> report.Add(describeRule (index + 1) rule))
report.Add ""
report.Add "These rules are a measurement baseline, not yet a polished teaching system. The next pass should merge mirrors, remove redundant conditions, and add relational features for bars, cycles, adjacency, and opposites."
File.WriteAllLines(reportPath, report)

printfn "Analyzed %i reachable non-solved states." states.Length
printfn "Found %i exact tree leaves, %i dead ends, and %i exact conjunction candidates." treeRules.Length deadEnds.Count candidates.Length
for count in checkpoints do
    let covered = coverage count
    printfn "%4i rules: %7i states (%6.2f%%)" count covered (100.0 * float covered / float states.Length)
printfn "Wrote %s" reportPath
