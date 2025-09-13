<template>
  <div style="padding: 20px;">
    <h1>Test Dashboard</h1>
    <p>This is a test to verify the application is loading properly.</p>
    <div v-if="loading">Loading...</div>
    <div v-else>
      <p>Employees loaded: {{ employees.length }}</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'

const loading = ref(true)
const employees = ref([])

const fetchEmployees = async () => {
  try {
    const response = await fetch('http://localhost:5000/api/employees')
    if (response.ok) {
      employees.value = await response.json()
    }
  } catch (error) {
    console.error('Error fetching employees:', error)
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  fetchEmployees()
})
</script>