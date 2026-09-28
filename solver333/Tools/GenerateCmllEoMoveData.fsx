#r "../../library/bin/Debug/net8.0/Library.dll"

open System
open System.IO
open System.Text.Json
open System.Text
open Cube

let edgePositions = [| Edge.UR; Edge.UF; Edge.UL; Edge.UB; Edge.DR; Edge.DF; Edge.DL; Edge.DB; Edge.FR; Edge.FL; Edge.BL; Edge.BR |]
let cornerPositions = [| Corner.URF; Corner.ULF; Corner.ULB; Corner.URB; Corner.DRF; Corner.DLF; Corner.DLB; Corner.DRB |]
let centerPositions = [| Center.U; Center.D; Center.L; Center.R; Center.F; Center.B |]
let faces = [| Face.U; Face.D; Face.L; Face.R; Face.F; Face.B |]
let stickers = [| Sticker.UL; Sticker.U; Sticker.UR; Sticker.L; Sticker.C; Sticker.R; Sticker.DL; Sticker.D; Sticker.DR |]
let facelets = [| for face in faces do for sticker in stickers do yield face, sticker |]
let colors = [| Color.R; Color.O; Color.W; Color.Y; Color.B; Color.G; Color.A |]

let colorsAt locations cube = locations |> List.map (fun (face, sticker) -> Cube.look face sticker cube)
let edgeColors edge cube = colorsAt (Cube.edgeToFaceStickers edge) cube
let cornerFacelets = function
    | Corner.URF -> [Face.U, Sticker.DR; Face.R, Sticker.UL; Face.F, Sticker.UR]
    | Corner.ULF -> [Face.U, Sticker.DL; Face.F, Sticker.UL; Face.L, Sticker.UR]
    | Corner.ULB -> [Face.U, Sticker.UL; Face.L, Sticker.UL; Face.B, Sticker.DL]
    | Corner.URB -> [Face.U, Sticker.UR; Face.B, Sticker.DR; Face.R, Sticker.UR]
    | Corner.DRF -> [Face.D, Sticker.UR; Face.F, Sticker.DR; Face.R, Sticker.DL]
    | Corner.DLF -> [Face.D, Sticker.UL; Face.L, Sticker.DR; Face.F, Sticker.DL]
    | Corner.DLB -> [Face.D, Sticker.DL; Face.B, Sticker.UL; Face.L, Sticker.DL]
    | Corner.DRB -> [Face.D, Sticker.DR; Face.R, Sticker.DR; Face.B, Sticker.UR]
let cornerColors corner cube = colorsAt (cornerFacelets corner) cube
let centerColor center cube = let face, sticker = Cube.centerToFaceSticker center in Cube.look face sticker cube
let sameColors left right = List.sort left = List.sort right
let isUpDown = function Color.W | Color.Y -> true | _ -> false

let solvedEdges = edgePositions |> Array.map (fun position -> edgeColors position Cube.solved)
let solvedCorners = cornerPositions |> Array.map (fun position -> cornerColors position Cube.solved)
let solvedCenters = centerPositions |> Array.map (fun position -> centerColor position Cube.solved)

let moveNames = [| "U"; "U'"; "U2"; "R"; "R'"; "R2"; "r"; "r'"; "r2"; "F"; "F'"; "F2" |]

type MoveData =
    { name: string
      edgeSource: int array
      edgeFlip: int array
      cornerSource: int array
      cornerTwist: int array
      centerSource: int array
      faceletSource: int array }

let probe digit =
    facelets
    |> Array.mapi (fun index (face, sticker) -> face, (sticker, colors[(index / pown 7 digit) % 7]))
    |> Array.groupBy fst
    |> Array.map (fun (face, values) -> face, (values |> Array.map snd |> Map.ofArray))
    |> Map.ofArray

let probes = Array.init 3 probe

let describe name =
    let moved = Cube.solved |> Cube.executeSteps (Render.stringToSteps name)
    let edgeSource = Array.zeroCreate 12
    let edgeFlip = Array.zeroCreate 12
    for destination in 0 .. 11 do
        let actual = edgeColors edgePositions[destination] moved
        let source = solvedEdges |> Array.findIndex (sameColors actual)
        edgeSource[destination] <- source
        edgeFlip[destination] <- if actual[0] = solvedEdges[source][0] then 0 else 1
    let cornerSource = Array.zeroCreate 8
    let cornerTwist = Array.zeroCreate 8
    for destination in 0 .. 7 do
        let actual = cornerColors cornerPositions[destination] moved
        cornerSource[destination] <- solvedCorners |> Array.findIndex (sameColors actual)
        cornerTwist[destination] <- actual |> List.findIndex isUpDown
    let centerSource = Array.zeroCreate 6
    for destination in 0 .. 5 do
        let actual = centerColor centerPositions[destination] moved
        centerSource[destination] <- solvedCenters |> Array.findIndex ((=) actual)
    let movedProbes = probes |> Array.map (fun cube -> cube |> Cube.executeSteps (Render.stringToSteps name))
    let faceletSource =
        facelets
        |> Array.map (fun (face, sticker) ->
            let digits = movedProbes |> Array.map (Cube.look face sticker >> fun color -> colors |> Array.findIndex ((=) color))
            digits[0] + 7 * digits[1] + 49 * digits[2])
    if faceletSource |> Array.exists (fun source -> source < 0 || source >= 54) then failwith $"Invalid facelet permutation for {name}."
    if faceletSource |> Array.distinct |> Array.length <> 54 then failwith $"Non-bijective facelet permutation for {name}."
    { name = name
      edgeSource = edgeSource
      edgeFlip = edgeFlip
      cornerSource = cornerSource
      cornerTwist = cornerTwist
      centerSource = centerSource
      faceletSource = faceletSource }

