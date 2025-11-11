<script setup>
import { ref } from 'vue'


const selectedItem = ref(['/'])


const items = [
  { title: 'Dashboard', icon: 'mdi-view-dashboard', value: '/', to: '/' },
  { title: 'My Links', icon: 'mdi-link', value: '/links', to: '/links' },
  { title: 'Login', icon: 'mdi-login', value: '/login', to: '/login' },
  { title: 'Register', icon: 'mdi-account-plus', value: '/register', to: '/register' },
]
</script>

<template>
  <v-navigation-drawer permanent app width="250" class="pa-2">
    <v-list-item class="mb-4">
      <template v-slot:prepend>
        <v-img src="/my-logo.png" width="200" contain class="mr-3"></v-img>
      </template>
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
