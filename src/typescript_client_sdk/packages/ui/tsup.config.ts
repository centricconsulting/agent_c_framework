import { defineConfig } from 'tsup';
import { cp } from 'fs/promises';
import { join } from 'path';

export default defineConfig({
  entry: ['src/index.ts'],
  format: ['cjs', 'esm'],
  dts: {
    resolve: true,
    entry: './src/index.ts',
  },
  splitting: false,
  sourcemap: true,
  clean: true,
  external: ['react', 'react-dom', '@agentc/realtime-react', 'tailwindcss'],
  tsconfig: './tsconfig.json',
  async onSuccess() {
    // Copy CSS files to dist
    await cp(
      join(__dirname, 'src/styles'),
      join(__dirname, 'dist/styles'),
      { recursive: true }
    );
    console.log('✓ Copied CSS files to dist/styles');
  },
});