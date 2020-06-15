module Solver

open System
open Cube
open Render

let quiet = false
let warnings = true

let scrambleWithMoves (moves: Move list) n =
    let rand = Random()
    let rec scramble' cube sequence history n =
        if n = 0 then (cube, Seq.rev sequence) else
            let m = List.item (rand.Next moves.Length) moves
            let cube' = move m cube
            scramble' cube' (m :: sequence) (cube' :: history) (n - 1)
    scramble' Cube.solved [] [Cube.solved] n

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

let hybridSolve steps hints patterns goal stage cube =
    let split (s: string) (c: char) = s.Split(c)
    let matches (cube: string) (pattern: string * bool * bool) =
        let mtch p c =
            p = '.' || p = c || // wildcard or perfect match
            (p = 'P' && c <> 'W' && c <> 'Y') || // bad edge (assume Y/W up/down)
            (p = 'E' && (c = 'W' || c = 'Y')) || // good edge
            (p = '*' && (c = 'B' || c = 'G')) // B/G for LR
        let matchPat p = Seq.forall2 mtch p cube
        let cycleCW   = function 'B' -> 'O' | 'O' -> 'G' | 'G' -> 'R' | 'R' -> 'B' | c -> c
        let cycleCCW  = function 'B' -> 'R' | 'R' -> 'G' | 'G' -> 'O' | 'O' -> 'B' | c -> c
        let cycleSwap = function 'B' -> 'G' | 'G' -> 'B' | 'R' -> 'O' | 'O' -> 'R' | c -> c
        let rotateCorners p =
            match List.ofSeq p with
            | [b0; b1; b2; b3; b4; b5; b6; b7; b8; u0; u1; u2; u3; u4; u5; u6; u7; u8; l0; l1; l2; f0; f1; f2; r0; r1; r2; l3; l4; l5; f3; f4; f5; r3; r4; r5; l6; l7; l8; f6; f7; f8; r6; r7; r8; d0; d1; d2; d3; d4; d5; d6; d7; d8] ->
              [b0; b1; b2; b3; b4; b5; l2; b7; l0; u6; u1; u0; u3; u4; u5; u8; u7; u2; f0; l1; f2; r0; f1; r2; b8; r1; b6; l3; l4; l5; f3; f4; f5; r3; r4; r5; l6; l7; l8; f6; f7; f8; r6; r7; r8; d0; d1; d2; d3; d4; d5; d6; d7; d8] |> Seq.ofList // U
            | invalid -> failwith (sprintf "Invalid pattern: %A" invalid)
        let cycleCornerColors c p =
            match List.ofSeq p with
            | [b0; b1; b2; b3; b4; b5;   b6; b7;   b8;   u0; u1;   u2; u3; u4; u5;   u6; u7;   u8;   l0; l1;   l2;   f0; f1;   f2;   r0; r1;   r2; l3; l4; l5; f3; f4; f5; r3; r4; r5; l6; l7; l8; f6; f7; f8; r6; r7; r8; d0; d1; d2; d3; d4; d5; d6; d7; d8] ->
              [b0; b1; b2; b3; b4; b5; c b6; b7; c b8; c u0; u1; c u2; u3; u4; u5; c u6; u7; c u8; c l0; l1; c l2; c f0; f1; c f2; c r0; r1; c r2; l3; l4; l5; f3; f4; f5; r3; r4; r5; l6; l7; l8; f6; f7; f8; r6; r7; r8; d0; d1; d2; d3; d4; d5; d6; d7; d8] |> Seq.ofList // U
            | invalid -> failwith (sprintf "Invalid pattern: %A" invalid)
        let pat, aufNeutral, colorNeutralUpCorners = pattern
        let patU () = rotateCorners pat
        let patU2 () = rotateCorners (patU ())
        let patU' () = rotateCorners (patU2 ())
        let cw p = cycleCornerColors cycleCW p
        let ccw p = cycleCornerColors cycleCCW p
        let swap p = cycleCornerColors cycleSwap p
        match (aufNeutral, colorNeutralUpCorners) with
        | true, true ->
            matchPat       pat  || matchPat       (patU ())  || matchPat       (patU2 ())  || matchPat       (patU' ())  ||
            matchPat   (cw pat) || matchPat   (cw (patU ())) || matchPat   (cw (patU2 ())) || matchPat   (cw (patU' ())) ||
            matchPat  (ccw pat) || matchPat  (ccw (patU ())) || matchPat  (ccw (patU2 ())) || matchPat  (ccw (patU' ())) ||
            matchPat (swap pat) || matchPat (swap (patU ())) || matchPat (swap (patU2 ())) || matchPat (swap (patU' ()))
        | true, false -> matchPat pat || matchPat (patU ()) || matchPat (patU2 ()) || matchPat (patU' ())
        | false, true -> matchPat pat || matchPat (cw pat) || matchPat (ccw pat) || matchPat (swap pat) 
        | false, false -> matchPat pat
    match Seq.tryFind (fun (s, p, _) -> s = stage && matches (cubeToString cube) p) patterns with
    | Some (_, _, algs) ->
        match algs with
        | a :: _ -> [split a ' ' |> Seq.map stringToStep |> List.ofSeq]
        | [] -> [] // skip
    | None ->
        if warnings then printfn "UNMATCHED: %s" (cubeToString cube)
        let tryHint h = 
            match Seq.tryHead h with
            | Some h' -> cube |> executeSteps h' |> goal
            | None -> false
        match hints |> Seq.filter tryHint |> Seq.tryHead with
        | Some solution -> solution
        | None -> solveWithSteps steps goal cube

let mutable best = 0
let mutable worst = 0
let genCasesAndSolutions patterns steps cubes goal stage =
    let rec gen cases (hints : ((Step list) list) list) b w = function
        | cube :: remaining ->
            let solutions = hybridSolve steps hints patterns goal stage cube
            let turns = if solutions.Length = 0 then 0 else solutions.[0].Length
            let b' = if turns < b then turns else b
            let w' = if turns > w then turns else w
            let algs = Seq.map stepsToString solutions
            // printfn "Algs: %s" algs
            let skip = Seq.length solutions = 0
            let key = if skip then "" else algs |> Seq.sort |> Seq.head
            // printfn "Key: %s" key
            match Map.tryFind key cases with
            | Some case -> gen (Map.add key ((cube, solutions, if skip then cube else executeSteps (Seq.head solutions) cube) :: case) cases) hints b' w' remaining
            | None ->
                if not quiet then printfn "New case: %i [\"%s\"] (%s)" (key.GetHashCode()) (String.Join("\"; \"", algs)) (cubeToString cube)
                let cube' = if skip then cube else executeSteps (Seq.head solutions) cube
                let hints' = if skip then hints else solutions :: hints
                gen (Map.add key [cube, solutions, cube'] cases) hints' b' w' remaining
        | _ -> cases, b, w
    let cases, b, w = gen Map.empty [] Int32.MaxValue Int32.MinValue cubes
    best <- best + b
    worst <- worst + w
    cases

let distinctCases solutions =
    let common (cubes: string list) =
        let commonNth n =
            let distinct = cubes |> Seq.map (fun c -> c.[n]) |> Seq.distinct
            if Seq.length distinct = 1 then Seq.head distinct else '.'
        String.Concat(Seq.init (9 * 6) commonNth)
    let cases = solutions |> Map.toList |> List.map snd |> List.map (List.map (fun (c, _, _) -> cubeToString c)) |> List.map common
    let algs = solutions |> Map.toList |> List.map fst
    List.zip cases algs |> Seq.iter (fun (c, a) -> (* c |> stringToCube |> render; *) if not quiet then printfn "Algs: %s (%i) \"%s\"" a (a.GetHashCode()) c)

let expandPatternsForAuf patterns =
    let expand (name, ((pat : string), cornerRotationNeutral, cornerColorNeutral, discoverAuf), algs) = seq {
        yield name, (pat, cornerRotationNeutral, cornerColorNeutral), algs
        if discoverAuf then
            let prepend a algs = if List.length algs = 0 then [a] else List.map (fun alg -> a + " " + alg) algs
            let auf p =
                match List.ofSeq p with
                | [b0; b1; b2; b3; b4; b5; b6; b7; b8; u0; u1; u2; u3; u4; u5; u6; u7; u8; l0; l1; l2; f0; f1; f2; r0; r1; r2; l3; l4; l5; f3; f4; f5; r3; r4; r5; l6; l7; l8; f6; f7; f8; r6; r7; r8; d0; d1; d2; d3; d4; d5; d6; d7; d8] ->
                  [b0; b1; b2; b3; b4; b5; l2; l1; l0; u6; u3; u0; u7; u4; u1; u8; u5; u2; f0; f1; f2; r0; r1; r2; b8; b7; b6; l3; l4; l5; f3; f4; f5; r3; r4; r5; l6; l7; l8; f6; f7; f8; r6; r7; r8; d0; d1; d2; d3; d4; d5; d6; d7; d8] |> Seq.ofList // U
                | invalid -> failwith (sprintf "Invalid pattern: %A" invalid)
            let u = auf pat
            let u2 = auf u
            let u' = auf u2
            yield name, (String.Join("", u),  cornerRotationNeutral, cornerColorNeutral), (prepend "U'" algs)
            yield name, (String.Join("", u2), cornerRotationNeutral, cornerColorNeutral), (prepend "U2" algs)
            yield name, (String.Join("", u'), cornerRotationNeutral, cornerColorNeutral), (prepend "U"  algs) }
    patterns |> Seq.map expand |> Seq.concat

let solveCase patterns steps name id case scrambled verify =
    printfn "\nCase: %s" name
    let solutions = genCasesAndSolutions patterns steps scrambled case id
    distinctCases solutions
    let solved = solutions |> Map.toList |> List.map snd |> List.concat |> List.map (fun (_, _, c) -> c)
    if verify then
        for s in solved do
            if not (case s) then
                printfn "UNSOLVED: %s" (cubeToString s)
                failwith "Authored pattern did not solve case"
    solved

let stageStats name numCubes =
    printfn "--------------------------------------------------------------------------------"
    printfn "STAGE STATS: %s" name
    let avgStageTwistCount = float Cube.stageCount / float numCubes
    printfn "Average: %f Best: %i Worst: %i" avgStageTwistCount best worst
    printfn "--------------------------------------------------------------------------------"
    Cube.stageCount <- 0
    best <- 0
    worst <- 0

let initScrambledCubes numCubes =
    printfn "Scrambling %i cubes" numCubes
    printfn ""
    List.init numCubes (fun _ -> printf "."; scramble 20 |> fst)
