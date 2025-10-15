# PLANE Docker Configuration

## Container Analysis

Based on `docker ps` output, here's your PLANE setup:

### Key Container: plane-app-proxy-1
```
Image: artifacts.plane.so/makeplane/plane-proxy:v1.0.0
Ports: 0.0.0.0:80->80/tcp, 0.0.0.0:443->443/tcp
```

**This is your entry point!** The proxy (Caddy) handles all incoming requests.

### PLANE Instance URL
✅ **Use: `http://localhost`** (port 80 is default, no need to specify)

Alternatively:
- `http://localhost:80` (explicit port)
- `https://localhost` (if SSL is configured)

### API Endpoint Structure
Based on standard PLANE setup:
- **Web UI:** `http://localhost/`
- **API Base:** `http://localhost/api/`
- **API v1:** `http://localhost/api/v1/`

### Backend Architecture
```
┌─────────────────────────────────────┐
│  Proxy (Caddy) - Port 80/443       │
│  plane-app-proxy-1                  │
└──────────┬──────────────────────────┘
           │
           ├──► Frontend (plane-app-web-1) - Port 3000 (internal)
           ├──► Admin (plane-app-admin-1) - Port 3000 (internal)
           ├──► Live (plane-app-live-1) - Port 3000 (internal)
           ├──► Space (plane-app-space-1) - Port 3000 (internal)
           └──► API (plane-app-api-1) - Port 8000 (internal)
                    │
                    ├──► Database (plane-db-1) - Port 5432
                    ├──► Redis (plane-redis-1) - Port 6379
                    ├──► RabbitMQ (plane-mq-1)
                    └──► MinIO (plane-minio-1) - Port 9000
```

### Supporting Services
- **PostgreSQL:** Database storage
- **Redis:** Caching and sessions
- **RabbitMQ:** Message queue for async tasks
- **MinIO:** Object storage for file uploads
- **Workers:** Background job processing

## Next Steps

1. **Verify PLANE is accessible:**
   - Open browser: `http://localhost`
   - Should see PLANE login/dashboard

2. **Get API Key:**
   - Log into PLANE web interface
   - Go to Settings → API Tokens
   - Create or copy your API key

3. **Get Workspace Info:**
   - When logged in, look at the URL
   - Format: `http://localhost/<workspace-slug>/projects`
   - Note your workspace slug

4. **Test Connection:**
   ```bash
   cd //plane/.scratch
   python test_plane_connection.py
   ```
   - Enter URL: `http://localhost`
   - Enter your API key
   - Verify all tests pass

## Configuration for Tools

Once verified, we'll use:
```yaml
plane:
  instance_url: "http://localhost"
  api_key: "YOUR_API_KEY_HERE"
  workspace_slug: "YOUR_WORKSPACE_SLUG"
  workspace_id: "YOUR_WORKSPACE_ID"
```

## Troubleshooting

If connection fails:
1. Check PLANE is accessible in browser: `http://localhost`
2. Verify all containers are running: `docker ps`
3. Check container logs: `docker logs plane-app-api-1`
4. Restart containers if needed: `docker compose restart`
