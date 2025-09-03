<template>
  <div class="team-detail">
    <div class="container">
      <div class="back-navigation">
        <button @click="goBack" class="back-btn">
          ← Back to Data Analysis
        </button>
      </div>
      
      <div class="detail-header">
        <h1>Team Detailed Profile</h1>
        <p class="subtitle">Team ID: {{ teamId }}</p>
      </div>
      
      <div class="detail-content">
        <div class="placeholder-card">
          <h2>📊 Team Statistics</h2>
          <p>Detailed team statistics will be displayed here...</p>
        </div>
        
        <div class="placeholder-card">
          <h2>📈 Performance Analysis</h2>
          <p>Performance analysis charts and data will be displayed here...</p>
        </div>
        
        <div class="placeholder-card">
          <h2>🏀 Player Information</h2>
          <p>Player roster and individual statistics will be displayed here...</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { useRoute, useRouter } from 'vue-router'

const route = useRoute()
const router = useRouter()

const teamId = route.params.id

const goBack = () => {
  router.push('/analyst')
}
</script>

<style scoped>
.team-detail {
  min-height: 100vh;
  background-color: #f8f9fa;
  padding: 2rem 0;
}

.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 2rem;
}

.back-navigation {
  margin-bottom: 2rem;
}

.back-btn {
  background: none;
  border: none;
  color: #007bff;
  font-size: 1rem;
  cursor: pointer;
  padding: 0.5rem 0;
  text-decoration: underline;
}

.back-btn:hover {
  color: #0056b3;
}

.detail-header {
  text-align: center;
  margin-bottom: 3rem;
}

.detail-header h1 {
  font-size: 2.5rem;
  color: #333;
  margin-bottom: 0.5rem;
}

.subtitle {
  color: #666;
  font-size: 1.1rem;
}

.detail-content {
  display: grid;
  gap: 2rem;
}

.placeholder-card {
  background: white;
  border-radius: 12px;
  padding: 2rem;
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
  text-align: center;
}

.placeholder-card h2 {
  color: #333;
  margin-bottom: 1rem;
  font-size: 1.5rem;
}

.placeholder-card p {
  color: #666;
  font-size: 1.1rem;
}

@media (max-width: 768px) {
  .container {
    padding: 0 1rem;
  }
  
  .detail-header h1 {
    font-size: 2rem;
  }
  
  .placeholder-card {
    padding: 1.5rem;
  }
}
</style>
