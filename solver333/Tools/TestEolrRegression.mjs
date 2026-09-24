import fs from 'node:fs';
import { setData } from '../../site/lab/solver/PatternData.js';
import { Color, Face, Sticker, executeSteps, look, solved } from '../../site/lab/solver/library/Cube.js';
import { cubeToString, stepsToString, stringToSteps } from '../../site/lab/solver/library/Render.js';

const data = JSON.parse(fs.readFileSync(new URL('../../site/lab/pattern-data.json', import.meta.url), 'utf8'));
setData(Object.keys(data), Object.values(data));
const { chooseDirectLrAlgorithm } = await import('../../site/lab/solver/Roux.js');

const steps = algorithm => stringToSteps(algorithm);
const run = (algorithm, cube) => executeSteps(steps(algorithm), cube);
const targetState = 'OOOOOOOOOYYYYYYYYYBBBRRRGGGBBBRRRGGGBBBRRRGGGWWWWWWWWW';
const rotations = ['', 'x', "x'", 'x2', 'y', "y'", 'y2', 'z', "z'", 'z2'];
let target;
for (const a of rotations) for (const b of rotations) for (const c of rotations) {
  const algorithm = [a, b, c].filter(Boolean).join(' ');
  const candidate = algorithm ? run(algorithm, solved) : solved;
  if (cubeToString(candidate) === targetState) target = candidate;
}
if (!target) throw Error('Could not construct the Roux solved orientation');

const inverse = algorithm => algorithm.trim().split(/\s+/).reverse().map(move =>
  move.endsWith('2') ? move : move.endsWith("'") ? move.slice(0, -1) : `${move}'`
).join(' ');
const eolrPrefix = "U M U' M' U";
const oldLr = "M U2 M' U2 M2 U";
const directLr = "M' U2 M U'";
const l4e = "M U2 M2 U2 M'";
const entry = run(inverse(`${eolrPrefix} ${oldLr} ${l4e}`), target);
const afterEolr = run(eolrPrefix, entry);

const face = tag => new Face(tag, []), sticker = tag => new Sticker(tag, []), color = tag => new Color(tag, []);
const is = (cube, faceTag, stickerTag, colorTag) => look(face(faceTag), sticker(stickerTag), cube).tag === color(colorTag).tag;
const isUpDown = (cube, faceTag, stickerTag) => [2, 3].includes(look(face(faceTag), sticker(stickerTag), cube).tag);
const lrSolved = cube =>
  [[0,3],[0,1],[0,5],[0,7],[1,1],[1,7]].every(([f,s]) => isUpDown(cube,f,s)) &&
  is(cube,2,1,4) && is(cube,3,1,5) && is(cube,2,0,4) &&
  is(cube,3,2,5) && is(cube,2,2,4) && is(cube,3,0,5);

if (!lrSolved(run(directLr, afterEolr))) throw Error(`Direct LR continuation does not solve LR: ${directLr}`);
if (cubeToString(run(`${eolrPrefix} ${oldLr} ${l4e}`, entry)) !== targetState) throw Error('Reported suffix does not solve its reconstructed entry state');
const chosen = chooseDirectLrAlgorithm(lrSolved,afterEolr);
if (stepsToString(chosen) !== directLr) throw Error(`Pattern solver chose ${stepsToString(chosen)} instead of ${directLr}`);

console.log(`Verified reported EOLR state accepts the four-move direct LR case: ${directLr}`);
