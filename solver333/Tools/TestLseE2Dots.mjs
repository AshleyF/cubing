import fs from 'node:fs';
import { setData } from '../../site/lab/solver/PatternData.js';
import { executeMoves, executeSteps, solved } from '../../site/lab/solver/library/Cube.js';
import { cubeToString, movesToString, stringToSteps } from '../../site/lab/solver/library/Render.js';
import { length } from '../../site/lab/solver/fable_modules/fable-library-js.4.16.0/List.js';

const bytes = fs.readFileSync(new URL('../Data/lse-policy-v1.dat', import.meta.url));
const patterns = JSON.parse(fs.readFileSync(new URL('../../site/lab/pattern-data.json', import.meta.url), 'utf8'));
setData(Object.keys(patterns), Object.values(patterns));
const { setLsePolicy } = await import('../../site/lab/solver/BrowserSolver.js');
const { requirePolicy, solveCubeRelativeBestKnown } = await import('../../site/lab/solver/library/Lse.js?v=20260924-1');
if (bytes.subarray(0, 4).toString() !== 'RLSE') throw Error('Invalid LSE policy header');
const slots = bytes.readInt32LE(8), reachable = bytes.readInt32LE(12), maximum = bytes.readInt32LE(16);
const distances = new Uint8Array(slots), optimalMoves = new Uint8Array(slots);
for (let index = 0, offset = 20; index < slots; index++, offset += 2) {
  distances[index] = bytes[offset];
  optimalMoves[index] = bytes[offset + 1];
}
setLsePolicy(distances, optimalMoves, reachable, maximum);
const policy = requirePolicy();

const cases = [
  ["M E2 M E2", "E2 M' E2 M'"],
  ["M' E2 M E2", "E2 M' E2 M"]
];
for (const [setup, expected] of cases) {
  const state = executeSteps(stringToSteps(setup), solved);
  const solution = solveCubeRelativeBestKnown(policy, solved, state);
  if (length(solution) !== 4) throw Error(`Expected four moves for ${setup}, got ${movesToString(solution)}`);
  if (movesToString(solution) !== expected) throw Error(`Expected ${expected}, got ${movesToString(solution)}`);
  if (cubeToString(executeMoves(solution, state)) !== cubeToString(solved)) throw Error(`Shortcut did not solve ${setup}`);
}

console.log('Verified both four-move E2 dot shortcuts in the generated browser solver.');
