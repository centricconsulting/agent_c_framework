const { execSync } = require('child_process');
const path = require('path');

console.log('Testing Next.js build...');
console.log('Current directory:', process.cwd());
console.log('Next.js path check:', path.join(__dirname, 'node_modules/.bin/next'));

try {
  // Try to find Next.js
  const nextPath = require.resolve('next/dist/bin/next', { paths: [__dirname] });
  console.log('Found Next.js at:', nextPath);
} catch (error) {
  console.error('Could not find Next.js:', error.message);
}

// Try running the build
try {
  console.log('\nAttempting build...');
  execSync('npx next build', { stdio: 'inherit' });
  console.log('Build successful!');
} catch (error) {
  console.error('Build failed:', error.message);
}