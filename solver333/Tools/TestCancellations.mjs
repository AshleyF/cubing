import { cancellationCases, reducePair, simplifyOrientation } from '../../site/lab/cancellations.js';
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

const rotationExamples = ["x2 y' x", "x y z", "z2 x' y", "y2 z x2"];
for (const source of rotationExamples) {
  const reduced = simplifyOrientation(source.split(' '));
  if (reduced.length > 2) throw Error(`Orientation did not simplify: ${source}`);
  if (state(source) !== state(reduced.join(' '))) throw Error(`Invalid orientation simplification: ${source} -> ${reduced.join(' ')}`);
}
const rotationTokens = ['x', "x'", 'x2', 'y', "y'", 'y2', 'z', "z'", 'z2'];
for (const first of rotationTokens) for (const second of rotationTokens) for (const third of rotationTokens) {
  const source = [first, second, third], reduced = simplifyOrientation(source);
  if (reduced.length > 2) throw Error(`Orientation did not simplify: ${source.join(' ')}`);
  if (state(source.join(' ')) !== state(reduced.join(' '))) throw Error(`Invalid orientation simplification: ${source.join(' ')} -> ${reduced.join(' ')}`);
}
console.log('Verified all three-rotation cube orientations reduce to at most two.');
