import { createApp } from 'vue'
import App from './App.vue'

// 1. Import Vuetify styles
import '@mdi/font/css/materialdesignicons.css'
import 'vuetify/styles'

// 2. Import Vuetify
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'

// 3. Create the Vuetify instance
const vuetify = createVuetify({
  components,
  directives,
})

// 4. Mount the app and tell it to use Vuetify
createApp(App).use(vuetify).mount('#app')
