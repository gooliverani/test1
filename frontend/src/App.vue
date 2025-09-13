<template>
  <div id="app">
    <router-view />
  </div>
</template>

<script setup lang="ts">
// Clean root component for our dashboard
</script>

<style>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

body {
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
  background: #f8fafc;
  color: #2d3748;
  line-height: 1.6;
}

#app {
  min-height: 100vh;
}
</style>
