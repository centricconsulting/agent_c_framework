from fastapi import APIRouter, HTTPException, Depends, Path, Request
from typing import Optional, Dict, Any

from agent_c.models.chat.user import ChatUser
from .services import UserService

# Create router with prefix and tags
router = APIRouter(
    prefix="/users",  # Prefix matches the module name
    tags=["users"],    # Tag for grouping in OpenAPI docs
    responses={
        404: {
            "description": "User not found",
            "content": {
                "application/json": {
                    "example": {
                        "detail": {
                            "error": "USER_NOT_FOUND",
                            "error_code": "USER_NOT_FOUND",
                            "message": "User not found",
                            "params": {"user_id": "example_user_id"}
                        }
                    }
                }
            }
        },
        400: {
            "description": "Bad request",
            "content": {
                "application/json": {
                    "example": {
                        "detail": {
                            "error": "VALIDATION_ERROR",
                            "error_code": "VALIDATION_ERROR",
                            "message": "User ID mismatch"
                        }
                    }
                }
            }
        },
        500: {
            "description": "Internal server error",
            "content": {
                "application/json": {
                    "example": {
                        "detail": {
                            "error": "Internal server error",
                            "error_code": "SERVER_ERROR",
                            "message": "An unexpected error occurred while processing the request"
                        }
                    }
                }
            }
        }
    }
)

# Dependency for getting the user service
def get_user_service(request: Request):
    """Dependency to get the user service
    
    Args:
        request: The FastAPI request object
        
    Returns:
        UserService: Initialized user service
    """
    return UserService()

@router.post("/", 
           response_model=ChatUser,
           summary="Create New User",
           description="Creates a new user in the system.")
async def create_user(
    user: ChatUser,
    service: UserService = Depends(get_user_service)
):
    """Create a new user.
    
    This endpoint creates a new user with the provided information.
    
    Args:
        user: The user data to create
        
    Returns:
        ChatUser: The created user with system-generated fields
        
    Example:
        ```python
        import requests
        
        user_data = {
            "user_id": "john_doe",
            "email": "john.doe@example.com",
            "first_name": "John",
            "last_name": "Doe"
        }
        
        response = requests.post("https://your-agent-c-instance.com/api/v2/users/", json=user_data)
        created_user = response.json()
        ```
    """
    try:
        return await service.add_user(user)
    except Exception as e:
        raise HTTPException(
            status_code=500, 
            detail={
                "error": "Failed to create user",
                "error_code": "USER_CREATION_ERROR",
                "message": str(e)
            }
        )

@router.get("/{user_id}", 
           response_model=ChatUser,
           summary="Get User Details",
           description="Returns detailed information about a specific user.")
async def get_user(
    user_id: str = Path(..., description="The unique identifier of the user to retrieve"),
    service: UserService = Depends(get_user_service)
):
    """Get a user by ID.
    
    This endpoint retrieves detailed information about a specific user identified by their ID.
    
    Args:
        user_id: The unique identifier of the user to retrieve
        
    Returns:
        ChatUser: The requested user's information
        
    Raises:
        HTTPException: 404 error if the user is not found
        
    Example:
        ```python
        import requests
        
        user_id = "john_doe"
        response = requests.get(f"https://your-agent-c-instance.com/api/v2/users/{user_id}")
        user = response.json()
        ```
    """
    user = await service.get_user(user_id)
    if not user:
        raise HTTPException(
            status_code=404, 
            detail={
                "error": "USER_NOT_FOUND",
                "error_code": "USER_NOT_FOUND",
                "message": f"User {user_id} not found",
                "params": {"user_id": user_id}
            }
        )
    return user

@router.put("/{user_id}", 
           response_model=ChatUser,
           summary="Update User",
           description="Updates an existing user with the provided information.")
async def update_user(
    user_id: str = Path(..., description="The unique identifier of the user to update"),
    user: ChatUser = ...,
    service: UserService = Depends(get_user_service)
):
    """Update a user.
    
    This endpoint updates an existing user with the provided information.
    The user_id in the path must match the user_id in the request body.
    
    Args:
        user_id: The unique identifier of the user to update
        user: The updated user data
        
    Returns:
        ChatUser: The updated user information
        
    Raises:
        HTTPException: 400 error if user IDs don't match, 404 if user not found
        
    Example:
        ```python
        import requests
        
        user_id = "john_doe"
        user_data = {
            "user_id": "john_doe",
            "email": "john.new@example.com",
            "first_name": "John",
            "last_name": "Doe"
        }
        
        response = requests.put(f"https://your-agent-c-instance.com/api/v2/users/{user_id}", json=user_data)
        updated_user = response.json()
        ```
    """
    if user_id != user.user_id:
        raise HTTPException(
            status_code=400, 
            detail={
                "error": "USER_ID_MISMATCH",
                "error_code": "VALIDATION_ERROR",
                "message": "User ID in path must match user_id in request body",
                "params": {"path_id": user_id, "body_id": user.user_id}
            }
        )
        
    existing_user = await service.get_user(user_id)
    if not existing_user:
        raise HTTPException(
            status_code=404, 
            detail={
                "error": "USER_NOT_FOUND",
                "error_code": "USER_NOT_FOUND",
                "message": f"User {user_id} not found",
                "params": {"user_id": user_id}
            }
        )
        
    try:
        return await service.update_user(user)
    except Exception as e:
        raise HTTPException(
            status_code=500, 
            detail={
                "error": "Failed to update user",
                "error_code": "USER_UPDATE_ERROR",
                "message": str(e)
            }
        )

@router.delete("/{user_id}",
              summary="Delete User",
              description="Deletes a user from the system.")
async def delete_user(
    user_id: str = Path(..., description="The unique identifier of the user to delete"),
    service: UserService = Depends(get_user_service)
):
    """Delete a user.
    
    This endpoint deletes a user from the system.
    
    Args:
        user_id: The unique identifier of the user to delete
        
    Returns:
        dict: A message confirming successful deletion
        
    Raises:
        HTTPException: 404 error if the user is not found
        
    Example:
        ```python
        import requests
        
        user_id = "john_doe"
        response = requests.delete(f"https://your-agent-c-instance.com/api/v2/users/{user_id}")
        result = response.json()  # Should contain success message
        ```
    """
    success = await service.delete_user(user_id)
    if not success:
        raise HTTPException(
            status_code=404, 
            detail={
                "error": "USER_NOT_FOUND",
                "error_code": "USER_NOT_FOUND",
                "message": f"User {user_id} not found",
                "params": {"user_id": user_id}
            }
        )
    return {
        "status": "success",
        "message": "User deleted successfully",
        "user_id": user_id
    }