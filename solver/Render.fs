module Render

open System
open Cube

let interactive = false

let renderWithHighlights highlights cube =
    let renderFace f x y slice =
        let renderSticker s =
            let setColorForWindows () =
                let toConsoleColor hilited = function
                    | Color.R -> if hilited then ConsoleColor.Red else ConsoleColor.DarkGray // ConsoleColor.DarkRed
                    | Color.O -> if hilited then ConsoleColor.Magenta else ConsoleColor.DarkGray // ConsoleColor.DarkMagenta
                    | Color.W -> if hilited then ConsoleColor.White else ConsoleColor.DarkGray // ConsoleColor.Gray
                    | Color.Y -> if hilited then ConsoleColor.Yellow else ConsoleColor.DarkGray // ConsoleColor.DarkYellow
                    | Color.B -> if hilited then ConsoleColor.Blue else ConsoleColor.DarkGray // ConsoleColor.DarkBlue
                    | Color.G -> if hilited then ConsoleColor.Green else ConsoleColor.DarkGray // ConsoleColor.DarkGreen
                let hilited = List.length highlights = 0 || List.contains (f, s) highlights
                Console.BackgroundColor <- cube |> Map.find f |> Map.find s |> toConsoleColor hilited
            let setColorForLinuxAndMac () =
                let toConsoleColor hilited = function
                    | Color.R -> if hilited then 9   else 52
                    | Color.O -> if hilited then 208 else 94
                    | Color.W -> if hilited then 15  else 240
                    | Color.Y -> if hilited then 220 else 100
                    | Color.B -> if hilited then 21  else 17
                    | Color.G -> if hilited then 10  else 22
                let hilited = List.length highlights = 0 || List.contains (f, s) highlights
                printf "\u001b[48;5;%im" (cube |> Map.find f |> Map.find s |> toConsoleColor hilited)
            // setColorForLinuxAndMac ()
            setColorForWindows ()
            printf "  "
        if interactive then 
            Console.CursorLeft <- x
            Console.CursorTop <- y
        if interactive || slice = 0 then renderSticker Sticker.UL
        if interactive || slice = 0 then renderSticker Sticker.U
        if interactive || slice = 0 then renderSticker Sticker.UR
        if interactive then 
            Console.CursorLeft <- x
            Console.CursorTop <- y + 1
        if interactive || slice = 1 then renderSticker Sticker.L
        if interactive || slice = 1 then renderSticker Sticker.C
        if interactive || slice = 1 then renderSticker Sticker.R
        if interactive then 
            Console.CursorLeft <- x
            Console.CursorTop <- y + 2
        if interactive || slice = 2 then renderSticker Sticker.DL
        if interactive || slice = 2 then renderSticker Sticker.D
        if interactive || slice = 2 then renderSticker Sticker.DR
    if interactive then Console.Clear()
    if interactive then 
        renderFace Face.B 8 1 0
        renderFace Face.U 8 4 0
        renderFace Face.L 2 7 0
        renderFace Face.F 8 7 0
        renderFace Face.R 14 7 0
        renderFace Face.D 8 10 0
    else
        let fill s = Console.BackgroundColor <- ConsoleColor.Black; printf s
        let newline () = Console.BackgroundColor <- ConsoleColor.Black; printfn ""
        fill "        "; renderFace Face.B 0 0 0; newline ()
        fill "        "; renderFace Face.B 0 0 1; newline ()
        fill "        "; renderFace Face.B 0 0 2; newline ()
        fill "        "; renderFace Face.U 0 0 0; newline ()
        fill "        "; renderFace Face.U 0 0 1; newline ()
        fill "        "; renderFace Face.U 0 0 2; newline ()
        fill "  "; renderFace Face.L 0 0 0; renderFace Face.F 0 0 0; renderFace Face.R 0 0 0; newline ()
        fill "  "; renderFace Face.L 0 0 1; renderFace Face.F 0 0 1; renderFace Face.R 0 0 1; newline ()
        fill "  "; renderFace Face.L 0 0 2; renderFace Face.F 0 0 2; renderFace Face.R 0 0 2; newline ()
        fill "        "; renderFace Face.D 0 0 0; newline ()
        fill "        "; renderFace Face.D 0 0 1; newline ()
        fill "        "; renderFace Face.D 0 0 2; newline ()
    Console.BackgroundColor <- ConsoleColor.Black
    printfn ""

let render = renderWithHighlights []

let rotationToString = function
    | X   -> "x"
    | X'  -> "x'"
    | X2  -> "x2"
    | Y   -> "y"
    | Y'  -> "y'"
    | Y2  -> "y2"
    | Z   -> "z"
    | Z'  -> "z'"
    | Z2  -> "z2"
let rotationsToString rots = String.Join(' ', Seq.map rotationToString rots)

let moveToString = function
    | Move.U   -> "U"
    | U'  -> "U'"
    | U2  -> "U2"
    | UW  -> "u"
    | UW' -> "u'"
    | UW2 -> "u2"
    | Move.D   -> "D"
    | D'  -> "D'"
    | D2  -> "D2"
    | DW  -> "d"
    | DW' -> "d'"
    | DW2 -> "d2"
    | Move.L   -> "L"
    | L'  -> "L'"
    | L2  -> "L2"
    | LW  -> "l"
    | LW' -> "l'"
    | LW2 -> "l2"
    | Move.R   -> "R"
    | R'  -> "R'"
    | R2  -> "R2"
    | RW  -> "r"
    | RW' -> "r'"
    | RW2 -> "r2"
    | Move.F   -> "F"
    | F'  -> "F'"
    | F2  -> "F2"
    | FW  -> "f"
    | FW' -> "f'"
    | FW2 -> "f2"
    | Move.B   -> "B"
    | B'  -> "B'"
    | B2  -> "B2"
    | BW  -> "b"
    | BW' -> "b'"
    | BW2 -> "bw"
    | M   -> "M"
    | M'  -> "M'"
    | M2  -> "M2"
    | S   -> "S"
    | S'  -> "S'"
    | S2  -> "S2"
    | E   -> "E"
    | E'  -> "E'"
    | E2  -> "E2"
let movesToString moves = String.Join(' ', Seq.map moveToString moves)

let stepToString = function
    | Rotate r -> rotationToString r
    | Move   m -> moveToString m
let stepsToString steps = String.Join(' ', Seq.map stepToString steps)

let colorToString = function
    | Color.R -> "Red"
    | Color.O -> "Orange"
    | Color.W -> "White"
    | Color.Y -> "Yellow"
    | Color.B -> "Blue"
    | Color.G -> "Green"

let pause () =
    if interactive then 
        printfn "Press Enter to continue..."
        Console.ReadLine() |> ignore

let execute steps message cube =
    let cube' = executeSteps steps cube
    render cube'
    steps |> stepsToString |> printfn "%s: %s" message
    pause ()
    cube'