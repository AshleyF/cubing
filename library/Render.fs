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
                    | Color.A -> ConsoleColor.DarkGray
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
                    | Color.A -> 238
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

let colorToString = function
    | Color.R -> "R"
    | Color.O -> "O"
    | Color.W -> "W"
    | Color.Y -> "Y"
    | Color.B -> "B"
    | Color.G -> "G"
    | Color.A -> "."

let charToColor = function
    | 'R' -> Color.R
    | 'O' -> Color.O
    | 'W' -> Color.W
    | 'Y' -> Color.Y
    | 'B' -> Color.B
    | 'G' -> Color.G
    | '.' -> Color.A
    | _ -> failwith "Invalid color character"

let piecesToString (cube: Cube) (pieces: Piece list) =
    let piecesSet = Set.ofList pieces
    let lookSticker face sticker cube piece =
        if piecesSet.Contains piece
        then look face sticker cube |> colorToString
        else "."
    let str = seq {
        yield lookSticker Face.B Sticker.UL cube (Corner DLB)
        yield lookSticker Face.B Sticker.U  cube (Edge DB)
        yield lookSticker Face.B Sticker.UR cube (Corner DRB)
        yield lookSticker Face.B Sticker.L  cube (Edge BL)
        yield lookSticker Face.B Sticker.C  cube (Center B)
        yield lookSticker Face.B Sticker.R  cube (Edge BR)
        yield lookSticker Face.B Sticker.DL cube (Corner ULB)
        yield lookSticker Face.B Sticker.D  cube (Edge UB)
        yield lookSticker Face.B Sticker.DR cube (Corner URB)
        yield lookSticker Face.U Sticker.UL cube (Corner ULB)
        yield lookSticker Face.U Sticker.U  cube (Edge UB)
        yield lookSticker Face.U Sticker.UR cube (Corner URB)
        yield lookSticker Face.U Sticker.L  cube (Edge UL)
        yield lookSticker Face.U Sticker.C  cube (Center U)
        yield lookSticker Face.U Sticker.R  cube (Edge UR)
        yield lookSticker Face.U Sticker.DL cube (Corner ULF)
        yield lookSticker Face.U Sticker.D  cube (Edge UF)
        yield lookSticker Face.U Sticker.DR cube (Corner URF)
        yield lookSticker Face.L Sticker.UL cube (Corner ULB)
        yield lookSticker Face.L Sticker.U  cube (Edge UL)
        yield lookSticker Face.L Sticker.UR cube (Corner ULF)
        yield lookSticker Face.F Sticker.UL cube (Corner ULF)
        yield lookSticker Face.F Sticker.U  cube (Edge UF)
        yield lookSticker Face.F Sticker.UR cube (Corner URF)
        yield lookSticker Face.R Sticker.UL cube (Corner URF)
        yield lookSticker Face.R Sticker.U  cube (Edge UR)
        yield lookSticker Face.R Sticker.UR cube (Corner URB)
        yield lookSticker Face.L Sticker.L  cube (Edge BL)
        yield lookSticker Face.L Sticker.C  cube (Center L)
        yield lookSticker Face.L Sticker.R  cube (Edge FL)
        yield lookSticker Face.F Sticker.L  cube (Edge FL)
        yield lookSticker Face.F Sticker.C  cube (Center F)
        yield lookSticker Face.F Sticker.R  cube (Edge FR)
        yield lookSticker Face.R Sticker.L  cube (Edge FR)
        yield lookSticker Face.R Sticker.C  cube (Center R)
        yield lookSticker Face.R Sticker.R  cube (Edge BR)
        yield lookSticker Face.L Sticker.DL cube (Corner DLB)
        yield lookSticker Face.L Sticker.D  cube (Edge DL)
        yield lookSticker Face.L Sticker.DR cube (Corner DLF)
        yield lookSticker Face.F Sticker.DL cube (Corner DLF)
        yield lookSticker Face.F Sticker.D  cube (Edge DF)
        yield lookSticker Face.F Sticker.DR cube (Corner DRF)
        yield lookSticker Face.R Sticker.DL cube (Corner DRF)
        yield lookSticker Face.R Sticker.D  cube (Edge DR)
        yield lookSticker Face.R Sticker.DR cube (Corner DRB)
        yield lookSticker Face.D Sticker.UL cube (Corner DLF)
        yield lookSticker Face.D Sticker.U  cube (Edge DF)
        yield lookSticker Face.D Sticker.UR cube (Corner DRF)
        yield lookSticker Face.D Sticker.L  cube (Edge DL)
        yield lookSticker Face.D Sticker.C  cube (Center D)
        yield lookSticker Face.D Sticker.R  cube (Edge DR)
        yield lookSticker Face.D Sticker.DL cube (Corner DLB)
        yield lookSticker Face.D Sticker.D  cube (Edge DB)
        yield lookSticker Face.D Sticker.DR cube (Corner DRB) }
    String.Concat(str)

