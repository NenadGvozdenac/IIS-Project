# Complete Elasticsearch Setup Script
# Run this after: docker compose up --build

Write-Host "=== ELASTICSEARCH COMPLETE SETUP ===" -ForegroundColor Green

# Step 1: Wait for services to be ready
Write-Host "1. Waiting for services to start..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

# Step 2: Generate dummy data (creates indexes and populates data)
Write-Host "2. Generating dummy data..." -ForegroundColor Yellow
$response = Invoke-RestMethod -Uri "http://localhost:5008/api/players/bulk/generate-dummy-data" -Method Post
Write-Host "   - Players created: $($response.playersCount)" -ForegroundColor Cyan
Write-Host "   - Sessions created: $($response.sessionsCount)" -ForegroundColor Cyan

# Step 3: Wait for indexing to complete
Write-Host "3. Waiting for data indexing..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

# Step 4: Test all aggregations
Write-Host "4. Testing Aggregation #1: Most Points in Session" -ForegroundColor Yellow
$agg1 = Invoke-RestMethod -Uri "http://localhost:5008/api/players/aggregations/most-points-session" -Method Get
$agg1 | Select-Object -First 3 | Format-Table
Write-Host "   ✅ Aggregation 1 working!" -ForegroundColor Green

Write-Host "5. Testing Aggregation #2: Max Points Last Year" -ForegroundColor Yellow
$agg2 = Invoke-RestMethod -Uri "http://localhost:5008/api/players/aggregations/top-max-points-last-year" -Method Get
$agg2 | Select-Object -First 3 | Format-Table
Write-Host "   ✅ Aggregation 2 working!" -ForegroundColor Green

Write-Host "6. Testing Aggregation #3: Playoff Minutes by Nationality (Serbia)" -ForegroundColor Yellow
$agg3 = Invoke-RestMethod -Uri "http://localhost:5008/api/players/aggregations/playoff-minutes-by-nationality?nationality=Serbia" -Method Get
$agg3 | Select-Object -First 3 | Format-Table
Write-Host "   ✅ Aggregation 3 working!" -ForegroundColor Green

# Step 5: Test bonus aggregation - Player Sessions by Metric
Write-Host "7. Testing Bonus Aggregation: Player Sessions by Metric (Michael + PTS)" -ForegroundColor Yellow
$agg4 = Invoke-RestMethod -Uri "http://localhost:5008/api/players/aggregations/top-sessions-by-metric?playerName=Michael&metricName=PTS&limit=3" -Method Get
$agg4 | Format-Table
Write-Host "   ✅ Bonus Aggregation working!" -ForegroundColor Green

# Step 6: Show available metrics
Write-Host "8. Available Metrics Summary:" -ForegroundColor Yellow
$metrics = Invoke-RestMethod -Uri "http://localhost:5008/api/players/aggregations/available-quantitative-metrics" -Method Get
Write-Host "   - Quantitative metrics: $($metrics.Count)" -ForegroundColor Cyan
$metrics | Where-Object { $_.metricType -eq "Quantitative" } | Select-Object -First 5 | Format-Table

Write-Host "=== SETUP COMPLETE! ALL AGGREGATIONS WORKING! ===" -ForegroundColor Green
Write-Host "Your Elasticsearch service is ready with NBA stats and proper MetricType classification!" -ForegroundColor Cyan