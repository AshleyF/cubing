import { stringHash, comparePrimitives, safeHash, equals, disposeSafe, getEnumerator, createAtom } from "../fable_modules/fable-library-js.4.16.0/Util.js";
import { initialize as initialize_1, concat as concat_1, map as map_2, zip, mapIndexed, choose, collect, tryFind as tryFind_1, head as head_1, tail, isEmpty, ofSeq, filter, tryPick, reverse as reverse_1, ofArray, append, singleton, cons, length, item, empty } from "../fable_modules/fable-library-js.4.16.0/List.js";
import { rotateZ2, rotateZ$0027, rotateZ, rotateY2, rotateY$0027, rotateY, rotateX2, rotateX$0027, rotateX, stageCount, Step, executeStep, step as step_1, Move, solved as solved_1, move, executeSteps } from "./Cube.js";
import { nonSeeded } from "../fable_modules/fable-library-js.4.16.0/Random.js";
import { empty as empty_2, singleton as singleton_1, append as append_1, delay, concat, initialize, iterate, sort, length as length_1, map as map_1, fold, filter as filter_1, tryHead, tryFind, ofList, forAll2, forAll, head, reverse } from "../fable_modules/fable-library-js.4.16.0/Seq.js";
import { stepsToString, stringToSteps, cubeToString, moveToString, rotationToString } from "./Render.js";
import { addToSet, tryGetValue } from "../fable_modules/fable-library-js.4.16.0/MapUtil.js";
import { FSharpRef } from "../fable_modules/fable-library-js.4.16.0/Types.js";
import { map, toArray } from "../fable_modules/fable-library-js.4.16.0/Option.js";
import { Queue$1__Dequeue, Queue$1__Enqueue_2B595, Queue$1_$ctor } from "../fable_modules/fable-library-js.4.16.0/System.Collections.Generic.js";
import { join, toConsole, printf, toText } from "../fable_modules/fable-library-js.4.16.0/String.js";
import { distinct as distinct_1, List_distinct } from "../fable_modules/fable-library-js.4.16.0/Seq2.js";
import { toList, empty as empty_1, add, tryFind as tryFind_2 } from "../fable_modules/fable-library-js.4.16.0/Map.js";

export const quiet = true;

export const warnings = true;

export let solutionTrace = createAtom(empty());

export let preferGoalMatchingAlgorithm = createAtom(false);

export function executeAndReportSteps(steps) {
    return (cube) => executeSteps(steps, cube);
}

export function scrambleWithMoves(moves, n) {
    const rand = nonSeeded();
    const scramble$0027 = (cube_mut, sequence_mut, history_mut, n_1_mut) => {
        scramble$0027:
        while (true) {
            const cube = cube_mut, sequence = sequence_mut, history = history_mut, n_1 = n_1_mut;
            if (n_1 === 0) {
                return [cube, reverse(sequence)];
            }
            else {
                const m = item(rand.Next1(length(moves)), moves);
                const cube$0027 = move(m)(cube);
                cube_mut = cube$0027;
                sequence_mut = cons(m, sequence);
                history_mut = cons(cube$0027, history);
                n_1_mut = (n_1 - 1);
                continue scramble$0027;
            }
            break;
        }
    };
    return scramble$0027(solved_1, empty(), singleton(solved_1), n);
}

export const scramble = (() => {
    const moves = append(ofArray([new Move(0, []), new Move(1, []), new Move(2, []), new Move(6, []), new Move(7, []), new Move(8, []), new Move(12, []), new Move(13, []), new Move(14, []), new Move(18, []), new Move(19, []), new Move(20, []), new Move(24, []), new Move(25, []), new Move(26, []), new Move(30, []), new Move(31, []), new Move(32, [])]), append(ofArray([new Move(36, []), new Move(37, []), new Move(38, [])]), ofArray([new Move(39, []), new Move(40, []), new Move(41, []), new Move(42, []), new Move(43, []), new Move(44, [])])));
    return (n) => scrambleWithMoves(moves, n);
})();

export function solveWithStepsBy(keyOf, includedSteps, check, cube) {
    const slice = (_arg) => {
        if (_arg.tag === 0) {
            return head(rotationToString(_arg.fields[0]).split(""));
        }
        else {
            return head(moveToString(_arg.fields[0]).split(""));
        }
    };
    const atDepth = (visited, remaining, previous, reversed, current) => {
        if (check(current)) {
            return reverse_1(reversed);
        }
        else if (remaining === 0) {
            return void 0;
        }
        else {
            const key = `${keyOf(current)}|${previous}`;
            let matchValue;
            let outArg = 0;
            matchValue = [tryGetValue(visited, key, new FSharpRef(() => outArg, (v) => {
                outArg = (v | 0);
            })), outArg];
            let matchResult;
            if (matchValue[0]) {
                if (matchValue[1] >= remaining) {
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
                    return void 0;
                default: {
                    visited.set(key, remaining);
                    return tryPick((candidate_1) => atDepth(visited, remaining - 1, candidate_1, cons(candidate_1, reversed), step_1(candidate_1)(current)), filter((candidate) => forAll((prior) => (slice(prior) !== slice(candidate)), toArray(previous)), includedSteps));
                }
            }
        }
    };
    const deepen = (depth_mut) => {
        deepen:
        while (true) {
            const depth = depth_mut;
            const matchValue_1 = atDepth(new Map([]), depth, void 0, empty(), cube);
            if (matchValue_1 == null) {
                depth_mut = (depth + 1);
                continue deepen;
            }
            else {
                return singleton(matchValue_1);
            }
            break;
        }
    };
    return deepen(0);
}

export function solveWithSteps(includedSteps, check, cube) {
    return solveWithStepsBy(cubeToString, includedSteps, check, cube);
}

export function solveShortestWithStepsBy(keyOf, includedSteps, check, cube) {
    const slice = (_arg) => {
        if (_arg.tag === 0) {
            return head(rotationToString(_arg.fields[0]).split(""));
        }
        else {
            return head(moveToString(_arg.fields[0]).split(""));
        }
    };
    const queue = Queue$1_$ctor();
    const visited = new Set([]);
    Queue$1__Enqueue_2B595(queue, [cube, empty(), void 0]);
    addToSet(`${keyOf(cube)}|`, visited);
    const search = () => {
        const patternInput = Queue$1__Dequeue(queue);
        const reversed = patternInput[1];
        const current = patternInput[0];
        if (check(current)) {
            return reverse_1(reversed);
        }
        else {
            const enumerator = getEnumerator(includedSteps);
            try {
                while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
                    const candidate = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
                    if (forAll((prior) => (prior !== slice(candidate)), toArray(patternInput[2]))) {
                        const next = step_1(candidate)(current);
                        if (addToSet(`${keyOf(next)}|${slice(candidate)}`, visited)) {
                            Queue$1__Enqueue_2B595(queue, [next, cons(candidate, reversed), slice(candidate)]);
                        }
                    }
                }
            }
            finally {
                disposeSafe(enumerator);
            }
            return search();
        }
    };
    return search();
}

export function solveShortestWithSteps(includedSteps, check, cube) {
    return solveShortestWithStepsBy(cubeToString, includedSteps, check, cube);
}

let lastMatchedCube = void 0;

let lastMatchedCubeString = "";

