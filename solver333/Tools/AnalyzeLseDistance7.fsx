#r "../../library/bin/Release/net8.0/Library.dll"

open System
open System.IO
open System.Collections.Generic
open Lse

type State =
    { Index: int
      Pieces: int array
      Flips: int array
      Center: int
      Auf: int
      Distance: int }

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

let decode index distance =
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
    { Index = index
      Pieces = permutationOfRank (flipAndAbove / 32)
      Flips = flips
      Center = center
      Auf = auf
      Distance = distance }

let positionOf piece state = state.Pieces |> Array.findIndex ((=) piece)
let uRing = [| 0; 3; 1; 2 |]
let ringIndex position = uRing |> Array.tryFindIndex ((=) position)
let adjacent left right =
    match ringIndex left, ringIndex right with
    | Some l, Some r -> let gap = abs (l - r) in gap = 1 || gap = 3
    | _ -> false
let opposite left right =
    match ringIndex left, ringIndex right with
    | Some l, Some r -> abs (l - r) = 2
    | _ -> false

let eoShape state =
    let top = state.Flips[0..3] |> Array.sum
    let bottom = state.Flips[4..5] |> Array.sum
    let flippedTop = [0..3] |> List.filter (fun position -> state.Flips[position] = 1)
    let oppositeFlips =
        match flippedTop |> List.choose ringIndex with
        | [left; right] -> abs (left - right) = 2
        | _ -> false
    match top, bottom with
    | 0, 0 -> "0/0"
    | 3, 1 -> "3/1"
    | 4, 0 -> "4/0"
    | 2, 2 when oppositeFlips -> "2o/2"
    | 2, 2 -> "2a/2"
    | 1, 1 -> "1/1"
    | 2, 0 when oppositeFlips -> "2o/0"
    | 2, 0 -> "2a/0"
    | 0, 2 -> "0/2"
    | 4, 2 -> "4/2"
    | _ -> failwith "invalid EO"

let lrPicture state =
    let positions = [| positionOf 0 state; positionOf 1 state |]
    let onD = positions |> Array.filter (fun position -> position >= 4) |> Array.length
    let flipped = positions |> Array.sumBy (fun position -> state.Flips[position])
    let placement =
        if onD = 2 then "both on D"
        elif onD = 1 then "split U/D"
        elif opposite positions[0] positions[1] then "opposite on U"
        elif adjacent positions[0] positions[1] then "adjacent on U"
        else "both in U/M"
    $"{placement}; {flipped} LR flipped"

let lrPlacement state =
    let positions = [| positionOf 0 state; positionOf 1 state |]
    let onD = positions |> Array.filter (fun position -> position >= 4) |> Array.length
    if onD = 2 then "both on D"
    elif onD = 1 then "split U/D"
    elif opposite positions[0] positions[1] then "opposite on U"
    elif adjacent positions[0] positions[1] then "adjacent on U"
    else "both in U/M"

let lrFlipped state =
    [| positionOf 0 state; positionOf 1 state |]
    |> Array.sumBy (fun position -> state.Flips[position])

let correctPairs state =
    // A visible two-sticker bar: a piece occupies its home slot with its
    // orientation matching that slot.  Count separately on U and the M orbit.
    let solved position = state.Pieces[position] = position && state.Flips[position] = 0
    let top = [0..3] |> List.filter solved |> List.length
    let bottom = [4..5] |> List.filter solved |> List.length
    top, bottom

let solvedTopMask state =
    [0..3]
    |> List.fold (fun mask position ->
        if state.Pieces[position] = position && state.Flips[position] = 0 then mask ||| (1 <<< position)
        else mask) 0

let centerDelta state = (state.Center - state.Auf + 4) % 4

let policyPath = Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, "..", "Data", "lse-policy-v1.dat"))
let policy = loadPolicy policyPath
let states =
    Array.init policy.Distances.Length (fun index ->
        let distance = policy.Distances[index]
        if distance = unreachable then None else Some(decode index (int distance)))
let stateAt index = states[index] |> Option.get
let moveNames = [| "M"; "M'"; "M2'"; "U"; "U'"; "U2'" |]
let inverseMove = [| 1; 0; 2; 4; 3; 5 |]

let solutionStartingWith firstMove source =
    let moves = ResizeArray<int>()
    moves.Add firstMove
    let mutable current = nextIndex firstMove source.Index
    while policy.Distances[current] <> 0uy do
        let mask = int policy.OptimalMoves[current]
        let move = [0..5] |> List.find (fun candidate -> mask &&& (1 <<< candidate) <> 0)
        moves.Add move
        current <- nextIndex move current
    moves |> Seq.toArray

let distance7 =
    states
    |> Array.choose id
    |> Array.filter (fun state -> state.Distance = 7)

