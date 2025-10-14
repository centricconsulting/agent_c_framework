/**
 * MSW Request Handlers for UI Package
 * Mock API endpoints for UI components testing
 */

import { http, HttpResponse, delay } from 'msw';
import { faker } from '@faker-js/faker';

/**
 * Base API URL
 */
const API_BASE_URL = process.env.VITE_API_URL || 'http://localhost:8080';

/**
 * Asset and theme handlers
 */
export const assetHandlers = [
  // Get theme configuration
  http.get(`${API_BASE_URL}/api/theme/config`, async () => {
    await delay(50);
    
    return HttpResponse.json(
      {
        success: true,
        data: {
          theme: 'light',
          primary_color: '#007bff',
          secondary_color: '#6c757d',
          font_family: 'Inter, system-ui, sans-serif',
          border_radius: '8px'
        }
      },
      { status: 200 }
    );
  }),

  // Get icons configuration
  http.get(`${API_BASE_URL}/api/assets/icons`, async () => {
    await delay(50);
    
    return HttpResponse.json(
      {
        success: true,
        data: {
          icon_set: 'heroicons',
          size: '24px',
          stroke_width: '1.5'
        }
      },
      { status: 200 }
    );
  })
];

/**
 * UI state handlers
 */
export const uiStateHandlers = [
  // Get UI preferences
  http.get(`${API_BASE_URL}/api/ui/preferences`, async () => {
    await delay(50);
    
    return HttpResponse.json(
      {
        success: true,
        data: {
          animations_enabled: true,
          reduced_motion: false,
          high_contrast: false,
          font_size: 'medium',
          sidebar_collapsed: false
        }
      },
      { status: 200 }
    );
  }),

  // Save UI state
  http.post(`${API_BASE_URL}/api/ui/state`, async ({ request }) => {
    await delay(50);
    
    const body = await request.json() as any;
    
    return HttpResponse.json(
      {
        success: true,
        data: {
          ...body,
          saved_at: new Date().toISOString()
        }
      },
      { status: 200 }
    );
  })
];

/**
 * Notification handlers
 */
export const notificationHandlers = [
  // Get notifications
  http.get(`${API_BASE_URL}/api/notifications`, async () => {
    await delay(50);
    
    return HttpResponse.json(
      {
        success: true,
        data: {
          notifications: [
            {
              id: faker.string.uuid(),
              type: 'info',
              title: 'Welcome!',
              message: 'Thanks for using our application',
              timestamp: faker.date.recent().toISOString(),
              read: false
            },
            {
              id: faker.string.uuid(),
              type: 'success',
              title: 'Connection established',
              message: 'You are now connected',
              timestamp: faker.date.recent().toISOString(),
              read: true
            }
          ]
        }
      },
      { status: 200 }
    );
  }),

  // Mark notification as read
  http.patch(`${API_BASE_URL}/api/notifications/:id/read`, async ({ params }) => {
    await delay(30);
    
    const { id } = params;
    
    return HttpResponse.json(
      {
        success: true,
        data: {
          notification_id: id,
          read: true,
          read_at: new Date().toISOString()
        }
      },
      { status: 200 }
    );
  })
];

/**
 * Form validation handlers
 */
export const validationHandlers = [
  // Validate form field
  http.post(`${API_BASE_URL}/api/validate/field`, async ({ request }) => {
    await delay(100);
    
    const body = await request.json() as any;
    
    // Simulate validation logic
    if (body.field === 'email' && !body.value.includes('@')) {
      return HttpResponse.json(
        {
          success: false,
          data: {
            valid: false,
            error: 'Invalid email format'
          }
        },
        { status: 200 }
      );
    }
    
    return HttpResponse.json(
      {
        success: true,
        data: {
          valid: true
        }
      },
      { status: 200 }
    );
  }),

  // Check username availability
  http.get(`${API_BASE_URL}/api/validate/username/:username`, async ({ params }) => {
    await delay(150);
    
    const { username } = params;
    
    // Simulate taken usernames
    const takenUsernames = ['admin', 'test', 'demo'];
    
    return HttpResponse.json(
      {
        success: true,
        data: {
          available: !takenUsernames.includes(username as string),
          suggestions: !takenUsernames.includes(username as string) 
            ? [] 
            : [`${username}123`, `${username}_user`, `real_${username}`]
        }
      },
      { status: 200 }
    );
  })
];

/**
 * File upload handlers
 */
export const uploadHandlers = [
  // Upload file
  http.post(`${API_BASE_URL}/api/upload`, async ({ request }) => {
    await delay(500);
    
    const formData = await request.formData();
    const file = formData.get('file') as File;
    
    if (!file) {
      return HttpResponse.json(
        {
          error: {
            code: 'NO_FILE',
            message: 'No file provided'
          }
        },
        { status: 400 }
      );
    }
    
    return HttpResponse.json(
      {
        success: true,
        data: {
          file_id: faker.string.uuid(),
          filename: file.name,
          size: file.size,
          type: file.type,
          url: faker.internet.url(),
          uploaded_at: new Date().toISOString()
        }
      },
      { status: 200 }
    );
  }),

  // Get upload progress
  http.get(`${API_BASE_URL}/api/upload/:id/progress`, async ({ params }) => {
    await delay(50);
    
    const { id } = params;
    
    return HttpResponse.json(
      {
        success: true,
        data: {
          upload_id: id,
          progress: faker.number.int({ min: 0, max: 100 }),
          status: faker.helpers.arrayElement(['uploading', 'processing', 'complete', 'failed'])
        }
      },
      { status: 200 }
    );
  })
];

/**
 * All handlers combined
 */
export const handlers = [
  ...assetHandlers,
  ...uiStateHandlers,
  ...notificationHandlers,
  ...validationHandlers,
  ...uploadHandlers
];

/**
 * UI component test scenarios
 */
export const uiTestScenarios = {
  // Dark theme
  darkTheme: [
    http.get(`${API_BASE_URL}/api/theme/config`, () => {
      return HttpResponse.json(
        {
          success: true,
          data: {
            theme: 'dark',
            primary_color: '#0d6efd',
            secondary_color: '#6c757d',
            background_color: '#1a1a1a',
            text_color: '#ffffff'
          }
        },
        { status: 200 }
      );
    })
  ],

  // High contrast mode
  highContrast: [
    http.get(`${API_BASE_URL}/api/ui/preferences`, () => {
      return HttpResponse.json(
        {
          success: true,
          data: {
            animations_enabled: false,
            reduced_motion: true,
            high_contrast: true,
            font_size: 'large'
          }
        },
        { status: 200 }
      );
    })
  ],

  // Many notifications
  manyNotifications: [
    http.get(`${API_BASE_URL}/api/notifications`, () => {
      const notifications = Array.from({ length: 20 }, (_, i) => ({
        id: faker.string.uuid(),
        type: faker.helpers.arrayElement(['info', 'success', 'warning', 'error']),
        title: faker.lorem.sentence(3),
        message: faker.lorem.sentence(),
        timestamp: faker.date.recent().toISOString(),
        read: i > 5
      }));
      
      return HttpResponse.json(
        {
          success: true,
          data: { notifications }
        },
        { status: 200 }
      );
    })
  ],

  // Form validation errors
  validationErrors: [
    http.post(`${API_BASE_URL}/api/validate/field`, () => {
      return HttpResponse.json(
        {
          success: false,
          data: {
            valid: false,
            error: faker.helpers.arrayElement([
              'This field is required',
              'Invalid format',
              'Value too short',
              'Value already exists'
            ])
          }
        },
        { status: 200 }
      );
    })
  ]
};