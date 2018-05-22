open System

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
    let renderFace f x y =
        let renderSticker s =
            let toConsoleColor = function
                | Color.R -> ConsoleColor.Red
                | Color.O -> ConsoleColor.DarkYellow
                | Color.W -> ConsoleColor.White
                | Color.Y -> ConsoleColor.Yellow
                | Color.B -> ConsoleColor.Blue
                | Color.G -> ConsoleColor.Green
            if List.length highlights = 0 || List.contains (f, s) highlights
            then Console.BackgroundColor <- cube |> Map.find f |> Map.find s |> toConsoleColor
            else Console.BackgroundColor <- ConsoleColor.DarkGray
            Console.Write("  ");
        Console.CursorLeft <- x
        Console.CursorTop <- y
        renderSticker Sticker.UL
        renderSticker Sticker.U
        renderSticker Sticker.UR
        Console.CursorLeft <- x
        Console.CursorTop <- y + 1
        renderSticker Sticker.L
        renderSticker Sticker.C
        renderSticker Sticker.R
        Console.CursorLeft <- x
        Console.CursorTop <- y + 2
        renderSticker Sticker.DL
        renderSticker Sticker.D
        renderSticker Sticker.DR
    renderFace Face.B 8 1
    renderFace Face.U 8 4
    renderFace Face.L 2 7
    renderFace Face.F 8 7
    renderFace Face.R 14 7
    renderFace Face.D 8 10
    Console.BackgroundColor <- ConsoleColor.Black

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
let rotateX2' = rotateX2

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
let rotateY2' = rotateY2

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
let rotateZ2' = rotateZ2

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
let moveU2' = moveU2

let moveD = rotateX2 >> moveU >> rotateX2'
let moveD' = rotateX2 >> moveU' >> rotateX2'
let moveD2 = moveD >> moveD
let moveD2' = moveD2

let moveL = rotateZ >> moveU >> rotateZ'
let moveL' = rotateZ >> moveU' >> rotateZ'
let moveL2 = moveL >> moveL
let moveL2' = moveL2

let moveR = rotateZ' >> moveU >> rotateZ
let moveR' = rotateZ' >> moveU' >> rotateZ
let moveR2 = moveR >> moveR
let moveR2' = moveR2

let moveF = rotateX >> moveU >> rotateX'
let moveF' = rotateX >> moveU' >> rotateX'
let moveF2 = moveF >> moveF
let moveF2' = moveF2

let moveB = rotateX' >> moveU >> rotateX
let moveB' = rotateX' >> moveU' >> rotateX
let moveB2 = moveB >> moveB
let moveB2' = moveB2

let moveM = rotateX' >> moveL' >> moveR
let moveM' = rotateX >> moveL >> moveR'
let moveM2 = moveM >> moveM
let moveM2' = moveM2

let moveE = rotateY' >> moveU >> moveD'
let moveE' = rotateY >> moveU' >> moveD
let moveE2 = moveE >> moveE
let moveE2' = moveE2

let moveS = rotateZ >> moveF' >> moveB
let moveS' = rotateZ' >> moveF >> moveB'
let moveS2 = moveS >> moveS
let moveS2' = moveS2

let moveUW = rotateY >> moveD
let moveUW' = rotateY' >> moveD'

let moveDW = rotateY' >> moveU
let moveDW' = rotateY >> moveU'

let moveLW = rotateX' >> moveR
let moveLW' = rotateX >> moveR'

let moveRW = rotateX >> moveL
let moveRW' = rotateX' >> moveL'

let moveFW = rotateZ >> moveB
let moveFW' = rotateZ' >> moveB'

let moveBW = rotateZ' >> moveF
let moveBW' = rotateZ >> moveF'

type Rotate =
    | X | X' | X2 | X2'
    | Y | Y' | Y2 | Y2'
    | Z | Z' | Z2 | Z2'

type Move =
    | U | U' | U2 | U2' | UW | UW'
    | D | D' | D2 | D2' | DW | DW'
    | L | L' | L2 | L2' | LW | LW'
    | R | R' | R2 | R2' | RW | RW'
    | F | F' | F2 | F2' | FW | FW'
    | B | B' | B2 | B2' | BW | BW'
    | M | M' | M2 | M2'
    | S | S' | S2 | S2'
    | E | E' | E2 | E2'

type Step = Rotate of Rotate | Move of Move

