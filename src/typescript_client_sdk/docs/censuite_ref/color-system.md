## Introduction


CenSuite offers a comprehensive color palette that includes Primary, Secondary, and Neutral colors. These colors form the foundation of your visual hierarchy, ensuring contrast, readability, and accessibility across all designs. The palette can be easily modified using design tokens and managed through the theming engine, which supports both Light and Dark modes.


### Primary Colors


Primary colors are used for branding, emphasis, and calls to action. These colors should stand out, drawing the user’s attention to key interactions such as buttons and links.


### Secondary Colors


Secondary colors complement the primary palette and provide additional contrast without overwhelming the interface. They’re typically used in backgrounds, borders, or secondary actions.


Secondary (.bg\-secondary)


### Neutral Colors


Neutral colors form the backbone of your interface. They’re typically used for backgrounds, borders, and shadows. Neutral colors allow primary and secondary colors to stand out while maintaining a clean and minimal aesthetic.


These colors are maintained via the Theming Engine and can be used throughout the codebase. Because these colors are rendered as variables, they can be used with tools like Tailwind CSS to ensure consistency across the codebase.


## Tailwind CSS Color Palette


To view the full list of colors and see how to customize them using design tokens and the theming engine, refer to the [CenSuite Theming](#) page for more information.


## Using HSL for Easier Color Manipulation


CenSuite uses HSL (Hue, Saturation, Lightness) values to define and manipulate colors, making it easier for designers and developers to adjust hues, tweak saturation levels, and control brightness. HSL provides a more intuitive approach to color customization than hex codes or RGB because it mirrors how humans perceive color.


### 1\. Hue


The hue represents the actual color—such as red, blue, or green—measured in degrees around the color wheel (0 to 360\). In CenSuite, you can adjust the hue to create different variations of primary or secondary colors without losing consistency. For instance, adjusting the hue slightly can help create theme variants or emphasize a specific part of your UI.


* Example: Primary Blue (`#1E3A8A`) has a hue of 225°, representing blue.


### 2\. Saturation


Saturation controls the intensity or purity of a color. A higher saturation value makes the color more vivid, while a lower saturation results in more muted tones. In CenSuite, you can use design tokens to tweak saturation for specific components, ensuring a balance between bold actions and subtler interface elements.


* Example: Primary Green (`#059669`) has a saturation of 92%, making it vibrant and eye\-catching, suitable for success states or call\-to\-action buttons.


### 3\. Lightness


Lightness refers to how light or dark a color is. Higher lightness values move a color toward white, while lower values make it darker. Lightness adjustments are particularly useful for creating depth and distinguishing between different UI layers.


* Example: Neutral Dark (`#111827`) has a lightness of 15%, providing contrast for text and key interface elements, especially in dark mode.


When working with HSL, experiment with minor tweaks to saturation and lightness to create subtle variations that enhance readability and usability without changing the core brand color.


## Design Tokens for Consistency


CenSuite uses design tokens to maintain consistency across all colors, ensuring that UI elements are styled uniformly across projects. These tokens are CSS variables that represent colors, spacing, typography, and more, making it easy to apply and modify colors throughout your product.


### Why Design Tokens Matter:


* Scalability Design tokens allow you to make global changes across your product. By updating a token, you can change the appearance of components throughout the interface without needing to manually adjust individual styles.
* Consistency Tokens ensure that colors and other design elements are applied consistently, regardless of where or how they’re used.
* Customization Design tokens work seamlessly with CenSuite’s theming engine, making it simple to switch between Light and Dark modes or introduce new themes by adjusting a few variables.


To learn more about how to leverage design tokens and customize your theme, check out the [CenSuite Theming](#) page.


## Best Practices for Using Color in UI Design


### 1\. Conveying Status and Importance


Color is often used to convey status or importance. It’s important to use consistent colors for states like success, warnings, or errors so that users can quickly interpret the meaning:


* Green: Success or positive actions (e.g., “Success”, “Submit”).
* Yellow: Warning or caution (e.g., “Warning”, “Pending”).
* Red: Error or negative actions (e.g., “Error”, “Delete”).


Ensure these colors are used consistently across all components, allowing users to quickly recognize patterns and meanings.


### 2\. Creating Visual Hierarchy


Use color to establish a clear visual hierarchy by applying different levels of emphasis:


* Primary colors are typically used for key actions or elements you want users to focus on.
* Secondary or neutral colors can be used for supporting content or backgrounds, allowing primary elements to stand out more.


### 3\. Contrast and Accessibility


High contrast is essential for ensuring readability and accessibility. Aim for a contrast ratio of at least 4\.5:1 for text against its background, as recommended by the [WCAG guidelines](https://www.w3.org/TR/WCAG21/). This is especially important in dark mode, where light text on dark backgrounds can be harder to read.


Use tools like [Contrast Checker](https://contrastchecker.com) to verify that your color combinations meet accessibility standards.


### 4\. Light and Dark Mode Support


CenSuite’s theming engine makes it easy to implement both Light and Dark modes, allowing you to create seamless transitions between color schemes. Each color in the palette has dark mode variants managed through design tokens, ensuring readability and contrast are maintained in both environments.


* Light Mode: Typically uses lighter backgrounds with dark text to ensure high contrast.
* Dark Mode: Uses darker backgrounds with light text, making sure that key interactive elements still stand out.


For more details on how to customize Light and Dark modes, refer to the [CenSuite Theming](#) page.




---


To dive deeper into theming options and customizing the color palette, check out the [CenSuite Theming](/docs/design/foundation/customization-and-theming) page, where you can learn how to extend the color system while maintaining alignment with CenSuite’s design principles.

