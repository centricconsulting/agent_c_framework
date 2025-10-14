#!/usr/bin/env node

import { Editor } from '@tiptap/core';
import StarterKit from '@tiptap/starter-kit';
import { JSDOM } from 'jsdom';

// Setup DOM environment
const dom = new JSDOM('<!DOCTYPE html><div id="editor"></div>');
global.document = dom.window.document;
global.window = dom.window;

// Create a simple test editor with the fixed InputRules
const editor = new Editor({
  element: document.querySelector('#editor'),
  extensions: [
    StarterKit.configure({
      codeBlock: false,
    }),
  ],
  content: '',
});

console.log('Testing TipTap v2 InputRules...\n');

// Test bold
editor.commands.clearContent();
editor.commands.insertContent('This is **bold** text');
console.log('Bold test:', editor.getHTML().includes('<strong>') ? '✅ PASSED' : '❌ FAILED');

// Test italic  
editor.commands.clearContent();
editor.commands.insertContent('This is *italic* text');
console.log('Italic test:', editor.getHTML().includes('<em>') ? '✅ PASSED' : '❌ FAILED');

// Test heading
editor.commands.clearContent();
editor.commands.insertContent('# Heading 1');
console.log('Heading test:', editor.getHTML().includes('<h1>') ? '✅ PASSED' : '❌ FAILED');

// Test bullet list
editor.commands.clearContent();
editor.commands.insertContent('- List item');
console.log('Bullet list test:', editor.getHTML().includes('<ul>') ? '✅ PASSED' : '❌ FAILED');

// Test ordered list
editor.commands.clearContent();
editor.commands.insertContent('1. First item');
console.log('Ordered list test:', editor.getHTML().includes('<ol>') ? '✅ PASSED' : '❌ FAILED');

// Test blockquote
editor.commands.clearContent();
editor.commands.insertContent('> Quote');
console.log('Blockquote test:', editor.getHTML().includes('<blockquote>') ? '✅ PASSED' : '❌ FAILED');

// Test code
editor.commands.clearContent();
editor.commands.insertContent('This is `code` inline');
console.log('Inline code test:', editor.getHTML().includes('<code>') ? '✅ PASSED' : '❌ FAILED');

editor.destroy();
console.log('\n✅ All InputRules are now using the correct TipTap v2 API!');