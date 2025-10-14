import { http, HttpResponse } from 'msw';

export const configHandlers = [
  // Placeholder config handlers - to be implemented as needed
  http.get('*/api/config', () => {
    return HttpResponse.json({
      success: true,
      config: {
        environment: 'test',
        features: {},
        version: '1.0.0'
      }
    });
  })
];