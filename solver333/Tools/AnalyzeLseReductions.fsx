#r "../../library/bin/Release/net8.0/Library.dll"

open System
open System.IO
open System.Collections.Generic
open Lse

// This experiment models the language used by EOLR guides: a rule chooses
// U/M powers by the relationship it wants to create, then continues from a
// named family.  Thus U-M-U is one parameterized schema, not 27 algorithms.

type State =
    { Index: int
      Pieces: int array
      Flips: int array
      Center: int
      Auf: int
      Distance: int }

let pieceNames = [| "UL"; "UR"; "UF"; "UB"; "DF"; "DB" |]
let eoNames = [| "0/0"; "3/1"; "4/0"; "2o/2"; "2a/2"; "1/1"; "2o/0"; "2a/0"; "0/2"; "4/2" |]
let roleNames = [| "oriented U-side"; "flipped U-side"; "oriented U-M"; "flipped U-M"; "oriented D"; "flipped D" |]
let relationNames = [| "split"; "both D"; "adjacent U"; "opposite U" |]

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

let positionOf piece (state: State) = state.Pieces |> Array.findIndex ((=) piece)
let uRing = [| 0; 3; 1; 2 |] // UL, UB, UR, UF
let uRingIndex position = uRing |> Array.tryFindIndex ((=) position)

let eoFamily state =
    let top = state.Flips[0..3] |> Array.sum
    let bottom = state.Flips[4..5] |> Array.sum
    let flippedTop = [0..3] |> List.filter (fun position -> state.Flips[position] = 1)
    let opposite =
        match flippedTop |> List.choose uRingIndex with
        | [left; right] -> abs (left - right) = 2
        | _ -> false
    match top, bottom with
    | 0, 0 -> 0 | 3, 1 -> 1 | 4, 0 -> 2
    | 2, 2 when opposite -> 3 | 2, 2 -> 4 | 1, 1 -> 5
    | 2, 0 when opposite -> 6 | 2, 0 -> 7 | 0, 2 -> 8 | 4, 2 -> 9
    | _ -> failwith $"Unclassified EO shape {state.Flips}."

let lrRole position flip =
    match position with
    | 0 | 1 -> flip
    | 2 | 3 -> 2 + flip
    | 4 | 5 -> 4 + flip
    | _ -> failwith "Invalid LSE edge position."

let lrRoles state =
    [| for piece in 0 .. 1 do
           let position = positionOf piece state
           yield lrRole position state.Flips[position] |]
    |> Array.sort

let lrRelation state =
    let left, right = positionOf 0 state, positionOf 1 state
    match uRingIndex left, uRingIndex right with
    | Some l, Some r when abs (l - r) = 2 -> 3
    | Some _, Some _ -> 2
    | None, None -> 1
    | _ -> 0

// This deliberately ignores colors, exact AUF, and which of a mirrored pair
// is front.  It is close to a guide's named recognition family.
let targetKey state =
    let roles = lrRoles state
    (((eoFamily state * 6 + roles[0]) * 6 + roles[1]) * 4 + lrRelation state)

let describeTarget key =
    let relation = key % 4
    let aboveRelation = key / 4
    let rightRole = aboveRelation % 6
    let aboveRight = aboveRelation / 6
    let leftRole = aboveRight % 6
    let eo = aboveRight / 6
    $"{eoNames[eo]}, LR {roleNames[leftRole]} + {roleNames[rightRole]}, {relationNames[relation]}"

let cycleType state =
    let seen = Array.zeroCreate<bool> 6
    let lengths = ResizeArray<int>()
    for start in 0 .. 5 do
        if not seen[start] then
            let mutable position = start
            let mutable length = 0
            while not seen[position] do
                seen[position] <- true
                length <- length + 1
                position <- state.Pieces[position]
            lengths.Add length
    lengths |> Seq.sortDescending |> Seq.fold (fun value length -> value * 7 + length) 0

let cycleTypeName value =
    let rec loop remaining result =
        if remaining = 0 then result else loop (remaining / 7) ((remaining % 7) :: result)
    loop value [] |> List.map string |> String.concat "+"

let solvedMCount state =
    [2 .. 5] |> List.sumBy (fun piece -> if positionOf piece state = piece then 1 else 0)

// Recognition is allowed a little more of the information a person can see:
// relative center/corner alignment and the cycle structure of the remaining
// edges.  These distinguish cases whose EO/LR pictures are identical but
// whose best reduction differs because of the future L4E.
let sourceKey state =
    targetKey state, (state.Center - state.Auf + 4) % 4, cycleType state, solvedMCount state

let describeSource (target, centerDelta, cycles, solvedM) =
    $"{describeTarget target}; center/corners delta {centerDelta}; cycles {cycleTypeName cycles}; {solvedM} solved M edges"

type Skeleton = { Name: string; Axes: char array }
let skeletons =
    [| { Name = "U-adjust"; Axes = [| 'U' |] }
       { Name = "M"; Axes = [| 'M' |] }
       { Name = "U-M"; Axes = [| 'U'; 'M' |] }
       { Name = "M-U"; Axes = [| 'M'; 'U' |] }
       { Name = "U-M-U"; Axes = [| 'U'; 'M'; 'U' |] }
       { Name = "M-U-M"; Axes = [| 'M'; 'U'; 'M' |] }
       { Name = "U-M-U-M"; Axes = [| 'U'; 'M'; 'U'; 'M' |] }
       { Name = "M-U-M-U"; Axes = [| 'M'; 'U'; 'M'; 'U' |] } |]

