<template>
  <div class="scout-page">
    <ScoutNavbar />
    <div class="main-content">
      <div class="container">
        <div class="page-header">
          <h1>Sessions</h1>
          <p>Scouting sessions and reports</p>
        </div>
        <div class="content-card">
          <p>Sessions page content will be implemented here.</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import ScoutNavbar from '../../components/scout/ScoutNavbar.vue'
</script>

<style scoped>
.scout-page {
  min-height: 100vh;
  background-color: var(--color-surface);
}

.main-content {
  padding: var(--spacing-xl);
}

.container {
  max-width: 1200px;
  margin: 0 auto;
}

.page-header {
  margin-bottom: var(--spacing-xl);
}

.page-header h1 {
  font-size: 2.5rem;
  color: var(--color-text);
  margin-bottom: var(--spacing-sm);
}

.page-header p {
  color: var(--color-text-light);
  font-size: 1.125rem;
}

.content-card {
  background: white;
  padding: var(--spacing-xl);
  border-radius: var(--border-radius);
  box-shadow: var(--shadow-sm);
}

:root {
  --color-surface: #f8fafc;
  --color-text: #1f2937;
  --color-text-light: #6b7280;
  --shadow-sm: 0 1px 2px 0 rgba(0, 0, 0, 0.05);
  --border-radius: 0.375rem;
  --spacing-sm: 0.5rem;
  --spacing-xl: 1.5rem;
}
</style>
