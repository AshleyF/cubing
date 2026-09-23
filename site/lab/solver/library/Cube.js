import { Union } from "../fable_modules/fable-library-js.4.16.0/Types.js";
import { union_type } from "../fable_modules/fable-library-js.4.16.0/Reflection.js";
import { find as find_1, ofList } from "../fable_modules/fable-library-js.4.16.0/Map.js";
import { tail, head, isEmpty, append, map, reverse, ofArray } from "../fable_modules/fable-library-js.4.16.0/List.js";
import { equals, createAtom, compare } from "../fable_modules/fable-library-js.4.16.0/Util.js";
import { find as find_2, length, fold } from "../fable_modules/fable-library-js.4.16.0/Seq.js";

export class Color extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["R", "O", "W", "Y", "B", "G", "A"];
    }
}

export function Color_$reflection() {
    return union_type("Cube.Color", [], Color, () => [[], [], [], [], [], [], []]);
}

export class Face extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["U", "D", "L", "R", "F", "B"];
    }
}

export function Face_$reflection() {
    return union_type("Cube.Face", [], Face, () => [[], [], [], [], [], []]);
}

export class Sticker extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["UL", "U", "UR", "L", "C", "R", "DL", "D", "DR"];
    }
}

export function Sticker_$reflection() {
    return union_type("Cube.Sticker", [], Sticker, () => [[], [], [], [], [], [], [], [], []]);
}

export class Slice extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["U", "D", "L", "R", "F", "B", "M", "S", "E"];
    }
}

export function Slice_$reflection() {
    return union_type("Cube.Slice", [], Slice, () => [[], [], [], [], [], [], [], [], []]);
}

export function faceOfStickers(ul, u, ur, l, c, r, dl, d, dr) {
    return ofList(ofArray([[new Sticker(0, []), ul], [new Sticker(1, []), u], [new Sticker(2, []), ur], [new Sticker(3, []), l], [new Sticker(4, []), c], [new Sticker(5, []), r], [new Sticker(6, []), dl], [new Sticker(7, []), d], [new Sticker(8, []), dr]]), {
        Compare: compare,
    });
}

export function cubeOfFaces(u, d, l, r, f, b) {
    return ofList(ofArray([[new Face(0, []), u], [new Face(1, []), d], [new Face(2, []), l], [new Face(3, []), r], [new Face(4, []), f], [new Face(5, []), b]]), {
        Compare: compare,
    });
}

export function faceU(c) {
    return find_1(new Face(0, []), c);
}

export function faceD(c) {
    return find_1(new Face(1, []), c);
}

export function faceL(c) {
    return find_1(new Face(2, []), c);
}

export function faceR(c) {
    return find_1(new Face(3, []), c);
}

export function faceF(c) {
    return find_1(new Face(4, []), c);
}

export function faceB(c) {
    return find_1(new Face(5, []), c);
}

export function stickerUL(c) {
    return find_1(new Sticker(0, []), c);
}

export function stickerU(c) {
    return find_1(new Sticker(1, []), c);
}

export function stickerUR(c) {
    return find_1(new Sticker(2, []), c);
}

export function stickerL(c) {
    return find_1(new Sticker(3, []), c);
}

export function stickerC(c) {
    return find_1(new Sticker(4, []), c);
}

export function stickerR(c) {
    return find_1(new Sticker(5, []), c);
}

export function stickerDL(c) {
    return find_1(new Sticker(6, []), c);
}

export function stickerD(c) {
    return find_1(new Sticker(7, []), c);
}

export function stickerDR(c) {
    return find_1(new Sticker(8, []), c);
}

export function faceRotateCW(face) {
    return faceOfStickers(stickerDL(face), stickerL(face), stickerUL(face), stickerD(face), stickerC(face), stickerU(face), stickerDR(face), stickerR(face), stickerUR(face));
}

export function faceRotateCCW(face) {
    return faceOfStickers(stickerUR(face), stickerR(face), stickerDR(face), stickerU(face), stickerC(face), stickerD(face), stickerUL(face), stickerL(face), stickerDL(face));
}

export function faceRotate180(c) {
    return faceRotateCW(faceRotateCW(c));
}

export function rotateX(cube) {
    return cubeOfFaces(faceF(cube), faceB(cube), faceRotateCCW(faceL(cube)), faceRotateCW(faceR(cube)), faceD(cube), faceU(cube));
}