let policyPath = Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, "..", "Data", "lse-policy-v1.dat"))
let policy = loadPolicy policyPath
let statesByIndex = Array.zeroCreate<State option> policy.Distances.Length
let states =
    [| for index in 0 .. policy.Distances.Length - 1 do
           let distance = policy.Distances[index]
           if distance <> unreachable then
               let state = decode index (int distance)
               statesByIndex[index] <- Some state
               if distance <> 0uy then yield state |]

let stateAt index = statesByIndex[index] |> Option.get
let choices axis = if axis = 'M' then [| 0; 1; 2 |] else [| 3; 4; 5 |]

// All target families reachable by an exactly distance-decreasing
// instantiation of a skeleton.  Every prefix move must itself be optimal.
let targetsFor (skeleton: Skeleton) state =
    let targets = HashSet<int>()
    let rec walk depth index =
        if depth = skeleton.Axes.Length then
            targets.Add(targetKey (stateAt index)) |> ignore
        else
            let distance = policy.Distances[index]
            for move in choices skeleton.Axes[depth] do
                let next = nextIndex move index
                if policy.Distances[next] <> unreachable && policy.Distances[next] = distance - 1uy then
                    walk (depth + 1) next
    if state.Distance >= skeleton.Axes.Length then walk 0 state.Index
    targets

type Reduction =
    { Source: int * int * int * int
      Target: int
      Skeleton: Skeleton
      Members: int
      MeanChoices: float }

let groups = states |> Array.groupBy sourceKey
let reductions = ResizeArray<Reduction>()

for source, members in groups do
    for skeleton in skeletons do
        let mutable common: HashSet<int> option = None
        let mutable choiceTotal = 0
        let mutable valid = true
        for state in members do
            if valid then
                let targets = targetsFor skeleton state
                choiceTotal <- choiceTotal + targets.Count
                if targets.Count = 0 then valid <- false
                else
                    match common with
                    | None -> common <- Some(HashSet<int>(targets))
                    | Some intersection ->
                        intersection.IntersectWith targets
                        if intersection.Count = 0 then valid <- false
        if valid then
            for target in common.Value do
                reductions.Add
                    { Source = source
                      Target = target
                      Skeleton = skeleton
                      Members = members.Length
                      MeanChoices = float choiceTotal / float members.Length }

let ranked =
    reductions
    |> Seq.sortByDescending (fun reduction -> reduction.Members, -reduction.Skeleton.Axes.Length)
    |> Seq.toArray

let coveredSources = ranked |> Array.map (fun reduction -> reduction.Source) |> Array.distinct |> Set.ofArray
let coveredStates =
    groups
    |> Array.sumBy (fun (source, members) -> if coveredSources.Contains source then members.Length else 0)

let report = ResizeArray<string>()
report.Add "# Exact parameterized LSE reductions"
report.Add ""
report.Add "This experiment treats a sequence shape such as `U-M-U` as one rule schema. Each U or M power is selected by the visible relationship that the rule promises to create. A schema is reported only when every concrete state in its normalized source family has at least one instantiation that follows the exact optimal policy at every move and reaches the same normalized target family."
report.Add ""
report.Add $"- Concrete non-solved states: {states.Length:N0}"
report.Add $"- Relational source families: {groups.Length:N0}"
report.Add $"- Universal exact reductions found: {ranked.Length:N0}"
report.Add $"- Source families with at least one reduction: {coveredSources.Count:N0}"
report.Add(sprintf "- Concrete states in those families: %s (%.2f%%)" (coveredStates.ToString("N0")) (100.0 * float coveredStates / float states.Length))
report.Add "- Sequence skeletons tested: U, M, U-M, M-U, U-M-U, M-U-M, U-M-U-M, M-U-M-U"
report.Add ""
report.Add "This is stricter than an ordinary EOLR description. Recognition uses EO family, the two color-neutral LR roles, their coarse geometric relation, relative center/corner alignment, permutation cycle type, and solved M-edge count. It does not yet use guide-specific ideas such as good/bad arrows, bars, or matching a particular corner color."
report.Add ""
report.Add "## Largest universal reductions"
report.Add ""
report.Add "| States | Schema | Source family | Target family | Mean reachable targets |"
report.Add "| ---: | --- | --- | --- | ---: |"
for reduction in ranked |> Seq.truncate 60 do
    report.Add(sprintf "| %s | %s | %s | %s | %.2f |"
        (reduction.Members.ToString("N0")) reduction.Skeleton.Name
        (describeSource reduction.Source) (describeTarget reduction.Target) reduction.MeanChoices)
report.Add ""
report.Add "## Interpretation"
report.Add ""
report.Add "A row is a genuine recursive rule candidate: recognize the source family, choose powers in the named skeleton that produce the target relationship, then continue using the rule for that target family. The next pass should add the guide's missing relational vocabulary, choose a small acyclic set of reductions, and test how many states can reach solved by recursive application without consulting the raw table."

let reportPath = Path.Combine(__SOURCE_DIRECTORY__, "LseReductionResults.md")
File.WriteAllLines(reportPath, report)
printfn "Analyzed %i states in %i normalized families." states.Length groups.Length
printfn "Found %i universal exact reductions." ranked.Length
printfn "Wrote %s" reportPath
