
When generating web pages, you MUST adhere to the following modern design system. The goal is to create clean, visually appealing, and consistent user interfaces that are fully functional in both light and dark modes. Every generated page must be a **single, self-contained HTML file** using CDNs for all external resources. **This instruction contains detailed documentation for all required libraries; you must follow these examples and patterns precisely to avoid errors.**

---

### **1. Core Principles (Unchanged)**
*   **Clean & Minimal:** Prioritize content and clarity.
*   **Whitespace is Key:** Use generous padding and margins.
*   **Dark Mode Native:** Every component must support dark mode.
*   **Consistency:** Use the defined palettes, typography, and components.
*   **Responsive First:** Use Tailwind's responsive prefixes (`sm:`, `md:`, `lg:`).

---

### **2. Layout & Spacing (Unchanged)**
*   **Container:** `max-w-7xl mx-auto px-4 sm:px-6 lg:px-8`
*   **Section Padding:** `py-16` or `py-20`
*   **Gaps:** `gap-8` for grids, `space-y-4` for stacks.

---

### **3. Color Palette (Unchanged)**
*   **Strictly adhere to this table.** Use the `dark:` prefix for all dark mode styles.

| Element                | Light Mode (`class`)          | Dark Mode (`dark:class`)              |
| ---------------------- | ----------------------------- | ------------------------------------- |
| **Main Background**    | `bg-slate-50`                 | `dark:bg-slate-900`                   |
| **Card/Surface Bg**    | `bg-white`                    | `dark:bg-slate-800`                   |
| **Primary Text**       | `text-slate-900`              | `dark:text-slate-100`                 |
| **Secondary Text**     | `text-slate-600`              | `dark:text-slate-400`                 |
| **Borders / Dividers** | `border-slate-200`            | `dark:border-slate-700`               |
| **Accent / Action**    | `bg-sky-500` `text-sky-500`   | (Stays the same for high contrast)    |

---

### **4. Typography (Unchanged)**
*   **Headings:** `font-bold text-slate-900 dark:text-slate-100`
*   **Body Text:** `text-lg text-slate-600 dark:text-slate-400 leading-relaxed`

---

### **5. Component Styles (Unchanged)**
*   **Primary Button:** `bg-sky-500 text-white font-semibold py-3 px-6 rounded-lg ...`
*   **Secondary Button:** `bg-transparent border-2 border-slate-300 text-slate-700 ... dark:border-slate-600 dark:text-slate-300 ...`
*   **Card:** `bg-white dark:bg-slate-800 rounded-xl shadow-lg dark:border dark:border-slate-700 ...`
*   **Input:** `block w-full ... bg-white dark:bg-slate-800 border border-slate-300 dark:border-slate-600 ...`

---

### **6. In-Depth Library Guide (For AI Implementation)**

This section provides the necessary details to use each library correctly. **Refer to these blueprints to avoid hallucinating attributes or class names.**

#### **A. Animate On Scroll (AOS)**
*   **Purpose:** To animate elements into view as the user scrolls down.
*   **Usage:** Add `data-aos="animation-name"` to any HTML element.
*   **Key Animations to Use:**
    *   `fade-up`: The default choice. Element fades and slides up.
    *   `fade-in`: Simple fade without movement.
    *   `fade-left`, `fade-right`: Element fades and slides in from the side.
    *   `zoom-in`: Element scales up from the center.
*   **Modifiers (add as separate attributes):**
    *   `data-aos-delay="100"`: Waits 100ms before animating. Use this to stagger animations in a list or grid.
    *   `data-aos-duration="1000"`: Makes the animation last 1000ms (1 second).
    *   `data-aos-offset="200"`: Starts the animation when the element is 200px into the viewport.

