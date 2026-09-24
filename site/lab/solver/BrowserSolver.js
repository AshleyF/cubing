import { FSharpException, Record } from "./fable_modules/fable-library-js.4.16.0/Types.js";
import { class_type, array_type, string_type, record_type, int32_type } from "./fable_modules/fable-library-js.4.16.0/Reflection.js";
import { setData } from "./PatternData.js";
import { installPolicy } from "./library/Lse.js";
import { stringToSteps, stepsToString } from "./library/Render.js";
import { filter, length, sumBy, isEmpty, append, mapIndexed, minBy, ofArray, singleton, empty, map, toArray, collect } from "./fable_modules/fable-library-js.4.16.0/List.js";
import { x2yColorNeutral, chooseShortestSecondBlockPairOrder, chooseShortestFirstBlockPairOrder, orientCentersWithSecondBlock, rfPairLevel, rbPairLevel, lfPairLevel, lbPairLevel, useOptimalLse, useEolr, edgeOrientationLevel, fullCmll, cornerPermutationLevel, cornerOrientationLevel } from "./Utility.js";
import { Color, Face, Sticker, look, solved, executeSteps } from "./library/Cube.js";
import { solutionTrace } from "./library/Solver.js";
import { progressCallback, generateFrom } from "./Roux.js";
import { FSharpSet__Contains, ofSeq } from "./fable_modules/fable-library-js.4.16.0/Set.js";
import { compare, comparePrimitives } from "./fable_modules/fable-library-js.4.16.0/Util.js";
import { find, map as map_1, ofList } from "./fable_modules/fable-library-js.4.16.0/Map.js";

export class Config extends Record {
    constructor(co, cp, cmll, eo, lb, lf, rb, rf, center, fbOrder, sbOrder, colorNeutral, lse) {
        super();
        this.co = (co | 0);
        this.cp = (cp | 0);
        this.cmll = (cmll | 0);
        this.eo = (eo | 0);
        this.lb = (lb | 0);
        this.lf = (lf | 0);
        this.rb = (rb | 0);
        this.rf = (rf | 0);
        this.center = (center | 0);
        this.fbOrder = (fbOrder | 0);
        this.sbOrder = (sbOrder | 0);
        this.colorNeutral = (colorNeutral | 0);
        this.lse = (lse | 0);
    }
}

export function Config_$reflection() {
    return record_type("BrowserSolver.Config", [], Config, () => [["co", int32_type], ["cp", int32_type], ["cmll", int32_type], ["eo", int32_type], ["lb", int32_type], ["lf", int32_type], ["rb", int32_type], ["rf", int32_type], ["center", int32_type], ["fbOrder", int32_type], ["sbOrder", int32_type], ["colorNeutral", int32_type], ["lse", int32_type]]);
}

export class StageResult extends Record {
    constructor(stage, moves) {
        super();
        this.stage = stage;
        this.moves = moves;
    }
}

export function StageResult_$reflection() {
    return record_type("BrowserSolver.StageResult", [], StageResult, () => [["stage", string_type], ["moves", string_type]]);
}

export class SolveResult extends Record {
    constructor(solution, stages) {
        super();
        this.solution = solution;
        this.stages = stages;
    }
}

export function SolveResult_$reflection() {
    return record_type("BrowserSolver.SolveResult", [], SolveResult, () => [["solution", string_type], ["stages", array_type(StageResult_$reflection())]]);
}

export class FirstBlockComplete extends FSharpException {
    constructor() {
        super();
    }
}

export function FirstBlockComplete_$reflection() {
    return class_type("BrowserSolver.FirstBlockComplete", void 0, FirstBlockComplete, class_type("System.Exception"));
}

export function setPatterns(keys, values) {
    setData(keys, values);
}

export function setLsePolicy(distances, optimalMoves, reachable, maximum) {
    installPolicy(distances, optimalMoves, reachable, maximum);
}

function resultFromTrace(trace) {
    return new SolveResult(stepsToString(collect((tuple) => tuple[1], trace)), toArray(map((tupledArg) => (new StageResult(tupledArg[0], stepsToString(tupledArg[1]))), trace)));
}

