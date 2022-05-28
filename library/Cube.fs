﻿module Cube

type Color = R | O | W | Y | B | G | A
type Face = U | D | L | R | F | B
type Sticker = UL | U | UR | L | C | R | DL | D | DR
type Slice = U | D | L | R | F | B | M | S | E

type Cube = Map<Face, Map<Sticker, Color>>

let faceOfStickers ul u ur l c r dl d dr =
    Map.ofList [Sticker.UL, ul; Sticker.U, u; Sticker.UR, ur
                Sticker.L,   l; Sticker.C, c; Sticker.R,  r
                Sticker.DL, dl; Sticker.D, d; Sticker.DR, dr]
let cubeOfFaces u d l r f b = Map.ofList [Face.U, u; Face.D, d; Face.L, l; Face.R, r; Face.F, f; Face.B, b]

let faceU c = Map.find Face.U c
let faceD c = Map.find Face.D c
let faceL c = Map.find Face.L c
let faceR c = Map.find Face.R c
let faceF c = Map.find Face.F c
let faceB c = Map.find Face.B c

let stickerUL c = Map.find Sticker.UL c
let stickerU  c = Map.find Sticker.U  c
let stickerUR c = Map.find Sticker.UR c
let stickerL  c = Map.find Sticker.L  c
let stickerC  c = Map.find Sticker.C  c
let stickerR  c = Map.find Sticker.R  c
let stickerDL c = Map.find Sticker.DL c
let stickerD  c = Map.find Sticker.D  c
let stickerDR c = Map.find Sticker.DR c

let faceRotateCW face =
    faceOfStickers (stickerDL face) (stickerL face) (stickerUL face)
                   (stickerD  face) (stickerC face) (stickerU  face)
                   (stickerDR face) (stickerR face) (stickerUR face)

let faceRotateCCW face =
    faceOfStickers (stickerUR face) (stickerR face) (stickerDR face)
                   (stickerU  face) (stickerC face) (stickerD  face)
                   (stickerUL face) (stickerL face) (stickerDL face)

let faceRotate180 c = c |> (faceRotateCW >> faceRotateCW)

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

let rotateX2 c = c |> (rotateX >> rotateX)

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

let rotateY2 c = c |> (rotateY >> rotateY)

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

let rotateZ2 c = c |> (rotateZ >> rotateZ)

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

let moveU2 c = c |> (moveU >> moveU)

let moveD c = c |> (rotateX2 >> moveU >> rotateX2)
let moveD' c = c |> (rotateX2 >> moveU' >> rotateX2)
let moveD2 c = c |> (moveD >> moveD)

