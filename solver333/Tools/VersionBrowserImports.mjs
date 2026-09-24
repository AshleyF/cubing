import fs from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';

const version = process.argv[2];
if (!version || !/^[A-Za-z0-9._-]+$/.test(version)) throw Error('Usage: VersionBrowserImports.mjs <version>');

const toolsDir = path.dirname(fileURLToPath(import.meta.url));
const solverDir = path.resolve(toolsDir, '../../site/lab/solver');
const targets = new Map([
  ['BrowserSolver.js', ['./Utility.js', './Roux.js', './library/Lse.js']],
  ['Roux.js', ['./Utility.js', './library/Lse.js']]
]);

for (const [file, dependencies] of targets) {
  const filePath = path.join(solverDir, file);
  let source = fs.readFileSync(filePath, 'utf8');
  for (const dependency of dependencies) {
    const escaped = dependency.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
    source = source.replace(new RegExp(`${escaped}(?:\\?v=[A-Za-z0-9._-]+)?`, 'g'), `${dependency}?v=${version}`);
  }
  fs.writeFileSync(filePath, source);
}

console.log(`Versioned generated browser solver imports with ${version}.`);
