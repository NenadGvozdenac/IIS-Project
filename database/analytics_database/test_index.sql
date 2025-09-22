--BEZ INDEKSA
docker exec -i iis-project-postgres_db-1 psql -U postgres -d sportsdb -c "SET enable_indexscan = off; SET enable_bitmapscan = off; EXPLAIN ANALYZE SELECT * FROM personal_event WHERE id_player = 2; SET enable_indexscan = on; SET enable_bitmapscan = on;"

--SA INDEKSOM
docker exec -i iis-project-postgres_db-1 psql -U postgres -d sportsdb -c "EXPLAIN ANALYZE SELECT * FROM personal_event WHERE id_player = 2;"