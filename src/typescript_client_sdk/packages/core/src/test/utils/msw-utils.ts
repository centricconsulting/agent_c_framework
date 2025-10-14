/**
 * MSW Utility Functions
 * Shared utilities for working with Mock Service Worker across packages
 */

import { http, HttpResponse, delay } from 'msw';
import type { RequestHandler } from 'msw';

/**
 * Common delay values for consistent timing
 */
export const DELAYS = {
  instant: 0,
  fast: 50,
  normal: 100,
  slow: 500,
  verySlow: 2000,
  timeout: 10000
} as const;

/**
 * Common HTTP status codes
 */
export const STATUS = {
  ok: 200,
  created: 201,
  accepted: 202,
  noContent: 204,
  badRequest: 400,
  unauthorized: 401,
  forbidden: 403,
  notFound: 404,
  conflict: 409,
  tooManyRequests: 429,
  serverError: 500,
  serviceUnavailable: 503
} as const;

/**
 * Create a successful JSON response
 */
export function successResponse(data: any, options?: {
  status?: number;
  headers?: Record<string, string>;
  delay?: number;
}) {
  const { status = STATUS.ok, headers = {}, delay: delayMs = DELAYS.normal } = options || {};
  
  return async () => {
    if (delayMs > 0) {
      await delay(delayMs);
    }
    
    return HttpResponse.json(
      {
        success: true,
        data
      },
      { status, headers }
    );
  };
}

/**
 * Create an error JSON response
 */
export function errorResponse(
  code: string,
  message: string,
  options?: {
    status?: number;
    details?: any;
    headers?: Record<string, string>;
    delay?: number;
  }
) {
  const { 
    status = STATUS.serverError, 
    details = {}, 
    headers = {}, 
    delay: delayMs = DELAYS.fast 
  } = options || {};
  
  return async () => {
    if (delayMs > 0) {
      await delay(delayMs);
    }
    
    return HttpResponse.json(
      {
        error: {
          code,
          message,
          details
        }
      },
      { status, headers }
    );
  };
}

/**
 * Create a handler that simulates network failure
 */
export function networkErrorHandler(pattern: string): RequestHandler {
  return http.all(pattern, () => {
    return HttpResponse.error();
  });
}

/**
 * Create a handler that simulates timeout
 */
export function timeoutHandler(pattern: string): RequestHandler {
  return http.all(pattern, async () => {
    await delay(DELAYS.timeout);
    return HttpResponse.error();
  });
}

/**
 * Create a paginated response
 */
export function paginatedResponse(
  items: any[],
  page: number = 1,
  perPage: number = 10
) {
  const start = (page - 1) * perPage;
  const end = start + perPage;
  const paginatedItems = items.slice(start, end);
  const totalPages = Math.ceil(items.length / perPage);
  
  return {
    items: paginatedItems,
    pagination: {
      page,
      per_page: perPage,
      total: items.length,
      total_pages: totalPages,
      has_next: page < totalPages,
      has_previous: page > 1
    }
  };
}

/**
 * Create a handler with authentication check
 */
export function authenticatedHandler(
  method: 'get' | 'post' | 'put' | 'patch' | 'delete',
  pattern: string,
  handler: (request: Request) => Promise<Response>
): RequestHandler {
  return http[method](pattern, async ({ request }) => {
    const authHeader = request.headers.get('Authorization');
    
    if (!authHeader || !authHeader.startsWith('Bearer ')) {
      return HttpResponse.json(
        {
          error: {
            code: 'UNAUTHORIZED',
            message: 'Authentication required'
          }
        },
        { status: STATUS.unauthorized }
      );
    }
    
    return handler(request);
  });
}

/**
 * Create handlers for CRUD operations
 */
