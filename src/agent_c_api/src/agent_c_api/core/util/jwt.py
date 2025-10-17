import os
import sys
import argparse

from typing import Optional, TYPE_CHECKING
from fastapi import WebSocket, HTTPException, status, Request
from fastapi.security import HTTPBearer
from jose import jwt, JWTError
from datetime import datetime, timedelta, UTC

from agent_c_api.models.auth_models import ChatUserResponse

if TYPE_CHECKING:
    from agent_c.models.chat_history.user import ChatUser


# JWT Configuration
ALGORITHM = "HS256"

security = HTTPBearer()

def _jwt_secret_key() -> str:
    """Get the JWT secret key from environment variable"""
    secret_key = os.getenv("JWT_SECRET_KEY")
    if not secret_key:
        raise ValueError("JWT_SECRET_KEY environment variable is required")

    return secret_key

def create_jwt_token(user: 'ChatUser',  time_delta: Optional[timedelta] = None ) -> str:
    """Create a JWT token for a user"""
    if time_delta is None:
        time_delta = timedelta(hours=24)
    now = datetime.now(UTC)  # Python 3.11+ has UTC constant
    payload = {
        "sub": user.user_id,
        "user": ChatUserResponse.from_chat_user(user).model_dump(),
        "exp": now + time_delta,
        "iat": now,
    }
    return jwt.encode(payload, _jwt_secret_key(), algorithm=ALGORITHM)

def verify_jwt_token(token: str) -> dict:
    """Verify and decode JWT token"""
    try:
        payload = jwt.decode(token, _jwt_secret_key(), algorithms=[ALGORITHM])
        user_id = payload.get("sub")
        if user_id is None:
            raise HTTPException(status_code=401, detail="Invalid token")
        return {
            "user_id": user_id,
            "permissions": payload.get("permissions", []),
            "exp": payload.get("exp")
        }
    except JWTError:
        raise HTTPException(status_code=401, detail="Invalid token")

async def validate_request_jwt(request: Request) -> dict:
    """Extract and validate JWT from WebSocket headers or query params"""
    # Try Authorization header first
    auth_header = None
    for name, value in request.headers.items():
        if name.lower() == "authorization":
            auth_header = value
            break

    token = None
    if auth_header and auth_header.startswith("Bearer "):
        token = auth_header[7:]  # Remove "Bearer " prefix

    if not token:
        raise HTTPException(status_code=401, detail="Missing authorization")

    return verify_jwt_token(token)


async def validate_websocket_jwt(websocket: WebSocket) -> dict:
    """Extract and validate JWT from WebSocket headers or query params"""
    # Try Authorization header first
    auth_header = None
    for name, value in websocket.headers.items():
        if name.lower() == "authorization":
            auth_header = value
            break

    token = None
    if auth_header and auth_header.startswith("Bearer "):
        token = auth_header[7:]  # Remove "Bearer " prefix
    else:
        # Fallback to query parameter for browser clients
        token = websocket.query_params.get("token")

    if not token:
        await websocket.close(code=status.WS_1008_POLICY_VIOLATION, reason="Missing authorization")
        raise HTTPException(status_code=401, detail="Missing authorization")

    return verify_jwt_token(token)


def main():
    """Command-line interface for JWT token operations"""
    parser = argparse.ArgumentParser(description='Generate and verify JWT tokens')
    subparsers = parser.add_subparsers(dest='command', help='Available commands')
    
    # Generate token command
    gen_parser = subparsers.add_parser('generate', help='Generate a new JWT token')
    gen_parser.add_argument('user_id', help='User ID for the token')
    gen_parser.add_argument('--permissions', '-p', 
                           help='Comma-separated list of permissions (e.g., "read,write,admin")',
                           default='')
    gen_parser.add_argument('--hours', '-t', type=int, default=24,
                           help='Token expiration time in hours (default: 24)')
    
    # Verify token command
    verify_parser = subparsers.add_parser('verify', help='Verify and decode a JWT token')
    verify_parser.add_argument('token', help='JWT token to verify')
    
    args = parser.parse_args()
    
    if not args.command:
        parser.print_help()
        sys.exit(1)
    
    try:
        if args.command == 'generate':
            # Parse permissions
            permissions = [p.strip() for p in args.permissions.split(',') if p.strip()] if args.permissions else []
            
            # Generate token
            token = create_jwt_token(
                user_id=args.user_id,
                permissions=permissions,
                time_delta=timedelta(hours=args.hours)
            )
            
            print(f"Generated JWT token for user '{args.user_id}':")
            print(f"Permissions: {permissions if permissions else 'none'}")
            print(f"Expires in: {args.hours} hours")
            print(f"\nToken:")
            print(token)
            
        elif args.command == 'verify':
            # Verify token
            try:
                payload = verify_jwt_token(args.token)
                print("Token is valid!")
                print(f"User ID: {payload['user_id']}")
                print(f"Permissions: {payload['permissions']}")
                
                # Convert exp timestamp to readable format
                exp_timestamp = payload['exp']
                exp_datetime = datetime.fromtimestamp(exp_timestamp, UTC)
                print(f"Expires: {exp_datetime.isoformat()}")
                
                # Check if token is expired
                now = datetime.now(UTC)
                if exp_datetime < now:
                    print("⚠️  WARNING: Token is expired!")
                else:
                    time_left = exp_datetime - now
                    print(f"Time remaining: {time_left}")
                    
            except HTTPException as e:
                print(f"❌ Token verification failed: {e.detail}")
                sys.exit(1)
                
    except Exception as e:
        print(f"❌ Error: {e}")
        sys.exit(1)


if __name__ == '__main__':
    from dotenv import load_dotenv
    load_dotenv()
    main()