export function matchesGeneric(cube, pattern_, pattern__1, pattern__2) {
    const pattern = [pattern_, pattern__1, pattern__2];
    let cubeString;
    let matchResult, cached_1;
    if (lastMatchedCube != null) {
        if (lastMatchedCube === cube) {
            matchResult = 0;
            cached_1 = lastMatchedCube;
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
            cubeString = lastMatchedCubeString;
            break;
        }
        default: {
            const rendered = cubeToString(cube);
            lastMatchedCube = cube;
            lastMatchedCubeString = rendered;
            cubeString = rendered;
        }
    }
    const matchPat = (p_1) => forAll2((p, c) => {
        if ((((p === ".") ? true : (p === c)) ? true : (((p === "P") && (c !== "W")) && (c !== "Y"))) ? true : ((p === "E") && ((c === "W") ? true : (c === "Y")))) {
            return true;
        }
        else if (p === "*") {
            if (c === "B") {
                return true;
            }
            else {
                return c === "G";
            }
        }
        else {
            return false;
        }
    }, p_1, cubeString.split(""));
    const rotateCorners = (p_2) => {
        const matchValue = ofSeq(p_2);
        let matchResult_1, b0, b1, b2, b3, b4, b5, b6, b7, b8, d0, d1, d2, d3, d4, d5, d6, d7, d8, f0, f1, f2, f3, f4, f5, f6, f7, f8, l0, l1, l2, l3, l4, l5, l6, l7, l8, r0, r1, r2, r3, r4, r5, r6, r7, r8, u0, u1, u2, u3, u4, u5, u6, u7, u8, invalid;
        if (!isEmpty(matchValue)) {
            if (!isEmpty(tail(matchValue))) {
                if (!isEmpty(tail(tail(matchValue)))) {
                    if (!isEmpty(tail(tail(tail(matchValue))))) {
                        if (!isEmpty(tail(tail(tail(tail(matchValue)))))) {
                            if (!isEmpty(tail(tail(tail(tail(tail(matchValue))))))) {
                                if (!isEmpty(tail(tail(tail(tail(tail(tail(matchValue)))))))) {
                                    if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))) {
                                        if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))) {
                                            if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))) {
                                                if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))) {
                                                    if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))) {
                                                        if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))) {
                                                            if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))) {
                                                                if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))) {
                                                                    if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))) {
                                                                        if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))) {
                                                                            if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))) {
                                                                                if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))) {
                                                                                    if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))) {
                                                                                        if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))) {
                                                                                            if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))) {
                                                                                                if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))) {
                                                                                                    if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))) {
                                                                                                        if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))) {
                                                                                                            if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))) {
                                                                                                                if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))) {
                                                                                                                    if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))) {
                                                                                                                        if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))) {
                                                                                                                            if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))) {
                                                                                                                                if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))) {
                                                                                                                                    if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))) {
                                                                                                                                        if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))) {
                                                                                                                                            if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))))) {
                                                                                                                                                if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))))) {
                                                                                                                                                    if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                        if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                            if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                    if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                        if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                            if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                    if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                        if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                            if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                                if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                                    if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                                        if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                                            if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                                                if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                                                    if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                                                        if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                                                            if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                                                                if (isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                                                                    matchResult_1 = 0;
                                                                                                                                                                                                                                    b0 = head_1(matchValue);
                                                                                                                                                                                                                                    b1 = head_1(tail(matchValue));
                                                                                                                                                                                                                                    b2 = head_1(tail(tail(matchValue)));
                                                                                                                                                                                                                                    b3 = head_1(tail(tail(tail(matchValue))));
                                                                                                                                                                                                                                    b4 = head_1(tail(tail(tail(tail(matchValue)))));
                                                                                                                                                                                                                                    b5 = head_1(tail(tail(tail(tail(tail(matchValue))))));
                                                                                                                                                                                                                                    b6 = head_1(tail(tail(tail(tail(tail(tail(matchValue)))))));
                                                                                                                                                                                                                                    b7 = head_1(tail(tail(tail(tail(tail(tail(tail(matchValue))))))));
                                                                                                                                                                                                                                    b8 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))));
                                                                                                                                                                                                                                    d0 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    d1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    d2 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    d3 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    d4 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    d5 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    d6 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    d7 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    d8 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    f0 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))));
                                                                                                                                                                                                                                    f1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))));
                                                                                                                                                                                                                                    f2 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))));
                                                                                                                                                                                                                                    f3 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    f4 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    f5 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    f6 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    f7 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    f8 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    l0 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))));
                                                                                                                                                                                                                                    l1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))));
                                                                                                                                                                                                                                    l2 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))));
                                                                                                                                                                                                                                    l3 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    l4 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    l5 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    l6 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    l7 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    l8 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    r0 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))));
                                                                                                                                                                                                                                    r1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))));
                                                                                                                                                                                                                                    r2 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))));
                                                                                                                                                                                                                                    r3 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    r4 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    r5 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    r6 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    r7 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    r8 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    u0 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))));
                                                                                                                                                                                                                                    u1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))));
                                                                                                                                                                                                                                    u2 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))));
                                                                                                                                                                                                                                    u3 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))));
                                                                                                                                                                                                                                    u4 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))));
                                                                                                                                                                                                                                    u5 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))));
                                                                                                                                                                                                                                    u6 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))));
                                                                                                                                                                                                                                    u7 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))));
                                                                                                                                                                                                                                    u8 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))));
                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                else {
                                                                                                                                                                                                                                    matchResult_1 = 1;
                                                                                                                                                                                                                                    invalid = matchValue;
                                                                                                                                                                                                                                }
                                                                                                                                                                                                                            }
                                                                                                                                                                                                                            else {
                                                                                                                                                                                                                                matchResult_1 = 1;
                                                                                                                                                                                                                                invalid = matchValue;
                                                                                                                                                                                                                            }
                                                                                                                                                                                                                        }
                                                                                                                                                                                                                        else {
                                                                                                                                                                                                                            matchResult_1 = 1;
                                                                                                                                                                                                                            invalid = matchValue;
                                                                                                                                                                                                                        }
                                                                                                                                                                                                                    }
                                                                                                                                                                                                                    else {
                                                                                                                                                                                                                        matchResult_1 = 1;
                                                                                                                                                                                                                        invalid = matchValue;
                                                                                                                                                                                                                    }
                                                                                                                                                                                                                }
                                                                                                                                                                                                                else {
                                                                                                                                                                                                                    matchResult_1 = 1;
                                                                                                                                                                                                                    invalid = matchValue;
                                                                                                                                                                                                                }
                                                                                                                                                                                                            }
                                                                                                                                                                                                            else {
                                                                                                                                                                                                                matchResult_1 = 1;
                                                                                                                                                                                                                invalid = matchValue;
                                                                                                                                                                                                            }
                                                                                                                                                                                                        }
                                                                                                                                                                                                        else {
                                                                                                                                                                                                            matchResult_1 = 1;
                                                                                                                                                                                                            invalid = matchValue;
                                                                                                                                                                                                        }
                                                                                                                                                                                                    }
                                                                                                                                                                                                    else {
                                                                                                                                                                                                        matchResult_1 = 1;
                                                                                                                                                                                                        invalid = matchValue;
                                                                                                                                                                                                    }
                                                                                                                                                                                                }
                                                                                                                                                                                                else {
                                                                                                                                                                                                    matchResult_1 = 1;
                                                                                                                                                                                                    invalid = matchValue;
                                                                                                                                                                                                }
                                                                                                                                                                                            }
                                                                                                                                                                                            else {
                                                                                                                                                                                                matchResult_1 = 1;
                                                                                                                                                                                                invalid = matchValue;
                                                                                                                                                                                            }
                                                                                                                                                                                        }
                                                                                                                                                                                        else {
                                                                                                                                                                                            matchResult_1 = 1;
                                                                                                                                                                                            invalid = matchValue;
                                                                                                                                                                                        }
                                                                                                                                                                                    }
                                                                                                                                                                                    else {
                                                                                                                                                                                        matchResult_1 = 1;
                                                                                                                                                                                        invalid = matchValue;
                                                                                                                                                                                    }
                                                                                                                                                                                }
                                                                                                                                                                                else {
                                                                                                                                                                                    matchResult_1 = 1;
                                                                                                                                                                                    invalid = matchValue;
                                                                                                                                                                                }
                                                                                                                                                                            }
                                                                                                                                                                            else {
                                                                                                                                                                                matchResult_1 = 1;
                                                                                                                                                                                invalid = matchValue;
                                                                                                                                                                            }
                                                                                                                                                                        }
                                                                                                                                                                        else {
                                                                                                                                                                            matchResult_1 = 1;
                                                                                                                                                                            invalid = matchValue;
                                                                                                                                                                        }
                                                                                                                                                                    }
                                                                                                                                                                    else {
                                                                                                                                                                        matchResult_1 = 1;
                                                                                                                                                                        invalid = matchValue;
                                                                                                                                                                    }
                                                                                                                                                                }
                                                                                                                                                                else {
                                                                                                                                                                    matchResult_1 = 1;
                                                                                                                                                                    invalid = matchValue;
                                                                                                                                                                }
                                                                                                                                                            }
                                                                                                                                                            else {
                                                                                                                                                                matchResult_1 = 1;
                                                                                                                                                                invalid = matchValue;
                                                                                                                                                            }
                                                                                                                                                        }
                                                                                                                                                        else {
                                                                                                                                                            matchResult_1 = 1;
                                                                                                                                                            invalid = matchValue;
                                                                                                                                                        }
                                                                                                                                                    }
                                                                                                                                                    else {
                                                                                                                                                        matchResult_1 = 1;
                                                                                                                                                        invalid = matchValue;
                                                                                                                                                    }
                                                                                                                                                }
                                                                                                                                                else {
                                                                                                                                                    matchResult_1 = 1;
                                                                                                                                                    invalid = matchValue;
                                                                                                                                                }
                                                                                                                                            }
                                                                                                                                            else {
                                                                                                                                                matchResult_1 = 1;
                                                                                                                                                invalid = matchValue;
                                                                                                                                            }
                                                                                                                                        }
                                                                                                                                        else {
                                                                                                                                            matchResult_1 = 1;
                                                                                                                                            invalid = matchValue;
                                                                                                                                        }
                                                                                                                                    }
                                                                                                                                    else {
                                                                                                                                        matchResult_1 = 1;
                                                                                                                                        invalid = matchValue;
                                                                                                                                    }
                                                                                                                                }
                                                                                                                                else {
                                                                                                                                    matchResult_1 = 1;
                                                                                                                                    invalid = matchValue;
                                                                                                                                }
                                                                                                                            }
                                                                                                                            else {
                                                                                                                                matchResult_1 = 1;
                                                                                                                                invalid = matchValue;
                                                                                                                            }
                                                                                                                        }
                                                                                                                        else {
                                                                                                                            matchResult_1 = 1;
                                                                                                                            invalid = matchValue;
                                                                                                                        }
                                                                                                                    }
                                                                                                                    else {
                                                                                                                        matchResult_1 = 1;
                                                                                                                        invalid = matchValue;
                                                                                                                    }
                                                                                                                }
                                                                                                                else {
                                                                                                                    matchResult_1 = 1;
                                                                                                                    invalid = matchValue;
                                                                                                                }
                                                                                                            }
                                                                                                            else {
                                                                                                                matchResult_1 = 1;
                                                                                                                invalid = matchValue;
                                                                                                            }
                                                                                                        }
                                                                                                        else {
                                                                                                            matchResult_1 = 1;
                                                                                                            invalid = matchValue;
                                                                                                        }
                                                                                                    }
                                                                                                    else {
                                                                                                        matchResult_1 = 1;
                                                                                                        invalid = matchValue;
                                                                                                    }
                                                                                                }
                                                                                                else {
                                                                                                    matchResult_1 = 1;
                                                                                                    invalid = matchValue;
                                                                                                }
                                                                                            }
                                                                                            else {
                                                                                                matchResult_1 = 1;
                                                                                                invalid = matchValue;
                                                                                            }
                                                                                        }
                                                                                        else {
                                                                                            matchResult_1 = 1;
                                                                                            invalid = matchValue;
                                                                                        }
                                                                                    }
                                                                                    else {
                                                                                        matchResult_1 = 1;
                                                                                        invalid = matchValue;
                                                                                    }
                                                                                }
                                                                                else {
                                                                                    matchResult_1 = 1;
                                                                                    invalid = matchValue;
                                                                                }
                                                                            }
                                                                            else {
                                                                                matchResult_1 = 1;
                                                                                invalid = matchValue;
                                                                            }
                                                                        }
                                                                        else {
                                                                            matchResult_1 = 1;
                                                                            invalid = matchValue;
                                                                        }
                                                                    }
                                                                    else {
                                                                        matchResult_1 = 1;
                                                                        invalid = matchValue;
                                                                    }
                                                                }
                                                                else {
                                                                    matchResult_1 = 1;
                                                                    invalid = matchValue;
                                                                }
                                                            }
                                                            else {
                                                                matchResult_1 = 1;
                                                                invalid = matchValue;
                                                            }
                                                        }
                                                        else {
                                                            matchResult_1 = 1;
                                                            invalid = matchValue;
                                                        }
                                                    }
                                                    else {
                                                        matchResult_1 = 1;
                                                        invalid = matchValue;
                                                    }
                                                }
                                                else {
                                                    matchResult_1 = 1;
                                                    invalid = matchValue;
                                                }
                                            }
                                            else {
                                                matchResult_1 = 1;
                                                invalid = matchValue;
                                            }
                                        }
                                        else {
                                            matchResult_1 = 1;
                                            invalid = matchValue;
                                        }
                                    }
                                    else {
                                        matchResult_1 = 1;
                                        invalid = matchValue;
                                    }
                                }
                                else {
                                    matchResult_1 = 1;
                                    invalid = matchValue;
                                }
                            }
                            else {
                                matchResult_1 = 1;
                                invalid = matchValue;
                            }
                        }
                        else {
                            matchResult_1 = 1;
                            invalid = matchValue;
                        }
                    }
                    else {
                        matchResult_1 = 1;
                        invalid = matchValue;
                    }
                }
                else {
                    matchResult_1 = 1;
                    invalid = matchValue;
                }
            }
            else {
                matchResult_1 = 1;
                invalid = matchValue;
            }
        }
        else {
            matchResult_1 = 1;
            invalid = matchValue;
        }
        switch (matchResult_1) {
            case 0:
                return ofList(ofArray([b0, b1, b2, b3, b4, b5, l2, b7, l0, u6, u1, u0, u3, u4, u5, u8, u7, u2, f0, l1, f2, r0, f1, r2, b8, r1, b6, l3, l4, l5, f3, f4, f5, r3, r4, r5, l6, l7, l8, f6, f7, f8, r6, r7, r8, d0, d1, d2, d3, d4, d5, d6, d7, d8]));
            default:
                throw new Error(toText(printf("Invalid pattern: %A"))(invalid));
        }
    };
    const cycleCornerColors = (c_4, p_3) => {
        const matchValue_1 = ofSeq(p_3);
        let matchResult_2, b0_1, b1_1, b2_1, b3_1, b4_1, b5_1, b6_1, b7_1, b8_1, d0_1, d1_1, d2_1, d3_1, d4_1, d5_1, d6_1, d7_1, d8_1, f0_1, f1_1, f2_1, f3_1, f4_1, f5_1, f6_1, f7_1, f8_1, l0_1, l1_1, l2_1, l3_1, l4_1, l5_1, l6_1, l7_1, l8_1, r0_1, r1_1, r2_1, r3_1, r4_1, r5_1, r6_1, r7_1, r8_1, u0_1, u1_1, u2_1, u3_1, u4_1, u5_1, u6_1, u7_1, u8_1, invalid_1;
        if (!isEmpty(matchValue_1)) {
            if (!isEmpty(tail(matchValue_1))) {
                if (!isEmpty(tail(tail(matchValue_1)))) {
                    if (!isEmpty(tail(tail(tail(matchValue_1))))) {
                        if (!isEmpty(tail(tail(tail(tail(matchValue_1)))))) {
                            if (!isEmpty(tail(tail(tail(tail(tail(matchValue_1))))))) {
                                if (!isEmpty(tail(tail(tail(tail(tail(tail(matchValue_1)))))))) {
                                    if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))) {
                                        if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))) {
                                            if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))) {
                                                if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))) {
                                                    if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))))) {
                                                        if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))))) {
                                                            if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))))))) {
                                                                if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))))))) {
                                                                    if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))))))))) {
                                                                        if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))))))))) {
                                                                            if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))))))))))) {
                                                                                if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))))))))))) {
                                                                                    if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))))))))))))) {
                                                                                        if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))))))))))))) {
                                                                                            if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))))))))))))))) {
                                                                                                if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))))))))))))))) {
                                                                                                    if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))))))))))))))))) {
                                                                                                        if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))))))))))))))))) {
                                                                                                            if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))))))))))))))))))) {
                                                                                                                if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))))))))))))))))))) {
                                                                                                                    if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))))))))))))))))))))) {
                                                                                                                        if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))))))))))))))))))))) {
                                                                                                                            if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))))))))))))))))))))))) {
                                                                                                                                if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))))))))))))))))))))))) {
                                                                                                                                    if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))))))))))))))))))))))))) {
                                                                                                                                        if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))))))))))))))))))))))))) {
                                                                                                                                            if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))))))))))))))))))))))))))) {
                                                                                                                                                if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))))))))))))))))))))))))))) {
                                                                                                                                                    if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                        if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                            if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                    if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                        if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                            if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                    if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                        if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                            if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                                if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                                    if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                                        if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                                            if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                                                if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                                                    if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))))))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                                                        if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))))))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                                                            if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))))))))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                                                                if (isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))))))))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                                                                    matchResult_2 = 0;
                                                                                                                                                                                                                                    b0_1 = head_1(matchValue_1);
                                                                                                                                                                                                                                    b1_1 = head_1(tail(matchValue_1));
                                                                                                                                                                                                                                    b2_1 = head_1(tail(tail(matchValue_1)));
                                                                                                                                                                                                                                    b3_1 = head_1(tail(tail(tail(matchValue_1))));
                                                                                                                                                                                                                                    b4_1 = head_1(tail(tail(tail(tail(matchValue_1)))));
                                                                                                                                                                                                                                    b5_1 = head_1(tail(tail(tail(tail(tail(matchValue_1))))));
                                                                                                                                                                                                                                    b6_1 = head_1(tail(tail(tail(tail(tail(tail(matchValue_1)))))));
                                                                                                                                                                                                                                    b7_1 = head_1(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))));
                                                                                                                                                                                                                                    b8_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))));
                                                                                                                                                                                                                                    d0_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    d1_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    d2_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    d3_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    d4_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    d5_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    d6_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))))))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    d7_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))))))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    d8_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))))))))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    f0_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))))))))))))));
                                                                                                                                                                                                                                    f1_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))))))))))))));
                                                                                                                                                                                                                                    f2_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))))))))))))))));
                                                                                                                                                                                                                                    f3_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    f4_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    f5_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    f6_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    f7_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    f8_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    l0_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))))))))));
                                                                                                                                                                                                                                    l1_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))))))))))));
                                                                                                                                                                                                                                    l2_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))))))))))));
                                                                                                                                                                                                                                    l3_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    l4_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    l5_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    l6_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    l7_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    l8_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    r0_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))))))))))))))));
                                                                                                                                                                                                                                    r1_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))))))))))))))))));
                                                                                                                                                                                                                                    r2_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))))))))))))))))));
                                                                                                                                                                                                                                    r3_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    r4_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    r5_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    r6_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    r7_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    r8_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                    u0_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))));
                                                                                                                                                                                                                                    u1_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))));
                                                                                                                                                                                                                                    u2_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))));
                                                                                                                                                                                                                                    u3_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))));
                                                                                                                                                                                                                                    u4_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))))));
                                                                                                                                                                                                                                    u5_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))))));
                                                                                                                                                                                                                                    u6_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))))))));
                                                                                                                                                                                                                                    u7_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1)))))))))))))))));
                                                                                                                                                                                                                                    u8_1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue_1))))))))))))))))));
                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                else {
                                                                                                                                                                                                                                    matchResult_2 = 1;
                                                                                                                                                                                                                                    invalid_1 = matchValue_1;
                                                                                                                                                                                                                                }
                                                                                                                                                                                                                            }
                                                                                                                                                                                                                            else {
                                                                                                                                                                                                                                matchResult_2 = 1;
                                                                                                                                                                                                                                invalid_1 = matchValue_1;
                                                                                                                                                                                                                            }
                                                                                                                                                                                                                        }
                                                                                                                                                                                                                        else {
                                                                                                                                                                                                                            matchResult_2 = 1;
                                                                                                                                                                                                                            invalid_1 = matchValue_1;
                                                                                                                                                                                                                        }
                                                                                                                                                                                                                    }
                                                                                                                                                                                                                    else {
                                                                                                                                                                                                                        matchResult_2 = 1;
                                                                                                                                                                                                                        invalid_1 = matchValue_1;
                                                                                                                                                                                                                    }
                                                                                                                                                                                                                }
                                                                                                                                                                                                                else {
                                                                                                                                                                                                                    matchResult_2 = 1;
                                                                                                                                                                                                                    invalid_1 = matchValue_1;
                                                                                                                                                                                                                }
                                                                                                                                                                                                            }
                                                                                                                                                                                                            else {
                                                                                                                                                                                                                matchResult_2 = 1;
                                                                                                                                                                                                                invalid_1 = matchValue_1;
                                                                                                                                                                                                            }
                                                                                                                                                                                                        }
                                                                                                                                                                                                        else {
                                                                                                                                                                                                            matchResult_2 = 1;
                                                                                                                                                                                                            invalid_1 = matchValue_1;
                                                                                                                                                                                                        }
                                                                                                                                                                                                    }
                                                                                                                                                                                                    else {
                                                                                                                                                                                                        matchResult_2 = 1;
                                                                                                                                                                                                        invalid_1 = matchValue_1;
                                                                                                                                                                                                    }
                                                                                                                                                                                                }
                                                                                                                                                                                                else {
                                                                                                                                                                                                    matchResult_2 = 1;
                                                                                                                                                                                                    invalid_1 = matchValue_1;
                                                                                                                                                                                                }
                                                                                                                                                                                            }
                                                                                                                                                                                            else {
                                                                                                                                                                                                matchResult_2 = 1;
                                                                                                                                                                                                invalid_1 = matchValue_1;
                                                                                                                                                                                            }
                                                                                                                                                                                        }
                                                                                                                                                                                        else {
                                                                                                                                                                                            matchResult_2 = 1;
                                                                                                                                                                                            invalid_1 = matchValue_1;
                                                                                                                                                                                        }
                                                                                                                                                                                    }
                                                                                                                                                                                    else {
                                                                                                                                                                                        matchResult_2 = 1;
                                                                                                                                                                                        invalid_1 = matchValue_1;
                                                                                                                                                                                    }
                                                                                                                                                                                }
                                                                                                                                                                                else {
                                                                                                                                                                                    matchResult_2 = 1;
                                                                                                                                                                                    invalid_1 = matchValue_1;
                                                                                                                                                                                }
                                                                                                                                                                            }
                                                                                                                                                                            else {
                                                                                                                                                                                matchResult_2 = 1;
                                                                                                                                                                                invalid_1 = matchValue_1;
                                                                                                                                                                            }
                                                                                                                                                                        }
                                                                                                                                                                        else {
                                                                                                                                                                            matchResult_2 = 1;
                                                                                                                                                                            invalid_1 = matchValue_1;
                                                                                                                                                                        }
                                                                                                                                                                    }
                                                                                                                                                                    else {
                                                                                                                                                                        matchResult_2 = 1;
                                                                                                                                                                        invalid_1 = matchValue_1;
                                                                                                                                                                    }
                                                                                                                                                                }
                                                                                                                                                                else {
                                                                                                                                                                    matchResult_2 = 1;
                                                                                                                                                                    invalid_1 = matchValue_1;
                                                                                                                                                                }
                                                                                                                                                            }
                                                                                                                                                            else {
                                                                                                                                                                matchResult_2 = 1;
                                                                                                                                                                invalid_1 = matchValue_1;
                                                                                                                                                            }
                                                                                                                                                        }
                                                                                                                                                        else {
                                                                                                                                                            matchResult_2 = 1;
                                                                                                                                                            invalid_1 = matchValue_1;
                                                                                                                                                        }
                                                                                                                                                    }
                                                                                                                                                    else {
                                                                                                                                                        matchResult_2 = 1;
                                                                                                                                                        invalid_1 = matchValue_1;
                                                                                                                                                    }
                                                                                                                                                }
                                                                                                                                                else {
                                                                                                                                                    matchResult_2 = 1;
                                                                                                                                                    invalid_1 = matchValue_1;
                                                                                                                                                }
                                                                                                                                            }
                                                                                                                                            else {
                                                                                                                                                matchResult_2 = 1;
                                                                                                                                                invalid_1 = matchValue_1;
                                                                                                                                            }
                                                                                                                                        }
                                                                                                                                        else {
                                                                                                                                            matchResult_2 = 1;
                                                                                                                                            invalid_1 = matchValue_1;
                                                                                                                                        }
                                                                                                                                    }
                                                                                                                                    else {
                                                                                                                                        matchResult_2 = 1;
                                                                                                                                        invalid_1 = matchValue_1;
                                                                                                                                    }
                                                                                                                                }
                                                                                                                                else {
                                                                                                                                    matchResult_2 = 1;
                                                                                                                                    invalid_1 = matchValue_1;
                                                                                                                                }
                                                                                                                            }
                                                                                                                            else {
                                                                                                                                matchResult_2 = 1;
                                                                                                                                invalid_1 = matchValue_1;
                                                                                                                            }
                                                                                                                        }
                                                                                                                        else {
                                                                                                                            matchResult_2 = 1;
                                                                                                                            invalid_1 = matchValue_1;
                                                                                                                        }
                                                                                                                    }
                                                                                                                    else {
                                                                                                                        matchResult_2 = 1;
                                                                                                                        invalid_1 = matchValue_1;
                                                                                                                    }
                                                                                                                }
                                                                                                                else {
                                                                                                                    matchResult_2 = 1;
                                                                                                                    invalid_1 = matchValue_1;
                                                                                                                }
                                                                                                            }
                                                                                                            else {
                                                                                                                matchResult_2 = 1;
                                                                                                                invalid_1 = matchValue_1;
                                                                                                            }
                                                                                                        }
                                                                                                        else {
                                                                                                            matchResult_2 = 1;
                                                                                                            invalid_1 = matchValue_1;
                                                                                                        }
                                                                                                    }
                                                                                                    else {
                                                                                                        matchResult_2 = 1;
                                                                                                        invalid_1 = matchValue_1;
                                                                                                    }
                                                                                                }
                                                                                                else {
                                                                                                    matchResult_2 = 1;
                                                                                                    invalid_1 = matchValue_1;
                                                                                                }
                                                                                            }
                                                                                            else {
                                                                                                matchResult_2 = 1;
                                                                                                invalid_1 = matchValue_1;
                                                                                            }
                                                                                        }
                                                                                        else {
                                                                                            matchResult_2 = 1;
                                                                                            invalid_1 = matchValue_1;
                                                                                        }
                                                                                    }
                                                                                    else {
                                                                                        matchResult_2 = 1;
                                                                                        invalid_1 = matchValue_1;
                                                                                    }
                                                                                }
                                                                                else {
                                                                                    matchResult_2 = 1;
                                                                                    invalid_1 = matchValue_1;
                                                                                }
                                                                            }
                                                                            else {
                                                                                matchResult_2 = 1;
                                                                                invalid_1 = matchValue_1;
                                                                            }
                                                                        }
                                                                        else {
                                                                            matchResult_2 = 1;
                                                                            invalid_1 = matchValue_1;
                                                                        }
                                                                    }
                                                                    else {
                                                                        matchResult_2 = 1;
                                                                        invalid_1 = matchValue_1;
                                                                    }
                                                                }
                                                                else {
                                                                    matchResult_2 = 1;
                                                                    invalid_1 = matchValue_1;
                                                                }
                                                            }
                                                            else {
                                                                matchResult_2 = 1;
                                                                invalid_1 = matchValue_1;
                                                            }
                                                        }
                                                        else {
                                                            matchResult_2 = 1;
                                                            invalid_1 = matchValue_1;
                                                        }
                                                    }
                                                    else {
                                                        matchResult_2 = 1;
                                                        invalid_1 = matchValue_1;
                                                    }
                                                }
                                                else {
                                                    matchResult_2 = 1;
                                                    invalid_1 = matchValue_1;
                                                }
                                            }
                                            else {
                                                matchResult_2 = 1;
                                                invalid_1 = matchValue_1;
                                            }
                                        }
                                        else {
                                            matchResult_2 = 1;
                                            invalid_1 = matchValue_1;
                                        }
                                    }
                                    else {
                                        matchResult_2 = 1;
                                        invalid_1 = matchValue_1;
                                    }
                                }
                                else {
                                    matchResult_2 = 1;
                                    invalid_1 = matchValue_1;
                                }
                            }
                            else {
                                matchResult_2 = 1;
                                invalid_1 = matchValue_1;
                            }
                        }
                        else {
                            matchResult_2 = 1;
                            invalid_1 = matchValue_1;
                        }
                    }
                    else {
                        matchResult_2 = 1;
                        invalid_1 = matchValue_1;
                    }
                }
                else {
                    matchResult_2 = 1;
                    invalid_1 = matchValue_1;
                }
            }
            else {
                matchResult_2 = 1;
                invalid_1 = matchValue_1;
            }
        }
        else {
            matchResult_2 = 1;
            invalid_1 = matchValue_1;
        }
        switch (matchResult_2) {
            case 0:
                return ofList(ofArray([b0_1, b1_1, b2_1, b3_1, b4_1, b5_1, c_4(b6_1), b7_1, c_4(b8_1), c_4(u0_1), u1_1, c_4(u2_1), u3_1, u4_1, u5_1, c_4(u6_1), u7_1, c_4(u8_1), c_4(l0_1), l1_1, c_4(l2_1), c_4(f0_1), f1_1, c_4(f2_1), c_4(r0_1), r1_1, c_4(r2_1), l3_1, l4_1, l5_1, f3_1, f4_1, f5_1, r3_1, r4_1, r5_1, l6_1, l7_1, l8_1, f6_1, f7_1, f8_1, r6_1, r7_1, r8_1, d0_1, d1_1, d2_1, d3_1, d4_1, d5_1, d6_1, d7_1, d8_1]));
            default:
                throw new Error(toText(printf("Invalid pattern: %A"))(invalid_1));
        }
    };
    const pat = pattern[0];
    const colorNeutralUpCorners = pattern[2];
    const patU = () => rotateCorners(pat.split(""));
    const patU2 = () => rotateCorners(patU());
    const patU$0027 = () => rotateCorners(patU2());
    const cw = (p_4) => cycleCornerColors((_arg) => {
        switch (_arg) {
            case "B":
                return "O";
            case "G":
                return "R";
            case "O":
                return "G";
            case "R":
                return "B";
            default:
                return _arg;
        }
    }, p_4);
    const ccw = (p_5) => cycleCornerColors((_arg_1) => {
        switch (_arg_1) {
            case "B":
                return "R";
            case "G":
                return "O";
            case "O":
                return "B";
            case "R":
                return "G";
            default:
                return _arg_1;
        }
    }, p_5);
    const swap = (p_6) => cycleCornerColors((_arg_2) => {
        switch (_arg_2) {
            case "B":
                return "G";
            case "G":
                return "B";
            case "O":
                return "R";
            case "R":
                return "O";
            default:
                return _arg_2;
        }
    }, p_6);
    if (pattern[1]) {
        if (colorNeutralUpCorners) {
            if ((((((((((((((matchPat(pat.split("")) ? true : matchPat(patU())) ? true : matchPat(patU2())) ? true : matchPat(patU$0027())) ? true : matchPat(cw(pat.split("")))) ? true : matchPat(cw(patU()))) ? true : matchPat(cw(patU2()))) ? true : matchPat(cw(patU$0027()))) ? true : matchPat(ccw(pat.split("")))) ? true : matchPat(ccw(patU()))) ? true : matchPat(ccw(patU2()))) ? true : matchPat(ccw(patU$0027()))) ? true : matchPat(swap(pat.split("")))) ? true : matchPat(swap(patU()))) ? true : matchPat(swap(patU2()))) {
                return true;
            }
            else {
                return matchPat(swap(patU$0027()));
            }
        }
        else if ((matchPat(pat.split("")) ? true : matchPat(patU())) ? true : matchPat(patU2())) {
            return true;
        }
        else {
            return matchPat(patU$0027());
        }
    }
    else if (colorNeutralUpCorners) {
        if ((matchPat(pat.split("")) ? true : matchPat(cw(pat.split("")))) ? true : matchPat(ccw(pat.split("")))) {
            return true;
        }
        else {
            return matchPat(swap(pat.split("")));
        }
    }
    else {
        return matchPat(pat.split(""));
    }
}

