const ready = (async () => {
  const { setData } = await import('./solver/PatternData.js');
  const patternData = await fetch('./pattern-data.json?v=20260923-4').then(response => {
    if (!response.ok) throw Error('Could not load solver patterns');
    return response.json();
  });
  setData(Object.keys(patternData), Object.values(patternData));
  return (await import('./solver/BrowserSolver.js?v=20260923-4')).solveWithProgress;
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
