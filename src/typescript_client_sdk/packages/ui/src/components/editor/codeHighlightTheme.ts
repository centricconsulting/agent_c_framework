/**
 * Code Highlighting Theme for CenSuite
 * Uses semantic color tokens from the design system
 */

export const codeHighlightStyles = `
  /* Code block container */
  .ProseMirror pre {
    background: hsl(var(--muted));
    border-radius: 0.375rem;
    padding: 1rem;
    overflow-x: auto;
    font-family: ui-monospace, SFMono-Regular, "SF Mono", Menlo, Consolas, "Liberation Mono", monospace;
    font-size: 0.875rem;
    line-height: 1.5;
    margin: 0.5rem 0;
  }

  .ProseMirror pre code {
    background: none;
    padding: 0;
    font-size: inherit;
    color: inherit;
    border-radius: 0;
  }

  /* Language label */
  .ProseMirror pre[data-language]::before {
    content: attr(data-language);
    position: absolute;
    top: 0.5rem;
    right: 0.5rem;
    padding: 0.125rem 0.5rem;
    background: hsl(var(--background));
    color: hsl(var(--muted-foreground));
    font-size: 0.75rem;
    border-radius: 0.25rem;
    font-family: inherit;
    text-transform: uppercase;
    opacity: 0.7;
  }

  .ProseMirror pre[data-language] {
    position: relative;
    padding-top: 2.5rem;
  }

  /* Highlight.js theme using CenSuite colors */
  
  /* Base text color */
  .hljs {
    color: hsl(var(--foreground));
  }

  /* Comments and documentation */
  .hljs-comment,
  .hljs-quote {
    color: hsl(var(--muted-foreground));
    font-style: italic;
  }

  /* Keywords, storage types, control flow */
  .hljs-keyword,
  .hljs-selector-tag,
  .hljs-addition,
  .hljs-function,
  .hljs-section,
  .hljs-shell {
    color: hsl(255 55% 40%); /* Primary darker */
    font-weight: 600;
  }

  /* Variables, parameters, attributes */
  .hljs-variable,
  .hljs-template-variable,
  .hljs-attribute,
  .hljs-tag,
  .hljs-name,
  .hljs-selector-id,
  .hljs-selector-class,
  .hljs-regexp,
  .hljs-link,
  .hljs-deletion {
    color: hsl(0 70% 50%); /* Semantic red */
  }

  /* Numbers, constants, boolean */
  .hljs-number,
  .hljs-built_in,
  .hljs-builtin-name,
  .hljs-literal,
  .hljs-type,
  .hljs-params,
  .hljs-meta,
  .hljs-link {
    color: hsl(30 100% 40%); /* Orange */
  }

  /* Strings */
  .hljs-string,
  .hljs-symbol,
  .hljs-bullet {
    color: hsl(140 50% 35%); /* Green */
  }

  /* Titles, function names, classes */
  .hljs-title,
  .hljs-class .hljs-title {
    color: hsl(255 55% 23.5%); /* Primary */
    font-weight: 600;
  }

  /* Strong emphasis */
  .hljs-strong {
    font-weight: bold;
  }

  /* Italic emphasis */
  .hljs-emphasis {
    font-style: italic;
  }

  /* Special highlighting for diffs */
  .hljs-addition {
    background-color: hsl(140 50% 95%);
    color: hsl(140 50% 25%);
  }

  .hljs-deletion {
    background-color: hsl(0 70% 95%);
    color: hsl(0 70% 35%);
  }

  /* Dark mode support (when data-theme="dark" is on root) */
  [data-theme="dark"] .ProseMirror pre {
    background: hsl(255 10% 10%);
  }

  [data-theme="dark"] .hljs {
    color: hsl(0 0% 90%);
  }

  [data-theme="dark"] .hljs-comment,
  [data-theme="dark"] .hljs-quote {
    color: hsl(255 5% 60%);
  }

  [data-theme="dark"] .hljs-keyword,
  [data-theme="dark"] .hljs-function {
    color: hsl(255 55% 70%);
  }

  [data-theme="dark"] .hljs-variable,
  [data-theme="dark"] .hljs-attribute {
    color: hsl(0 70% 70%);
  }

  [data-theme="dark"] .hljs-number,
  [data-theme="dark"] .hljs-literal {
    color: hsl(30 100% 65%);
  }

  [data-theme="dark"] .hljs-string {
    color: hsl(140 50% 60%);
  }

  [data-theme="dark"] .hljs-title {
    color: hsl(255 55% 80%);
  }

  /* Inline code styling to match */
  .ProseMirror code {
    background: hsl(var(--muted));
    padding: 0.125rem 0.375rem;
    border-radius: 0.25rem;
    font-family: ui-monospace, SFMono-Regular, "SF Mono", Menlo, Consolas, "Liberation Mono", monospace;
    font-size: 0.875em;
    color: hsl(var(--foreground));
  }

  /* Code block focus state */
  .ProseMirror pre:focus-visible {
    outline: 2px solid hsl(var(--ring));
    outline-offset: 2px;
  }

  /* Selection in code blocks */
  .ProseMirror pre::selection,
  .ProseMirror pre *::selection {
    background: hsl(255 55% 23.5% / 0.2);
  }

  /* Scrollbar styling for code blocks */
  .ProseMirror pre::-webkit-scrollbar {
    height: 8px;
    width: 8px;
  }

  .ProseMirror pre::-webkit-scrollbar-track {
    background: hsl(var(--muted));
    border-radius: 4px;
  }

  .ProseMirror pre::-webkit-scrollbar-thumb {
    background: hsl(var(--muted-foreground) / 0.3);
    border-radius: 4px;
  }

  .ProseMirror pre::-webkit-scrollbar-thumb:hover {
    background: hsl(var(--muted-foreground) / 0.5);
  }
`;

/**
 * Get language display name from language identifier
 */
export function getLanguageDisplayName(language: string): string {
  const languageMap: Record<string, string> = {
    javascript: 'JavaScript',
    js: 'JavaScript',
    typescript: 'TypeScript',
    ts: 'TypeScript',
    python: 'Python',
    py: 'Python',
    json: 'JSON',
    html: 'HTML',
    xml: 'XML',
    css: 'CSS',
    markdown: 'Markdown',
    md: 'Markdown',
    bash: 'Bash',
    sh: 'Shell',
    shell: 'Shell',
    yaml: 'YAML',
    yml: 'YAML',
    sql: 'SQL',
    dockerfile: 'Dockerfile',
    docker: 'Dockerfile',
    plaintext: 'Plain Text',
  };

  return languageMap[language.toLowerCase()] || language.toUpperCase();
}