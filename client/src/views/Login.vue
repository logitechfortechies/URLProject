<script setup>
import { ref } from 'vue'
// We will use the router to redirect on success
import { useRouter } from 'vue-router'

const email = ref('')
const password = ref('')
const isLoading = ref(false)
const errorMessage = ref('')
const router = useRouter()

//Live API URL on render
const API_URL = ''

async function handleLogin() {
  isLoading.value = true
  errorMessage.value = ''

  try {
    const response = await fetch(`${API_URL}/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        email: email.value,
        password: password.value,
      }),
    })

    const data = await response.json()

    if (!response.ok) {
      throw new Error(data.title || 'Login failed.')
    }

    // On success, store the token and redirect
    localStorage.setItem('authToken', data.accessToken)
    router.push('/') // Redirect to the main dashboard
  } catch (error) {
    errorMessage.value = error.message
  } finally {
    isLoading.value = false
  }
}
</script>

<template>
  <v-container>
    <v-row justify="center">
      <v-col cols="12" md="6" lg="4">
        <v-card class="pa-4 pa-md-6" elevation="2">
          <h1 class="text-h4 text-center mb-6">Login</h1>

          <v-text-field
            v-model="email"
            label="Email"
            type="email"
            variant="outlined"
            class="mb-4"
            @keyup.enter="handleLogin"
          ></v-text-field>

          <v-text-field
            v-model="password"
            label="Password"
            type="password"
            variant="outlined"
            class="mb-4"
            @keyup.enter="handleLogin"
          ></v-text-field>

          <v-btn :loading="isLoading" @click="handleLogin" color="primary" size="large" block>
            Login
          </v-btn>

          <v-alert
            v-if="errorMessage"
            type="error"
            class="mt-6"
            closable
            @click:close="errorMessage = ''"
          >
            {{ errorMessage }}
          </v-alert>

          <div class="text-center mt-4">
            Don't have an account? <router-link to="/register">Sign up</router-link>
          </div>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>
