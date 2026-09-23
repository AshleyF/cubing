import { Move, Step, Rotate, cubeOfFaces, faceOfStickers, Center, Edge, Piece, Corner, Sticker, Face, look, Color } from "./Cube.js";
import { FSharpSet__Contains, ofList } from "../fable_modules/fable-library-js.4.16.0/Set.js";
import { equals, compare } from "../fable_modules/fable-library-js.4.16.0/Util.js";
import { split as split_1, join } from "../fable_modules/fable-library-js.4.16.0/String.js";
import { map, singleton, append, delay } from "../fable_modules/fable-library-js.4.16.0/Seq.js";
import { ofSeq, ofArray, empty } from "../fable_modules/fable-library-js.4.16.0/List.js";

export const interactive = false;

export function colorToString(_arg) {
    switch (_arg.tag) {
        case 1:
            return "O";
        case 2:
            return "W";
        case 3:
            return "Y";
        case 4:
            return "B";
        case 5:
            return "G";
        case 6:
            return ".";
        default:
            return "R";
    }
}

export function charToColor(_arg) {
    switch (_arg) {
        case ".":
            return new Color(6, []);
        case "B":
            return new Color(4, []);
        case "G":
            return new Color(5, []);
        case "O":
            return new Color(1, []);
        case "R":
            return new Color(0, []);
        case "W":
            return new Color(2, []);
        case "Y":
            return new Color(3, []);
        default:
            throw new Error("Invalid color character");
    }
}

