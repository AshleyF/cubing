module CMLL

open Cube
open System
open System.IO

printfn "Generating CMLL Cases"

let mutable count = 0

let gen known cases (name, alg) = seq {
    let cubeToString (cube: Map<Face, Map<Sticker, Color>>) =
        let str = seq {
            yield look Face.U Sticker.UL cube |> Render.colorToString
            yield look Face.U Sticker.UR cube |> Render.colorToString
            yield look Face.U Sticker.DL cube |> Render.colorToString
            yield look Face.U Sticker.DR cube |> Render.colorToString
            yield look Face.F Sticker.UL cube |> Render.colorToString
            yield look Face.F Sticker.UR cube |> Render.colorToString
            yield look Face.R Sticker.UL cube |> Render.colorToString
            yield look Face.R Sticker.UR cube |> Render.colorToString
            yield look Face.B Sticker.DL cube |> Render.colorToString
            yield look Face.B Sticker.DR cube |> Render.colorToString
            yield look Face.L Sticker.UL cube |> Render.colorToString
            yield look Face.L Sticker.UR cube |> Render.colorToString }
        String.Concat(str)
    let newCases =
        cases
        |> Map.toSeq
        |> Seq.map (fun (_, (moves, cube)) -> (List.rev alg @ moves), Cube.executeSteps alg cube) // execute alg
        |> Seq.map (fun (moves, cube) -> (cubeToString cube), (moves, cube)) // render case
        |> Seq.filter (fun (case, _) -> not (Map.containsKey case known)) // filter existing
    newCases |> Seq.iter (fun _ ->
        count <- count + 1
        if count % 1000 = 0 then printf ".")
    yield! newCases }

let rec genCMLL known cases = seq {
    printfn "Count: %i" (Map.count cases)
    yield! gen known cases ("JPerm", Render.stringToSteps "R U R' F' R U R' U' R' F R2 U' R'")
    yield! gen known cases ("Sune", Render.stringToSteps "R U R' U R U2 R'")
    yield! gen known cases ("U", Render.stringToSteps "U")
    yield! gen known cases ("U'", Render.stringToSteps "U'")
    yield! gen known cases ("U2", Render.stringToSteps "U2")
    }

let yellowUpRedFront = Cube.executeRotation (Cube.executeRotation Cube.solved Rotate.X2) Rotate.Y
let init = [Render.cubeToString yellowUpRedFront, ([], yellowUpRedFront)]

let rec iter cases known =
    let initialCount = count
    let cases' = cases |> Map.ofSeq |> genCMLL (Map.ofSeq known) |> List.ofSeq
    if count > initialCount
    then iter cases' (known @ cases')
    else known

let cases = iter init init
printfn "Total Cases: %i" (List.length cases)

let subset name file (index : StreamWriter) (diagrams: StreamWriter) selector =
    diagrams.WriteLine(sprintf "sub(\"%s\");" name)
    let sub =
        cases
        |> Seq.filter (fun (_, (moves, cube)) -> not (Seq.isEmpty moves) && selector cube)
        |> Seq.distinctBy fst // distinct cube state
        |> Seq.map (fun (_, (moves, _)) -> moves |> List.rev) // scrambles
        |> Seq.map (fun scramble -> scramble, yellowUpRedFront |> Cube.executeSteps scramble |> Render.cubeToString) // add cube rendering
        |> Seq.map (fun (scramble, rendered) ->
            let alg = Render.stepsToString (inverseSteps scramble)
            diagrams.WriteLine(sprintf "diag(\"%s\");" alg)
            sprintf "1. `%s` (%s)" rendered (alg))
        |> List.ofSeq
    printfn "%s (%i)" name (List.length sub)
    File.WriteAllLines(sprintf "../../../%s.md" file, sprintf "# %s [%i cases]" name sub.Length :: "" :: sub)
    index.WriteLine(sprintf "- [%s](%s.md)" name file)

let oriented cube =
    let boyCorner c = c = Color.B || c = Color.O || c = Color.Y
    Cube.look Face.U Sticker.UL cube |> boyCorner &&
    Cube.look Face.L Sticker.UL cube |> boyCorner &&
    Cube.look Face.B Sticker.DL cube |> boyCorner

