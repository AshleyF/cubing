open System

let interactive = false // affects pause

type Color = R | O | W | Y | B | G
type Face = U | D | L | R | F | B
type Sticker = UL | U | UR | L | C | R | DL | D | DR
type Slice = U | D | L | R | F | B | M | S | E

type Cube = Map<Face, Map<Sticker, Color>>

let faceOfStickers ul u ur l c r dl d dr =
    Map.ofList [Sticker.UL, ul; Sticker.U, u; Sticker.UR, ur
                Sticker.L,   l; Sticker.C, c; Sticker.R,  r
                Sticker.DL, dl; Sticker.D, d; Sticker.DR, dr]
let cubeOfFaces u d l r f b = Map.ofList [Face.U, u; Face.D, d; Face.L, l; Face.R, r; Face.F, f; Face.B, b]

let solved =
    let u = faceOfStickers Color.W Color.W Color.W Color.W Color.W Color.W Color.W Color.W Color.W
    let d = faceOfStickers Color.Y Color.Y Color.Y Color.Y Color.Y Color.Y Color.Y Color.Y Color.Y
    let l = faceOfStickers Color.O Color.O Color.O Color.O Color.O Color.O Color.O Color.O Color.O
    let r = faceOfStickers Color.R Color.R Color.R Color.R Color.R Color.R Color.R Color.R Color.R
    let f = faceOfStickers Color.G Color.G Color.G Color.G Color.G Color.G Color.G Color.G Color.G
    let b = faceOfStickers Color.B Color.B Color.B Color.B Color.B Color.B Color.B Color.B Color.B
    cubeOfFaces u d l r f b

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

let faceU = Map.find Face.U
let faceD = Map.find Face.D
let faceL = Map.find Face.L
let faceR = Map.find Face.R
let faceF = Map.find Face.F
let faceB = Map.find Face.B

let stickerUL = Map.find Sticker.UL
let stickerU  = Map.find Sticker.U
let stickerUR = Map.find Sticker.UR
let stickerL  = Map.find Sticker.L
let stickerC  = Map.find Sticker.C
let stickerR  = Map.find Sticker.R
let stickerDL = Map.find Sticker.DL
let stickerD  = Map.find Sticker.D
let stickerDR = Map.find Sticker.DR

let faceRotateCW face =
    faceOfStickers (stickerDL face) (stickerL face) (stickerUL face)
                   (stickerD  face) (stickerC face) (stickerU  face)
                   (stickerDR face) (stickerR face) (stickerUR face)

let faceRotateCCW face =
    faceOfStickers (stickerUR face) (stickerR face) (stickerDR face)
                   (stickerU  face) (stickerC face) (stickerD  face)
                   (stickerUL face) (stickerL face) (stickerDL face)

let faceRotate180 = faceRotateCW >> faceRotateCW

let rotateX cube =
    let u = faceF cube
    let d = faceB cube
    let l = faceL cube |> faceRotateCCW
    let r = faceR cube |> faceRotateCW
    let f = faceD cube
    let b = faceU cube
    cubeOfFaces u d l r f b

let rotateX' cube =
    let u = faceB cube
    let d = faceF cube
    let l = faceL cube |> faceRotateCW
    let r = faceR cube |> faceRotateCCW
    let f = faceU cube
    let b = faceD cube
    cubeOfFaces u d l r f b

let rotateX2 = rotateX >> rotateX

let rotateY cube =
    let u = faceU cube |> faceRotateCW
    let d = faceD cube |> faceRotateCCW
    let l = faceF cube
    let r = faceB cube |> faceRotate180
    let f = faceR cube
    let b = faceL cube |> faceRotate180
    cubeOfFaces u d l r f b

let rotateY' cube =
    let u = faceU cube |> faceRotateCCW
    let d = faceD cube |> faceRotateCW
    let l = faceB cube |> faceRotate180
    let r = faceF cube
    let f = faceL cube
    let b = faceR cube |> faceRotate180
    cubeOfFaces u d l r f b

let rotateY2 = rotateY >> rotateY

let rotateZ cube =
    let u = faceL cube |> faceRotateCW
    let d = faceR cube |> faceRotateCW
    let l = faceD cube |> faceRotateCW
    let r = faceU cube |> faceRotateCW
    let f = faceF cube |> faceRotateCW
    let b = faceB cube |> faceRotateCCW
    cubeOfFaces u d l r f b

let rotateZ' cube =
    let u = faceR cube |> faceRotateCCW
    let d = faceL cube |> faceRotateCCW
    let l = faceU cube |> faceRotateCCW
    let r = faceD cube |> faceRotateCCW
    let f = faceF cube |> faceRotateCCW
    let b = faceB cube |> faceRotateCW
    cubeOfFaces u d l r f b

