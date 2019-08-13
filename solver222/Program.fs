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

printfn "Testing..."
let foo = [|1; 2; 3; 0; 4; 5; 6; 7|], [|0; 0; 0; 0; 0; 0; 0; 0|]
let bar = Twist L |> moveToTransform |> applyTransform solvedState
let baz = Rotation X |> moveToTransform |> applyTransform solvedState
let i = baz |> canonicalize |> findStateIndex states
printfn "Found (%i): %A" i states.[i]

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
            else acc
        goals' 0 []
    goals |> List.length |> printfn "Goal States: %i"
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