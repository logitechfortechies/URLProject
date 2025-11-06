<script setup>
import Sidebar from './components/Sidebar.vue'
import Dashboard from './components/Dashboard.vue'
</script>

<template>
  <v-app>
    <Sidebar />

    <v-main class="bg-grey-lighten-5">
      <Dashboard />
    </v-main>
  </v-app>
</template>

<style>
/* Global styles if needed */
</style>
