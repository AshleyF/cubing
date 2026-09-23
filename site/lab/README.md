# RuLab

RuLab is the interactive Roux pattern laboratory published by GitHub Pages at:

https://ashleyf.github.io/cubing/site/lab/

It is intentionally static. The F# solver is compiled to JavaScript with Fable and runs in `solver-worker.js`, so changing a scramble or method option never requires a web server.

## Rebuild

From the repository root:

```sh
site/lab/build.sh
```

This regenerates:

- `pattern-data.json` from the canonical pattern files under `solver333/Patterns/`;
- `patterns.json` for the pattern explorer;
- `solver/`, the Fable-compiled browser version of the F# solver.

The ordinary .NET command-line solver and benchmark tools remain under `solver333/`.

## Local static test

Serve the repository root with any plain static server and open `/site/lab/`. Testing through the old Node API server is not sufficient because production is GitHub Pages.
