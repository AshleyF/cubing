import { execFileSync } from 'node:child_process';
import { readFile } from 'node:fs/promises';
import { resolve } from 'node:path';

const repo = resolve(import.meta.dirname, '..', '..');
const lab = resolve(repo, 'site', 'lab');
const solver = resolve(repo, 'solver333', 'bin', 'Release', 'net8.0', 'Solver.dll');
const patterns = JSON.parse(await readFile(resolve(lab, 'pattern-data.json'), 'utf8'));
const { setData } = await import(resolve(lab, 'solver', 'PatternData.js'));
setData(Object.keys(patterns), Object.values(patterns));
const { solve } = await import(resolve(lab, 'solver', 'BrowserSolver.js'));

const scramble = "R U R' U' F2 D L2 B U2 R2 F' U L' D2 B2 R U' F D' L";
const defaults = { co:0, cp:0, cmll:0, eo:0, lb:0, lf:0, rb:0, rf:0, center:0, fbOrder:0, sbOrder:0, colorNeutral:0 };
const configurations = [
  defaults,
  { ...defaults, lb:1, lf:1, rb:1, rf:1, fbOrder:1, sbOrder:1 },
  { ...defaults, lb:1, lf:1, rb:1, rf:1, fbOrder:1, sbOrder:1, cmll:1, eo:2, center:1 },
  { ...defaults, lb:1, lf:1, rb:1, rf:1, fbOrder:1, sbOrder:1, cmll:1, eo:2, center:1, colorNeutral:1 }
];

const flagNames = { center:'center-with-sb', fbOrder:'best-fb-order', sbOrder:'best-sb-order', colorNeutral:'x2y-color-neutral' };
for (const config of configurations) {
  const flags = Object.entries(config).map(([key, value]) => `--${flagNames[key] || key}=${value}`);
  const output = execFileSync('dotnet', [solver, scramble, ...flags], { cwd:resolve(repo, 'solver333'), encoding:'utf8' });
  const line = output.split('\n').find(value => value.startsWith('ROUX_RESULT|'));
  if (!line) throw Error('The .NET solver returned no result');
  const dotnet = JSON.parse(line.slice('ROUX_RESULT|'.length));
  const browser = solve(scramble, config);
  const normalize = result => ({ solution:result.solution, stages:result.stages.map(stage => ({ stage:stage.stage, moves:stage.moves })) });
  if (JSON.stringify(normalize(browser)) !== JSON.stringify(normalize(dotnet))) throw Error(`Browser solver differs for ${JSON.stringify(config)}`);
}

console.log(`Browser and .NET solvers agree for ${configurations.length} representative configurations.`);
