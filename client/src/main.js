import router from './router'
import { createApp } from 'vue'
import App from './App.vue'

// 1. Import Vuetify styles
import '@mdi/font/css/materialdesignicons.css'
import 'vuetify/styles'

// 2. Import Vuetify
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'

// --- 1. DEFINE CUSTOM THEME ---
const myCustomLightTheme = {
  dark: true,
  colors: {
    background: '#2a0c4e', // A rich dark purple
    surface: '#FFFFFF', // White for surfaces
    primary: '#EE6123', // A vibrant orange
    secondary: '#0B234F',// A deep navy blue
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
    defaultTheme: 'myCustomLightTheme',
    themes: {
      myCustomLightTheme,
    },
  },

  // --- 4. APPLY THE FONT ---
  typography: {
    fontFamily: '"Poppins", "sans-serif"', // Set Poppins as the default font
  },
})

createApp(App).use(router).use(vuetify).mount('#app')
