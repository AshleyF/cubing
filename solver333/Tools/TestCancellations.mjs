import { cancellationCases, reducePair } from '../../site/lab/cancellations.js';
import { executeSteps, solved } from '../../site/lab/solver/library/Cube.js';
import { cubeToString, stringToSteps } from '../../site/lab/solver/library/Render.js';

if (reducePair("R'", 'r') !== "M'") throw Error("Expected R' r to reduce to M'");
if (reducePair("R'", 'Rw') !== "M'") throw Error("Expected R' Rw to reduce to M'");

const state = algorithm => algorithm ? cubeToString(executeSteps(stringToSteps(algorithm), solved)) : cubeToString(solved);
let checked = 0;
for (const [source, reduced] of cancellationCases()) {
  // The canonical F# parser uses lowercase wide moves; Rw spellings are UI aliases.
  if (source.includes('w')) continue;
  if (state(source) !== state(reduced)) throw Error(`Invalid cancellation: ${source} -> ${reduced || 'skip'}`);
  checked++;
}

console.log(`Verified ${checked} layer-aware cancellation cases.`);
