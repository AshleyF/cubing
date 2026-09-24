import fs from 'node:fs';
import { setData } from '../../site/lab/solver/PatternData.js';
import { Color, Face, Sticker, executeSteps, look, solved } from '../../site/lab/solver/library/Cube.js';
import { cubeToString, stringToSteps } from '../../site/lab/solver/library/Render.js';

const data = JSON.parse(fs.readFileSync(new URL('../../site/lab/pattern-data.json', import.meta.url), 'utf8'));
setData(Object.keys(data), Object.values(data));
const { expandPatternsForAuf } = await import('../../site/lab/solver/library/Solver.js');
const { lrIntermediatePatterns } = await import('../../site/lab/solver/Roux.js');

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

const face = tag => new Face(tag, []), sticker = tag => new Sticker(tag, []), color = tag => new Color(tag, []);
const is = (cube, faceTag, stickerTag, colorTag) => look(face(faceTag), sticker(stickerTag), cube).tag === color(colorTag).tag;
const isUpDown = (cube, faceTag, stickerTag) => [2, 3].includes(look(face(faceTag), sticker(stickerTag), cube).tag);
const matches = (pattern, cube) => [...cubeToString(cube)].every((value,index) => pattern[index] === '.' || pattern[index] === value);
const caseCoPattern = 'O.OO.O...Y.Y...Y.Y.........BBBR.RG.GBBBR.RGGGW.WW.WW.W';
const centersAndCorners = cube => matches(caseCoPattern,cube) &&
  look(face(2),sticker(0),cube).tag === look(face(2),sticker(2),cube).tag &&
  look(face(3),sticker(0),cube).tag === look(face(3),sticker(2),cube).tag &&
  isUpDown(cube,0,4);
const eo = cube => centersAndCorners(cube) && [[0,3],[0,1],[0,5],[0,7],[1,1],[1,7]].every(([f,s]) => isUpDown(cube,f,s));
const lrBottomEither = cube => eo(cube) && [[4,7,4,5,1,5],[4,7,5,5,1,4]].some(([ff,fs,fc,bf,bs,bc]) => is(cube,ff,fs,fc) && is(cube,bf,bs,bc));
const lrSolved = cube => eo(cube) && is(cube,2,1,4) && is(cube,3,1,5) && is(cube,2,0,4) && is(cube,3,2,5) && is(cube,2,2,4) && is(cube,3,0,5);
const eolrGoal = cube => lrBottomEither(cube) || lrSolved(cube);

const moveAlgorithms = ['M', "M'", 'M2', 'U', "U'", 'U2'];
const queue = [target], states = new Map([[cubeToString(target), target]]);
for (let index = 0; index < queue.length; index++) {
  const cube = queue[index];
  for (const move of moveAlgorithms) {
    const next = run(move, cube), key = cubeToString(next);
    if (!states.has(key)) { states.set(key, next); queue.push(next); }
  }
}

const patterns = Array.from(expandPatternsForAuf(lrIntermediatePatterns)).filter(pattern => pattern[1] === 'LREdges');
let admitted = 0;
for (const cube of states.values()) {
  if (!eolrGoal(cube)) continue;
  admitted++;
  const matching = patterns.filter(([match, , pattern]) => match(cube, pattern));
  if (!matching.length) throw Error(`Uncovered direct LR state: ${cubeToString(cube)}`);
  const candidates = matching.flatMap(pattern => {
    const algorithms = Array.from(pattern[3]);
    return algorithms.length ? algorithms : [''];
  });
  const valid = candidates.filter(algorithm => lrSolved(algorithm ? run(algorithm,cube) : cube));
  if (!valid.length) throw Error(`Direct LR patterns have no valid action for admitted state: ${cubeToString(cube)}`);
}

console.log(`Verified direct LR coverage for all ${admitted} EOLR goal states (${states.size} reachable LSE states).`);
