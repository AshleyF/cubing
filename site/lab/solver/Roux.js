import { comparePrimitives, safeHash, stringHash, equals, createAtom } from "./fable_modules/fable-library-js.4.16.0/Util.js";
import { cubeToString, stringToSteps, piecesToString } from "./library/Render.js";
import { useEolr, chooseShortestSecondBlockPairOrder, orientCentersWithSecondBlock, chooseShortestFirstBlockPairOrder, rfPairLevel, rbPairLevel, lfPairLevel, lbPairLevel, edgeOrientationLevel, cornerPermutationLevel, cornerOrientationLevel, fullCmll, level, readPatterns } from "./Utility.js";
import { initScrambledCubes, solveWithStepsBy, preferGoalMatchingAlgorithm, solutionTrace, stageStats, lookPattern, expandPatternsForAuf, solveCase, matchesGeneric } from "./library/Solver.js";
import { head, minBy, cons, map3, item, tryFindIndex, tryFind, map2, tail, splitAt, mapIndexed, choose, concat, sumBy, skip, sortBy, tryHead, filter, isEmpty, collect, map, length, singleton, empty, ofArray, append } from "./fable_modules/fable-library-js.4.16.0/List.js";
import { Edge, Sticker, Face, look, executeSteps, Rotate, Step, Move, findCorner, findEdge, Piece, Color, findCenter } from "./library/Cube.js";
import { split, join, isNullOrWhiteSpace } from "./fable_modules/fable-library-js.4.16.0/String.js";
import { List_distinct } from "./fable_modules/fable-library-js.4.16.0/Seq2.js";
import { map as map_1, collect as collect_1, delay, toList } from "./fable_modules/fable-library-js.4.16.0/Seq.js";
import { rangeDouble } from "./fable_modules/fable-library-js.4.16.0/Range.js";
import { defaultArg, value as value_1, bind, map as map_2 } from "./fable_modules/fable-library-js.4.16.0/Option.js";
import { find, forAll } from "./fable_modules/fable-library-js.4.16.0/Map.js";

export const sune = "R U R\' U R U2 R\'";

export const jperm = "R U R\' F\' R U R\' U\' R\' F R2 U\' R\'";

export const mum = "M\' U\' M\'";

export const diagSwap = "r2 D r\' U r D\' R2 U\' F\' U\' F";

export const sexy = "R U R\' U\'";

export const sledge = "R\' F R F\'";

export let progressCallback = createAtom((value) => {
});

export function matchesPieces(pieces, cube, pattern, _arg, _arg_1) {
    return pattern === piecesToString(cube, pieces);
}

export const dlEdgeBeginnerPatterns = readPatterns((cube) => ((tupledArg) => matchesGeneric(cube, tupledArg[0], tupledArg[1], tupledArg[2])), "Roux", "Beginner", "DLEdge", false, false, false);

export const lCenterBeginnerPatterns = readPatterns((cube) => ((tupledArg) => matchesGeneric(cube, tupledArg[0], tupledArg[1], tupledArg[2])), "Roux", "Beginner", "LCenter", false, false, false);

export const lbPairBeginnerPatterns = append(readPatterns((cube) => ((tupledArg) => matchesGeneric(cube, tupledArg[0], tupledArg[1], tupledArg[2])), "Roux", "Beginner", "TuckLBtoFD", false, false, false), append(readPatterns((cube_1) => ((tupledArg_1) => matchesGeneric(cube_1, tupledArg_1[0], tupledArg_1[1], tupledArg_1[2])), "Roux", "Beginner", "BringDLBtoU", false, false, false), readPatterns((cube_2) => ((tupledArg_2) => matchesGeneric(cube_2, tupledArg_2[0], tupledArg_2[1], tupledArg_2[2])), "Roux", "Beginner", "InsertLBPair", false, false, false)));

export const lbPairIntermediatePatterns = readPatterns((cube) => ((tupledArg) => matchesGeneric(cube, tupledArg[0], tupledArg[1], tupledArg[2])), "Roux", "Intermediate", "InsertLBPair", false, false, false);

export function matchesLBSquare(cube) {
    const pieces = ofArray([new Piece(0, [findCenter(new Color(4, []), cube)[0]]), new Piece(1, [findEdge(new Color(4, []), new Color(2, []), cube)[0]]), new Piece(1, [findEdge(new Color(4, []), new Color(1, []), cube)[0]]), new Piece(2, [findCorner(new Color(4, []), new Color(1, []), new Color(2, []), cube)[0]])]);
    return (tupledArg) => matchesPieces(pieces, cube, tupledArg[0], tupledArg[1], tupledArg[2]);
}

export const lbSquareGodPattern = readPatterns(matchesLBSquare, "Roux", "God", "BuildLBSquare", false, false, false);

export const lfPairBeginnerPatterns = append(readPatterns((cube) => ((tupledArg) => matchesGeneric(cube, tupledArg[0], tupledArg[1], tupledArg[2])), "Roux", "Beginner", "TuckLFtoBD", false, false, false), append(readPatterns((cube_1) => ((tupledArg_1) => matchesGeneric(cube_1, tupledArg_1[0], tupledArg_1[1], tupledArg_1[2])), "Roux", "Beginner", "BringDLFtoURF", false, false, false), readPatterns((cube_2) => ((tupledArg_2) => matchesGeneric(cube_2, tupledArg_2[0], tupledArg_2[1], tupledArg_2[2])), "Roux", "Beginner", "InsertLFPair", false, false, false)));

export const lfPairIntermediatePatterns = readPatterns((cube) => ((tupledArg) => matchesGeneric(cube, tupledArg[0], tupledArg[1], tupledArg[2])), "Roux", "Intermediate", "InsertLFPair", false, false, false);

export const lfPairFirstIntermediatePatterns = readPatterns((cube) => ((tupledArg) => matchesGeneric(cube, tupledArg[0], tupledArg[1], tupledArg[2])), "Roux", "Intermediate", "InsertLFFirstPair", false, false, false);

export const lbPairLastIntermediatePatterns = readPatterns((cube) => ((tupledArg) => matchesGeneric(cube, tupledArg[0], tupledArg[1], tupledArg[2])), "Roux", "Intermediate", "InsertLBLastPair", false, false, false);

export const fbBeginnerPatterns = append(dlEdgeBeginnerPatterns, append(lCenterBeginnerPatterns, append(lbPairBeginnerPatterns, lfPairBeginnerPatterns)));

export const fbIntermediatePatterns = append(dlEdgeBeginnerPatterns, append(lCenterBeginnerPatterns, append(lbPairIntermediatePatterns, lfPairIntermediatePatterns)));

export const fbAdvancedPatterns = append(dlEdgeBeginnerPatterns, append(lCenterBeginnerPatterns, append(lbPairIntermediatePatterns, lfPairIntermediatePatterns)));

export const fbGodPatterns = append(lbSquareGodPattern, lfPairIntermediatePatterns);

export const drEdgeBeginnerPatterns = readPatterns((cube) => ((tupledArg) => matchesGeneric(cube, tupledArg[0], tupledArg[1], tupledArg[2])), "Roux", "Beginner", "DREdge", false, false, false);

export const rbPairBeginnerPatterns = append(readPatterns((cube) => ((tupledArg) => matchesGeneric(cube, tupledArg[0], tupledArg[1], tupledArg[2])), "Roux", "Beginner", "TuckRBtoFD", false, false, false), append(readPatterns((cube_1) => ((tupledArg_1) => matchesGeneric(cube_1, tupledArg_1[0], tupledArg_1[1], tupledArg_1[2])), "Roux", "Beginner", "BringDRBtoULB", false, false, false), readPatterns((cube_2) => ((tupledArg_2) => matchesGeneric(cube_2, tupledArg_2[0], tupledArg_2[1], tupledArg_2[2])), "Roux", "Beginner", "InsertRBPair", false, false, false)));

export const rbPairIntermediatePatterns = readPatterns((cube) => ((tupledArg) => matchesGeneric(cube, tupledArg[0], tupledArg[1], tupledArg[2])), "Roux", "Intermediate", "InsertRBPair", false, false, false);

export const rfPairBeginnerPatterns = ofArray([[(cube) => ((tupledArg) => matchesGeneric(cube, tupledArg[0], tupledArg[1], tupledArg[2])), "TuckRFtoBD", ["OGOO.O.....................BBBR...GGBBBR...GGW..W.WWRW", false, false, false], empty()], [(cube_1) => ((tupledArg_1) => matchesGeneric(cube_1, tupledArg_1[0], tupledArg_1[1], tupledArg_1[2])), "TuckRFtoBD", ["O.OO.O.G..R................BBBR...GGBBBR...GGW..W.WW.W", false, false, false], singleton("U2 M2")], [(cube_2) => ((tupledArg_2) => matchesGeneric(cube_2, tupledArg_2[0], tupledArg_2[1], tupledArg_2[2])), "TuckRFtoBD", ["O.OO.O.....................BBBR.RGGGBBBR...GGW..W.WW.W", false, false, false], ofArray(["R U R r2", "R U R\' M2", "R U r\' M", "R U r2 R", "R U M r\'", "R U M2 R\'", "r U R r2", "r U R\' M2", "r U r\' M", "r U r2 R", "r U M r\'", "r U M2 R\'", "F\' U2 F M\'"])], [(cube_3) => ((tupledArg_3) => matchesGeneric(cube_3, tupledArg_3[0], tupledArg_3[1], tupledArg_3[2])), "TuckRFtoBD", ["O.OO.O.R..G................BBBR...GGBBBR...GGW..W.WW.W", false, false, false], singleton("M\'")], [(cube_4) => ((tupledArg_4) => matchesGeneric(cube_4, tupledArg_4[0], tupledArg_4[1], tupledArg_4[2])), "TuckRFtoBD", ["O.OO.O..........R.....G....BBBR...GGBBBR...GGW..W.WW.W", false, false, false], singleton("M2")], [(cube_5) => ((tupledArg_5) => matchesGeneric(cube_5, tupledArg_5[0], tupledArg_5[1], tupledArg_5[2])), "TuckRFtoBD", ["OROO.O.....................BBBR...GGBBBR...GGW..W.WWGW", false, false, false], ofArray(["M U2 M2", "M2 U2 M\'"])], [(cube_6) => ((tupledArg_6) => matchesGeneric(cube_6, tupledArg_6[0], tupledArg_6[1], tupledArg_6[2])), "TuckRFtoBD", ["O.OO.O........R..........G.BBBR...GGBBBR...GGW..W.WW.W", false, false, false], singleton("U M2")], [(cube_7) => ((tupledArg_7) => matchesGeneric(cube_7, tupledArg_7[0], tupledArg_7[1], tupledArg_7[2])), "TuckRFtoBD", ["O.OO.O......G......R.......BBBR...GGBBBR...GGW..W.WW.W", false, false, false], singleton("U M\'")], [(cube_8) => ((tupledArg_8) => matchesGeneric(cube_8, tupledArg_8[0], tupledArg_8[1], tupledArg_8[2])), "TuckRFtoBD", ["O.OO.O......R......G.......BBBR...GGBBBR...GGW..W.WW.W", false, false, false], singleton("U\' M2")], [(cube_9) => ((tupledArg_9) => matchesGeneric(cube_9, tupledArg_9[0], tupledArg_9[1], tupledArg_9[2])), "TuckRFtoBD", ["O.OO.O........G..........R.BBBR...GGBBBR...GGW..W.WW.W", false, false, false], singleton("U\' M\'")], [(cube_10) => ((tupledArg_10) => matchesGeneric(cube_10, tupledArg_10[0], tupledArg_10[1], tupledArg_10[2])), "TuckRFtoBD", ["O.OO.O.....................BBBR.GRGGBBBR...GGW..W.WW.W", false, false, false], ofArray(["R U\' R\' M\'", "R U\' R2 r", "R U\' r R2", "R U\' r\' M2", "R U\' M\' R\'", "R U\' M2 r\'", "r U\' R\' M\'", "r U\' R2 r", "r U\' r R2", "r U\' r\' M2", "r U\' M\' R\'", "r U\' M2 r\'"])], [(cube_11) => ((tupledArg_11) => matchesGeneric(cube_11, tupledArg_11[0], tupledArg_11[1], tupledArg_11[2])), "TuckRFtoBD", ["O.OO.O..........G.....R....BBBR...GGBBBR...GGW..W.WW.W", false, false, false], singleton("U2 M\'")], [(cube_12) => ((tupledArg_12) => matchesGeneric(cube_12, tupledArg_12[0], tupledArg_12[1], tupledArg_12[2])), "TuckRFtoBD", ["O.OO.O.....................BBBR...GGBBBRR..GGWG.W.WW.W", false, false, false], singleton("M")], [(cube_13) => ((tupledArg_13) => matchesGeneric(cube_13, tupledArg_13[0], tupledArg_13[1], tupledArg_13[2])), "TuckRFtoBD", ["O.OO.O.....................BBBR...GGBBBRG..GGWR.W.WW.W", false, false, false], ofArray(["M\' U2 M\'", "M2 U2 M2"])], [(cube_14) => ((tupledArg_14) => matchesGeneric(cube_14, tupledArg_14[0], tupledArg_14[1], tupledArg_14[2])), "BringDRFtoULF", ["OGOO.O.........R....WG.....BBBR...GGBBBR...GGW..W.WWRW", false, false, false], empty()], [(cube_15) => ((tupledArg_15) => matchesGeneric(cube_15, tupledArg_15[0], tupledArg_15[1], tupledArg_15[2])), "BringDRFtoULF", ["OGOO.OR..G........W........BBBR...GGBBBR...GGW..W.WWRW", false, false, false], ofArray(["U2 R U2 R\'", "U2 r U2 r\'", "F\' U\' F U"])], [(cube_16) => ((tupledArg_16) => matchesGeneric(cube_16, tupledArg_16[0], tupledArg_16[1], tupledArg_16[2])), "BringDRFtoULF", ["OGOO.O..R..W..............GBBBR...GGBBBR...GGW..W.WWRW", false, false, false], singleton("R\' F R F\'")], [(cube_17) => ((tupledArg_17) => matchesGeneric(cube_17, tupledArg_17[0], tupledArg_17[1], tupledArg_17[2])), "BringDRFtoULF", ["OGOO.O.....................BBBR...GGBBBR.WRGGW.GW.WWRW", false, false, false], ofArray(["R U\' R\' U", "r U\' r\' U", "F\' U\' F U2", "F\' U2 F U\'"])], [(cube_18) => ((tupledArg_18) => matchesGeneric(cube_18, tupledArg_18[0], tupledArg_18[1], tupledArg_18[2])), "BringDRFtoULF", ["OGOO.O...........R.....WG..BBBR...GGBBBR...GGW..W.WWRW", false, false, false], singleton("U")], [(cube_19) => ((tupledArg_19) => matchesGeneric(cube_19, tupledArg_19[0], tupledArg_19[1], tupledArg_19[2])), "BringDRFtoULF", ["OGOO.O..G..R..............WBBBR...GGBBBR...GGW..W.WWRW", false, false, false], singleton("U2")], [(cube_20) => ((tupledArg_20) => matchesGeneric(cube_20, tupledArg_20[0], tupledArg_20[1], tupledArg_20[2])), "BringDRFtoULF", ["OGOO.O..W..G..............RBBBR...GGBBBR...GGW..W.WWRW", false, false, false], ofArray(["U R U2 R\'", "U r U2 r\'", "F R\' F\' R", "F\' U2 F U"])], [(cube_21) => ((tupledArg_21) => matchesGeneric(cube_21, tupledArg_21[0], tupledArg_21[1], tupledArg_21[2])), "BringDRFtoULF", ["OGOO.O.........G....RW.....BBBR...GGBBBR...GGW..W.WWRW", false, false, false], ofArray(["U\' R U2 R\'", "U\' r U2 r\'"])], [(cube_22) => ((tupledArg_22) => matchesGeneric(cube_22, tupledArg_22[0], tupledArg_22[1], tupledArg_22[2])), "BringDRFtoULF", ["OGOO.O.....................BBBR...GGBBBR.RGGGW.WW.WWRW", false, false, false], ofArray(["R U R\'", "r U r\'"])], [(cube_23) => ((tupledArg_23) => matchesGeneric(cube_23, tupledArg_23[0], tupledArg_23[1], tupledArg_23[2])), "BringDRFtoULF", ["OGOO.OW..R........G........BBBR...GGBBBR...GGW..W.WWRW", false, false, false], singleton("U\'")], [(cube_24) => ((tupledArg_24) => matchesGeneric(cube_24, tupledArg_24[0], tupledArg_24[1], tupledArg_24[2])), "BringDRFtoULF", ["OGOO.O.........W....GR.....BBBR...GGBBBR...GGW..W.WWRW", false, false, false], ofArray(["R U2 R\' U", "r U2 r\' U"])], [(cube_25) => ((tupledArg_25) => matchesGeneric(cube_25, tupledArg_25[0], tupledArg_25[1], tupledArg_25[2])), "BringDRFtoULF", ["OGOO.O...........W.....GR..BBBR...GGBBBR...GGW..W.WWRW", false, false, false], ofArray(["F\' U F U\'", "F\' U2 F U2"])], [(cube_26) => ((tupledArg_26) => matchesGeneric(cube_26, tupledArg_26[0], tupledArg_26[1], tupledArg_26[2])), "BringDRFtoULF", ["OGOO.OG..W........R........BBBR...GGBBBR...GGW..W.WWRW", false, false, false], ofArray(["R U R\' U", "r U r\' U"])], [(cube_27) => ((tupledArg_27) => matchesGeneric(cube_27, tupledArg_27[0], tupledArg_27[1], tupledArg_27[2])), "BringDRFtoULF", ["OGOO.O.....................BBBR...GGBBBR.GWGGW.RW.WWRW", false, false, false], ofArray(["R U\' B U\' B\' R\'", "R B U2 B\' R\' U", "R2 B\' R\' B U\' R\'", "R2 B\' R\' B R\' U\'", "r U\' B U\' B\' r\'", "r B U2 B\' r\' U", "r2 B r2 B\' U2 B", "F U2 F U2 F\' U2", "F R\' F\' R2 U2 R\'", "F\' U F R U2 R\'", "F\' U F r U2 r\'", "B r2 B r2 B\' U2"])], [(cube_28) => ((tupledArg_28) => matchesGeneric(cube_28, tupledArg_28[0], tupledArg_28[1], tupledArg_28[2])), "BringDRFtoULF", ["OGOO.O...........G.....RW..BBBR...GGBBBR...GGW..W.WWRW", false, false, false], ofArray(["R U2 R\'", "r U2 r\'"])], [(cube_29) => ((tupledArg_29) => matchesGeneric(cube_29, tupledArg_29[0], tupledArg_29[1], tupledArg_29[2])), "InsertRFPair", ["OGOO.O.........R....WG.....BBBR...GGBBBR...GGW..W.WWRW", false, false, false], ofArray(["R M2 U\' R\'", "R M2 U\' r\'", "R\' r2 U\' R\'", "R\' r2 U\' r\'", "r M\' U\' R\'", "r M\' U\' r\'", "r2 R\' U\' R\'", "r2 R\' U\' r\'", "M\' r U\' R\'", "M\' r U\' r\'", "M2 R U\' R\'", "M2 R U\' r\'"])]]);

