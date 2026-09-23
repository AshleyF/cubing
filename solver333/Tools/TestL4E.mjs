import { executeSteps, solved } from '../../site/lab/solver/library/Cube.js';
import { cubeToString, stringToSteps } from '../../site/lab/solver/library/Render.js';

const state = algorithm => cubeToString(executeSteps(stringToSteps(algorithm), solved));
const solvedState = cubeToString(solved);
const entry = "M U2 M2 U2 M U2 M U2 M";
const direct = "M U2 M' U2 M2";
const repetitive = "M' U2 M' U2 M' U2 M2 U2 M'";

if (state(`${entry} ${direct}`) !== solvedState) {
  throw Error(`Direct L4E regression does not solve the reported state: ${direct}`);
}
if (state(`${entry} ${repetitive}`) !== solvedState) {
  throw Error(`Reported repetitive L4E sequence does not solve its entry state: ${repetitive}`);
}

console.log('Verified reported L4E state uses a valid five-move direct solution.');