let rotateZ2 = rotateZ >> rotateZ

let moveU cube =
    let l = faceL cube
    let r = faceR cube
    let f = faceF cube
    let b = faceB cube
    let u' = faceU cube |> faceRotateCW
    let d' = faceD cube
    let l' = faceOfStickers (stickerUL f) (stickerU f) (stickerUR f)
                            (stickerL  l) (stickerC l) (stickerR  l)
                            (stickerDL l) (stickerD l) (stickerDR l)
    let r' = faceOfStickers (stickerDR b) (stickerD b) (stickerDL b)
                            (stickerL  r) (stickerC r) (stickerR  r)
                            (stickerDL r) (stickerD r) (stickerDR r)
    let f' = faceOfStickers (stickerUL r) (stickerU r) (stickerUR r)
                            (stickerL  f) (stickerC f) (stickerR  f)
                            (stickerDL f) (stickerD f) (stickerDR f)
    let b' = faceOfStickers (stickerUL b) (stickerU b) (stickerUR b)
                            (stickerL  b) (stickerC b) (stickerR  b)
                            (stickerUR l) (stickerU l) (stickerUL l)
    cubeOfFaces u' d' l' r' f' b'

let moveU' cube =
    let l = faceL cube
    let r = faceR cube
    let f = faceF cube
    let b = faceB cube
    let u' = faceU cube |> faceRotateCCW
    let d' = faceD cube
    let l' = faceOfStickers (stickerDR b) (stickerD b) (stickerDL b)
                            (stickerL  l) (stickerC l) (stickerR  l)
                            (stickerDL l) (stickerD l) (stickerDR l)
    let r' = faceOfStickers (stickerUL f) (stickerU f) (stickerUR f)
                            (stickerL  r) (stickerC r) (stickerR  r)
                            (stickerDL r) (stickerD r) (stickerDR r)
    let f' = faceOfStickers (stickerUL l) (stickerU l) (stickerUR l)
                            (stickerL  f) (stickerC f) (stickerR  f)
                            (stickerDL f) (stickerD f) (stickerDR f)
    let b' = faceOfStickers (stickerUL b) (stickerU b) (stickerUR b)
                            (stickerL  b) (stickerC b) (stickerR  b)
                            (stickerUR r) (stickerU r) (stickerUL r)
    cubeOfFaces u' d' l' r' f' b'

let moveU2 = moveU >> moveU

let moveD = rotateX2 >> moveU >> rotateX2
let moveD' = rotateX2 >> moveU' >> rotateX2
let moveD2 = moveD >> moveD

let moveL = rotateZ >> moveU >> rotateZ'
let moveL' = rotateZ >> moveU' >> rotateZ'
let moveL2 = moveL >> moveL

let moveR = rotateZ' >> moveU >> rotateZ
let moveR' = rotateZ' >> moveU' >> rotateZ
let moveR2 = moveR >> moveR

let moveF = rotateX >> moveU >> rotateX'
let moveF' = rotateX >> moveU' >> rotateX'
let moveF2 = moveF >> moveF

let moveB = rotateX' >> moveU >> rotateX
let moveB' = rotateX' >> moveU' >> rotateX
let moveB2 = moveB >> moveB

let moveM = rotateX' >> moveL' >> moveR
let moveM' = rotateX >> moveL >> moveR'
let moveM2 = moveM >> moveM

let moveE = rotateY' >> moveU >> moveD'
let moveE' = rotateY >> moveU' >> moveD
let moveE2 = moveE >> moveE

let moveS = rotateZ >> moveF' >> moveB
let moveS' = rotateZ' >> moveF >> moveB'
let moveS2 = moveS >> moveS

let moveUW = rotateY >> moveD
let moveUW' = rotateY' >> moveD'
let moveUW2 = moveUW >> moveUW

let moveDW = rotateY' >> moveU
let moveDW' = rotateY >> moveU'
let moveDW2 = moveDW >> moveDW

let moveLW = rotateX' >> moveR
let moveLW' = rotateX >> moveR'
let moveLW2 = moveLW >> moveLW

let moveRW = rotateX >> moveL
let moveRW' = rotateX' >> moveL'
let moveRW2 = moveRW >> moveRW

let moveFW = rotateZ >> moveB
let moveFW' = rotateZ' >> moveB'
let moveFW2 = moveFW >> moveFW

let moveBW = rotateZ' >> moveF
let moveBW' = rotateZ >> moveF'
let moveBW2 = moveBW >> moveBW

