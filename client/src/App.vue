<script setup>
import { ref } from 'vue'

// This stores what the user types in the text box
const longUrl = ref('')

// This will store the result from your API
const shortUrl = ref('')
const isLoading = ref(false)
const errorMessage = ref('')

// This is your live API URL on Render
const API_URL = 'https://nginx-latest-1-mem4.onrender.com'

// This function runs when the user clicks the button
async function createShortUrl() {
  isLoading.value = true
  errorMessage.value = ''
  shortUrl.value = ''

  try {
    const response = await fetch(`${API_URL}/api/shorten`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        longUrl: longUrl.value,
      }),
    })

    if (!response.ok) {
      // Handle errors from the API (like "Invalid URL")
      const errorData = await response.json()
      throw new Error(errorData.title || 'An error occurred.')
    }

    // Get the JSON response (e.g., { "shortUrl": "https://.../abc" })
    const data = await response.json()
    shortUrl.value = data.shortUrl
  } catch (error) {
    errorMessage.value = error.message
  } finally {
    isLoading.value = false
  }
}
</script>

<template>
  <main>
    <div class="wrapper">
      <h1>My URL Shortener</h1>

      <div class="form-container">
        <input
          type="text"
          v-model="longUrl"
          placeholder="Enter your long URL here (e.g., https://...)"
        />
        <button @click="createShortUrl" :disabled="isLoading">
          {{ isLoading ? 'Shortening...' : 'Shorten!' }}
        </button>
      </div>

      <div v="if" class="result-container">
        <div v-if="shortUrl" class="success-result">
          <p>Here is your short link:</p>
          <a :href="shortUrl" target="_blank">{{ shortUrl }}</a>
        </div>

        <div v-if="errorMessage" class="error-result">
          <p>{{ errorMessage }}</p>
        </div>
      </div>
    </div>
  </main>
</template>

<style scoped>
main {
  display: flex;
  justify-content: center;
  align-items: flex-start;
  padding-top: 50px;
  width: 100%;
  min-height: 100vh;
  background-color: #f0f2f5;
  font-family: Arial, sans-serif;
}

.wrapper {
  width: 100%;
  max-width: 600px;
  padding: 2rem;
  background-color: #ffffff;
  border-radius: 8px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
}

h1 {
  text-align: center;
  color: #333;
  margin-bottom: 2rem;
}

.form-container {
  display: flex;
  gap: 10px;
}

input[type='text'] {
  flex-grow: 1;
  padding: 12px;
  font-size: 1rem;
  border: 1px solid #ccc;
  border-radius: 4px;
}

button {
  padding: 12px 20px;
  font-size: 1rem;
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  transition: background-color 0.2s;
}

button:hover {
  background-color: #0056b3;
}

button:disabled {
  background-color: #ccc;
  cursor: not-allowed;
}

.result-container {
  margin-top: 2rem;
  padding: 1rem;
  border-radius: 4px;
  background-color: #f8f9fa;
  word-wrap: break-word;
}

.success-result a {
  font-size: 1.1rem;
  color: #007bff;
  font-weight: bold;
}

.error-result p {
  color: #d93025;
  font-weight: bold;
}
</style>
