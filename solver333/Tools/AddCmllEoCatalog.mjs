import { readFile, writeFile } from 'node:fs/promises';
import { resolve } from 'node:path';
import { executeSteps, inverseSteps, solved } from '../../site/lab/solver/library/Cube.js';
import { cubeToString, stringToSteps } from '../../site/lab/solver/library/Render.js';

const solverDirectory = resolve(import.meta.dirname, '..');
const catalogPath = resolve(solverDirectory, '..', 'site', 'lab', 'patterns.json');
const algorithmsPath = resolve(solverDirectory, 'Patterns', 'Roux', 'Experimental', 'CmllEoCandidates.txt');
const catalog = JSON.parse(await readFile(catalogPath, 'utf8'));
const algorithms = (await readFile(algorithmsPath, 'utf8')).split(/\r?\n/).filter(Boolean);
const cases = algorithms.map(action => ({
  pattern: cubeToString(executeSteps(inverseSteps(stringToSteps(action)), solved)),
  action
}));
const entry = { id: 'cmllEo', cases };
const index = catalog.findIndex(item => item.id === entry.id);
if (index < 0) catalog.push(entry); else catalog[index] = entry;
await writeFile(catalogPath, `${JSON.stringify(catalog)}\n`);
console.log(`Wrote ${cases.length} CMLL + EO alternatives to ${catalogPath}`);
