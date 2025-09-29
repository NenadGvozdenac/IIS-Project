#!/bin/bash

# Čeka da Elasticsearch bude dostupan
echo "Čekam da Elasticsearch bude dostupan..."
until curl -s http://elasticsearch:9200/_cluster/health?wait_for_status=yellow&timeout=30s > /dev/null; do
  echo "Elasticsearch još uvek nije spreman..."
  sleep 5
done

echo "Elasticsearch je spreman! Kreiram indekse..."

# Kreiraj players indeks
echo "Kreiram players indeks..."
curl -X PUT "elasticsearch:9200/players" \
  -H "Content-Type: application/json" \
  -d @/elasticsearch/players_index_mapping.json

# Kreiraj sessions indeks
echo "Kreiram sessions indeks..."
curl -X PUT "elasticsearch:9200/sessions" \
  -H "Content-Type: application/json" \
  -d @/elasticsearch/sessions_index_mapping.json

# Kreiraj alias-e za lakše upravljanje
echo "Kreiram alias-e..."
curl -X POST "elasticsearch:9200/_aliases" \
  -H "Content-Type: application/json" \
  -d '{
    "actions": [
      {
        "add": {
          "index": "players",
          "alias": "players_search"
        }
      },
      {
        "add": {
          "index": "sessions",
          "alias": "sessions_search"
        }
      }
    ]
  }'

echo "Indeksi su uspešno kreirani!"

# Proveri da li su indeksi kreirani
echo "Provera kreiranh indeksa:"
curl -X GET "elasticsearch:9200/_cat/indices?v"