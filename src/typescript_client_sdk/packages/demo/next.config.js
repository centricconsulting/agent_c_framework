/** @type {import('next').NextConfig} */
const path = require('path');

const nextConfig = {
  // Transpile the UI package to handle framer-motion properly
  transpilePackages: ['@agentc/realtime-ui'],
  
  // Configure source maps for development (disabled in production for security)
  productionBrowserSourceMaps: false,
  
  webpack: (config, { dev, isServer }) => {
    // Add alias for @agentc packages to help resolve them when imported by UI package
    config.resolve.alias = {
      ...config.resolve.alias,
      '@agentc/realtime-core': path.resolve(__dirname, '../core/dist'),
      '@agentc/realtime-react': path.resolve(__dirname, '../react/dist'),
    };
    
    // Ignore source map warnings for browser extension files (React DevTools)
    config.ignoreWarnings = [
      ...(config.ignoreWarnings || []),
      {
        module: /installHook/,
      },
      {
        message: /Failed to parse source map/,
      },
    ];
    
    return config;
  },
}

module.exports = nextConfig