#### **B. Alpine.js (for all interactive UI)**
*   **Purpose:** To add interactivity (like dropdowns, modals, tabs) directly in your HTML.
*   **Core Directives (These are attributes you add to HTML tags):**
    *   `x-data="{...}"`: **Declares a new component.** Initializes its data. E.g., `x-data="{ open: false, name: 'Guest' }"`.
    *   `x-show="condition"`: **Toggles visibility.** Shows the element if the condition is `true`. E.g., `x-show="open"`.
    *   `@click="expression"`: **Runs code on click.** E.g., `@click="open = !open"`.
    *   `@click.away="expression"`: **Runs code when clicking outside the element.** Used for closing dropdowns/modals. E.g., `@click.away="open = false"`.
    *   `x-bind:attribute="expression"` or `:attribute="expression"`: **Binds an attribute to data.** E.g., `:class="{ 'bg-sky-500': open }"`.
    *   `x-transition`: **Adds smooth transitions** to elements with `x-show`. You MUST add modifier classes for it to work.
        *   **Standard Transition Blueprint:**
            ```html
            <div x-show="open"
                 x-transition:enter="transition ease-out duration-200"
                 x-transition:enter-start="opacity-0 transform -translate-y-2"
                 x-transition:enter-end="opacity-100 transform translate-y-0"
                 x-transition:leave="transition ease-in duration-150"
                 x-transition:leave-start="opacity-100 transform translate-y-0"
                 x-transition:leave-end="opacity-0 transform -translate-y-2">
                 ...
            </div>
            ```

*   **Blueprint 1: Dropdown Menu**
    ```html
    <div x-data="{ open: false }" class="relative">
        <button @click="open = !open" class="inline-flex items-center gap-2 justify-center rounded-md border border-slate-300 dark:border-slate-600 px-4 py-2 bg-white dark:bg-slate-800 text-sm font-medium text-slate-700 dark:text-slate-300 hover:bg-slate-50 dark:hover:bg-slate-700">
            <span>Options</span>
            <i data-lucide="chevron-down" class="w-4 h-4"></i>
        </button>
        <div x-show="open" @click.away="open = false" x-transition class="origin-top-right absolute right-0 mt-2 w-56 rounded-md shadow-lg bg-white dark:bg-slate-800 ring-1 ring-black ring-opacity-5 py-1 z-10">
            <a href="#" class="block px-4 py-2 text-sm text-slate-700 dark:text-slate-300 hover:bg-slate-100 dark:hover:bg-slate-700">Account</a>
            <a href="#" class="block px-4 py-2 text-sm text-slate-700 dark:text-slate-300 hover:bg-slate-100 dark:hover:bg-slate-700">Settings</a>
        </div>
    </div>
    ```

*   **Blueprint 2: Modal Dialog**
    ```html
    <div x-data="{ open: false }">
        <!-- Trigger Button -->
        <button @click="open = true" class="bg-sky-500 text-white font-semibold py-2 px-4 rounded-lg">Open Modal</button>

        <!-- Modal -->
        <div x-show="open" style="display: none;" x-transition:enter="ease-out duration-300" x-transition:enter-start="opacity-0" x-transition:enter-end="opacity-100" x-transition:leave="ease-in duration-200" x-transition:leave-start="opacity-100" x-transition:leave-end="opacity-0" class="fixed inset-0 z-50 overflow-y-auto bg-black/50 backdrop-blur-sm">
            <div @click.away="open = false" x-show="open" x-transition:enter="ease-out duration-300" x-transition:enter-start="opacity-0 scale-95" x-transition:enter-end="opacity-100 scale-100" x-transition:leave="ease-in duration-200" x-transition:leave-start="opacity-100 scale-100" x-transition:leave-end="opacity-0 scale-95" class="relative mx-auto mt-20 w-full max-w-lg rounded-xl bg-white dark:bg-slate-800 p-8 shadow-2xl">
                <h3 class="text-xl font-bold text-slate-900 dark:text-slate-100">Modal Title</h3>
                <p class="mt-2 text-slate-600 dark:text-slate-400">This is the modal content. You can put anything here.</p>
                <div class="mt-6 flex justify-end gap-4">
                    <button @click="open = false" class="bg-transparent border-2 border-slate-300 text-slate-700 ...">Cancel</button>
                    <button @click="open = false; notyf.success('Action Confirmed!')" class="bg-sky-500 text-white ...">Confirm</button>
                </div>
            </div>
        </div>
    </div>
    ```

#### **C. Notyf (for Toast Notifications)**
*   **Purpose:** To show simple, non-blocking alerts like "Success!" or "Error!".
*   **Initialization (already in boilerplate):** The `new Notyf({...})` configuration sets the colors and position. **Do not change this configuration.**
*   **Usage (call these from `onclick` attributes):**
    *   `notyf.success('Your message here')`: For successful operations. (Green)
    *   `notyf.error('Your message here')`: For failed operations. (Red)

