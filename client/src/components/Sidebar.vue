<script setup>
import { ref } from 'vue'

// This will control which item is currently "active"
const selectedItem = ref(['Dashboard']) // Add brackets

// This is the list of links for your sidebar
const items = [
  { title: 'Dashboard', icon: 'mdi-view-dashboard', value: 'Dashboard' },
  { title: 'Links', icon: 'mdi-link', value: 'Links' },
  { title: 'QR Codes', icon: 'mdi-qrcode', value: 'QR Codes' },
  { title: 'Pages', icon: 'mdi-file-document-outline', value: 'Pages' },
  { title: 'Analytics', icon: 'mdi-chart-bar', value: 'Analytics' },
]
</script>

<template>
  <v-navigation-drawer permanent app width="250" class="pa-2">
    <v-list-item
      title="URL Shortener"
      subtitle="Hi im out of ideas"
      class="text-h5 font-weight-bold mb-4"
    ></v-list-item>

    <v-list v-model:selected="selectedItem" nav density="compact" color="primary">
      <v-list-item
        v-for="item in items"
        :key="item.value"
        :title="item.title"
        :prepend-icon="item.icon"
        :value="item.value"
        rounded="lg"
      ></v-list-item>
    </v-list>
  </v-navigation-drawer>
</template>

<style scoped>
/* You can add custom styles here if needed */
.v-list-item--active {
  font-weight: bold;
}
</style>
