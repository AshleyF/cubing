module LSE

open Cube

let generate () =
    printfn "Generating LSE Cases"

    let mutable count = 0

    let gen known cases move = seq {
        let newCases =
            cases
            |> Map.toSeq
            |> Seq.map (fun (_, (moves, cube)) -> (move :: moves), Cube.executeMove cube move) // execute move
            |> Seq.map (fun (moves, cube) -> (Render.cubeToString cube), (moves, cube)) // render case
            |> Seq.filter (fun (case, _) -> not (Map.containsKey case known)) // filter existing
        newCases |> Seq.iter (fun (case, _) ->
            count <- count + 1
            if count % 1000 = 0 then printf ".")
        yield! newCases }

    let rec genMU known cases = seq {
        printfn "Count: %i" (Map.count cases)
        yield! gen known cases Move.U'
        yield! gen known cases Move.U
        yield! gen known cases Move.U2
        yield! gen known cases Move.M'
        yield! gen known cases Move.M
        yield! gen known cases Move.M2 }

    let yellowUpRedFront = Cube.executeRotation (Cube.executeRotation Cube.solved Rotate.X2) Rotate.Y
    let init = [Render.cubeToString yellowUpRedFront, ([], yellowUpRedFront)]

    let rec iter cases known =
        let initialCount = count
        let cases' = cases |> Map.ofSeq |> genMU (Map.ofSeq known) |> List.ofSeq
        if count > initialCount
        then iter cases' (known @ cases')
        else known

    let cases = iter init init
    printfn "Total Cases: %i" (Seq.length cases)

    let cornersSolved cube = Color.B = Cube.look Face.L Sticker.UR cube
    let anyAUF predicate cube =
        cube |> predicate ||
        Cube.executeMove cube Move.U |> predicate ||
        Cube.executeMove cube Move.U' |> predicate ||
        Cube.executeMove cube Move.U2 |> predicate

    let centerOriented cube =
        let center = Cube.look Face.U Sticker.C cube
        center = Color.Y || center = Color.W

    let lrEdge face sticker cube =
        let color = Cube.look face sticker cube
        color = Color.B || color = Color.G

    let oriented face sticker cube =
        let color = Cube.look face sticker cube
        color = Color.Y || color = Color.W

    let misoriented face sticker cube = not (oriented face sticker cube)

    let subset name file selector =
        let sub =
            cases
            |> Seq.filter (fun (_, (_, cube)) -> selector cube)
            |> Seq.distinctBy fst
            |> Seq.map (fun (_, (moves, _)) -> moves |> List.rev |> Render.movesToString)
            |> Seq.filter ((<>) "")
            |> List.ofSeq
        printfn "%s (%i)" name (List.length sub)
        System.IO.File.WriteAllLines(sprintf "%s.txt" file, sprintf "%s [%i cases]" name sub.Length :: "" :: sub)

    let l4e includeMisorientedCenters cube =
        let oriented =
            (if oriented Face.U Sticker.U cube then 1 else 0) +
            (if oriented Face.U Sticker.D cube then 1 else 0) +
            (if oriented Face.D Sticker.D cube then 1 else 0) +
            (if oriented Face.D Sticker.U cube then 1 else 0) +
            if centerOriented cube then 4 else 0
        cornersSolved cube &&
        Cube.look Face.L Sticker.U cube = Color.B &&
        Cube.look Face.R Sticker.U cube = Color.G &&
        (oriented = 8 || (includeMisorientedCenters && oriented = 0))

    let arrow front cube =
        centerOriented cube &&
        misoriented Face.U Sticker.L cube &&
        misoriented Face.U Sticker.R cube &&
        if front then
            oriented Face.U Sticker.U cube &&
            misoriented Face.U Sticker.D cube &&
            misoriented Face.D Sticker.U cube
        else
            misoriented Face.U Sticker.U cube &&
            oriented Face.U Sticker.D cube &&
            oriented Face.D Sticker.U cube

    let arrowBest front cube =
        arrow front cube &&
        if front then
            lrEdge Face.U Sticker.D cube &&
            lrEdge Face.D Sticker.U cube
        else
            lrEdge Face.U Sticker.U cube &&
            lrEdge Face.D Sticker.D cube

    let arrowGood front cube =
        arrow front cube &&
        (lrEdge Face.U Sticker.L cube || lrEdge Face.U Sticker.R cube) &&
        if front then (lrEdge Face.B Sticker.U cube || lrEdge Face.B Sticker.D cube)
        else (lrEdge Face.F Sticker.U cube || lrEdge Face.F Sticker.D cube)

    let arrowAdjacent front cube =
        arrow front cube &&
        (lrEdge Face.U Sticker.L cube || lrEdge Face.U Sticker.R cube) &&
        if front then lrEdge Face.U Sticker.D cube
        else lrEdge Face.U Sticker.U cube

    let arrowBottom front cube =
        arrow front cube &&
        if front then
            lrEdge Face.D Sticker.U cube &&
            lrEdge Face.B Sticker.U cube
        else
            lrEdge Face.D Sticker.D cube &&
            lrEdge Face.F Sticker.D cube

    let arrowBad front cube =
        arrow front cube &&
        not (arrowBest front cube) &&
        not (arrowGood front cube) &&
        not (arrowAdjacent front cube) &&
        not (arrowBottom front cube)

    subset "L4E (oriented centers)" "l4e_oriented" (l4e false)
    subset "L4E (including misoriented centers)" "l4e_misoriented" (l4e true)

    subset "EOLR Best Arrow (front)" "arrow_best_front" (arrowBest true)
    subset "EOLR Best Arrow (back)" "arrow_best_back" (arrowBest false)
    subset "EOLR Good Arrow (front)" "arrow_good_front" (arrowGood true)
    subset "EOLR Good Arrow (back)" "arrow_good_back" (arrowGood false)
    subset "EOLR Adjacent Arrow (front)" "arrow_adjacent_front" (arrowAdjacent true)
    subset "EOLR Adjacent Arrow (back)" "arrow_adjacent_back" (arrowAdjacent false)
    subset "EOLR Bottom Arrow (front)" "arrow_bottom_front" (arrowBottom true)
    subset "EOLR Bottom Arrow (back)" "arrow_bottom_back" (arrowBottom false)
    subset "EOLR Bad Arrow (front)" "arrow_bad_front" (arrowBad true)
    subset "EOLR Bad Arrow (back)" "arrow_bad_back" (arrowBad false)
