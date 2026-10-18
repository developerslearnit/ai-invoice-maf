/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./Views/**/*.cshtml",
    "./wwwroot/js/**/*.js"
  ],
  darkMode: 'class',
  theme: {
    extend: {
      colors: {
        brand: {
          50: '#f5f7ff',
          100: '#ebf0fe',
          200: '#ced9fd',
          300: '#a4b7fb',
          400: '#758df7',
          500: '#4f63f3',
          600: '#3b47e8',
          700: '#2f35d4',
          800: '#272cab',
          900: '#242a87',
          950: '#151752',
        }
      },
      fontFamily: {
        sans: ['Inter', 'system-ui', '-apple-system', 'BlinkMacSystemFont', 'Segoe UI', 'Roboto', 'Helvetica Neue', 'Arial', 'sans-serif'],
      },
      boxShadow: {
        'soft': '0 2px 15px -3px rgba(0, 0, 0, 0.07), 0 10px 20px -2px rgba(0, 0, 0, 0.04)',
        'soft-lg': '0 10px 30px -5px rgba(0, 0, 0, 0.08), 0 20px 25px -5px rgba(0, 0, 0, 0.04)',
        'floating': '0 20px 35px -5px rgba(79, 70, 229, 0.25), 0 10px 15px -5px rgba(0, 0, 0, 0.05)',
      }
    },
  },
  plugins: [],
}
