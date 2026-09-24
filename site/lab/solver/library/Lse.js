import { centerToFaceSticker, cornerToFaceStickers, edgeToFaceStickers, look, solved, executeMove, Center, Corner, Edge, Move } from "./Cube.js";
import { Record } from "../fable_modules/fable-library-js.4.16.0/Types.js";
import { record_type, int32_type, array_type, uint8_type } from "../fable_modules/fable-library-js.4.16.0/Reflection.js";
import { fill, mapIndexed, choose, tryFindIndex, setItem, item, map as map_1, initialize } from "../fable_modules/fable-library-js.4.16.0/Array.js";
import { ofSeq, find, ofArray, item as item_1, sort, map } from "../fable_modules/fable-library-js.4.16.0/List.js";
import { compare, equals } from "../fable_modules/fable-library-js.4.16.0/Util.js";
import { toList } from "../fable_modules/fable-library-js.4.16.0/Seq.js";
import { rangeDouble } from "../fable_modules/fable-library-js.4.16.0/Range.js";
import { value, defaultArgWith } from "../fable_modules/fable-library-js.4.16.0/Option.js";
import { max } from "../fable_modules/fable-library-js.4.16.0/Double.js";

export const moves = [new Move(36, []), new Move(37, []), new Move(38, []), new Move(0, []), new Move(1, []), new Move(2, [])];

export const slotCount = ((720 * 32) * 4) * 4;

export const unreachable = 255;

export class Policy extends Record {
    constructor(Distances, OptimalMoves, ReachableCount, MaxDistance) {
        super();
        this.Distances = Distances;
        this.OptimalMoves = OptimalMoves;
        this.ReachableCount = (ReachableCount | 0);
        this.MaxDistance = (MaxDistance | 0);
    }
}

export function Policy_$reflection() {
    return record_type("Lse.Policy", [], Policy, () => [["Distances", array_type(uint8_type)], ["OptimalMoves", array_type(uint8_type)], ["ReachableCount", int32_type], ["MaxDistance", int32_type]]);
}

class State extends Record {
    constructor(Pieces, Flips, Center, Auf) {
        super();
        this.Pieces = Pieces;
        this.Flips = Flips;
        this.Center = (Center | 0);
        this.Auf = (Auf | 0);
    }
}

function State_$reflection() {
    return record_type("Lse.State", [], State, () => [["Pieces", array_type(int32_type)], ["Flips", array_type(int32_type)], ["Center", int32_type], ["Auf", int32_type]]);
}

const lseEdges = [new Edge(0, []), new Edge(1, []), new Edge(2, []), new Edge(3, []), new Edge(6, []), new Edge(7, [])];

const otherEdges = [new Edge(4, []), new Edge(5, []), new Edge(8, []), new Edge(9, []), new Edge(10, []), new Edge(11, [])];

const topCorners = [new Corner(0, []), new Corner(1, []), new Corner(3, []), new Corner(2, [])];

const bottomCorners = [new Corner(4, []), new Corner(5, []), new Corner(7, []), new Corner(6, [])];

const allCenters = [new Center(0, []), new Center(1, []), new Center(2, []), new Center(3, []), new Center(4, []), new Center(5, [])];

function apply(move, cube) {
    return executeMove(cube, move);
}

function repeat(count, move) {
    let cube = solved;
    for (let forLoopVar = 1; forLoopVar <= count; forLoopVar++) {
        cube = apply(move, cube);
    }
    return cube;
}

const mReferences = initialize(4, (count) => repeat(count, new Move(36, [])));

const uReferences = initialize(4, (count) => repeat(count, new Move(0, [])));

function colorsAt(locations, cube) {
    return map((tupledArg) => look(tupledArg[0], tupledArg[1], cube), locations);
}

function sameColors(left, right) {
    return equals(sort(left, {
        Compare: compare,
    }), sort(right, {
        Compare: compare,
    }));
}

function edgeColors(edge, cube) {
    return colorsAt(edgeToFaceStickers(edge), cube);
}

function cornerColors(corner, cube) {
    return colorsAt(cornerToFaceStickers(corner), cube);
}

function centerColor(center, cube) {
    const patternInput = centerToFaceSticker(center);
    return look(patternInput[0], patternInput[1], cube);
}

const solvedLseEdgeColors = map_1((edge) => edgeColors(edge, solved), lseEdges);

function permutationRank(permutation) {
    let rank = 0;
    for (let index = 0; index <= (permutation.length - 1); index++) {
        let smaller = 0;
        for (let later = index + 1; later <= (permutation.length - 1); later++) {
            if (item(later, permutation) < item(index, permutation)) {
                smaller = ((smaller + 1) | 0);
            }
        }
        rank = (((rank * (permutation.length - index)) + smaller) | 0);
    }
    return rank | 0;
}

const factorial = new Int32Array([1, 1, 2, 6, 24, 120, 720]);

