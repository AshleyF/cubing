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

let solve states distances cube =
    let bestTwists (states : State []) (distances : byte []) (cube : State) =
        let scoredTwists = seq {
            let score (t : Transform) = distances.[cube |> applyTransform t |> canonicalize |> findStateIndex states]
            yield ("", identityTransform, score identityTransform)
            yield ("U",  twistU,  score twistU)
            yield ("U'", twistU', score twistU')
            yield ("U2", twistU2, score twistU2)
            yield ("R",  twistR,  score twistR)
            yield ("R'", twistR', score twistR')
            yield ("R2", twistR2, score twistR2)
            yield ("F",  twistF,  score twistF)
            yield ("F'", twistF', score twistF')
            yield ("F2", twistF2, score twistF2) } |> List.ofSeq
            
        let scoreValue (_, _, s) = s
        let best = Seq.minBy scoreValue scoredTwists
        Seq.filter (fun x -> scoreValue x = scoreValue best) scoredTwists
    let rec solve' c =
        let (n, t, s) = bestTwists states distances c |> Seq.head
        printf "%s " n
        if t <> identityTransform then applyTransform t c |> solve' else c
    cube |> solve'

let faces ((p, o) as cube : State) =
    let face c f = [|[|'U'; 'F'; 'R'|]; [|'U'; 'R'; 'B'|]; [|'U'; 'B'; 'L'|]; [|'U'; 'L'; 'F'|]; [|'D'; 'R'; 'F'|]; [|'D'; 'F'; 'L'|]; [|'D'; 'L'; 'B'|]; [|'D'; 'B'; 'R'|]|].[p.[c]].[(o.[c] + f) % 3]
    [                           (face 6 2); (face 7 1);
                                (face 2 1); (face 1 2);
        (face 6 1); (face 2 2); (face 2 0); (face 1 0); (face 1 1); (face 7 2); (face 7 0); (face 6 0);
        (face 5 2); (face 3 1); (face 3 0); (face 0 0); (face 0 2); (face 4 1); (face 4 0); (face 5 0);
                                (face 3 2); (face 0 1);
                                (face 5 1); (face 4 2)]
;
let display label cube =
    let f = faces cube
    printfn "%s:\n  %c%c\n  %c%c\n%c%c%c%c%c%c%c%c\n%c%c%c%c%c%c%c%c\n  %c%c\n  %c%c" label
            f.[0] f.[1] f.[2] f.[3] f.[4] f.[5] f.[6] f.[7] f.[8] f.[9] f.[10] f.[11] f.[12]
            f.[13] f.[14] f.[15] f.[16] f.[17] f.[18] f.[19] f.[20] f.[21] f.[22] f.[23]
    cube

let exportForTensorflow (states : State []) (distances : byte []) (name : string) (num : int) =
    let dataFile = "../../../models/tensorflow/data.csv"
    let labelsFile = "../../../models/tensorflow/labels.csv"
    if not (File.Exists dataFile && File.Exists labelsFile) then
        printfn "Exporting data/labels.csv for TensorFlow..."
        let maxDist = Seq.max distances |> double
        use data = new StreamWriter(dataFile)
        use labels = new StreamWriter(labelsFile)
        let export' n =
            let d = double distances.[n] / maxDist
            let f i = if int distances.[n] = i then 1 else 0
            let (perm, orient) = states.[n]
            let p i = double perm.[i] / 7.0
            let o i = double orient.[i] / 2.0
            data.WriteLine(sprintf "%f,%f,%f,%f,%f,%f,%f,%f,%f,%f,%f,%f,%f,%f,%f,%f"
                                    (p 0) (p 1) (p 2) (p 3) (p 4) (p 5) (p 6) (p 7)
                                    (o 0) (o 1) (o 2) (o 3) (o 4) (o 5) (o 6) (o 7))
            labels.WriteLine(sprintf "%i,%i,%i,%i,%i,%i,%i,%i,%i,%i,%i,%i" (f 0) (f 1) (f 2) (f 3) (f 4) (f 5) (f 6) (f 7) (f 8) (f 9) (f 10) (f 11)) // (f 12) (f 13) (f 14) // max 11 HTM, 14 QTM)
        for n in 0 .. (num - 1) do
            export' n

let entropy cube =
    let f = faces cube
    let facePairs = [f.[0] = f.[1]; f.[0] = f.[2]; f.[1] = f.[3]; f.[2] = f.[3]
                     f.[4] = f.[5]; f.[4] = f.[12]; f.[5] = f.[13]; f.[12] = f.[13]
                     f.[6] = f.[7]; f.[6] = f.[14]; f.[7] = f.[15]; f.[14] = f.[15]
                     f.[8] = f.[9]; f.[8] = f.[16]; f.[9] = f.[17]; f.[16] = f.[17]
                     f.[10] = f.[11]; f.[10] = f.[18]; f.[11] = f.[19]; f.[18] = f.[19]
                     f.[20] = f.[21]; f.[20] = f.[22]; f.[21] = f.[23]; f.[22] = f.[23]]
    let facePairEntropy = facePairs |> Seq.filter ((=) false) |> Seq.length
    let faceQuads = [f.[0] = f.[1] && f.[1] = f.[2] && f.[2] = f.[3]
                     f.[4] = f.[5] && f.[5] = f.[12] && f.[12] = f.[13]
                     f.[6] = f.[7] && f.[7] = f.[14] && f.[14] = f.[15]
                     f.[8] = f.[9] && f.[9] = f.[16] && f.[16] = f.[17]
                     f.[10] = f.[11] && f.[11] = f.[18] && f.[18] = f.[19]
                     f.[20] = f.[21] && f.[21] = f.[22] && f.[22] = f.[23]]
    let faceQuadEntropy = faceQuads |> Seq.filter ((=) false) |> Seq.length
    double facePairEntropy + double faceQuadEntropy * 2.
