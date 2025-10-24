### **Setup Tailwind CSS v4**

1. **Install Dependencies**
   ```bash
   npm install tailwindcss @tailwindcss/cli
   ```

2. **Create Input CSS**
   * Path: `src/input.css`
   * Content:
     ```css
     @import "tailwindcss";
     ```

3. **Create HTML File**
   * Path: `src/index.html`
   * Link output.css into the html file:
     ```html
       <link href="./output.css" rel="stylesheet" />
     ```

4. **Build Tailwind Output**
   ```bash
   npx @tailwindcss/cli -i ./src/input.css -o ./src/output.css
   ```