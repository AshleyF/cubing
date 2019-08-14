open System
open Utility

printfn "2x2 Solver - See https://en.wikipedia.org/wiki/Pocket_Cube#Permutations"
printfn "First run initialization requires ~1.5 hr (generating *.bin state and distances files)."
printfn "Runs thereafter are faster (~15 seconds)."

let numStates = 3674160 

let getRawStates () =
    // this generates all 3,674,160 possible 2x2 states, assuming DLB corner is fixed
    printfn "%A Generating all states...
             This one-time process may require ~15 minutes and 2GB RAM
             States grows to %i, Queue grows to 1380349 and shrinks to zero" (DateTime.Now) numStates
    let nextPly progress states queue current =
        if progress % 10000 = 0 then printfn "States: %i Queue: %i" (Set.count states) (List.length queue)
        let enqueue next (progress, states, queue) =
            if Set.contains next states then progress, states, queue
            else (progress + 1), (Set.add next states), next :: queue
        let u = applyTransform current twistU
        let r = applyTransform current twistR
        let f = applyTransform current twistF
        (progress, states, queue) |> enqueue u |> enqueue r |> enqueue f // avoiding DLB corner
    let rec go progress states = function
        | current :: queue ->
            let progress', states', queue' = nextPly progress states queue current
            go progress' states' queue'
        | [] -> states
    go 0 Set.empty [solvedState]

let getOrderedStates () =
    // ordered by hash code (allows for quicker reconstitution to simple array with binary search retrieval)
    if statesPersisted () then
        printf "%A Loading states (~15 sec)..." (DateTime.Now)
        let states = loadStates numStates
        states.Length |> printfn "\n%A Total States: %i" (DateTime.Now)
        states
    else
        let rawStates = getRawStates ()
        rawStates |> Set.count |> printfn "%A Total States: %i" (DateTime.Now)
        printfn "%A Sorting (~3 min)..." (DateTime.Now)
        let orderedStates = rawStates |> Set.toList |> Array.ofList |> Array.sort
        printfn "%A Saving states (~17.5MB)..." (DateTime.Now)
        saveStates orderedStates
        orderedStates

let states = getOrderedStates ()

let rec computeGoalDistances htm (states : State []) goal =
    let goalDistances = Array.create numStates Byte.MaxValue
    let plySingleTwist t q i =
        let i' = applyTransform states.[i] t |> findStateIndex states
        let d = goalDistances.[i]
        if d < goalDistances.[i'] then
            goalDistances.[i'] <- d + 1uy
            Set.add i' q
        else q
    let plyTwists q gs =
        let chain t q = List.fold (plySingleTwist t) q gs
        q |> chain twistU  |> chain twistR  |> chain twistF
          |> chain twistU' |> chain twistR' |> chain twistF'
          |> if htm then (fun q -> q |> chain twistU2 |> chain twistR2 |> chain twistF2) else id
    let rec iterate n q =
        printfn "n=%i %i" n (List.length q)
        let q' = plyTwists Set.empty q |> Set.toList
        if q'.Length > 0 then iterate (n + 1) q'
    let goals =
        let rec goals' i acc =
            if i < numStates then
                if goal states.[i] then
                    goalDistances.[i] <- 0uy
                    goals' (i + 1) (i :: acc)
                else goals' (i + 1) acc
            else acc
        goals' 0 []
    goals |> List.length |> printfn "Goal States: %i"
    iterate 0 goals
    goalDistances

let isSolved = ((=) solvedState)

let qtmDistances = computeOrLoadDistances "QTM" (fun () -> computeGoalDistances false states isSolved)
let htmDistances = computeOrLoadDistances "HTM" (fun () -> computeGoalDistances true states isSolved)

let bestTwists (states : State []) (distances : byte []) (cube : State) =
    let scoredTwists = seq {
        let score (t : Transform) = distances.[cube |> applyTransform t |> canonicalize |> findStateIndex states]
        yield ("", identityTransform, score identityTransform)
        yield ("U",  twistU,  score twistU)
        yield ("U'", twistU', score twistU')
        yield ("U2", twistU2, score twistU2)
        // yield ("D",  twistD,  score twistD)
        // yield ("D'", twistD', score twistD')
        // yield ("D2", twistD2, score twistD2)
        // yield ("L",  twistL,  score twistL)
        // yield ("L'", twistL', score twistL')
        // yield ("L2", twistL2, score twistL2)
        yield ("R",  twistR,  score twistR)
        yield ("R'", twistR', score twistR')
        yield ("R2", twistR2, score twistR2)
        yield ("F",  twistF,  score twistF)
        yield ("F'", twistF', score twistF')
        yield ("F2", twistF2, score twistF2) } |> List.ofSeq
        // yield ("B",  twistB,  score twistB)
        // yield ("B'", twistB', score twistB')
        // yield ("B2", twistB2, score twistB2)
        
    let scoreValue (_, _, s) = s
    let best = Seq.minBy scoreValue scoredTwists
    Seq.filter (fun x -> scoreValue x = scoreValue best) scoredTwists

let solve states distances cube =
    let rec solve' c =
        let (n, t, s) = bestTwists states distances c |> Seq.head
        printf "%s " n
        if t <> identityTransform then applyTransform t c |> solve' else c
    cube |> solve'

let test distances =
    printfn "Testing"
    let test' i cube =
        printf "%i " i
        let solved = solve states distances cube
        if solved <> solvedState then printfn " !!!!!!!!!!!!!!!!!!!!!!!!!!!!!! FAILED !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!"
        printfn ""
    Seq.iteri test' states

let scrambled =
    solvedState
    |> applyTransform twistR
    |> applyTransform twistU
    |> applyTransform twistR'
    |> applyTransform twistU
    |> applyTransform twistR
    |> applyTransform twistU2
    |> applyTransform twistR'
    |> applyTransform twistU2
    |> applyTransform twistF

printfn "Solving..."
let solved = solve states htmDistances scrambled
if solved <> solvedState then printfn " !!!!!!!!!!!!!!!!!!!!!!!!!!!!!! WHAT?! !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!"

test htmDistances

printfn "%A Done! (press return to exit)" (DateTime.Now)
Console.ReadLine() |> ignore