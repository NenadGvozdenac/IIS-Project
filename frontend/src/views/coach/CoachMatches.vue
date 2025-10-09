<template>
  <div class="matches-page">
    <div class="container">
      <div class="page-header">
        <h1>Matches</h1>
        <p class="subtitle">Match management and scheduling</p>
      </div>

      <div class="placeholder-content">
        <div class="placeholder-icon">📅</div>
        <h2>Coming Soon</h2>
        <p>Match scheduling and management features will be available here.</p>
        <p class="secondary-text">This section is currently under development.</p>
      </div>
    </div>
  </div>
</template>

<script setup>
// Placeholder component - to be implemented later
</script>

<style scoped>
.matches-page {
  min-height: 100vh;
  background-color: #f8f9fa;
  padding: 2rem;
}

.container {
  max-width: 1200px;
  margin: 0 auto;
}

.page-header {
  text-align: center;
  margin-bottom: 3rem;
}

.page-header h1 {
  font-size: 2.5rem;
  font-weight: 700;
  color: #2e7d32;
  margin: 0 0 0.5rem 0;
}

.subtitle {
  font-size: 1.1rem;
  color: #666;
  margin: 0;
}

.placeholder-content {
  background: white;
  border: 2px solid #e9ecef;
  border-radius: 12px;
  padding: 4rem 2rem;
  text-align: center;
  max-width: 600px;
  margin: 0 auto;
}

.placeholder-icon {
  font-size: 4rem;
  margin-bottom: 1.5rem;
}

.placeholder-content h2 {
  font-size: 2rem;
  font-weight: 600;
  color: #333;
  margin: 0 0 1rem 0;
}

.placeholder-content p {
  font-size: 1.1rem;
  color: #666;
  margin: 0.5rem 0;
}

.secondary-text {
  font-style: italic;
  color: #999;
  font-size: 1rem;
}

/* Responsive Design */
@media (max-width: 768px) {
  .page-header h1 {
    font-size: 2rem;
  }

  .placeholder-content {
    padding: 3rem 1.5rem;
  }

  .placeholder-icon {
    font-size: 3rem;
  }

  .placeholder-content h2 {
    font-size: 1.5rem;
  }
}
</style>