type Transition =
    { Source: State
      Move: int
      Target: State }

let transitions =
    [| for source in distance7 do
           let mask = int policy.OptimalMoves[source.Index]
           for move in 0 .. 5 do
               if mask &&& (1 <<< move) <> 0 then
                   yield { Source = source; Move = move; Target = stateAt (nextIndex move source.Index) } |]

let targetKey transition =
    let topBars, bottomBars = correctPairs transition.Target
    eoShape transition.Target,
    lrPicture transition.Target,
    centerDelta transition.Target,
    topBars,
    bottomBars

printfn "distance-7 states: %d; exact optimal transitions: %d" distance7.Length transitions.Length
if false then
    printfn "\nLargest exact target pictures after one move:"
    transitions
    |> Array.groupBy targetKey
    |> Array.sortByDescending (fun (_, members) -> members.Length)
    |> Array.truncate 40
    |> Array.iter (fun ((eo, lr, center, topBars, bottomBars), members) ->
        let distinctSources = members |> Array.map (fun transition -> transition.Source.Index) |> Array.distinct |> Array.length
        let moves = members |> Array.countBy (fun transition -> moveNames[transition.Move]) |> Array.sortByDescending snd |> Array.map (fun (move, count) -> $"{move}:{count}") |> String.concat " "
        printfn "%4d sources / %4d transitions | EO %-4s | %-28s | center %d | solved U %d D %d | %s" distinctSources members.Length eo lr center topBars bottomBars moves)

// Mine rules of the human form:
//   "In visible source family S, turn one axis until visible target T appears."
// A rule is emitted only when every move producing T is an exact optimal move.
// This is deliberately stricter than merely observing that one example works.
type Rule =
    { SourceEo: string option
      SourceLr: string option
      SourceLrFlips: int option
      Axis: char
      TargetEo: string option
      TargetLr: string option
      TargetLrFlips: int option
      TargetCenterDelta: int option
      TargetSolvedTop: int option
      TargetSolvedTopMask: int option
      Covered: Set<int> }

let optionChoices values = Array.append [| None |] (values |> Array.distinct |> Array.map Some)
let eoValues = distance7 |> Array.map eoShape
let lrValues = distance7 |> Array.map lrPlacement
let flipValues = distance7 |> Array.map lrFlipped
let centerValues = [| 0; 1; 2; 3 |]

let sourceMatches rule state =
    rule.SourceEo |> Option.forall ((=) (eoShape state))
    && rule.SourceLr |> Option.forall ((=) (lrPlacement state))
    && rule.SourceLrFlips |> Option.forall ((=) (lrFlipped state))

let targetMatches rule state =
    rule.TargetEo |> Option.forall ((=) (eoShape state))
    && rule.TargetLr |> Option.forall ((=) (lrPlacement state))
    && rule.TargetLrFlips |> Option.forall ((=) (lrFlipped state))
    && rule.TargetCenterDelta |> Option.forall ((=) (centerDelta state))
    && rule.TargetSolvedTop |> Option.forall ((=) (fst (correctPairs state)))
    && rule.TargetSolvedTopMask |> Option.forall ((=) (solvedTopMask state))

let movesOn axis = if axis = 'M' then [| 0; 1; 2 |] else [| 3; 4; 5 |]

let evaluate rule =
    let mutable valid = true
    let covered = HashSet<int>()
    for source in distance7 do
        if valid && sourceMatches rule source then
            let candidates =
                movesOn rule.Axis
                |> Array.filter (fun move -> targetMatches rule (stateAt (nextIndex move source.Index)))
            if candidates.Length > 0 then
                let mask = int policy.OptimalMoves[source.Index]
                if candidates |> Array.forall (fun move -> mask &&& (1 <<< move) <> 0) then
                    covered.Add(source.Index) |> ignore
                else valid <- false
    if valid && covered.Count > 0 then Some(Set.ofSeq covered) else None

let mutable rules = ResizeArray<Rule>()
let sourceEos = optionChoices eoValues
let sourceLrs = optionChoices lrValues
let sourceFlips = optionChoices flipValues
let concreteEos = eoValues |> Array.distinct
let concreteLrs = lrValues |> Array.distinct
let concreteFlips = flipValues |> Array.distinct
let concreteSolvedTops = [| 0; 1; 2; 3; 4 |]

