<script setup>
import { ref } from 'vue'

// --- 1. STATE ---
const longUrl = ref('')
const customAlias = ref('')
const shortUrl = ref('')
const qrCodeBase64 = ref('')
const isLoading = ref(false)
const errorMessage = ref('')


const API_URL = ''

// --- 2. THE createShortUrl FUNCTION
async function createShortUrl() {
  isLoading.value = true
  errorMessage.value = ''
  shortUrl.value = ''
  qrCodeBase64.value = ''

  // 2a. Get the token from storage
  const token = localStorage.getItem('authToken')

  if (!token) {
    errorMessage.value = 'You are not logged in. Please go to the Login page.'
    isLoading.value = false
    return
  }

  // 2b. Add the token to the 'Authorization' header
  const headers = {
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token}` // Bearer token authentication
  }

  // 2c. Add the new 'customAlias' to the request body
  const body = JSON.stringify({
    longUrl: longUrl.value,
    customAlias: customAlias.value || null // Send the alias or null
  })

  try {
    const response = await fetch(`${API_URL}/api/shorten`, {
      method: 'POST',
      headers: headers,
      body: body,
    })

    // 2d. Handle validation and other errors from the API
    if (!response.ok) {
       if (response.status === 401) {
        throw new Error('Your session has expired. Please log in again.')
       }
       // This will display validation errors (e.g., "Alias already taken")
       const errorData = await response.json()
       if (errorData.errors) {
         // Get the first error message from the validation object
         const firstErrorKey = Object.keys(errorData.errors)[0];
         const firstError = errorData.errors[firstErrorKey][0];
         throw new Error(firstError)
       }
       throw new Error(errorData.title || 'An unknown error occurred.')
    }

    // 2e. Set the results on success
    const data = await response.json()
    shortUrl.value = data.shortUrl
    qrCodeBase64.value = data.qrCodeBase64

  } catch (error) {
    errorMessage.value = error.message
  } finally {
    isLoading.value = false
  }
}

async function copyToClipboard() {
  try {
    await navigator.clipboard.writeText(shortUrl.value)
    alert('Copied to clipboard!')
  } catch (err) {
    errorMessage.value = 'Failed to copy text.'
  }
}
</script>

<template>
  <!-- 3. THE TEMPLATE -->
  <v-container>
    <v-row justify="center">
      <v-col cols="12" md="10" lg="8">

        <!-- The Main Input Card -->
        <v-card class="pa-4 pa-md-6" elevation="2">
          <h1 class="text-h4 text-center mb-6">URL Shortener</h1>

          <v-text-field
            v-model="longUrl"
            label="Paste your long URL here"
            variant="outlined"
            placeholder="https://www.a-very-long-url-to-shorten.com/..."
            clearable
          ></v-text-field>

          <!-- 3a. THIS IS THE  INPUT FIELD FOR THE ALIAS -->
          <v-text-field
            v-model="customAlias"
            label="Custom alias (optional)"
            variant="outlined"
            placeholder="my-cool-link"
            clearable
            class="mt-4"
          ></v-text-field>

          <v-btn
            :loading="isLoading"
            @click="createShortUrl"
            color="primary"
            size="large"
            block
            class="mt-4"
          >
            Shorten!
          </v-btn>
        </v-card>

        <!-- The Result Card -->
        <v-card v-if="shortUrl" class="mt-6 pa-4 pa-md-6" elevation="2">
          <div class="text-h6 mb-4">Your short link is ready!</div>
          <v-text-field
            :model-value="shortUrl"
            label="Short Link"
            variant="filled"
            readonly
          >
            <template v-slot:append-inner>
              <v-btn
                icon="mdi-content-copy"
                variant="text"
                @click="copyToClipboard"
                title="Copy to clipboard"
              ></v-btn>
            </template>
          </v-text-field>

          <!-- QR Code Display -->
          <v-row justify="center" class="mt-4">
            <v-col cols="auto">
              <v-img
                :src="qrCodeBase64"
                alt="QR Code"
                width="200"
                height="200"
                class="bg-white"
              ></v-img>
            </v-col>
          </v-row>
        </v-card>

        <v-alert
          v-if="errorMessage"
          type="error"
          class="mt-6"
          closable
          @click:close="errorMessage = ''"
        >
          {{ errorMessage }}
        </v-alert>

      </v-col>
    </v-row>
  </v-container>
</template>
