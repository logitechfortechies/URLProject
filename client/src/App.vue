<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import Sidebar from './components/Sidebar.vue'

const router = useRouter()
const userEmail = ref(null)

onMounted(() => {
  userEmail.value = localStorage.getItem('userEmail')
})

function handleLogout() {
  localStorage.clear() // Clear the token and email
  router.push('/login') // Redirect to login
}
</script>

<template>
  <v-app>
    <Sidebar />

    <v-app-bar app elevation="1">
      <v-spacer></v-spacer>

      <v-chip v-if="userEmail" pill class="mr-4">
        <v-avatar start>
          <v-icon>mdi-account-circle</v-icon>
        </v-avatar>
        {{ userEmail }}
      </v-chip>

      <v-btn @click="handleLogout" icon>
        <v-icon>mdi-logout</v-icon>
      </v-btn>
    </v-app-bar>

    <v-main>
      <router-view />
    </v-main>
  </v-app>
</template>
