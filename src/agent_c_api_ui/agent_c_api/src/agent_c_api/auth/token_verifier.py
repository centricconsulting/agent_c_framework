import httpx
from jose import jwt
from fastapi import HTTPException

class TokenVerifier:
    """Class to verify Microsoft Azure AD JWT tokens"""
    
    def __init__(self, client_id, tenant_id):
        """Initialize the token verifier with Azure AD configuration
        
        Args:
            client_id (str): Azure AD client ID
            tenant_id (str): Azure AD tenant ID
        """
        self.client_id = client_id
        self.tenant_id = tenant_id
        self.authority = f"https://login.microsoftonline.com/{tenant_id}"
        self.openid_config_url = f"{self.authority}/v2.0/.well-known/openid-configuration"
        self.algorithms = ["RS256"]
        self._load_config()
    
    def _load_config(self):
        """Load OpenID configuration and JWKS"""
        try:
            response = httpx.get(self.openid_config_url)
            response.raise_for_status()
            self.config = response.json()
            self.issuer = self.config["issuer"]
            self.jwks_uri = self.config["jwks_uri"]
        except Exception as e:
            raise RuntimeError(f"Failed to load OpenID configuration: {str(e)}")
    
    def _get_jwks(self):
        """Fetch JWKS from the jwks_uri"""
        try:
            response = httpx.get(self.jwks_uri)
            response.raise_for_status()
            return response.json()["keys"]
        except Exception as e:
            raise RuntimeError(f"Failed to fetch JWKS: {str(e)}")
    
    def verify(self, token):
        """Verify the JWT token
        
        Args:
            token (str): JWT token to verify
            
        Returns:
            dict: Token claims if valid
            
        Raises:
            HTTPException: If token is invalid
        """
        try:
            # Get the token header
            header = jwt.get_unverified_header(token)
            
            # Fetch JWKS and find the matching key
            jwks = self._get_jwks()
            key = next((k for k in jwks if k["kid"] == header["kid"]), None)
            
            if not key:
                raise HTTPException(status_code=401, detail="Invalid token: Key ID not found")
            
            # Verify the token
            claims = jwt.decode(
                token, 
                key, 
                algorithms=self.algorithms, 
                audience=self.client_id,
                issuer=self.issuer
            )
            
            return claims
        except jwt.ExpiredSignatureError:
            raise HTTPException(status_code=401, detail="Token has expired")
        except jwt.JWTClaimsError:
            raise HTTPException(status_code=401, detail="Invalid claims")
        except jwt.JWTError:
            raise HTTPException(status_code=401, detail="Invalid token signature")
        except Exception as e:
            raise HTTPException(status_code=401, detail=f"Token validation failed: {str(e)}")