let cubeToString (cube: Cube) =
    [ Center U; Center D; Center L; Center R; Center F; Center B
      Edge UL; Edge UR; Edge UF; Edge UB; Edge DL; Edge DR; Edge DF; Edge DB; Edge FL; Edge FR; Edge BL; Edge BR
      Corner ULF; Corner ULB; Corner URF; Corner URB; Corner DLF; Corner DLB; Corner DRF; Corner DRB]
    |> piecesToString cube

let stringToCube (s: string) =
    let c i = charToColor s.[i]
    let b = faceOfStickers (c  0) (c  1) (c  2) (c  3) (c  4) (c  5) (c  6) (c  7) (c  8)
    let u = faceOfStickers (c  9) (c 10) (c 11) (c 12) (c 13) (c 14) (c 15) (c 16) (c 17)
    let l = faceOfStickers (c 18) (c 19) (c 20) (c 27) (c 28) (c 29) (c 36) (c 37) (c 38)
    let f = faceOfStickers (c 21) (c 22) (c 23) (c 30) (c 31) (c 32) (c 39) (c 40) (c 41)
    let r = faceOfStickers (c 24) (c 25) (c 26) (c 33) (c 34) (c 35) (c 42) (c 43) (c 44)
    let d = faceOfStickers (c 45) (c 46) (c 47) (c 48) (c 49) (c 50) (c 51) (c 52) (c 53)
    cubeOfFaces u d l r f b

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
    | BW2 -> "b2"
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

let stringToStep = function
    | "x"   -> Rotate X
    | "x'"  -> Rotate X'
    | "x2"  -> Rotate X2
    | "x2'" -> Rotate X2
    | "y"   -> Rotate Y
    | "y'"  -> Rotate Y'
    | "y2"  -> Rotate Y2
    | "y2'" -> Rotate Y2
    | "z"   -> Rotate Z
    | "z'"  -> Rotate Z'
    | "z2"  -> Rotate Z2
    | "z2'" -> Rotate Z2
    | "U"   -> Move Move.U
    | "U'"  -> Move U'
    | "U2"  -> Move U2
    | "U2'" -> Move U2
    | "u"   -> Move UW
    | "u'"  -> Move UW'
    | "u2"  -> Move UW2
    | "u2'" -> Move UW2
    | "D"   -> Move Move.D
    | "D'"  -> Move D'
    | "D2"  -> Move D2
    | "D2'" -> Move D2
    | "d"   -> Move DW
    | "d'"  -> Move DW'
    | "d2"  -> Move DW2
    | "d2'" -> Move DW2
    | "L"   -> Move Move.L
    | "L'"  -> Move L'
    | "L2"  -> Move L2
    | "L2'" -> Move L2
    | "l"   -> Move LW
    | "l'"  -> Move LW'
    | "l2"  -> Move LW2
    | "l2'" -> Move LW2
    | "R"   -> Move Move.R
    | "R'"  -> Move R'
    | "R2"  -> Move R2
    | "R2'" -> Move R2
    | "r"   -> Move RW
    | "r'"  -> Move RW'
    | "r2"  -> Move RW2
    | "r2'" -> Move RW2
    | "F"   -> Move Move.F
    | "F'"  -> Move F'
    | "F2"  -> Move F2
    | "F2'" -> Move F2
    | "f"   -> Move FW
    | "f'"  -> Move FW'
    | "f2"  -> Move FW2
    | "f2'" -> Move FW2
    | "B"   -> Move Move.B
    | "B'"  -> Move B'
    | "B2"  -> Move B2
    | "B2'" -> Move B2
    | "b"   -> Move BW
    | "b'"  -> Move BW'
    | "b2"  -> Move BW2
    | "b2'" -> Move BW2
    | "M"   -> Move M
    | "M'"  -> Move M'
    | "M2"  -> Move M2
    | "M2'" -> Move M2
    | "S"   -> Move S
    | "S'"  -> Move S'
    | "S2"  -> Move S2
    | "S2'" -> Move S2
    | "E"   -> Move E
    | "E'"  -> Move E'
    | "E2"  -> Move E2
    | "E2'" -> Move E2
    | _ -> failwith "Unknown step notation"

let stringToSteps alg =
    let split (s: string) (c: char) = s.Split(c)
    split alg ' ' |> Seq.map stringToStep |> List.ofSeq

let stepToString = function
    | Rotate r -> rotationToString r
    | Move   m -> moveToString m
let stepsToString steps = String.Join(' ', Seq.map stepToString steps)

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