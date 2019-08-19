module Utility

open System
open System.IO
open System.Collections.Generic

type State = int array * int array

let solvedState : State = [|0; 1; 2; 3; 4; 5; 6; 7|], [|0; 0; 0; 0; 0; 0; 0; 0|]

type Transform = int array * int array

let identityTransform = solvedState |> Transform

type Twist = U | D | L | R | F | B

let twistToTransform = function
    | U -> [|1; 2; 3; 0; 4; 5; 6; 7|], [|0; 0; 0; 0; 0; 0; 0; 0|] : Transform
    | D -> [|0; 1; 2; 3; 5; 6; 7; 4|], [|0; 0; 0; 0; 0; 0; 0; 0|] : Transform
    | L -> [|0; 1; 6; 2; 4; 3; 5; 7|], [|0; 0; 2; 1; 0; 2; 1; 0|] : Transform
    | R -> [|4; 0; 2; 3; 7; 5; 6; 1|], [|2; 1; 0; 0; 1; 0; 0; 2|] : Transform
    | F -> [|3; 1; 2; 5; 0; 4; 6; 7|], [|1; 0; 0; 2; 2; 1; 0; 0|] : Transform
    | B -> [|0; 7; 1; 3; 4; 5; 2; 6|], [|0; 2; 1; 0; 0; 0; 2; 1|] : Transform

let twistU = U |> twistToTransform
let twistD = D |> twistToTransform
let twistL = L |> twistToTransform
let twistR = R |> twistToTransform
let twistF = F |> twistToTransform
let twistB = B |> twistToTransform

type Rotation = X | Y | Z

let rotationToTransform = function
    | Z -> [|3; 2; 6; 5; 0; 4; 7; 1|], [|1; 2; 1; 2; 2; 1; 2; 1|] : Transform
    | X -> [|4; 0; 3; 5; 7; 6; 2; 1|], [|2; 1; 2; 1; 1; 2; 1; 2|] : Transform
    | Y -> [|1; 2; 3; 0; 7; 4; 5; 6|], [|0; 0; 0; 0; 0; 0; 0; 0|] : Transform

let rotateX = X |> rotationToTransform
let rotateY = Y |> rotationToTransform
let rotateZ = Z |> rotationToTransform

type Move = Twist of Twist | Rotation of Rotation

let moveToTransform = function
    | Twist t -> twistToTransform t
    | Rotation r -> rotationToTransform r

// let invert t =

// let combine t0 t1 =

// let multiply t n =
// < 0 -> invert t -n
// = 0 -> identity
// = 1 -> t
//   2 -> combine t t
// > 2 -> recurse

let applyTransform ((pt, ot) : Transform) ((ps, os) : State) : State =
    Array.init 8 (fun i -> ps.[pt.[i]]),
    Array.init 8 (fun i -> (os.[pt.[i]] + ot.[i]) % 3)

let combine (t0 : Transform) t1 = applyTransform t1 (State t0) |> Transform

let twistU2  = combine twistU  twistU
let twistD2  = combine twistD  twistD
let twistL2  = combine twistL  twistL
let twistR2  = combine twistR  twistR
let twistF2  = combine twistF  twistF
let twistB2  = combine twistB  twistB
let rotateX2 = combine rotateX rotateX
let rotateY2 = combine rotateY rotateY
let rotateZ2 = combine rotateZ rotateZ

let invert ((p, o) : Transform) : Transform =
    let p' = Array.zeroCreate 8
    let o' = Array.zeroCreate 8
    for i in 0 .. 7 do
        let from = p.[i]
        p'.[from] <- i
        o'.[from] <- (3 - o.[i] + 3) % 3
    (p', o') |> Transform

let twistU'  = invert twistU
let twistD'  = invert twistD
let twistL'  = invert twistL
let twistR'  = invert twistR
let twistF'  = invert twistF
let twistB'  = invert twistB
let rotateX' = invert rotateX
let rotateY' = invert rotateY
let rotateZ' = invert rotateZ

let rotateXY   = combine rotateX  rotateY
let rotateXY'  = combine rotateX  rotateY'
let rotateXY2  = combine rotateX  rotateY2
let rotateXZ   = combine rotateX  rotateZ
let rotateXZ'  = combine rotateX  rotateZ'
let rotateXZ2  = combine rotateX  rotateZ2
let rotateYZ'  = combine rotateY  rotateZ'
let rotateZX'  = combine rotateZ  rotateX'
let rotateZY   = combine rotateZ  rotateY
let rotateX'Z' = combine rotateX' rotateZ'
let rotateX2Y  = combine rotateX2 rotateY
let rotateX2Y' = combine rotateX2 rotateY'
let rotateX2Z  = combine rotateX2 rotateZ
let rotateX2Z' = combine rotateX2 rotateZ'