export function hybridSolve(steps, hints, patterns, goal, stage, cube) {
    const matchValue = tryFind((tupledArg) => (equals(tupledArg[1], stage) && tupledArg[0](cube)(tupledArg[2])), patterns);
    if (matchValue == null) {
        if (warnings) {
            const arg_1 = cubeToString(cube);
            toConsole(printf("UNMATCHED: %s"))(arg_1);
        }
        const matchValue_3 = tryHead(filter_1((arg_3) => {
            const matchValue_2 = tryHead(arg_3);
            if (matchValue_2 == null) {
                return false;
            }
            else {
                return goal(executeAndReportSteps(matchValue_2)(cube));
            }
        }, hints));
        if (matchValue_3 == null) {
            return solveWithSteps(steps, goal, cube);
        }
        else {
            return matchValue_3;
        }
    }
    else {
        const algs = matchValue[3];
        if (!isEmpty(algs)) {
            if (preferGoalMatchingAlgorithm()) {
                const matchValue_1 = tryFind_1((candidate_2) => goal(fold(executeStep, cube, candidate_2)), List_distinct(collect((arg) => {
                    const steps_1 = stringToSteps(arg);
                    const equivalents = cons(steps_1, choose((x) => x, mapIndexed((index, step) => {
                        let _arg_1;
                        return map((replacement) => mapIndexed((candidateIndex, candidate) => {
                            if (candidateIndex === index) {
                                return replacement;
                            }
                            else {
                                return candidate;
                            }
                        }, steps_1), (_arg_1 = step, (_arg_1.tag === 1) ? ((_arg_1.fields[0].tag === 18) ? (new Step(1, [new Move(21, [])])) : ((_arg_1.fields[0].tag === 19) ? (new Step(1, [new Move(22, [])])) : ((_arg_1.fields[0].tag === 20) ? (new Step(1, [new Move(23, [])])) : ((_arg_1.fields[0].tag === 21) ? (new Step(1, [new Move(18, [])])) : ((_arg_1.fields[0].tag === 22) ? (new Step(1, [new Move(19, [])])) : ((_arg_1.fields[0].tag === 23) ? (new Step(1, [new Move(20, [])])) : void 0)))))) : void 0));
                    }, steps_1)));
                    return append(equivalents, collect((candidate_1) => ofArray([append(candidate_1, singleton(new Step(1, [new Move(36, [])]))), append(candidate_1, singleton(new Step(1, [new Move(37, [])])))]), equivalents));
                }, algs), {
                    Equals: equals,
                    GetHashCode: safeHash,
                }));
                if (matchValue_1 == null) {
                    throw new Error(`Matched pattern has no algorithm satisfying its required goal: ${cubeToString(cube)}`);
                }
                else {
                    return singleton(matchValue_1);
                }
            }
            else {
                return singleton(stringToSteps(head_1(algs)));
            }
        }
        else {
            return empty();
        }
    }
}

