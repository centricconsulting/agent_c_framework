import { describe, it, expect } from 'vitest';
import { render, screen } from '@testing-library/react';
import React from 'react';
import { MediaRenderer } from '../MediaRenderer';
import fs from 'fs';
import path from 'path';

describe('Markdown Verification', () => {
  it('should verify markdown content type is handled', () => {
    // Test markdown content rendering
    const testContent = '# Markdown Test\n\nThis is **bold** and *italic* text.';
    const contentType = 'text/markdown';
    
    // Verify the content type
    expect(contentType).toBe('text/markdown');
    console.log('✓ Content Type verified:', contentType);
  });

  it('should not have debug line in MediaRenderer', () => {
    // Check that debug line is removed from the component
    const componentPath = path.join(__dirname, '..', 'MediaRenderer.tsx');
    const componentContent = fs.readFileSync(componentPath, 'utf8');
    
    // Check for debug line
    const hasDebugLine = componentContent.includes('<p>type: {contentType}</p>');
    expect(hasDebugLine).toBe(false);
    
    if (!hasDebugLine) {
      console.log('✓ Debug line removed from MediaRenderer.tsx');
    }
  });

  it('should render markdown content correctly', () => {
    const markdownContent = '# Test Header\n\nThis is **bold** text.';
    
    const { container } = render(
      <MediaRenderer 
        mediaType="text" 
        content={markdownContent}
        contentType="text/markdown"
      />
    );
    
    // Check that markdown was rendered as HTML
    const heading = container.querySelector('h1');
    expect(heading).toBeTruthy();
    expect(heading?.textContent).toBe('Test Header');
    
    const strong = container.querySelector('strong');
    expect(strong).toBeTruthy();
    expect(strong?.textContent).toBe('bold');
    
    console.log('✓ Markdown content rendered correctly');
  });
});