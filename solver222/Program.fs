type State = int array * int array

let solvedState : State = [|0; 1; 2; 3; 4; 5; 6; 7|], [|0; 0; 0; 0; 0; 0; 0; 0|]

type Transform = int array * int array

type Twist = U | D | L | R | F | B

let twistToTransform = function
    | U -> [|1; 2; 3; 0; 4; 5; 6; 7|], [|0; 0; 0; 0; 0; 0; 0; 0|] : Transform
    | D -> [|0; 1; 2; 3; 5; 6; 7; 4|], [|0; 0; 0; 0; 0; 0; 0; 0|] : Transform
    | L -> [|0; 1; 6; 2; 4; 3; 5; 7|], [|0; 0; 2; 1; 0; 2; 1; 0|] : Transform
    | R -> [|4; 0; 2; 3; 7; 5; 6; 1|], [|2; 1; 0; 0; 1; 0; 0; 2|] : Transform
    | F -> [|3; 1; 2; 5; 0; 4; 6; 7|], [|1; 0; 0; 2; 2; 1; 0; 0|] : Transform
    | B -> [|0; 7; 1; 3; 4; 5; 2; 6|], [|0; 2; 1; 0; 0; 0; 2; 1|] : Transform

type Rotation = X | Y | Z

let rotationToTransform = function
    | Z -> [|3; 2; 6; 5; 0; 4; 7; 1|], [|1; 2; 1; 2; 2; 1; 2; 1|] : Transform
    | X -> [|4; 0; 3; 5; 7; 6; 2; 1|], [|2; 1; 2; 1; 1; 2; 1; 2|] : Transform
    | Y -> [|1; 2; 3; 0; 7; 4; 5; 6|], [|0; 0; 0; 0; 0; 0; 0; 0|] : Transform

type Move = Twist of Twist | Rotation of Rotation

let moveToTransform = function
    | Twist t -> twistToTransform t
    | Rotation r -> rotationToTransform r

let applyTransform ((ps, os) : State) ((pt, ot) : Transform) : State =
    Array.init 8 (fun i -> ps.[pt.[i]]),
    Array.init 8 (fun i -> (os.[pt.[i]] + ot.[i]) % 3)

let gen () =
    let u = Twist U |> moveToTransform
    let r = Twist R |> moveToTransform
    let f = Twist F |> moveToTransform
    let nextPly progress states queue current =
        if progress % 1000 = 0 then
            printfn "States: %i Queue: %i" (Set.count states) (List.length queue)
        let enqueue next (progress, states, queue) =
            if Set.contains next states then progress, states, queue
            else (progress + 1), (Set.add next states), next :: queue
        let uu = applyTransform current u
        let rr = applyTransform current r
        let ff = applyTransform current f
        (progress, states, queue) |> enqueue uu |> enqueue rr |> enqueue ff
    let rec go progress states = function
        | current :: queue ->
            let progress', states', queue' = nextPly progress states queue current
            go progress' states' queue'
        | [] -> states
    go 0 Set.empty [solvedState]

// this takes about a half-hour and 2GB RAM on a MacBook (generates 3,674,160 unique states)
gen () |> Set.count |> printfn "Done: %i"

// let apply transform cube = function

// let identity =

// let invert t =

// let combine t0 t1 =

// let multiply t n =
// < 0 -> invert t -n
// = 0 -> identity
// = 1 -> t
//   2 -> combine t t
// > 2 -> recurse