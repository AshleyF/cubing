import fs from 'node:fs';
import { setData } from '../../site/lab/solver/PatternData.js';
import { executeSteps, solved } from '../../site/lab/solver/library/Cube.js';
import { cubeToString, stringToSteps } from '../../site/lab/solver/library/Render.js';

const patterns = JSON.parse(fs.readFileSync(new URL('../../site/lab/pattern-data.json', import.meta.url), 'utf8'));
setData(Object.keys(patterns), Object.values(patterns));
const { setLsePolicy, solve } = await import('../../site/lab/solver/BrowserSolver.js');

const policy = fs.readFileSync(new URL('../Data/lse-policy-v1.dat', import.meta.url));
if (policy.subarray(0, 4).toString() !== 'RLSE') throw Error('Invalid LSE policy header');
const slots = policy.readInt32LE(8), reachable = policy.readInt32LE(12), maximum = policy.readInt32LE(16);
const distances = new Uint8Array(slots), optimalMoves = new Uint8Array(slots);
for (let index = 0, offset = 20; index < slots; index++, offset += 2) {
  distances[index] = policy[offset];
  optimalMoves[index] = policy[offset + 1];
}
setLsePolicy(distances, optimalMoves, reachable, maximum);

const scrambles = [
  "R' L B D B F2 U' F' B2 R2 F B' U' L F2 D2 U' R2 B D'",
  "F R2 B' U2 L D' F2 R U' B2 L2 D F' U R' B D2 L' U2 F2",
  "U2 R F2 D' L2 B U R2 F' D2 B2 L U' F R' D B' U2 L2 R2"
];
const config = { co: 0, cp: 0, cmll: 1, eo: 2, lb: 1, lf: 1, rb: 1, rf: 1, center: 1, fbOrder: 1, sbOrder: 1, colorNeutral: 0, lse: 1 };
const rouxSolved = 'OOOOOOOOOYYYYYYYYYBBBRRRGGGBBBRRRGGGBBBRRRGGGWWWWWWWWW';

for (const scramble of scrambles) {
  const result = solve(scramble, config);
  const lse = result.stages.filter(stage => stage.stage === 'OptimalLSE');
  if (lse.length !== 1) throw Error(`Expected one OptimalLSE stage, found ${lse.length}`);
  if (result.stages.some(stage => ['CenterOrientation', 'EdgeOrientation', 'EOLR', 'LToDF', 'LREdgesBottom', 'LREdges', 'L4E'].includes(stage.stage))) {
    throw Error('God mode emitted a human-oriented LSE stage');
  }
  const lseLength = lse[0].moves.trim().split(/\s+/).filter(Boolean).length;
  if (lseLength > maximum) throw Error(`Optimal LSE stage exceeded maximum table depth: ${lseLength}`);
  const cube = executeSteps(stringToSteps(`${scramble} ${result.solution}`), solved);
  if (cubeToString(cube) !== rouxSolved) throw Error(`God-mode solution did not solve scramble: ${scramble}`);
}

console.log(`Verified browser God-mode LSE on ${scrambles.length} full solves (policy depth ${maximum}).`);