export let best = createAtom(0);

export let worst = createAtom(0);

export function genCasesAndSolutions(patterns, steps, cubes, goal, stage) {
    const gen = (cases_mut, hints_mut, b_mut, w_mut, _arg_mut) => {
        gen:
        while (true) {
            const cases = cases_mut, hints = hints_mut, b = b_mut, w = w_mut, _arg = _arg_mut;
            if (!isEmpty(_arg)) {
                const remaining = tail(_arg);
                const cube = head_1(_arg);
                const solutions = hybridSolve(steps, hints, patterns, goal, stage, cube);
                const turns = ((length(solutions) === 0) ? 0 : length(item(0, solutions))) | 0;
                const b$0027 = ((turns < b) ? turns : b) | 0;
                const w$0027 = ((turns > w) ? turns : w) | 0;
                const algs = map_1(stepsToString, solutions);
                const skip = length_1(solutions) === 0;
                solutionTrace(append(solutionTrace(), singleton([stage, skip ? empty() : head(solutions)])));
                const key = skip ? "" : head(sort(algs, {
                    Compare: comparePrimitives,
                }));
                const matchValue = tryFind_2(key, cases);
                if (matchValue == null) {
                    if (!quiet) {
                        const arg_1 = stringHash(key) | 0;
                        const arg_2 = join("\"; \"", algs);
                        const arg_3 = cubeToString(cube);
                        toConsole(printf("New case: %i [\"%s\"] (%s)"))(arg_1)(arg_2)(arg_3);
                    }
                    const hints$0027 = skip ? hints : cons(solutions, hints);
                    cases_mut = add(key, singleton([cube, solutions, skip ? cube : executeAndReportSteps(head(solutions))(cube)]), cases);
                    hints_mut = hints$0027;
                    b_mut = b$0027;
                    w_mut = w$0027;
                    _arg_mut = remaining;
                    continue gen;
                }
                else {
                    const case$ = matchValue;
                    cases_mut = add(key, cons([cube, solutions, skip ? cube : executeAndReportSteps(head(solutions))(cube)], case$), cases);
                    hints_mut = hints;
                    b_mut = b$0027;
                    w_mut = w$0027;
                    _arg_mut = remaining;
                    continue gen;
                }
            }
            else {
                return [cases, b, w];
            }
            break;
        }
    };
    const patternInput = gen(empty_1({
        Compare: comparePrimitives,
    }), empty(), 2147483647, -2147483648, cubes);
    best(best() + patternInput[1]);
    worst(worst() + patternInput[2]);
    return patternInput[0];
}

