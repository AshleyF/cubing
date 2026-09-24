const ready = (async () => {
  const { setData } = await import('./solver/PatternData.js');
  const patternData = await fetch('./pattern-data.json?v=20260923-4').then(response => {
    if (!response.ok) throw Error('Could not load solver patterns');
    return response.json();
  });
  setData(Object.keys(patternData), Object.values(patternData));
  const policyBuffer = await fetch('./lse-policy-v1.dat?v=20260923-1').then(response => {
    if (!response.ok) throw Error('Could not load the exact LSE policy');
    return response.arrayBuffer();
  });
  const view = new DataView(policyBuffer), bytes = new Uint8Array(policyBuffer);
  if (String.fromCharCode(...bytes.slice(0, 4)) !== 'RLSE') throw Error('Invalid exact LSE policy');
  const version = view.getInt32(4, true), slots = view.getInt32(8, true), reachable = view.getInt32(12, true), maximum = view.getInt32(16, true);
  if (version !== 1 || policyBuffer.byteLength !== 20 + slots * 2) throw Error('Unsupported exact LSE policy');
  const distances = new Uint8Array(slots), optimalMoves = new Uint8Array(slots);
  for (let index = 0, offset = 20; index < slots; index++, offset += 2) {
    distances[index] = bytes[offset];
    optimalMoves[index] = bytes[offset + 1];
  }
  const solver = await import('./solver/BrowserSolver.js?v=20260924-1');
  solver.setLsePolicy(distances, optimalMoves, reachable, maximum);
  return solver.solveWithProgress;
})();

self.onmessage = async event => {
  const { id, scramble, config } = event.data;
  try {
    const solve = await ready;
    const result = solve(scramble, config, (milestone, partial) => {
      self.postMessage({ id, progress: { milestone, result: partial } });
    });
    self.postMessage({ id, result });
  } catch (error) {
    self.postMessage({ id, error: error?.message || String(error) });
  }
};