export function solveWithProgress(scramble, config, progress) {
    cornerOrientationLevel(config.co);
    cornerPermutationLevel(config.cp);
    fullCmll(config.cmll === 1);
    edgeOrientationLevel((config.eo === 1) ? 1 : 0);
    useEolr(config.eo === 2);
    useOptimalLse(config.lse === 1);
    lbPairLevel(config.lb);
    lfPairLevel(config.lf);
    rbPairLevel(config.rb);
    rfPairLevel(config.rf);
    orientCentersWithSecondBlock(config.center === 1);
    chooseShortestFirstBlockPairOrder(config.fbOrder === 1);
    chooseShortestSecondBlockPairOrder(config.sbOrder === 1);
    x2yColorNeutral(config.colorNeutral === 1);
    const scrambled = executeSteps(stringToSteps(scramble), solved);
    const solveOne = (cube) => {
        solutionTrace(empty());
        generateFrom(singleton(cube));
        return solutionTrace();
    };
    let trace;
    if (!x2yColorNeutral()) {
        progressCallback((milestone) => {
            progress(milestone, resultFromTrace(solutionTrace()));
        });
        trace = solveOne(scrambled);
    }
    else {
        const orientations = map((algorithm) => {
            if (algorithm === "") {
                return empty();
            }
            else {
                return stringToSteps(algorithm);
            }
        }, ofArray(["", "y", "y2", "y\'", "x2", "x2 y", "x2 y2", "x2 y\'"]));
        const firstBlockStages = ofSeq(["ColorNeutralOrientation", "DLEdge", "LCenter", "TuckLBtoFD", "BringDLBtoU", "InsertLBPair", "TuckLFtoBD", "BringDLFtoURF", "InsertLFPair", "TuckLFFirsttoBD", "BringDLFFirsttoURF", "InsertLFFirstPair", "TuckLBLasttoFD", "BringDLBLasttoU", "InsertLBLastPair"], {
            Compare: comparePrimitives,
        });
        const patternInput = minBy((tupledArg_1) => tupledArg_1[3], mapIndexed((index, orientation) => {
            let candidateCube;
            const cube_1 = executeSteps(orientation, scrambled);
            const colorMap = ofList(map((tupledArg) => [look(tupledArg[0], new Sticker(4, []), cube_1), tupledArg[1]], ofArray([[new Face(0, []), new Color(2, [])], [new Face(1, []), new Color(3, [])], [new Face(2, []), new Color(1, [])], [new Face(3, []), new Color(0, [])], [new Face(4, []), new Color(5, [])], [new Face(5, []), new Color(4, [])]])), {
                Compare: compare,
            });
            candidateCube = map_1((_arg, face_1) => map_1((_arg_1, color) => find(color, colorMap), face_1), cube_1);
            let candidateTrace;
            let firstBlockTrace = empty();
            solutionTrace(empty());
            progressCallback((milestone_1) => {
                if (milestone_1 === "First block") {
                    firstBlockTrace = solutionTrace();
                    throw new FirstBlockComplete();
                }
            });
            try {
                generateFrom(singleton(candidateCube));
            }
            catch (matchValue) {
                if (matchValue instanceof FirstBlockComplete) {
                }
                else {
                    throw matchValue;
                }
            }
            candidateTrace = firstBlockTrace;
            progress(`Inspection ${index + 1}/8`, resultFromTrace(append(isEmpty(orientation) ? empty() : singleton(["ColorNeutralOrientation", orientation]), candidateTrace)));
            return [orientation, candidateCube, candidateTrace, sumBy((arg_1) => length(arg_1[1]), filter((arg) => FSharpSet__Contains(firstBlockStages, arg[0]), candidateTrace), {
                GetZero: () => 0,
                Add: (x_2, y_2) => (x_2 + y_2),
            })];
        }, orientations), {
            Compare: comparePrimitives,
        });
        const orientation_1 = patternInput[0];
        const orientationTrace = isEmpty(orientation_1) ? empty() : singleton(["ColorNeutralOrientation", orientation_1]);
        progress("First block", resultFromTrace(append(orientationTrace, patternInput[2])));
        progressCallback((milestone_2) => {
            if (milestone_2 !== "First block") {
                progress(milestone_2, resultFromTrace(append(orientationTrace, solutionTrace())));
            }
        });
        trace = append(orientationTrace, solveOne(patternInput[1]));
    }
    solutionTrace(trace);
    progressCallback((value_1) => {
    });
    return resultFromTrace(trace);
}

export function solve(scramble, config) {
    return solveWithProgress(scramble, config, (_arg, _arg_1) => {
    });
}