function permutationOfRank(rank) {
    const available = Array.from(toList(rangeDouble(0, 1, 5)));
    const permutation = new Int32Array(6);
    let remaining = rank;
    for (let index = 0; index <= 5; index++) {
        const block = item(5 - index, factorial) | 0;
        const digit = ~~(remaining / block) | 0;
        remaining = ((remaining % block) | 0);
        setItem(permutation, index, available[digit] | 0);
        available.splice(digit, 1);
    }
    return permutation;
}

function indexState(state) {
    let flipRank = 0;
    for (let index = 0; index <= 4; index++) {
        flipRank = ((flipRank | (item(index, state.Flips) << index)) | 0);
    }
    return ((((((permutationRank(state.Pieces) * 32) + flipRank) * 4) + state.Center) * 4) + state.Auf) | 0;
}

function stateOfIndex(index) {
    const auf = (index % 4) | 0;
    const centerAndAbove = ~~(index / 4) | 0;
    const center = (centerAndAbove % 4) | 0;
    const flipAndAbove = ~~(centerAndAbove / 4) | 0;
    const flipRank = (flipAndAbove % 32) | 0;
    const permutation = permutationOfRank(~~(flipAndAbove / 32));
    const flips = new Int32Array(6);
    let parity = 0;
    for (let bit = 0; bit <= 4; bit++) {
        setItem(flips, bit, ((flipRank >> bit) & 1) | 0);
        parity = ((parity ^ item(bit, flips)) | 0);
    }
    setItem(flips, 5, parity | 0);
    return new State(permutation, flips, center, auf);
}

function stateOfCubeUnchecked(cube) {
    const pieces = new Int32Array(6);
    const flips = new Int32Array(6);
    for (let position = 0; position <= 5; position++) {
        const actual = edgeColors(item(position, lseEdges), cube);
        const piece = tryFindIndex((right) => sameColors(actual, right), solvedLseEdgeColors);
        if (piece != null) {
            const pieceIndex = piece | 0;
            setItem(pieces, position, pieceIndex | 0);
            setItem(flips, position, (equals(item_1(0, actual), item_1(0, item(pieceIndex, solvedLseEdgeColors))) ? 0 : 1) | 0);
        }
        else {
            throw new Error(((`Non-LSE edge occupies ${item(position, lseEdges)}.`) + "\\nParameter name: ") + "cube");
        }
    }
    return new State(pieces, flips, defaultArgWith(tryFindIndex((reference) => allCenters.every((position_1) => equals(centerColor(position_1, cube), centerColor(position_1, reference))), mReferences), () => {
        throw new Error("Centers are not in the M-slice orbit.\\nParameter name: cube");
    }), defaultArgWith(tryFindIndex((reference_1) => topCorners.every((position_2) => equals(cornerColors(position_2, cube), cornerColors(position_2, reference_1))), uReferences), () => {
        throw new Error("Top corners are not solved up to AUF.\\nParameter name: cube");
    }));
}

function validateLsePreconditions(cube) {
    otherEdges.forEach((position) => {
        if (!equals(edgeColors(position, cube), edgeColors(position, solved))) {
            throw new Error(((`Block edge ${position} is not solved.`) + "\\nParameter name: ") + "cube");
        }
    });
    bottomCorners.forEach((position_1) => {
        if (!equals(cornerColors(position_1, cube), cornerColors(position_1, solved))) {
            throw new Error(((`Bottom corner ${position_1} is not solved.`) + "\\nParameter name: ") + "cube");
        }
    });
    if (!equals(centerColor(new Center(2, []), cube), centerColor(new Center(2, []), solved)) ? true : !equals(centerColor(new Center(3, []), cube), centerColor(new Center(3, []), solved))) {
        throw new Error("Left or right center is not solved.\\nParameter name: cube");
    }
}

export function indexCube(cube) {
    validateLsePreconditions(cube);
    return indexState(stateOfCubeUnchecked(cube)) | 0;
}

const solvedState = stateOfCubeUnchecked(solved);

const transforms = map_1((move) => stateOfCubeUnchecked(apply(move, solved)), moves);

function moveState(moveIndex, state) {
    const transform = item(moveIndex, transforms);
    const pieces = new Int32Array(6);
    const flips = new Int32Array(6);
    for (let destination = 0; destination <= 5; destination++) {
        const source = item(destination, transform.Pieces) | 0;
        setItem(pieces, destination, item(source, state.Pieces) | 0);
        setItem(flips, destination, (item(source, state.Flips) ^ item(destination, transform.Flips)) | 0);
    }
    return new State(pieces, flips, (state.Center + transform.Center) % 4, (state.Auf + transform.Auf) % 4);
}

export function nextIndex(moveIndex, index) {
    return indexState(moveState(moveIndex, stateOfIndex(index)));
}

export function distance(policy, index) {
    if (((index < 0) ? true : (index >= slotCount)) ? true : (item(index, policy.Distances) === unreachable)) {
        return void 0;
    }
    else {
        return ~~item(index, policy.Distances);
    }
}