export function rotateX$0027(cube) {
    return cubeOfFaces(faceB(cube), faceF(cube), faceRotateCW(faceL(cube)), faceRotateCCW(faceR(cube)), faceU(cube), faceD(cube));
}

export function rotateX2(c) {
    return rotateX(rotateX(c));
}

export function rotateY(cube) {
    return cubeOfFaces(faceRotateCW(faceU(cube)), faceRotateCCW(faceD(cube)), faceF(cube), faceRotate180(faceB(cube)), faceR(cube), faceRotate180(faceL(cube)));
}

export function rotateY$0027(cube) {
    return cubeOfFaces(faceRotateCCW(faceU(cube)), faceRotateCW(faceD(cube)), faceRotate180(faceB(cube)), faceF(cube), faceL(cube), faceRotate180(faceR(cube)));
}

export function rotateY2(c) {
    return rotateY(rotateY(c));
}

export function rotateZ(cube) {
    return cubeOfFaces(faceRotateCW(faceL(cube)), faceRotateCW(faceR(cube)), faceRotateCW(faceD(cube)), faceRotateCW(faceU(cube)), faceRotateCW(faceF(cube)), faceRotateCCW(faceB(cube)));
}

export function rotateZ$0027(cube) {
    return cubeOfFaces(faceRotateCCW(faceR(cube)), faceRotateCCW(faceL(cube)), faceRotateCCW(faceU(cube)), faceRotateCCW(faceD(cube)), faceRotateCCW(faceF(cube)), faceRotateCW(faceB(cube)));
}

export function rotateZ2(c) {
    return rotateZ(rotateZ(c));
}

export function moveU(cube) {
    const l = faceL(cube);
    const r = faceR(cube);
    const f = faceF(cube);
    const b = faceB(cube);
    return cubeOfFaces(faceRotateCW(faceU(cube)), faceD(cube), faceOfStickers(stickerUL(f), stickerU(f), stickerUR(f), stickerL(l), stickerC(l), stickerR(l), stickerDL(l), stickerD(l), stickerDR(l)), faceOfStickers(stickerDR(b), stickerD(b), stickerDL(b), stickerL(r), stickerC(r), stickerR(r), stickerDL(r), stickerD(r), stickerDR(r)), faceOfStickers(stickerUL(r), stickerU(r), stickerUR(r), stickerL(f), stickerC(f), stickerR(f), stickerDL(f), stickerD(f), stickerDR(f)), faceOfStickers(stickerUL(b), stickerU(b), stickerUR(b), stickerL(b), stickerC(b), stickerR(b), stickerUR(l), stickerU(l), stickerUL(l)));
}

export function moveU$0027(cube) {
    const l = faceL(cube);
    const r = faceR(cube);
    const f = faceF(cube);
    const b = faceB(cube);
    return cubeOfFaces(faceRotateCCW(faceU(cube)), faceD(cube), faceOfStickers(stickerDR(b), stickerD(b), stickerDL(b), stickerL(l), stickerC(l), stickerR(l), stickerDL(l), stickerD(l), stickerDR(l)), faceOfStickers(stickerUL(f), stickerU(f), stickerUR(f), stickerL(r), stickerC(r), stickerR(r), stickerDL(r), stickerD(r), stickerDR(r)), faceOfStickers(stickerUL(l), stickerU(l), stickerUR(l), stickerL(f), stickerC(f), stickerR(f), stickerDL(f), stickerD(f), stickerDR(f)), faceOfStickers(stickerUL(b), stickerU(b), stickerUR(b), stickerL(b), stickerC(b), stickerR(b), stickerUR(r), stickerU(r), stickerUL(r)));
}

export function moveU2(c) {
    return moveU(moveU(c));
}

export function moveD(c) {
    return rotateX2(moveU(rotateX2(c)));
}

export function moveD$0027(c) {
    return rotateX2(moveU$0027(rotateX2(c)));
}

export function moveD2(c) {
    return moveD(moveD(c));
}

export function moveL(c) {
    return rotateZ$0027(moveU(rotateZ(c)));
}

export function moveL$0027(c) {
    return rotateZ$0027(moveU$0027(rotateZ(c)));
}

export function moveL2(c) {
    return moveL(moveL(c));
}

export function moveR(c) {
    return rotateZ(moveU(rotateZ$0027(c)));
}

export function moveR$0027(c) {
    return rotateZ(moveU$0027(rotateZ$0027(c)));
}

