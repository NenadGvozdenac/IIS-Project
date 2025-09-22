-- -- Minimal, idempotent script to add 5 teams, 10 players per team, and link them via team_member.
-- -- This file intentionally only inserts: teams, players, and team_member rows.

-- -- NOTE: The `player` table requires `id_nationality` and `id_position` (NOT NULL and FK).
-- -- We'll ensure one minimal nationality/position exist (ids chosen small) to satisfy constraints.

-- -- Ensure a minimal nationality exists (id_nationality = 1)
-- INSERT INTO nationality (id_nationality, state)
-- VALUES (1, 'SRB')
-- ON CONFLICT (id_nationality) DO NOTHING;

-- -- Ensure a minimal position exists (id_position = 1)
-- INSERT INTO position (id_position, name)
-- VALUES (1, 'Unknown')
-- ON CONFLICT (id_position) DO NOTHING;

-- -- Insert 5 teams (IDs chosen to avoid common existing IDs; adjust if these collide in your DB)
-- INSERT INTO team (id_team, name, state, city, hall, coach)
-- VALUES
--   (3001, 'KK Novi Grad', 'Srbija', 'Novi Grad', 'Novi Hall', 'Coach A'),
--   (3002, 'KK Srem', 'Srbija', 'Sremska Mitrovica', 'Srem Arena', 'Coach B'),
--   (3003, 'KK Zlatibor', 'Srbija', 'Čajetina', 'Zlatibor Hall', 'Coach C'),
--   (3004, 'KK Vojvodina II', 'Srbija', 'Novi Sad', 'Vojvodina Hall II', 'Coach D'),
--   (3005, 'KK Tamis', 'Srbija', 'Pančevo', 'Tamiš Hall', 'Coach E')
-- ON CONFLICT (id_team) DO NOTHING;

-- -- Insert 10 players per team and link them in team_member.
-- -- We'll create explicit id_player values so linking is deterministic.
-- DO $$
-- DECLARE
--   base integer := 7000; -- base for id_player
--   team_id integer;
--   i integer;
--   pid integer;
-- BEGIN
--   FOR team_id IN 3001..3005 LOOP
--     FOR i IN 1..10 LOOP
--       pid := base + (team_id - 3000) * 10 + i; -- unique id
--       -- insert player (id_nationality and id_position set to 1)
--       INSERT INTO player (id_player, name, surname, birthday, weight, height, id_nationality, id_position)
--       VALUES (pid, 'Player' || (team_id - 3000) || '_' || i, 'Surname' || (team_id - 3000) || '_' || i, NULL, NULL, NULL, 1, 1)
--       ON CONFLICT (id_player) DO NOTHING;

--       -- link player to team
--       INSERT INTO team_member (jersey_number, status, id_player, id_team)
--       VALUES (i, 'active', pid, team_id)
--       ON CONFLICT (id_team, id_player) DO NOTHING;
--     END LOOP;
--   END LOOP;
-- END$$;

-- -- End of minimal insert script
