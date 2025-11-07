<script setup>
import { ref } from 'vue'

// All your logic now lives in this component
const longUrl = ref('')
const shortUrl = ref('')
const qrCodeBase64 = ref('') // This stores the QR code
const isLoading = ref(false)
const errorMessage = ref('')

// This is your live API URL on Render
const API_URL = '' // This will now call /api/shorten

async function createShortUrl() {
  isLoading.value = true
  errorMessage.value = ''
  shortUrl.value = ''
  qrCodeBase64.value = '' // Reset the QR code

  try {
    const response = await fetch(`${API_URL}/api/shorten`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ longUrl: longUrl.value }),
    })

    if (!response.ok) {
      throw new Error('Failed to create short URL. Is the URL valid?')
    }

    const data = await response.json()
    shortUrl.value = data.shortUrl
    qrCodeBase64.value = data.qrCodeBase64 // Save the QR code
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
  <v-container>
    <v-row justify="center">
      <v-col cols="12" md="10" lg="8">
        <v-card class="pa-4 pa-md-6" elevation="2">
          <h1 class="text-h4 text-center mb-6">URL Shortener</h1>
          <v-text-field
            v-model="longUrl"
            label="Paste your long URL here"
            variant="outlined"
            placeholder="https://www.a-very-long-url-to-shorten.com/..."
            clearable
            @keyup.enter="createShortUrl"
          ></v-text-field>

          <v-btn :loading="isLoading" @click="createShortUrl" color="primary" size="large" block>
            Shorten!
          </v-btn>
        </v-card>

        <v-card v-if="shortUrl" class="mt-6 pa-4 pa-md-6" elevation="2">
          <div class="text-h6 mb-4">Your short link is ready!</div>
          <v-text-field :model-value="shortUrl" label="Short Link" variant="filled" readonly>
            <template v-slot:append-inner>
              <v-btn
                icon="mdi-content-copy"
                variant="text"
                @click="copyToClipboard"
                title="Copy to clipboard"
              ></v-btn>
            </template>
          </v-text-field>

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