export function moveR2(c) {
    return moveR(moveR(c));
}

export function moveF(c) {
    return rotateX$0027(moveU(rotateX(c)));
}

export function moveF$0027(c) {
    return rotateX$0027(moveU$0027(rotateX(c)));
}

export function moveF2(c) {
    return moveF(moveF(c));
}

export function moveB(c) {
    return rotateX(moveU(rotateX$0027(c)));
}

export function moveB$0027(c) {
    return rotateX(moveU$0027(rotateX$0027(c)));
}

export function moveB2(c) {
    return moveB(moveB(c));
}

export function moveM(c) {
    return moveR(moveL$0027(rotateX$0027(c)));
}

export function moveM$0027(c) {
    return moveR$0027(moveL(rotateX(c)));
}

export function moveM2(c) {
    return moveM(moveM(c));
}

export function moveE(c) {
    return moveD$0027(moveU(rotateY$0027(c)));
}

export function moveE$0027(c) {
    return moveD(moveU$0027(rotateY(c)));
}

export function moveE2(c) {
    return moveE(moveE(c));
}

export function moveS(c) {
    return moveB(moveF$0027(rotateZ(c)));
}

export function moveS$0027(c) {
    return moveB$0027(moveF(rotateZ$0027(c)));
}

export function moveS2(c) {
    return moveS(moveS(c));
}

export function moveUW(c) {
    return moveD(rotateY(c));
}

export function moveUW$0027(c) {
    return moveD$0027(rotateY$0027(c));
}

export function moveUW2(c) {
    return moveUW(moveUW(c));
}

export function moveDW(c) {
    return moveU(rotateY$0027(c));
}

export function moveDW$0027(c) {
    return moveU$0027(rotateY(c));
}

export function moveDW2(c) {
    return moveDW(moveDW(c));
}

export function moveLW(c) {
    return moveR(rotateX$0027(c));
}

export function moveLW$0027(c) {
    return moveR$0027(rotateX(c));
}

export function moveLW2(c) {
    return moveLW(moveLW(c));
}

export function moveRW(c) {
    return moveL(rotateX(c));
}

export function moveRW$0027(c) {
    return moveL$0027(rotateX$0027(c));
}

export function moveRW2(c) {
    return moveRW(moveRW(c));
}

export function moveFW(c) {
    return moveB(rotateZ(c));
}

export function moveFW$0027(c) {
    return moveB$0027(rotateZ$0027(c));
}

export function moveFW2(c) {
    return moveFW(moveFW(c));
}

export function moveBW(c) {
    return moveF(rotateZ$0027(c));
}

export function moveBW$0027(c) {
    return moveF$0027(rotateZ(c));
}

export function moveBW2(c) {
    return moveBW(moveBW(c));
}

export class Rotate extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["X", "X\'", "X2", "Y", "Y\'", "Y2", "Z", "Z\'", "Z2"];
    }
}

export function Rotate_$reflection() {
    return union_type("Cube.Rotate", [], Rotate, () => [[], [], [], [], [], [], [], [], []]);
}

export class Move extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["U", "U\'", "U2", "UW", "UW\'", "UW2", "D", "D\'", "D2", "DW", "DW\'", "DW2", "L", "L\'", "L2", "LW", "LW\'", "LW2", "R", "R\'", "R2", "RW", "RW\'", "RW2", "F", "F\'", "F2", "FW", "FW\'", "FW2", "B", "B\'", "B2", "BW", "BW\'", "BW2", "M", "M\'", "M2", "S", "S\'", "S2", "E", "E\'", "E2"];
    }
}

export function Move_$reflection() {
    return union_type("Cube.Move", [], Move, () => [[], [], [], [], [], [], [], [], [], [], [], [], [], [], [], [], [], [], [], [], [], [], [], [], [], [], [], [], [], [], [], [], [], [], [], [], [], [], [], [], [], [], [], [], []]);
}

export class Step extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["Rotate", "Move"];
    }
}

export function Step_$reflection() {
    return union_type("Cube.Step", [], Step, () => [[["Item", Rotate_$reflection()]], [["Item", Move_$reflection()]]]);
}

export function inverseRotation(_arg) {
    switch (_arg.tag) {
        case 1:
            return new Rotate(0, []);
        case 2:
            return new Rotate(2, []);
        case 3:
            return new Rotate(4, []);
        case 4:
            return new Rotate(3, []);
        case 5:
            return new Rotate(5, []);
        case 6:
            return new Rotate(7, []);
        case 7:
            return new Rotate(6, []);
        case 8:
            return new Rotate(8, []);
        default:
            return new Rotate(1, []);
    }
}

