import { createApp } from 'vue'
import App from './App.vue'

// Import Vuetify styles
import '@mdi/font/css/materialdesignicons.css'
import 'vuetify/styles'

// Import Vuetify
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'

// --- 1. DEFINE YOUR CUSTOM THEME ---
// We create a new "dark" theme inspired by your screenshot
const myCustomDarkTheme = {
  dark: true,
  colors: {
    background: '#2a0c4e', // A rich dark purple
    surface: '#FFFFFF', // Your white cards
    primary: '#6D28D9', // A brighter purple for accents
    secondary: '#ECECEC', // A light grey for contrast
    error: '#B00020',
    info: '#2196F3',
    success: '#4CAF50',
    warning: '#FB8C00',
  },
}

// --- 2. CREATE THE VUETIFY INSTANCE ---
const vuetify = createVuetify({
  components,
  directives,

  // --- 3. APPLY THE THEME ---
  theme: {
    defaultTheme: 'myCustomDarkTheme', // Set your new theme as the default
    themes: {
      myCustomDarkTheme, // Register your new theme
    },
  },

  // --- 4. APPLY THE FONT ---
  typography: {
    fontFamily: '"Poppins", "sans-serif"', // Set Poppins as the default font
  },
})

// Mount the app and tell it to use Vuetify
createApp(App).use(vuetify).mount('#app')