export function distinctCases(solutions) {
    iterate((tupledArg_1) => {
        const a = tupledArg_1[1];
        if (!quiet) {
            const arg_1 = stringHash(a) | 0;
            toConsole(printf("Algs: %s (%i) \"%s\""))(a)(arg_1)(tupledArg_1[0]);
        }
    }, zip(map_2((cubes) => join("", initialize(9 * 6, (n) => {
        const distinct = distinct_1(map_1((c) => c[n], cubes), {
            Equals: (x, y) => (x === y),
            GetHashCode: stringHash,
        });
        if (length_1(distinct) === 1) {
            return head(distinct);
        }
        else {
            return ".";
        }
    })), map_2((list_1) => map_2((tupledArg) => cubeToString(tupledArg[0]), list_1), map_2((tuple) => tuple[1], toList(solutions)))), map_2((tuple_1) => tuple_1[0], toList(solutions))));
}

export function expandPatternsForAuf(patterns) {
    return concat(map_1((tupledArg) => {
        const matchFn = tupledArg[0];
        const name = tupledArg[1];
        const _arg = tupledArg[2];
        const algs = tupledArg[3];
        const pat = _arg[0];
        const cornerRotationNeutral = _arg[1];
        const cornerColorNeutral = _arg[2];
        return delay(() => append_1(singleton_1([matchFn, name, [pat, cornerRotationNeutral, cornerColorNeutral], algs]), delay(() => {
            if (_arg[3]) {
                const prepend = (a, algs_1) => {
                    if (length(algs_1) === 0) {
                        return singleton(a);
                    }
                    else {
                        return map_2((alg) => ((a + " ") + alg), algs_1);
                    }
                };
                const auf = (p) => {
                    const matchValue = ofSeq(p);
                    let matchResult, b0, b1, b2, b3, b4, b5, b6, b7, b8, d0, d1, d2, d3, d4, d5, d6, d7, d8, f0, f1, f2, f3, f4, f5, f6, f7, f8, l0, l1, l2, l3, l4, l5, l6, l7, l8, r0, r1, r2, r3, r4, r5, r6, r7, r8, u0, u1, u2, u3, u4, u5, u6, u7, u8, invalid;
                    if (!isEmpty(matchValue)) {
                        if (!isEmpty(tail(matchValue))) {
                            if (!isEmpty(tail(tail(matchValue)))) {
                                if (!isEmpty(tail(tail(tail(matchValue))))) {
                                    if (!isEmpty(tail(tail(tail(tail(matchValue)))))) {
                                        if (!isEmpty(tail(tail(tail(tail(tail(matchValue))))))) {
                                            if (!isEmpty(tail(tail(tail(tail(tail(tail(matchValue)))))))) {
                                                if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))) {
                                                    if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))) {
                                                        if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))) {
                                                            if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))) {
                                                                if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))) {
                                                                    if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))) {
                                                                        if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))) {
                                                                            if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))) {
                                                                                if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))) {
                                                                                    if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))) {
                                                                                        if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))) {
                                                                                            if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))) {
                                                                                                if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))) {
                                                                                                    if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))) {
                                                                                                        if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))) {
                                                                                                            if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))) {
                                                                                                                if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))) {
                                                                                                                    if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))) {
                                                                                                                        if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))) {
                                                                                                                            if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))) {
                                                                                                                                if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))) {
                                                                                                                                    if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))) {
                                                                                                                                        if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))) {
                                                                                                                                            if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))) {
                                                                                                                                                if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))) {
                                                                                                                                                    if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))) {
                                                                                                                                                        if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))))) {
                                                                                                                                                            if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                    if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                        if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                            if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                    if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                        if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                            if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                                if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                                    if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                                        if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                                            if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                                                if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                                                    if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                                                        if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                                                            if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                                                                if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                                                                    if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                                                                        if (!isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                                                                            if (isEmpty(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))))))))))))))))))))))))) {
                                                                                                                                                                                                                                                matchResult = 0;
                                                                                                                                                                                                                                                b0 = head_1(matchValue);
                                                                                                                                                                                                                                                b1 = head_1(tail(matchValue));
                                                                                                                                                                                                                                                b2 = head_1(tail(tail(matchValue)));
                                                                                                                                                                                                                                                b3 = head_1(tail(tail(tail(matchValue))));
                                                                                                                                                                                                                                                b4 = head_1(tail(tail(tail(tail(matchValue)))));
                                                                                                                                                                                                                                                b5 = head_1(tail(tail(tail(tail(tail(matchValue))))));
                                                                                                                                                                                                                                                b6 = head_1(tail(tail(tail(tail(tail(tail(matchValue)))))));
                                                                                                                                                                                                                                                b7 = head_1(tail(tail(tail(tail(tail(tail(tail(matchValue))))))));
                                                                                                                                                                                                                                                b8 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))));
                                                                                                                                                                                                                                                d0 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                                d1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                                d2 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                                d3 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                                d4 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                                d5 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                                d6 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                                d7 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                                d8 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                                f0 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))));
                                                                                                                                                                                                                                                f1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))));
                                                                                                                                                                                                                                                f2 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))));
                                                                                                                                                                                                                                                f3 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                                f4 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                                f5 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                                f6 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                                f7 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                                f8 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                                l0 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))));
                                                                                                                                                                                                                                                l1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))));
                                                                                                                                                                                                                                                l2 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))));
                                                                                                                                                                                                                                                l3 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))));
                                                                                                                                                                                                                                                l4 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))));
                                                                                                                                                                                                                                                l5 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                                l6 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                                l7 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                                l8 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                                r0 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))));
                                                                                                                                                                                                                                                r1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))));
                                                                                                                                                                                                                                                r2 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))));
                                                                                                                                                                                                                                                r3 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                                r4 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                                r5 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                                r6 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                                r7 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                                r8 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))))))))))))))))))))))))))))))));
                                                                                                                                                                                                                                                u0 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))));
                                                                                                                                                                                                                                                u1 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))));
                                                                                                                                                                                                                                                u2 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))));
                                                                                                                                                                                                                                                u3 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))));
                                                                                                                                                                                                                                                u4 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))));
                                                                                                                                                                                                                                                u5 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))));
                                                                                                                                                                                                                                                u6 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))));
                                                                                                                                                                                                                                                u7 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue)))))))))))))))));
                                                                                                                                                                                                                                                u8 = head_1(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(tail(matchValue))))))))))))))))));
                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                            else {
                                                                                                                                                                                                                                                matchResult = 1;
                                                                                                                                                                                                                                                invalid = matchValue;
                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                        else {
                                                                                                                                                                                                                                            matchResult = 1;
                                                                                                                                                                                                                                            invalid = matchValue;
                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                    else {
                                                                                                                                                                                                                                        matchResult = 1;
                                                                                                                                                                                                                                        invalid = matchValue;
                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                else {
                                                                                                                                                                                                                                    matchResult = 1;
                                                                                                                                                                                                                                    invalid = matchValue;
                                                                                                                                                                                                                                }
                                                                                                                                                                                                                            }
                                                                                                                                                                                                                            else {
                                                                                                                                                                                                                                matchResult = 1;
                                                                                                                                                                                                                                invalid = matchValue;
                                                                                                                                                                                                                            }
                                                                                                                                                                                                                        }
                                                                                                                                                                                                                        else {
                                                                                                                                                                                                                            matchResult = 1;
                                                                                                                                                                                                                            invalid = matchValue;
                                                                                                                                                                                                                        }
                                                                                                                                                                                                                    }
                                                                                                                                                                                                                    else {
                                                                                                                                                                                                                        matchResult = 1;
                                                                                                                                                                                                                        invalid = matchValue;
                                                                                                                                                                                                                    }
                                                                                                                                                                                                                }
                                                                                                                                                                                                                else {
                                                                                                                                                                                                                    matchResult = 1;
                                                                                                                                                                                                                    invalid = matchValue;
                                                                                                                                                                                                                }
                                                                                                                                                                                                            }
                                                                                                                                                                                                            else {
                                                                                                                                                                                                                matchResult = 1;
                                                                                                                                                                                                                invalid = matchValue;
                                                                                                                                                                                                            }
                                                                                                                                                                                                        }
                                                                                                                                                                                                        else {
                                                                                                                                                                                                            matchResult = 1;
                                                                                                                                                                                                            invalid = matchValue;
                                                                                                                                                                                                        }
                                                                                                                                                                                                    }
                                                                                                                                                                                                    else {
                                                                                                                                                                                                        matchResult = 1;
                                                                                                                                                                                                        invalid = matchValue;
                                                                                                                                                                                                    }
                                                                                                                                                                                                }
                                                                                                                                                                                                else {
                                                                                                                                                                                                    matchResult = 1;
                                                                                                                                                                                                    invalid = matchValue;
                                                                                                                                                                                                }
                                                                                                                                                                                            }
                                                                                                                                                                                            else {
                                                                                                                                                                                                matchResult = 1;
                                                                                                                                                                                                invalid = matchValue;
                                                                                                                                                                                            }
                                                                                                                                                                                        }
                                                                                                                                                                                        else {
                                                                                                                                                                                            matchResult = 1;
                                                                                                                                                                                            invalid = matchValue;
                                                                                                                                                                                        }
                                                                                                                                                                                    }
                                                                                                                                                                                    else {
                                                                                                                                                                                        matchResult = 1;
                                                                                                                                                                                        invalid = matchValue;
                                                                                                                                                                                    }
                                                                                                                                                                                }
                                                                                                                                                                                else {
                                                                                                                                                                                    matchResult = 1;
                                                                                                                                                                                    invalid = matchValue;
                                                                                                                                                                                }
                                                                                                                                                                            }
                                                                                                                                                                            else {
                                                                                                                                                                                matchResult = 1;
                                                                                                                                                                                invalid = matchValue;
                                                                                                                                                                            }
                                                                                                                                                                        }
                                                                                                                                                                        else {
                                                                                                                                                                            matchResult = 1;
                                                                                                                                                                            invalid = matchValue;
                                                                                                                                                                        }
                                                                                                                                                                    }
                                                                                                                                                                    else {
                                                                                                                                                                        matchResult = 1;
                                                                                                                                                                        invalid = matchValue;
                                                                                                                                                                    }
                                                                                                                                                                }
                                                                                                                                                                else {
                                                                                                                                                                    matchResult = 1;
                                                                                                                                                                    invalid = matchValue;
                                                                                                                                                                }
                                                                                                                                                            }
                                                                                                                                                            else {
                                                                                                                                                                matchResult = 1;
                                                                                                                                                                invalid = matchValue;
                                                                                                                                                            }
                                                                                                                                                        }
                                                                                                                                                        else {
                                                                                                                                                            matchResult = 1;
                                                                                                                                                            invalid = matchValue;
                                                                                                                                                        }
                                                                                                                                                    }
                                                                                                                                                    else {
                                                                                                                                                        matchResult = 1;
                                                                                                                                                        invalid = matchValue;
                                                                                                                                                    }
                                                                                                                                                }
                                                                                                                                                else {
                                                                                                                                                    matchResult = 1;
                                                                                                                                                    invalid = matchValue;
                                                                                                                                                }
                                                                                                                                            }
                                                                                                                                            else {
                                                                                                                                                matchResult = 1;
                                                                                                                                                invalid = matchValue;
                                                                                                                                            }
                                                                                                                                        }
                                                                                                                                        else {
                                                                                                                                            matchResult = 1;
                                                                                                                                            invalid = matchValue;
                                                                                                                                        }
                                                                                                                                    }
                                                                                                                                    else {
                                                                                                                                        matchResult = 1;
                                                                                                                                        invalid = matchValue;
                                                                                                                                    }
                                                                                                                                }
                                                                                                                                else {
                                                                                                                                    matchResult = 1;
                                                                                                                                    invalid = matchValue;
                                                                                                                                }
                                                                                                                            }
                                                                                                                            else {
                                                                                                                                matchResult = 1;
                                                                                                                                invalid = matchValue;
                                                                                                                            }
                                                                                                                        }
                                                                                                                        else {
                                                                                                                            matchResult = 1;
                                                                                                                            invalid = matchValue;
                                                                                                                        }
                                                                                                                    }
                                                                                                                    else {
                                                                                                                        matchResult = 1;
                                                                                                                        invalid = matchValue;
                                                                                                                    }
                                                                                                                }
                                                                                                                else {
                                                                                                                    matchResult = 1;
                                                                                                                    invalid = matchValue;
                                                                                                                }
                                                                                                            }
                                                                                                            else {
                                                                                                                matchResult = 1;
                                                                                                                invalid = matchValue;
                                                                                                            }
                                                                                                        }
                                                                                                        else {
                                                                                                            matchResult = 1;
                                                                                                            invalid = matchValue;
                                                                                                        }
                                                                                                    }
                                                                                                    else {
                                                                                                        matchResult = 1;
                                                                                                        invalid = matchValue;
                                                                                                    }
                                                                                                }
                                                                                                else {
                                                                                                    matchResult = 1;
                                                                                                    invalid = matchValue;
                                                                                                }
                                                                                            }
                                                                                            else {
                                                                                                matchResult = 1;
                                                                                                invalid = matchValue;
                                                                                            }
                                                                                        }
                                                                                        else {
                                                                                            matchResult = 1;
                                                                                            invalid = matchValue;
                                                                                        }
                                                                                    }
                                                                                    else {
                                                                                        matchResult = 1;
                                                                                        invalid = matchValue;
                                                                                    }
                                                                                }
                                                                                else {
                                                                                    matchResult = 1;
                                                                                    invalid = matchValue;
                                                                                }
                                                                            }
                                                                            else {
                                                                                matchResult = 1;
                                                                                invalid = matchValue;
                                                                            }
                                                                        }
                                                                        else {
                                                                            matchResult = 1;
                                                                            invalid = matchValue;
                                                                        }
                                                                    }
                                                                    else {
                                                                        matchResult = 1;
                                                                        invalid = matchValue;
                                                                    }
                                                                }
                                                                else {
                                                                    matchResult = 1;
                                                                    invalid = matchValue;
                                                                }
                                                            }
                                                            else {
                                                                matchResult = 1;
                                                                invalid = matchValue;
                                                            }
                                                        }
                                                        else {
                                                            matchResult = 1;
                                                            invalid = matchValue;
                                                        }
                                                    }
                                                    else {
                                                        matchResult = 1;
                                                        invalid = matchValue;
                                                    }
                                                }
                                                else {
                                                    matchResult = 1;
                                                    invalid = matchValue;
                                                }
                                            }
                                            else {
                                                matchResult = 1;
                                                invalid = matchValue;
                                            }
                                        }
                                        else {
                                            matchResult = 1;
                                            invalid = matchValue;
                                        }
                                    }
                                    else {
                                        matchResult = 1;
                                        invalid = matchValue;
                                    }
                                }
                                else {
                                    matchResult = 1;
                                    invalid = matchValue;
                                }
                            }
                            else {
                                matchResult = 1;
                                invalid = matchValue;
                            }
                        }
                        else {
                            matchResult = 1;
                            invalid = matchValue;
                        }
                    }
                    else {
                        matchResult = 1;
                        invalid = matchValue;
                    }
                    switch (matchResult) {
                        case 0:
                            return ofList(ofArray([b0, b1, b2, b3, b4, b5, l2, l1, l0, u6, u3, u0, u7, u4, u1, u8, u5, u2, f0, f1, f2, r0, r1, r2, b8, b7, b6, l3, l4, l5, f3, f4, f5, r3, r4, r5, l6, l7, l8, f6, f7, f8, r6, r7, r8, d0, d1, d2, d3, d4, d5, d6, d7, d8]));
                        default:
                            throw new Error(toText(printf("Invalid pattern: %A"))(invalid));
                    }
                };
                const u = auf(pat.split(""));
                const u2_1 = auf(u);
                const u$0027 = auf(u2_1);
                return append_1(singleton_1([matchFn, name, [join("", u), cornerRotationNeutral, cornerColorNeutral], prepend("U\'", algs)]), delay(() => append_1(singleton_1([matchFn, name, [join("", u2_1), cornerRotationNeutral, cornerColorNeutral], prepend("U2", algs)]), delay(() => singleton_1([matchFn, name, [join("", u$0027), cornerRotationNeutral, cornerColorNeutral], prepend("U", algs)])))));
            }
            else {
                return empty_2();
            }
        })));
    }, patterns));
}

