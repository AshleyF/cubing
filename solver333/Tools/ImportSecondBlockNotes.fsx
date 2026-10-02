#r "../../library/bin/Release/net8.0/Library.dll"

open System
open System.IO
open System.Text.RegularExpressions
open Cube

let root = Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, "..", ".."))
let notesFile = Path.Combine(root, "site", "notes", "sb.html")
let patternsDirectory = Path.Combine(root, "solver333", "Patterns", "Roux", "Intermediate")
let write = fsi.CommandLineArgs |> Array.contains "--write"

let normalize (algorithm: string) =
    Regex.Replace(algorithm.Trim(), @"\s+", " ")
    |> fun value -> Regex.Replace(value, @"([URFDLBMESxyzurfdlb])2'", "$12")

let algorithms =
    Regex.Matches(File.ReadAllText notesFile, """\balg=([^|"]+?)(?:\s*\||")""")
    |> Seq.cast<Match>
    |> Seq.map (fun matched -> matched.Groups[1].Value.Trim())
    |> Seq.distinctBy normalize
    |> Seq.toList

let yellowUpRedFront = Cube.executeRotation (Cube.executeRotation Cube.solved Rotate.X2) Rotate.Y

let solvedFB cube =
    Cube.look Face.L Sticker.C cube = Color.B &&
    Cube.look Face.L Sticker.D cube = Color.B &&
    Cube.look Face.D Sticker.L cube = Color.W &&
    Cube.look Face.L Sticker.R cube = Color.B &&
    Cube.look Face.L Sticker.DR cube = Color.B &&
    Cube.look Face.D Sticker.UL cube = Color.W &&
    Cube.look Face.F Sticker.L cube = Color.R &&
    Cube.look Face.F Sticker.DL cube = Color.R &&
    Cube.look Face.L Sticker.L cube = Color.B &&
    Cube.look Face.L Sticker.DL cube = Color.B &&
    Cube.look Face.D Sticker.DL cube = Color.W &&
    Cube.look Face.B Sticker.UL cube = Color.O &&
    Cube.look Face.B Sticker.L cube = Color.O

let solvedDR cube =
    solvedFB cube &&
    Cube.look Face.R Sticker.C cube = Color.G &&
    Cube.look Face.R Sticker.D cube = Color.G &&
    Cube.look Face.D Sticker.R cube = Color.W

let solvedRF cube =
    solvedDR cube &&
    Cube.look Face.R Sticker.L cube = Color.G &&
    Cube.look Face.R Sticker.DL cube = Color.G &&
    Cube.look Face.D Sticker.UR cube = Color.W &&
    Cube.look Face.F Sticker.R cube = Color.R &&
    Cube.look Face.F Sticker.DR cube = Color.R

let solvedRB cube =
    solvedDR cube &&
    Cube.look Face.R Sticker.R cube = Color.G &&
    Cube.look Face.R Sticker.DR cube = Color.G &&
    Cube.look Face.D Sticker.DR cube = Color.W &&
    Cube.look Face.B Sticker.R cube = Color.O &&
    Cube.look Face.B Sticker.DR cube = Color.O

type Bank =
    { Name: string
      File: string
      Preconditions: Cube -> bool
      Goal: Cube -> bool }

let banks =
    [ { Name = "RB first"; File = "InsertRBPair.txt"; Preconditions = solvedDR; Goal = solvedRB }
      { Name = "RF last"; File = "InsertRFPair.txt"; Preconditions = solvedRB; Goal = fun cube -> solvedRB cube && solvedRF cube }
      { Name = "RF first"; File = "InsertRFFirstPair.txt"; Preconditions = solvedDR; Goal = solvedRF }
      { Name = "RB last"; File = "InsertRBLastPair.txt"; Preconditions = solvedRF; Goal = fun cube -> solvedRF cube && solvedRB cube } ]

let splitLine (line: string) =
    let comma = line.IndexOf(',')
    if comma < 0 then line, []
    else
        line.Substring(0, comma),
        line.Substring(comma + 1).Split(';', StringSplitOptions.RemoveEmptyEntries)
        |> Array.map _.Trim()
        |> Array.toList

let mutable additions = []
let mutable applicableAlgorithms = Set.empty

for bank in banks do
    let path = Path.Combine(patternsDirectory, bank.File)
    let lines = File.ReadAllLines path
    let parsed = lines |> Array.map splitLine
    for algorithm in algorithms do
        let steps = Render.stringToSteps algorithm
        let state = yellowUpRedFront |> Cube.executeSteps (Cube.inverseSteps steps)
        if bank.Preconditions state then
            let matches =
                parsed
                |> Array.indexed
                |> Array.filter (fun (_, (pattern, _)) -> Solver.matchesGeneric state (pattern, false, false))
            if matches.Length <> 1 then
                failwith $"{bank.Name}: expected exactly one pattern for '{algorithm}', found {matches.Length}. State: {Render.cubeToString state}"
            if not (state |> Cube.executeSteps steps |> bank.Goal) then
                failwith $"{bank.Name}: '{algorithm}' does not satisfy the bank goal."
            applicableAlgorithms <- applicableAlgorithms.Add(normalize algorithm)
            let index, (pattern, existing) = matches[0]
            if existing |> List.exists (normalize >> (=) (normalize algorithm)) |> not then
                let updated = existing @ [algorithm]
                lines[index] <- pattern + "," + String.Join(';', updated)
                parsed[index] <- pattern, updated
                additions <- (bank.Name, algorithm) :: additions
    if write then File.WriteAllLines(path, lines)

let unapplied = algorithms |> List.filter (normalize >> applicableAlgorithms.Contains >> not)
if not (List.isEmpty unapplied) then
    let names = String.Join("; ", unapplied)
    failwith $"Algorithms outside every bank precondition: {names}"

printfn "Checked %i unique algorithms from %s." algorithms.Length notesFile
printfn "%i bank additions%s:" additions.Length (if write then " written" else " required")
additions |> List.rev |> List.iter (fun (bank, algorithm) -> printfn "  %-8s %s" bank algorithm)
