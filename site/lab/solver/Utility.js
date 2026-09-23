import { createAtom } from "./fable_modules/fable-library-js.4.16.0/Util.js";
import { empty, ofSeq } from "./fable_modules/fable-library-js.4.16.0/List.js";
import { map } from "./fable_modules/fable-library-js.4.16.0/Seq.js";
import { item } from "./fable_modules/fable-library-js.4.16.0/Array.js";
import { split } from "./fable_modules/fable-library-js.4.16.0/String.js";
import { read } from "./PatternData.js";

export const level = 0;

export let cornerOrientationLevel = createAtom(0);

export let cornerPermutationLevel = createAtom(0);

export let fullCmll = createAtom(false);

export let edgeOrientationLevel = createAtom(0);

export let lbPairLevel = createAtom(0);

export let lfPairLevel = createAtom(0);

export let rbPairLevel = createAtom(0);

export let rfPairLevel = createAtom(0);

export let orientCentersWithSecondBlock = createAtom(false);

export let chooseShortestSecondBlockPairOrder = createAtom(false);

export let chooseShortestFirstBlockPairOrder = createAtom(false);

export let x2yColorNeutral = createAtom(false);

export let useEolr = createAtom(false);

export function readPatterns(matchFn, method, level_1, name, cornerRotationNeutral, cornerColorNeutral, discoverAuf) {
    return ofSeq(map((x) => [matchFn, name, [item(0, x), cornerRotationNeutral, cornerColorNeutral, discoverAuf], (item(1, x).length === 0) ? empty() : ofSeq(split(item(1, x), [";"], void 0, 0))], map((line) => split(line, [","], void 0, 0), read(`${method}/${level_1}/${name}`))));
}

