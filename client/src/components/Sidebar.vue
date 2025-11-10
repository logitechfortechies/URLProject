<script setup>
import { ref } from 'vue'

// This will control which item is currently "active"

const selectedItem = ref(['/'])

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
  <v-navigation-drawer permanent app width="250" class="pa-2">
    <v-list-item class="mb-4">
      <template v-slot:prepend>
        <v-img src="/my-logo.png" width="40" contain class="mr-3"></v-img>
      </template>
      <v-list-item-title class="text-h5 font-weight-bold"> Shortener </v-list-item-title>
      <v-list-item-subtitle> Your Project </v-list-item-subtitle>
    </v-list-item>
    <v-list v-model:selected="selectedItem" nav density="compact" color="primary">
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
.v-list-item--active {
  font-weight: bold;
}
</style>
