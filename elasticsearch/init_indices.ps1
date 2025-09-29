# PowerShell skripta za kreiranje Elasticsearch indeksa

Write-Host "Čekam da Elasticsearch bude dostupan..." -ForegroundColor Yellow

# Čeka da Elasticsearch bude dostupan
do {
    try {
        $response = Invoke-RestMethod -Uri "http://localhost:9200/_cluster/health" -Method GET -TimeoutSec 5
        if ($response.status -eq "yellow" -or $response.status -eq "green") {
            break
        }
    }
    catch {
        Write-Host "Elasticsearch još uvek nije spreman..." -ForegroundColor Yellow
        Start-Sleep -Seconds 5
    }
} while ($true)

Write-Host "Elasticsearch je spreman! Kreiram indekse..." -ForegroundColor Green

# Kreiraj players indeks
Write-Host "Kreiram players indeks..." -ForegroundColor Blue
try {
    $playersMapping = Get-Content -Path ".\elasticsearch\players_index_mapping.json" -Raw
    $response = Invoke-RestMethod -Uri "http://localhost:9200/players" -Method PUT -Body $playersMapping -ContentType "application/json"
    Write-Host "Players indeks kreiran uspešno!" -ForegroundColor Green
}
catch {
    Write-Host "Greška pri kreiranju players indeksa: $($_.Exception.Message)" -ForegroundColor Red
}

# Kreiraj sessions indeks
Write-Host "Kreiram sessions indeks..." -ForegroundColor Blue
try {
    $sessionsMapping = Get-Content -Path ".\elasticsearch\sessions_index_mapping.json" -Raw
    $response = Invoke-RestMethod -Uri "http://localhost:9200/sessions" -Method PUT -Body $sessionsMapping -ContentType "application/json"
    Write-Host "Sessions indeks kreiran uspešno!" -ForegroundColor Green
}
catch {
    Write-Host "Greška pri kreiranju sessions indeksa: $($_.Exception.Message)" -ForegroundColor Red
}

# Kreiraj alias-e
Write-Host "Kreiram alias-e..." -ForegroundColor Blue
try {
    $aliasesBody = @{
        actions = @(
            @{
                add = @{
                    index = "players"
                    alias = "players_search"
                }
            },
            @{
                add = @{
                    index = "sessions"
                    alias = "sessions_search"
                }
            }
        )
    } | ConvertTo-Json -Depth 10

    $response = Invoke-RestMethod -Uri "http://localhost:9200/_aliases" -Method POST -Body $aliasesBody -ContentType "application/json"
    Write-Host "Alias-i kreirani uspešno!" -ForegroundColor Green
}
catch {
    Write-Host "Greška pri kreiranju alias-a: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "Indeksi su uspešno kreirani!" -ForegroundColor Green

# Proveri da li su indeksi kreirani
Write-Host "Provera kreiranh indeksa:" -ForegroundColor Blue
try {
    $indices = Invoke-RestMethod -Uri "http://localhost:9200/_cat/indices?v&format=json" -Method GET
    $indices | Format-Table -Property index, health, status, "docs.count", "store.size"
}
catch {
    Write-Host "Greška pri proveri indeksa: $($_.Exception.Message)" -ForegroundColor Red
}