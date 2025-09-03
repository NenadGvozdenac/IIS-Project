<template>
  <div class="home">
    <!-- Hero Section -->
    <section class="hero">
      <div class="container">
        <div class="hero-content">
          <h1 class="hero-title">Welcome to Sports Hub</h1>
          <p class="hero-subtitle">
            Your ultimate destination for sports events, tickets, and scouting opportunities.
            Connect with the sporting world like never before.
          </p>
          <div class="hero-actions" v-if="!isAuthenticated">
            <router-link to="/register" class="btn btn-primary btn-lg">Get Started</router-link>
            <router-link to="/login" class="btn btn-secondary btn-lg">Sign In</router-link>
          </div>
          <div class="hero-actions" v-else>
            <h2 class="welcome-message">Welcome back, {{ userName }}!</h2>
            <router-link to="/dashboard" class="btn btn-primary btn-lg">Go to Dashboard</router-link>
          </div>
        </div>
      </div>
    </section>

    <!-- Features Section -->
    <section class="features">
      <div class="container">
        <div class="section-header">
          <h2>Why Choose Sports Hub?</h2>
          <p>Everything you need for your sporting journey in one place</p>
        </div>
        
        <div class="features-grid">
          <div class="feature-card">
            <div class="feature-icon">🎫</div>
            <h3>Event Tickets</h3>
            <p>Book tickets for your favorite sports events with ease and security.</p>
          </div>
          
          <div class="feature-card">
            <div class="feature-icon">🔍</div>
            <h3>Scouting Services</h3>
            <p>Professional scouting services to help discover and develop talent.</p>
          </div>
          
          <div class="feature-card">
            <div class="feature-icon">✈️</div>
            <h3>Travel Arrangements</h3>
            <p>Complete travel solutions for teams and individual athletes.</p>
          </div>
          
          <div class="feature-card">
            <div class="feature-icon">🏆</div>
            <h3>Match Management</h3>
            <p>Comprehensive match organization and management services.</p>
          </div>
        </div>
      </div>
    </section>

    <!-- CTA Section -->
    <section class="cta" v-if="!isAuthenticated">
      <div class="container">
        <div class="cta-content">
          <h2>Ready to Get Started?</h2>
          <p>Join thousands of sports enthusiasts and professionals already using Sports Hub.</p>
          <router-link to="/register" class="btn btn-primary btn-lg">Sign Up Today</router-link>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup>

</script>

<style scoped>
.home {
  min-height: calc(100vh - 4rem);
}

/* Hero Section */
.hero {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: var(--spacing-2xl) 0;
  min-height: 500px;
  display: flex;
  align-items: center;
}

.hero-content {
  text-align: center;
  max-width: 800px;
  margin: 0 auto;
}

.hero-title {
  font-size: 3.5rem;
  font-weight: 700;
  margin-bottom: var(--spacing-md);
  color: white;
}

.hero-subtitle {
  font-size: 1.25rem;
  margin-bottom: var(--spacing-2xl);
  color: rgba(255, 255, 255, 0.9);
  line-height: 1.6;
}

.hero-actions {
  display: flex;
  gap: var(--spacing-md);
  justify-content: center;
  flex-wrap: wrap;
  align-items: center;
}

.welcome-message {
  color: white;
  font-size: 2rem;
  margin-bottom: var(--spacing-md);
  width: 100%;
}

.btn-lg {
  padding: var(--spacing-md) var(--spacing-xl);
  font-size: 1rem;
}

.btn-secondary {
  background-color: transparent;
  color: white;
  border: 2px solid white;
}

.btn-secondary:hover {
  background-color: white;
  color: var(--color-primary);
  text-decoration: none;
}

/* Features Section */
.features {
  padding: var(--spacing-2xl) 0;
  background-color: var(--color-surface);
}

.section-header {
  text-align: center;
  margin-bottom: var(--spacing-2xl);
}

.section-header h2 {
  font-size: 2.5rem;
  margin-bottom: var(--spacing-md);
  color: var(--color-text);
}

.section-header p {
  font-size: 1.125rem;
  color: var(--color-text-light);
}

.features-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: var(--spacing-xl);
  margin-top: var(--spacing-2xl);
}

.feature-card {
  background: white;
  padding: var(--spacing-xl);
  border-radius: var(--radius-lg);
  text-align: center;
  box-shadow: var(--shadow-md);
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.feature-card:hover {
  transform: translateY(-4px);
  box-shadow: var(--shadow-lg);
}

.feature-icon {
  font-size: 3rem;
  margin-bottom: var(--spacing-md);
}

.feature-card h3 {
  font-size: 1.5rem;
  margin-bottom: var(--spacing-md);
  color: var(--color-text);
}

.feature-card p {
  color: var(--color-text-light);
  line-height: 1.6;
}

/* CTA Section */
.cta {
  padding: var(--spacing-2xl) 0;
  background-color: var(--color-primary);
  color: white;
}

.cta-content {
  text-align: center;
  max-width: 600px;
  margin: 0 auto;
}

.cta h2 {
  font-size: 2.5rem;
  margin-bottom: var(--spacing-md);
  color: white;
}

.cta p {
  font-size: 1.125rem;
  margin-bottom: var(--spacing-xl);
  color: rgba(255, 255, 255, 0.9);
}

/* Responsive */
@media (max-width: 768px) {
  .hero-title {
    font-size: 2.5rem;
  }
  
  .hero-subtitle {
    font-size: 1.125rem;
  }
  
  .hero-actions {
    flex-direction: column;
    align-items: center;
  }
  
  .btn-lg {
    width: 100%;
    max-width: 300px;
  }
  
  .welcome-message {
    font-size: 1.5rem;
  }
  
  .section-header h2,
  .cta h2 {
    font-size: 2rem;
  }
  
  .features-grid {
    grid-template-columns: 1fr;
  }
}
</style>