type Rotate =
    | X | X' | X2
    | Y | Y' | Y2
    | Z | Z' | Z2

type Move =
    | U | U' | U2 | UW | UW' | UW2
    | D | D' | D2 | DW | DW' | DW2
    | L | L' | L2 | LW | LW' | LW2
    | R | R' | R2 | RW | RW' | RW2
    | F | F' | F2 | FW | FW' | FW2
    | B | B' | B2 | BW | BW' | BW2
    | M | M' | M2
    | S | S' | S2
    | E | E' | E2

type Step = Rotate of Rotate | Move of Move

let rotate = function
    | X   -> rotateX
    | X'  -> rotateX'
    | X2  -> rotateX2
    | Y   -> rotateY
    | Y'  -> rotateY'
    | Y2  -> rotateY2
    | Z   -> rotateZ
    | Z'  -> rotateZ'
    | Z2  -> rotateZ2

let move = function
    | U   -> moveU
    | U'  -> moveU'
    | U2  -> moveU2
    | UW  -> moveUW
    | UW' -> moveUW'
    | UW2 -> moveUW2
    | D   -> moveD
    | D'  -> moveD'
    | D2  -> moveD2
    | DW  -> moveDW
    | DW' -> moveDW'
    | DW2 -> moveDW2
    | L   -> moveL
    | L'  -> moveL'
    | L2  -> moveL2
    | LW  -> moveLW
    | LW' -> moveLW'
    | LW2 -> moveLW2
    | R   -> moveR
    | R'  -> moveR'
    | R2  -> moveR2
    | RW  -> moveRW
    | RW' -> moveRW'
    | RW2 -> moveRW2
    | F   -> moveF
    | F'  -> moveF'
    | F2  -> moveF2
    | FW  -> moveFW
    | FW' -> moveFW'
    | FW2 -> moveFW2
    | B   -> moveB
    | B'  -> moveB'
    | B2  -> moveB2
    | BW  -> moveBW
    | BW' -> moveBW'
    | BW2 -> moveBW2
    | M   -> moveM
    | M'  -> moveM'
    | M2  -> moveM2
    | S   -> moveS
    | S'  -> moveS'
    | S2  -> moveS2
    | E   -> moveE
    | E'  -> moveE'
    | E2  -> moveE2

let step = function
    | Rotate r -> rotate r
    | Move   m -> move m

let executeRotation cube r = rotate r cube
let executeMove     cube m = move   m cube
let executeStep     cube s = step   s cube

let executeRotations rs cube = Seq.fold executeRotation cube rs
let executeMoves     ms cube = Seq.fold executeMove cube ms
let executeSteps     ss cube = Seq.fold executeStep cube ss

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
    | U   -> "U"
    | U'  -> "U'"
    | U2  -> "U2"
    | UW  -> "u"
    | UW' -> "u'"
    | UW2 -> "u2"
    | D   -> "D"
    | D'  -> "D'"
    | D2  -> "D2"
    | DW  -> "d"
    | DW' -> "d'"
    | DW2 -> "d2"
    | L   -> "L"
    | L'  -> "L'"
    | L2  -> "L2"
    | LW  -> "l"
    | LW' -> "l'"
    | LW2 -> "l2"
    | R   -> "R"
    | R'  -> "R'"
    | R2  -> "R2"
    | RW  -> "r"
    | RW' -> "r'"
    | RW2 -> "r2"
    | F   -> "F"
    | F'  -> "F'"
    | F2  -> "F2"
    | FW  -> "f"
    | FW' -> "f'"
    | FW2 -> "f2"
    | B   -> "B"
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