// Limit predicates to readable combinations: zero or one source property,
// and a concrete EO/LR target picture, optionally refined by center alignment.
for sourceEo in sourceEos do
  for sourceLr in sourceLrs do
   for sourceFlip in sourceFlips do
    let sourceTerms = [sourceEo.IsSome; sourceLr.IsSome; sourceFlip.IsSome] |> List.filter id |> List.length
    if sourceTerms <= 1 then
     for axis in [| 'M'; 'U' |] do
      let targetSpecs =
        if axis = 'M' then
          [| for eo in concreteEos do
               for lr in concreteLrs do
                for flips in concreteFlips do
                 yield Some eo, Some lr, Some flips, None, None, None |]
        else
          [| for mask in 0..15 do
               for eo in concreteEos do yield Some eo, None, None, None, None, Some mask
               for lr in concreteLrs do yield None, Some lr, None, None, None, Some mask
               for flips in concreteFlips do yield None, None, Some flips, None, None, Some mask |]
      for targetEo, targetLr, targetFlip, targetCenter, targetSolvedTop, targetSolvedTopMask in targetSpecs do
            let prototype =
                { SourceEo = sourceEo; SourceLr = sourceLr; SourceLrFlips = sourceFlip
                  Axis = axis; TargetEo = targetEo; TargetLr = targetLr
                  TargetLrFlips = targetFlip; TargetCenterDelta = targetCenter
                  TargetSolvedTop = targetSolvedTop
                  TargetSolvedTopMask = targetSolvedTopMask
                  Covered = Set.empty }
            match evaluate prototype with
            | Some covered when covered.Count >= 4 -> rules.Add { prototype with Covered = covered }
            | _ -> ()

let specificity rule =
    [ rule.SourceEo.IsSome; rule.SourceLr.IsSome; rule.SourceLrFlips.IsSome
      rule.TargetEo.IsSome; rule.TargetLr.IsSome; rule.TargetLrFlips.IsSome
      rule.TargetCenterDelta.IsSome; rule.TargetSolvedTop.IsSome; rule.TargetSolvedTopMask.IsSome ]
    |> List.filter id |> List.length

let deduplicated =
    rules
    |> Seq.groupBy (fun rule -> rule.Covered, rule.Axis)
    |> Seq.map (fun (_, sameCoverage) -> sameCoverage |> Seq.minBy specificity)
    |> Seq.toArray

let mutable uncovered = distance7 |> Array.map (fun state -> state.Index) |> Set.ofArray
let chosen = ResizeArray<Rule>()
let gains = ResizeArray<int>()
let mutable keepChoosing = true
while chosen.Count < 30 && keepChoosing do
    let candidate =
        deduplicated
        |> Array.map (fun rule -> rule, Set.intersect rule.Covered uncovered |> Set.count)
        |> Array.filter (fun (_, gain) -> gain > 0)
        |> Array.sortByDescending (fun (rule, gain) -> gain, -specificity rule)
        |> Array.tryHead
    match candidate with
    | None -> keepChoosing <- false
    | Some(rule, gain) -> chosen.Add rule; gains.Add gain; uncovered <- Set.difference uncovered rule.Covered

let showOption fallback value = value |> Option.defaultValue fallback
let moveForRule rule source =
    movesOn rule.Axis
    |> Array.find (fun move ->
        targetMatches rule (stateAt (nextIndex move source.Index))
        && (int policy.OptimalMoves[source.Index] &&& (1 <<< move) <> 0))
printfn "\nGreedy exact visual-target rules (first 20):"
chosen
|> Seq.filter (fun rule -> not (isNull (box rule)))
|> Seq.truncate 20
|> Seq.iteri (fun index rule ->
    let newly = gains[index]
    let example = distance7 |> Array.find (fun state -> rule.Covered.Contains state.Index)
    let exampleMove = moveForRule rule example
    let solution = solutionStartingWith exampleMove example
    let setup = solution |> Array.rev |> Array.map (fun move -> moveNames[inverseMove[move]]) |> String.concat " "
    printfn "%2d. covers %4d | source EO=%s LR=%s flips=%s | turn %c until EO=%s LR=%s flips=%s center=%s solved-U=%s mask=%s"
        (index + 1) newly
        (showOption "*" rule.SourceEo) (showOption "*" rule.SourceLr) (rule.SourceLrFlips |> Option.map string |> Option.defaultValue "*") rule.Axis
        (showOption "*" rule.TargetEo) (showOption "*" rule.TargetLr) (rule.TargetLrFlips |> Option.map string |> Option.defaultValue "*")
        (rule.TargetCenterDelta |> Option.map string |> Option.defaultValue "*")
        (rule.TargetSolvedTop |> Option.map string |> Option.defaultValue "*")
        (rule.TargetSolvedTopMask |> Option.map string |> Option.defaultValue "*")
    printfn "    example setup %s; next %s" setup moveNames[exampleMove])
printfn "covered by 30 greedily selected exact rules: %d / %d (%.2f%%)" (distance7.Length - uncovered.Count) distance7.Length (100.0 * float (distance7.Length - uncovered.Count) / float distance7.Length)
