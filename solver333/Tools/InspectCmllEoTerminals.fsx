#r "../../library/bin/Debug/net8.0/Library.dll"

open System
open System.IO

let tablePath = Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, "../Data/lse-policy-v1.dat"))
let bytes = File.ReadAllBytes tablePath

let readInt offset = BitConverter.ToInt32(bytes, offset)
if bytes[0..3] <> [| byte 'R'; byte 'L'; byte 'S'; byte 'E' |] then failwith "Not an LSE policy"
if readInt 4 <> 1 then failwith "Unsupported LSE policy version"

let slots = readInt 8
let headerSize = 20
let mutable count = 0
let mutable maximum = 0
let histogram = Array.zeroCreate<int> 256

// LSE index layout is (((permutation * 32 + flip) * 4 + center) * 4 + AUF).
// CMLLEO requires flip=0 and the current Roux center convention accepts the
// two M-axis offsets with white or yellow on U: center offsets 0 and 2.
for index in 0 .. slots - 1 do
    let auf = index % 4
    let centerAndAbove = index / 4
    let center = centerAndAbove % 4
    let flipAndAbove = centerAndAbove / 4
    let flip = flipAndAbove % 32
    let distance = bytes[headerSize + index * 2]
    if flip = 0 && (center = 0 || center = 2) && distance <> Byte.MaxValue then
        count <- count + 1
        maximum <- max maximum (int distance)
        histogram[int distance] <- histogram[int distance] + 1
        ignore auf

if count <> 2880 then failwith $"Expected 2,880 CMLLEO terminals, found {count}."
printfn "CMLLEO_TERMINALS|count=%d|max_lse=%d" count maximum
histogram
|> Array.iteri (fun distance frequency ->
    if frequency <> 0 then printfn "distance=%d states=%d" distance frequency)
