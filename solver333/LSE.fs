module LSE

open Cube
open System
open System.IO

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
        newCases |> Seq.iter (fun _ ->
            count <- count + 1
            if count % 1000 = 0 then printf ".")
        yield! newCases }

    let rec genMU known cases = seq {
        printfn "Count: %i" (Map.count cases)
        //yield! gen known cases Move.E2
        //yield! gen known cases Move.D2
        yield! gen known cases Move.M2
        yield! gen known cases Move.M
        yield! gen known cases Move.M'
        yield! gen known cases Move.U2
        yield! gen known cases Move.U
        yield! gen known cases Move.U' }

    let yellowUpRedFront = Cube.executeRotation (Cube.executeRotation Cube.solved Rotate.X2) Rotate.Y
    let init = [Render.cubeToString yellowUpRedFront, ([], yellowUpRedFront)]

    let rec iter cases known =
        let initialCount = count
        let cases' = cases |> Map.ofSeq |> genMU (Map.ofSeq known) |> List.ofSeq
        if count > initialCount
        then iter cases' (known @ cases')
        else known

    let cases = iter init init
    printfn "Total Cases: %i" (List.length cases)

    let sideBlocksAligned cube = // assumes E2/D2
        Cube.look Face.L Sticker.C cube = Color.B &&
        Cube.look Face.L Sticker.DL cube = Color.B

    let casesWithBlocksSolved = cases |> List.filter (fun (_, (_, cube)) -> sideBlocksAligned cube)
    printfn "Total Cases (with blocks solved): %i" (List.length casesWithBlocksSolved)

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

    let lrAligned l r cube =
        (Cube.look Face.L Sticker.UL cube = l &&
         Cube.look Face.L Sticker.U cube = l &&
         Cube.look Face.R Sticker.UL cube = r &&
         Cube.look Face.R Sticker.U cube = r) ||
        (Cube.look Face.L Sticker.UL cube = r &&
         Cube.look Face.L Sticker.U cube = r &&
         Cube.look Face.R Sticker.UL cube = l &&
         Cube.look Face.R Sticker.U cube = l)

    let lrComplete = lrAligned Color.B Color.G
    let fbComplete cube =
        lrAligned Color.O Color.R cube &&
        Cube.look Face.U Sticker.L cube = Color.Y && 
        Cube.look Face.U Sticker.R cube = Color.Y

    let subset name file selector =
        let solution scramble =
            let mutable first = true
            let annotate cube =
                if first && lrComplete cube then first <- false; " [EOLR]"
                elif first && fbComplete cube then first <- false; " [EOFB]"
                else ""
            let cube = Cube.executeMoves scramble yellowUpRedFront
            let solution = Cube.inverseMoves scramble
            let sol =
                String.Join(' ',
                    List.scan Cube.executeMove cube solution
                    |> List.tail // not including initial scrambled state
                    |> List.zip solution
                    |> List.map (fun (m, c) -> sprintf "%s%s" (Render.moveToString m) (annotate c)))
            let i = sol.IndexOf('[')
            if i <> -1 then sprintf "%s: %s" (sol.Substring(i + 1, 4)) (sol.Substring(0, i - 1)) else sol // quick & dirty reformatting
        let sub =
            casesWithBlocksSolved
            |> Seq.filter (fun (_, (_, cube)) -> sideBlocksAligned cube)
            |> Seq.filter (fun (_, (moves, cube)) -> not (Seq.isEmpty moves) && selector cube)
            |> Seq.distinctBy fst // distinct cube state
            |> Seq.map (fun (_, (moves, _)) -> moves |> List.rev) // scrambles
            |> Seq.map (fun scramble -> scramble, solution scramble) // add annotated solution
            |> Seq.map (fun (scramble, eolr) -> sprintf "%s    (%s)" (Render.movesToString scramble) eolr)
            |> List.ofSeq
        printfn "%s (%i)" name (List.length sub)
        File.WriteAllLines(sprintf "%s.txt" file, sprintf "%s [%i cases]" name sub.Length :: "" :: sub)

    let l4e includeMisorientedCenters cube =
        let oriented =
            (if oriented Face.U Sticker.U cube then 1 else 0) +
            (if oriented Face.U Sticker.D cube then 1 else 0) +
            (if oriented Face.D Sticker.D cube then 1 else 0) +
            (if oriented Face.D Sticker.U cube then 1 else 0) +
            if centerOriented cube then 4 else 0
        lrComplete cube && Cube.look Face.L Sticker.U cube = Color.B && (oriented = 8 || (includeMisorientedCenters && oriented = 0))

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

    let diagOpposite11 front cube =
        centerOriented cube &&
        oriented Face.U Sticker.L cube &&
        oriented Face.U Sticker.R cube &&
        if front then
            misoriented Face.U Sticker.D cube &&
            misoriented Face.D Sticker.D cube &&
            oriented Face.U Sticker.U cube &&
            oriented Face.D Sticker.U cube
        else
            misoriented Face.U Sticker.U cube &&
            misoriented Face.D Sticker.U cube &&
            oriented Face.U Sticker.D cube &&
            oriented Face.D Sticker.D cube

    let diagOpposite11Good front cube =
        diagOpposite11 front cube &&
        (lrEdge Face.L Sticker.U cube || lrEdge Face.R Sticker.U cube) &&
        if front then (lrEdge Face.F Sticker.D cube || lrEdge Face.B Sticker.D cube)
        else (lrEdge Face.F Sticker.U cube || lrEdge Face.B Sticker.U cube)

    let diagOpposite11Alright front cube =
        diagOpposite11 front cube &&
        (lrEdge Face.L Sticker.U cube || lrEdge Face.R Sticker.U cube) &&
        lrEdge Face.D (if front then Sticker.D else Sticker.U) cube

    let diagOpposite11QuickEO front cube =
        diagOpposite11 front cube &&
        if front then (lrEdge Face.D Sticker.D cube && lrEdge Face.B Sticker.D cube)
        else (lrEdge Face.D Sticker.U cube && lrEdge Face.F Sticker.U cube)

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

    subset "EOLR Good 1F-1B (front)" "1f-1b_good_front" (diagOpposite11Good true)
    subset "EOLR Good 1F-1B (back)" "1f-1b_good_back" (diagOpposite11Good false)
    subset "EOLR Alright 1F-1B (front)" "1f-1b_alright_front" (diagOpposite11Alright true)
    subset "EOLR Alright 1F-1B (back)" "1f-1b_alright_back" (diagOpposite11Alright false)
    subset "EOLR Quick EO 1F-1B (front)" "1f-1b_quick_eo_front" (diagOpposite11QuickEO true)
    subset "EOLR Quick EO 1F-1B (back)" "1f-1b_quick_eo_back" (diagOpposite11QuickEO false)