export function inverseMove(_arg) {
    switch (_arg.tag) {
        case 1:
            return new Move(0, []);
        case 2:
            return new Move(2, []);
        case 3:
            return new Move(4, []);
        case 4:
            return new Move(3, []);
        case 5:
            return new Move(5, []);
        case 6:
            return new Move(7, []);
        case 7:
            return new Move(6, []);
        case 8:
            return new Move(8, []);
        case 9:
            return new Move(10, []);
        case 10:
            return new Move(9, []);
        case 11:
            return new Move(11, []);
        case 12:
            return new Move(13, []);
        case 13:
            return new Move(12, []);
        case 14:
            return new Move(14, []);
        case 15:
            return new Move(16, []);
        case 16:
            return new Move(15, []);
        case 17:
            return new Move(17, []);
        case 18:
            return new Move(19, []);
        case 19:
            return new Move(18, []);
        case 20:
            return new Move(20, []);
        case 21:
            return new Move(22, []);
        case 22:
            return new Move(21, []);
        case 23:
            return new Move(23, []);
        case 24:
            return new Move(25, []);
        case 25:
            return new Move(24, []);
        case 26:
            return new Move(26, []);
        case 27:
            return new Move(28, []);
        case 28:
            return new Move(27, []);
        case 29:
            return new Move(29, []);
        case 30:
            return new Move(31, []);
        case 31:
            return new Move(30, []);
        case 32:
            return new Move(32, []);
        case 33:
            return new Move(34, []);
        case 34:
            return new Move(33, []);
        case 35:
            return new Move(35, []);
        case 36:
            return new Move(37, []);
        case 37:
            return new Move(36, []);
        case 38:
            return new Move(38, []);
        case 39:
            return new Move(40, []);
        case 40:
            return new Move(39, []);
        case 41:
            return new Move(41, []);
        case 42:
            return new Move(43, []);
        case 43:
            return new Move(42, []);
        case 44:
            return new Move(44, []);
        default:
            return new Move(1, []);
    }
}

export function inverseStep(_arg) {
    if (_arg.tag === 1) {
        return new Step(1, [inverseMove(_arg.fields[0])]);
    }
    else {
        return new Step(0, [inverseRotation(_arg.fields[0])]);
    }
}

export const inverseRotations = (arg) => reverse(map(inverseRotation, arg));

export const inverseMoves = (arg) => reverse(map(inverseMove, arg));

export const inverseSteps = (arg) => reverse(map(inverseStep, arg));

export function rotate(_arg) {
    switch (_arg.tag) {
        case 1:
            return rotateX$0027;
        case 2:
            return rotateX2;
        case 3:
            return rotateY;
        case 4:
            return rotateY$0027;
        case 5:
            return rotateY2;
        case 6:
            return rotateZ;
        case 7:
            return rotateZ$0027;
        case 8:
            return rotateZ2;
        default:
            return rotateX;
    }
}

export function move(_arg) {
    switch (_arg.tag) {
        case 1:
            return moveU$0027;
        case 2:
            return moveU2;
        case 3:
            return moveUW;
        case 4:
            return moveUW$0027;
        case 5:
            return moveUW2;
        case 6:
            return moveD;
        case 7:
            return moveD$0027;
        case 8:
            return moveD2;
        case 9:
            return moveDW;
        case 10:
            return moveDW$0027;
        case 11:
            return moveDW2;
        case 12:
            return moveL;
        case 13:
            return moveL$0027;
        case 14:
            return moveL2;
        case 15:
            return moveLW;
        case 16:
            return moveLW$0027;
        case 17:
            return moveLW2;
        case 18:
            return moveR;
        case 19:
            return moveR$0027;
        case 20:
            return moveR2;
        case 21:
            return moveRW;
        case 22:
            return moveRW$0027;
        case 23:
            return moveRW2;
        case 24:
            return moveF;
        case 25:
            return moveF$0027;
        case 26:
            return moveF2;
        case 27:
            return moveFW;
        case 28:
            return moveFW$0027;
        case 29:
            return moveFW2;
        case 30:
            return moveB;
        case 31:
            return moveB$0027;
        case 32:
            return moveB2;
        case 33:
            return moveBW;
        case 34:
            return moveBW$0027;
        case 35:
            return moveBW2;
        case 36:
            return moveM;
        case 37:
            return moveM$0027;
        case 38:
            return moveM2;
        case 39:
            return moveS;
        case 40:
            return moveS$0027;
        case 41:
            return moveS2;
        case 42:
            return moveE;
        case 43:
            return moveE$0027;
        case 44:
            return moveE2;
        default:
            return moveU;
    }
}