export function piecesToStringWithEdgeOrientation(cube, pieces, edges) {
    const piecesSet = ofList(pieces, {
        Compare: compare,
    });
    const edgeSet = ofList(edges, {
        Compare: compare,
    });
    const lookSticker = (face, sticker, cube_1, piece) => {
        if (FSharpSet__Contains(edgeSet, piece)) {
            const c = look(face, sticker, cube_1);
            if (equals(c, new Color(4, []))) {
                return "*";
            }
            else if (equals(c, new Color(5, []))) {
                return "*";
            }
            else if (equals(face, new Face(0, [])) ? true : equals(face, new Face(1, []))) {
                if (equals(c, new Color(3, [])) ? true : equals(c, new Color(2, []))) {
                    return "E";
                }
                else {
                    return "P";
                }
            }
            else if (equals(c, new Color(3, [])) ? true : equals(c, new Color(2, []))) {
                return "P";
            }
            else {
                return "E";
            }
        }
        else if (FSharpSet__Contains(piecesSet, piece)) {
            return colorToString(look(face, sticker, cube_1));
        }
        else {
            return ".";
        }
    };
    return join("", delay(() => append(singleton(lookSticker(new Face(5, []), new Sticker(0, []), cube, new Piece(2, [new Corner(5, [])]))), delay(() => append(singleton(lookSticker(new Face(5, []), new Sticker(1, []), cube, new Piece(1, [new Edge(7, [])]))), delay(() => append(singleton(lookSticker(new Face(5, []), new Sticker(2, []), cube, new Piece(2, [new Corner(7, [])]))), delay(() => append(singleton(lookSticker(new Face(5, []), new Sticker(3, []), cube, new Piece(1, [new Edge(10, [])]))), delay(() => append(singleton(lookSticker(new Face(5, []), new Sticker(4, []), cube, new Piece(0, [new Center(5, [])]))), delay(() => append(singleton(lookSticker(new Face(5, []), new Sticker(5, []), cube, new Piece(1, [new Edge(11, [])]))), delay(() => append(singleton(lookSticker(new Face(5, []), new Sticker(6, []), cube, new Piece(2, [new Corner(1, [])]))), delay(() => append(singleton(lookSticker(new Face(5, []), new Sticker(7, []), cube, new Piece(1, [new Edge(3, [])]))), delay(() => append(singleton(lookSticker(new Face(5, []), new Sticker(8, []), cube, new Piece(2, [new Corner(3, [])]))), delay(() => append(singleton(lookSticker(new Face(0, []), new Sticker(0, []), cube, new Piece(2, [new Corner(1, [])]))), delay(() => append(singleton(lookSticker(new Face(0, []), new Sticker(1, []), cube, new Piece(1, [new Edge(3, [])]))), delay(() => append(singleton(lookSticker(new Face(0, []), new Sticker(2, []), cube, new Piece(2, [new Corner(3, [])]))), delay(() => append(singleton(lookSticker(new Face(0, []), new Sticker(3, []), cube, new Piece(1, [new Edge(0, [])]))), delay(() => append(singleton(lookSticker(new Face(0, []), new Sticker(4, []), cube, new Piece(0, [new Center(0, [])]))), delay(() => append(singleton(lookSticker(new Face(0, []), new Sticker(5, []), cube, new Piece(1, [new Edge(1, [])]))), delay(() => append(singleton(lookSticker(new Face(0, []), new Sticker(6, []), cube, new Piece(2, [new Corner(0, [])]))), delay(() => append(singleton(lookSticker(new Face(0, []), new Sticker(7, []), cube, new Piece(1, [new Edge(2, [])]))), delay(() => append(singleton(lookSticker(new Face(0, []), new Sticker(8, []), cube, new Piece(2, [new Corner(2, [])]))), delay(() => append(singleton(lookSticker(new Face(2, []), new Sticker(0, []), cube, new Piece(2, [new Corner(1, [])]))), delay(() => append(singleton(lookSticker(new Face(2, []), new Sticker(1, []), cube, new Piece(1, [new Edge(0, [])]))), delay(() => append(singleton(lookSticker(new Face(2, []), new Sticker(2, []), cube, new Piece(2, [new Corner(0, [])]))), delay(() => append(singleton(lookSticker(new Face(4, []), new Sticker(0, []), cube, new Piece(2, [new Corner(0, [])]))), delay(() => append(singleton(lookSticker(new Face(4, []), new Sticker(1, []), cube, new Piece(1, [new Edge(2, [])]))), delay(() => append(singleton(lookSticker(new Face(4, []), new Sticker(2, []), cube, new Piece(2, [new Corner(2, [])]))), delay(() => append(singleton(lookSticker(new Face(3, []), new Sticker(0, []), cube, new Piece(2, [new Corner(2, [])]))), delay(() => append(singleton(lookSticker(new Face(3, []), new Sticker(1, []), cube, new Piece(1, [new Edge(1, [])]))), delay(() => append(singleton(lookSticker(new Face(3, []), new Sticker(2, []), cube, new Piece(2, [new Corner(3, [])]))), delay(() => append(singleton(lookSticker(new Face(2, []), new Sticker(3, []), cube, new Piece(1, [new Edge(10, [])]))), delay(() => append(singleton(lookSticker(new Face(2, []), new Sticker(4, []), cube, new Piece(0, [new Center(2, [])]))), delay(() => append(singleton(lookSticker(new Face(2, []), new Sticker(5, []), cube, new Piece(1, [new Edge(8, [])]))), delay(() => append(singleton(lookSticker(new Face(4, []), new Sticker(3, []), cube, new Piece(1, [new Edge(8, [])]))), delay(() => append(singleton(lookSticker(new Face(4, []), new Sticker(4, []), cube, new Piece(0, [new Center(4, [])]))), delay(() => append(singleton(lookSticker(new Face(4, []), new Sticker(5, []), cube, new Piece(1, [new Edge(9, [])]))), delay(() => append(singleton(lookSticker(new Face(3, []), new Sticker(3, []), cube, new Piece(1, [new Edge(9, [])]))), delay(() => append(singleton(lookSticker(new Face(3, []), new Sticker(4, []), cube, new Piece(0, [new Center(3, [])]))), delay(() => append(singleton(lookSticker(new Face(3, []), new Sticker(5, []), cube, new Piece(1, [new Edge(11, [])]))), delay(() => append(singleton(lookSticker(new Face(2, []), new Sticker(6, []), cube, new Piece(2, [new Corner(5, [])]))), delay(() => append(singleton(lookSticker(new Face(2, []), new Sticker(7, []), cube, new Piece(1, [new Edge(4, [])]))), delay(() => append(singleton(lookSticker(new Face(2, []), new Sticker(8, []), cube, new Piece(2, [new Corner(4, [])]))), delay(() => append(singleton(lookSticker(new Face(4, []), new Sticker(6, []), cube, new Piece(2, [new Corner(4, [])]))), delay(() => append(singleton(lookSticker(new Face(4, []), new Sticker(7, []), cube, new Piece(1, [new Edge(6, [])]))), delay(() => append(singleton(lookSticker(new Face(4, []), new Sticker(8, []), cube, new Piece(2, [new Corner(6, [])]))), delay(() => append(singleton(lookSticker(new Face(3, []), new Sticker(6, []), cube, new Piece(2, [new Corner(6, [])]))), delay(() => append(singleton(lookSticker(new Face(3, []), new Sticker(7, []), cube, new Piece(1, [new Edge(5, [])]))), delay(() => append(singleton(lookSticker(new Face(3, []), new Sticker(8, []), cube, new Piece(2, [new Corner(7, [])]))), delay(() => append(singleton(lookSticker(new Face(1, []), new Sticker(0, []), cube, new Piece(2, [new Corner(4, [])]))), delay(() => append(singleton(lookSticker(new Face(1, []), new Sticker(1, []), cube, new Piece(1, [new Edge(6, [])]))), delay(() => append(singleton(lookSticker(new Face(1, []), new Sticker(2, []), cube, new Piece(2, [new Corner(6, [])]))), delay(() => append(singleton(lookSticker(new Face(1, []), new Sticker(3, []), cube, new Piece(1, [new Edge(4, [])]))), delay(() => append(singleton(lookSticker(new Face(1, []), new Sticker(4, []), cube, new Piece(0, [new Center(1, [])]))), delay(() => append(singleton(lookSticker(new Face(1, []), new Sticker(5, []), cube, new Piece(1, [new Edge(5, [])]))), delay(() => append(singleton(lookSticker(new Face(1, []), new Sticker(6, []), cube, new Piece(2, [new Corner(5, [])]))), delay(() => append(singleton(lookSticker(new Face(1, []), new Sticker(7, []), cube, new Piece(1, [new Edge(7, [])]))), delay(() => singleton(lookSticker(new Face(1, []), new Sticker(8, []), cube, new Piece(2, [new Corner(7, [])])))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))));
}

export function piecesToString(cube, pieces) {
    return piecesToStringWithEdgeOrientation(cube, pieces, empty());
}

export const piecesAll = ofArray([new Piece(0, [new Center(0, [])]), new Piece(0, [new Center(1, [])]), new Piece(0, [new Center(2, [])]), new Piece(0, [new Center(3, [])]), new Piece(0, [new Center(4, [])]), new Piece(0, [new Center(5, [])]), new Piece(1, [new Edge(0, [])]), new Piece(1, [new Edge(1, [])]), new Piece(1, [new Edge(2, [])]), new Piece(1, [new Edge(3, [])]), new Piece(1, [new Edge(4, [])]), new Piece(1, [new Edge(5, [])]), new Piece(1, [new Edge(6, [])]), new Piece(1, [new Edge(7, [])]), new Piece(1, [new Edge(8, [])]), new Piece(1, [new Edge(9, [])]), new Piece(1, [new Edge(10, [])]), new Piece(1, [new Edge(11, [])]), new Piece(2, [new Corner(0, [])]), new Piece(2, [new Corner(1, [])]), new Piece(2, [new Corner(2, [])]), new Piece(2, [new Corner(3, [])]), new Piece(2, [new Corner(4, [])]), new Piece(2, [new Corner(5, [])]), new Piece(2, [new Corner(6, [])]), new Piece(2, [new Corner(7, [])])]);

export function cubeToString(cube) {
    return piecesToString(cube, piecesAll);
}

export function cubeToStringWithEdgeOrientation(cube) {
    return piecesToStringWithEdgeOrientation(cube, piecesAll, ofArray([new Piece(0, [new Center(0, [])]), new Piece(0, [new Center(1, [])]), new Piece(0, [new Center(4, [])]), new Piece(0, [new Center(5, [])]), new Piece(1, [new Edge(0, [])]), new Piece(1, [new Edge(1, [])]), new Piece(1, [new Edge(2, [])]), new Piece(1, [new Edge(3, [])]), new Piece(1, [new Edge(6, [])]), new Piece(1, [new Edge(7, [])])]));
}

export function stringToCube(s) {
    const c = (i) => charToColor(s[i]);
    const b = faceOfStickers(c(0), c(1), c(2), c(3), c(4), c(5), c(6), c(7), c(8));
    const u = faceOfStickers(c(9), c(10), c(11), c(12), c(13), c(14), c(15), c(16), c(17));
    const l = faceOfStickers(c(18), c(19), c(20), c(27), c(28), c(29), c(36), c(37), c(38));
    const f = faceOfStickers(c(21), c(22), c(23), c(30), c(31), c(32), c(39), c(40), c(41));
    const r = faceOfStickers(c(24), c(25), c(26), c(33), c(34), c(35), c(42), c(43), c(44));
    return cubeOfFaces(u, faceOfStickers(c(45), c(46), c(47), c(48), c(49), c(50), c(51), c(52), c(53)), l, r, f, b);
}

export function rotationToString(_arg) {
    switch (_arg.tag) {
        case 1:
            return "x\'";
        case 2:
            return "x2";
        case 3:
            return "y";
        case 4:
            return "y\'";
        case 5:
            return "y2";
        case 6:
            return "z";
        case 7:
            return "z\'";
        case 8:
            return "z2";
        default:
            return "x";
    }
}

export function rotationsToString(rots) {
    return join(" ", map(rotationToString, rots));
}

export function moveToString(_arg) {
    switch (_arg.tag) {
        case 1:
            return "U\'";
        case 2:
            return "U2";
        case 3:
            return "u";
        case 4:
            return "u\'";
        case 5:
            return "u2";
        case 6:
            return "D";
        case 7:
            return "D\'";
        case 8:
            return "D2";
        case 9:
            return "d";
        case 10:
            return "d\'";
        case 11:
            return "d2";
        case 12:
            return "L";
        case 13:
            return "L\'";
        case 14:
            return "L2";
        case 15:
            return "l";
        case 16:
            return "l\'";
        case 17:
            return "l2";
        case 18:
            return "R";
        case 19:
            return "R\'";
        case 20:
            return "R2";
        case 21:
            return "r";
        case 22:
            return "r\'";
        case 23:
            return "r2";
        case 24:
            return "F";
        case 25:
            return "F\'";
        case 26:
            return "F2";
        case 27:
            return "f";
        case 28:
            return "f\'";
        case 29:
            return "f2";
        case 30:
            return "B";
        case 31:
            return "B\'";
        case 32:
            return "B2";
        case 33:
            return "b";
        case 34:
            return "b\'";
        case 35:
            return "b2";
        case 36:
            return "M";
        case 37:
            return "M\'";
        case 38:
            return "M2";
        case 39:
            return "S";
        case 40:
            return "S\'";
        case 41:
            return "S2";
        case 42:
            return "E";
        case 43:
            return "E\'";
        case 44:
            return "E2";
        default:
            return "U";
    }
}

export function movesToString(moves) {
    return join(" ", map(moveToString, moves));
}

export function stringToStep(_arg) {
    switch (_arg) {
        case "x":
            return new Step(0, [new Rotate(0, [])]);
        case "x\'":
            return new Step(0, [new Rotate(1, [])]);
        case "x2":
            return new Step(0, [new Rotate(2, [])]);
        case "x2\'":
            return new Step(0, [new Rotate(2, [])]);
        case "y":
            return new Step(0, [new Rotate(3, [])]);
        case "y\'":
            return new Step(0, [new Rotate(4, [])]);
        case "y2":
            return new Step(0, [new Rotate(5, [])]);
        case "y2\'":
            return new Step(0, [new Rotate(5, [])]);
        case "z":
            return new Step(0, [new Rotate(6, [])]);
        case "z\'":
            return new Step(0, [new Rotate(7, [])]);
        case "z2":
            return new Step(0, [new Rotate(8, [])]);
        case "z2\'":
            return new Step(0, [new Rotate(8, [])]);
        case "U":
            return new Step(1, [new Move(0, [])]);
        case "U\'":
            return new Step(1, [new Move(1, [])]);
        case "U2":
            return new Step(1, [new Move(2, [])]);
        case "U2\'":
            return new Step(1, [new Move(2, [])]);
        case "u":
            return new Step(1, [new Move(3, [])]);
        case "u\'":
            return new Step(1, [new Move(4, [])]);
        case "u2":
            return new Step(1, [new Move(5, [])]);
        case "u2\'":
            return new Step(1, [new Move(5, [])]);
        case "D":
            return new Step(1, [new Move(6, [])]);
        case "D\'":
            return new Step(1, [new Move(7, [])]);
        case "D2":
            return new Step(1, [new Move(8, [])]);
        case "D2\'":
            return new Step(1, [new Move(8, [])]);
        case "d":
            return new Step(1, [new Move(9, [])]);
        case "d\'":
            return new Step(1, [new Move(10, [])]);
        case "d2":
            return new Step(1, [new Move(11, [])]);
        case "d2\'":
            return new Step(1, [new Move(11, [])]);
        case "L":
            return new Step(1, [new Move(12, [])]);
        case "L\'":
            return new Step(1, [new Move(13, [])]);
        case "L2":
            return new Step(1, [new Move(14, [])]);
        case "L2\'":
            return new Step(1, [new Move(14, [])]);
        case "l":
            return new Step(1, [new Move(15, [])]);
        case "l\'":
            return new Step(1, [new Move(16, [])]);
        case "l2":
            return new Step(1, [new Move(17, [])]);
        case "l2\'":
            return new Step(1, [new Move(17, [])]);
        case "R":
            return new Step(1, [new Move(18, [])]);
        case "R\'":
            return new Step(1, [new Move(19, [])]);
        case "R2":
            return new Step(1, [new Move(20, [])]);
        case "R2\'":
            return new Step(1, [new Move(20, [])]);
        case "r":
            return new Step(1, [new Move(21, [])]);
        case "r\'":
            return new Step(1, [new Move(22, [])]);
        case "r2":
            return new Step(1, [new Move(23, [])]);
        case "r2\'":
            return new Step(1, [new Move(23, [])]);
        case "F":
            return new Step(1, [new Move(24, [])]);
        case "F\'":
            return new Step(1, [new Move(25, [])]);
        case "F2":
            return new Step(1, [new Move(26, [])]);
        case "F2\'":
            return new Step(1, [new Move(26, [])]);
        case "f":
            return new Step(1, [new Move(27, [])]);
        case "f\'":
            return new Step(1, [new Move(28, [])]);
        case "f2":
            return new Step(1, [new Move(29, [])]);
        case "f2\'":
            return new Step(1, [new Move(29, [])]);
        case "B":
            return new Step(1, [new Move(30, [])]);
        case "B\'":
            return new Step(1, [new Move(31, [])]);
        case "B2":
            return new Step(1, [new Move(32, [])]);
        case "B2\'":
            return new Step(1, [new Move(32, [])]);
        case "b":
            return new Step(1, [new Move(33, [])]);
        case "b\'":
            return new Step(1, [new Move(34, [])]);
        case "b2":
            return new Step(1, [new Move(35, [])]);
        case "b2\'":
            return new Step(1, [new Move(35, [])]);
        case "M":
            return new Step(1, [new Move(36, [])]);
        case "M\'":
            return new Step(1, [new Move(37, [])]);
        case "M2":
            return new Step(1, [new Move(38, [])]);
        case "M2\'":
            return new Step(1, [new Move(38, [])]);
        case "S":
            return new Step(1, [new Move(39, [])]);
        case "S\'":
            return new Step(1, [new Move(40, [])]);
        case "S2":
            return new Step(1, [new Move(41, [])]);
        case "S2\'":
            return new Step(1, [new Move(41, [])]);
        case "E":
            return new Step(1, [new Move(42, [])]);
        case "E\'":
            return new Step(1, [new Move(43, [])]);
        case "E2":
            return new Step(1, [new Move(44, [])]);
        case "E2\'":
            return new Step(1, [new Move(44, [])]);
        default:
            throw new Error("Unknown step notation");
    }
}

export function stringToSteps(alg) {
    return ofSeq(map(stringToStep, split_1(alg, [" "], void 0, 0)));
}

export function stepToString(_arg) {
    if (_arg.tag === 1) {
        return moveToString(_arg.fields[0]);
    }
    else {
        return rotationToString(_arg.fields[0]);
    }
}

export function stepsToString(steps) {
    return join(" ", map(stepToString, steps));
}