let algSolvesCmll alg cube =
    let isCmllSolved cube =
        let isFBSolved cube = 
            Cube.look Face.L Sticker.L  cube = Color.B &&
            Cube.look Face.L Sticker.C  cube = Color.B &&
            Cube.look Face.L Sticker.R  cube = Color.B &&
            Cube.look Face.L Sticker.DL cube = Color.B &&
            Cube.look Face.L Sticker.D  cube = Color.B &&
            Cube.look Face.L Sticker.DR cube = Color.B &&
            Cube.look Face.F Sticker.L  cube = Color.R &&
            Cube.look Face.F Sticker.DL cube = Color.R &&
            Cube.look Face.B Sticker.L  cube = Color.O &&
            Cube.look Face.B Sticker.UL cube = Color.O
        let isSBSolved cube =
            Cube.look Face.R Sticker.L  cube = Color.G &&
            Cube.look Face.R Sticker.C  cube = Color.G &&
            Cube.look Face.R Sticker.R  cube = Color.G &&
            Cube.look Face.R Sticker.DL cube = Color.G &&
            Cube.look Face.R Sticker.D  cube = Color.G &&
            Cube.look Face.R Sticker.DR cube = Color.G &&
            Cube.look Face.F Sticker.R  cube = Color.R &&
            Cube.look Face.F Sticker.DR cube = Color.R &&
            Cube.look Face.B Sticker.R  cube = Color.O &&
            Cube.look Face.B Sticker.UR cube = Color.O
        oriented cube &&
        isFBSolved cube &&
        isSBSolved cube &&
        Cube.look Face.U Sticker.UL cube = Color.Y &&
        Cube.look Face.U Sticker.UR cube = Color.Y &&
        Cube.look Face.U Sticker.DL cube = Color.Y &&
        Cube.look Face.U Sticker.DR cube = Color.Y &&
        Cube.look Face.F Sticker.UL cube = Cube.look Face.F Sticker.UR cube &&
        Cube.look Face.L Sticker.UL cube = Cube.look Face.L Sticker.UR cube &&
        Cube.look Face.R Sticker.UL cube = Cube.look Face.R Sticker.UR cube &&
        Cube.look Face.B Sticker.DL cube = Cube.look Face.B Sticker.DR cube
    cube |> Cube.executeSteps alg |> isCmllSolved ||
    Cube.executeMove cube Move.U  |> Cube.executeSteps alg |> isCmllSolved ||
    Cube.executeMove cube Move.U' |> Cube.executeSteps alg |> isCmllSolved ||
    Cube.executeMove cube Move.U2 |> Cube.executeSteps alg |> isCmllSolved

let case alg = algSolvesCmll (Render.stringToSteps alg)

let index = new StreamWriter(File.OpenWrite "../../../CMLL.md")
let diagrams = new StreamWriter(File.OpenWrite "../../../Diagrams.js")

let writeCaseHeader name =
    index.WriteLine()
    index.WriteLine(sprintf "## %s Cases" name)
    index.WriteLine()
    diagrams.WriteLine(sprintf "head(\"%s Cases\");" name)
                    
let writeCase name file alg = case alg |> subset name file index diagrams

index.WriteLine("# CMLLs ([generated by](./CMLL.fs))")

writeCaseHeader "O"
writeCase "O-Adjacent Swap" "o_adjacent_swap" "R U R' F' R U R' U' R' F R2 U' R'"
writeCase "O-Diagonal Swap" "o_diagonal_swap" "r2 D r' U r D' R2 U' F' U' F"

writeCaseHeader "H"
writeCase "H-Columns" "h_columns" "R U R' U R U' R' U R U2 R'"
writeCase "H-Rows"    "h_rows"    "F R U R' U' R U R' U' R U R' U' F'"
writeCase "H-Column"  "h_column"  "U R U2' R2' F R F' U2 R' F R F'"
writeCase "H-Row"     "h_row"     "r U' r2' D' r U' r' D r2 U r'"

