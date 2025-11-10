<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const email = ref('')
const password = ref('')
const isLoading = ref(false)
const errorMessage = ref('')
const router = useRouter()

const API_URL = ''
async function handleRegister() {
  isLoading.value = true
  errorMessage.value = ''

  try {
    const response = await fetch(`${API_URL}/register`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        email: email.value,
        password: password.value,
      }),
    })

    if (!response.ok) {
      const errorData = await response.json()
      // Handle Identity's complex errors
      const errorMessages = Object.values(errorData.errors).flat().join(' ')
      throw new Error(errorMessages || 'Registration failed.')
    }

    // On success, redirect to login
    router.push('/login')
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
          <h1 class="text-h4 text-center mb-6">Register</h1>

          <v-text-field
            v-model="email"
            label="Email"
            type="email"
            variant="outlined"
            class="mb-4"
            @keyup.enter="handleRegister"
          ></v-text-field>

          <v-text-field
            v-model="password"
            label="Password"
            type="password"
            variant="outlined"
            class="mb-4"
            @keyup.enter="handleRegister"
          ></v-text-field>

          <v-btn :loading="isLoading" @click="handleRegister" color="primary" size="large" block>
            Create Account
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
            Already have an account? <router-link to="/login">Login</router-link>
          </div>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>
