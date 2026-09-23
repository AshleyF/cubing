import { find, ofArray, empty } from "./fable_modules/fable-library-js.4.16.0/Map.js";
import { comparePrimitives } from "./fable_modules/fable-library-js.4.16.0/Util.js";
import { map2 } from "./fable_modules/fable-library-js.4.16.0/Array.js";
import { ofArray as ofArray_1 } from "./fable_modules/fable-library-js.4.16.0/List.js";

let patterns = empty({
    Compare: comparePrimitives,
});

export function setData(keys, values) {
    patterns = ofArray(map2((key, rows) => [key, ofArray_1(rows)], keys, values), {
        Compare: comparePrimitives,
    });
}

export function read(key) {
    return find(key, patterns);
}