#### **D. Lucide (for Icons)**
*   **Purpose:** To display clean, modern SVG icons.
*   **Initialization (already in boilerplate):** `lucide.createIcons()` is called to render the icons.
*   **Usage:**
    1.  Find an icon name on the [lucide.dev](https://lucide.dev/) website (e.g., "home", "check-circle").
    2.  Add an `<i>` tag with the `data-lucide` attribute: `<i data-lucide="home"></i>`.
    3.  **Style the icon using Tailwind classes for size and color.** This is critical.
        *   `class="w-5 h-5"` (for size)
        *   `class="text-sky-500"` (for color)
        *   Example: `<i data-lucide="check-circle" class="w-6 h-6 text-green-500"></i>`

---

### **8. Master Boilerplate (Mandatory Starting Point)**

ALL generated web pages MUST start with this exact HTML structure. It includes all libraries and initializations.

```html
<!DOCTYPE html>
<html lang="en" class=""> <!-- The 'dark' class is added here dynamically -->
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Modern Web Page</title>

    <!-- 1. Tailwind CSS -->
    <script src="https://cdn.tailwindcss.com"></script>
    <script>
        tailwind.config = { darkMode: 'class' }
    </script>

    <!-- 2. Animate On Scroll (AOS) CSS -->
    <link href="https://unpkg.com/aos@2.3.1/dist/aos.css" rel="stylesheet">

    <!-- 3. Notyf Toast Notification CSS -->
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/notyf@3/notyf.min.css">

    <!-- 4. Alpine.js (deferred) -->
    <script defer src="https://cdn.jsdelivr.net/npm/alpinejs@3.x.x/dist/cdn.min.js"></script>

</head>
<body class="bg-slate-50 dark:bg-slate-900 font-sans text-slate-800 dark:text-slate-200 transition-colors duration-300">

    <header class="py-4 shadow-sm bg-white/50 dark:bg-slate-800/50 backdrop-blur-lg sticky top-0 z-20">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 flex justify-between items-center">
            <h2 class="font-bold text-xl text-slate-900 dark:text-white flex items-center gap-2">
                <i data-lucide="layers"></i>
                <span>MySite</span>
            </h2>
            <button id="theme-toggle" class="p-2 rounded-full hover:bg-slate-200 dark:hover:bg-slate-700">
                <i data-lucide="sun" class="block dark:hidden"></i>
                <i data-lucide="moon" class="hidden dark:block"></i>
            </button>
        </div>
    </header>

    <!-- START: YOUR PAGE CONTENT HERE -->
    <main class="py-20">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 space-y-12">
            <!-- Content goes here -->
        </div>
    </main>
    <!-- END: YOUR PAGE CONTENT HERE -->

    <!-- SCRIPTS (Order is important) -->
    <!-- Animate On Scroll (AOS) JS -->
    <script src="https://unpkg.com/aos@2.3.1/dist/aos.js"></script>
    
    <!-- Notyf JS -->
    <script src="https://cdn.jsdelivr.net/npm/notyf@3/notyf.min.js"></script>

    <!-- Lucide Icons JS -->
    <script src="https://unpkg.com/lucide@latest"></script>

    <!-- MAIN INITIALIZATION SCRIPT -->
    <script>
        // --- 1. INITIALIZE NOTYF ---
        // This configuration sets up the toast notifications with our design system's colors.
        const notyf = new Notyf({
            duration: 4000,
            position: { x: 'right', y: 'top' },
            types: [
                { type: 'success', background: '#10B981', icon: false }, // Tailwind green-500
                { type: 'error', background: '#EF4444', icon: false },   // Tailwind red-500
            ]
        });

        // --- 2. INITIALIZE DARK MODE ---
        // This logic checks user preference and applies the 'dark' class.
        const themeToggle = document.getElementById('theme-toggle');
        const docElement = document.documentElement;
        const isDarkMode = localStorage.getItem('theme') === 'dark' || (!('theme' in localStorage) && window.matchMedia('(prefers-color-scheme: dark)').matches);
        if (isDarkMode) { docElement.classList.add('dark'); }

        themeToggle.addEventListener('click', () => {
            docElement.classList.toggle('dark');
            const theme = docElement.classList.contains('dark') ? 'dark' : 'light';
            localStorage.setItem('theme', theme);
            lucide.createIcons(); // Re-render icons after theme change to fix display issues
        });
        
        // --- 3. INITIALIZE LIBRARIES ON PAGE LOAD ---
        // This ensures libraries that scan the DOM run after it's ready.
        document.addEventListener('DOMContentLoaded', (event) => {
            // Initialize AOS animations
            AOS.init({ duration: 800, once: false });
            // Initialize Lucide icons
            lucide.createIcons();
        });
    </script>
</body>
</html>
```