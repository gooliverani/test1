<template>
  <div class="pa-4">
    <h1>Profiles</h1>
    <v-btn color="primary" @click="fetchProfiles">Refresh</v-btn>
    <v-table v-if="profiles.length">
      <thead>
        <tr>
          <th>Name</th>
          <th>Email</th>
          <th>Type</th>
          <th>Status</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="p in profiles" :key="p.id">
          <td>{{ p.displayName }}</td>
          <td>{{ p.email }}</td>
          <td>{{ p.type }}</td>
          <td>{{ p.status }}</td>
        </tr>
      </tbody>
    </v-table>
    <div v-else>No profiles found.</div>
  </div>
</template>
<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { listProfiles, Profile } from '../profilesApi';

const profiles = ref<Profile[]>([]);

async function fetchProfiles() {
  profiles.value = await listProfiles();
}

onMounted(fetchProfiles);
</script>