export const movesRegular = ofArray([new Move(0, []), new Move(1, []), new Move(2, []), new Move(6, []), new Move(7, []), new Move(8, []), new Move(12, []), new Move(13, []), new Move(14, []), new Move(18, []), new Move(19, []), new Move(20, []), new Move(24, []), new Move(25, []), new Move(26, []), new Move(30, []), new Move(31, []), new Move(32, []), new Move(36, []), new Move(37, []), new Move(38, [])]);

export const movesWide = ofArray([new Move(3, []), new Move(4, []), new Move(5, []), new Move(9, []), new Move(10, []), new Move(11, []), new Move(15, []), new Move(16, []), new Move(17, []), new Move(21, []), new Move(22, []), new Move(23, []), new Move(27, []), new Move(28, []), new Move(29, []), new Move(33, []), new Move(34, []), new Move(35, [])]);

export const movesM = ofArray([new Move(36, []), new Move(37, []), new Move(38, [])]);

export const movesSlice = append(movesM, ofArray([new Move(39, []), new Move(40, []), new Move(41, []), new Move(42, []), new Move(43, []), new Move(44, [])]));

export const movesAll = append(movesRegular, append(movesWide, movesSlice));

export const movesCommonRoux = append(movesRegular, append(movesWide, movesM));

export function step(_arg) {
    if (_arg.tag === 1) {
        return move(_arg.fields[0]);
    }
    else {
        return rotate(_arg.fields[0]);
    }
}

export function executeRotation(cube, r) {
    return rotate(r)(cube);
}

export function executeMove(cube, m) {
    return move(m)(cube);
}

export function executeStep(cube, s) {
    return step(s)(cube);
}

export function executeRotations(rs, cube) {
    return fold(executeRotation, cube, rs);
}

export function executeMoves(ms, cube) {
    return fold(executeMove, cube, ms);
}

export let twistCount = createAtom(0);

export let stageCount = createAtom(0);

export function executeSteps(ss, cube) {
    twistCount(twistCount() + length(ss));
    stageCount(stageCount() + length(ss));
    return fold(executeStep, cube, ss);
}

export class Center extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["U", "D", "L", "R", "F", "B"];
    }
}

export function Center_$reflection() {
    return union_type("Cube.Center", [], Center, () => [[], [], [], [], [], []]);
}

export class Edge extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["UL", "UR", "UF", "UB", "DL", "DR", "DF", "DB", "FL", "FR", "BL", "BR"];
    }
}

export function Edge_$reflection() {
    return union_type("Cube.Edge", [], Edge, () => [[], [], [], [], [], [], [], [], [], [], [], []]);
}

export class Corner extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["ULF", "ULB", "URF", "URB", "DLF", "DLB", "DRF", "DRB"];
    }
}

export function Corner_$reflection() {
    return union_type("Cube.Corner", [], Corner, () => [[], [], [], [], [], [], [], []]);
}

export class Piece extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["Center", "Edge", "Corner"];
    }
}

export function Piece_$reflection() {
    return union_type("Cube.Piece", [], Piece, () => [[["Item", Center_$reflection()]], [["Item", Edge_$reflection()]], [["Item", Corner_$reflection()]]]);
}

export function isAttachedCenterEdge(center, edge) {
    switch (center.tag) {
        case 1:
            switch (edge.tag) {
                case 4:
                case 5:
                case 6:
                case 7:
                    return true;
                default:
                    return false;
            }
        case 2:
            switch (edge.tag) {
                case 0:
                case 4:
                case 8:
                case 10:
                    return true;
                default:
                    return false;
            }
        case 3:
            switch (edge.tag) {
                case 1:
                case 5:
                case 9:
                case 11:
                    return true;
                default:
                    return false;
            }
        case 4:
            switch (edge.tag) {
                case 2:
                case 6:
                case 8:
                case 9:
                    return true;
                default:
                    return false;
            }
        case 5:
            switch (edge.tag) {
                case 3:
                case 7:
                case 10:
                case 11:
                    return true;
                default:
                    return false;
            }
        default:
            switch (edge.tag) {
                case 0:
                case 1:
                case 2:
                case 3:
                    return true;
                default:
                    return false;
            }
    }
}

