module LSE

open Cube

let generate () =
    printfn "Generating LSE Cases"

    let mutable count = 0

    let gen known cases move = seq {
        let newCases =
            cases
            |> Map.toSeq
            |> Seq.map (fun (_, cube) -> Cube.executeMove cube move) // execute move
            |> Seq.map (fun cube -> (Render.cubeToString cube), cube) // render case
            |> Seq.filter (fun (case, _) -> not (Map.containsKey case known)) // filter existing
        newCases |> Seq.iter (fun (case, _) ->
            // printfn "Case: %s" case
            count <- count + 1
            if count % 1000 = 0 then printf ".")
        yield! newCases }

    let rec genMU known cases = seq {
        printfn "Count: %i" (Map.count cases)
        yield! gen known cases Move.U
        yield! gen known cases Move.U'
        yield! gen known cases Move.U2
        yield! gen known cases Move.M'
        yield! gen known cases Move.M
        yield! gen known cases Move.M2 }

    let yellowUpRedFront = Cube.executeRotation (Cube.executeRotation Cube.solved Rotate.X2) Rotate.Y
    let init = [Render.cubeToString yellowUpRedFront, yellowUpRedFront]

    let rec iter cases known =
        let initialCount = count
        let cases' = cases |> Map.ofSeq |> genMU (Map.ofSeq known) |> List.ofSeq
        if count > initialCount
        then iter cases' (known @ cases')
        else
            printfn "Total: %i" count
            known

    let solve states cube =
        let rec solve' solution cube =
            let (_, move, cube') =
                let score cube = Map.find (Render.cubeToString cube) states |> fst
                [Move.U2; Move.M'; Move.M; Move.M2]
                |> List.map (fun m -> let cube' = Cube.executeMove cube m in (score cube'), m, cube')
                |> List.minBy (fun (s, _, _) -> s)
            if cube = yellowUpRedFront then List.rev solution else solve' ((Cube.inverseMove move) :: solution) cube'
        solve' [] cube

    let cases = iter init init

    let centerOriented cube =
        let center = Cube.look Face.U Sticker.C cube
        center = Color.Y || center = Color.W
    let cornersSolved cube = Color.B = Cube.look Face.L Sticker.UR cube
    let states = cases |> List.mapi (fun i (case, cube) -> (case, (i, cube))) |> Map.ofList
    let valid = List.filter (fun (_, cube) -> cornersSolved cube && centerOriented cube) cases |> List.distinctBy fst
    printfn "Total Cases: %i" (Seq.length valid)

    let subset =
        let lrEdge face sticker cube =
            let color = Cube.look face sticker cube
            color = Color.B || color = Color.G
        let oriented face sticker cube =
            let color = Cube.look face sticker cube
            color = Color.Y || color = Color.W
        let misoriented face sticker cube = not (oriented face sticker cube)
        let arrow cube =
            oriented Face.U Sticker.U cube &&
            misoriented Face.U Sticker.L cube &&
            misoriented Face.U Sticker.R cube &&
            misoriented Face.U Sticker.D cube &&
            misoriented Face.D Sticker.U cube &&
            oriented Face.D Sticker.D cube
        let anyAUF predicate cube =
            Cube.executeMove cube Move.U |> predicate ||
            Cube.executeMove cube Move.U' |> predicate ||
            Cube.executeMove cube Move.U2 |> predicate
        let bestArrow cube =
            anyAUF arrow cube &&
            lrEdge Face.U Sticker.D cube &&
            lrEdge Face.D Sticker.U cube
        valid
        |> Seq.filter (fun (_, cube) -> bestArrow cube)
    printfn "Total Subset: %i" (Seq.length subset)

    printfn "Scrambles:"
    for (case, cube) in subset do
        printf "Case: %s " case
        let scramble = solve states cube |> Cube.inverseMoves |> Render.movesToString
        printfn "%s" scramble

(*
L4E Cases:
M2
U2 M2 U2
M' U2 M U2
M U2 M' U2
U2 M2 U2 M2
M' U2 M' U2
M U2 M U2
U2 M' U2 M
U2 M U2 M
U2 M' U2 M'
U2 M U2 M'
M2 U2 M U2 M
M' U2 M2 U2 M
M' U2 M2 U2 M'
M2 U2 M' U2 M
M2 U2 M U2 M'
M2 U2 M' U2 M'
M U2 M' U2 M2
M' U2 M U2 M2
M U2 M U2 M2
M' U2 M' U2 M2
U2 M' U2 M2 U2 M' U2
U2 M' U2 M2 U2 M' U2 M2
*)