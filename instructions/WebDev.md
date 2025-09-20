
When generating web pages, you should build upon the following modern design system. The goal is to create fluid, interactive, and visually appealing landing pages. Every generated page should be a **single, self-contained HTML file**, with all CSS and JavaScript included directly.

This document contains the complete specification for this template. You should follow these guidelines and use the provided boilerplate as a starting point to ensure full functionality and design consistency. All interactivity is handled by the built-in vanilla JavaScript; do not add external libraries like Alpine.js or AOS.

---

### **1. Core Principles**

*   **Motion & Fluidity:** The design is built around smooth, non-intrusive animations. Sections reveal on scroll, a progress bar tracks reading, and a particle background adds subtle life.
*   **Glassmorphism:** Key UI elements like the navigation bar use a "frosted glass" effect that adapts for both light and dark modes.
*   **Performance First:** Animations are built with performance in mind, using CSS transitions and IntersectionObserver. Reduced motion preferences are respected.
*   **Dark Mode Native:** Every element is designed to work flawlessly in both light and dark themes, controlled by a toggle that saves the user's preference.
*   **Self-Contained:** The final output is always a single HTML file using the Tailwind CSS CDN.

---

### **2. Layout & Structure**

*   **Container:** Content is generally centered within a `max-w-6xl mx-auto px-6` container.
*   **Full-Height Sections:** Use the `vh-screen-min` utility class on `<section>` elements to create a deliberate "scrolling presentation" feel where each section takes up at least the full viewport height.
*   **Header:** A single, fixed header (`<header class="fixed top-4 ...">`) with the `.glass` style is centered horizontally. It contains the site title, scroll percentage, and theme/action buttons.
*   **Fixed Elements:** The header, progress bar, and toast container are all `position: fixed`. Their `z-index` values are pre-configured in the boilerplate for correct layering.

---

### **3. Color & Typography**

*   **Palette:** Adhere strictly to this `slate` and `indigo` based palette.

| Element                | Light Mode (`class`)          | Dark Mode (`dark:class`)              |
| ---------------------- | ----------------------------- | ------------------------------------- |
| **Main Background**    | `bg-slate-50`                 | `dark:bg-slate-900`                   |
| **Main Text**          | `text-slate-900`              | `dark:text-slate-100`                 |
| **Secondary Text**     | `opacity-80` or `opacity-70`  | (Same opacity classes apply)          |
| **Glass Background**   | Custom `.glass` style         | Custom `.dark .glass` style           |
| **Primary Accent/CTA** | `bg-indigo-600`               | (Stays the same for high contrast)    |

*   **Typography:**
    *   **Hero Heading:** `text-4xl sm:text-6xl font-extrabold`
    *   **Section Headings:** `text-2xl` or `text-3xl font-bold`
    *   **Body Text:** `text-lg opacity-80` for the hero, standard size with `opacity-80` for other sections.

---

### **4. Key Visual Effects & Components (Built-in)**

These are custom CSS classes defined in the boilerplate's `<style>` tag.

*   **Glassmorphism (`.glass`)**:
    *   **Purpose:** Creates a blurred, semi-transparent background.
    *   **Usage:** Apply this class to elements like the header or cards. It automatically adapts to dark mode. Example: `<header class="... glass ...">`

*   **Scroll-Reveal Animation (`.reveal`)**:
    *   **Purpose:** Animates elements into view as the user scrolls.
    *   **Mechanism:** The element starts hidden (`opacity: 0`). A JavaScript IntersectionObserver adds the `.in-view` class when the element becomes visible, triggering a smooth CSS transition.
    *   **Usage:** Add the `.reveal` class to any section or element you want to animate on scroll. The script handles the rest automatically. Example: `<div class="reveal">...</div>`

---

### **5. Interactive Feature Blueprints (Vanilla JS)**

All interactivity is powered by the script block in the boilerplate. You do not need to write new JavaScript, but you **must know how to trigger its features from your HTML.**

*   **A. Dark/Light Mode Toggle**
    *   **Functionality:** The `<button id="toggleTheme">` in the header is pre-configured. It automatically handles toggling the theme, saving the preference, and updating its own icon. No action is needed.

*   **B. Scroll Progress Bar & Percentage**
    *   **Functionality:** The `<div id="progress">` and `<span id="pctLabel">` are automatically updated on scroll. This is fully automatic.

