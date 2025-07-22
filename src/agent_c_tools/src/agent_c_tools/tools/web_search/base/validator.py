"""
Parameter validation for web search requests.

This module provides comprehensive validation of search parameters to ensure
they meet engine requirements and are properly formatted before execution.
"""

import re
from typing import Dict, Any, List, Optional, Union
from datetime import datetime, date
from urllib.parse import urlparse
from agent_c.util.structured_logging import get_logger

from .models import SearchParameters, SearchType, SafeSearchLevel, SearchDepth

logger = get_logger(__name__)


class ValidationError(Exception):
    """Exception raised when parameter validation fails."""
    
    def __init__(self, message: str, parameter: str = None, value: Any = None):
        super().__init__(message)
        self.parameter = parameter
        self.value = value


class ParameterValidator:
    """Validates and normalizes search parameters."""
    
    # Validation rules and constraints
    VALIDATION_RULES = {
        'query': {
            'type': str,
            'required': True,
            'min_length': 1,
            'max_length': 2048,
            'strip_whitespace': True
        },
        'engine': {
            'type': str,
            'required': False,
            'default': 'auto',
            'allowed_values': ['auto']  # Will be extended with available engines
        },
        'max_results': {
            'type': int,
            'required': False,
            'default': 10,
            'min_value': 1,
            'max_value': 100
        },
        'safesearch': {
            'type': str,
            'required': False,
            'default': 'moderate',
            'allowed_values': ['on', 'moderate', 'off']
        },
        'language': {
            'type': str,
            'required': False,
            'default': 'en',
            'pattern': r'^[a-z]{2}$',
            'description': 'ISO 639-1 language code'
        },
        'region': {
            'type': str,
            'required': False,
            'default': 'us',
            'pattern': r'^[a-z]{2}$',
            'description': 'ISO 3166-1 alpha-2 country code'
        },
        'include_images': {
            'type': bool,
            'required': False,
            'default': False
        },
        'search_depth': {
            'type': str,
            'required': False,
            'default': 'standard',
            'allowed_values': ['basic', 'standard', 'advanced']
        },
        'page': {
            'type': int,
            'required': False,
            'default': 1,
            'min_value': 1,
            'max_value': 100
        }
    }
    
    # Domain validation pattern
    DOMAIN_PATTERN = re.compile(
        r'^(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?\.)*[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?$'
    )
    
    def __init__(self, available_engines: Optional[List[str]] = None):
        """
        Initialize the parameter validator.
        
        Args:
            available_engines: List of available engine names for validation
        """
        self.available_engines = available_engines or ['auto']
        self._update_engine_validation()
    
    def _update_engine_validation(self) -> None:
        """Update engine validation rules with available engines."""
        self.VALIDATION_RULES['engine']['allowed_values'] = ['auto'] + self.available_engines
    
    def update_available_engines(self, engines: List[str]) -> None:
        """
        Update the list of available engines for validation.
        
        Args:
            engines: List of available engine names
        """
        self.available_engines = engines
        self._update_engine_validation()
    
    def validate_parameters(self, raw_params: Dict[str, Any]) -> SearchParameters:
        """
        Validate and normalize search parameters.
        
        Args:
            raw_params: Raw parameter dictionary from user input
            
        Returns:
            Validated and normalized SearchParameters object
            
        Raises:
            ValidationError: If validation fails
        """
        try:
            # Start with a copy of raw parameters
            params = raw_params.copy()
            
            # Apply validation rules
            validated_params = {}
            
            for param_name, rules in self.VALIDATION_RULES.items():
                value = params.get(param_name)
                validated_value = self._validate_parameter(param_name, value, rules)
                if validated_value is not None:
                    validated_params[param_name] = validated_value
            
            # Handle special parameters not in basic rules
            self._validate_special_parameters(params, validated_params)
            
            # Create SearchParameters object
            search_params = self._create_search_parameters(validated_params)
            
            # Perform cross-parameter validation
            self._validate_parameter_combinations(search_params)
            
            logger.debug(f"Parameters validated successfully: {search_params.query[:50]}...")
            return search_params
            
        except ValidationError:
            raise
        except Exception as e:
            logger.error(f"Unexpected validation error: {e}")
            raise ValidationError(f"Parameter validation failed: {str(e)}")
    
    def _validate_parameter(self, name: str, value: Any, rules: Dict[str, Any]) -> Any:
        """
        Validate a single parameter against its rules.
        
        Args:
            name: Parameter name
            value: Parameter value
            rules: Validation rules for the parameter
            
        Returns:
            Validated and normalized value
            
        Raises:
            ValidationError: If validation fails
        """
        # Handle missing required parameters
        if value is None:
            if rules.get('required', False):
                raise ValidationError(f"Required parameter '{name}' is missing", name, value)
            elif 'default' in rules:
                return rules['default']
            else:
                return None
        
        # Type validation
        expected_type = rules.get('type')
        if expected_type and not isinstance(value, expected_type):
            try:
                # Attempt type conversion
                if expected_type == int:
                    value = int(value)
                elif expected_type == float:
                    value = float(value)
                elif expected_type == bool:
                    value = bool(value) if not isinstance(value, str) else value.lower() in ('true', '1', 'yes', 'on')
                elif expected_type == str:
                    value = str(value)
                else:
                    raise ValidationError(f"Parameter '{name}' must be of type {expected_type.__name__}", name, value)
            except (ValueError, TypeError):
                raise ValidationError(f"Parameter '{name}' cannot be converted to {expected_type.__name__}", name, value)
        
        # String-specific validations
        if isinstance(value, str):
            # Strip whitespace if specified
            if rules.get('strip_whitespace', False):
                value = value.strip()
            
            # Length validation
            if 'min_length' in rules and len(value) < rules['min_length']:
                raise ValidationError(f"Parameter '{name}' is too short (minimum {rules['min_length']} characters)", name, value)
            
            if 'max_length' in rules and len(value) > rules['max_length']:
                raise ValidationError(f"Parameter '{name}' is too long (maximum {rules['max_length']} characters)", name, value)
            
            # Pattern validation
            if 'pattern' in rules:
                pattern = re.compile(rules['pattern'])
                if not pattern.match(value):
                    description = rules.get('description', f"pattern {rules['pattern']}")
                    raise ValidationError(f"Parameter '{name}' does not match required format: {description}", name, value)
            
            # Allowed values validation
            if 'allowed_values' in rules and value not in rules['allowed_values']:
                allowed = ', '.join(rules['allowed_values'])
                raise ValidationError(f"Parameter '{name}' must be one of: {allowed}", name, value)
        
        # Numeric validations
        if isinstance(value, (int, float)):
            if 'min_value' in rules and value < rules['min_value']:
                raise ValidationError(f"Parameter '{name}' is too small (minimum {rules['min_value']})", name, value)
            
            if 'max_value' in rules and value > rules['max_value']:
                raise ValidationError(f"Parameter '{name}' is too large (maximum {rules['max_value']})", name, value)
        
        return value
    
    def _validate_special_parameters(self, raw_params: Dict[str, Any], validated_params: Dict[str, Any]) -> None:
        """
        Validate special parameters that require custom logic.
        
        Args:
            raw_params: Original raw parameters
            validated_params: Dictionary to add validated parameters to
        """
        # Validate search_type
        search_type_value = raw_params.get('search_type', 'web')
        try:
            if isinstance(search_type_value, str):
                validated_params['search_type'] = SearchType(search_type_value)
            elif isinstance(search_type_value, SearchType):
                validated_params['search_type'] = search_type_value
            else:
                raise ValueError(f"Invalid search type: {search_type_value}")
        except ValueError:
            valid_types = [st.value for st in SearchType]
            raise ValidationError(f"Invalid search_type. Must be one of: {', '.join(valid_types)}", 'search_type', search_type_value)
        
        # Validate domain lists
        for domain_param in ['include_domains', 'exclude_domains']:
            domains = raw_params.get(domain_param)
            if domains is not None:
                validated_domains = self._validate_domain_list(domains, domain_param)
                if validated_domains:
                    validated_params[domain_param] = validated_domains
        
        # Validate date parameters
        for date_param in ['start_date', 'end_date']:
            date_value = raw_params.get(date_param)
            if date_value is not None:
                validated_date = self._validate_date(date_value, date_param)
                if validated_date:
                    validated_params[date_param] = validated_date
        
        # Validate category (for news searches)
        category = raw_params.get('category')
        if category is not None:
            validated_params['category'] = self._validate_category(category)
        
        # Validate sort parameter (for news searches)
        sort_param = raw_params.get('sort')
        if sort_param is not None:
            validated_params['sort'] = self._validate_sort_parameter(sort_param)
        
        # Handle additional parameters
        additional_params = {}
        for key, value in raw_params.items():
            if key not in self.VALIDATION_RULES and key not in validated_params:
                additional_params[key] = value
        
        if additional_params:
            validated_params['additional_params'] = additional_params
    
    def _validate_domain_list(self, domains: Any, param_name: str) -> List[str]:
        """
        Validate a list of domains.
        
        Args:
            domains: Domain list to validate
            param_name: Parameter name for error messages
            
        Returns:
            Validated list of domains
        """
        if not isinstance(domains, (list, tuple)):
            raise ValidationError(f"Parameter '{param_name}' must be a list", param_name, domains)
        
        if len(domains) > 10:
            raise ValidationError(f"Parameter '{param_name}' cannot contain more than 10 domains", param_name, domains)
        
        validated_domains = []
        for domain in domains:
            if not isinstance(domain, str):
                raise ValidationError(f"Domain in '{param_name}' must be a string: {domain}", param_name, domain)
            
            domain = domain.strip().lower()
            if not domain:
                continue
            
            # Remove protocol if present
            if domain.startswith(('http://', 'https://')):
                parsed = urlparse(domain)
                domain = parsed.netloc or parsed.path
            
            # Validate domain format
            if not self.DOMAIN_PATTERN.match(domain):
                raise ValidationError(f"Invalid domain format in '{param_name}': {domain}", param_name, domain)
            
            validated_domains.append(domain)
        
        return validated_domains
    
    def _validate_date(self, date_value: Any, param_name: str) -> Optional[datetime]:
        """
        Validate and parse a date parameter.
        
        Args:
            date_value: Date value to validate
            param_name: Parameter name for error messages
            
        Returns:
            Parsed datetime object or None
        """
        if date_value is None:
            return None
        
        if isinstance(date_value, datetime):
            return date_value
        
        if isinstance(date_value, date):
            return datetime.combine(date_value, datetime.min.time())
        
        if isinstance(date_value, str):
            # Try to parse various date formats
            date_formats = [
                '%Y-%m-%d',
                '%Y-%m-%dT%H:%M:%S',
                '%Y-%m-%dT%H:%M:%SZ',
                '%Y-%m-%d %H:%M:%S',
                '%m/%d/%Y',
                '%d/%m/%Y'
            ]
            
            for fmt in date_formats:
                try:
                    return datetime.strptime(date_value, fmt)
                except ValueError:
                    continue
            
            raise ValidationError(f"Invalid date format for '{param_name}': {date_value}", param_name, date_value)
        
        raise ValidationError(f"Parameter '{param_name}' must be a date string or datetime object", param_name, date_value)
    
    def _validate_category(self, category: str) -> str:
        """Validate news category parameter."""
        valid_categories = [
            'business', 'entertainment', 'general', 'health', 
            'science', 'sports', 'technology', 'financial'
        ]
        
        if category.lower() not in valid_categories:
            raise ValidationError(f"Invalid category. Must be one of: {', '.join(valid_categories)}", 'category', category)
        
        return category.lower()
    
    def _validate_sort_parameter(self, sort_param: str) -> str:
        """Validate sort parameter for news searches."""
        valid_sorts = ['relevancy', 'popularity', 'publishedAt']
        
        if sort_param not in valid_sorts:
            raise ValidationError(f"Invalid sort parameter. Must be one of: {', '.join(valid_sorts)}", 'sort', sort_param)
        
        return sort_param
    
    def _create_search_parameters(self, validated_params: Dict[str, Any]) -> SearchParameters:
        """
        Create SearchParameters object from validated parameters.
        
        Args:
            validated_params: Dictionary of validated parameters
            
        Returns:
            SearchParameters object
        """
        # Convert string enums to enum objects
        safesearch_str = validated_params.get('safesearch', 'moderate')
        safesearch = SafeSearchLevel(safesearch_str)
        
        search_depth_str = validated_params.get('search_depth', 'standard')
        search_depth = SearchDepth(search_depth_str)
        
        return SearchParameters(
            query=validated_params['query'],
            engine=validated_params.get('engine', 'auto'),
            search_type=validated_params.get('search_type', SearchType.WEB),
            max_results=validated_params.get('max_results', 10),
            safesearch=safesearch,
            language=validated_params.get('language', 'en'),
            region=validated_params.get('region', 'us'),
            include_images=validated_params.get('include_images', False),
            include_domains=validated_params.get('include_domains'),
            exclude_domains=validated_params.get('exclude_domains'),
            search_depth=search_depth,
            start_date=validated_params.get('start_date'),
            end_date=validated_params.get('end_date'),
            page=validated_params.get('page', 1),
            additional_params=validated_params.get('additional_params', {})
        )
    
    def _validate_parameter_combinations(self, params: SearchParameters) -> None:
        """
        Validate parameter combinations and constraints.
        
        Args:
            params: SearchParameters object to validate
            
        Raises:
            ValidationError: If parameter combinations are invalid
        """
        # Validate date range
        if params.start_date and params.end_date:
            if params.start_date > params.end_date:
                raise ValidationError("start_date cannot be after end_date")
        
        # Validate that dates are not in the future for most search types
        if params.search_type in [SearchType.NEWS, SearchType.TRENDS]:
            now = datetime.now()
            if params.start_date and params.start_date > now:
                raise ValidationError("start_date cannot be in the future for news/trends searches")
            if params.end_date and params.end_date > now:
                raise ValidationError("end_date cannot be in the future for news/trends searches")
        
        # Validate domain lists don't overlap
        if params.include_domains and params.exclude_domains:
            overlap = set(params.include_domains) & set(params.exclude_domains)
            if overlap:
                overlapping = ', '.join(overlap)
                raise ValidationError(f"Domains cannot be in both include and exclude lists: {overlapping}")
    
    def get_validation_schema(self) -> Dict[str, Any]:
        """
        Get the complete validation schema for documentation purposes.
        
        Returns:
            Dictionary describing all validation rules
        """
        schema = {
            'basic_parameters': self.VALIDATION_RULES.copy(),
            'special_parameters': {
                'search_type': {
                    'type': 'string',
                    'allowed_values': [st.value for st in SearchType],
                    'default': 'web'
                },
                'include_domains': {
                    'type': 'array',
                    'items': {'type': 'string', 'format': 'domain'},
                    'max_items': 10
                },
                'exclude_domains': {
                    'type': 'array',
                    'items': {'type': 'string', 'format': 'domain'},
                    'max_items': 10
                },
                'start_date': {
                    'type': 'string',
                    'format': 'date',
                    'description': 'Start date for date-filtered searches'
                },
                'end_date': {
                    'type': 'string',
                    'format': 'date',
                    'description': 'End date for date-filtered searches'
                },
                'category': {
                    'type': 'string',
                    'allowed_values': ['business', 'entertainment', 'general', 'health', 'science', 'sports', 'technology', 'financial'],
                    'description': 'News category filter'
                },
                'sort': {
                    'type': 'string',
                    'allowed_values': ['relevancy', 'popularity', 'publishedAt'],
                    'description': 'Sort order for results'
                }
            },
            'available_engines': self.available_engines
        }
        
        return schema