export function solveCase(patterns, steps, name, id, case$, scrambled, verify) {
    toConsole(printf("\nCase: %s"))(name);
    const solutions = genCasesAndSolutions(patterns, steps, scrambled, case$, id);
    distinctCases(solutions);
    const solved = map_2((tupledArg) => tupledArg[2], concat_1(map_2((tuple) => tuple[1], toList(solutions))));
    if (verify) {
        const enumerator = getEnumerator(solved);
        try {
            while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
                const s = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
                if (!case$(s)) {
                    const arg_1 = cubeToString(s);
                    toConsole(printf("UNSOLVED: %s"))(arg_1);
                    throw new Error("Authored pattern did not solve case");
                }
            }
        }
        finally {
            disposeSafe(enumerator);
        }
    }
    return solved;
}

export function stageStats(name, numCubes) {
    toConsole(printf("--------------------------------------------------------------------------------"));
    toConsole(printf("STAGE STATS: %s"))(name);
    const avgStageTwistCount = stageCount() / numCubes;
    const arg_2 = best() | 0;
    const arg_3 = worst() | 0;
    toConsole(printf("Average: %f Best: %i Worst: %i"))(avgStageTwistCount)(arg_2)(arg_3);
    toConsole(printf("--------------------------------------------------------------------------------"));
    stageCount(0);
    best(0);
    worst(0);
}