let scramble n =
    let moves = [U; U'; U2; D; D'; D2; L; L'; L2; R; R'; R2; F; F'; F2; B; B'; B2] @ [M; M'; M2] // @ [S; S'; S2; E; E'; E2]
    let rand = Random()
    let rec scramble' cube sequence history n =
        let isRepeat c = List.contains c history
        let rec canUndoTwoInOneMove c = function
            | m :: t ->
                match history with
                    | _ :: previous :: _ -> if move m c = previous then true else canUndoTwoInOneMove c t
                    | _ -> false
            | _ -> false
        if n = 0 then (cube, Seq.rev sequence) else
            let m = List.item (rand.Next moves.Length) moves
            let cube' = move m cube
            if isRepeat cube' || canUndoTwoInOneMove cube' moves
            then scramble' cube sequence history n // try again
            else scramble' cube' (m :: sequence) (cube' :: history) (n - 1)
    scramble' solved [] [solved] n

let look face sticker cube = cube |> Map.find face |> Map.find sticker

type Center = U | D | L | R | F | B
type Edge = UL | UR | UF | UB | DL | DR | DF | DB | FL | FR | BL | BR
type Corner = ULF | ULB | URF | URB | DLF | DLB | DRF | DRB
type Piece = Center of Center | Edge of Edge | Corner of Corner

let centerToFaceSticker = function
    | Center.U -> Face.U, Sticker.C
    | Center.D -> Face.D, Sticker.C
    | Center.L -> Face.L, Sticker.C
    | Center.R -> Face.R, Sticker.C
    | Center.F -> Face.F, Sticker.C
    | Center.B -> Face.B, Sticker.C

let edgeToFaceStickers = function
    | Edge.UL -> [Face.U, Sticker.L; Face.L, Sticker.U]
    | Edge.UR -> [Face.U, Sticker.R; Face.R, Sticker.U]
    | Edge.UF -> [Face.U, Sticker.D; Face.F, Sticker.U]
    | Edge.UB -> [Face.U, Sticker.U; Face.B, Sticker.D]
    | Edge.DL -> [Face.D, Sticker.L; Face.L, Sticker.D]
    | Edge.DR -> [Face.D, Sticker.R; Face.R, Sticker.D]
    | Edge.DF -> [Face.D, Sticker.U; Face.F, Sticker.D]
    | Edge.DB -> [Face.D, Sticker.D; Face.B, Sticker.U]
    | Edge.FL -> [Face.F, Sticker.L; Face.L, Sticker.R]
    | Edge.FR -> [Face.F, Sticker.R; Face.R, Sticker.L]
    | Edge.BL -> [Face.B, Sticker.L; Face.L, Sticker.L]
    | Edge.BR -> [Face.B, Sticker.R; Face.R, Sticker.R]

let cornerToFaceStickers = function
    | Corner.ULF -> [Face.U, Sticker.DL; Face.L, Sticker.UR; Face.F, Sticker.UL]
    | Corner.ULB -> [Face.U, Sticker.UL; Face.L, Sticker.UL; Face.B, Sticker.DL]
    | Corner.URF -> [Face.U, Sticker.DR; Face.R, Sticker.UL; Face.F, Sticker.UR]
    | Corner.URB -> [Face.U, Sticker.UR; Face.R, Sticker.UR; Face.B, Sticker.DR]
    | Corner.DLF -> [Face.D, Sticker.UL; Face.L, Sticker.DR; Face.F, Sticker.DL]
    | Corner.DLB -> [Face.D, Sticker.DL; Face.L, Sticker.DL; Face.B, Sticker.UL]
    | Corner.DRF -> [Face.D, Sticker.UR; Face.R, Sticker.DL; Face.F, Sticker.DR]
    | Corner.DRB -> [Face.D, Sticker.DR; Face.R, Sticker.DR; Face.B, Sticker.UR]

let searchCenters centers color cube =
    let find c =
        let f, s = centerToFaceSticker c
        look f s cube = color
    Seq.find find centers
let findCenter = searchCenters [Center.U; Center.D; Center.L; Center.R; Center.F; Center.B]

let searchEdges edges color0 color1 cube =
    let rec find = function
        | e :: t ->
            match edgeToFaceStickers e with
            | [f0, s0; f1, s1] ->
                let l0 = look f0 s0 cube
                let l1 = look f1 s1 cube
                if l0 = color0 && l1 = color1 || l1 = color0 && l0 = color1 then e, (l0, l1)
                else find t
            | _ -> failwith "Expected exactly two edge stickers"
        | _ -> failwith "Edge not found"
    find edges
let findEdge = searchEdges [UL; UR; UF; UB; DL; DR; DF; DB; FL; FR; BL; BR]

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

let roux cube =
    let rotateDLEdgeIntoPosition colorD colorL cube =
        let e, (c0, c1) = findEdge colorD colorL cube
        renderWithHighlights (edgeToFaceStickers e) cube
        printfn "Found %s/%s edge (%s/%s)" (colorToString colorL) (colorToString colorD) (colorToString c0) (colorToString c1)
        pause ()
        let cube' =
            let task = "Rotate it to DL position"
            match e, c0, c1 with
            | UL, u, _ when u = colorD -> execute [Rotate X2] task cube
            | UL, u, _ when u = colorL -> execute [Rotate Z'] task cube
            | UR, u, _ when u = colorD -> execute [Rotate Z2] task cube
            | UR, u, _ when u = colorL -> execute [Rotate X2; Rotate Z] task cube
            | UF, u, _ when u = colorD -> execute [Rotate Z2; Rotate Y] task cube
            | UF, u, _ when u = colorL -> execute [Rotate X'; Rotate Y] task cube
            | UB, u, _ when u = colorD -> execute [Rotate X2; Rotate Y] task cube
            | UB, u, _ when u = colorL -> execute [Rotate X; Rotate Y'] task cube
            | DL, d, _ when d = colorD -> execute [] "Already in place!" cube
            | DL, d, _ when d = colorL -> execute [Rotate X2; Rotate Z'] task cube
            | DR, d, _ when d = colorD -> execute [Rotate Y2] task cube
            | DR, d, _ when d = colorL -> execute [Rotate Z] task cube
            | DF, d, _ when d = colorD -> execute [Rotate Y] task cube
            | DF, d, _ when d = colorL -> execute [Rotate X2; Rotate Y'; Rotate Z'] task cube
            | DB, d, _ when d = colorD -> execute [Rotate Y'] task cube
            | DB, d, _ when d = colorL -> execute [Rotate X; Rotate Y] task cube
            | FL, f, _ when f = colorD -> execute [Rotate X'] task cube
            | FL, f, _ when f = colorL -> execute [Rotate Z'; Rotate Y] task cube
            | FR, f, _ when f = colorD -> execute [Rotate X'; Rotate Y2] task cube
            | FR, f, _ when f = colorL -> execute [Rotate Z; Rotate Y] task cube
            | BL, b, _ when b = colorD -> execute [Rotate X] task cube
            | BL, b, _ when b = colorL -> execute [Rotate X'; Rotate Z'] task cube
            | BR, b, _ when b = colorD -> execute [Rotate X; Rotate Y2] task cube
            | BR, b, _ when b = colorL -> execute [Rotate X; Rotate Z] task cube
            | _ -> failwith "Unexpected edge position"
        cube'
    let moveCenterToLeftSide color cube =
        let c = findCenter color cube
        renderWithHighlights [centerToFaceSticker c] cube
        printfn "Found %s center" (colorToString color)
        pause ()
        let cube' =
            let task = "Rotate it to left side"
            match c with
            | Center.U -> execute [Move RW'; Move UW] task cube
            | Center.D -> execute [Move RW; Move UW] task cube
            | Center.L -> execute [] "Already in place!" cube
            | Center.R -> execute [Move UW2] task cube // or Y2
            | Center.F -> execute [Move UW] task cube
            | Center.B -> execute [Move UW'] task cube
        cube'
    let down, left = Color.W, Color.B // TODO: color neutral
    let cube = rotateDLEdgeIntoPosition down left cube
    let cube = moveCenterToLeftSide Color.B cube // TODO: color neutral
    cube

Console.BackgroundColor <- ConsoleColor.Black
Console.ForegroundColor <- ConsoleColor.Gray
Console.Clear()

let hiligePieces () =
    for c in [Center.U; D; L; R; F; B] do
        let stickers = centerToFaceSticker c
        renderWithHighlights [stickers] solved
        printfn "Center: %A" c
        pause ()
    for e in [UL; UR; UF; UB; DL; DR; DF; DB; FL; FR; BL; BR] do
        let stickers = edgeToFaceStickers e
        renderWithHighlights stickers solved
        printfn "Edge: %A" e
        pause ()
    for c in [ULF; ULB; URF; URB; DLF; DLB; DRF; DRB] do
        let stickers = cornerToFaceStickers c
        renderWithHighlights stickers solved
        printfn "Corner: %A" c
        pause ()

let testRoux () =
    let c, s = scramble 20
    render c
    printfn "Scramble: %s                  " (movesToString s)
    pause ()
    let c = roux c
    if look Face.L Sticker.C c <> Color.B then failwith "Not solved LC"
    if look Face.L Sticker.D c <> Color.B then failwith "Not solved DL"
    if look Face.D Sticker.L c <> Color.W then failwith "Not solved DL"
    ignore

while true do testRoux () |> ignore

render solved
pause ()

let test = solved |> moveM' |> moveD' |> moveU' |> moveL2 |> moveLW
render test
pause ()

renderWithHighlights [Face.U, Sticker.C; Face.U, Sticker.L; Face.U, Sticker.R] test
pause ()

(*

TODO:
- Formalize edge and corner orientations in search
- Search centers/corners/edges by "ease" (T/F then L/R then D then B
- Gather metrics (moves, rotations, turns [half/quarter], looks [batch and individual stickers])
    - By phase (inspections rotations, cross/LR blocks, PLL, ...)
- Algorithm search
- Rank algorithms by "ease" (R, L, U, F, D, B, ... combinations, finger tricks, ...)
    - Maybe with user input or video analysis

Long term:
- Video analysis
*)