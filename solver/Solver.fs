module Solver

open System
open Cube
open Render

let solved =
    let u = faceOfStickers Color.W Color.W Color.W Color.W Color.W Color.W Color.W Color.W Color.W
    let d = faceOfStickers Color.Y Color.Y Color.Y Color.Y Color.Y Color.Y Color.Y Color.Y Color.Y
    let l = faceOfStickers Color.O Color.O Color.O Color.O Color.O Color.O Color.O Color.O Color.O
    let r = faceOfStickers Color.R Color.R Color.R Color.R Color.R Color.R Color.R Color.R Color.R
    let f = faceOfStickers Color.G Color.G Color.G Color.G Color.G Color.G Color.G Color.G Color.G
    let b = faceOfStickers Color.B Color.B Color.B Color.B Color.B Color.B Color.B Color.B Color.B
    cubeOfFaces u d l r f b

let scrambleWithMoves (moves: Move list) n =
    let rand = Random()
    let rec scramble' cube sequence history n =
        // let isRepeat c = List.contains c history
        let rec canUndoTwoInOneMove c = function
            | m :: t ->
                match history with
                    | _ :: previous :: _ -> if move m c = previous then true else canUndoTwoInOneMove c t
                    | _ -> false
            | _ -> false
        if n = 0 then (cube, Seq.rev sequence) else
            let m = List.item (rand.Next moves.Length) moves
            let cube' = move m cube
            // if isRepeat cube' || canUndoTwoInOneMove cube' moves
            // then scramble' cube sequence history n // try again
            // else
            scramble' cube' (m :: sequence) (cube' :: history) (n - 1)
    scramble' solved [] [solved] n

let scrambleRouxL4E =
    let moves = [U2; M; M'; M2]
    scrambleWithMoves moves

let scramble =
    let moves = [Move.U; U'; U2; Move.D; D'; D2; Move.L; L'; L2; Move.R; R'; R2; Move.F; F'; F2; Move.B; B'; B2] @ [M; M'; M2] @ [S; S'; S2; E; E'; E2] // NOTE: centers don't move without slices
    scrambleWithMoves moves

let solveWithSteps includedSteps check cube =
    let mutable count = 0
    let rec solve' max depth steps cube = seq {
        count <- count + 1
        if count % 1000 = 0 then printf "."
        let recurse s = seq { yield! solve' max (depth + 1) (s :: steps) (step s cube) }
        if check cube then yield Seq.rev steps |> Seq.toList
        elif depth < max then
            for s in includedSteps do
                yield! recurse s }
    let rec iterativeDeepening depth = seq { // TODO: something more efficient (breadth-first)
        let solutions = solve' depth 0 [] cube |> List.ofSeq
        yield! solutions
        if Seq.length solutions = 0 then yield! iterativeDeepening (depth + 1) }
    iterativeDeepening 0 |> List.ofSeq

let hybridSolve steps (hints: ((Step list) list) list) (patterns : (string * string * string list) seq) goal stage cube =
    let matches (c: string) (p: string) = Seq.forall2 (fun p c -> p = '.' || p = c) p c
    match Seq.tryFind (fun (s, p, _) -> s = stage && matches (cubeToString cube) p) patterns with
    | Some (_, _, algs) ->
        match algs with
        | a :: _ -> [a.Split(' ') |> Seq.map stringToStep |> List.ofSeq]
        | [] -> [] // skip
    | None ->
        let tryHint h = 
            match Seq.tryHead h with
            | Some h' -> cube |> executeSteps h' |> goal
            | None -> false
        match hints |> Seq.filter tryHint |> Seq.tryHead with
        | Some solution -> solution
        | None -> solveWithSteps steps goal cube

let genCasesAndSolutions patterns steps cubes goal stage =
    let rec gen cases (hints : ((Step list) list) list) = function
        | cube :: remaining ->
            let solutions = hybridSolve steps hints patterns goal stage cube
            let algs = Seq.map stepsToString solutions
            // printfn "Algs: %s" algs
            let skip = Seq.length solutions = 0
            let key = if skip then "" else algs |> Seq.sort |> Seq.head
            // printfn "Key: %s" key
            match Map.tryFind key cases with
            | Some case -> gen (Map.add key ((cube, solutions, if skip then cube else executeSteps (Seq.head solutions) cube) :: case) cases) hints remaining
            | None ->
                printfn "New case: %i [\"%s\"] (%s)" (key.GetHashCode()) (String.Join("\"; \"", algs)) (cubeToString cube)
                let cube' = if skip then cube else executeSteps (Seq.head solutions) cube
                let hints' = if skip then hints else solutions :: hints
                gen (Map.add key [cube, solutions, cube'] cases) hints' remaining
        | _ -> cases
    gen Map.empty [] cubes

let distinctCases solutions =
    let common (cubes: string list) =
        let commonNth n =
            let distinct = cubes |> Seq.map (fun c -> c.[n]) |> Seq.distinct
            if Seq.length distinct = 1 then Seq.head distinct else '.'
        String.Concat(Seq.init (9 * 6) commonNth)
    let cases = solutions |> Map.toList |> List.map snd |> List.map (List.map (fun (c, _, _) -> cubeToString c)) |> List.map common
    let algs = solutions |> Map.toList |> List.map fst
    List.zip cases algs |> Seq.iter (fun (c, a) -> (* c |> stringToCube |> render; *) printfn "Algs: %s (%i) \"%s\"" a (a.GetHashCode()) c)

let solveCase patterns steps name id case scrambled =
    printfn "\nCase: %s" name
    let solutions = genCasesAndSolutions patterns steps scrambled case id
    distinctCases solutions
    solutions |> Map.toList |> List.map snd |> List.concat |> List.map (fun (_, _, c) -> c)
