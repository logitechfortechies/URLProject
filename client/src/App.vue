<script setup>
import Sidebar from './components/Sidebar.vue'
</script>

<template>
  <v-app>
    <Sidebar />

    <v-main class="bg-grey-lighten-5">
      <router-view />
    </v-main>
  </v-app>
</template>

<style>
/* Global styles if needed */
</style>
