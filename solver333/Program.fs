open System

let numCubes = 10000

Roux.generate numCubes
// Pairing.generate numCubes

let avgTwistCount = float Cube.twistCount / float numCubes;
printfn "Total Average Twists (STM): %f" avgTwistCount

Render.pause()
printfn "DONE!"
Console.ReadLine() |> ignore