export const rfPairIntermediatePatterns = readPatterns((cube) => ((tupledArg) => matchesGeneric(cube, tupledArg[0], tupledArg[1], tupledArg[2])), "Roux", "Intermediate", "InsertRFPair", false, false, false);

export const rfPairFirstIntermediatePatterns = readPatterns((cube) => ((tupledArg) => matchesGeneric(cube, tupledArg[0], tupledArg[1], tupledArg[2])), "Roux", "Intermediate", "InsertRFFirstPair", false, false, false);

export const rbPairLastIntermediatePatterns = readPatterns((cube) => ((tupledArg) => matchesGeneric(cube, tupledArg[0], tupledArg[1], tupledArg[2])), "Roux", "Intermediate", "InsertRBLastPair", false, false, false);

export const sbBeginnerPatterns = append(drEdgeBeginnerPatterns, append(rbPairBeginnerPatterns, rfPairBeginnerPatterns));

export const centerOrientationPatterns = ofArray([[(cube) => ((tupledArg) => matchesGeneric(cube, tupledArg[0], tupledArg[1], tupledArg[2])), "CenterOrientation", ["O.OO.O.......E.............BBBR.RGGGBBBR.RGGGW.WWEWW.W", true, true, false], empty()], [(cube_1) => ((tupledArg_1) => matchesGeneric(cube_1, tupledArg_1[0], tupledArg_1[1], tupledArg_1[2])), "CenterOrientation", ["O.OO.O.......P.............BBBR.RGGGBBBR.RGGGW.WWPWW.W", true, true, false], ofArray(["M\'", "M"])]]);

export const sbIntermediatePatterns = append(centerOrientationPatterns, append(drEdgeBeginnerPatterns, append(rbPairIntermediatePatterns, rfPairIntermediatePatterns)));