export function optimalNextMoves(policy, index) {
    if (((index < 0) ? true : (index >= slotCount)) ? true : (item(index, policy.Distances) === unreachable)) {
        throw new Error(((`State ${index} is not in the reachable LSE orbit.`) + "\\nParameter name: ") + "index");
    }
    const mask = ~~item(index, policy.OptimalMoves) | 0;
    return ofArray(choose((tupledArg) => {
        if ((mask & (1 << tupledArg[0])) !== 0) {
            return tupledArg[1];
        }
        else {
            return void 0;
        }
    }, mapIndexed((bit, move) => [bit, move], moves)));
}

export function buildPolicy() {
    const distances = fill(new Uint8Array(slotCount), 0, slotCount, unreachable);
    const optimalMoves = new Uint8Array(slotCount);
    const queue = new Int32Array(slotCount);
    const solvedIndex = indexState(solvedState) | 0;
    let head = 0;
    let tail = 1;
    let maxDistance = 0;
    setItem(queue, 0, solvedIndex | 0);
    setItem(distances, solvedIndex, 0);
    while (head < tail) {
        const current = item(head, queue) | 0;
        head = ((head + 1) | 0);
        const nextDistance = (~~item(current, distances) + 1) | 0;
        for (let moveIndex = 0; moveIndex <= (moves.length - 1); moveIndex++) {
            const neighbor = nextIndex(moveIndex, current) | 0;
            if (item(neighbor, distances) === unreachable) {
                setItem(distances, neighbor, nextDistance & 0xFF);
                maxDistance = (max(maxDistance, nextDistance) | 0);
                setItem(queue, tail, neighbor | 0);
                tail = ((tail + 1) | 0);
            }
        }
    }
    for (let index = 0; index <= (slotCount - 1); index++) {
        if ((item(index, distances) !== unreachable) && (item(index, distances) !== 0)) {
            const targetDistance = item(index, distances) - 1;
            let mask = 0;
            for (let moveIndex_1 = 0; moveIndex_1 <= (moves.length - 1); moveIndex_1++) {
                if (item(nextIndex(moveIndex_1, index), distances) === targetDistance) {
                    mask = ((mask | (1 << moveIndex_1)) | 0);
                }
            }
            if (mask === 0) {
                throw new Error(`Reachable LSE state ${index} has no optimal next move.`);
            }
            setItem(optimalMoves, index, mask & 0xFF);
        }
    }
    return new Policy(distances, optimalMoves, tail, maxDistance);
}

export function validatePolicy(policy) {
    if ((policy.Distances.length !== slotCount) ? true : (policy.OptimalMoves.length !== slotCount)) {
        throw new Error(((`Expected ${slotCount} LSE policy slots.`) + "\\nParameter name: ") + "policy");
    }
    let reachable = 0;
    let maximum = 0;
    for (let index = 0; index <= (slotCount - 1); index++) {
        const distance_1 = item(index, policy.Distances);
        if (distance_1 !== unreachable) {
            reachable = ((reachable + 1) | 0);
            maximum = (max(maximum, ~~distance_1) | 0);
            if (distance_1 === 0) {
                if (index !== indexState(solvedState)) {
                    throw new Error(`Unexpected zero-distance LSE state ${index}.`);
                }
            }
            else {
                const mask = ~~item(index, policy.OptimalMoves) | 0;
                if ((mask & ~63) !== 0) {
                    throw new Error(`LSE state ${index} contains invalid optimal-move bits.`);
                }
                if (mask === 0) {
                    throw new Error(`LSE state ${index} has no stored optimal move.`);
                }
                for (let moveIndex = 0; moveIndex <= (moves.length - 1); moveIndex++) {
                    if (((mask & (1 << moveIndex)) !== 0) && (item(nextIndex(moveIndex, index), policy.Distances) !== (distance_1 - 1))) {
                        throw new Error(`LSE state ${index} marks a non-optimal move ${item(moveIndex, moves)}.`);
                    }
                }
            }
        }
    }
    if (reachable !== policy.ReachableCount) {
        throw new Error("LSE reachable-count metadata is incorrect.");
    }
    if (maximum !== policy.MaxDistance) {
        throw new Error("LSE maximum-distance metadata is incorrect.");
    }
    return [reachable, maximum];
}

export function solveIndex(policy, index) {
    if (((index < 0) ? true : (index >= slotCount)) ? true : (item(index, policy.Distances) === unreachable)) {
        throw new Error(((`State ${index} is not in the reachable LSE orbit.`) + "\\nParameter name: ") + "index");
    }
    const solution = [];
    let current = index;
    while (item(current, policy.Distances) !== 0) {
        const mask = ~~item(current, policy.OptimalMoves) | 0;
        const moveIndex = find((bit) => ((mask & (1 << bit)) !== 0), toList(rangeDouble(0, 1, moves.length - 1))) | 0;
        void (solution.push(item(moveIndex, moves)));
        current = (nextIndex(moveIndex, current) | 0);
    }
    return ofSeq(solution);
}

export function solveCube(policy, cube) {
    return solveIndex(policy, indexCube(cube));
}

export function distanceCube(policy, cube) {
    return value(distance(policy, indexCube(cube)));
}

export function optimalNextMovesCube(policy, cube) {
    return optimalNextMoves(policy, indexCube(cube));
}

