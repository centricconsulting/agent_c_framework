// Simple test runner script to get better output
const { exec } = require('child_process');

exec('pnpm test -- src/components/audio/__tests__/AudioControlsPanel.test.tsx --reporter=verbose', (error, stdout, stderr) => {
  console.log('=== TEST RESULTS ===');
  if (stdout) console.log(stdout);
  if (stderr) console.log('\n=== WARNINGS/ERRORS ===\n', stderr);
  console.log('\n=== EXIT CODE ===', error ? error.code : 0);
});