let distinctRotations state = seq {
    yield state
    yield state |> applyTransform rotateX
    yield state |> applyTransform rotateXY
    yield state |> applyTransform rotateXY'
    yield state |> applyTransform rotateXY2
    yield state |> applyTransform rotateXZ
    yield state |> applyTransform rotateXZ'
    yield state |> applyTransform rotateXZ2
    yield state |> applyTransform rotateX'
    yield state |> applyTransform rotateYZ'
    yield state |> applyTransform rotateZX'
    yield state |> applyTransform rotateZY
    yield state |> applyTransform rotateX'Z'
    yield state |> applyTransform rotateX2
    yield state |> applyTransform rotateX2Y
    yield state |> applyTransform rotateX2Y'
    yield state |> applyTransform rotateX2Z
    yield state |> applyTransform rotateX2Z'
    yield state |> applyTransform rotateY
    yield state |> applyTransform rotateY'
    yield state |> applyTransform rotateY2
    yield state |> applyTransform rotateZ
    yield state |> applyTransform rotateZ'
    yield state |> applyTransform rotateZ2 }

let isCanonical ((p, o) : State) = p.[6] = 6 && o.[6] = 0 // DLB corner fixed

let canonicalize = distinctRotations >> Seq.filter isCanonical >> Seq.head

let statesFile = "../../../States222.bin"

let saveStates states =
    let writeState (bw : BinaryWriter) ((p, o) : State) =
        let compress bits s =
            let rec compress' v = function
                | i :: tail -> compress' ((v <<< bits) ||| i) tail
                | [] -> if bits = 3 then bw.Write(byte (v >>> 16))
                        bw.Write(byte (v >>> 8))
                        bw.Write(byte v)
            s |> List.ofArray |> compress' 0
        compress 3 p
        compress 2 o
    use bw = new BinaryWriter(new FileStream(statesFile, FileMode.Create))
    Array.iter (writeState bw) states

let statesPersisted () = File.Exists statesFile

let loadStates numStates =
    let readState (br : BinaryReader) : State =
        let readVal bits mask =
            let decompress v =
                let rec decompress' v = seq {
                    yield int (v &&& mask)
                    yield! decompress' (v >>> bits) }
                decompress' v |> Seq.take 8 |> Seq.rev |> Array.ofSeq
            let b0 = if bits = 3 then br.ReadByte() |> int else 0
            let b1 = br.ReadByte() |> int
            let b2 = br.ReadByte() |> int
            (b0 <<< 16) ||| (b1 <<< 8) ||| b2 |> decompress
        readVal 3 0b111, readVal 2 0b11
    use br = new BinaryReader(File.Open(statesFile, FileMode.Open))
    let read i =
        if i % 100000 = 0 then printf "."
        readState br
    Array.init numStates read

type StateComparer() =
    interface IComparer<State> with
        member this.Compare(a, b) =
            if a < b then -1
            elif a > b then 1
            else 0
let stateComparer = StateComparer()

let findStateIndex states s =
    Array.BinarySearch(states, s, stateComparer)

let computeOrLoadDistances name comp =
    let file = sprintf "../../../%sDistances222.bin" name
    if File.Exists file then
        printfn "%A Loading %s distances..." (DateTime.Now) name
        File.ReadAllBytes(file)
    else
        printfn "%A Computing %s distances (~40 min)..." (DateTime.Now) name
        let distances = comp ()
        File.WriteAllBytes(file, distances)
        distances

let display label ((p, o) as cube : State) =
    let color c f =
        let colors = [|[|'U'; 'F'; 'R'|]; [|'U'; 'R'; 'B'|]; [|'U'; 'B'; 'L'|]; [|'U'; 'L'; 'F'|];
                       [|'D'; 'R'; 'F'|]; [|'D'; 'F'; 'L'|]; [|'D'; 'L'; 'B'|]; [|'D'; 'B'; 'R'|]|]
        colors.[p.[c]].[(o.[c] + f) % 3]
    printfn "%s:\n  %c%c\n  %c%c\n%c%c%c%c%c%c%c%c\n%c%c%c%c%c%c%c%c\n  %c%c\n  %c%c" label
                                (color 6 2) (color 7 1)
                                (color 2 1) (color 1 2)
        (color 6 1) (color 2 2) (color 2 0) (color 1 0) (color 1 1) (color 7 2) (color 7 0) (color 6 0)
        (color 5 2) (color 3 1) (color 3 0) (color 0 0) (color 0 2) (color 4 1) (color 4 0) (color 5 0)
                                (color 3 2) (color 0 1)
                                (color 5 1) (color 4 2)
    cube
