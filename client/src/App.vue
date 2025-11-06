<script setup>
// This file now only imports your components
import Sidebar from './components/SidebarSubsection.vue'
import Container from './components/ContainerSubsection.vue'
</script>

<template>
  <v-app>
    <Sidebar />

    <v-main class="bg-grey-lighten-4">
      <Container />
    </v-main>
  </v-app>
</template>

<style>
/* You can put global styles here if you want */
</style>
