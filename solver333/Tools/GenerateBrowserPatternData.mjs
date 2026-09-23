import { readdir, readFile, writeFile } from 'node:fs/promises';
import { join, relative, resolve } from 'node:path';

const root = resolve(import.meta.dirname, '..', 'Patterns');
const output = resolve(import.meta.dirname, '..', '..', 'site', 'lab', 'pattern-data.json');
const bridge = resolve(import.meta.dirname, '..', 'PatternData.fs');

async function filesBelow(directory) {
  const entries = await readdir(directory, { withFileTypes: true });
  const nested = await Promise.all(entries.map(entry => {
    const path = join(directory, entry.name);
    return entry.isDirectory() ? filesBelow(path) : path.endsWith('.txt') ? [path] : [];
  }));
  return nested.flat();
}

const patterns = {};
for (const file of (await filesBelow(root)).sort()) {
  const key = relative(root, file).replaceAll('\\', '/').replace(/\.txt$/, '');
  const lines = (await readFile(file, 'utf8')).split(/\r?\n/).filter(Boolean);
  patterns[key] = lines;
}

await writeFile(output, `${JSON.stringify(patterns)}\n`);
await writeFile(bridge, `module PatternData\n\nlet mutable private patterns : Map<string, string list> = Map.empty\n\nlet setData (keys: string array) (values: string array array) =\n    patterns <- Array.map2 (fun key rows -> key, List.ofArray rows) keys values |> Map.ofArray\n\nlet read key = Map.find key patterns\n`);
console.log(`Wrote ${output} with ${Object.keys(patterns).length} pattern sets.`);
