import { http, HttpResponse } from 'msw';

export const audioHandlers = [
  // Placeholder audio handlers - to be implemented as needed
  http.post('*/api/audio/upload', () => {
    return HttpResponse.json({
      success: true,
      audioId: 'test-audio-id'
    });
  }),
  http.get('*/api/audio/:id', ({ params }) => {
    return HttpResponse.json({
      success: true,
      audioId: params.id,
      url: `https://test.example.com/audio/${params.id}`
    });
  })
];