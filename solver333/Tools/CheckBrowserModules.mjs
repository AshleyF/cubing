import { existsSync, readFileSync, readdirSync } from "node:fs";
import { dirname, join, resolve } from "node:path";
import { fileURLToPath } from "node:url";

const toolsDir = dirname(fileURLToPath(import.meta.url));
const solverDir = resolve(toolsDir, "../../site/lab/solver");
const modules = [];

function visit(directory) {
  for (const entry of readdirSync(directory, { withFileTypes: true })) {
    const path = join(directory, entry.name);
    if (entry.isDirectory()) visit(path);
    else if (entry.name.endsWith(".js")) modules.push(path);
  }
}

visit(solverDir);

const missing = [];
const relativeImport = /(?:from\s+|import\s*\()(["'])(\.\.?\/[^"']+)\1/g;
for (const module of modules) {
  const source = readFileSync(module, "utf8");
  for (const match of source.matchAll(relativeImport)) {
    const dependency = resolve(dirname(module), match[2].split(/[?#]/, 1)[0]);
    if (!existsSync(dependency)) missing.push(`${module}: ${match[2]}`);
  }
}

if (missing.length) {
  console.error("Missing browser module dependencies:\n" + missing.join("\n"));
  process.exit(1);
}

console.log(`Verified ${modules.length} browser modules.`);