export function isPairedCenterEdge(center, c, edge, _arg) {
    const e1 = _arg[1];
    const e0 = _arg[0];
    if (isAttachedCenterEdge(center, edge)) {
        switch (edge.tag) {
            case 8:
            case 9:
            case 10:
            case 11:
                switch (center.tag) {
                    case 2:
                    case 3:
                        return equals(e1, c);
                    case 4:
                    case 5:
                        return equals(e0, c);
                    default:
                        throw new Error("FL/FR/BL/BR edge cannot be attached to U/D center");
                }
            default:
                switch (center.tag) {
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                        return equals(e1, c);
                    default:
                        return equals(e0, c);
                }
        }
    }
    else {
        return false;
    }
}

export function isAttachedEdgeCorner(edge, corner) {
    switch (edge.tag) {
        case 1:
            switch (corner.tag) {
                case 2:
                case 3:
                    return true;
                default:
                    return false;
            }
        case 2:
            switch (corner.tag) {
                case 0:
                case 2:
                    return true;
                default:
                    return false;
            }
        case 3:
            switch (corner.tag) {
                case 1:
                case 3:
                    return true;
                default:
                    return false;
            }
        case 4:
            switch (corner.tag) {
                case 4:
                case 5:
                    return true;
                default:
                    return false;
            }
        case 5:
            switch (corner.tag) {
                case 6:
                case 7:
                    return true;
                default:
                    return false;
            }
        case 6:
            switch (corner.tag) {
                case 4:
                case 6:
                    return true;
                default:
                    return false;
            }
        case 7:
            switch (corner.tag) {
                case 5:
                case 7:
                    return true;
                default:
                    return false;
            }
        case 8:
            switch (corner.tag) {
                case 0:
                case 4:
                    return true;
                default:
                    return false;
            }
        case 9:
            switch (corner.tag) {
                case 2:
                case 6:
                    return true;
                default:
                    return false;
            }
        case 10:
            switch (corner.tag) {
                case 1:
                case 5:
                    return true;
                default:
                    return false;
            }
        case 11:
            switch (corner.tag) {
                case 3:
                case 7:
                    return true;
                default:
                    return false;
            }
        default:
            switch (corner.tag) {
                case 0:
                case 1:
                    return true;
                default:
                    return false;
            }
    }
}

export function isPairedEdgeCorner(edge, _arg, corner, _arg_1) {
    const e1 = _arg[1];
    const e0 = _arg[0];
    const ud = _arg_1[0];
    const lr = _arg_1[1];
    const fb = _arg_1[2];
    if (isAttachedEdgeCorner(edge, corner)) {
        switch (edge.tag) {
            case 2:
            case 3:
            case 6:
            case 7:
                if (equals(e0, ud)) {
                    return equals(e1, fb);
                }
                else {
                    return false;
                }
            case 8:
            case 9:
            case 10:
            case 11:
                if (equals(e0, fb)) {
                    return equals(e1, lr);
                }
                else {
                    return false;
                }
            default:
                if (equals(e0, ud)) {
                    return equals(e1, lr);
                }
                else {
                    return false;
                }
        }
    }
    else {
        return false;
    }
}

export function centerToFaceSticker(_arg) {
    switch (_arg.tag) {
        case 1:
            return [new Face(1, []), new Sticker(4, [])];
        case 2:
            return [new Face(2, []), new Sticker(4, [])];
        case 3:
            return [new Face(3, []), new Sticker(4, [])];
        case 4:
            return [new Face(4, []), new Sticker(4, [])];
        case 5:
            return [new Face(5, []), new Sticker(4, [])];
        default:
            return [new Face(0, []), new Sticker(4, [])];
    }
}

