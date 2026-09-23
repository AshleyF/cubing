import { readdir, readFile } from 'node:fs/promises';
import { join, relative, resolve } from 'node:path';

const root = resolve(import.meta.dirname, '..', 'Patterns', 'Roux');

async function filesUnder(dir) {
  const entries = await readdir(dir, { withFileTypes: true });
  const nested = await Promise.all(entries.map(entry => {
    const path = join(dir, entry.name);
    return entry.isDirectory() ? filesUnder(path) : entry.name.endsWith('.txt') ? [path] : [];
  }));
  return nested.flat();
}

function moveCount(algorithm) {
  return algorithm.trim() ? algorithm.trim().split(/\s+/).length : 0;
}

async function analyze(path) {
  const lines = (await readFile(path, 'utf8')).split(/\r?\n/).filter(Boolean);
  const cases = lines.map(line => {
    const comma = line.indexOf(',');
    const algorithms = line.slice(comma + 1).split(';').filter(Boolean);
    const lengths = algorithms.map(moveCount);
    return { algorithms, best: lengths.length ? Math.min(...lengths) : 0 };
  });
  const learned = cases.filter(item => item.best > 0);
  const lengths = learned.map(item => item.best);
  return {
    source: relative(root, path),
    cases: cases.length,
    algorithms: new Set(cases.flatMap(item => item.algorithms)).size,
    averageBestMoves: lengths.length ? lengths.reduce((a, b) => a + b, 0) / lengths.length : 0,
    minBestMoves: lengths.length ? Math.min(...lengths) : 0,
    maxBestMoves: lengths.length ? Math.max(...lengths) : 0
  };
}

const report = await Promise.all((await filesUnder(root)).sort().map(analyze));
process.stdout.write(`${JSON.stringify(report, null, 2)}\n`);
