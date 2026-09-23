module PatternData

let mutable private patterns : Map<string, string list> = Map.empty

let setData (keys: string array) (values: string array array) =
    patterns <- Array.map2 (fun key rows -> key, List.ofArray rows) keys values |> Map.ofArray

let read key = Map.find key patterns
