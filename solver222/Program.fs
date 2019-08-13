open System
open Utility

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

let goalDistance = Array.create numStates Byte.MaxValue
let rec computeGoalDistances (states : State []) goal =
    let goals =
        let rec goals' i acc =
            if i < numStates then
                if i % 10000 = 0 then printf "."
                if goal states.[i] then
                    goalDistance.[i] <- 0uy
                    goals' (i + 1) (i :: acc)
                else goals' (i + 1) acc
            else
                printfn "!"
                acc
        goals' 0 []
    goals |> List.length |> printfn "Goal States: %i"
    let plySingleTwist t q i =
        let s = states.[i]
        let d = goalDistance.[i]
        let s' = applyTransform s t
        let i' = findStateIndex states s'
        let d' = goalDistance.[i']
        if d < d' then
            goalDistance.[i'] <- d + 1uy
            Set.add i' q
        else q
    let plyTwists q gs =
        let chain t q = List.fold (plySingleTwist t) q gs
        q |> chain twistU |> chain twistU' // |> chain twistU2
          |> chain twistR |> chain twistR' // |> chain twistR2
          |> chain twistF |> chain twistF' // |> chain twistF2
    let ply1 = plyTwists Set.empty goals |> Set.toList
    printfn "Ply1 %A" ply1.Length
    let ply2 = plyTwists Set.empty ply1 |> Set.toList
    printfn "Ply2 %A" ply2.Length
    let ply3 = plyTwists Set.empty ply2 |> Set.toList
    printfn "Ply3 %A" ply3.Length
    let ply4 = plyTwists Set.empty ply3 |> Set.toList
    printfn "Ply4 %A" ply4.Length
    let ply5 = plyTwists Set.empty ply4 |> Set.toList
    printfn "Ply5 %A" ply5.Length
    let ply6 = plyTwists Set.empty ply5 |> Set.toList
    printfn "Ply6 %A" ply6.Length
    let ply7 = plyTwists Set.empty ply6 |> Set.toList
    printfn "Ply7 %A" ply7.Length
    let ply8 = plyTwists Set.empty ply7 |> Set.toList
    printfn "Ply8 %A" ply8.Length
    let ply9 = plyTwists Set.empty ply8 |> Set.toList
    printfn "Ply9 %A" ply9.Length
    let ply10 = plyTwists Set.empty ply9 |> Set.toList
    printfn "Ply10 %A" ply10.Length
    let ply11 = plyTwists Set.empty ply10 |> Set.toList
    printfn "Ply11 %A" ply11.Length
    let ply12 = plyTwists Set.empty ply11 |> Set.toList
    printfn "Ply12 %A" ply12.Length
    let ply13 = plyTwists Set.empty ply12 |> Set.toList
    printfn "Ply13 %A" ply13.Length
    let ply14 = plyTwists Set.empty ply13 |> Set.toList
    printfn "Ply14 %A" ply14.Length
    let ply15 = plyTwists Set.empty ply14 |> Set.toList
    printfn "Ply15 %A" ply15.Length






    (*
    let update depth queue t =
        let update' s t q =
            let s' = applyTransform s t
            let i = findStateIndex states s'
            if depth < goalDistance.[i] then
                goalDistance.[i] <- depth
                i :: q
            else q
        update' 
    *)

computeGoalDistances states ((=) solvedState)

printfn "%A Done! (press return to exit)" (DateTime.Now)
Console.ReadLine() |> ignore