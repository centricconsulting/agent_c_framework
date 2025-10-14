import { http, HttpResponse } from 'msw';

export const authHandlers = [
  // Placeholder auth handlers - to be implemented as needed
  http.post('*/api/auth/login', () => {
    return HttpResponse.json({
      success: true,
      token: 'test-token',
      user: { id: 'test-user', name: 'Test User' }
    });
  }),
  http.post('*/api/auth/logout', () => {
    return HttpResponse.json({ success: true });
  })
];