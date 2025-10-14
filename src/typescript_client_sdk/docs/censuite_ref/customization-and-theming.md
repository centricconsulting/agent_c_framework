This approach leverages HSL (Hue, Saturation, Lightness) for color definitions, rather than RGB (Red, Green, Blue). HSL offers several advantages, including more intuitive color adjustments and simplified management of color variations. By using this method, the system ensures visual consistency and appeal across all components.


**Note:** Design tokens must be defined without color space function. See the [Tailwind CSS documentation](https://tailwindcss.com/docs/customizing-colors#using-css-variables) for more information.


#### Built\-in Design Tokens

##### Background and Foreground


```
--background: 255 100% 100%; // Body background color, used for <body> and full-page containers
--foreground: 255 5% 10%; // Default text color for the entire application
```
Copy##### Primary Colors


```
--primary: 255 55% 23.5%; // Background color for primary buttons and prominent UI elements
--primary-foreground: 0 0% 100%; // Text color on primary-colored elements
```
Copy##### Secondary Colors


```
--secondary: 255 30% 90%; // Background color for secondary buttons and less prominent UI elements
--secondary-foreground: 0 0% 0%; // Text color on secondary-colored elements
```
Copy##### Muted/Neutral


```
--muted: 217 30% 95%; // Background color for subtle UI elements like unselected tabs, skeletons, and switch tracks
--muted-foreground: 255 5% 40%; // Text color for content within muted components
```
Copy##### Border Color


```
--border: 255 30% 82%; // Default border color used throughout the application for various components
```
Copy##### Focus Ring


```
--ring: 255 55% 23.5%; // Color of the focus ring around interactive elements for accessibility
```
Copy##### Border Radius


```
--radius: 0rem; // Default border radius applied to various components for rounded corners
```
Copy##### Input Color

Border color for inputs such as \<Input /\>, \<Select /\>, \<Textarea /\>


```
--input: 255 30% 50%; // Border color specifically for input-type components like text inputs, selects, and textareas
```
Copy##### Popover Colors


```
--popover: 255 100% 100%; // Background color for floating UI elements like dropdowns and popovers
--popover-foreground: 255 100% 10%; // Text color within popover components
```
Copy##### Accent Colors


```
--accent: 217 30% 90%; // Background color for hover states or to highlight active items
--accent-foreground: 255 5% 15%; // Text color on accented elements
```
Copy##### Destructive Colors


```
--destructive: 0 100% 50%; // Background color for destructive actions, typically a shade of red
--destructive-foreground: 255 5% 100%; // Text color on destructive elements, usually white or light
```
Copy##### Card


```
--card: 255 50% 100%; // Background color for card components
--card-foreground: 255 5% 15%; // Text color within card components
```
Copy##### Charts


```
@layer base {
  :root {
    --chart-1: 255 55% 23%; // First color in the chart palette for light mode
    --chart-2: 217 30% 90%; // Second color in the chart palette for light mode
    --chart-3: 30 80% 60%; // Third color in the chart palette for light mode
    --chart-4: 300 60% 70%; // Fourth color in the chart palette for light mode
    --chart-5: 180 50% 60%; // Fifth color in the chart palette for light mode
  }
  .dark {
    --chart-1: 220 70% 50%; // First color in the chart palette for dark mode
    --chart-2: 160 60% 45%; // Second color in the chart palette for dark mode
    --chart-3: 30 80% 55%; // Third color in the chart palette for dark mode
    --chart-4: 280 65% 60%; // Fourth color in the chart palette for dark mode
    --chart-5: 340 75% 55%; // Fifth color in the chart palette for dark mode
  }
}
```
Copy#### Light/Dark Mode

By utilizing a combination of Design Tokens, Tailwind CSS, and Next.js's `<ThemeProvider />` component, we can seamlessly switch between light and dark modes or even entirely different themes. This ensures a consistent and visually appealing experience for all users. Below is an example of how this can be implemented using CSS Variables.


```
@layer base { // Hooks into the base layer of Tailwind CSS
  :root { // Defines the Light Mode/Default Color Palette
    --background: 255 100% 100%;
    --foreground: 255 5% 10%;
    /* Add more tokens here */
  }
  .dark { // Defines the Dark Mode Color Palette
    --background: 0 0 5%;
    --foreground: 255 0% 90%;
    /* Add more tokens here */
  }
}
```
Copy#### Adding More Tokens

Add custom tokens in `app/globals.css`:


```
:root {
  --warning: 38 92% 50%;
  --warning-foreground: 48 96% 89%;
}
 
.dark {
  --warning: 48 96% 89%;
  --warning-foreground: 38 92% 50%;
}
```
CopyUpdate `tailwind.config.cjs` to extend the theme:


```
module.exports = {
  theme: {
    extend: {
      colors: {
        warning: "hsl(var(--warning))",
        "warning-foreground": "hsl(var(--warning-foreground))",
      },
    },
  },
}
```
CopyNow, use the `warning` utility class in your components:


```
<div className="bg-warning text-warning-foreground">
  Warning content here
</div>
```
Copy#### Bringing It All Together


```
@layer base {
  :root {
    --background: 255 100% 100%;
    --foreground: 255 5% 10%;
    --card: 255 50% 100%;
    --card-foreground: 255 5% 15%;
    --popover: 255 100% 100%;
    --popover-foreground: 255 100% 10%;
    --primary: 255 55% 23%;
    --primary-foreground: 0 0% 100%;
    --secondary: 255 30% 90%;
    --secondary-foreground: 0 0% 0%;
    --muted: 217 30% 95%;
    --muted-foreground: 255 5% 40%;
    --accent: 217 30% 90%;
    --accent-foreground: 255 5% 15%;
    --destructive: 0 100% 50%;
    --destructive-foreground: 255 5% 100%;
    --border: 255 30% 82%;
    --input: 255 30% 50%;
    --ring: 255 55% 23%;
    --radius: 0rem;
    --chart-1: 255 55% 23%;
    --chart-2: 217 30% 90%;
    --chart-3: 30 80% 60%;
    --chart-4: 300 60% 70%;
    --chart-5: 180 50% 60%;
  }
  .dark {
    --background: 0 0 5%;
    --foreground: 255 0% 90%;
    --card: 255 0% 10%;
    --card-foreground: 255 0% 100%;
    --popover: 255 10% 5%;
    --popover-foreground: 255 0% 100%;
    --primary: 255 55% 23%;
    --primary-foreground: 0 0% 100%;
    --secondary: 255 10% 20%;
    --secondary-foreground: 0 0% 100%;
    --muted: 217 10% 25%;
    --muted-foreground: 255 0% 65%;
    --accent: 217 10% 25%;
    --accent-foreground
 
: 255 0% 95%;
    --destructive: 0 50% 50%;
    --destructive-foreground: 255 0% 100%;
    --border: 255 20% 50%;
    --input: 255 20% 50%;
    --ring: 255 55% 23%;
    --radius: 0rem;
    --chart-1: 220 70% 50%;
    --chart-2: 160 60% 45%;
    --chart-3: 30 80% 55%;
    --chart-4: 280 65% 60%;
    --chart-5: 340 75% 55%;
  }
}
```
Copy
**Note:** The `background` suffix is omitted when the variable is used for the background color of the component. Updating these variables will automatically update the corresponding styles throughout your application and across components.


Tailwind CSS is used throughout the system, making it easy to quickly modify components and create layouts.