let moveL c = c |> (rotateZ >> moveU >> rotateZ')
let moveL' c = c |> (rotateZ >> moveU' >> rotateZ')
let moveL2 c = c |> (moveL >> moveL)

let moveR c = c |> (rotateZ' >> moveU >> rotateZ)
let moveR' c = c |> (rotateZ' >> moveU' >> rotateZ)
let moveR2 c = c |> (moveR >> moveR)

let moveF c = c |> (rotateX >> moveU >> rotateX')
let moveF' c = c |> (rotateX >> moveU' >> rotateX')
let moveF2 c = c |> (moveF >> moveF)

let moveB c = c |> (rotateX' >> moveU >> rotateX)
let moveB' c = c |> (rotateX' >> moveU' >> rotateX)
let moveB2 c = c |> (moveB >> moveB)

let moveM c = c |> (rotateX' >> moveL' >> moveR)
let moveM' c = c |> (rotateX >> moveL >> moveR')
let moveM2 c = c |> (moveM >> moveM)

let moveE c = c |> (rotateY' >> moveU >> moveD')
let moveE' c = c |> (rotateY >> moveU' >> moveD)
let moveE2 c = c |> (moveE >> moveE)

let moveS c = c |> (rotateZ >> moveF' >> moveB)
let moveS' c = c |> (rotateZ' >> moveF >> moveB')
let moveS2 c = c |> (moveS >> moveS)

let moveUW c = c |> (rotateY >> moveD)
let moveUW' c = c |> (rotateY' >> moveD')
let moveUW2 c = c |> (moveUW >> moveUW)

let moveDW c = c |> (rotateY' >> moveU)
let moveDW' c = c |> (rotateY >> moveU')
let moveDW2 c = c |> (moveDW >> moveDW)

let moveLW c = c |> (rotateX' >> moveR)
let moveLW' c = c |> (rotateX >> moveR')
let moveLW2 c = c |> (moveLW >> moveLW)

let moveRW c = c |> (rotateX >> moveL)
let moveRW' c = c |> (rotateX' >> moveL')
let moveRW2 c = c |> (moveRW >> moveRW)

let moveFW c = c |> (rotateZ >> moveB)
let moveFW' c = c |> (rotateZ' >> moveB')
let moveFW2 c = c |> (moveFW >> moveFW)

let moveBW c = c |> (rotateZ' >> moveF)
let moveBW' c = c |> (rotateZ >> moveF')
let moveBW2 c = c |> (moveBW >> moveBW)

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

let inverseRotation = function
    | X -> X' | X' -> X | X2 -> X2
    | Y -> Y' | Y' -> Y | Y2 -> Y2
    | Z -> Z' | Z' -> Z | Z2 -> Z2

let inverseMove = function
    | U -> U' | U' -> U | U2 -> U2 | UW -> UW' | UW' -> UW | UW2 -> UW2
    | D -> D' | D' -> D | D2 -> D2 | DW -> DW' | DW' -> DW | DW2 -> DW2
    | L -> L' | L' -> L | L2 -> L2 | LW -> LW' | LW' -> LW | LW2 -> LW2
    | R -> R' | R' -> R | R2 -> R2 | RW -> RW' | RW' -> RW | RW2 -> RW2
    | F -> F' | F' -> F | F2 -> F2 | FW -> FW' | FW' -> FW | FW2 -> FW2
    | B -> B' | B' -> B | B2 -> B2 | BW -> BW' | BW' -> BW | BW2 -> BW2
    | M -> M' | M' -> M | M2 -> M2
    | S -> S' | S' -> S | S2 -> S2
    | E -> E' | E' -> E | E2 -> E2 

let inverseStep = function
    | Rotate r -> inverseRotation r |> Rotate
    | Move m -> inverseMove m |> Move

let inverseRotations = List.map inverseRotation >> List.rev
let inverseMoves = List.map inverseMove >> List.rev
let inverseSteps = List.map inverseStep >> List.rev

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

let movesRegular = [
    Move.U; Move.U'; Move.U2
    Move.D; Move.D'; Move.D2
    Move.L; Move.L'; Move.L2
    Move.R; Move.R'; Move.R2
    Move.F; Move.F'; Move.F2
    Move.B; Move.B'; Move.B2
    Move.M; Move.M'; Move.M2]

let movesWide = [
    Move.UW; Move.UW'; Move.UW2
    Move.DW; Move.DW'; Move.DW2
    Move.LW; Move.LW'; Move.LW2
    Move.RW; Move.RW'; Move.RW2
    Move.FW; Move.FW'; Move.FW2
    Move.BW; Move.BW'; Move.BW2]

let movesM = [Move.M; Move.M'; Move.M2]

let movesSlice = movesM @ [
    Move.S; Move.S'; Move.S2
    Move.E; Move.E'; Move.E2]

let movesAll = movesRegular @ movesWide @ movesSlice

let movesCommonRoux = movesRegular @ movesWide @ movesM

let step = function
    | Rotate r -> rotate r
    | Move   m -> move m

let executeRotation (cube : Cube) r = rotate r cube
let executeMove     (cube : Cube) m = move   m cube
let executeStep     (cube : Cube) s = step   s cube

let executeRotations rs cube = Seq.fold executeRotation cube rs
let executeMoves     ms cube = Seq.fold executeMove cube ms
// let executeSteps     ss cube = Seq.fold executeStep cube ss
let mutable twistCount = 0
let mutable stageCount = 0
let executeSteps ss cube =
    twistCount <- twistCount + Seq.length ss
    stageCount <- stageCount + Seq.length ss
    Seq.fold executeStep cube ss

type Center = U | D | L | R | F | B
type Edge = UL | UR | UF | UB | DL | DR | DF | DB | FL | FR | BL | BR
type Corner = ULF | ULB | URF | URB | DLF | DLB | DRF | DRB
type Piece = Center of Center | Edge of Edge | Corner of Corner

let isAttachedCenterEdge center edge =
    match center with
    | Center.U -> match edge with UL | UR | UF | UB -> true | _ -> false
    | Center.D -> match edge with DL | DR | DF | DB -> true | _ -> false 
    | Center.L -> match edge with UL | DL | FL | BL -> true | _ -> false
    | Center.R -> match edge with UR | DR | FR | BR -> true | _ -> false
    | Center.F -> match edge with UF | DF | FL | FR -> true | _ -> false
    | Center.B -> match edge with UB | DB | BL | BR -> true | _ -> false

let isPairedCenterEdge (center, c) (edge, (e0, e1)) =
    isAttachedCenterEdge center edge && 
    match edge with
    | Edge.UL | Edge.UR | Edge.UF | Edge.UB | Edge.DL | Edge.DR | Edge.DF | Edge.DB ->
        match center with
        | Center.U | Center.D -> e0 = c // e0 and c represent U/D
        | Center.L | Center.R | Center.F | Center.B -> e1 = c // e1 and c represent L/R/F/B
    | Edge.FL | Edge.FR | Edge.BL | Edge.BR -> 
        match center with
        | Center.U | Center.D -> failwith "FL/FR/BL/BR edge cannot be attached to U/D center"
        | Center.L | Center.R -> e1 = c // e1 and c represent L/R
        | Center.F | Center.B -> e0 = c // e0 and c represent F/B

let isAttachedEdgeCorner edge corner =
    match edge with
    | Edge.UL -> match corner with ULF | ULB -> true | _ -> false
    | Edge.UR -> match corner with URF | URB -> true | _ -> false
    | Edge.UF -> match corner with ULF | URF -> true | _ -> false
    | Edge.UB -> match corner with ULB | URB -> true | _ -> false
    | Edge.DL -> match corner with DLF | DLB -> true | _ -> false
    | Edge.DR -> match corner with DRF | DRB -> true | _ -> false
    | Edge.DF -> match corner with DLF | DRF -> true | _ -> false
    | Edge.DB -> match corner with DLB | DRB -> true | _ -> false
    | Edge.FL -> match corner with ULF | DLF -> true | _ -> false
    | Edge.FR -> match corner with URF | DRF -> true | _ -> false
    | Edge.BL -> match corner with ULB | DLB -> true | _ -> false
    | Edge.BR -> match corner with URB | DRB -> true | _ -> false

let isPairedEdgeCorner (edge, (e0, e1)) (corner, (ud, lr, fb)) =
    isAttachedEdgeCorner edge corner && 
    match edge with
    | Edge.UL | Edge.UR | Edge.DL | Edge.DR -> e0 = ud && e1 = lr // e0 represent U/D and e1 represents L/R
    | Edge.UF | Edge.UB | Edge.DF | Edge.DB -> e0 = ud && e1 = fb // e0 represents U/D and e1 represents F/B
    | Edge.FL | Edge.FR | Edge.BL | Edge.BR -> e0 = fb && e1 = lr // e0 represent F/B and e1 represents L/R

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

let look face sticker cube = cube |> Map.find face |> Map.find sticker

let searchCenters centers color cube =
    let find c =
        let f, s = centerToFaceSticker c
        look f s cube = color
    Seq.find find centers, color
let findCenter color cube = searchCenters [Center.U; Center.D; Center.L; Center.R; Center.F; Center.B] color cube

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
let findEdge color0 color1 cube = searchEdges [UL; UR; UF; UB; DL; DR; DF; DB; FL; FR; BL; BR] color0 color1 cube

let searchCorners corners color0 color1 color2 cube =
    let rec find = function
        | c :: t ->
            match cornerToFaceStickers c with
            | [f0, s0; f1, s1; f2, s2] ->
                let l0 = look f0 s0 cube
                let l1 = look f1 s1 cube
                let l2 = look f2 s2 cube
                if (l0 = color0 || l1 = color0 || l2 = color0) &&
                   (l0 = color1 || l1 = color1 || l2 = color1) &&
                   (l0 = color2 || l1 = color2 || l2 = color2) then c, (l0, l1, l2)
                else find t
            | _ -> failwith "Expected exactly two edge stickers"
        | _ -> failwith "Edge not found"
    find corners
let findCorner color0 color1 color2 cube = searchCorners [ULF; ULB; URF; URB; DLF; DLB; DRF; DRB] color0 color1 color2 cube

let solved =
    let u = faceOfStickers Color.W Color.W Color.W Color.W Color.W Color.W Color.W Color.W Color.W
    let d = faceOfStickers Color.Y Color.Y Color.Y Color.Y Color.Y Color.Y Color.Y Color.Y Color.Y
    let l = faceOfStickers Color.O Color.O Color.O Color.O Color.O Color.O Color.O Color.O Color.O
    let r = faceOfStickers Color.R Color.R Color.R Color.R Color.R Color.R Color.R Color.R Color.R
    let f = faceOfStickers Color.G Color.G Color.G Color.G Color.G Color.G Color.G Color.G Color.G
    let b = faceOfStickers Color.B Color.B Color.B Color.B Color.B Color.B Color.B Color.B Color.B
    cubeOfFaces u d l r f b
