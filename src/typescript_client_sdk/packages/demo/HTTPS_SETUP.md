# HTTPS Configuration for Demo App

## Overview
The demo app is configured to run with HTTPS using Next.js's built-in experimental HTTPS support with self-signed certificates for local development.

## Configuration Details

### Server Configuration
- **Method**: Next.js built-in `--experimental-https` flag
- **Port**: 3000 (default Next.js port)
- **Protocol**: HTTPS
- **URL**: https://localhost:3000

### Certificate Paths
The development server uses self-signed certificates located at (relative to demo directory):
- **Certificate**: `../../agent_c_config/localhost_self_signed.pem`
- **Private Key**: `../../agent_c_config/localhost_self_signed-key.pem`

### Scripts (in package.json)
- `pnpm dev` - Starts the HTTPS development server using Next.js experimental HTTPS
- `pnpm dev:http` - Fallback HTTP development server (standard Next.js dev)

## Usage

### Starting the HTTPS Server
```bash
pnpm dev
```

The server will start at https://localhost:3000 with automatic certificate configuration.

### Browser Certificate Warning
Since we're using self-signed certificates, you'll need to:
1. Navigate to https://localhost:3000
2. Accept the browser's certificate warning
3. Proceed to the site (usually under "Advanced" options)

### API Connection
The demo app is configured to connect to the API at:
- **API URL**: https://localhost:8080

This is configured in `.env.local`:
```env
NEXT_PUBLIC_API_URL=https://localhost:8080
```

## Implementation Details

### Next.js Experimental HTTPS
The demo uses Next.js's built-in HTTPS support instead of a custom server:

```json
{
  "scripts": {
    "dev": "next dev --experimental-https --experimental-https-key ../../agent_c_config/localhost_self_signed-key.pem --experimental-https-cert ../../agent_c_config/localhost_self_signed.pem"
  }
}
```

This approach:
- Reduces complexity by eliminating custom server code
- Maintains all Next.js development features (Fast Refresh, etc.)
- Uses the same certificate infrastructure as other services

### Authentication Service
The authentication service (`src/lib/auth.ts`) is configured to:
- Use secure cookies for JWT tokens
- Handle HTTPS connections to the API
- Support self-signed certificates in development

## Troubleshooting

### Certificate Not Found
If you see an error about missing certificates, ensure:
1. The `agent_c_config` directory exists at `../../agent_c_config/` relative to the demo directory
2. The certificate files have the correct names:
   - `localhost_self_signed.pem`
   - `localhost_self_signed-key.pem`

### Port Already in Use
If port 3000 is already in use, you can specify a different port:
```bash
pnpm dev -- --port 3001
```

### Fallback to HTTP
If you need to run without HTTPS temporarily:
```bash
pnpm dev:http
```

### API Connection Issues
If the demo can't connect to the API:
1. Ensure the API is running on https://localhost:8080
2. Check that both services use the same certificates
3. Verify `.env.local` has the correct `NEXT_PUBLIC_API_URL`

## Security Note
These self-signed certificates are for local development only. Never use them in production environments.

## Migration from Custom Server
This demo previously used a custom `server.js` file for HTTPS. It has been migrated to use Next.js's built-in experimental HTTPS support for:
- Simpler configuration
- Better integration with Next.js features
- Consistency with modern Next.js best practices