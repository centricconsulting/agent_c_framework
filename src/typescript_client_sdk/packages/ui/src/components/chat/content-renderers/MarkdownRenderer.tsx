'use client'

import * as React from 'react'
import { cn } from '../../../lib/utils'
import ReactMarkdown from 'react-markdown'
import remarkGfm from 'remark-gfm'
import remarkDirective from 'remark-directive'
import remarkMath from 'remark-math'
import remarkToc from 'remark-toc'
import rehypeRaw from 'rehype-raw'
import rehypeHighlight from 'rehype-highlight'
import rehypeKatex from 'rehype-katex'
import rehypeSanitize from 'rehype-sanitize'
import mermaid from 'mermaid'
import { defaultSchema } from 'rehype-sanitize'
import { visit } from 'unist-util-visit'
import '../../../styles/syntax-highlighting.css'
import 'katex/dist/katex.min.css'
import { Button } from '../../ui/button'
import { Check, Copy, ChevronRight, ChevronDown, Info, Lightbulb, Megaphone, AlertTriangle, ShieldAlert } from 'lucide-react'

// Initialize mermaid once
let mermaidInitialized = false
const initializeMermaid = () => {
  if (!mermaidInitialized) {
    mermaid.initialize({
      startOnLoad: false,
      theme: 'default',
      securityLevel: 'strict',
      fontFamily: 'inherit',
      logLevel: 'error'
    })
    mermaidInitialized = true
  }
}

/**
 * Slugify text for use as heading IDs
 * Matches GitHub's heading ID generation behavior
 */
const slugify = (text: string): string => {
  return text
    .toLowerCase()
    .trim()
    // Remove special characters except spaces and hyphens
    .replace(/[^a-z0-9\s-]/g, '')
    // Replace spaces with hyphens
    .replace(/\s+/g, '-')
    // Remove consecutive hyphens
    .replace(/-+/g, '-')
    // Remove leading/trailing hyphens
    .replace(/^-+|-+$/g, '')
}

/**
 * Extract text content from React children for heading IDs
 */
const extractTextFromChildren = (children: any): string => {
  if (typeof children === 'string') return children
  if (typeof children === 'number') return String(children)
  if (Array.isArray(children)) {
    return children.map(extractTextFromChildren).join('')
  }
  if (children?.props?.children) {
    return extractTextFromChildren(children.props.children)
  }
  return ''
}

/**
 * MermaidDiagram component for rendering Mermaid diagrams
 * Handles client-side rendering with error handling and theme support
 */
interface MermaidDiagramProps {
  code: string
  compact?: boolean
}