export const coBeginnerPatterns = ofArray([[(cube) => ((tupledArg) => matchesGeneric(cube, tupledArg[0], tupledArg[1], tupledArg[2])), "CornerOrientation", ["O.OO.O...Y.Y...Y.Y.........BBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, false], empty()], [(cube_1) => ((tupledArg_1) => matchesGeneric(cube_1, tupledArg_1[0], tupledArg_1[1], tupledArg_1[2])), "CornerOrientation", ["O.OO.OY.Y............Y.Y...BBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true], singleton((((((sune + " U2 ") + sune) + " ") + sune) + " U2 ") + sune)], [(cube_2) => ((tupledArg_2) => matchesGeneric(cube_2, tupledArg_2[0], tupledArg_2[1], tupledArg_2[2])), "CornerOrientation", ["O.OO.OY..............Y..Y.YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true], singleton((((((sune + " U2 ") + sune) + " U\' ") + sune) + " U2 ") + sune)], [(cube_3) => ((tupledArg_3) => matchesGeneric(cube_3, tupledArg_3[0], tupledArg_3[1], tupledArg_3[2])), "CornerOrientation", ["O.OO.O...Y.Y.........Y.Y...BBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true], singleton((((sune + " U\' ") + sune) + " U2 ") + sune)], [(cube_4) => ((tupledArg_4) => matchesGeneric(cube_4, tupledArg_4[0], tupledArg_4[1], tupledArg_4[2])), "CornerOrientation", ["O.OO.OY....Y.....Y...Y.....BBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true], singleton((((sune + " U ") + sune) + " U2 ") + sune)], [(cube_5) => ((tupledArg_5) => matchesGeneric(cube_5, tupledArg_5[0], tupledArg_5[1], tupledArg_5[2])), "CornerOrientation", ["O.OO.OY........Y.......Y..YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true], singleton(sune)], [(cube_6) => ((tupledArg_6) => matchesGeneric(cube_6, tupledArg_6[0], tupledArg_6[1], tupledArg_6[2])), "CornerOrientation", ["O.OO.O..Y......Y..Y.....Y..BBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true], singleton((sune + " U2 ") + sune)], [(cube_7) => ((tupledArg_7) => matchesGeneric(cube_7, tupledArg_7[0], tupledArg_7[1], tupledArg_7[2])), "CornerOrientation", ["O.OO.O...Y.......Y...Y....YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true], singleton((((sune + " ") + sune) + " U2 ") + sune)]]);

export const coIntermediatePatterns = ofArray([[(cube) => ((tupledArg) => matchesGeneric(cube, tupledArg[0], tupledArg[1], tupledArg[2])), "CornerOrientation", ["O.OO.O...Y.Y...Y.Y.........BBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, false], empty()], [(cube_1) => ((tupledArg_1) => matchesGeneric(cube_1, tupledArg_1[0], tupledArg_1[1], tupledArg_1[2])), "CornerOrientation", ["O.OO.OY.Y............Y.Y...BBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true], singleton(("R U2 R\' U\' " + sexy) + " R U\' R\'")], [(cube_2) => ((tupledArg_2) => matchesGeneric(cube_2, tupledArg_2[0], tupledArg_2[1], tupledArg_2[2])), "CornerOrientation", ["O.OO.O..Y.........Y.Y..Y...BBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true], singleton(((("F " + sexy) + " ") + sexy) + " F\'")], [(cube_3) => ((tupledArg_3) => matchesGeneric(cube_3, tupledArg_3[0], tupledArg_3[1], tupledArg_3[2])), "CornerOrientation", ["O.OO.O.....Y.....YY.Y......BBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true], singleton(("F " + sexy) + " F\'")], [(cube_4) => ((tupledArg_4) => matchesGeneric(cube_4, tupledArg_4[0], tupledArg_4[1], tupledArg_4[2])), "CornerOrientation", ["O.OO.OY....Y.....Y...Y.....BBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true], singleton((sexy + " ") + sledge)], [(cube_5) => ((tupledArg_5) => matchesGeneric(cube_5, tupledArg_5[0], tupledArg_5[1], tupledArg_5[2])), "CornerOrientation", ["O.OO.OY........Y.......Y..YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true], singleton(sune)], [(cube_6) => ((tupledArg_6) => matchesGeneric(cube_6, tupledArg_6[0], tupledArg_6[1], tupledArg_6[2])), "CornerOrientation", ["O.OO.O..Y........YY..Y.....BBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true], singleton("L\' U\' L U\' L\' U2 L")], [(cube_7) => ((tupledArg_7) => matchesGeneric(cube_7, tupledArg_7[0], tupledArg_7[1], tupledArg_7[2])), "CornerOrientation", ["O.OO.O...Y.......Y...Y....YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true], singleton("F R\' F\' R U R U\' R\'")]]);

export const cpBeginnerPatterns = ofArray([[(cube) => ((tupledArg) => matchesGeneric(cube, tupledArg[0], tupledArg[1], tupledArg[2])), "CornerPermutation", ["O.OO.OO.OY.Y...Y.YB.BR.RG.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, false], empty()], [(cube_1) => ((tupledArg_1) => matchesGeneric(cube_1, tupledArg_1[0], tupledArg_1[1], tupledArg_1[2])), "CornerPermutation", ["O.OO.OG.RY.Y...Y.YO.OB.RG.BBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true], singleton(jperm)], [(cube_2) => ((tupledArg_2) => matchesGeneric(cube_2, tupledArg_2[0], tupledArg_2[1], tupledArg_2[2])), "CornerPermutation", ["O.OO.OR.OY.Y...Y.YG.BR.OB.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, false], singleton((jperm + " U ") + jperm)]]);

export const cpIntermediatePatterns = ofArray([[(cube) => ((tupledArg) => matchesGeneric(cube, tupledArg[0], tupledArg[1], tupledArg[2])), "CornerPermutation", ["O.OO.OO.OY.Y...Y.YB.BR.RG.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, false], empty()], [(cube_1) => ((tupledArg_1) => matchesGeneric(cube_1, tupledArg_1[0], tupledArg_1[1], tupledArg_1[2])), "CornerPermutation", ["O.OO.OG.RY.Y...Y.YO.OB.RG.BBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true], singleton(jperm)], [(cube_2) => ((tupledArg_2) => matchesGeneric(cube_2, tupledArg_2[0], tupledArg_2[1], tupledArg_2[2])), "CornerPermutation", ["O.OO.OR.OY.Y...Y.YG.BR.OB.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, false], singleton(diagSwap)]]);

export const cmllBeginnerPatterns = append(coBeginnerPatterns, cpBeginnerPatterns);

export const cmllIntermediatePatterns = append(coIntermediatePatterns, cpIntermediatePatterns);

export const cmllAdvancedPatterns = ofArray([[(cube) => ((tupledArg) => matchesGeneric(cube, tupledArg[0], tupledArg[1], tupledArg[2])), "CornerOrientation", ["O.OO.OO.OY.Y...Y.YB.BR.RG.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, false], empty()], [(cube_1) => ((tupledArg_1) => matchesGeneric(cube_1, tupledArg_1[0], tupledArg_1[1], tupledArg_1[2])), "CornerOrientation", ["O.OO.OG.RY.Y...Y.YO.OB.RG.BBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true], singleton(jperm)], [(cube_2) => ((tupledArg_2) => matchesGeneric(cube_2, tupledArg_2[0], tupledArg_2[1], tupledArg_2[2])), "CornerOrientation", ["O.OO.OR.OY.Y...Y.YG.BR.OB.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, false], singleton(diagSwap)], [(cube_3) => ((tupledArg_3) => matchesGeneric(cube_3, tupledArg_3[0], tupledArg_3[1], tupledArg_3[2])), "CornerOrientation", ["O.OO.OY.YO.O...R.RG.GY.YB.BBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true], singleton(((((("F " + sexy) + " ") + sexy) + " ") + sexy) + " F\'")], [(cube_4) => ((tupledArg_4) => matchesGeneric(cube_4, tupledArg_4[0], tupledArg_4[1], tupledArg_4[2])), "CornerOrientation", ["O.OO.OR.OB.B...G.GY.YR.OY.YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true], singleton("R U R\' U R U\' R\' U R U2 R\'")], [(cube_5) => ((tupledArg_5) => matchesGeneric(cube_5, tupledArg_5[0], tupledArg_5[1], tupledArg_5[2])), "CornerOrientation", ["O.OO.OR.RB.G...O.OY.YG.BY.YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true], singleton("R U2\' R2\' F R F\' U2 R\' F R F\'")], [(cube_6) => ((tupledArg_6) => matchesGeneric(cube_6, tupledArg_6[0], tupledArg_6[1], tupledArg_6[2])), "CornerOrientation", ["O.OO.OY.YB.B...R.OO.GY.YG.RBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true], singleton("r U\' r2\' D\' r U\' r\' D r2 U r\'")], [(cube_7) => ((tupledArg_7) => matchesGeneric(cube_7, tupledArg_7[0], tupledArg_7[1], tupledArg_7[2])), "CornerOrientation", ["O.OO.OB.YO.G...R.GY.YB.YR.OBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true], singleton("F R U R\' U\' R U R\' U\' F\'")], [(cube_8) => ((tupledArg_8) => matchesGeneric(cube_8, tupledArg_8[0], tupledArg_8[1], tupledArg_8[2])), "CornerOrientation", ["O.OO.OB.BO.R...G.GY.OY.YR.YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true], singleton("R\' U\' R\' F R F\' R U\' R\' U2 R")], [(cube_9) => ((tupledArg_9) => matchesGeneric(cube_9, tupledArg_9[0], tupledArg_9[1], tupledArg_9[2])), "CornerOrientation", ["O.OO.OY.YG.O...O.BR.YG.RY.BBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true], singleton("F R\' F\' R U2 R U\' R\' U R U2\' R\'")], [(cube_10) => ((tupledArg_10) => matchesGeneric(cube_10, tupledArg_10[0], tupledArg_10[1], tupledArg_10[2])), "CornerOrientation", ["O.OO.OG.YR.B...B.OY.YO.YG.RBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true], singleton("R U2 R\' U\' R U R\' U2\' R\' F R F\'")], [(cube_11) => ((tupledArg_11) => matchesGeneric(cube_11, tupledArg_11[0], tupledArg_11[1], tupledArg_11[2])), "CornerOrientation", ["O.OO.OB.BO.R...R.OY.GY.YG.YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true], singleton("R\' F R U F U\' R U R\' U\' F\'")], [(cube_12) => ((tupledArg_12) => matchesGeneric(cube_12, tupledArg_12[0], tupledArg_12[1], tupledArg_12[2])), "CornerOrientation", ["O.OO.OG.BR.R...O.OY.BY.YG.YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true], singleton("r U\' r2\' D\' r U r\' D r2 U r\'")], [(cube_13) => ((tupledArg_13) => matchesGeneric(cube_13, tupledArg_13[0], tupledArg_13[1], tupledArg_13[2])), "CornerOrientation", ["O.OO.OG.OR.Y...R.YY.YB.OB.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true], singleton("F R U R\' U\' F\'")], [(cube_14) => ((tupledArg_14) => matchesGeneric(cube_14, tupledArg_14[0], tupledArg_14[1], tupledArg_14[2])), "CornerOrientation", ["O.OO.OO.GY.Y...G.RB.OY.YB.RBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true], singleton("R2 D R\' U2 R D\' R\' U2 R\'")], [(cube_15) => ((tupledArg_15) => matchesGeneric(cube_15, tupledArg_15[0], tupledArg_15[1], tupledArg_15[2])), "CornerOrientation", ["O.OO.OY.YR.G...Y.YB.OB.RG.OBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true], singleton("R2\' D\' R U2 R\' D R U2 R")], [(cube_16) => ((tupledArg_16) => matchesGeneric(cube_16, tupledArg_16[0], tupledArg_16[1], tupledArg_16[2])), "CornerOrientation", ["O.OO.OY.YG.B...Y.YR.GO.OB.RBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true], singleton("R2\' F U\' F U F2 R2 U\' R\' F R")], [(cube_17) => ((tupledArg_17) => matchesGeneric(cube_17, tupledArg_17[0], tupledArg_17[1], tupledArg_17[2])), "CornerOrientation", ["O.OO.OG.BR.Y...R.YY.YB.GO.OBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true], singleton("F R2 D R\' U R D\' R2\' U\' F\'")], [(cube_18) => ((tupledArg_18) => matchesGeneric(cube_18, tupledArg_18[0], tupledArg_18[1], tupledArg_18[2])), "CornerOrientation", ["O.OO.OG.BY.Y...B.GO.RY.YR.OBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true], singleton("r U\' r\' U r\' D\' r U\' r\' D r")], [(cube_19) => ((tupledArg_19) => matchesGeneric(cube_19, tupledArg_19[0], tupledArg_19[1], tupledArg_19[2])), "CornerOrientation", ["O.OO.OY.BR.Y...G.YB.OY.RG.OBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true], singleton("R U R\' U\' R\' F R F\'")], [(cube_20) => ((tupledArg_20) => matchesGeneric(cube_20, tupledArg_20[0], tupledArg_20[1], tupledArg_20[2])), "CornerOrientation", ["O.OO.OG.YY.R...Y.BO.BR.YO.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true], singleton("L\' U\' L U L F\' L\' F")], [(cube_21) => ((tupledArg_21) => matchesGeneric(cube_21, tupledArg_21[0], tupledArg_21[1], tupledArg_21[2])), "CornerOrientation", ["O.OO.OG.BR.R...Y.YY.GO.OB.YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true], singleton("F R\' F R2 U\' R\' U\' R U R\' F2")], [(cube_22) => ((tupledArg_22) => matchesGeneric(cube_22, tupledArg_22[0], tupledArg_22[1], tupledArg_22[2])), "CornerOrientation", ["O.OO.OR.RB.G...Y.YY.GO.OB.YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true], singleton("r\' U r U2\' R2\' F R F\' R")], [(cube_23) => ((tupledArg_23) => matchesGeneric(cube_23, tupledArg_23[0], tupledArg_23[1], tupledArg_23[2])), "CornerOrientation", ["O.OO.OR.OB.B...Y.YY.GO.RG.YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true], singleton("r\' D\' r U r\' D r U\' r U r\'")], [(cube_24) => ((tupledArg_24) => matchesGeneric(cube_24, tupledArg_24[0], tupledArg_24[1], tupledArg_24[2])), "CornerOrientation", ["O.OO.OG.BY.Y...G.BO.YR.RY.OBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true], singleton("r2\' D\' r U r\' D r2 U\' r\' U\' r")], [(cube_25) => ((tupledArg_25) => matchesGeneric(cube_25, tupledArg_25[0], tupledArg_25[1], tupledArg_25[2])), "CornerOrientation", ["O.OO.OY.GB.O...Y.GO.BR.YR.YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true], singleton(sune)], [(cube_26) => ((tupledArg_26) => matchesGeneric(cube_26, tupledArg_26[0], tupledArg_26[1], tupledArg_26[2])), "CornerOrientation", ["O.OO.OY.GG.O...Y.RR.OB.YB.YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true], singleton("L\' U2 L U2\' L F\' L\' F")], [(cube_27) => ((tupledArg_27) => matchesGeneric(cube_27, tupledArg_27[0], tupledArg_27[1], tupledArg_27[2])), "CornerOrientation", ["O.OO.OY.GR.O...Y.BB.RG.YO.YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true], singleton("F R\' F\' R U2 R U2\' R\'")], [(cube_28) => ((tupledArg_28) => matchesGeneric(cube_28, tupledArg_28[0], tupledArg_28[1], tupledArg_28[2])), "CornerOrientation", ["O.OO.OY.GB.O...Y.RO.RG.YB.YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true], singleton("R U R\' U\' R\' F R F\' R U R\' U R U2 R\'")], [(cube_29) => ((tupledArg_29) => matchesGeneric(cube_29, tupledArg_29[0], tupledArg_29[1], tupledArg_29[2])), "CornerOrientation", ["O.OO.OY.BO.Y...G.RG.YR.YB.OBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true], singleton("R U R\' U R\' F R F\' R U2\' R\'")], [(cube_30) => ((tupledArg_30) => matchesGeneric(cube_30, tupledArg_30[0], tupledArg_30[1], tupledArg_30[2])), "CornerOrientation", ["O.OO.OY.BO.R...Y.BG.RG.YO.YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true], singleton("R U\' L\' U R\' U\' L")], [(cube_31) => ((tupledArg_31) => matchesGeneric(cube_31, tupledArg_31[0], tupledArg_31[1], tupledArg_31[2])), "CornerOrientation", ["O.OO.OR.YB.O...R.YY.GY.GO.BBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true], singleton("L\' U\' L U\' L\' U2 L")], [(cube_32) => ((tupledArg_32) => matchesGeneric(cube_32, tupledArg_32[0], tupledArg_32[1], tupledArg_32[2])), "CornerOrientation", ["O.OO.OB.YO.G...R.YY.GY.BR.OBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true], singleton("R2 D R\' U R D\' R\' U R\' U\' R U\' R\'")], [(cube_33) => ((tupledArg_33) => matchesGeneric(cube_33, tupledArg_33[0], tupledArg_33[1], tupledArg_33[2])), "CornerOrientation", ["O.OO.OR.YB.G...O.YY.BY.RG.OBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true], singleton("F\' L F L\' U2\' L\' U2 L")], [(cube_34) => ((tupledArg_34) => matchesGeneric(cube_34, tupledArg_34[0], tupledArg_34[1], tupledArg_34[2])), "CornerOrientation", ["O.OO.OG.YR.G...O.YY.BY.BR.OBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true], singleton("R U2\' R\' U2 R\' F R F\'")], [(cube_35) => ((tupledArg_35) => matchesGeneric(cube_35, tupledArg_35[0], tupledArg_35[1], tupledArg_35[2])), "CornerOrientation", ["O.OO.OR.YB.G...R.YY.GY.OB.OBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true], singleton("L\' U R U\' L U R\'")], [(cube_36) => ((tupledArg_36) => matchesGeneric(cube_36, tupledArg_36[0], tupledArg_36[1], tupledArg_36[2])), "CornerOrientation", ["O.OO.OB.YO.B...G.YY.OY.RG.RBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true], singleton("R\' U\' R U\' L U\' R\' U L\' U2 R")], [(cube_37) => ((tupledArg_37) => matchesGeneric(cube_37, tupledArg_37[0], tupledArg_37[1], tupledArg_37[2])), "CornerOrientation", ["O.OO.OO.YY.B...G.YB.YR.GO.RBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true], singleton("R2\' D\' R U\' R\' D R U R")], [(cube_38) => ((tupledArg_38) => matchesGeneric(cube_38, tupledArg_38[0], tupledArg_38[1], tupledArg_38[2])), "CornerOrientation", ["O.OO.OO.RY.G...B.YB.RY.GO.YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true], singleton("F R\' F\' R U R U\' R\'")], [(cube_39) => ((tupledArg_39) => matchesGeneric(cube_39, tupledArg_39[0], tupledArg_39[1], tupledArg_39[2])), "CornerOrientation", ["O.OO.OR.YY.B...O.YG.YG.OB.RBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true], singleton("R U2 R\' U\' R U R\' U\' R U R\' U\' R U\' R\'")], [(cube_40) => ((tupledArg_40) => matchesGeneric(cube_40, tupledArg_40[0], tupledArg_40[1], tupledArg_40[2])), "CornerOrientation", ["O.OO.OO.BY.R...R.YB.GY.GO.YBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true], singleton("R U2 R D R\' U2 R D\' R2\'")], [(cube_41) => ((tupledArg_41) => matchesGeneric(cube_41, tupledArg_41[0], tupledArg_41[1], tupledArg_41[2])), "CornerOrientation", ["O.OO.OG.YY.O...G.YO.YR.BR.BBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true], singleton("R\' U\' R U R\' F\' R U R\' U\' R\' F R2")], [(cube_42) => ((tupledArg_42) => matchesGeneric(cube_42, tupledArg_42[0], tupledArg_42[1], tupledArg_42[2])), "CornerOrientation", ["O.OO.OR.YY.O...O.YG.YG.BR.BBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, true], singleton("U R\' U2 R\' D\' R U2 R\' D R2")], [(cube_43) => ((tupledArg_43) => matchesGeneric(cube_43, tupledArg_43[0], tupledArg_43[1], tupledArg_43[2])), "CornerPermutation", ["O.OO.OO.OY.Y...Y.YB.BR.RG.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, true, false], empty()]]);

export const edgeBeginnerOrientationPatters = ofArray([[(cube) => ((tupledArg) => matchesGeneric(cube, tupledArg[0], tupledArg[1], tupledArg[2])), "EdgeOrientation", ["O.OO.OO.OYEYEEEYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWEW", true, true, false], empty()], [(cube_1) => ((tupledArg_1) => matchesGeneric(cube_1, tupledArg_1[0], tupledArg_1[1], tupledArg_1[2])), "EdgeOrientation", ["O.OO.OO.OYPYEEEYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWEW", true, true, true], singleton((((mum + " ") + mum) + " U\' ") + mum)], [(cube_2) => ((tupledArg_2) => matchesGeneric(cube_2, tupledArg_2[0], tupledArg_2[1], tupledArg_2[2])), "EdgeOrientation", ["O.OO.OO.OYEYEEEYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWPW", true, true, false], singleton((((("M2 " + mum) + " ") + mum) + " U\' ") + mum)], [(cube_3) => ((tupledArg_3) => matchesGeneric(cube_3, tupledArg_3[0], tupledArg_3[1], tupledArg_3[2])), "EdgeOrientation", ["O.OO.OO.OYPYEEPYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWEW", true, true, true], singleton((mum + " U2 ") + mum)], [(cube_4) => ((tupledArg_4) => matchesGeneric(cube_4, tupledArg_4[0], tupledArg_4[1], tupledArg_4[2])), "EdgeOrientation", ["O.OO.OO.OYPYEEEYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWPW", true, true, true], singleton((((((((mum + " ") + mum) + " U2 ") + mum) + " U ") + mum) + " U\' ") + mum)], [(cube_5) => ((tupledArg_5) => matchesGeneric(cube_5, tupledArg_5[0], tupledArg_5[1], tupledArg_5[2])), "EdgeOrientation", ["O.OO.OO.OYPYEEEYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWEW", true, true, true], singleton((((mum + " U ") + mum) + " U\' ") + mum)], [(cube_6) => ((tupledArg_6) => matchesGeneric(cube_6, tupledArg_6[0], tupledArg_6[1], tupledArg_6[2])), "EdgeOrientation", ["O.OO.OO.OYPYPEPYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWEW", true, true, false], singleton((((((mum + " U\' ") + mum) + " U ") + mum) + " U\' ") + mum)], [(cube_7) => ((tupledArg_7) => matchesGeneric(cube_7, tupledArg_7[0], tupledArg_7[1], tupledArg_7[2])), "EdgeOrientation", ["O.OO.OO.OYEYPEPYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWEW", true, true, true], singleton(mum)], [(cube_8) => ((tupledArg_8) => matchesGeneric(cube_8, tupledArg_8[0], tupledArg_8[1], tupledArg_8[2])), "EdgeOrientation", ["O.OO.OO.OYEYPEPYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWPW", true, true, true], singleton((((mum + " U\' ") + mum) + " U2 ") + mum)], [(cube_9) => ((tupledArg_9) => matchesGeneric(cube_9, tupledArg_9[0], tupledArg_9[1], tupledArg_9[2])), "EdgeOrientation", ["O.OO.OO.OYEYPEPYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWPW", true, true, true], singleton((((mum + " U2 ") + mum) + " U2 ") + mum)], [(cube_10) => ((tupledArg_10) => matchesGeneric(cube_10, tupledArg_10[0], tupledArg_10[1], tupledArg_10[2])), "EdgeOrientation", ["O.OO.OO.OYPYEEPYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWPW", true, true, true], singleton((mum + " U\' ") + mum)], [(cube_11) => ((tupledArg_11) => matchesGeneric(cube_11, tupledArg_11[0], tupledArg_11[1], tupledArg_11[2])), "EdgeOrientation", ["O.OO.OO.OYPYPEPYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWPW", true, true, false], singleton((((((mum + " U2 ") + mum) + " U ") + mum) + " U\' ") + mum)]]);

export const edgeIntermediateOrientationPatters = ofArray([[(cube) => ((tupledArg) => matchesGeneric(cube, tupledArg[0], tupledArg[1], tupledArg[2])), "EdgeOrientation", ["O.OO.OO.OYEYEEEYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWEW", true, true, false], empty()], [(cube_1) => ((tupledArg_1) => matchesGeneric(cube_1, tupledArg_1[0], tupledArg_1[1], tupledArg_1[2])), "EdgeOrientation", ["O.OO.OO.OYPYEEEYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWEW", true, true, true], singleton("M\' U M U\' " + mum)], [(cube_2) => ((tupledArg_2) => matchesGeneric(cube_2, tupledArg_2[0], tupledArg_2[1], tupledArg_2[2])), "EdgeOrientation", ["O.OO.OO.OYEYEEEYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWPW", true, true, false], singleton(mum + " U M U\' M\'")], [(cube_3) => ((tupledArg_3) => matchesGeneric(cube_3, tupledArg_3[0], tupledArg_3[1], tupledArg_3[2])), "EdgeOrientation", ["O.OO.OO.OYEYEEPYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWEW", true, true, false], singleton("M U M\' U2 M U\' M\'")], [(cube_4) => ((tupledArg_4) => matchesGeneric(cube_4, tupledArg_4[0], tupledArg_4[1], tupledArg_4[2])), "EdgeOrientation", ["O.OO.OO.OYEYPEEYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWEW", true, true, false], singleton("M U\' M\' U2 M U\' M\'")], [(cube_5) => ((tupledArg_5) => matchesGeneric(cube_5, tupledArg_5[0], tupledArg_5[1], tupledArg_5[2])), "EdgeOrientation", ["O.OO.OO.OYPYPEEYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWEW", true, true, false], singleton("M\' U M\' U2 " + mum)], [(cube_6) => ((tupledArg_6) => matchesGeneric(cube_6, tupledArg_6[0], tupledArg_6[1], tupledArg_6[2])), "EdgeOrientation", ["O.OO.OO.OYPYEEPYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWEW", true, true, false], singleton("M\' U\' M U2 " + mum)], [(cube_7) => ((tupledArg_7) => matchesGeneric(cube_7, tupledArg_7[0], tupledArg_7[1], tupledArg_7[2])), "EdgeOrientation", ["O.OO.OO.OYEYEEEYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWPW", true, true, true], singleton(mum + " U\' M U\' M\'")], [(cube_8) => ((tupledArg_8) => matchesGeneric(cube_8, tupledArg_8[0], tupledArg_8[1], tupledArg_8[2])), "EdgeOrientation", ["O.OO.OO.OYPYEEEYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWEW", true, true, true], singleton("M\' U\' M U\' " + mum)], [(cube_9) => ((tupledArg_9) => matchesGeneric(cube_9, tupledArg_9[0], tupledArg_9[1], tupledArg_9[2])), "EdgeOrientation", ["O.OO.OO.OYPYPEPYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWEW", true, true, false], singleton("M\' U2 M\' U2 " + mum)], [(cube_10) => ((tupledArg_10) => matchesGeneric(cube_10, tupledArg_10[0], tupledArg_10[1], tupledArg_10[2])), "EdgeOrientation", ["O.OO.OO.OYPYPEPYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWEW", true, true, false], singleton((((((mum + " U\' ") + mum) + " U ") + mum) + " U\' ") + mum)], [(cube_11) => ((tupledArg_11) => matchesGeneric(cube_11, tupledArg_11[0], tupledArg_11[1], tupledArg_11[2])), "EdgeOrientation", ["O.OO.OO.OYEYPEPYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWEW", true, true, true], singleton(mum)], [(cube_12) => ((tupledArg_12) => matchesGeneric(cube_12, tupledArg_12[0], tupledArg_12[1], tupledArg_12[2])), "EdgeOrientation", ["O.OO.OO.OYPYPEPYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWEWWEWWPW", true, true, true], singleton("M U\' M\'")], [(cube_13) => ((tupledArg_13) => matchesGeneric(cube_13, tupledArg_13[0], tupledArg_13[1], tupledArg_13[2])), "EdgeOrientation", ["O.OO.OO.OYEYPEPYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWPW", true, true, true], singleton("M\' U2 M\' U2 M U\' M\'")], [(cube_14) => ((tupledArg_14) => matchesGeneric(cube_14, tupledArg_14[0], tupledArg_14[1], tupledArg_14[2])), "EdgeOrientation", ["O.OO.OO.OYEYEEPYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWPW", true, true, false], singleton("M2 U\' M U\' M\'")], [(cube_15) => ((tupledArg_15) => matchesGeneric(cube_15, tupledArg_15[0], tupledArg_15[1], tupledArg_15[2])), "EdgeOrientation", ["O.OO.OO.OYEYPEEYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWPW", true, true, false], singleton("M2 U M U\' M\'")], [(cube_16) => ((tupledArg_16) => matchesGeneric(cube_16, tupledArg_16[0], tupledArg_16[1], tupledArg_16[2])), "EdgeOrientation", ["O.OO.OO.OYPYPEEYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWPW", true, true, false], singleton("M2 U\' " + mum)], [(cube_17) => ((tupledArg_17) => matchesGeneric(cube_17, tupledArg_17[0], tupledArg_17[1], tupledArg_17[2])), "EdgeOrientation", ["O.OO.OO.OYPYEEPYEYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWPW", true, true, false], singleton("M2 U " + mum)], [(cube_18) => ((tupledArg_18) => matchesGeneric(cube_18, tupledArg_18[0], tupledArg_18[1], tupledArg_18[2])), "EdgeOrientation", ["O.OO.OO.OYPYPEPYPYB.BR.RG.GBBBR.RGGGBBBR.RGGGWPWWEWWPW", true, true, false], singleton("R U\' r\' U\' M\' U r U r\'")]]);

export const eoBeginnerPatterns = append(centerOrientationPatterns, edgeBeginnerOrientationPatters);

export const eoIntermediatePatterns = edgeIntermediateOrientationPatters;

export const lrBeginnerPatterns = ofArray([[(cube) => ((tupledArg) => matchesGeneric(cube, tupledArg[0], tupledArg[1], tupledArg[2])), "LToDF", ["OPOOPOOPOYEYEEEYEYBPBRPRGPGBBBRPRGGGBBBRBRGGGWEWWEWWEW", true, true, false], empty()], [(cube_1) => ((tupledArg_1) => matchesGeneric(cube_1, tupledArg_1[0], tupledArg_1[1], tupledArg_1[2])), "LToDF", ["OPOOPOOBOYEYEEEYEYBPBRPRGPGBBBRPRGGGBBBRPRGGGWEWWEWWEW", true, true, true], singleton("M2")], [(cube_2) => ((tupledArg_2) => matchesGeneric(cube_2, tupledArg_2[0], tupledArg_2[1], tupledArg_2[2])), "LToDF", ["OBOOPOOPOYEYEEEYEYBPBRPRGPGBBBRPRGGGBBBRPRGGGWEWWEWWEW", true, true, false], ofArray(["M U2 M", "M2 U2 M2"])], [(cube_3) => ((tupledArg_3) => matchesGeneric(cube_3, tupledArg_3[0], tupledArg_3[1], tupledArg_3[2])), "LREdgesBottom", ["OGOO.OOPOYEYEEEYEYBPBR.RGPGBBBRPRGGGBBBRBRGGGWEWWEWWEW", true, true, false], empty()], [(cube_4) => ((tupledArg_4) => matchesGeneric(cube_4, tupledArg_4[0], tupledArg_4[1], tupledArg_4[2])), "LREdgesBottom", ["OPOO.OOGOYEYEEEYEYBPBR.RGPGBBBRPRGGGBBBRBRGGGWEWWEWWEW", true, true, true], singleton("M U2 M\'")], [(cube_5) => ((tupledArg_5) => matchesGeneric(cube_5, tupledArg_5[0], tupledArg_5[1], tupledArg_5[2])), "LREdges", ["OGOO.OBPBYEYEEEYEYRPRG.GOPOBBBRPRGGGBBBRBRGGGWEWWEWWEW", false, false, true], singleton("M2 U\'")]]);

export const lrIntermediatePatterns = ofArray([[(cube) => ((tupledArg) => matchesGeneric(cube, tupledArg[0], tupledArg[1], tupledArg[2])), "LREdges", ["O.OO.OO.OY.Y...Y.YBBBR.RGGGBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true], empty()], [(cube_1) => ((tupledArg_1) => matchesGeneric(cube_1, tupledArg_1[0], tupledArg_1[1], tupledArg_1[2])), "LREdges", ["OGOO.OG.GY.Y...Y.YO.OBBBR.RBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true], singleton("M\' U2 M\' U\'")], [(cube_2) => ((tupledArg_2) => matchesGeneric(cube_2, tupledArg_2[0], tupledArg_2[1], tupledArg_2[2])), "LREdges", ["O.OO.OOGOY.Y...Y.YBBBR.RG.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, false], singleton("M2 U M U2 M U")], [(cube_3) => ((tupledArg_3) => matchesGeneric(cube_3, tupledArg_3[0], tupledArg_3[1], tupledArg_3[2])), "LREdges", ["O.OO.OR.RY.Y...Y.YG.GOGOBBBBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true], singleton("M2 U M\' U2 M\' U\'")], [(cube_4) => ((tupledArg_4) => matchesGeneric(cube_4, tupledArg_4[0], tupledArg_4[1], tupledArg_4[2])), "LREdges", ["O.OO.ORGRY.Y...Y.YG.GO.OBBBBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, false], singleton("M2 U\' M U2 M U")], [(cube_5) => ((tupledArg_5) => matchesGeneric(cube_5, tupledArg_5[0], tupledArg_5[1], tupledArg_5[2])), "LREdges", ["O.OO.OO.OY.Y...Y.YBBBRGRG.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true], singleton("M2 U\' M\' U2 M\' U\'")], [(cube_6) => ((tupledArg_6) => matchesGeneric(cube_6, tupledArg_6[0], tupledArg_6[1], tupledArg_6[2])), "LREdges", ["O.OO.OBBBY.Y...Y.YR.RG.GO.OBBBR.RGGGBBBRGRGGGW.WW.WW.W", false, false, true], singleton("M U2 M U")], [(cube_7) => ((tupledArg_7) => matchesGeneric(cube_7, tupledArg_7[0], tupledArg_7[1], tupledArg_7[2])), "LREdges", ["O.OO.OBGBY.Y...Y.YR.RGBGO.OBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, false], singleton("M2 U2 M2 U")], [(cube_8) => ((tupledArg_8) => matchesGeneric(cube_8, tupledArg_8[0], tupledArg_8[1], tupledArg_8[2])), "LREdges", ["O.OO.OGBGY.Y...Y.YO.OBGBR.RBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true], singleton("M2 U2 M2 U\'")], [(cube_9) => ((tupledArg_9) => matchesGeneric(cube_9, tupledArg_9[0], tupledArg_9[1], tupledArg_9[2])), "LREdges", ["OGOO.OB.BY.Y...Y.YR.RGBGO.OBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true], singleton("M U2 M\' U")], [(cube_10) => ((tupledArg_10) => matchesGeneric(cube_10, tupledArg_10[0], tupledArg_10[1], tupledArg_10[2])), "LREdges", ["O.OO.OR.RY.Y...Y.YGBGOGOB.BBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, false], singleton("M2 U\' M U2 M\' U")], [(cube_11) => ((tupledArg_11) => matchesGeneric(cube_11, tupledArg_11[0], tupledArg_11[1], tupledArg_11[2])), "LREdges", ["O.OO.OOGOY.Y...Y.YB.BR.RGBGBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true], singleton("M2 U\' M\' U2 M U\'")], [(cube_12) => ((tupledArg_12) => matchesGeneric(cube_12, tupledArg_12[0], tupledArg_12[1], tupledArg_12[2])), "LREdges", ["O.OO.OO.OY.Y...Y.YB.BRGRGBGBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, false], singleton("M2 U M U2 M\' U")], [(cube_13) => ((tupledArg_13) => matchesGeneric(cube_13, tupledArg_13[0], tupledArg_13[1], tupledArg_13[2])), "LREdges", ["O.OO.ORGRY.Y...Y.YGBGO.OB.BBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true], singleton("M2 U M\' U2 M U\'")], [(cube_14) => ((tupledArg_14) => matchesGeneric(cube_14, tupledArg_14[0], tupledArg_14[1], tupledArg_14[2])), "LREdges", ["O.OO.OGBGY.Y...Y.YO.OB.BR.RBBBR.RGGGBBBRGRGGGW.WW.WW.W", false, false, true], singleton("M\' U2 M U\'")], [(cube_15) => ((tupledArg_15) => matchesGeneric(cube_15, tupledArg_15[0], tupledArg_15[1], tupledArg_15[2])), "LREdges", ["OBOO.OG.GY.Y...Y.YO.OBGBR.RBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true], singleton("M U2 M\' U\'")], [(cube_16) => ((tupledArg_16) => matchesGeneric(cube_16, tupledArg_16[0], tupledArg_16[1], tupledArg_16[2])), "LREdges", ["OBOO.OB.BY.Y...Y.YR.RGGGO.OBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true], singleton("M\' U2 M\' U")], [(cube_17) => ((tupledArg_17) => matchesGeneric(cube_17, tupledArg_17[0], tupledArg_17[1], tupledArg_17[2])), "LREdges", ["OBOO.OR.RY.Y...Y.YG.GOGOB.BBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true], ofArray(["M\' U2 M U\' M2 U", "M U2 M U M2 U\'"])], [(cube_18) => ((tupledArg_18) => matchesGeneric(cube_18, tupledArg_18[0], tupledArg_18[1], tupledArg_18[2])), "LREdges", ["OBOO.OO.OY.Y...Y.YB.BRGRG.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true], ofArray(["M\' U2 M U M2 U", "M U2 M U\' M2 U\'"])], [(cube_19) => ((tupledArg_19) => matchesGeneric(cube_19, tupledArg_19[0], tupledArg_19[1], tupledArg_19[2])), "LREdges", ["OBOO.OG.GY.Y...Y.YO.OB.BR.RBBBR.RGGGBBBRGRGGGW.WW.WW.W", false, false, true], singleton("M2 U")], [(cube_20) => ((tupledArg_20) => matchesGeneric(cube_20, tupledArg_20[0], tupledArg_20[1], tupledArg_20[2])), "LREdges", ["O.OO.OOBOY.Y...Y.YBGBR.RG.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true], singleton("M2 U M\' U2 M U")], [(cube_21) => ((tupledArg_21) => matchesGeneric(cube_21, tupledArg_21[0], tupledArg_21[1], tupledArg_21[2])), "LREdges", ["O.OO.OR.RY.Y...Y.YG.GOBOBGBBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true], singleton("M2 U M U2 M\' U\'")], [(cube_22) => ((tupledArg_22) => matchesGeneric(cube_22, tupledArg_22[0], tupledArg_22[1], tupledArg_22[2])), "LREdges", ["O.OO.OR.RY.Y...Y.YGGGOBOB.BBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true], singleton("M2 U\' M\' U2 M\' U")], [(cube_23) => ((tupledArg_23) => matchesGeneric(cube_23, tupledArg_23[0], tupledArg_23[1], tupledArg_23[2])), "LREdges", ["OGOO.OR.RY.Y...Y.YG.GOBOB.BBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true], ofArray(["M\' U2 M U M2 U\'", "M U2 M U\' M2 U"])], [(cube_24) => ((tupledArg_24) => matchesGeneric(cube_24, tupledArg_24[0], tupledArg_24[1], tupledArg_24[2])), "LREdges", ["O.OO.ORGRY.Y...Y.YG.GOBOB.BBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, false], singleton("M2 U M2 U")], [(cube_25) => ((tupledArg_25) => matchesGeneric(cube_25, tupledArg_25[0], tupledArg_25[1], tupledArg_25[2])), "LREdges", ["O.OO.OOBOY.Y...Y.YB.BRGRG.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true], singleton("M2 U M2 U\'")], [(cube_26) => ((tupledArg_26) => matchesGeneric(cube_26, tupledArg_26[0], tupledArg_26[1], tupledArg_26[2])), "LREdges", ["O.OO.OOBOY.Y...Y.YB.BR.RG.GBBBR.RGGGBBBRGRGGGW.WW.WW.W", false, false, true], ofArray(["M\' U2 M\' U\' M2 U\'", "M U2 M\' U M2 U"])], [(cube_27) => ((tupledArg_27) => matchesGeneric(cube_27, tupledArg_27[0], tupledArg_27[1], tupledArg_27[2])), "LREdges", ["O.OO.OO.OY.Y...Y.YBGBRBRG.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, false], singleton("M2 U\' M U2 M\' U\'")], [(cube_28) => ((tupledArg_28) => matchesGeneric(cube_28, tupledArg_28[0], tupledArg_28[1], tupledArg_28[2])), "LREdges", ["O.OO.ORBRY.Y...Y.YG.GO.OBGBBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true], singleton("M2 U\' M\' U2 M U")], [(cube_29) => ((tupledArg_29) => matchesGeneric(cube_29, tupledArg_29[0], tupledArg_29[1], tupledArg_29[2])), "LREdges", ["O.OO.ORBRY.Y...Y.YGGGO.OB.BBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, false], singleton("M2 U M U2 M U\'")], [(cube_30) => ((tupledArg_30) => matchesGeneric(cube_30, tupledArg_30[0], tupledArg_30[1], tupledArg_30[2])), "LREdges", ["O.OO.OO.OY.Y...Y.YB.BRBRGGGBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true], singleton("M2 U M\' U2 M\' U")], [(cube_31) => ((tupledArg_31) => matchesGeneric(cube_31, tupledArg_31[0], tupledArg_31[1], tupledArg_31[2])), "LREdges", ["OGOO.OO.OY.Y...Y.YB.BRBRG.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true], ofArray(["M\' U2 M U\' M2 U\'", "M U2 M U M2 U"])], [(cube_32) => ((tupledArg_32) => matchesGeneric(cube_32, tupledArg_32[0], tupledArg_32[1], tupledArg_32[2])), "LREdges", ["O.OO.OOGOY.Y...Y.YB.BRBRG.GBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, false], singleton("M2 U\' M2 U")], [(cube_33) => ((tupledArg_33) => matchesGeneric(cube_33, tupledArg_33[0], tupledArg_33[1], tupledArg_33[2])), "LREdges", ["O.OO.ORBRY.Y...Y.YG.GOGOB.BBBBR.RGGGBBBR.RGGGW.WW.WW.W", false, false, true], singleton("M2 U\' M2 U\'")], [(cube_34) => ((tupledArg_34) => matchesGeneric(cube_34, tupledArg_34[0], tupledArg_34[1], tupledArg_34[2])), "LREdges", ["O.OO.ORBRY.Y...Y.YG.GO.OB.BBBBR.RGGGBBBRGRGGGW.WW.WW.W", false, false, true], ofArray(["M\' U2 M\' U M2 U\'", "M U2 M\' U\' M2 U"])], [(cube_35) => ((tupledArg_35) => matchesGeneric(cube_35, tupledArg_35[0], tupledArg_35[1], tupledArg_35[2])), "LREdges", ["O.OO.OBGBY.Y...Y.YR.RG.GO.OBBBR.RGGGBBBRBRGGGW.WW.WW.W", false, false, true], singleton("M\' U2 M U")], [(cube_36) => ((tupledArg_36) => matchesGeneric(cube_36, tupledArg_36[0], tupledArg_36[1], tupledArg_36[2])), "LREdges", ["O.OO.OGGGY.Y...Y.YO.OB.BR.RBBBR.RGGGBBBRBRGGGW.WW.WW.W", false, false, true], singleton("M U2 M U\'")], [(cube_37) => ((tupledArg_37) => matchesGeneric(cube_37, tupledArg_37[0], tupledArg_37[1], tupledArg_37[2])), "LREdges", ["OGOO.OB.BY.Y...Y.YR.RG.GO.OBBBR.RGGGBBBRBRGGGW.WW.WW.W", false, false, true], singleton("M2 U\'")], [(cube_38) => ((tupledArg_38) => matchesGeneric(cube_38, tupledArg_38[0], tupledArg_38[1], tupledArg_38[2])), "LREdges", ["O.OO.OOGOY.Y...Y.YB.BR.RG.GBBBR.RGGGBBBRBRGGGW.WW.WW.W", false, false, true], ofArray(["M\' U2 M\' U M2 U", "M U2 M\' U\' M2 U\'"])], [(cube_39) => ((tupledArg_39) => matchesGeneric(cube_39, tupledArg_39[0], tupledArg_39[1], tupledArg_39[2])), "LREdges", ["O.OO.ORGRY.Y...Y.YG.GO.OB.BBBBR.RGGGBBBRBRGGGW.WW.WW.W", false, false, true], ofArray(["M\' U2 M\' U\' M2 U", "M U2 M\' U M2 U\'"])]]);

export const eolrBeginnerPatterns = append(eoBeginnerPatterns, lrBeginnerPatterns);

export const eolrIntermediatePatterns = append(eoIntermediatePatterns, lrIntermediatePatterns);

export const l4eBeginnerPatterns = ofArray([[(cube) => ((tupledArg) => matchesGeneric(cube, tupledArg[0], tupledArg[1], tupledArg[2])), "L4E", ["OOOOOOOOOYYYYYYYYYBBBRRRGGGBBBRRRGGGBBBRRRGGGWWWWWWWWW", false, false, false], empty()], [(cube_1) => ((tupledArg_1) => matchesGeneric(cube_1, tupledArg_1[0], tupledArg_1[1], tupledArg_1[2])), "L4E", ["OROOROOROYWYYWYYWYBBBRORGGGBBBRORGGGBBBRORGGGWYWWYWWYW", false, false, false], singleton("M2")], [(cube_2) => ((tupledArg_2) => matchesGeneric(cube_2, tupledArg_2[0], tupledArg_2[1], tupledArg_2[2])), "L4E", ["OOOOROOROYWYYWYYYYBBBRRRGGGBBBRORGGGBBBRORGGGWWWWYWWYW", false, false, false], singleton("M\' U2 M\' U2")], [(cube_3) => ((tupledArg_3) => matchesGeneric(cube_3, tupledArg_3[0], tupledArg_3[1], tupledArg_3[2])), "L4E", ["OROOROOOOYWYYWYYYYBBBRRRGGGBBBRORGGGBBBRORGGGWYWWYWWWW", false, false, false], singleton("M\' U2 M\' U2 M\' U2 M\' U2 M\' U2 M\' U2 M\' U2 M\' U2 M\' U2 M\' U2")], [(cube_4) => ((tupledArg_4) => matchesGeneric(cube_4, tupledArg_4[0], tupledArg_4[1], tupledArg_4[2])), "L4E", ["OROOOOOOOYYYYYYYWYBBBRORGGGBBBRRRGGGBBBRRRGGGWYWWWWWWW", false, false, false], singleton("M\' U2 M\' U2 M2")], [(cube_5) => ((tupledArg_5) => matchesGeneric(cube_5, tupledArg_5[0], tupledArg_5[1], tupledArg_5[2])), "L4E", ["OOOOOOOROYYYYYYYWYBBBRORGGGBBBRRRGGGBBBRRRGGGWWWWWWWYW", false, false, false], singleton("M\' U2 M\' U2 M\' U2 M\' U2 M\' U2 M\' U2 M\' U2 M\' U2 M\' U2 M\' U2 M2")], [(cube_6) => ((tupledArg_6) => matchesGeneric(cube_6, tupledArg_6[0], tupledArg_6[1], tupledArg_6[2])), "L4E", ["OROOOOOOOYWYYYYYYYBBBRORGGGBBBRRRGGGBBBRRRGGGWWWWWWWYW", false, false, false], singleton("M\' U2 M\' U2 M\' U2 M2 U2 M")], [(cube_7) => ((tupledArg_7) => matchesGeneric(cube_7, tupledArg_7[0], tupledArg_7[1], tupledArg_7[2])), "L4E", ["OOOOOOOROYWYYYYYYYBBBRORGGGBBBRRRGGGBBBRRRGGGWYWWWWWWW", false, false, false], singleton("M\' U2 M\' U2 M\' U2 M\' U2 U2 M2 U2 M2")], [(cube_8) => ((tupledArg_8) => matchesGeneric(cube_8, tupledArg_8[0], tupledArg_8[1], tupledArg_8[2])), "L4E", ["OOOOROOROYYYYWYYWYBBBRRRGGGBBBRORGGGBBBRORGGGWYWWYWWWW", false, false, false], singleton("M\' U2 M\' U2 M\' U2 M2 U2 M\'")], [(cube_9) => ((tupledArg_9) => matchesGeneric(cube_9, tupledArg_9[0], tupledArg_9[1], tupledArg_9[2])), "L4E", ["OROOROOOOYYYYWYYWYBBBRRRGGGBBBRORGGGBBBRORGGGWWWWYWWYW", false, false, false], singleton("M\' U2 M\' U2 M\' U2 M\' U2 U2 M2 U2")], [(cube_10) => ((tupledArg_10) => matchesGeneric(cube_10, tupledArg_10[0], tupledArg_10[1], tupledArg_10[2])), "L4E", ["OOOOOOOROYYYYYYYWYBBBRRRGGGBBBRRRGGGBBBRORGGGWYWWWWWWW", false, false, false], singleton("M\' U2 M\' U2 U2 M2 U2")], [(cube_11) => ((tupledArg_11) => matchesGeneric(cube_11, tupledArg_11[0], tupledArg_11[1], tupledArg_11[2])), "L4E", ["OROOOOOOOYYYYYYYWYBBBRRRGGGBBBRRRGGGBBBRORGGGWWWWWWWYW", false, false, false], singleton("M\' U2 M\' U2 M\' U2 M\' U2 M\' U2 M2 U2 M\'")], [(cube_12) => ((tupledArg_12) => matchesGeneric(cube_12, tupledArg_12[0], tupledArg_12[1], tupledArg_12[2])), "L4E", ["OROOROOOOYWYYWYYYYBBBRORGGGBBBRORGGGBBBRRRGGGWWWWYWWYW", false, false, false], singleton("M\' U2 M\' U2 U2 M2 U2 M2")], [(cube_13) => ((tupledArg_13) => matchesGeneric(cube_13, tupledArg_13[0], tupledArg_13[1], tupledArg_13[2])), "L4E", ["OOOOROOROYWYYWYYYYBBBRORGGGBBBRORGGGBBBRRRGGGWYWWYWWWW", false, false, false], singleton("M\' U2 M\' U2 M\' U2 M\' U2 M\' U2 M2 U2 M")], [(cube_14) => ((tupledArg_14) => matchesGeneric(cube_14, tupledArg_14[0], tupledArg_14[1], tupledArg_14[2])), "L4E", ["OROOROOOOYYYYWYYWYBBBRORGGGBBBRORGGGBBBRRRGGGWYWWYWWWW", false, false, false], singleton("M\' U2 M\' U2 M\' U2 M\' U2 M\' U2 M\' U2 M\' U2 M\' U2 M2")], [(cube_15) => ((tupledArg_15) => matchesGeneric(cube_15, tupledArg_15[0], tupledArg_15[1], tupledArg_15[2])), "L4E", ["OOOOROOROYYYYWYYWYBBBRORGGGBBBRORGGGBBBRRRGGGWWWWYWWYW", false, false, false], singleton("M\' U2 M\' U2 M\' U2 M\' U2 M2")], [(cube_16) => ((tupledArg_16) => matchesGeneric(cube_16, tupledArg_16[0], tupledArg_16[1], tupledArg_16[2])), "L4E", ["OOOOOOOROYWYYYYYYYBBBRRRGGGBBBRRRGGGBBBRORGGGWWWWWWWYW", false, false, false], singleton("M\' U2 M\' U2 M\' U2 M\' U2 M\' U2 M\' U2 M\' U2 M\' U2")], [(cube_17) => ((tupledArg_17) => matchesGeneric(cube_17, tupledArg_17[0], tupledArg_17[1], tupledArg_17[2])), "L4E", ["OROOOOOOOYWYYYYYYYBBBRRRGGGBBBRRRGGGBBBRORGGGWYWWWWWWW", false, false, false], singleton("M\' U2 M\' U2 M\' U2 M\' U2")], [(cube_18) => ((tupledArg_18) => matchesGeneric(cube_18, tupledArg_18[0], tupledArg_18[1], tupledArg_18[2])), "L4E", ["OOOOROOOOYWYYWYYWYBBBRRRGGGBBBRORGGGBBBRRRGGGWYWWYWWYW", false, false, false], singleton("U2 M2 U2")], [(cube_19) => ((tupledArg_19) => matchesGeneric(cube_19, tupledArg_19[0], tupledArg_19[1], tupledArg_19[2])), "L4E", ["OROOOOOROYYYYYYYYYBBBRORGGGBBBRRRGGGBBBRORGGGWWWWWWWWW", false, false, false], singleton("U2 M2 U2 M2")], [(cube_20) => ((tupledArg_20) => matchesGeneric(cube_20, tupledArg_20[0], tupledArg_20[1], tupledArg_20[2])), "L4E", ["OROOROOROYYYYWYYYYBBBRORGGGBBBRORGGGBBBRORGGGWWWWYWWWW", false, false, false], singleton("M U2 M2 U2 M\'")], [(cube_21) => ((tupledArg_21) => matchesGeneric(cube_21, tupledArg_21[0], tupledArg_21[1], tupledArg_21[2])), "L4E", ["OOOOOOOOOYWYYYYYWYBBBRRRGGGBBBRRRGGGBBBRRRGGGWYWWWWWYW", false, false, false], singleton("M\' U2 M2 U2 M\'")], [(cube_22) => ((tupledArg_22) => matchesGeneric(cube_22, tupledArg_22[0], tupledArg_22[1], tupledArg_22[2])), "L4E", ["OROOOOOROYWYYYYYWYBBBRORGGGBBBRRRGGGBBBRORGGGWYWWWWWYW", false, false, false], singleton("M\' U2 M\' U2 M\' U2 M\' U2 M\' U2 M\' U2 M2")], [(cube_23) => ((tupledArg_23) => matchesGeneric(cube_23, tupledArg_23[0], tupledArg_23[1], tupledArg_23[2])), "L4E", ["OOOOROOOOYYYYWYYYYBBBRRRGGGBBBRORGGGBBBRRRGGGWWWWYWWWW", false, false, false], singleton("M\' U2 M\' U2 M\' U2 M\' U2 M\' U2 M\' U2")]]);

export const l4eIntermediatePatterns = ofArray([[(cube) => ((tupledArg) => matchesGeneric(cube, tupledArg[0], tupledArg[1], tupledArg[2])), "L4E", ["OOOOOOOOOYYYYYYYYYBBBRRRGGGBBBRRRGGGBBBRRRGGGWWWWWWWWW", false, false, false], empty()], [(cube_1) => ((tupledArg_1) => matchesGeneric(cube_1, tupledArg_1[0], tupledArg_1[1], tupledArg_1[2])), "L4E", ["OROOROOROYWYYWYYWYBBBRORGGGBBBRORGGGBBBRORGGGWYWWYWWYW", false, false, false], singleton("M2")], [(cube_2) => ((tupledArg_2) => matchesGeneric(cube_2, tupledArg_2[0], tupledArg_2[1], tupledArg_2[2])), "L4E", ["OOOOROOROYWYYWYYYYBBBRRRGGGBBBRORGGGBBBRORGGGWWWWYWWYW", false, false, false], singleton("M\' U2 M\' U2")], [(cube_3) => ((tupledArg_3) => matchesGeneric(cube_3, tupledArg_3[0], tupledArg_3[1], tupledArg_3[2])), "L4E", ["OROOROOOOYWYYWYYYYBBBRRRGGGBBBRORGGGBBBRORGGGWYWWYWWWW", false, false, false], singleton("U2 M U2 M")], [(cube_4) => ((tupledArg_4) => matchesGeneric(cube_4, tupledArg_4[0], tupledArg_4[1], tupledArg_4[2])), "L4E", ["OROOOOOOOYYYYYYYWYBBBRORGGGBBBRRRGGGBBBRRRGGGWYWWWWWWW", false, false, false], singleton("M\' U2 M\' U2 M2")], [(cube_5) => ((tupledArg_5) => matchesGeneric(cube_5, tupledArg_5[0], tupledArg_5[1], tupledArg_5[2])), "L4E", ["OOOOOOOROYYYYYYYWYBBBRORGGGBBBRRRGGGBBBRRRGGGWWWWWWWYW", false, false, false], singleton("U2 M U2 M\'")], [(cube_6) => ((tupledArg_6) => matchesGeneric(cube_6, tupledArg_6[0], tupledArg_6[1], tupledArg_6[2])), "L4E", ["OROOOOOOOYWYYYYYYYBBBRORGGGBBBRRRGGGBBBRRRGGGWWWWWWWYW", false, false, false], singleton("M U2 M\' U2")], [(cube_7) => ((tupledArg_7) => matchesGeneric(cube_7, tupledArg_7[0], tupledArg_7[1], tupledArg_7[2])), "L4E", ["OOOOOOOROYWYYYYYYYBBBRORGGGBBBRRRGGGBBBRRRGGGWYWWWWWWW", false, false, false], singleton("U2 M\' U2 M")], [(cube_8) => ((tupledArg_8) => matchesGeneric(cube_8, tupledArg_8[0], tupledArg_8[1], tupledArg_8[2])), "L4E", ["OOOOROOROYYYYWYYWYBBBRRRGGGBBBRORGGGBBBRORGGGWYWWYWWWW", false, false, false], singleton("M U2 M\' U2 M2")], [(cube_9) => ((tupledArg_9) => matchesGeneric(cube_9, tupledArg_9[0], tupledArg_9[1], tupledArg_9[2])), "L4E", ["OROOROOOOYYYYWYYWYBBBRRRGGGBBBRORGGGBBBRORGGGWWWWYWWYW", false, false, false], singleton("U2 M\' U2 M\'")], [(cube_10) => ((tupledArg_10) => matchesGeneric(cube_10, tupledArg_10[0], tupledArg_10[1], tupledArg_10[2])), "L4E", ["OOOOOOOROYYYYYYYWYBBBRRRGGGBBBRRRGGGBBBRORGGGWYWWWWWWW", false, false, false], singleton("M\' U2 M U2")], [(cube_11) => ((tupledArg_11) => matchesGeneric(cube_11, tupledArg_11[0], tupledArg_11[1], tupledArg_11[2])), "L4E", ["OROOOOOOOYYYYYYYWYBBBRRRGGGBBBRRRGGGBBBRORGGGWWWWWWWYW", false, false, false], singleton("M2 U2 M U2 M")], [(cube_12) => ((tupledArg_12) => matchesGeneric(cube_12, tupledArg_12[0], tupledArg_12[1], tupledArg_12[2])), "L4E", ["OROOROOOOYWYYWYYYYBBBRORGGGBBBRORGGGBBBRRRGGGWWWWYWWYW", false, false, false], singleton("M\' U2 M U2 M2")], [(cube_13) => ((tupledArg_13) => matchesGeneric(cube_13, tupledArg_13[0], tupledArg_13[1], tupledArg_13[2])), "L4E", ["OOOOROOROYWYYWYYYYBBBRORGGGBBBRORGGGBBBRRRGGGWYWWYWWWW", false, false, false], singleton("M2 U2 M U2 M\'")], [(cube_14) => ((tupledArg_14) => matchesGeneric(cube_14, tupledArg_14[0], tupledArg_14[1], tupledArg_14[2])), "L4E", ["OROOROOOOYYYYWYYWYBBBRORGGGBBBRORGGGBBBRRRGGGWYWWYWWWW", false, false, false], singleton("M U2 M U2")], [(cube_15) => ((tupledArg_15) => matchesGeneric(cube_15, tupledArg_15[0], tupledArg_15[1], tupledArg_15[2])), "L4E", ["OOOOROOROYYYYWYYWYBBBRORGGGBBBRORGGGBBBRRRGGGWWWWYWWYW", false, false, false], singleton("M2 U2 M\' U2 M")], [(cube_16) => ((tupledArg_16) => matchesGeneric(cube_16, tupledArg_16[0], tupledArg_16[1], tupledArg_16[2])), "L4E", ["OOOOOOOROYWYYYYYYYBBBRRRGGGBBBRRRGGGBBBRORGGGWWWWWWWYW", false, false, false], singleton("M U2 M U2 M2")], [(cube_17) => ((tupledArg_17) => matchesGeneric(cube_17, tupledArg_17[0], tupledArg_17[1], tupledArg_17[2])), "L4E", ["OROOOOOOOYWYYYYYYYBBBRRRGGGBBBRRRGGGBBBRORGGGWYWWWWWWW", false, false, false], singleton("M2 U2 M\' U2 M\'")], [(cube_18) => ((tupledArg_18) => matchesGeneric(cube_18, tupledArg_18[0], tupledArg_18[1], tupledArg_18[2])), "L4E", ["OOOOROOOOYWYYWYYWYBBBRRRGGGBBBRORGGGBBBRRRGGGWYWWYWWYW", false, false, false], singleton("U2 M2 U2")], [(cube_19) => ((tupledArg_19) => matchesGeneric(cube_19, tupledArg_19[0], tupledArg_19[1], tupledArg_19[2])), "L4E", ["OROOOOOROYYYYYYYYYBBBRORGGGBBBRRRGGGBBBRORGGGWWWWWWWWW", false, false, false], singleton("U2 M2 U2 M2")], [(cube_20) => ((tupledArg_20) => matchesGeneric(cube_20, tupledArg_20[0], tupledArg_20[1], tupledArg_20[2])), "L4E", ["OROOROOROYYYYWYYYYBBBRORGGGBBBRORGGGBBBRORGGGWWWWYWWWW", false, false, false], singleton("M U2 M2 U2 M\'")], [(cube_21) => ((tupledArg_21) => matchesGeneric(cube_21, tupledArg_21[0], tupledArg_21[1], tupledArg_21[2])), "L4E", ["OOOOOOOOOYWYYYYYWYBBBRRRGGGBBBRRRGGGBBBRRRGGGWYWWWWWYW", false, false, false], singleton("M\' U2 M2 U2 M\'")], [(cube_22) => ((tupledArg_22) => matchesGeneric(cube_22, tupledArg_22[0], tupledArg_22[1], tupledArg_22[2])), "L4E", ["OROOOOOROYWYYYYYWYBBBRORGGGBBBRRRGGGBBBRORGGGWYWWWWWYW", false, false, false], singleton("E2 M E2 M")], [(cube_23) => ((tupledArg_23) => matchesGeneric(cube_23, tupledArg_23[0], tupledArg_23[1], tupledArg_23[2])), "L4E", ["OOOOROOOOYYYYWYYYYBBBRRRGGGBBBRORGGGBBBRRRGGGWWWWYWWWW", false, false, false], singleton("E2 M E2 M\'")]]);

export const lseBeginnerPatterns = append(eolrBeginnerPatterns, l4eIntermediatePatterns);

export const lseIntermediatePatterns = append(eolrIntermediatePatterns, l4eIntermediatePatterns);

export const rouxBeginnerPatterns = append(fbBeginnerPatterns, append(sbBeginnerPatterns, append(cmllBeginnerPatterns, lseBeginnerPatterns)));

export const rouxIntermediatePatterns = append(fbIntermediatePatterns, append(sbIntermediatePatterns, append(cmllIntermediatePatterns, lseIntermediatePatterns)));

export const rouxAdvancedPatterns = append(fbIntermediatePatterns, append(sbIntermediatePatterns, append(cmllAdvancedPatterns, lseIntermediatePatterns)));

export const rouxGodPatterns = append(fbGodPatterns, append(sbIntermediatePatterns, append(cmllAdvancedPatterns, lseIntermediatePatterns)));

export function solve(moves, description, name, target, cubes, search) {
    let corners, edgeOrientation;
    return solveCase(expandPatternsForAuf((level === 0) ? ((corners = (fullCmll() ? cmllAdvancedPatterns : append((cornerOrientationLevel() === 0) ? coBeginnerPatterns : coIntermediatePatterns, (cornerPermutationLevel() === 0) ? cpBeginnerPatterns : cpIntermediatePatterns)), (edgeOrientation = ((edgeOrientationLevel() === 0) ? edgeBeginnerOrientationPatters : edgeIntermediateOrientationPatters), append(dlEdgeBeginnerPatterns, append(lCenterBeginnerPatterns, append((lbPairLevel() === 0) ? lbPairBeginnerPatterns : lbPairIntermediatePatterns, append((lfPairLevel() === 0) ? lfPairBeginnerPatterns : lfPairIntermediatePatterns, append(drEdgeBeginnerPatterns, append((rbPairLevel() === 0) ? rbPairBeginnerPatterns : rbPairIntermediatePatterns, append((rfPairLevel() === 0) ? rfPairBeginnerPatterns : rfPairIntermediatePatterns, append(corners, append(centerOrientationPatterns, append(edgeOrientation, append(lrBeginnerPatterns, l4eIntermediatePatterns)))))))))))))) : ((level === 1) ? rouxIntermediatePatterns : ((level === 2) ? rouxAdvancedPatterns : ((level === 3) ? rouxGodPatterns : (() => {
        throw new Error("Unknown level");
    })())))), moves, description, name, target, cubes, search);
}

export function generateFrom(scrambled) {
    const numCubes = length(scrambled) | 0;
    const moves = append(ofArray([new Step(1, [new Move(0, [])]), new Step(1, [new Move(1, [])]), new Step(1, [new Move(2, [])]), new Step(1, [new Move(3, [])]), new Step(1, [new Move(4, [])]), new Step(1, [new Move(5, [])])]), append(ofArray([new Step(1, [new Move(6, [])]), new Step(1, [new Move(7, [])]), new Step(1, [new Move(8, [])]), new Step(1, [new Move(9, [])]), new Step(1, [new Move(10, [])]), new Step(1, [new Move(11, [])])]), append(ofArray([new Step(1, [new Move(12, [])]), new Step(1, [new Move(13, [])]), new Step(1, [new Move(14, [])]), new Step(1, [new Move(15, [])]), new Step(1, [new Move(16, [])]), new Step(1, [new Move(17, [])])]), append(ofArray([new Step(1, [new Move(18, [])]), new Step(1, [new Move(19, [])]), new Step(1, [new Move(20, [])]), new Step(1, [new Move(21, [])]), new Step(1, [new Move(22, [])]), new Step(1, [new Move(23, [])])]), append(ofArray([new Step(1, [new Move(24, [])]), new Step(1, [new Move(25, [])]), new Step(1, [new Move(26, [])]), new Step(1, [new Move(27, [])]), new Step(1, [new Move(28, [])]), new Step(1, [new Move(29, [])])]), append(ofArray([new Step(1, [new Move(30, [])]), new Step(1, [new Move(31, [])]), new Step(1, [new Move(32, [])]), new Step(1, [new Move(33, [])]), new Step(1, [new Move(34, [])]), new Step(1, [new Move(35, [])])]), append(ofArray([new Step(1, [new Move(36, [])]), new Step(1, [new Move(37, [])]), new Step(1, [new Move(38, [])])]), append(ofArray([new Step(1, [new Move(39, [])]), new Step(1, [new Move(40, [])]), new Step(1, [new Move(41, [])])]), ofArray([new Step(1, [new Move(42, [])]), new Step(1, [new Move(43, [])]), new Step(1, [new Move(44, [])])])))))))));
    const caseLBPair = (cube) => lookPattern("O..O.......................BB.......BB..........W..W..".split(""), cube);
    const caseLFPair = (cube_1) => lookPattern("............................BBR......BBR.....W..W.....".split(""), cube_1);
    const caseSolvedFB = (cube_2) => lookPattern("O..O.......................BBBR.....BBBR.....W..W..W..".split(""), cube_2);
    const solvedDL = solve(ofArray([new Step(0, [new Rotate(0, [])]), new Step(0, [new Rotate(1, [])]), new Step(0, [new Rotate(2, [])]), new Step(0, [new Rotate(3, [])]), new Step(0, [new Rotate(4, [])]), new Step(0, [new Rotate(5, [])]), new Step(0, [new Rotate(6, [])]), new Step(0, [new Rotate(7, [])]), new Step(0, [new Rotate(8, [])])]), "Solve DL edge (during inspection)", "DLEdge", (cube_3) => lookPattern(".....................................B..........W.....".split(""), cube_3), scrambled, false);
    stageStats("Inspection", numCubes);
    const solvedLC = solve(moves, "Solve L center", "LCenter", (cube_4) => lookPattern("............................B........B..........W.....".split(""), cube_4), solvedDL, false);
    const solveLBFirst = (input) => {
        let solvedLBPair;
        if ((level === 0) && (lbPairLevel() === 0)) {
            const caseLBtoFD = (cube_5) => lookPattern("............................B........B..B.....O.W.....".split(""), cube_5);
            solvedLBPair = solve(moves, "Pair and insert LB pair", "InsertLBPair", caseLBPair, solve(moves, "Bring DLB corner to URB", "BringDLBtoU", (c) => {
                if (caseLBtoFD(c)) {
                    if (lookPattern("........B..O..............W...........................".split(""), c)) {
                        return true;
                    }
                    else if (level > 0) {
                        return lookPattern(".................B.....OW.............................".split(""), c);
                    }
                    else {
                        return false;
                    }
                }
                else {
                    return false;
                }
            }, solve(moves, "Tuck LB to FD", "TuckLBtoFD", caseLBtoFD, input, false), false), false);
        }
        else {
            solvedLBPair = solve(moves, "Pair and insert LB pair", "InsertLBPair", caseLBPair, input, true);
        }
        if ((level === 0) && (lfPairLevel() === 0)) {
            return solve(moves, "Pair and insert LF pair (complete FB)", "InsertLFPair", caseSolvedFB, solve(moves, "Bring DLF corner to URF", "BringDLFtoURF", (cube_9) => lookPattern("OB.O.............R.....BW..BB.......BB..........W..WR.".split(""), cube_9), solve(moves, "Tuck LF to BD", "TuckLFtoBD", (cube_8) => lookPattern("OB.O.......................BB.......BB..........W..WR.".split(""), cube_8), solvedLBPair, false), false), true);
        }
        else {
            return solve(moves, "Pair and insert LF pair (complete FB)", "InsertLFPair", caseSolvedFB, solvedLBPair, true);
        }
    };
    const solveLFFirst = (input_1) => {
        const candidates = (patterns, stage) => {
            const algorithms = map((algorithm) => {
                if (isNullOrWhiteSpace(algorithm)) {
                    return empty();
                }
                else {
                    return stringToSteps(join(" ", split(algorithm, [" "], void 0, 1)));
                }
            }, List_distinct(collect((tupledArg_1) => {
                const values = tupledArg_1[3];
                if (isEmpty(values)) {
                    return singleton("");
                }
                else {
                    return values;
                }
            }, filter((tupledArg) => equals(tupledArg[1], stage), patterns)), {
                Equals: (x, y) => (x === y),
                GetHashCode: stringHash,
            }));
            return List_distinct(toList(delay(() => collect_1((auf) => map_1((algorithm_1) => append(auf, algorithm_1), algorithms), [empty(), singleton(new Step(1, [new Move(0, [])])), singleton(new Step(1, [new Move(1, [])])), singleton(new Step(1, [new Move(2, [])]))]))), {
                Equals: equals,
                GetHashCode: safeHash,
            });
        };
        const solveCandidates = (description, stage_1, goal, algorithms_1, cubes) => map((cube_10) => {
            const matchValue = tryHead(sortBy(length, filter((algorithm_2) => goal(executeSteps(algorithm_2, cube_10)), algorithms_1), {
                Compare: comparePrimitives,
            }));
            if (matchValue == null) {
                throw new Error(`No ${description} candidate preserves the completed block`);
            }
            else {
                const algorithm_3 = matchValue;
                solutionTrace(append(solutionTrace(), singleton([stage_1, algorithm_3])));
                return executeSteps(algorithm_3, cube_10);
            }
        }, cubes);
        let solvedLFPair;
        if (lfPairLevel() === 0) {
            const tucked = solveCandidates("LF tuck", "TuckLFFirsttoBD", (cube_11) => lookPattern(".B..........................B........B..........W...R.".split(""), cube_11), candidates(lfPairBeginnerPatterns, "TuckLFtoBD"), input_1);
            const ready = solveCandidates("LF corner setup", "BringDLFFirsttoURF", (cube_12) => lookPattern(".B...............R.....BW...B........B..........W...R.".split(""), cube_12), candidates(lfPairBeginnerPatterns, "BringDLFtoURF"), tucked);
            solvedLFPair = solveCandidates("LF-first", "InsertLFFirstPair", caseLFPair, candidates(lfPairBeginnerPatterns, "InsertLFPair"), ready);
        }
        else {
            solvedLFPair = solveCandidates("LF-first", "InsertLFFirstPair", caseLFPair, candidates(lfPairFirstIntermediatePatterns, "InsertLFFirstPair"), input_1);
        }
        if (lbPairLevel() === 0) {
            const tucked_1 = solveCandidates("LB tuck", "TuckLBLasttoFD", (cube_13) => lookPattern("............................BBR......BBRB....WO.W.....".split(""), cube_13), candidates(lbPairBeginnerPatterns, "TuckLBtoFD"), solvedLFPair);
            const ready_1 = solveCandidates("LB corner setup", "BringDLBLasttoU", (cube_14) => lookPattern("........B..O..............W.BBR......BBRB....WO.W.....".split(""), cube_14), candidates(lbPairBeginnerPatterns, "BringDLBtoU"), tucked_1);
            return solveCandidates("LB-last", "InsertLBLastPair", caseSolvedFB, candidates(lbPairBeginnerPatterns, "InsertLBPair"), ready_1);
        }
        else {
            return solveCandidates("LB-last", "InsertLBLastPair", caseSolvedFB, candidates(lbPairLastIntermediatePatterns, "InsertLBLastPair"), solvedLFPair);
        }
    };
    let solvedFB;
    if (level >= 3) {
        solvedFB = solve(moves, "Build LB square", "BuildLBSquare", caseLBPair, scrambled, true);
    }
    else if (chooseShortestFirstBlockPairOrder() && (numCubes === 1)) {
        const prefix = solutionTrace();
        const backFirst = solveLBFirst(solvedLC);
        const backTrace = skip(length(prefix), solutionTrace());
        solutionTrace(prefix);
        const frontFirst = solveLFFirst(solvedLC);
        const frontTrace = skip(length(prefix), solutionTrace());
        const moveCount = (trace) => sumBy((arg) => length(arg[1]), trace, {
            GetZero: () => 0,
            Add: (x_3, y_3) => (x_3 + y_3),
        });
        if (moveCount(backTrace) <= moveCount(frontTrace)) {
            solutionTrace(append(prefix, backTrace));
            solvedFB = backFirst;
        }
        else {
            solutionTrace(append(prefix, frontTrace));
            solvedFB = frontFirst;
        }
    }
    else if (chooseShortestFirstBlockPairOrder()) {
        const prefix_1 = solutionTrace();
        const backFirst_1 = solveLBFirst(solvedLC);
        const backTrace_1 = skip(length(prefix_1), solutionTrace());
        const perCube = (count_3, trace_1) => map((cubeIndex) => concat(choose((tupledArg_3) => {
            if ((tupledArg_3[0] % count_3) === cubeIndex) {
                return tupledArg_3[1];
            }
            else {
                return void 0;
            }
        }, mapIndexed((index, tupledArg_2) => [index, tupledArg_2[1]], trace_1))), toList(rangeDouble(0, 1, count_3 - 1)));
        const tryFront = (inputs) => {
            solutionTrace(prefix_1);
            try {
                return map2((cube_15, steps_2) => [cube_15, steps_2], solveLFFirst(inputs), perCube(length(inputs), skip(length(prefix_1), solutionTrace())));
            }
            catch (matchValue_1) {
                let matchResult;
                if (!isEmpty(inputs)) {
                    if (isEmpty(tail(inputs))) {
                        matchResult = 0;
                    }
                    else {
                        matchResult = 1;
                    }
                }
                else {
                    matchResult = 1;
                }
                switch (matchResult) {
                    case 0:
                        return singleton(void 0);
                    default: {
                        const patternInput = splitAt(~~(length(inputs) / 2), inputs);
                        return append(tryFront(patternInput[0]), tryFront(patternInput[1]));
                    }
                }
            }
        };
        const tryApplyPattern = (goal_1, patterns_1, cube_16) => map_2((algorithm_5) => [executeSteps(algorithm_5, cube_16), algorithm_5], bind((tupledArg_5) => {
            const values_1 = tupledArg_5[3];
            return tryHead(sortBy(length, filter((algorithm_4) => goal_1(executeSteps(algorithm_4, cube_16)), isEmpty(values_1) ? singleton(empty()) : map(stringToSteps, values_1)), {
                Compare: comparePrimitives,
            }));
        }, tryFind((tupledArg_4) => {
            const _arg_8 = tupledArg_4[2];
            return tupledArg_4[0](cube_16)([_arg_8[0], _arg_8[1], _arg_8[2]]);
        }, patterns_1)));
        const frontOptions = ((lfPairLevel() > 0) && (lbPairLevel() > 0)) ? map((cube_17) => bind((tupledArg_6) => map_2((tupledArg_7) => [tupledArg_7[0], append(tupledArg_6[1], tupledArg_7[1])], tryApplyPattern(caseSolvedFB, lbPairLastIntermediatePatterns, tupledArg_6[0])), tryApplyPattern(caseLFPair, lfPairFirstIntermediatePatterns, cube_17)), solvedLC) : tryFront(solvedLC);
        const backMoves = perCube(numCubes, backTrace_1);
        const matchValue_2 = tryFindIndex((option_4) => (option_4 == null), frontOptions);
        if (matchValue_2 == null) {
        }
        else {
            throw new Error(`Missing front-first first-block pattern for state: ${cubeToString(item(matchValue_2, solvedLC))}`);
        }
        const choices = map2((back, front) => (length(value_1(front)[1]) < length(back)), backMoves, frontOptions);
        solutionTrace(append(prefix_1, map((steps_3) => ["FirstBlockPairs", steps_3], map3((useFront, back_1, front_1) => (useFront ? value_1(front_1)[1] : back_1), choices, backMoves, frontOptions))));
        solvedFB = map3((useFront_1, back_2, front_2) => (useFront_1 ? value_1(front_2)[0] : back_2), choices, backFirst_1, frontOptions);
    }
    else {
        solvedFB = solveLBFirst(solvedLC);
    }
    stageStats("FB", numCubes);
    progressCallback()("First block");
    const sbMoves = ofArray([new Step(1, [new Move(0, [])]), new Step(1, [new Move(1, [])]), new Step(1, [new Move(2, [])]), new Step(1, [new Move(18, [])]), new Step(1, [new Move(19, [])]), new Step(1, [new Move(20, [])]), new Step(1, [new Move(21, [])]), new Step(1, [new Move(22, [])]), new Step(1, [new Move(23, [])]), new Step(1, [new Move(24, [])]), new Step(1, [new Move(25, [])]), new Step(1, [new Move(30, [])]), new Step(1, [new Move(31, [])]), new Step(1, [new Move(36, [])]), new Step(1, [new Move(37, [])]), new Step(1, [new Move(38, [])])]);
    const solvedDR = solve(sbMoves, "Solve DR edge", "DREdge", (cube_18) => lookPattern("O..O.......................BBBR.....BBBR...G.W..W.WW..".split(""), cube_18), solvedFB, false);
    const caseRBPair = (cube_19) => lookPattern("O.OO.O.....................BBBR....GBBBR...GGW..W.WW.W".split(""), cube_19);
    const caseRFPair = (cube_20) => lookPattern("O..O.......................BBBR.RG..BBBR.RGG.W.WW.WW..".split(""), cube_20);
    const caseSolvedSB = (cube_21) => lookPattern("O.OO.O.....................BBBR.RG.GBBBR.RGGGW.WW.WW.W".split(""), cube_21);
    const finishingGoal = orientCentersWithSecondBlock() ? ((c_1) => {
        if (caseSolvedSB(c_1)) {
            if (equals(look(new Face(0, []), new Sticker(4, []), c_1), new Color(2, []))) {
                return true;
            }
            else {
                return equals(look(new Face(0, []), new Sticker(4, []), c_1), new Color(3, []));
            }
        }
        else {
            return false;
        }
    }) : caseSolvedSB;
    const solveSecondBlockFinishingPair = (description_1, stage_2, input_2) => {
        preferGoalMatchingAlgorithm(orientCentersWithSecondBlock());
        const solved_2 = solve(sbMoves, description_1, stage_2, finishingGoal, input_2, !orientCentersWithSecondBlock());
        preferGoalMatchingAlgorithm(false);
        return solved_2;
    };
    const solveRBFirst = (input_3) => {
        const solvedRBPair = ((level === 0) && (rbPairLevel() === 0)) ? solve(sbMoves, "Pair and insert RB pair", "InsertRBPair", caseRBPair, solve(sbMoves, "Bring DRB to ULB", "BringDRBtoULB", (cube_23) => lookPattern("O..O..G.....O.....W........BBBR.....BBBRG..G.WO.W.WW..".split(""), cube_23), solve(sbMoves, "Tuck RB to FD", "TuckRBtoFD", (cube_22) => lookPattern("O..O.......................BBBR.....BBBRG..G.WO.W.WW..".split(""), cube_22), input_3, false), false), false) : solve(sbMoves, "Pair and insert RB pair", "InsertRBPair", caseRBPair, input_3, true);
        if ((level === 0) && (rfPairLevel() === 0)) {
            return solveSecondBlockFinishingPair("Pair and insert RF pair (complete SB)", "InsertRFPair", solve(sbMoves, "Bring DRF to ULF", "BringDRFtoULF", (cube_25) => lookPattern("OGOO.O.........R....WG.....BBBR....GBBBR...GGW..W.WWRW".split(""), cube_25), solve(sbMoves, "Tuck RF to BD", "TuckRFtoBD", (cube_24) => lookPattern("OGOO.O.....................BBBR....GBBBR...GGW..W.WWRW".split(""), cube_24), solvedRBPair, false), false));
        }
        else {
            return solveSecondBlockFinishingPair("Pair and insert RF pair (complete SB)", "InsertRFPair", solvedRBPair);
        }
    };
    const solveRFFirst = (input_4) => {
        const candidates_1 = (patterns_2, stage_3) => {
            const algorithms_2 = map((algorithm_6) => {
                if (isNullOrWhiteSpace(algorithm_6)) {
                    return empty();
                }
                else {
                    return stringToSteps(join(" ", split(algorithm_6, [" "], void 0, 1)));
                }
            }, List_distinct(collect((tupledArg_9) => {
                const values_2 = tupledArg_9[3];
                if (isEmpty(values_2)) {
                    return singleton("");
                }
                else {
                    return values_2;
                }
            }, filter((tupledArg_8) => equals(tupledArg_8[1], stage_3), patterns_2)), {
                Equals: (x_6, y_5) => (x_6 === y_5),
                GetHashCode: stringHash,
            }));
            return List_distinct(collect((steps_4) => {
                const equivalents = cons(steps_4, choose((x_5) => x_5, mapIndexed((index_3, step) => {
                    let _arg_13;
                    return map_2((replacement) => mapIndexed((candidateIndex, candidate) => {
                        if (candidateIndex === index_3) {
                            return replacement;
                        }
                        else {
                            return candidate;
                        }
                    }, steps_4), (_arg_13 = step, (_arg_13.tag === 1) ? ((_arg_13.fields[0].tag === 18) ? (new Step(1, [new Move(21, [])])) : ((_arg_13.fields[0].tag === 19) ? (new Step(1, [new Move(22, [])])) : ((_arg_13.fields[0].tag === 20) ? (new Step(1, [new Move(23, [])])) : ((_arg_13.fields[0].tag === 21) ? (new Step(1, [new Move(18, [])])) : ((_arg_13.fields[0].tag === 22) ? (new Step(1, [new Move(19, [])])) : ((_arg_13.fields[0].tag === 23) ? (new Step(1, [new Move(20, [])])) : void 0)))))) : void 0));
                }, steps_4)));
                if (orientCentersWithSecondBlock()) {
                    return append(equivalents, collect((candidate_1) => ofArray([append(candidate_1, singleton(new Step(1, [new Move(36, [])]))), append(candidate_1, singleton(new Step(1, [new Move(37, [])])))]), equivalents));
                }
                else {
                    return equivalents;
                }
            }, toList(delay(() => collect_1((auf_2) => map_1((algorithm_7) => append(auf_2, algorithm_7), algorithms_2), [empty(), singleton(new Step(1, [new Move(0, [])])), singleton(new Step(1, [new Move(1, [])])), singleton(new Step(1, [new Move(2, [])]))])))), {
                Equals: equals,
                GetHashCode: safeHash,
            });
        };
        const solveFromCandidates = (description_2, stage_4, goal_2, candidates_2, cubes_1) => map((cube_26) => {
            const matchValue_3 = tryHead(sortBy(length, filter((algorithm_8) => goal_2(executeSteps(algorithm_8, cube_26)), candidates_2), {
                Compare: comparePrimitives,
            }));
            if (matchValue_3 == null) {
                throw new Error(`No ${description_2} candidate preserves the completed block`);
            }
            else {
                const algorithm_9 = matchValue_3;
                solutionTrace(append(solutionTrace(), singleton([stage_4, algorithm_9])));
                return executeSteps(algorithm_9, cube_26);
            }
        }, cubes_1);
        let solvedRFPair;
        if (rfPairLevel() === 0) {
            const tucked_2 = solveFromCandidates("RF tuck", "TuckRFFirsttoBD", (cube_27) => lookPattern("OG.O.......................BBBR.....BBBR...G.W..W.WWR.".split(""), cube_27), candidates_1(rfPairBeginnerPatterns, "TuckRFtoBD"), input_4);
            const cornerReady = solveFromCandidates("RF corner setup", "BringDRFFirsttoULF", (cube_28) => lookPattern("OG.O...........R....WG.....BBBR.....BBBR...G.W..W.WWR.".split(""), cube_28), candidates_1(rfPairBeginnerPatterns, "BringDRFtoULF"), tucked_2);
            solvedRFPair = solveFromCandidates("RF-first", "InsertRFFirstPair", caseRFPair, candidates_1(rfPairBeginnerPatterns, "InsertRFPair"), cornerReady);
        }
        else {
            solvedRFPair = solveFromCandidates("RF-first", "InsertRFFirstPair", caseRFPair, candidates_1(rfPairFirstIntermediatePatterns, "InsertRFFirstPair"), input_4);
        }
        if (rbPairLevel() === 0) {
            const tucked_3 = solveFromCandidates("RB tuck", "TuckRBLasttoFD", (cube_29) => lookPattern("O..O.......................BBBR.RG..BBBRGRGG.WOWW.WW..".split(""), cube_29), candidates_1(rbPairBeginnerPatterns, "TuckRBtoFD"), solvedRFPair);
            const cornerReady_1 = solveFromCandidates("RB corner setup", "BringDRBLasttoULB", (cube_30) => lookPattern("O..O..G.....O.....W........BBBR.RG..BBBRGRGG.WOWW.WW..".split(""), cube_30), candidates_1(rbPairBeginnerPatterns, "BringDRBtoULB"), tucked_3);
            return solveFromCandidates("RB-last", "InsertRBLastPair", finishingGoal, candidates_1(rbPairBeginnerPatterns, "InsertRBPair"), cornerReady_1);
        }
        else {
            return solveFromCandidates("RB-last", "InsertRBLastPair", finishingGoal, candidates_1(rbPairLastIntermediatePatterns, "InsertRBLastPair"), solvedRFPair);
        }
    };
    const mMoves = ofArray([new Step(1, [new Move(36, [])]), new Step(1, [new Move(37, [])]), new Step(1, [new Move(38, [])])]);
    const caseCO = (cube_31) => lookPattern("O.OO.O...Y.Y...Y.Y.........BBBR.RG.GBBBR.RGGGW.WW.WW.W".split(""), cube_31);
    const caseCP = (c_2) => {
        if (caseCO(c_2) && equals(look(new Face(2, []), new Sticker(0, []), c_2), look(new Face(2, []), new Sticker(2, []), c_2))) {
            return equals(look(new Face(3, []), new Sticker(0, []), c_2), look(new Face(3, []), new Sticker(2, []), c_2));
        }
        else {
            return false;
        }
    };
    const caseCenterO = (c_3) => {
        if (caseCP(c_3)) {
            if (equals(look(new Face(0, []), new Sticker(4, []), c_3), new Color(2, []))) {
                return true;
            }
            else {
                return equals(look(new Face(0, []), new Sticker(4, []), c_3), new Color(3, []));
            }
        }
        else {
            return false;
        }
    };
    let solvedSB;
    if (chooseShortestSecondBlockPairOrder() && (numCubes === 1)) {
        const prefix_2 = solutionTrace();
        const backFirst_2 = solveRBFirst(solvedDR);
        const backTrace_2 = skip(length(prefix_2), solutionTrace());
        solutionTrace(prefix_2);
        const frontFirst_1 = solveRFFirst(solvedDR);
        const frontTrace_1 = skip(length(prefix_2), solutionTrace());
        const moveCount_1 = (trace_2) => sumBy((arg_1) => length(arg_1[1]), trace_2, {
            GetZero: () => 0,
            Add: (x_9, y_8) => (x_9 + y_8),
        });
        if (moveCount_1(backTrace_2) <= moveCount_1(frontTrace_1)) {
            solutionTrace(append(prefix_2, backTrace_2));
            solvedSB = backFirst_2;
        }
        else {
            solutionTrace(append(prefix_2, frontTrace_1));
            solvedSB = frontFirst_1;
        }
    }
    else if (chooseShortestSecondBlockPairOrder()) {
        const prefix_3 = solutionTrace();
        const backFirst_3 = solveRBFirst(solvedDR);
        const backTrace_3 = skip(length(prefix_3), solutionTrace());
        const perCube_1 = (count_8, trace_3) => map((cubeIndex_1) => concat(choose((tupledArg_11) => {
            if ((tupledArg_11[0] % count_8) === cubeIndex_1) {
                return tupledArg_11[1];
            }
            else {
                return void 0;
            }
        }, mapIndexed((index_4, tupledArg_10) => [index_4, tupledArg_10[1]], trace_3))), toList(rangeDouble(0, 1, count_8 - 1)));
        const tryFront_1 = (inputs_1) => {
            solutionTrace(prefix_3);
            try {
                return map2((cube_32, steps_7) => [cube_32, steps_7], solveRFFirst(inputs_1), perCube_1(length(inputs_1), skip(length(prefix_3), solutionTrace())));
            }
            catch (matchValue_4) {
                let matchResult_1;
                if (!isEmpty(inputs_1)) {
                    if (isEmpty(tail(inputs_1))) {
                        matchResult_1 = 0;
                    }
                    else {
                        matchResult_1 = 1;
                    }
                }
                else {
                    matchResult_1 = 1;
                }
                switch (matchResult_1) {
                    case 0:
                        return singleton(void 0);
                    default: {
                        const patternInput_1 = splitAt(~~(length(inputs_1) / 2), inputs_1);
                        return append(tryFront_1(patternInput_1[0]), tryFront_1(patternInput_1[1]));
                    }
                }
            }
        };
        const matchingAlgorithms = (patterns_3, cube_33) => defaultArg(map_2((tupledArg_13) => {
            const values_3 = tupledArg_13[3];
            return List_distinct(collect((steps_8) => {
                const equivalents_1 = cons(steps_8, choose((x_10) => x_10, mapIndexed((index_6, step_1) => {
                    let _arg_21;
                    return map_2((replacement_1) => mapIndexed((candidateIndex_1, candidate_2) => {
                        if (candidateIndex_1 === index_6) {
                            return replacement_1;
                        }
                        else {
                            return candidate_2;
                        }
                    }, steps_8), (_arg_21 = step_1, (_arg_21.tag === 1) ? ((_arg_21.fields[0].tag === 18) ? (new Step(1, [new Move(21, [])])) : ((_arg_21.fields[0].tag === 19) ? (new Step(1, [new Move(22, [])])) : ((_arg_21.fields[0].tag === 20) ? (new Step(1, [new Move(23, [])])) : ((_arg_21.fields[0].tag === 21) ? (new Step(1, [new Move(18, [])])) : ((_arg_21.fields[0].tag === 22) ? (new Step(1, [new Move(19, [])])) : ((_arg_21.fields[0].tag === 23) ? (new Step(1, [new Move(20, [])])) : void 0)))))) : void 0));
                }, steps_8)));
                if (orientCentersWithSecondBlock()) {
                    return append(equivalents_1, collect((candidate_3) => ofArray([append(candidate_3, singleton(new Step(1, [new Move(36, [])]))), append(candidate_3, singleton(new Step(1, [new Move(37, [])])))]), equivalents_1));
                }
                else {
                    return equivalents_1;
                }
            }, isEmpty(values_3) ? singleton(empty()) : map(stringToSteps, values_3)), {
                Equals: equals,
                GetHashCode: safeHash,
            });
        }, tryFind((tupledArg_12) => {
            const _arg_23 = tupledArg_12[2];
            return tupledArg_12[0](cube_33)([_arg_23[0], _arg_23[1], _arg_23[2]]);
        }, patterns_3)), empty());
        const frontOptions_1 = ((rfPairLevel() > 0) && (rbPairLevel() > 0)) ? map((cube_34) => tryHead(sortBy((arg_3) => length(arg_3[1]), choose((rfMoves) => {
            const rfSolved = executeSteps(rfMoves, cube_34);
            if (!caseRFPair(rfSolved)) {
                return void 0;
            }
            else {
                return tryHead(sortBy((arg_2) => length(arg_2[1]), choose((rbMoves) => {
                    const solved_4 = executeSteps(rbMoves, rfSolved);
                    if (finishingGoal(solved_4)) {
                        return [solved_4, append(rfMoves, rbMoves)];
                    }
                    else {
                        return void 0;
                    }
                }, matchingAlgorithms(rbPairLastIntermediatePatterns, rfSolved)), {
                    Compare: comparePrimitives,
                }));
            }
        }, matchingAlgorithms(rfPairFirstIntermediatePatterns, cube_34)), {
            Compare: comparePrimitives,
        })), solvedDR) : tryFront_1(solvedDR);
        const backMoves_1 = perCube_1(numCubes, backTrace_3);
        const matchValue_5 = tryFindIndex((option_9) => (option_9 == null), frontOptions_1);
        if (matchValue_5 == null) {
        }
        else {
            throw new Error(`Missing front-first second-block pattern for state: ${cubeToString(item(matchValue_5, solvedDR))}`);
        }
        const choices_1 = map2((back_3, front_3) => (length(value_1(front_3)[1]) < length(back_3)), backMoves_1, frontOptions_1);
        solutionTrace(append(prefix_3, map((steps_9) => ["SecondBlockPairs", steps_9], map3((useFront_2, back_4, front_4) => (useFront_2 ? value_1(front_4)[1] : back_4), choices_1, backMoves_1, frontOptions_1))));
        solvedSB = map3((useFront_3, back_5, front_5) => (useFront_3 ? value_1(front_5)[0] : back_5), choices_1, backFirst_3, frontOptions_1);
    }
    else {
        solvedSB = solveRBFirst(solvedDR);
    }
    stageStats("SB", numCubes);
    progressCallback()("Second block");
    const solvedCP = solve(ofArray([new Step(1, [new Move(0, [])]), new Step(1, [new Move(1, [])]), new Step(1, [new Move(2, [])]), new Step(1, [new Move(18, [])]), new Step(1, [new Move(19, [])]), new Step(1, [new Move(20, [])]), new Step(1, [new Move(24, [])]), new Step(1, [new Move(25, [])]), new Step(1, [new Move(26, [])])]), "Permute corners (CP)", "CornerPermutation", caseCP, solve(moves, "Orient corners (CO)", "CornerOrientation", caseCO, solvedSB, false), true);
    stageStats("CMLL", numCubes);
    progressCallback()("CMLL");
    const solvedCenterO = (level === 0) ? solve(mMoves, "Orient center", "CenterOrientation", caseCenterO, solvedCP, false) : solvedCP;
    const muMoves = append(mMoves, ofArray([new Step(1, [new Move(0, [])]), new Step(1, [new Move(1, [])]), new Step(1, [new Move(2, [])])]));
    const caseEO = (c_4) => {
        if (((((caseCenterO(c_4) && (equals(look(new Face(0, []), new Sticker(3, []), c_4), new Color(2, [])) ? true : equals(look(new Face(0, []), new Sticker(3, []), c_4), new Color(3, [])))) && (equals(look(new Face(0, []), new Sticker(1, []), c_4), new Color(2, [])) ? true : equals(look(new Face(0, []), new Sticker(1, []), c_4), new Color(3, [])))) && (equals(look(new Face(0, []), new Sticker(5, []), c_4), new Color(2, [])) ? true : equals(look(new Face(0, []), new Sticker(5, []), c_4), new Color(3, [])))) && (equals(look(new Face(0, []), new Sticker(7, []), c_4), new Color(2, [])) ? true : equals(look(new Face(0, []), new Sticker(7, []), c_4), new Color(3, [])))) && (equals(look(new Face(1, []), new Sticker(1, []), c_4), new Color(2, [])) ? true : equals(look(new Face(1, []), new Sticker(1, []), c_4), new Color(3, [])))) {
            if (equals(look(new Face(1, []), new Sticker(7, []), c_4), new Color(2, []))) {
                return true;
            }
            else {
                return equals(look(new Face(1, []), new Sticker(7, []), c_4), new Color(3, []));
            }
        }
        else {
            return false;
        }
    };
    const caseLtoDF = (c_5) => {
        if (caseEO(c_5)) {
            return equals(look(new Face(4, []), new Sticker(7, []), c_5), new Color(4, []));
        }
        else {
            return false;
        }
    };
    const caseLRSolved = (c_8) => {
        if (((((caseEO(c_8) && equals(look(new Face(2, []), new Sticker(1, []), c_8), new Color(4, []))) && equals(look(new Face(3, []), new Sticker(1, []), c_8), new Color(5, []))) && equals(look(new Face(2, []), new Sticker(0, []), c_8), new Color(4, []))) && equals(look(new Face(3, []), new Sticker(2, []), c_8), new Color(5, []))) && equals(look(new Face(2, []), new Sticker(2, []), c_8), new Color(4, []))) {
            return equals(look(new Face(3, []), new Sticker(0, []), c_8), new Color(5, []));
        }
        else {
            return false;
        }
    };
    let solvedEO;
    if (useEolr()) {
        const directEoAlgorithms = map((algorithm_10) => {
            if (isNullOrWhiteSpace(algorithm_10)) {
                return empty();
            }
            else {
                return stringToSteps(algorithm_10);
            }
        }, List_distinct(collect((tupledArg_14) => {
            const algorithms_3 = tupledArg_14[3];
            if (isEmpty(algorithms_3)) {
                return singleton("");
            }
            else {
                return algorithms_3;
            }
        }, edgeIntermediateOrientationPatters), {
            Equals: (x_14, y_12) => (x_14 === y_12),
            GetHashCode: stringHash,
        }));
        const eoCandidates = List_distinct(toList(delay(() => collect_1((auf_4) => map_1((algorithm_11) => append(auf_4, algorithm_11), directEoAlgorithms), [empty(), singleton(new Step(1, [new Move(0, [])])), singleton(new Step(1, [new Move(1, [])])), singleton(new Step(1, [new Move(2, [])]))]))), {
            Equals: equals,
            GetHashCode: safeHash,
        });
        solvedEO = map((cube_35) => {
            const algorithm_12 = minBy(length, choose((eoAlgorithm) => {
                const oriented = executeSteps(eoAlgorithm, cube_35);
                if (!caseEO(oriented)) {
                    return void 0;
                }
                else {
                    return append(eoAlgorithm, head(solveWithStepsBy((state) => (`${look(new Face(0, []), new Sticker(4, []), state)}${look(new Face(1, []), new Sticker(4, []), state)}${look(new Face(4, []), new Sticker(4, []), state)}${look(new Face(5, []), new Sticker(4, []), state)}${piecesToString(state, ofArray([new Piece(1, [new Edge(0, [])]), new Piece(1, [new Edge(1, [])]), new Piece(1, [new Edge(2, [])]), new Piece(1, [new Edge(3, [])]), new Piece(1, [new Edge(6, [])]), new Piece(1, [new Edge(7, [])])]))}`), muMoves, (c_9) => {
                        let c_7, front_6, back_6;
                        if ((c_7 = c_9, (front_6 = look(new Face(4, []), new Sticker(7, []), c_7), (back_6 = look(new Face(5, []), new Sticker(1, []), c_7), caseEO(c_7) && ((equals(front_6, new Color(4, [])) && equals(back_6, new Color(5, []))) ? true : (equals(front_6, new Color(5, [])) && equals(back_6, new Color(4, [])))))))) {
                            return true;
                        }
                        else {
                            return caseLRSolved(c_9);
                        }
                    }, oriented)));
                }
            }, eoCandidates), {
                Compare: comparePrimitives,
            });
            solutionTrace(append(solutionTrace(), singleton(["EOLR", algorithm_12])));
            return executeSteps(algorithm_12, cube_35);
        }, solvedCenterO);
    }
    else {
        solvedEO = solve(muMoves, "Orient edges (EO)", "EdgeOrientation", caseEO, solvedCenterO, true);
    }
    stageStats("EO", numCubes);
    progressCallback()(useEolr() ? "EOLR" : "Edge orientation");
    const solvedLR = useEolr() ? solve(muMoves, "LR edges solved", "LREdges", caseLRSolved, solvedEO, true) : ((level === 0) ? solve(muMoves, "LR edges solved", "LREdges", caseLRSolved, solve(muMoves, "LR edges to bottom", "LREdgesBottom", (c_6) => {
        if (caseLtoDF(c_6)) {
            return equals(look(new Face(5, []), new Sticker(1, []), c_6), new Color(5, []));
        }
        else {
            return false;
        }
    }, solve(muMoves, "L edge to DF", "LToDF", caseLtoDF, solvedEO, false), false), true) : solve(muMoves, "LR edges solved", "LREdges", caseLRSolved, solvedEO, true));
    stageStats("LR", numCubes);
    progressCallback()("Last six edges");
    const solved_6 = solve(ofArray([new Step(1, [new Move(36, [])]), new Step(1, [new Move(37, [])]), new Step(1, [new Move(38, [])]), new Step(1, [new Move(2, [])]), new Step(1, [new Move(8, [])])]), "Last 4 edges -> Solved!", "L4E", (cube_36) => {
        const solved_5 = (face, color) => forAll((_arg_31, col) => equals(col, color), find(face, cube_36));
        if ((((solved_5(new Face(4, []), new Color(0, [])) && solved_5(new Face(5, []), new Color(1, []))) && solved_5(new Face(2, []), new Color(4, []))) && solved_5(new Face(3, []), new Color(5, []))) && solved_5(new Face(0, []), new Color(3, []))) {
            return solved_5(new Face(1, []), new Color(2, []));
        }
        else {
            return false;
        }
    }, solvedLR, true);
    stageStats("L4E", numCubes);
    progressCallback()("Last four edges");
}

export function generate(numCubes) {
    generateFrom(initScrambledCubes(numCubes));
}