*   **C. Toast Notification System**
    *   **HTML Container:** A `<div id="toasts">` exists to hold the notifications.
    *   **How to Trigger a Toast:** A global function `pushToast(message)` is available. Call it from an `onclick` attribute on any button or interactive element.
    *   **Blueprint:**
        ```html
        <!-- Trigger a toast with a simple message -->
        <button onclick="pushToast('Your settings have been saved.')">Save Settings</button>

        <!-- Use on a primary call-to-action button -->
        <button class="... bg-indigo-600 ..." onclick="pushToast('Welcome! Your download will begin shortly.')">Download</button>
        ```

*   **D. Animated Particle Background**
    *   **Functionality:** The `<canvas id="bgCanvas">` is animated automatically by the script. It is theme-aware and respects `prefers-reduced-motion`. No action is needed.

---

### **6. Master Boilerplate (Default Starting Point)**

All generated pages should start with this HTML structure. It includes Tailwind CSS from the CDN, the complete custom stylesheet, and the all-in-one JavaScript for all interactive features. You will add your content primarily inside the `<main>` tag.

```html
<!doctype html>
<html lang="en" class="scroll-smooth">
<head>
  <meta charset="utf-8" />
  <meta name="viewport" content="width=device-width,initial-scale=1" />
  <title>StellarSync - Unify Your Workflow</title>
  <meta name="description" content="A modern landing page featuring smooth scrolling animations, dark mode, and interactive elements.">

  <!-- Google Fonts: Inter -->
  <link rel="preconnect" href="https://fonts.googleapis.com">
  <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
  <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700;800;900&display=swap" rel="stylesheet">

  <!-- Tailwind Play CDN -->
  <script src="https://cdn.tailwindcss.com"></script>

  <!-- Tailwind Config -->
  <script>
    tailwind.config = {
      darkMode: 'class',
      theme: {
        extend: {
          fontFamily: {
            sans: ['Inter', 'sans-serif'],
          },
          transitionTimingFunction: {
            'snappy': 'cubic-bezier(.2,.9,.2,1)'
          }
        }
      }
    }
  </script>

  <!-- Custom Styles for the Design System -->
  <style>
    :root { --accent: 200 100% 60%; }
    #progressWrap { position: fixed; inset: 0 0 auto 0; pointer-events: none; z-index: 60; height: 4px; mix-blend-mode: normal; }
    #progress { height: 100%; width: 0%; transition: width 200ms cubic-bezier(.2,.9,.2,1); background: linear-gradient(90deg, rgba(99,102,241,0.95), rgba(129,140,248,0.95)); backdrop-filter: blur(6px); }
    @keyframes toastIn { from { transform: translateY(10px) scale(.98); opacity: 0; } to { transform: translateY(0) scale(1); opacity: 1; } }
    @keyframes toastOut { from { transform: translateY(0) scale(1); opacity: 1; } to { transform: translateY(8px) scale(.98); opacity: 0; } }
    .reveal { opacity: 0; transform: translateY(32px) scale(.995); transition: opacity 800ms cubic-bezier(.2,.9,.2,1), transform 800ms cubic-bezier(.2,.9,.2,1); will-change: opacity, transform; }
    .reveal.in-view { opacity: 1; transform: translateY(0) scale(1); }
    .hero-media { transform: translateZ(0); will-change: transform, filter; transition: transform 1000ms cubic-bezier(.2,.9,.2,1), filter 1000ms; filter: drop-shadow(0 20px 40px rgba(2,6,23,0.35)); }
    @media (prefers-reduced-motion: reduce) { #progress { transition: none; } .reveal { transition: none; transform: none !important; opacity: 1 !important; } .hero-media { transition: none; transform: none !important; } }
    .glass { background: linear-gradient(180deg, rgba(255,255,255,0.03), rgba(255,255,255,0.02)); backdrop-filter: blur(8px); }
    .dark .glass { background: linear-gradient(180deg, rgba(6,6,23,0.45), rgba(17,24,39,0.35)); }
    #bgCanvas { position: fixed; inset: 0; z-index: 0; pointer-events: none; }
    #toasts { position: fixed; right: 1rem; bottom: 1.25rem; z-index: 70; display:flex; flex-direction:column; gap:.5rem; align-items:flex-end; }
    .vh-screen-min { min-height: 100vh; }
    .meta-pill { font-size: .72rem; padding: .18rem .5rem; border-radius: 999px; backdrop-filter: blur(6px); }
  </style>
</head>
<body class="bg-slate-50 text-slate-900 dark:bg-slate-900 dark:text-slate-100 antialiased transition-colors duration-500 font-sans">

  <!-- BACKGROUND: Animated particles -->
  <canvas id="bgCanvas" width="800" height="600"></canvas>

  <!-- UI: Top progress bar -->
  <div id="progressWrap" aria-hidden="true"><div id="progress" role="progressbar" aria-valuemin="0" aria-valuemax="100"></div></div>

  <!-- UI: Top navigation / controls -->
  <header class="fixed top-4 left-1/2 -translate-x-1/2 z-50 w-[96%] max-w-5xl glass rounded-2xl p-2 px-4 flex items-center justify-between gap-3 shadow-lg border border-white/10 dark:border-white/5">
    <div class="flex items-center gap-3">
      <div class="flex items-center gap-2 text-sm font-semibold tracking-tight">
        <svg class="w-6 h-6 text-indigo-400" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="m12 14 4-4"></path><path d="M3.34 19a10 10 0 1 1 17.32 0"></path></svg>
        StellarSync
      </div>
    </div>
    <nav class="hidden md:flex items-center gap-1">
        <a href="#features" class="px-3 py-1.5 text-sm rounded-lg hover:bg-black/5 dark:hover:bg-white/5 transition-colors">Features</a>
        <a href="#pricing" class="px-3 py-1.5 text-sm rounded-lg hover:bg-black/5 dark:hover:bg-white/5 transition-colors">Pricing</a>
        <a href="#testimonials" class="px-3 py-1.5 text-sm rounded-lg hover:bg-black/5 dark:hover:bg-white/5 transition-colors">Reviews</a>
    </nav>
    <div class="flex items-center gap-2">
      <div class="text-xs opacity-70 mr-2 hidden sm:inline">Scroll: <span id="pctLabel">0%</span></div>
      <button id="toggleTheme" class="rounded-full p-2 glass hover:scale-105 transition-transform" title="Toggle dark / light">
        <svg id="iconTheme" class="w-5 h-5" viewBox="0 0 24 24" fill="none" stroke="currentColor"><path d="M12 3v1M12 20v1M4.2 4.2l.7.7M19.1 19.1l.7.7M1 12h1M22 12h1M4.2 19.8l.7-.7M19.1 4.9l.7-.7M12 5a7 7 0 100 14 7 7 0 000-14z" stroke-width="1.2" stroke-linecap="round" stroke-linejoin="round"></path></svg>
      </button>
      <button id="toastBtn" class="rounded-lg px-3 py-1.5 bg-indigo-600 text-white text-sm font-medium hover:bg-indigo-500 transition-colors shadow-lg" title="Show toast">Sign Up</button>
    </div>
  </header>

  <!-- UI: Toasts container -->
  <div id="toasts" aria-live="polite" ></div>

  <!-- CONTENT: Main page content starts here -->
  <main class="relative z-10">

    <!-- HERO SECTION -->
    <section class="vh-screen-min flex items-center justify-center relative overflow-hidden">
      <div class="container mx-auto max-w-6xl px-6 text-center">
        <div class="reveal in-view">
          <p class="text-sm uppercase tracking-widest text-indigo-400 font-semibold">Unify your workflow</p>
          <h1 class="text-4xl sm:text-6xl lg:text-7xl font-extrabold tracking-tighter mt-4">Collaboration, <br>Simplified.</h1>
          <p class="mt-6 text-lg opacity-80 max-w-2xl mx-auto">StellarSync is the all-in-one platform that brings your tasks, documents, and team communication into a single, beautiful workspace.</p>
          <div class="mt-8 flex items-center justify-center gap-3">
            <button class="rounded-full px-6 py-3 bg-slate-900 text-white dark:bg-white dark:text-slate-900 font-semibold shadow-lg hover:scale-[1.02] transition-transform" onclick="pushToast('Welcome! Your journey starts now.')">Start for Free</button>
            <a href="#features" class="rounded-full px-5 py-2.5 border border-slate-300/20 glass text-sm hover:backdrop-brightness-95 transition">Learn more</a>
          </div>
        </div>
        <div class="mt-16 hero-media mx-auto w-[90%] max-w-5xl aspect-[16/7] rounded-3xl overflow-hidden glass border border-white/5">
          <div class="w-full h-full grid grid-cols-1 md:grid-cols-3 gap-4 p-4">
            <!-- Feature Card 1 -->
            <div class="rounded-xl bg-slate-200/50 dark:bg-slate-800/80 p-6 flex flex-col items-center text-center">
                <div class="w-12 h-12 rounded-full bg-indigo-500/20 grid place-items-center mb-4"><svg class="w-6 h-6 text-indigo-400" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2"><path stroke-linecap="round" stroke-linejoin="round" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" /></svg></div>
                <h3 class="font-semibold">Task Management</h3>
                <p class="text-sm opacity-70 mt-1">Organize, assign, and track progress seamlessly.</p>
            </div>
            <!-- Feature Card 2 -->
            <div class="rounded-xl bg-slate-200/50 dark:bg-slate-800/80 p-6 flex flex-col items-center text-center">
                <div class="w-12 h-12 rounded-full bg-emerald-500/20 grid place-items-center mb-4"><svg class="w-6 h-6 text-emerald-400" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2"><path stroke-linecap="round" stroke-linejoin="round" d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z" /></svg></div>
                <h3 class="font-semibold">Team Chat</h3>
                <p class="text-sm opacity-70 mt-1">Real-time messaging to keep everyone in sync.</p>
            </div>
            <!-- Feature Card 3 -->
            <div class="rounded-xl bg-slate-200/50 dark:bg-slate-800/80 p-6 flex flex-col items-center text-center">
                <div class="w-12 h-12 rounded-full bg-amber-500/20 grid place-items-center mb-4"><svg class="w-6 h-6 text-amber-400" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2"><path stroke-linecap="round" stroke-linejoin="round" d="M7 21a4 4 0 01-4-4V5a2 2 0 012-2h4a2 2 0 012 2v12a4 4 0 01-4 4zm0 0h12a2 2 0 002-2v-4a2 2 0 00-2-2h-2.343M11 7.343l1.657-1.657a2 2 0 012.828 0l2.829 2.829a2 2 0 010 2.828l-8.486 8.485M7 17h.01" /></svg></div>
                <h3 class="font-semibold">Shared Docs</h3>
                <p class="text-sm opacity-70 mt-1">Collaborate on documents with live editing.</p>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- FEATURES SECTION -->
    <section id="features" class="py-20 sm:py-32">
        <div class="container mx-auto max-w-6xl px-6">
            <div class="reveal text-center">
                <h2 class="text-3xl sm:text-4xl font-bold tracking-tight">Everything You Need, All in One Place</h2>
                <p class="mt-4 opacity-80 max-w-2xl mx-auto">Stop jumping between apps. StellarSync provides a unified hub for your team's productivity, designed for focus and flow.</p>
            </div>
            <div class="mt-16 grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
                <!-- Feature Item -->
                <div class="reveal p-8 glass border border-white/5 rounded-2xl">
                    <div class="w-12 h-12 rounded-lg bg-indigo-500/20 grid place-items-center mb-4"><svg class="w-6 h-6 text-indigo-400" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M4 6h16M4 12h16M4 18h16"></path></svg></div>
                    <h3 class="font-semibold text-lg">Project Boards</h3>
                    <p class="mt-2 text-sm opacity-70">Visualize your workflow with customizable Kanban boards, lists, and timeline views.</p>
                </div>
                <!-- Feature Item -->
                <div class="reveal p-8 glass border border-white/5 rounded-2xl" style="transition-delay: 100ms;">
                    <div class="w-12 h-12 rounded-lg bg-indigo-500/20 grid place-items-center mb-4"><svg class="w-6 h-6 text-indigo-400" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M21.21 15.89A10 10 0 1 1 8 2.83"></path><path d="M22 12A10 10 0 0 0 12 2v10z"></path></svg></div>
                    <h3 class="font-semibold text-lg">Powerful Analytics</h3>
                    <p class="mt-2 text-sm opacity-70">Gain insights into team performance and project progress with beautiful, easy-to-read dashboards.</p>
                </div>
                <!-- Feature Item -->
                <div class="reveal p-8 glass border border-white/5 rounded-2xl" style="transition-delay: 200ms;">
                    <div class="w-12 h-12 rounded-lg bg-indigo-500/20 grid place-items-center mb-4"><svg class="w-6 h-6 text-indigo-400" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"></path><circle cx="9" cy="7" r="4"></circle><path d="M23 21v-2a4 4 0 0 0-3-3.87"></path><path d="M16 3.13a4 4 0 0 1 0 7.75"></path></svg></div>
                    <h3 class="font-semibold text-lg">Guest Access</h3>
                    <p class="mt-2 text-sm opacity-70">Collaborate with clients and freelancers by giving them secure, permission-based access.</p>
                </div>
            </div>
        </div>
    </section>

    <!-- PRICING SECTION -->
    <section id="pricing" class="py-20 sm:py-32 bg-slate-100 dark:bg-slate-900/50">
        <div class="container mx-auto max-w-6xl px-6">
            <div class="reveal text-center">
                <h2 class="text-3xl sm:text-4xl font-bold tracking-tight">Simple, Transparent Pricing</h2>
                <p class="mt-4 opacity-80 max-w-2xl mx-auto">Choose the plan that's right for your team. All plans come with a 14-day free trial.</p>
            </div>
            <div class="mt-16 grid grid-cols-1 lg:grid-cols-3 gap-8">
                <!-- Pricing Tier 1 -->
                <div class="reveal p-8 glass border border-white/5 rounded-2xl flex flex-col">
                    <h3 class="font-semibold text-lg">Starter</h3>
                    <p class="text-4xl font-bold mt-4">$10 <span class="text-lg font-medium opacity-60">/ user / month</span></p>
                    <ul class="mt-6 space-y-3 text-sm opacity-80 flex-1">
                        <li class="flex items-center gap-3"><svg class="w-5 h-5 text-emerald-500" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="3"><path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7" /></svg> Up to 5 projects</li>
                        <li class="flex items-center gap-3"><svg class="w-5 h-5 text-emerald-500" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="3"><path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7" /></svg> Basic task management</li>
                        <li class="flex items-center gap-3"><svg class="w-5 h-5 text-emerald-500" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="3"><path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7" /></svg> 5GB file storage</li>
                    </ul>
                    <button class="mt-8 w-full rounded-lg px-6 py-3 border border-slate-300/20 text-sm hover:backdrop-brightness-95 transition">Choose Plan</button>
                </div>
                <!-- Pricing Tier 2 (Featured) -->
                <div class="reveal p-8 bg-indigo-600 text-white rounded-2xl flex flex-col shadow-2xl shadow-indigo-500/30 ring-2 ring-indigo-400">
                    <div class="flex justify-between items-center"><h3 class="font-semibold text-lg">Pro</h3> <span class="text-xs font-semibold uppercase tracking-wider bg-white/20 px-2 py-0.5 rounded-full">Most Popular</span></div>
                    <p class="text-4xl font-bold mt-4">$25 <span class="text-lg font-medium opacity-80">/ user / month</span></p>
                    <ul class="mt-6 space-y-3 text-sm opacity-90 flex-1">
                        <li class="flex items-center gap-3"><svg class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="3"><path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7" /></svg> Unlimited projects</li>
                        <li class="flex items-center gap-3"><svg class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="3"><path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7" /></svg> Advanced analytics & reports</li>
                        <li class="flex items-center gap-3"><svg class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="3"><path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7" /></svg> Guest access & integrations</li>
                        <li class="flex items-center gap-3"><svg class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="3"><path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7" /></svg> Priority support</li>
                    </ul>
                    <button class="mt-8 w-full rounded-lg px-6 py-3 bg-white text-indigo-600 font-semibold text-sm hover:bg-white/90 transition-colors">Choose Plan</button>
                </div>
                <!-- Pricing Tier 3 -->
                <div class="reveal p-8 glass border border-white/5 rounded-2xl flex flex-col">
                    <h3 class="font-semibold text-lg">Enterprise</h3>
                    <p class="text-3xl font-bold mt-4">Let's Talk</p>
                    <ul class="mt-6 space-y-3 text-sm opacity-80 flex-1">
                        <li class="flex items-center gap-3"><svg class="w-5 h-5 text-emerald-500" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="3"><path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7" /></svg> All Pro features</li>
                        <li class="flex items-center gap-3"><svg class="w-5 h-5 text-emerald-500" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="3"><path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7" /></svg> Dedicated account manager</li>
                        <li class="flex items-center gap-3"><svg class="w-5 h-5 text-emerald-500" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="3"><path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7" /></svg> Custom security & compliance</li>
                        <li class="flex items-center gap-3"><svg class="w-5 h-5 text-emerald-500" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="3"><path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7" /></svg> On-premise deployment option</li>
                    </ul>
                    <button class="mt-8 w-full rounded-lg px-6 py-3 border border-slate-300/20 text-sm hover:backdrop-brightness-95 transition">Contact Sales</button>
                </div>
            </div>
        </div>
    </section>

    <!-- TESTIMONIALS SECTION -->
    <section id="testimonials" class="py-20 sm:py-32 overflow-hidden">
        <div class="container mx-auto max-w-6xl px-6">
            <div class="reveal text-center">
                <h2 class="text-3xl sm:text-4xl font-bold tracking-tight">Loved by Teams Worldwide</h2>
                <p class="mt-4 opacity-80 max-w-2xl mx-auto">Don't just take our word for it. Here's what some of our amazing customers have to say.</p>
            </div>
            <div class="mt-16 grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
                 <!-- Testimonial Card -->
                 <div class="reveal p-8 glass border border-white/5 rounded-2xl">
                    <p class="opacity-90">"StellarSync has been a game-changer for our workflow. We're more organized and efficient than ever before."</p>
                    <div class="flex items-center gap-3 mt-6">
                        <img src="https://i.pravatar.cc/48?u=1" alt="Avatar" class="w-12 h-12 rounded-full object-cover">
                        <div>
                            <p class="font-semibold">Sarah L.</p>
                            <p class="text-sm opacity-60">Marketing Director, Acme Inc.</p>
                        </div>
                    </div>
                 </div>
                 <!-- Testimonial Card -->
                 <div class="reveal p-8 glass border border-white/5 rounded-2xl md:mt-8" style="transition-delay: 100ms;">
                    <p class="opacity-90">"The interface is intuitive and beautiful. It's the first project management tool our team has actually enjoyed using."</p>
                    <div class="flex items-center gap-3 mt-6">
                        <img src="https://i.pravatar.cc/48?u=2" alt="Avatar" class="w-12 h-12 rounded-full object-cover">
                        <div>
                            <p class="font-semibold">Mike R.</p>
                            <p class="text-sm opacity-60">Lead Engineer, Innovate Co.</p>
                        </div>
                    </div>
                 </div>
                 <!-- Testimonial Card -->
                 <div class="reveal p-8 glass border border-white/5 rounded-2xl" style="transition-delay: 200ms;">
                    <p class="opacity-90">"From task tracking to document collaboration, it does everything we need. Support has been fantastic too."</p>
                    <div class="flex items-center gap-3 mt-6">
                        <img src="https://i.pravatar.cc/48?u=3" alt="Avatar" class="w-12 h-12 rounded-full object-cover">
                        <div>
                            <p class="font-semibold">Elena Chen</p>
                            <p class="text-sm opacity-60">Founder, Creative Studio</p>
                        </div>
                    </div>
                 </div>
            </div>
        </div>
    </section>

    <!-- FINAL CTA SECTION -->
    <section class="py-20 sm:py-32">
        <div class="container mx-auto max-w-4xl px-6">
            <div class="reveal text-center bg-gradient-to-br from-indigo-600 to-purple-600 p-12 rounded-3xl">
                <h2 class="text-3xl sm:text-4xl font-bold tracking-tight text-white">Ready to Streamline Your Workflow?</h2>
                <p class="mt-4 text-white/80 max-w-2xl mx-auto">Join thousands of teams building their best work with StellarSync. Get started for free—no credit card required.</p>
                <div class="mt-8 flex items-center justify-center gap-3">
                    <button class="rounded-full px-6 py-3 bg-white text-indigo-600 font-semibold shadow-lg hover:scale-[1.02] transition-transform" onclick="pushToast('Let\'s go! Setting up your workspace...')">Start Your Free Trial</button>
                </div>
            </div>
        </div>
    </section>

    <!-- FOOTER -->
    <footer class="py-12 border-t border-slate-200 dark:border-slate-800">
        <div class="container mx-auto max-w-6xl px-6 flex flex-col md:flex-row justify-between items-center gap-6">
            <p class="text-sm opacity-70">&copy; 2024 StellarSync. All rights reserved.</p>
            <div class="flex items-center gap-4 text-sm opacity-80">
                <a href="#" class="hover:text-indigo-400 transition-colors">Terms</a>
                <a href="#" class="hover:text-indigo-400 transition-colors">Privacy</a>
                <a href="#" class="hover:text-indigo-400 transition-colors">Contact</a>
            </div>
        </div>
    </footer>
  </main>

  <!-- MAIN SCRIPT: Handles all interactivity -->
  <script>
    /* -------------------------
       Theme toggle + init
    ------------------------- */
    const root = document.documentElement;
    const toggle = document.getElementById('toggleTheme');
    const iconTheme = document.getElementById('iconTheme');
    const stored = localStorage.getItem('site-theme');
    if (stored === 'dark' || (!stored && window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches)) {
      root.classList.add('dark');
    } else {
      root.classList.remove('dark');
    }
    function updateThemeIcon() {
      if (root.classList.contains('dark')) {
        iconTheme.innerHTML = '<path d=\"M21 12.79A9 9 0 1111.21 3 7 7 0 0021 12.79z\" stroke-width=\"1.2\" stroke-linecap=\"round\" stroke-linejoin=\"round\" />';
      } else {
        iconTheme.innerHTML = '<path d=\"M12 3v1M12 20v1M4.2 4.2l.7.7M19.1 19.1l.7.7M1 12h1M22 12h1M4.2 19.8l.7-.7M19.1 4.9l.7-.7M12 5a7 7 0 100 14 7 7 0 000-14z\" stroke-width=\"1.2\" stroke-linecap=\"round\" stroke-linejoin=\"round\" />';
      }
    }
    updateThemeIcon();
    toggle.addEventListener('click', () => {
      root.classList.toggle('dark');
      localStorage.setItem('site-theme', root.classList.contains('dark') ? 'dark' : 'light');
      updateThemeIcon();
    });

    /* -------------------------
       Scroll progress bar
    ------------------------- */
    const progress = document.getElementById('progress');
    const pctLabel = document.getElementById('pctLabel');
    function updateProgress() {
      const scrolled = window.scrollY;
      const docHeight = Math.max(document.documentElement.scrollHeight, document.body.scrollHeight) - window.innerHeight;
      const pct = docHeight > 0 ? Math.round((scrolled / docHeight) * 100) : 0;
      progress.style.width = pct + '%';
      pctLabel.textContent = pct + '%';
      progress.setAttribute('aria-valuenow', pct);
    }
    window.addEventListener('scroll', updateProgress, { passive: true });
    window.addEventListener('resize', updateProgress);
    updateProgress();

    /* -------------------------
       Simple toast system
    ------------------------- */
    const toastsWrap = document.getElementById('toasts');
    let toastCounter = 0;
    function pushToast(msg, opts = {}) {
      const id = ++toastCounter;
      const el = document.createElement('div');
      el.className = 'max-w-xs w-full rounded-lg p-3 shadow-lg glass flex items-start gap-3 border border-white/10';
      el.style.animation = 'toastIn .28s cubic-bezier(.2,.9,.2,1)';
      el.setAttribute('role', 'status');
      el.setAttribute('aria-live', 'polite');
      el.innerHTML = `
        <div class="flex-none mt-0.5"><div class="w-8 h-8 rounded-full bg-indigo-600 text-white grid place-items-center text-lg font-semibold">
            <svg xmlns="http://www.w3.org/2000/svg" height="24px" viewBox="0 -960 960 960" width="24px" fill="#e3e3e3"><path d="M382-240 154-468l57-57 171 171 367-367 57 57-424 424Z"/></svg>
        </div></div>
        <div class="flex-1"><div class="font-medium text-sm">${escapeHtml(msg)}</div><div class="text-sm opacity-70 mt-1">${opts.sub || ''}</div></div>
        <button aria-label="dismiss toast" class="ml-auto opacity-60 hover:opacity-90 flex-none" style="background:none;border:none;font-size:1.1rem;line-height:1;">&times;</button>
      `;
      const btn = el.querySelector('button');
      btn.addEventListener('click', () => removeToast(id, el));
      toastsWrap.prepend(el);
      const removeAfter = opts.timeout ?? 4200;
      const timer = setTimeout(() => removeToast(id, el), removeAfter);
      function removeToast(id, el) {
        clearTimeout(timer);
        el.style.animation = 'toastOut .28s cubic-bezier(.2,.9,.2,1)';
        el.style.opacity = '0';
        setTimeout(() => el.remove(), 300);
      }
    }
    window.pushToast = pushToast;
    function escapeHtml(str) {
      return String(str).replace(/[&<>\"']/g, m => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '\"': '&quot;', "'": '&#39;' }[m]));
    }
    document.getElementById('toastBtn').addEventListener('click', () => pushToast('This is a sample toast notification!'));

    /* -------------------------
       IntersectionObserver for reveal animations
    ------------------------- */
    const revealElems = document.querySelectorAll('.reveal');
    const io = new IntersectionObserver((entries) => {
      for (const e of entries) {
        if (e.isIntersecting) {
          e.target.classList.add('in-view');
        // Optional: unobserve after revealing to save performance
        // io.unobserve(e.target);
        } else {
        // Optional: hide element again when it scrolls out of view
        // e.target.classList.remove('in-view');
        }
      }
    }, { root: null, threshold: 0.15, rootMargin: '0px 0px -50px 0px' });
    revealElems.forEach(el => io.observe(el));

    /* -------------------------
       Responsive particle canvas
    ------------------------- */
    const canvas = document.getElementById('bgCanvas'), ctx = canvas.getContext('2d');
    let W = canvas.width = innerWidth, H = canvas.height = innerHeight, particles = [];
    function rand(min, max) { return Math.random() * (max - min) + min; }
    function initParticles() {
      particles = [];
      const count = Math.max(24, Math.floor((W * H) / 90000));
      for (let i = 0; i < count; i++) {
        particles.push({ x: rand(0, W), y: rand(0, H), r: rand(0.6, 3.6), vx: rand(-0.25, 0.45), vy: rand(-0.15, 0.15), a: rand(0.06, 0.6), });
      }
    }
    initParticles();
    function drawParticles() {
      ctx.clearRect(0,0,W,H);
      const g = ctx.createLinearGradient(0, 0, W, H);
      if (document.documentElement.classList.contains('dark')) {
        g.addColorStop(0, 'rgba(6,6,23,0.25)'); g.addColorStop(1, 'rgba(17,24,39,0.25)');
      } else {
        g.addColorStop(0, 'rgba(245,247,250,0.25)'); g.addColorStop(1, 'rgba(220,234,255,0.25)');
      }
      ctx.fillStyle = g; ctx.fillRect(0,0,W,H);
      for (const p of particles) {
        p.x += p.vx; p.y += p.vy;
        if (p.x > W + 20) p.x = -20; if (p.x < -20) p.x = W + 20;
        if (p.y > H + 20) p.y = -20; if (p.y < -20) p.y = H + 20;
        ctx.beginPath(); ctx.globalAlpha = p.a; ctx.arc(p.x, p.y, p.r, 0, Math.PI*2);
        ctx.fillStyle = document.documentElement.classList.contains('dark') ? 'rgba(140,200,180,1)' : 'rgba(34,139,230,1)';
        ctx.fill();
      }
      requestAnimationFrame(drawParticles);
    }
    if (!window.matchMedia('(prefers-reduced-motion: reduce)').matches) {
      drawParticles();
    }
    window.addEventListener('resize', () => { W = canvas.width = innerWidth; H = canvas.height = innerHeight; initParticles(); });

    /* -------------------------
       Minor hover parallax on hero media
    ------------------------- */
    const hero = document.querySelector('.hero-media');
    if (hero && !window.matchMedia('(prefers-reduced-motion: reduce)').matches) {
      hero.addEventListener('mousemove', (ev) => {
        const r = hero.getBoundingClientRect();
        const mx = (ev.clientX - (r.left + r.width/2)) / r.width;
        const my = (ev.clientY - (r.top + r.height/2)) / r.height;
        hero.style.transform = `perspective(1000px) rotateY(${mx * 5}deg) rotateX(${-my * 5}deg) scale3d(1.02, 1.02, 1.02)`;
        hero.style.filter = `drop-shadow(0 30px 50px rgba(2,6,23,0.45))`;
      });
      hero.addEventListener('mouseleave', () => { 
        hero.style.transform = 'perspective(1000px) rotateY(0deg) rotateX(0deg) scale3d(1, 1, 1)'; 
        hero.style.filter = `drop-shadow(0 20px 40px rgba(2,6,23,0.35))`;
      });
    }
    
    /* -------------------------
       Smooth scroll for nav links
    ------------------------- */
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();
            document.querySelector(this.getAttribute('href')).scrollIntoView({
                behavior: 'smooth'
            });
        });
    });
  </script>
</body>
</html>
```