const MermaidDiagram: React.FC<MermaidDiagramProps> = ({ code, compact = false }) => {
  const [svg, setSvg] = React.useState<string>('')
  const [error, setError] = React.useState<string | null>(null)
  const containerRef = React.useRef<HTMLDivElement>(null)
  
  React.useEffect(() => {
    const renderDiagram = async () => {
      try {
        // Initialize mermaid if not already done
        initializeMermaid()
        
        // Generate unique ID for this diagram
        const id = `mermaid-${Math.random().toString(36).substr(2, 9)}`
        
        // Clean up any existing mermaid error elements before rendering
        // Mermaid can create orphaned error divs during failed renders
        const existingErrorDivs = document.querySelectorAll('[id^="d"][id*="mermaid"]')
        existingErrorDivs.forEach(div => {
          // Only remove if it's an orphaned error element (not in our container)
          if (!containerRef.current?.contains(div)) {
            div.remove()
          }
        })
        
        // Render the diagram
        const { svg } = await mermaid.render(id, code)
        setSvg(svg)
        setError(null)
        
        // Clean up the temporary div that mermaid.render creates
        const tempDiv = document.getElementById(id)
        if (tempDiv && !containerRef.current?.contains(tempDiv)) {
          tempDiv.remove()
        }
      } catch (err) {
        console.error('Mermaid rendering error:', err)
        setError(err instanceof Error ? err.message : 'Failed to render diagram')
        
        // Clean up any error divs that mermaid might have created
        const errorDivs = document.querySelectorAll('[id^="d"][id*="mermaid"]')
        errorDivs.forEach(div => {
          if (!containerRef.current?.contains(div)) {
            div.remove()
          }
        })
      }
    }
    
    renderDiagram()
    
    // Cleanup on unmount
    return () => {
      const orphanedDivs = document.querySelectorAll('[id^="d"][id*="mermaid"]')
      orphanedDivs.forEach(div => {
        if (!containerRef.current?.contains(div)) {
          div.remove()
        }
      })
    }
  }, [code])
  
  if (error) {
    return (
      <div
        className={cn(
          'border border-destructive bg-destructive/10 rounded-md',
          compact ? 'p-3 my-2' : 'p-4 my-4'
        )}
        role="alert"
      >
        <p className={cn(
          'text-destructive font-medium',
          compact ? 'text-xs mb-1' : 'text-sm mb-2'
        )}>
          Error rendering Mermaid diagram
        </p>
        <pre className={cn(
          'text-muted-foreground font-mono overflow-x-auto',
          compact ? 'text-xs' : 'text-xs'
        )}>
          {error}
        </pre>
        <details className="mt-2">
          <summary className={cn(
            'cursor-pointer text-muted-foreground hover:text-foreground',
            compact ? 'text-xs' : 'text-sm'
          )}>
            Show diagram code
          </summary>
          <pre className={cn(
            'mt-2 text-muted-foreground font-mono overflow-x-auto bg-muted p-2 rounded',
            compact ? 'text-xs' : 'text-xs'
          )}>
            {code}
          </pre>
        </details>
      </div>
    )
  }
  
  if (!svg) {
    return (
      <div
        className={cn(
          'bg-muted rounded-md flex items-center justify-center',
          compact ? 'p-4 my-2' : 'p-8 my-4'
        )}
        role="status"
        aria-live="polite"
      >
        <p className={cn(
          'text-muted-foreground',
          compact ? 'text-xs' : 'text-sm'
        )}>
          Rendering diagram...
        </p>
      </div>
    )
  }
  
  return (
    <div
      ref={containerRef}
      className={cn(
        'mermaid-diagram overflow-x-auto',
        compact ? 'my-2' : 'my-4'
      )}
      dangerouslySetInnerHTML={{ __html: svg }}
      role="img"
      aria-label="Mermaid diagram"
    />
  )
}

export interface MarkdownRendererProps {
  /**
   * The markdown content to render
   */
  content: string
  
  /**
   * Optional CSS classes to apply to the prose wrapper
   */
  className?: string
  
  /**
   * Optional role context for accessibility
   */
  ariaLabel?: string
  
  /**
   * Whether to enable code copy buttons (default: true)
   */
  enableCodeCopy?: boolean
  
  /**
   * Compact mode for thoughts/condensed displays (default: false)
   * - Reduces spacing
   * - Smaller text sizes
   * - Simplified code blocks
   */
  compact?: boolean
}

/**
 * Shared markdown renderer for all chat message types
 * Provides consistent markdown rendering across regular messages,
 * thoughts, and media content with full feature support including:
 * - Code blocks with copy functionality
 * - Tables with GFM support
 * - Lists, headings, links, blockquotes
 * - Images
 * - Full accessibility support
 */