export function createCRUDHandlers(
  resourceName: string,
  baseUrl: string = 'http://localhost:8080'
) {
  const resourcePath = `${baseUrl}/api/${resourceName}`;
  const items = new Map<string, any>();
  
  return [
    // List
    http.get(resourcePath, async ({ request }) => {
      await delay(DELAYS.normal);
      
      const url = new URL(request.url);
      const page = parseInt(url.searchParams.get('page') || '1');
      const perPage = parseInt(url.searchParams.get('per_page') || '10');
      
      const allItems = Array.from(items.values());
      
      return HttpResponse.json(
        {
          success: true,
          data: paginatedResponse(allItems, page, perPage)
        },
        { status: STATUS.ok }
      );
    }),
    
    // Get by ID
    http.get(`${resourcePath}/:id`, async ({ params }) => {
      await delay(DELAYS.fast);
      
      const { id } = params;
      const item = items.get(id as string);
      
      if (!item) {
        return HttpResponse.json(
          {
            error: {
              code: 'NOT_FOUND',
              message: `${resourceName} not found`
            }
          },
          { status: STATUS.notFound }
        );
      }
      
      return HttpResponse.json(
        {
          success: true,
          data: item
        },
        { status: STATUS.ok }
      );
    }),
    
    // Create
    http.post(resourcePath, async ({ request }) => {
      await delay(DELAYS.normal);
      
      const body = await request.json() as any;
      const id = `${resourceName}_${Date.now()}`;
      
      const item = {
        id,
        ...body,
        created_at: new Date().toISOString()
      };
      
      items.set(id, item);
      
      return HttpResponse.json(
        {
          success: true,
          data: item
        },
        { status: STATUS.created }
      );
    }),
    
    // Update
    http.patch(`${resourcePath}/:id`, async ({ params, request }) => {
      await delay(DELAYS.normal);
      
      const { id } = params;
      const body = await request.json() as any;
      const item = items.get(id as string);
      
      if (!item) {
        return HttpResponse.json(
          {
            error: {
              code: 'NOT_FOUND',
              message: `${resourceName} not found`
            }
          },
          { status: STATUS.notFound }
        );
      }
      
      const updated = {
        ...item,
        ...body,
        updated_at: new Date().toISOString()
      };
      
      items.set(id as string, updated);
      
      return HttpResponse.json(
        {
          success: true,
          data: updated
        },
        { status: STATUS.ok }
      );
    }),
    
    // Delete
    http.delete(`${resourcePath}/:id`, async ({ params }) => {
      await delay(DELAYS.fast);
      
      const { id } = params;
      
      if (!items.has(id as string)) {
        return HttpResponse.json(
          {
            error: {
              code: 'NOT_FOUND',
              message: `${resourceName} not found`
            }
          },
          { status: STATUS.notFound }
        );
      }
      
      items.delete(id as string);
      
      return HttpResponse.json(
        {
          success: true,
          message: `${resourceName} deleted successfully`
        },
        { status: STATUS.ok }
      );
    })
  ];
}

/**
 * Create a handler that tracks requests
 */
export class RequestTracker {
  private requests: Map<string, Request[]> = new Map();
  
  track(pattern: string): RequestHandler {
    return http.all(pattern, ({ request }) => {
      const key = `${request.method} ${pattern}`;
      const existing = this.requests.get(key) || [];
      this.requests.set(key, [...existing, request.clone()]);
      
      return HttpResponse.json({ success: true });
    });
  }
  
  getRequests(method: string, pattern: string): Request[] {
    return this.requests.get(`${method} ${pattern}`) || [];
  }
  
  getRequestCount(method: string, pattern: string): number {
    return this.getRequests(method, pattern).length;
  }
  
  wasRequested(method: string, pattern: string): boolean {
    return this.getRequestCount(method, pattern) > 0;
  }
  
  clear() {
    this.requests.clear();
  }
}

/**
 * Create handlers for file operations
 */
export function createFileHandlers(baseUrl: string = 'http://localhost:8080') {
  const uploadedFiles = new Map<string, any>();
  
  return [
    // Upload file
    http.post(`${baseUrl}/api/upload`, async ({ request }) => {
      await delay(DELAYS.slow);
      
      const formData = await request.formData();
      const file = formData.get('file') as File;
      
      if (!file) {
        return errorResponse('NO_FILE', 'No file provided', { 
          status: STATUS.badRequest 
        })();
      }
      
      const fileId = `file_${Date.now()}`;
      const fileData = {
        id: fileId,
        name: file.name,
        size: file.size,
        type: file.type,
        uploaded_at: new Date().toISOString()
      };
      
      uploadedFiles.set(fileId, fileData);
      
      return successResponse(fileData, { status: STATUS.created })();
    }),
    
    // Get file info
    http.get(`${baseUrl}/api/files/:id`, async ({ params }) => {
      await delay(DELAYS.fast);
      
      const { id } = params;
      const file = uploadedFiles.get(id as string);
      
      if (!file) {
        return errorResponse('FILE_NOT_FOUND', 'File not found', {
          status: STATUS.notFound
        })();
      }
      
      return successResponse(file)();
    }),
    
    // Delete file
    http.delete(`${baseUrl}/api/files/:id`, async ({ params }) => {
      await delay(DELAYS.fast);
      
      const { id } = params;
      
      if (!uploadedFiles.has(id as string)) {
        return errorResponse('FILE_NOT_FOUND', 'File not found', {
          status: STATUS.notFound
        })();
      }
      
      uploadedFiles.delete(id as string);
      
      return successResponse({ message: 'File deleted' })();
    })
  ];
}

/**
 * Batch create handlers from config
 */
export function createHandlersFromConfig(
  config: Array<{
    method: 'get' | 'post' | 'put' | 'patch' | 'delete';
    path: string;
    response: any;
    status?: number;
    delay?: number;
    error?: boolean;
  }>,
  baseUrl: string = 'http://localhost:8080'
): RequestHandler[] {
  return config.map(({ method, path, response, status, delay: delayMs, error }) => {
    const fullPath = `${baseUrl}${path}`;
    
    return http[method](fullPath, async () => {
      if (delayMs) {
        await delay(delayMs);
      }
      
      if (error) {
        return HttpResponse.json(
          { error: response },
          { status: status || STATUS.serverError }
        );
      }
      
      return HttpResponse.json(
        { success: true, data: response },
        { status: status || STATUS.ok }
      );
    });
  });
}