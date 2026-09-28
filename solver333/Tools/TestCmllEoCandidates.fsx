#r "../bin/Release/net8.0/BrowserSolver.dll"

Roux.validateCmllEoCandidates ()

if Roux.cmllEoCandidates.Length <> 92 then
    failwith $"Expected 92 CMLL/EO candidates, found {Roux.cmllEoCandidates.Length}."

printfn "Validated %d CMLL/EO candidates." Roux.cmllEoCandidates.Length
