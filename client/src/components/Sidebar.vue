<script setup>
import { ref } from 'vue'

// This will control which item is currently "active"
// We use an array to fix the Vuetify warning.
// We use the route path (e.g., '/') as the value.
const selectedItem = ref(['/'])

// This is the list of links for your sidebar.
// The "to" property tells vue-router where to navigate.
const items = [
  { title: 'Dashboard', icon: 'mdi-view-dashboard', value: '/', to: '/' },
  { title: 'Login', icon: 'mdi-login', value: '/login', to: '/login' },
  { title: 'Register', icon: 'mdi-account-plus', value: '/register', to: '/register' },
  // { title: 'Links', icon: 'mdi-link', value: 'Links' },
  // { title: 'QR Codes', icon: 'mdi-qrcode', value: 'QR Codes' },
]
</script>

<template>
  <v-navigation-drawer
    permanent
    app
    width="250"
    class="pa-2"
  >
    <v-list-item
      title="Shortly-URLShortner"
      subtitle="I'm out of ideas -seriously"
      class="text-h5 font-weight-bold mb-4"
    ></v-list-item>

    <v-list
      v-model:selected="selectedItem"
      nav
      density="compact"
      color="primary"
    >
      <v-list-item
        v-for="item in items"
        :key="item.value"
        :title="item.title"
        :prepend-icon="item.icon"
        :value="item.value"
        :to="item.to"
        rounded="lg"
      ></v-list-item>
    </v-list>
  </v-navigation-drawer>
</template>

<style scoped>
/* You can add custom styles here if you want */
.v-list-item--active {
  font-weight: bold;
}
</style>