export const MarkdownRenderer: React.FC<MarkdownRendererProps> = ({
  content,
  className,
  ariaLabel = 'Markdown content',
  enableCodeCopy = true,
  compact = false
}) => {
  const [copiedCode, setCopiedCode] = React.useState<string | null>(null)
  
  // Handle code copy
  const handleCopyCode = React.useCallback((code: string) => {
    navigator.clipboard.writeText(code).then(() => {
      setCopiedCode(code)
      setTimeout(() => setCopiedCode(null), 2000)
    }).catch(err => {
      console.error('Failed to copy code:', err)
    })
  }, [])
  
  // Custom remark plugin to convert directives to HTML
  const remarkDirectiveToHtml = React.useCallback(() => {
    return (tree: any) => {
      visit(tree, (node) => {
        if (
          node.type === 'containerDirective' ||
          node.type === 'leafDirective' ||
          node.type === 'textDirective'
        ) {
          const data = node.data || (node.data = {})
          const hName = 'div'
          const hProperties = {
            'data-directive': node.name,
            className: `directive directive-${node.name}`
          }
          
          data.hName = hName
          data.hProperties = hProperties
        }
      })
    }
  }, [])
  
  // Alert type configurations
  const alertConfig = React.useMemo(() => ({
    note: {
      icon: Info,
      label: 'Note',
      borderColor: 'border-l-blue-500',
      bgColor: 'bg-blue-50 dark:bg-blue-900/20',
      iconColor: 'text-blue-500',
      textColor: 'text-blue-900 dark:text-blue-100'
    },
    tip: {
      icon: Lightbulb,
      label: 'Tip',
      borderColor: 'border-l-green-500',
      bgColor: 'bg-green-50 dark:bg-green-900/20',
      iconColor: 'text-green-500',
      textColor: 'text-green-900 dark:text-green-100'
    },
    important: {
      icon: Megaphone,
      label: 'Important',
      borderColor: 'border-l-purple-500',
      bgColor: 'bg-purple-50 dark:bg-purple-900/20',
      iconColor: 'text-purple-500',
      textColor: 'text-purple-900 dark:text-purple-100'
    },
    warning: {
      icon: AlertTriangle,
      label: 'Warning',
      borderColor: 'border-l-yellow-500',
      bgColor: 'bg-yellow-50 dark:bg-yellow-900/20',
      iconColor: 'text-yellow-600 dark:text-yellow-500',
      textColor: 'text-yellow-900 dark:text-yellow-100'
    },
    caution: {
      icon: ShieldAlert,
      label: 'Caution',
      borderColor: 'border-l-red-500',
      bgColor: 'bg-red-50 dark:bg-red-900/20',
      iconColor: 'text-red-500',
      textColor: 'text-red-900 dark:text-red-100'
    }
  }), [])
  
  // Configure rehype-sanitize to allow only specific HTML tags
  // This filters out phantom tags like <prototype>, <anonymous>, <empty> that rehype-raw creates
  // Also allows highlight.js classes for syntax highlighting and directive classes for alerts
  // Also allows KaTeX classes for math rendering
  const sanitizeSchema = React.useMemo(() => ({
    ...defaultSchema,
    tagNames: [
      ...(defaultSchema.tagNames || []),
      'details',
      'summary',
      // KaTeX math elements
      'math',
      'semantics',
      'mrow',
      'mi',
      'mn',
      'mo',
      'mtext',
      'annotation',
      // Mermaid SVG elements
      'svg',
      'g',
      'path',
      'rect',
      'circle',
      'ellipse',
      'line',
      'polyline',
      'polygon',
      'text',
      'tspan',
      'textPath',
      'defs',
      'linearGradient',
      'radialGradient',
      'stop',
      'marker',
      'clipPath',
      'mask',
      'pattern',
      'image',
      'use',
      'foreignObject',
      'desc',
      'title',
      'style'
    ].filter(tag => !['prototype', 'anonymous', 'empty'].includes(tag)),
    attributes: {
      ...defaultSchema.attributes,
      // Allow all hljs-* classes for syntax highlighting
      '*': [
        ...(defaultSchema.attributes?.['*'] || []),
        'className',
        'dataDirective',
        'dataDirectiveLabel'
      ],
      div: [
        ...(defaultSchema.attributes?.div || []),
        ['className', /^directive/, /^alert-/, /^katex/],
        'dataDirective',
        'dataDirectiveLabel'
      ],
      span: [
        ...(defaultSchema.attributes?.span || []),
        ['className', /^hljs-/, /^katex/]
      ],
      code: [
        ...(defaultSchema.attributes?.code || []),
        ['className', /^language-/, /^hljs/]
      ],
      // KaTeX-specific attributes
      math: [
        'xmlns'
      ],
      annotation: [
        'encoding'
      ],
      // Mermaid SVG attributes
      svg: [
        'xmlns',
        'width',
        'height',
        'viewBox',
        'preserveAspectRatio',
        'style',
        'role',
        'aria-label',
        'aria-roledescription',
        ['className', /^mermaid/]
      ],
      g: [
        'transform',
        'className',
        'id',
        'style'
      ],
      path: [
        'd',
        'fill',
        'stroke',
        'strokeWidth',
        'strokeDasharray',
        'strokeLinecap',
        'strokeLinejoin',
        'transform',
        'className',
        'id',
        'style',
        'markerStart',
        'markerEnd'
      ],
      rect: [
        'x',
        'y',
        'width',
        'height',
        'rx',
        'ry',
        'fill',
        'stroke',
        'strokeWidth',
        'transform',
        'className',
        'id',
        'style'
      ],
      circle: [
        'cx',
        'cy',
        'r',
        'fill',
        'stroke',
        'strokeWidth',
        'transform',
        'className',
        'id',
        'style'
      ],
      ellipse: [
        'cx',
        'cy',
        'rx',
        'ry',
        'fill',
        'stroke',
        'strokeWidth',
        'transform',
        'className',
        'id',
        'style'
      ],
      line: [
        'x1',
        'y1',
        'x2',
        'y2',
        'stroke',
        'strokeWidth',
        'strokeDasharray',
        'strokeLinecap',
        'transform',
        'className',
        'id',
        'style',
        'markerStart',
        'markerEnd'
      ],
      polyline: [
        'points',
        'fill',
        'stroke',
        'strokeWidth',
        'strokeDasharray',
        'strokeLinecap',
        'strokeLinejoin',
        'transform',
        'className',
        'id',
        'style',
        'markerStart',
        'markerEnd'
      ],
      polygon: [
        'points',
        'fill',
        'stroke',
        'strokeWidth',
        'transform',
        'className',
        'id',
        'style'
      ],
      text: [
        'x',
        'y',
        'dx',
        'dy',
        'textAnchor',
        'dominantBaseline',
        'fill',
        'fontSize',
        'fontFamily',
        'fontWeight',
        'fontStyle',
        'transform',
        'className',
        'id',
        'style'
      ],
      tspan: [
        'x',
        'y',
        'dx',
        'dy',
        'textAnchor',
        'fill',
        'fontSize',
        'fontFamily',
        'fontWeight',
        'fontStyle',
        'className',
        'id',
        'style'
      ],
      textPath: [
        'href',
        'xlinkHref',
        'startOffset',
        'method',
        'spacing',
        'className',
        'id',
        'style'
      ],
      defs: [
        'id'
      ],
      linearGradient: [
        'id',
        'x1',
        'y1',
        'x2',
        'y2',
        'gradientUnits',
        'gradientTransform'
      ],
      radialGradient: [
        'id',
        'cx',
        'cy',
        'r',
        'fx',
        'fy',
        'gradientUnits',
        'gradientTransform'
      ],
      stop: [
        'offset',
        'stopColor',
        'stopOpacity',
        'style'
      ],
      marker: [
        'id',
        'viewBox',
        'refX',
        'refY',
        'markerWidth',
        'markerHeight',
        'orient',
        'markerUnits',
        'className',
        'style'
      ],
      clipPath: [
        'id',
        'clipPathUnits'
      ],
      mask: [
        'id',
        'maskUnits',
        'maskContentUnits',
        'x',
        'y',
        'width',
        'height'
      ],
      pattern: [
        'id',
        'x',
        'y',
        'width',
        'height',
        'patternUnits',
        'patternContentUnits',
        'patternTransform',
        'viewBox'
      ],
      image: [
        'x',
        'y',
        'width',
        'height',
        'href',
        'xlinkHref',
        'preserveAspectRatio',
        'transform',
        'className',
        'id',
        'style'
      ],
      use: [
        'href',
        'xlinkHref',
        'x',
        'y',
        'width',
        'height',
        'transform',
        'className',
        'id',
        'style'
      ],
      foreignObject: [
        'x',
        'y',
        'width',
        'height',
        'transform',
        'className',
        'id',
        'style'
      ],
      desc: [],
      title: [],
      style: [
        'type'
      ]
    }
  }), [])
  
  // Markdown components configuration
  const markdownComponents = React.useMemo(() => ({
    // Div component - intercept directive alerts
    div({ className, children, node, ...props }: any) {
      // Check if this is a directive container for alerts
      // remark-directive-rehype creates divs with data-directive attribute
      const directiveType = node?.properties?.dataDirective || props['data-directive']
      
      if (directiveType) {
        const alertType = directiveType.toLowerCase() as keyof typeof alertConfig
        const config = alertConfig[alertType]
        
        if (config) {
          const Icon = config.icon
          
          return (
            <div
              className={cn(
                'border-l-4 rounded-md flex gap-3',
                config.borderColor,
                config.bgColor,
                compact ? 'p-3 my-2' : 'p-4 my-3'
              )}
              role="note"
              aria-label={`${config.label} alert`}
            >
              <div className="flex-shrink-0 pt-0.5">
                <Icon 
                  className={cn(
                    config.iconColor,
                    compact ? 'h-4 w-4' : 'h-5 w-5'
                  )}
                  aria-hidden="true"
                />
              </div>
              <div className={cn(
                'flex-1 space-y-2',
                config.textColor
              )}>
                <div className={cn(
                  'font-semibold uppercase tracking-wide',
                  compact ? 'text-xs' : 'text-sm'
                )}>
                  {config.label}
                </div>
                <div className={cn(
                  compact ? 'text-xs' : 'text-sm'
                )}>
                  {children}
                </div>
              </div>
            </div>
          )
        }
      }
      
      // Regular div, render normally
      return <div className={className} {...props}>{children}</div>
    },
    // Pre component for code blocks - handles wrapper and copy button
    pre({ children, ...props }: any) {
      // Check if this is a code block by examining children
      const codeChild = React.Children.toArray(children).find(
        (child: any) => child?.props?.node?.tagName === 'code'
      ) as any
      
      if (!codeChild) {
        // Not a code block, render as plain pre
        return <pre className={cn(
          "bg-muted rounded-md overflow-x-auto",
          compact ? "p-2 text-xs" : "p-4 text-sm"
        )} {...props}>{children}</pre>
      }
      
      // Extract code block information from the code child
      const codeProps = codeChild.props
      const match = /language-(\w+)/.exec(codeProps.className || '')
      const language = match ? match[1] : ''
      
      // Get the actual code text content
      let codeString = ''
      const extractText = (node: any): string => {
        if (typeof node === 'string') return node
        if (typeof node === 'number') return String(node)
        if (Array.isArray(node)) return node.map(extractText).join('')
        if (node?.props?.children) return extractText(node.props.children)
        return ''
      }
      codeString = extractText(codeProps.children).replace(/\n$/, '')
      
      // Handle Mermaid diagrams
      if (language === 'mermaid') {
        return <MermaidDiagram code={codeString} compact={compact} />
      }
      
      // Render code block with wrapper and copy button
      return (
        <div className={cn(
          "relative group w-full overflow-hidden",
          compact ? "my-2" : "my-4"
        )}>
          {enableCodeCopy && (
            <div className="absolute right-2 top-2 z-10">
              <Button
                variant="ghost"
                size="sm"
                className={cn(
                  "px-2 opacity-0 group-hover:opacity-100 transition-opacity",
                  compact ? "h-6 text-xs" : "h-8"
                )}
                onClick={() => handleCopyCode(codeString)}
                aria-label={language ? `Copy ${language} code` : 'Copy code'}
              >
                {copiedCode === codeString ? (
                  <>
                    <Check className={cn(compact ? "h-2.5 w-2.5 mr-0.5" : "h-3 w-3 mr-1")} aria-hidden="true" />
                    <span className="text-xs">Copied</span>
                  </>
                ) : (
                  <>
                    <Copy className={cn(compact ? "h-2.5 w-2.5 mr-0.5" : "h-3 w-3 mr-1")} aria-hidden="true" />
                    <span className="text-xs">Copy</span>
                  </>
                )}
              </Button>
            </div>
          )}
          <pre 
            className={cn(
              "bg-muted rounded-md overflow-x-auto w-full",
              compact ? "p-2" : "p-4"
            )}
            aria-label={language ? `Code block in ${language}` : 'Code block'}
          >
            {children}
          </pre>
        </div>
      )
    },
    
    // Code component - styling for both inline and block code
    code({ inline, className: codeClassName, children, ...props }: any) {
      // For inline code
      if (inline) {
        return (
          <code 
            className={cn(
              "bg-muted rounded font-mono",
              compact ? "px-1 py-0.5 text-xs" : "px-1.5 py-0.5 text-sm"
            )}
            {...props}
          >
            {children}
          </code>
        )
      }
      
      // For block code (inside pre), just apply styling
      return (
        <code 
          className={cn(
            "font-mono block min-w-0",
            compact ? "text-xs" : "text-sm",
            codeClassName
          )}
          {...props}
        >
          {children}
        </code>
      )
    },
    
    // Custom paragraph styling
    p({ children }: any) {
      return (
        <p className={cn(
          "last:mb-0 leading-relaxed",
          compact ? "mb-2" : "mb-3"
        )}>
          {children}
        </p>
      )
    },
    
    // Lists
    ul({ children }: any) {
      return (
        <ul className={cn(
          "list-disc pl-6 space-y-1",
          compact ? "mb-2" : "mb-3"
        )}>
          {children}
        </ul>
      )
    },
    ol({ children }: any) {
      return (
        <ol className={cn(
          "list-decimal pl-6 space-y-1",
          compact ? "mb-2" : "mb-3"
        )}>
          {children}
        </ol>
      )
    },
    
    // Headings with auto-generated IDs for anchor links
    h1({ children }: any) {
      const text = extractTextFromChildren(children)
      const id = slugify(text)
      return (
        <h1 
          id={id}
          className={cn(
            "font-bold",
            compact ? "text-lg mb-2" : "text-2xl mb-3"
          )}
        >
          {children}
        </h1>
      )
    },
    h2({ children }: any) {
      const text = extractTextFromChildren(children)
      const id = slugify(text)
      return (
        <h2 
          id={id}
          className={cn(
            "font-semibold",
            compact ? "text-base mb-1.5" : "text-xl mb-2"
          )}
        >
          {children}
        </h2>
      )
    },
    h3({ children }: any) {
      const text = extractTextFromChildren(children)
      const id = slugify(text)
      return (
        <h3 
          id={id}
          className={cn(
            "font-semibold",
            compact ? "text-sm mb-1.5" : "text-lg mb-2"
          )}
        >
          {children}
        </h3>
      )
    },
    h4({ children }: any) {
      const text = extractTextFromChildren(children)
      const id = slugify(text)
      return (
        <h4 
          id={id}
          className={cn(
            "font-semibold",
            compact ? "text-sm mb-1" : "text-base mb-2"
          )}
        >
          {children}
        </h4>
      )
    },
    h5({ children }: any) {
      const text = extractTextFromChildren(children)
      const id = slugify(text)
      return (
        <h5 
          id={id}
          className={cn(
            "font-semibold",
            compact ? "text-xs mb-1" : "text-sm mb-1"
          )}
        >
          {children}
        </h5>
      )
    },
    h6({ children }: any) {
      const text = extractTextFromChildren(children)
      const id = slugify(text)
      return (
        <h6 
          id={id}
          className={cn(
            "font-medium",
            compact ? "text-xs mb-1" : "text-sm mb-1"
          )}
        >
          {children}
        </h6>
      )
    },
    
    // Links
    a({ href, children }: any) {
      // Check if this is an anchor link (starts with #)
      const isAnchor = href?.startsWith('#')
      
      if (isAnchor) {
        // Anchor link - smooth scroll within container, no new tab
        return (
          <a
            href={href}
            onClick={(e) => {
              e.preventDefault()
              // Find the target element by ID
              const targetId = href.substring(1)
              const targetElement = document.getElementById(targetId)
              
              if (targetElement) {
                // Smooth scroll to the target element
                targetElement.scrollIntoView({
                  behavior: 'smooth',
                  block: 'start'
                })
              }
            }}
            className="text-primary hover:underline cursor-pointer"
            aria-label={`Jump to ${children}`}
          >
            {children}
          </a>
        )
      }
      
      // External link - open in new tab
      return (
        <a 
          href={href}
          target="_blank"
          rel="noopener noreferrer"
          className="text-primary hover:underline"
          aria-label={`External link: ${children}`}
        >
          {children}
        </a>
      )
    },
    
    // Blockquotes
    blockquote({ children }: any) {
      return (
        <blockquote className={cn(
          "border-l-4 border-muted pl-4 py-1 text-muted-foreground",
          compact ? "my-2" : "my-3"
        )}>
          {children}
        </blockquote>
      )
    },
    
    // Horizontal rule
    hr() {
      return <hr className={cn(
        "border-border",
        compact ? "my-2" : "my-4"
      )} />
    },
    
    // Tables (with GFM)
    table({ children }: any) {
      return (
        <div className={cn(
          "overflow-x-auto",
          compact ? "my-2" : "my-3"
        )}>
          <table className="min-w-full divide-y divide-border">
            {children}
          </table>
        </div>
      )
    },
    thead({ children }: any) {
      return <thead className="bg-muted/30">{children}</thead>
    },
    tbody({ children }: any) {
      return <tbody className="divide-y divide-border">{children}</tbody>
    },
    th({ children, align }: any) {
      return (
        <th 
          className={cn(
            "text-left font-medium uppercase tracking-wider",
            compact ? "px-2 py-1 text-xs" : "px-3 py-2 text-xs"
          )}
          style={{ textAlign: align || 'left' }}
        >
          {children}
        </th>
      )
    },
    td({ children, align }: any) {
      return (
        <td 
          className={cn(
            compact ? "px-2 py-1 text-xs" : "px-3 py-2 text-sm"
          )}
          style={{ textAlign: align || 'left' }}
        >
          {children}
        </td>
      )
    },
    
    // Strong/emphasis
    strong({ children }: any) {
      return <strong className="font-semibold">{children}</strong>
    },
    em({ children }: any) {
      return <em className="italic">{children}</em>
    },
    
    // Images
    img({ src, alt }: any) {
      return (
        <img 
          src={src} 
          alt={alt || 'Image'} 
          className={cn(
            "max-w-full h-auto rounded-lg",
            compact ? "my-2" : "my-3"
          )}
        />
      )
    },
    
    // Collapsible sections (HTML details/summary)
    details({ children, open, ...props }: any) {
      return (
        <details 
          className={cn(
            "border border-border rounded-md group",
            compact ? "my-2 p-2" : "my-3 p-3"
          )}
          open={open}
          {...props}
        >
          {children}
        </details>
      )
    },
    summary({ children }: any) {
      const summaryRef = React.useRef<HTMLElement>(null)
      const [isOpen, setIsOpen] = React.useState(false)
      
      // Sync with details element state
      React.useEffect(() => {
        const summaryEl = summaryRef.current
        if (!summaryEl) return
        
        const detailsEl = summaryEl.closest('details')
        if (!detailsEl) return
        
        // Set initial state
        setIsOpen(detailsEl.open)
        
        // Listen for toggle events
        const handleToggle = () => {
          setIsOpen(detailsEl.open)
        }
        
        detailsEl.addEventListener('toggle', handleToggle)
        
        return () => {
          detailsEl.removeEventListener('toggle', handleToggle)
        }
      }, [])
      
      return (
        <summary 
          ref={summaryRef}
          className={cn(
            "cursor-pointer list-none flex items-center gap-2",
            "hover:text-primary transition-colors select-none",
            "focus:outline-none focus:ring-2 focus:ring-primary focus:ring-offset-1 rounded",
            compact ? "text-sm font-medium" : "text-base font-semibold"
          )}
          role="button"
          aria-expanded={isOpen}
          tabIndex={0}
          onKeyDown={(e) => {
            if (e.key === 'Enter' || e.key === ' ') {
              e.preventDefault()
              ;(e.target as HTMLElement).click()
            }
          }}
        >
          {isOpen ? (
            <ChevronDown className={cn(compact ? "h-3 w-3" : "h-4 w-4")} aria-hidden="true" />
          ) : (
            <ChevronRight className={cn(compact ? "h-3 w-3" : "h-4 w-4")} aria-hidden="true" />
          )}
          <span>{children}</span>
        </summary>
      )
    }
  }), [compact, enableCodeCopy, copiedCode, handleCopyCode, alertConfig])
  
  return (
    <div 
      className={cn(
        "prose max-w-none overflow-hidden",
        compact ? "prose-sm" : "prose-sm",
        "prose-headings:text-foreground",
        "prose-p:text-foreground",
        "prose-strong:text-foreground",
        "prose-ul:text-foreground prose-ol:text-foreground",
        "prose-blockquote:text-muted-foreground",
        "prose-code:text-foreground prose-code:bg-muted",
        "prose-pre:bg-muted prose-pre:overflow-x-auto",
        "prose-th:text-muted-foreground",
        "prose-td:text-foreground",
        "prose-a:text-primary",
        className
      )}
      role="article"
      aria-label={ariaLabel}
    >
      <ReactMarkdown
        remarkPlugins={[
          remarkGfm,
          remarkDirective,
          remarkDirectiveToHtml,
          remarkMath,
          [remarkToc, {
            heading: '(table[ -]of[ -])?contents?|toc',  // Matches "Table of Contents", "TOC", "Contents"
            tight: true,         // Compact list style
            ordered: false,      // Use bullet list (not numbered)
            maxDepth: 4,         // Include headings up to h4
            skip: undefined      // Don't skip any headings
          }]
        ]}
        rehypePlugins={[
          rehypeRaw,
          rehypeHighlight,
          [rehypeKatex, {
            strict: false,
            throwOnError: false,
            errorColor: '#cc0000',
            output: 'html'
          }],
          [rehypeSanitize, sanitizeSchema]
        ]}
        components={markdownComponents}
      >
        {content}
      </ReactMarkdown>
    </div>
  )
}

export default MarkdownRenderer
