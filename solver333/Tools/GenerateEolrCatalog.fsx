#r "../../library/bin/Release/net8.0/Library.dll"

open System
open System.IO
open System.Text.Json
open Cube

let sourceRoot = Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, "..", "..", "lse"))
let output = Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, "..", "..", "site", "lab", "eolr-cases.json"))

let displayName (path: string) =
    Path.GetFileNameWithoutExtension(path)
        .Replace("eolr_", "")
        .Replace("1f-1b", "1F–1B")
        .Replace("4-0", "4–0")
        .Replace("_", " ")

let yellowUpRedFront = executeRotation (executeRotation solved Rotate.X2) Rotate.Y

let cases =
    Directory.GetFiles(sourceRoot, "eolr_*.md")
    |> Array.sort
    |> Array.collect (fun path ->
        File.ReadLines(path)
        |> Seq.choose (fun line ->
            if not (line.StartsWith("1. `")) then None
            else
                let scrambleEnd = line.IndexOf('`', 4)
                let detailsStart = line.IndexOf('(', scrambleEnd)
                let detailsEnd = line.LastIndexOf(')')
                if scrambleEnd < 0 || detailsStart < 0 || detailsEnd < detailsStart then None
                else
                    let scramble = line.Substring(4, scrambleEnd - 4)
                    let details = line.Substring(detailsStart + 1, detailsEnd - detailsStart - 1)
                    let cube = executeSteps (Render.stringToSteps scramble) yellowUpRedFront
                    Some {| pattern = Render.cubeToStringWithEdgeOrientation cube
                            action = $"{displayName path} · {details}" |})
        |> Seq.toArray)

File.WriteAllText(output, JsonSerializer.Serialize(cases))
printfn "Generated %i EOLR examples in %s" cases.Length output