let rotate = function
    | X   -> rotateX
    | X'  -> rotateX'
    | X2  -> rotateX2
    | X2' -> rotateX2'
    | Y   -> rotateY
    | Y'  -> rotateY'
    | Y2  -> rotateY2
    | Y2' -> rotateY2'
    | Z   -> rotateZ
    | Z'  -> rotateZ'
    | Z2  -> rotateZ2
    | Z2' -> rotateZ2'

let move = function
    | U   -> moveU
    | U'  -> moveU'
    | U2  -> moveU2
    | U2' -> moveU2'
    | UW  -> moveUW
    | UW' -> moveUW'
    | D   -> moveD
    | D'  -> moveD'
    | D2  -> moveD2
    | D2' -> moveD2'
    | DW  -> moveDW
    | DW' -> moveDW'
    | L   -> moveL
    | L'  -> moveL'
    | L2  -> moveL2
    | L2' -> moveL2'
    | LW  -> moveLW
    | LW' -> moveLW'
    | R   -> moveR
    | R'  -> moveR'
    | R2  -> moveR2
    | R2' -> moveR2'
    | RW  -> moveRW
    | RW' -> moveRW'
    | F   -> moveF
    | F'  -> moveF'
    | F2  -> moveF2
    | F2' -> moveF2'
    | FW  -> moveFW
    | FW' -> moveFW'
    | B   -> moveB
    | B'  -> moveB'
    | B2  -> moveB2
    | B2' -> moveB2'
    | BW  -> moveBW
    | BW' -> moveBW'
    | M   -> moveM
    | M'  -> moveM'
    | M2  -> moveM2
    | M2' -> moveM2'
    | S   -> moveS
    | S'  -> moveS'
    | S2  -> moveS2
    | S2' -> moveS2'
    | E   -> moveE
    | E'  -> moveE'
    | E2  -> moveE2
    | E2' -> moveE2'

let step = function
    | Rotate r -> rotate r
    | Move   m -> move m

let rotationToString = function
    | X   -> "x"
    | X'  -> "x'"
    | X2  -> "x2"
    | X2' -> "x2'"
    | Y   -> "y"
    | Y'  -> "y'"
    | Y2  -> "y2"
    | Y2' -> "y2'"
    | Z   -> "z"
    | Z'  -> "z'"
    | Z2  -> "z2"
    | Z2' -> "z2"
let rotationsToString rots = String.Join(' ', Seq.map rotationToString rots)

let moveToString = function
    | U   -> "U"
    | U'  -> "U'"
    | U2  -> "U2"
    | U2' -> "U2'"
    | UW  -> "u"
    | UW' -> "u'"
    | D   -> "D"
    | D'  -> "D'"
    | D2  -> "D2"
    | D2' -> "D2'"
    | DW  -> "d"
    | DW' -> "d'"
    | L   -> "L"
    | L'  -> "L'"
    | L2  -> "L2"
    | L2' -> "L2'"
    | LW  -> "l"
    | LW' -> "l'"
    | R   -> "R"
    | R'  -> "R'"
    | R2  -> "R2"
    | R2' -> "R2'"
    | RW  -> "r"
    | RW' -> "r'"
    | F   -> "F"
    | F'  -> "F'"
    | F2  -> "F2"
    | F2' -> "F2'"
    | FW  -> "f"
    | FW' -> "f'"
    | B   -> "B"
    | B'  -> "B'"
    | B2  -> "B2"
    | B2' -> "B2'"
    | BW  -> "b"
    | BW' -> "b"
    | M   -> "M"
    | M'  -> "M'"
    | M2  -> "M2"
    | M2' -> "M2'"
    | S   -> "S"
    | S'  -> "S'"
    | S2  -> "S2"
    | S2' -> "S2'"
    | E   -> "E"
    | E'  -> "E'"
    | E2  -> "E2"
    | E2' -> "E2'"
let movesToString moves = String.Join(' ', Seq.map moveToString moves)

let stepToString = function
    | Rotate r -> rotationToString r
    | Move   m -> moveToString m
let stepsToString steps = String.Join(' ', Seq.map stepToString steps)

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

Console.BackgroundColor <- ConsoleColor.Black
Console.ForegroundColor <- ConsoleColor.Gray
Console.Clear()

let c, s = scramble 20
render c
printf "Scramble: %s                  " (movesToString s)
Console.ReadLine() |> ignore

render solved
Console.ReadLine () |> ignore

let test = solved |> moveM' |> moveD' |> moveU' |> moveL2 |> moveLW
render test
Console.ReadLine () |> ignore

renderWithHighlights [Face.U, Sticker.C; Face.U, Sticker.L; Face.U, Sticker.R] test
Console.ReadLine () |> ignore