export function initScrambledCubes(numCubes) {
    toConsole(printf("Scrambling %i cubes"))(numCubes);
    toConsole(printf(""));
    return initialize_1(numCubes, (i) => {
        if ((i % 100) === 0) {
            toConsole(printf("."));
        }
        return scramble(100)[0];
    });
}

export function lookPattern(pattern, cube) {
    return forAll2((p, c) => {
        if (p === ".") {
            return true;
        }
        else {
            return p === c;
        }
    }, pattern, cubeToString(cube).split(""));
}

export function lookPatternAnyOrientation(pattern, cube) {
    const look = (cube_1) => lookPattern(pattern, cube_1);
    const cubeX = rotateX(cube);
    const cubeX$0027 = rotateX$0027(cube);
    const cubeX2 = rotateX2(cube);
    const cubeY = rotateY(cube);
    const cubeY$0027 = rotateY$0027(cube);
    const cubeY2 = rotateY2(cube);
    if ((((((((((((((((((((((look(cube) ? true : look(cubeX)) ? true : look(cubeX$0027)) ? true : look(cubeX2)) ? true : look(cubeY)) ? true : look(cubeY$0027)) ? true : look(cubeY2)) ? true : look(rotateZ(cube))) ? true : look(rotateZ$0027(cube))) ? true : look(rotateZ2(cube))) ? true : look(rotateY(cubeX))) ? true : look(rotateY$0027(cubeX))) ? true : look(rotateY2(cubeX))) ? true : look(rotateZ(cubeX))) ? true : look(rotateX(cubeY))) ? true : look(rotateZ2(cubeX))) ? true : look(rotateY(cubeX$0027))) ? true : look(rotateZ(cubeY$0027))) ? true : look(rotateZ(cubeX$0027))) ? true : look(rotateZ$0027(cubeX$0027))) ? true : look(rotateY(cubeX2))) ? true : look(rotateZ(cubeX2))) ? true : look(rotateX2(cubeY))) {
        return true;
    }
    else {
        return look(rotateZ(cubeY2));
    }
}