let transforms = moveNames |> Array.map describe
let solvedEdgesState = [|0 .. 11|]
let solvedFlipsState = Array.zeroCreate 12
let solvedCornersState = [|0 .. 7|]
let solvedTwistsState = Array.zeroCreate 8
let random = Random 3332026
for sample in 1 .. 1000 do
    let length = random.Next(1, 40)
    let indices = Array.init length (fun _ -> random.Next transforms.Length)
    let movedProbes =
        probes
        |> Array.map (fun probe -> indices |> Array.fold (fun cube index -> Cube.executeSteps (Render.stringToSteps moveNames[index]) cube) probe)
    let expected =
        facelets
        |> Array.map (fun (face, sticker) ->
            let digits = movedProbes |> Array.map (Cube.look face sticker >> fun color -> colors |> Array.findIndex ((=) color))
            digits[0] + 7 * digits[1] + 49 * digits[2])
    let actual =
        indices
        |> Array.fold (fun state index -> Array.init 54 (fun destination -> state[transforms[index].faceletSource[destination]])) [|0 .. 53|]
    if actual <> expected then failwith $"Compact transform validation failed for sample {sample}."

    let expectedCube = indices |> Array.fold (fun cube index -> Cube.executeSteps (Render.stringToSteps moveNames[index]) cube) Cube.solved
    let expectedEdges = edgePositions |> Array.map (fun position -> let value = edgeColors position expectedCube in solvedEdges |> Array.findIndex (sameColors value))
    let expectedFlips = edgePositions |> Array.map (fun position -> let value = edgeColors position expectedCube in let source = solvedEdges |> Array.findIndex (sameColors value) in if value[0] = solvedEdges[source][0] then 0 else 1)
    let expectedCorners = cornerPositions |> Array.map (fun position -> let value = cornerColors position expectedCube in solvedCorners |> Array.findIndex (sameColors value))
    let expectedTwists = cornerPositions |> Array.map (fun position -> cornerColors position expectedCube |> List.findIndex isUpDown)
    let actualEdges, actualFlips, actualCorners, actualTwists =
        indices
        |> Array.fold (fun (edges, flips, corners, twists) index ->
            let transform = transforms[index]
            (Array.init 12 (fun destination -> edges[transform.edgeSource[destination]]),
             Array.init 12 (fun destination -> flips[transform.edgeSource[destination]] ^^^ transform.edgeFlip[destination]),
             Array.init 8 (fun destination -> corners[transform.cornerSource[destination]]),
             Array.init 8 (fun destination -> (twists[transform.cornerSource[destination]] + transform.cornerTwist[destination]) % 3)))
            (solvedEdgesState, solvedFlipsState, solvedCornersState, solvedTwistsState)
    if actualEdges <> expectedEdges || actualFlips <> expectedFlips || actualCorners <> expectedCorners || actualTwists <> expectedTwists then
        failwith $"Cubie transform validation failed for sample {sample}."

let output =
    {| version = 1
       metric = "STM"
       alphabet = "U,R,r,F (all powers)"
       moves = transforms |}

let path = Path.Combine(__SOURCE_DIRECTORY__, "..", "Data", "cmll-eo-moves-v1.json")
File.WriteAllText(path, JsonSerializer.Serialize(output, JsonSerializerOptions(WriteIndented = true)))

let header = StringBuilder()
header.AppendLine("/* Generated by GenerateCmllEoMoveData.fsx; do not edit. */") |> ignore
header.AppendLine("#ifndef ROUXLAB_CMLLEO_MOVES_V1_H") |> ignore
header.AppendLine("#define ROUXLAB_CMLLEO_MOVES_V1_H") |> ignore
header.AppendLine("#include <stdint.h>") |> ignore
header.AppendLine($"#define CMLLEO_MOVE_COUNT {transforms.Length}") |> ignore
header.AppendLine("static const char *const cmll_eo_move_names[CMLLEO_MOVE_COUNT] = {") |> ignore
header.AppendLine(transforms |> Array.map (fun transform -> sprintf "  \"%s\"" transform.name) |> String.concat ",\n") |> ignore
header.AppendLine("};") |> ignore
header.AppendLine("static const uint8_t cmll_eo_facelet_source[CMLLEO_MOVE_COUNT][54] = {") |> ignore
for transform in transforms do
    header.AppendLine("  { " + (transform.faceletSource |> Array.map string |> String.concat ", ") + " },") |> ignore
header.AppendLine("};") |> ignore
let appendMatrix name width selector =
    header.AppendLine($"static const uint8_t {name}[CMLLEO_MOVE_COUNT][{width}] = {{") |> ignore
    for transform in transforms do
        header.AppendLine("  { " + (selector transform |> Array.map string |> String.concat ", ") + " },") |> ignore
    header.AppendLine("};") |> ignore
appendMatrix "cmll_eo_edge_source" 12 (fun transform -> transform.edgeSource)
appendMatrix "cmll_eo_edge_flip" 12 (fun transform -> transform.edgeFlip)
appendMatrix "cmll_eo_corner_source" 8 (fun transform -> transform.cornerSource)
appendMatrix "cmll_eo_corner_twist" 8 (fun transform -> transform.cornerTwist)
appendMatrix "cmll_eo_center_source" 6 (fun transform -> transform.centerSource)
header.AppendLine("#endif") |> ignore
let headerPath = Path.Combine(__SOURCE_DIRECTORY__, "CmllEoMoves.generated.h")
File.WriteAllText(headerPath, header.ToString())
printfn "CMLLEO_MOVE_DATA|%s|%s|moves=%d" path headerPath moveNames.Length