writeCaseHeader "Pi"
writeCase "Pi-Right-Bar"      "pi_right_bar"      "F R U R' U' R U R' U' F'"
writeCase "Pi-Back-Slash"     "pi_back_slash"     "U F R' F' R U2 R U' R' U R U2' R'"
writeCase "Pi-X-Checkerboard" "pi_x_checkerboard" "U' R' F R U F U' R U R' U' F'"
writeCase "Pi-Forward-Slash"  "pi_forward_slash"  "R U2 R' U' R U R' U2' R' F R F'"
writeCase "Pi-Columns"        "pi_columns"        "U' r U' r2' D' r U r' D r2 U r'"
writeCase "Pi-Left-Bar"       "pi_left_bar"       "U' R' U' R' F R F' R U' R' U2 R"

writeCaseHeader "U"
writeCase "U-Forward-Slash" "u_forward_slash" "U2 R2 D R' U2 R D' R' U2 R'"
writeCase "U-Back-Slash" "u_back_slash" "R2' D' R U2 R' D R U2 R"
writeCase "U-Front-Row" "u_front_row" "R2' F U' F U F2 R2 U' R' F R"
writeCase "U-Rows" "u_rows" "U' F R2 D R' U R D' R2' U' F'"
writeCase "U-X-Checkerboard" "u_x_checkerboard" "U2 r U' r' U r' D' r U' r' D r"
writeCase "U-Back-Row" "u_back_row" "U' F R U R' U' F'"

writeCaseHeader "T"
writeCase "T-Left-Bar" "t_left_bar" "U' R U R' U' R' F R F'"
writeCase "T-Right-Bar" "t_right_bar" "U L' U' L U L F' L' F"
writeCase "T-Rows" "t_rows" "F R' F R2 U' R' U' R U R' F2"
writeCase "T-Front-Row" "t_front_row" "r' U r U2' R2' F R F' R"
writeCase "T-Back-Row" "t_back_row" "r' D' r U r' D r U' r U r'"
writeCase "T-Columns" "t_columns" "U2 r2' D' r U r' D r2 U' r' U' r"

writeCaseHeader "S"
writeCase "S-Left-Bar" "s_left_bar" "U R U R' U R U2 R'"
writeCase "S-X-Checkerboard" "s_x_checkerboard" "U L' U2 L U2' L F' L' F"
writeCase "S-Forward-Slash" "s_forward_slash" "U F R' F' R U2 R U2' R'"
writeCase "S-Columns" "s_columns" "U2 R' U' R U' R2' F' R U R U' R' F U2' R"
writeCase "S-Right-Bar" "s_right_bar" "U' R U R' U R' F R F' R U2' R'"
writeCase "S-Back-Slash" "s_back_slash" "U R U' L' U R' U' L"

writeCaseHeader "AS"
writeCase "AS-Right-Bar" "as_right_bar" "U R' U' R U' R' U2' R"
writeCase "AS-Columns" "as_columns" "U' R2 D R' U R D' R' U R' U' R U' R'"
writeCase "AS-Back-Slash" "as_back_slash" "U' F' L F L' U2' L' U2 L"
writeCase "AS-X-Checkerboard" "as_x_checkerboard" "U' R U2' R' U2 R' F R F'"
writeCase "AS-Forward-Slash" "as_forward_slash" "U' L' U R U' L U R'"
writeCase "AS-Left-Bar" "as_left_bar" "R' U' R U' L U' R' U L' U2 R"

writeCaseHeader "L"
writeCase "L-Mirror" "l_mirror" "F R U' R' U' R U R' F'"
writeCase "L-Inverse" "l_inverse" "F R' F' R U R U' R'"
writeCase "L-Pure" "l_pure" "R U2 R' U' R U R' U' R U R' U' R U' R'"
writeCase "L-Front_commutator" "l_front_commutator" "R U2 R D R' U2 R D' R2'"
writeCase "L-Diag" "l_diag" "R' U' R U R' F' R U R' U' R' F R2"
writeCase "L-Back-Commutator" "l_back_commutator" "U R' U2 R' D' R U2 R' D R2"

index.Close()
diagrams.Close();