export function edgeToFaceStickers(_arg) {
    switch (_arg.tag) {
        case 1:
            return ofArray([[new Face(0, []), new Sticker(5, [])], [new Face(3, []), new Sticker(1, [])]]);
        case 2:
            return ofArray([[new Face(0, []), new Sticker(7, [])], [new Face(4, []), new Sticker(1, [])]]);
        case 3:
            return ofArray([[new Face(0, []), new Sticker(1, [])], [new Face(5, []), new Sticker(7, [])]]);
        case 4:
            return ofArray([[new Face(1, []), new Sticker(3, [])], [new Face(2, []), new Sticker(7, [])]]);
        case 5:
            return ofArray([[new Face(1, []), new Sticker(5, [])], [new Face(3, []), new Sticker(7, [])]]);
        case 6:
            return ofArray([[new Face(1, []), new Sticker(1, [])], [new Face(4, []), new Sticker(7, [])]]);
        case 7:
            return ofArray([[new Face(1, []), new Sticker(7, [])], [new Face(5, []), new Sticker(1, [])]]);
        case 8:
            return ofArray([[new Face(4, []), new Sticker(3, [])], [new Face(2, []), new Sticker(5, [])]]);
        case 9:
            return ofArray([[new Face(4, []), new Sticker(5, [])], [new Face(3, []), new Sticker(3, [])]]);
        case 10:
            return ofArray([[new Face(5, []), new Sticker(3, [])], [new Face(2, []), new Sticker(3, [])]]);
        case 11:
            return ofArray([[new Face(5, []), new Sticker(5, [])], [new Face(3, []), new Sticker(5, [])]]);
        default:
            return ofArray([[new Face(0, []), new Sticker(3, [])], [new Face(2, []), new Sticker(1, [])]]);
    }
}

export function cornerToFaceStickers(_arg) {
    switch (_arg.tag) {
        case 1:
            return ofArray([[new Face(0, []), new Sticker(0, [])], [new Face(2, []), new Sticker(0, [])], [new Face(5, []), new Sticker(6, [])]]);
        case 2:
            return ofArray([[new Face(0, []), new Sticker(8, [])], [new Face(3, []), new Sticker(0, [])], [new Face(4, []), new Sticker(2, [])]]);
        case 3:
            return ofArray([[new Face(0, []), new Sticker(2, [])], [new Face(3, []), new Sticker(2, [])], [new Face(5, []), new Sticker(8, [])]]);
        case 4:
            return ofArray([[new Face(1, []), new Sticker(0, [])], [new Face(2, []), new Sticker(8, [])], [new Face(4, []), new Sticker(6, [])]]);
        case 5:
            return ofArray([[new Face(1, []), new Sticker(6, [])], [new Face(2, []), new Sticker(6, [])], [new Face(5, []), new Sticker(0, [])]]);
        case 6:
            return ofArray([[new Face(1, []), new Sticker(2, [])], [new Face(3, []), new Sticker(6, [])], [new Face(4, []), new Sticker(8, [])]]);
        case 7:
            return ofArray([[new Face(1, []), new Sticker(8, [])], [new Face(3, []), new Sticker(8, [])], [new Face(5, []), new Sticker(2, [])]]);
        default:
            return ofArray([[new Face(0, []), new Sticker(6, [])], [new Face(2, []), new Sticker(2, [])], [new Face(4, []), new Sticker(0, [])]]);
    }
}

export function look(face, sticker, cube) {
    return find_1(sticker, find_1(face, cube));
}

export function searchCenters(centers, color, cube) {
    return [find_2((c) => {
        const patternInput = centerToFaceSticker(c);
        return equals(look(patternInput[0], patternInput[1], cube), color);
    }, centers), color];
}

export function findCenter(color, cube) {
    return searchCenters([new Center(0, []), new Center(1, []), new Center(2, []), new Center(3, []), new Center(4, []), new Center(5, [])], color, cube);
}

export function searchEdges(edges, color0, color1, cube) {
    const find = (_arg_mut) => {
        find:
        while (true) {
            const _arg = _arg_mut;
            if (!isEmpty(_arg)) {
                const e = head(_arg);
                const matchValue = edgeToFaceStickers(e);
                let matchResult, f0, f1, s0, s1;
                if (!isEmpty(matchValue)) {
                    if (!isEmpty(tail(matchValue))) {
                        if (isEmpty(tail(tail(matchValue)))) {
                            matchResult = 0;
                            f0 = head(matchValue)[0];
                            f1 = head(tail(matchValue))[0];
                            s0 = head(matchValue)[1];
                            s1 = head(tail(matchValue))[1];
                        }
                        else {
                            matchResult = 1;
                        }
                    }
                    else {
                        matchResult = 1;
                    }
                }
                else {
                    matchResult = 1;
                }
                switch (matchResult) {
                    case 0: {
                        const l0 = look(f0, s0, cube);
                        const l1 = look(f1, s1, cube);
                        if ((equals(l0, color0) && equals(l1, color1)) ? true : (equals(l1, color0) && equals(l0, color1))) {
                            return [e, [l0, l1]];
                        }
                        else {
                            _arg_mut = tail(_arg);
                            continue find;
                        }
                    }
                    default:
                        throw new Error("Expected exactly two edge stickers");
                }
            }
            else {
                throw new Error("Edge not found");
            }
            break;
        }
    };
    return find(edges);
}

