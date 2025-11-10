<script setup>
import { ref, onMounted } from 'vue'

const links = ref([])
const isLoading = ref(true)
const errorMessage = ref('')

const API_URL = 'https://shortly-app.onrender.com' // Use your new custom URL

async function fetchLinks() {
  const token = localStorage.getItem('authToken')
  if (!token) {
    errorMessage.value = "You are not logged in."
    isLoading.value = false
    return
  }

  try {
    const response = await fetch(`${API_URL}/api/my-links`, {
      headers: { 'Authorization': `Bearer ${token}` }
    })

    if (!response.ok) {
      throw new Error('Failed to fetch links.')
    }

    links.value = await response.json()
  } catch (error) {
    errorMessage.value = error.message
  } finally {
    isLoading.value = false
  }
}

// This runs the fetchLinks function when the page loads
onMounted(fetchLinks)

// Vuetify Data Table headers
const headers = [
  { title: 'Short Link', value: 'shortCode' },
  { title: 'Original URL', value: 'longUrl' },
  { title: 'Created', value: 'createdOnUtc' },
]
</script>

<template>
  <v-container>
    <v-row>
      <v-col>
        <v-card class="pa-4 pa-md-6" elevation="2">
          <h1 class="text-h4 mb-6">My Links</h1>

          <v-alert v-if="errorMessage" type="error" class="mb-6">
            {{ errorMessage }}
          </v-alert>

          <v-data-table
            :headers="headers"
            :items="links"
            :loading="isLoading"
            item-key="id"
            class="elevation-1"
          >
            <template v-slot:item.longUrl="{ item }">
              <span class="d-inline-block text-truncate" style="max-width: 300px;">
                {{ item.longUrl }}
              </span>
            </template>

            <template v-slot:item.createdOnUtc="{ item }">
              {{ new Date(item.createdOnUtc).toLocaleString() }}
            </template>

             <template v-slot:item.shortCode="{ item }">
              <a :href="`${API_URL}/${item.shortCode}`" target="_blank">
                {{ item.shortCode }}
              </a>
            </template>
          </v-data-table>

        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>
