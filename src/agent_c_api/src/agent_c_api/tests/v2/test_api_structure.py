import pytest
from fastapi.testclient import TestClient
from fastapi import FastAPI

from agent_c_api.api.v2 import router as v2_router
from agent_c_api.core.setup import create_application
from agent_c_api.api import router as main_router


def test_v2_router_has_expected_routes():
    """Test that all expected v2 API routes are registered."""
    # Get all defined routes
    routes = v2_router.routes
    route_paths = [route.path for route in routes]
    
    # Check for expected route patterns
    config_routes = [path for path in route_paths if '/config' in path]
    sessions_routes = [path for path in route_paths if '/sessions' in path]
    history_routes = [path for path in route_paths if '/history' in path]
    debug_routes = [path for path in route_paths if '/debug' in path]
    
    # Verify each resource category has routes
    assert len(config_routes) > 0, "Config routes not found"
    assert len(sessions_routes) > 0, "Sessions routes not found"
    assert len(history_routes) > 0, "History routes not found"
    assert len(debug_routes) > 0, "Debug routes not found"
    
    # Log the routes for debugging
    print(f"Found {len(routes)} total routes in v2 API")
    print(f"Config routes: {len(config_routes)}")
    print(f"Sessions routes: {len(sessions_routes)}")
    print(f"History routes: {len(history_routes)}")
    print(f"Debug routes: {len(debug_routes)}")


def test_versioning_implementation():
    """Test that proper API versioning is implemented."""
    # Create a test application
    from fastapi_versioning import VersionedFastAPI
    from fastapi import FastAPI
    
    # Create a mini test app
    base_app = FastAPI()
    base_app.include_router(main_router)
    
    # Apply versioning
    app = VersionedFastAPI(base_app,
                         version_format='{major}',
                         prefix_format='/api/v{major}',
                         default_version=1,
                         enable_latest=True)
    
    # Check that versioned routes are created
    routes = [route.path for route in app.routes]
    
    # We should have paths with /api/v1/ and /api/v2/ prefixes
    v1_routes = [r for r in routes if r.startswith('/api/v1/')]
    v2_routes = [r for r in routes if r.startswith('/api/v2/')]
    latest_routes = [r for r in routes if r.startswith('/api/latest/')]
    
    # Verify we have routes for both versions and latest
    assert len(v1_routes) > 0, "No v1 API routes found"
    assert len(v2_routes) > 0, "No v2 API routes found"
    assert len(latest_routes) > 0, "No /api/latest routes found"
    
    # Log route counts for debugging
    print(f"Found {len(v1_routes)} v1 routes")
    print(f"Found {len(v2_routes)} v2 routes")
    print(f"Found {len(latest_routes)} latest routes")


def test_route_registration_in_app():
    """Test that routes are properly registered in the FastAPI application."""
    # Create a test application with versioning
    from fastapi_versioning import VersionedFastAPI
    
    base_app = FastAPI()
    base_app.include_router(main_router)
    app = VersionedFastAPI(base_app,
                         version_format='{major}',
                         prefix_format='/api/v{major}',
                         default_version=1,
                         enable_latest=True)
    
    # Use TestClient to verify routes
    client = TestClient(app)
    
    # Define expected endpoint paths to test
    # We'll test one from each resource category with versioning prefix
    expected_endpoints = [
        "/api/v2/config/models",           # Config endpoint
        "/api/v2/sessions",               # Sessions listing endpoint
        "/api/v2/history",               # History listing endpoint
        "/api/v2/debug/sessions/123"      # Debug endpoint with placeholder ID
    ]
    
    # Check OpenAPI docs endpoint is accessible
    response = client.get("/openapi.json")
    assert response.status_code == 200, "OpenAPI schema not accessible"
    
    # Extract paths from OpenAPI schema
    openapi_schema = response.json()
    api_paths = openapi_schema.get("paths", {})
    
    # Print all available paths for debugging
    print("Available API paths:")
    for path in api_paths.keys():
        print(f"  {path}")
    
    # Verify each expected endpoint is in the OpenAPI schema
    for endpoint in expected_endpoints:
        # With versioning, the paths will have a different structure
        # We need to check if any path contains our endpoint components
        endpoint_parts = endpoint.split('/')
        endpoint_parts = [p for p in endpoint_parts if p]  # Remove empty strings
        
        # Check if any OpenAPI path contains all our endpoint parts
        found = False
        for api_path in api_paths.keys():
            matches = True
            for part in endpoint_parts:
                # Skip the ID part which will be {session_id} in the OpenAPI schema
                if part != '123' and part not in api_path:
                    matches = False
                    break
            if matches:
                found = True
                break
                
        assert found, f"Endpoint components {endpoint_parts} not found in OpenAPI schema"