export function findEdge(color0, color1, cube) {
    return searchEdges(ofArray([new Edge(0, []), new Edge(1, []), new Edge(2, []), new Edge(3, []), new Edge(4, []), new Edge(5, []), new Edge(6, []), new Edge(7, []), new Edge(8, []), new Edge(9, []), new Edge(10, []), new Edge(11, [])]), color0, color1, cube);
}

export function searchCorners(corners, color0, color1, color2, cube) {
    const find = (_arg_mut) => {
        find:
        while (true) {
            const _arg = _arg_mut;
            if (!isEmpty(_arg)) {
                const c = head(_arg);
                const matchValue = cornerToFaceStickers(c);
                let matchResult, f0, f1, f2, s0, s1, s2;
                if (!isEmpty(matchValue)) {
                    if (!isEmpty(tail(matchValue))) {
                        if (!isEmpty(tail(tail(matchValue)))) {
                            if (isEmpty(tail(tail(tail(matchValue))))) {
                                matchResult = 0;
                                f0 = head(matchValue)[0];
                                f1 = head(tail(matchValue))[0];
                                f2 = head(tail(tail(matchValue)))[0];
                                s0 = head(matchValue)[1];
                                s1 = head(tail(matchValue))[1];
                                s2 = head(tail(tail(matchValue)))[1];
                            }
                            else {
                                matchResult = 1;
                            }
                        }
                        else {
                            matchResult = 1;
                        }
                    }
                    else {
                        matchResult = 1;
                    }
                }
                else {
                    matchResult = 1;
                }
                switch (matchResult) {
                    case 0: {
                        const l0 = look(f0, s0, cube);
                        const l1 = look(f1, s1, cube);
                        const l2 = look(f2, s2, cube);
                        if ((((equals(l0, color0) ? true : equals(l1, color0)) ? true : equals(l2, color0)) && ((equals(l0, color1) ? true : equals(l1, color1)) ? true : equals(l2, color1))) && ((equals(l0, color2) ? true : equals(l1, color2)) ? true : equals(l2, color2))) {
                            return [c, [l0, l1, l2]];
                        }
                        else {
                            _arg_mut = tail(_arg);
                            continue find;
                        }
                    }
                    default:
                        throw new Error("Expected exactly two edge stickers");
                }
            }
            else {
                throw new Error("Edge not found");
            }
            break;
        }
    };
    return find(corners);
}

export function findCorner(color0, color1, color2, cube) {
    return searchCorners(ofArray([new Corner(0, []), new Corner(1, []), new Corner(2, []), new Corner(3, []), new Corner(4, []), new Corner(5, []), new Corner(6, []), new Corner(7, [])]), color0, color1, color2, cube);
}

export const solved = cubeOfFaces(faceOfStickers(new Color(2, []), new Color(2, []), new Color(2, []), new Color(2, []), new Color(2, []), new Color(2, []), new Color(2, []), new Color(2, []), new Color(2, [])), faceOfStickers(new Color(3, []), new Color(3, []), new Color(3, []), new Color(3, []), new Color(3, []), new Color(3, []), new Color(3, []), new Color(3, []), new Color(3, [])), faceOfStickers(new Color(1, []), new Color(1, []), new Color(1, []), new Color(1, []), new Color(1, []), new Color(1, []), new Color(1, []), new Color(1, []), new Color(1, [])), faceOfStickers(new Color(0, []), new Color(0, []), new Color(0, []), new Color(0, []), new Color(0, []), new Color(0, []), new Color(0, []), new Color(0, []), new Color(0, [])), faceOfStickers(new Color(5, []), new Color(5, []), new Color(5, []), new Color(5, []), new Color(5, []), new Color(5, []), new Color(5, []), new Color(5, []), new Color(5, [])), faceOfStickers(new Color(4, []), new Color(4, []), new Color(4, []), new Color(4, []), new Color(4, []), new Color(4, []), new Color(4, []), new Color(4, []), new Color(4, [])));

