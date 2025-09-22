-- Match Analytics Data with Events
-- This script creates 10 matches with detailed tracking and events

-- First, let's insert some basic data that might be missing

-- Insert nationalities if not exists
INSERT INTO nationality (state) VALUES 
('Serbia'), ('Croatia'), ('Slovenia'), ('Montenegro'), ('Bosnia and Herzegovina'), 
('North Macedonia'), ('Bulgaria'), ('Greece'), ('Turkey'), ('Spain')
ON CONFLICT DO NOTHING;

-- Insert positions if not exists
INSERT INTO position (name) VALUES 
('Point Guard'), ('Shooting Guard'), ('Small Forward'), ('Power Forward'), ('Center')
ON CONFLICT DO NOTHING;

-- Insert competition if not exists
INSERT INTO competition (name, started_at, ended_at, number_of_matches) VALUES 
('ABA Liga 2024/25', '2024-09-15', '2025-05-30', 30)
ON CONFLICT DO NOTHING;

-- Insert season if not exists
INSERT INTO season (name, started_at, ended_at, tickets_for_sale, tickets_went_on_sale) VALUES 
('2024/25', '2024-09-01', '2025-05-31', true, '2024-08-15 10:00:00+00')
ON CONFLICT DO NOTHING;

-- Create 5 new teams
INSERT INTO team (name, state, city, hall, founded_date, coach, playing_style, key_strengths, key_weaknesses) VALUES 
('Cedevita Olimpija', 'Slovenia', 'Ljubljana', 'Stožice Arena', '1946-01-01', 'Jurica Golemac', 'Fast Break', 'Athletic team, good transition', 'Inconsistent shooting'),
('Budućnost VOLI', 'Montenegro', 'Podgorica', 'Morača Sports Center', '1949-01-01', 'Andrej Žakelj', 'Half-court', 'Strong defense, rebounding', 'Limited depth'),
('Igokea m:tel', 'Bosnia and Herzegovina', 'Laktaši', 'Laktaši Sports Hall', '1947-01-01', 'Dragan Bajić', 'Balanced', 'Team chemistry', 'Lack of star players'),
('Krka Novo mesto', 'Slovenia', 'Novo mesto', 'Leon Štukelj Hall', '1948-01-01', 'Dalibor Damjanović', 'Motion Offense', 'Good ball movement', 'Size disadvantage'),
('Split', 'Croatia', 'Split', 'Gripe Hall', '1945-01-01', 'Slaven Rimac', 'Defensive', 'Tough mentality', 'Offensive struggles');

-- Get team IDs for reference
-- Assuming the teams will get IDs starting from the next available ID

-- Create players for each team (12 players per team)
-- Team 1: Cedevita Olimpija
INSERT INTO player (name, surname, birthday, weight, height, id_nationality, id_position) VALUES 
-- Point Guards
('Aleksej', 'Nikolić', '1995-03-15', 85, 185, (SELECT id_nationality FROM nationality WHERE state = 'Slovenia'), (SELECT id_position FROM position WHERE name = 'Point Guard')),
('Jaka', 'Blažič', '1990-07-22', 88, 190, (SELECT id_nationality FROM nationality WHERE state = 'Slovenia'), (SELECT id_position FROM position WHERE name = 'Shooting Guard')),
-- Shooting Guards
('Edo', 'Murić', '1991-09-08', 92, 193, (SELECT id_nationality FROM nationality WHERE state = 'Slovenia'), (SELECT id_position FROM position WHERE name = 'Shooting Guard')),
('Žiga', 'Dimec', '1998-04-12', 87, 188, (SELECT id_nationality FROM nationality WHERE state = 'Slovenia'), (SELECT id_position FROM position WHERE name = 'Shooting Guard')),
-- Small Forwards
('Yogi', 'Ferrell', '1993-05-09', 82, 180, (SELECT id_nationality FROM nationality WHERE state = 'Serbia'), (SELECT id_position FROM position WHERE name = 'Small Forward')),
('Devin', 'Oliver', '1991-12-03', 95, 200, (SELECT id_nationality FROM nationality WHERE state = 'Serbia'), (SELECT id_position FROM position WHERE name = 'Small Forward')),
-- Power Forwards
('Martins', 'Meiers', '1991-02-18', 102, 205, (SELECT id_nationality FROM nationality WHERE state = 'Slovenia'), (SELECT id_position FROM position WHERE name = 'Power Forward')),
('Alen', 'Omić', '1989-05-17', 115, 208, (SELECT id_nationality FROM nationality WHERE state = 'Slovenia'), (SELECT id_position FROM position WHERE name = 'Power Forward')),
-- Centers
('Marko', 'Radovanović', '1992-08-25', 108, 210, (SELECT id_nationality FROM nationality WHERE state = 'Serbia'), (SELECT id_position FROM position WHERE name = 'Center')),
('Žan Mark', 'Šiško', '1996-11-14', 95, 201, (SELECT id_nationality FROM nationality WHERE state = 'Slovenia'), (SELECT id_position FROM position WHERE name = 'Center')),
('Miha', 'Lapornik', '1999-01-30', 88, 195, (SELECT id_nationality FROM nationality WHERE state = 'Slovenia'), (SELECT id_position FROM position WHERE name = 'Small Forward')),
('Gregor', 'Glas', '1997-06-18', 92, 198, (SELECT id_nationality FROM nationality WHERE state = 'Slovenia'), (SELECT id_position FROM position WHERE name = 'Shooting Guard'));

-- Team 2: Budućnost VOLI
INSERT INTO player (name, surname, birthday, weight, height, id_nationality, id_position) VALUES 
('Nikola', 'Ivanović', '1994-02-10', 90, 192, (SELECT id_nationality FROM nationality WHERE state = 'Montenegro'), (SELECT id_position FROM position WHERE name = 'Point Guard')),
('Marko', 'Jeremić', '1996-08-30', 85, 187, (SELECT id_nationality FROM nationality WHERE state = 'Montenegro'), (SELECT id_position FROM position WHERE name = 'Point Guard')),
('Fletcher', 'Magee', '1993-03-07', 88, 190, (SELECT id_nationality FROM nationality WHERE state = 'Serbia'), (SELECT id_position FROM position WHERE name = 'Shooting Guard')),
('McKenzie', 'Moore', '1995-11-22', 93, 196, (SELECT id_nationality FROM nationality WHERE state = 'Serbia'), (SELECT id_position FROM position WHERE name = 'Shooting Guard')),
('Kenan', 'Kamenjaš', '1992-04-15', 98, 203, (SELECT id_nationality FROM nationality WHERE state = 'Montenegro'), (SELECT id_position FROM position WHERE name = 'Small Forward')),
('Justin', 'Cobbs', '1991-07-08', 91, 194, (SELECT id_nationality FROM nationality WHERE state = 'Montenegro'), (SELECT id_position FROM position WHERE name = 'Small Forward')),
('Yoan', 'Makoundou', '1998-01-25', 105, 206, (SELECT id_nationality FROM nationality WHERE state = 'Montenegro'), (SELECT id_position FROM position WHERE name = 'Power Forward')),
('Aleksandar', 'Balić', '1990-12-12', 110, 210, (SELECT id_nationality FROM nationality WHERE state = 'Montenegro'), (SELECT id_position FROM position WHERE name = 'Power Forward')),
('Ivan', 'Buva', '1989-09-03', 112, 212, (SELECT id_nationality FROM nationality WHERE state = 'Montenegro'), (SELECT id_position FROM position WHERE name = 'Center')),
('Petar', 'Popović', '1997-05-20', 107, 208, (SELECT id_nationality FROM nationality WHERE state = 'Montenegro'), (SELECT id_position FROM position WHERE name = 'Center')),
('Miloš', 'Popović', '1999-03-14', 95, 199, (SELECT id_nationality FROM nationality WHERE state = 'Montenegro'), (SELECT id_position FROM position WHERE name = 'Small Forward')),
('Stefan', 'Pot', '1998-10-05', 89, 191, (SELECT id_nationality FROM nationality WHERE state = 'Montenegro'), (SELECT id_position FROM position WHERE name = 'Shooting Guard'));

-- Team 3: Igokea m:tel
INSERT INTO player (name, surname, birthday, weight, height, id_nationality, id_position) VALUES 
('Nemanja', 'Gordić', '1993-06-18', 87, 189, (SELECT id_nationality FROM nationality WHERE state = 'Bosnia and Herzegovina'), (SELECT id_position FROM position WHERE name = 'Point Guard')),
('Marko', 'Lukić', '1995-04-02', 84, 186, (SELECT id_nationality FROM nationality WHERE state = 'Bosnia and Herzegovina'), (SELECT id_position FROM position WHERE name = 'Point Guard')),
('Darko', 'Talić', '1992-12-25', 91, 193, (SELECT id_nationality FROM nationality WHERE state = 'Bosnia and Herzegovina'), (SELECT id_position FROM position WHERE name = 'Shooting Guard')),
('Bogdan', 'Radošević', '1996-07-14', 89, 191, (SELECT id_nationality FROM nationality WHERE state = 'Bosnia and Herzegovina'), (SELECT id_position FROM position WHERE name = 'Shooting Guard')),
('Stefan', 'Sinovec', '1994-09-11', 96, 201, (SELECT id_nationality FROM nationality WHERE state = 'Bosnia and Herzegovina'), (SELECT id_position FROM position WHERE name = 'Small Forward')),
('Marko', 'Jagodić Kuridža', '1991-01-28', 99, 204, (SELECT id_nationality FROM nationality WHERE state = 'Bosnia and Herzegovina'), (SELECT id_position FROM position WHERE name = 'Small Forward')),
('Mihailo', 'Jovičić', '1993-08-16', 103, 207, (SELECT id_nationality FROM nationality WHERE state = 'Bosnia and Herzegovina'), (SELECT id_position FROM position WHERE name = 'Power Forward')),
('Luka', 'Asceric', '1995-11-09', 108, 209, (SELECT id_nationality FROM nationality WHERE state = 'Bosnia and Herzegovina'), (SELECT id_position FROM position WHERE name = 'Power Forward')),
('Vladimir', 'Mihailović', '1990-02-22', 114, 213, (SELECT id_nationality FROM nationality WHERE state = 'Bosnia and Herzegovina'), (SELECT id_position FROM position WHERE name = 'Center')),
('Novica', 'Veličković', '1987-10-07', 111, 211, (SELECT id_nationality FROM nationality WHERE state = 'Bosnia and Herzegovina'), (SELECT id_position FROM position WHERE name = 'Center')),
('Ognjen', 'Jaramaz', '1995-05-31', 93, 197, (SELECT id_nationality FROM nationality WHERE state = 'Bosnia and Herzegovina'), (SELECT id_position FROM position WHERE name = 'Small Forward')),
('Miloš', 'Glišić', '1997-12-19', 86, 188, (SELECT id_nationality FROM nationality WHERE state = 'Bosnia and Herzegovina'), (SELECT id_position FROM position WHERE name = 'Shooting Guard'));

-- Team 4: Krka Novo mesto
INSERT INTO player (name, surname, birthday, weight, height, id_nationality, id_position) VALUES 
('Tibor', 'Mirtič', '1994-03-08', 88, 190, (SELECT id_nationality FROM nationality WHERE state = 'Slovenia'), (SELECT id_position FROM position WHERE name = 'Point Guard')),
('Luka', 'Božič', '1996-09-21', 86, 188, (SELECT id_nationality FROM nationality WHERE state = 'Slovenia'), (SELECT id_position FROM position WHERE name = 'Point Guard')),
('Jan', 'Špan', '1993-01-17', 92, 194, (SELECT id_nationality FROM nationality WHERE state = 'Slovenia'), (SELECT id_position FROM position WHERE name = 'Shooting Guard')),
('Tilen', 'Klemenčič', '1995-06-03', 90, 192, (SELECT id_nationality FROM nationality WHERE state = 'Slovenia'), (SELECT id_position FROM position WHERE name = 'Shooting Guard')),
('Leon', 'Stergar', '1992-11-26', 97, 202, (SELECT id_nationality FROM nationality WHERE state = 'Slovenia'), (SELECT id_position FROM position WHERE name = 'Small Forward')),
('Matej', 'Rudan', '1994-08-15', 100, 205, (SELECT id_nationality FROM nationality WHERE state = 'Slovenia'), (SELECT id_position FROM position WHERE name = 'Small Forward')),
('Luka', 'Lapornik', '1991-04-12', 104, 208, (SELECT id_nationality FROM nationality WHERE state = 'Slovenia'), (SELECT id_position FROM position WHERE name = 'Power Forward')),
('Miha', 'Cerkvenik', '1993-12-05', 106, 209, (SELECT id_nationality FROM nationality WHERE state = 'Slovenia'), (SELECT id_position FROM position WHERE name = 'Power Forward')),
('Rok', 'Radović', '1989-07-19', 113, 212, (SELECT id_nationality FROM nationality WHERE state = 'Slovenia'), (SELECT id_position FROM position WHERE name = 'Center')),
('Tadej', 'Ferme', '1995-02-28', 109, 210, (SELECT id_nationality FROM nationality WHERE state = 'Slovenia'), (SELECT id_position FROM position WHERE name = 'Center')),
('Gašper', 'Vidmar', '1997-10-14', 94, 198, (SELECT id_nationality FROM nationality WHERE state = 'Slovenia'), (SELECT id_position FROM position WHERE name = 'Small Forward')),
('Žiga', 'Fifolt', '1998-05-27', 87, 189, (SELECT id_nationality FROM nationality WHERE state = 'Slovenia'), (SELECT id_position FROM position WHERE name = 'Shooting Guard'));

-- Team 5: Split
INSERT INTO player (name, surname, birthday, weight, height, id_nationality, id_position) VALUES 
('Roko', 'Prkačin', '1993-08-11', 89, 191, (SELECT id_nationality FROM nationality WHERE state = 'Croatia'), (SELECT id_position FROM position WHERE name = 'Point Guard')),
('Josip', 'Bilinovac', '1995-01-24', 87, 189, (SELECT id_nationality FROM nationality WHERE state = 'Croatia'), (SELECT id_position FROM position WHERE name = 'Point Guard')),
('Toni', 'Nakić', '1994-06-07', 93, 195, (SELECT id_nationality FROM nationality WHERE state = 'Croatia'), (SELECT id_position FROM position WHERE name = 'Shooting Guard')),
('Dario', 'Drežnjak', '1992-03-30', 91, 193, (SELECT id_nationality FROM nationality WHERE state = 'Croatia'), (SELECT id_position FROM position WHERE name = 'Shooting Guard')),
('Marino', 'Marić', '1991-09-13', 98, 203, (SELECT id_nationality FROM nationality WHERE state = 'Croatia'), (SELECT id_position FROM position WHERE name = 'Small Forward')),
('Filip', 'Krušlin', '1993-11-26', 101, 206, (SELECT id_nationality FROM nationality WHERE state = 'Croatia'), (SELECT id_position FROM position WHERE name = 'Small Forward')),
('Andrija', 'Stipanović', '1990-04-19', 105, 208, (SELECT id_nationality FROM nationality WHERE state = 'Croatia'), (SELECT id_position FROM position WHERE name = 'Power Forward')),
('Luka', 'Božić', '1988-12-08', 107, 210, (SELECT id_nationality FROM nationality WHERE state = 'Croatia'), (SELECT id_position FROM position WHERE name = 'Power Forward')),
('Marin', 'Rozić', '1989-08-02', 115, 214, (SELECT id_nationality FROM nationality WHERE state = 'Croatia'), (SELECT id_position FROM position WHERE name = 'Center')),
('Tomislav', 'Zubčić', '1990-05-16', 112, 211, (SELECT id_nationality FROM nationality WHERE state = 'Croatia'), (SELECT id_position FROM position WHERE name = 'Center')),
('Mateo', 'Drežnjak', '1996-07-29', 95, 199, (SELECT id_nationality FROM nationality WHERE state = 'Croatia'), (SELECT id_position FROM position WHERE name = 'Small Forward')),
('Ivan', 'Ramljak', '1997-02-11', 88, 190, (SELECT id_nationality FROM nationality WHERE state = 'Croatia'), (SELECT id_position FROM position WHERE name = 'Shooting Guard'));

-- Now create team_member relationships
-- Team 1: Cedevita Olimpija (assuming team_id will be the next available after existing teams)
DO $$
DECLARE 
    olimpija_id INTEGER;
    buducnost_id INTEGER;
    igokea_id INTEGER;
    krka_id INTEGER;
    split_id INTEGER;
    player_counter INTEGER;
BEGIN
    -- Get team IDs
    SELECT id_team INTO olimpija_id FROM team WHERE name = 'Cedevita Olimpija';
    SELECT id_team INTO buducnost_id FROM team WHERE name = 'Budućnost VOLI';
    SELECT id_team INTO igokea_id FROM team WHERE name = 'Igokea m:tel';
    SELECT id_team INTO krka_id FROM team WHERE name = 'Krka Novo mesto';
    SELECT id_team INTO split_id FROM team WHERE name = 'Split';
    
    -- Get the starting player ID for team assignments
    SELECT MIN(p.id_player) INTO player_counter 
    FROM player p 
    JOIN nationality n ON p.id_nationality = n.id_nationality 
    WHERE n.state = 'Slovenia' 
    AND p.name = 'Aleksej' AND p.surname = 'Nikolić';
    
    -- Team 1: Cedevita Olimpija (12 players)
    FOR i IN 0..11 LOOP
        INSERT INTO team_member (jersey_number, status, id_player, id_team) 
        VALUES (i + 4, 'active', player_counter + i, olimpija_id);
    END LOOP;
    
    -- Team 2: Budućnost VOLI (12 players)
    FOR i IN 0..11 LOOP
        INSERT INTO team_member (jersey_number, status, id_player, id_team) 
        VALUES (i + 4, 'active', player_counter + 12 + i, buducnost_id);
    END LOOP;
    
    -- Team 3: Igokea m:tel (12 players)
    FOR i IN 0..11 LOOP
        INSERT INTO team_member (jersey_number, status, id_player, id_team) 
        VALUES (i + 4, 'active', player_counter + 24 + i, igokea_id);
    END LOOP;
    
    -- Team 4: Krka Novo mesto (12 players)
    FOR i IN 0..11 LOOP
        INSERT INTO team_member (jersey_number, status, id_player, id_team) 
        VALUES (i + 4, 'active', player_counter + 36 + i, krka_id);
    END LOOP;
    
    -- Team 5: Split (12 players)
    FOR i IN 0..11 LOOP
        INSERT INTO team_member (jersey_number, status, id_player, id_team) 
        VALUES (i + 4, 'active', player_counter + 48 + i, split_id);
    END LOOP;
END $$;

-- Insert users for match tracking (assuming some exist, we'll create some if needed)
INSERT INTO users (name, surname, email, phone, password, type) VALUES 
('Ana', 'Petrović', 'ana.petrovic@klub.rs', '+381601234567', 'hashed_password_1', 'analyst'),
('Marko', 'Janković', 'marko.jankovic@klub.rs', '+381601234568', 'hashed_password_2', 'scouting manager'),
('Stefan', 'Milenković', 'stefan.milenkovic@klub.rs', '+381601234569', 'hashed_password_3', 'team manager')
ON CONFLICT (email) DO NOTHING;

-- Create 10 matches against different opponents
DO $$
DECLARE 
    season_id INTEGER;
    competition_id INTEGER;
    olimpija_id INTEGER;
    buducnost_id INTEGER;
    igokea_id INTEGER;
    krka_id INTEGER;
    split_id INTEGER;
    home_team_id INTEGER := 1; -- Assuming our main team has ID 1
    analyst_user_id INTEGER;
BEGIN
    -- Get IDs we need
    SELECT id_season INTO season_id FROM season WHERE name = '2024/25';
    SELECT id_competition INTO competition_id FROM competition WHERE name = 'ABA Liga 2024/25';
    SELECT id_team INTO olimpija_id FROM team WHERE name = 'Cedevita Olimpija';
    SELECT id_team INTO buducnost_id FROM team WHERE name = 'Budućnost VOLI';
    SELECT id_team INTO igokea_id FROM team WHERE name = 'Igokea m:tel';
    SELECT id_team INTO krka_id FROM team WHERE name = 'Krka Novo mesto';
    SELECT id_team INTO split_id FROM team WHERE name = 'Split';
    SELECT id_user INTO analyst_user_id FROM users WHERE type = 'analyst' LIMIT 1;
    
    -- Match 1: vs Cedevita Olimpija (Home)
    INSERT INTO match (name, scheduled_at, type, state, city, hall, is_in_our_hall, tickets_for_sale, 
                      tickets_went_on_sale, transportation_required, accommodation_required, 
                      id_competition, id_season, id_team) 
    VALUES ('Home vs Cedevita Olimpija', '2024-10-05 19:00:00+00', 'home', 'finished', 
            'Belgrade', 'Aleksandar Nikolić Hall', true, true, '2024-09-20 10:00:00+00', 
            false, false, competition_id, season_id, olimpija_id);
    
    -- Match 2: vs Budućnost VOLI (Away)
    INSERT INTO match (name, scheduled_at, type, state, city, hall, is_in_our_hall, tickets_for_sale, 
                      tickets_went_on_sale, transportation_required, accommodation_required, 
                      id_competition, id_season, id_team) 
    VALUES ('Away vs Budućnost VOLI', '2024-10-12 20:00:00+00', 'away', 'finished', 
            'Podgorica', 'Morača Sports Center', false, false, NULL, 
            true, true, competition_id, season_id, buducnost_id);
    
    -- Match 3: vs Igokea m:tel (Home)
    INSERT INTO match (name, scheduled_at, type, state, city, hall, is_in_our_hall, tickets_for_sale, 
                      tickets_went_on_sale, transportation_required, accommodation_required, 
                      id_competition, id_season, id_team) 
    VALUES ('Home vs Igokea m:tel', '2024-10-19 18:30:00+00', 'home', 'finished', 
            'Belgrade', 'Aleksandar Nikolić Hall', true, true, '2024-10-05 10:00:00+00', 
            false, false, competition_id, season_id, igokea_id);
    
    -- Match 4: vs Krka Novo mesto (Away)
    INSERT INTO match (name, scheduled_at, type, state, city, hall, is_in_our_hall, tickets_for_sale, 
                      tickets_went_on_sale, transportation_required, accommodation_required, 
                      id_competition, id_season, id_team) 
    VALUES ('Away vs Krka Novo mesto', '2024-10-26 19:30:00+00', 'away', 'finished', 
            'Novo mesto', 'Leon Štukelj Hall', false, false, NULL, 
            true, true, competition_id, season_id, krka_id);
    
    -- Match 5: vs Split (Home)
    INSERT INTO match (name, scheduled_at, type, state, city, hall, is_in_our_hall, tickets_for_sale, 
                      tickets_went_on_sale, transportation_required, accommodation_required, 
                      id_competition, id_season, id_team) 
    VALUES ('Home vs Split', '2024-11-02 17:00:00+00', 'home', 'finished', 
            'Belgrade', 'Aleksandar Nikolić Hall', true, true, '2024-10-19 10:00:00+00', 
            false, false, competition_id, season_id, split_id);
    
    -- Match 6: vs Cedevita Olimpija (Away)
    INSERT INTO match (name, scheduled_at, type, state, city, hall, is_in_our_hall, tickets_for_sale, 
                      tickets_went_on_sale, transportation_required, accommodation_required, 
                      id_competition, id_season, id_team) 
    VALUES ('Away vs Cedevita Olimpija', '2024-11-09 20:30:00+00', 'away', 'finished', 
            'Ljubljana', 'Stožice Arena', false, false, NULL, 
            true, true, competition_id, season_id, olimpija_id);
    
    -- Match 7: vs Budućnost VOLI (Home)
    INSERT INTO match (name, scheduled_at, type, state, city, hall, is_in_our_hall, tickets_for_sale, 
                      tickets_went_on_sale, transportation_required, accommodation_required, 
                      id_competition, id_season, id_team) 
    VALUES ('Home vs Budućnost VOLI', '2024-11-16 19:00:00+00', 'home', 'finished', 
            'Belgrade', 'Aleksandar Nikolić Hall', true, true, '2024-11-02 10:00:00+00', 
            false, false, competition_id, season_id, buducnost_id);
    
    -- Match 8: vs Igokea m:tel (Away)
    INSERT INTO match (name, scheduled_at, type, state, city, hall, is_in_our_hall, tickets_for_sale, 
                      tickets_went_on_sale, transportation_required, accommodation_required, 
                      id_competition, id_season, id_team) 
    VALUES ('Away vs Igokea m:tel', '2024-11-23 18:00:00+00', 'away', 'finished', 
            'Laktaši', 'Laktaši Sports Hall', false, false, NULL, 
            true, true, competition_id, season_id, igokea_id);
    
    -- Match 9: vs Krka Novo mesto (Home)
    INSERT INTO match (name, scheduled_at, type, state, city, hall, is_in_our_hall, tickets_for_sale, 
                      tickets_went_on_sale, transportation_required, accommodation_required, 
                      id_competition, id_season, id_team) 
    VALUES ('Home vs Krka Novo mesto', '2024-11-30 16:30:00+00', 'home', 'finished', 
            'Belgrade', 'Aleksandar Nikolić Hall', true, true, '2024-11-16 10:00:00+00', 
            false, false, competition_id, season_id, krka_id);
    
    -- Match 10: vs Split (Away)
    INSERT INTO match (name, scheduled_at, type, state, city, hall, is_in_our_hall, tickets_for_sale, 
                      tickets_went_on_sale, transportation_required, accommodation_required, 
                      id_competition, id_season, id_team) 
    VALUES ('Away vs Split', '2024-12-07 20:00:00+00', 'away', 'finished', 
            'Split', 'Gripe Hall', false, false, NULL, 
            true, true, competition_id, season_id, split_id);

END $$;

-- Create match_tracking entries for all 10 matches
DO $$
DECLARE 
    match_rec RECORD;
    analyst_user_id INTEGER;
    random_our_score INTEGER;
    random_opp_score INTEGER;
BEGIN
    -- Get analyst user ID
    SELECT id_user INTO analyst_user_id FROM users WHERE type = 'analyst' LIMIT 1;
    
    -- Create match tracking for each match
    FOR match_rec IN 
        SELECT id_match, scheduled_at 
        FROM match 
        WHERE name LIKE '%vs %' 
        ORDER BY id_match DESC 
        LIMIT 10
    LOOP
        -- Generate random but realistic scores
        random_our_score := 75 + floor(random() * 40); -- 75-115 points
        random_opp_score := 70 + floor(random() * 40); -- 70-110 points
        
        INSERT INTO match_tracking (
            start_time, end_time, tracking_status, period_duration, current_period, 
            period_status, period_start_time, elapsed_period_time, 
            last_pause_start_time, total_pause_time_in_period, last_update_time,
            our_points, opponent_points, id_user, id_match
        ) VALUES (
            match_rec.scheduled_at,
            match_rec.scheduled_at + INTERVAL '2 hours 15 minutes',
            'finished',
            600, -- 10 minutes per period
            'end',
            'finished',
            match_rec.scheduled_at + INTERVAL '2 hours',
            600,
            match_rec.scheduled_at + INTERVAL '1 hour 55 minutes',
            45, -- total pause time in last period
            match_rec.scheduled_at + INTERVAL '2 hours 15 minutes',
            random_our_score,
            random_opp_score,
            analyst_user_id,
            match_rec.id_match
        );
    END LOOP;
END $$;

-- Create team_member_match entries for player participation
DO $$
DECLARE 
    match_rec RECORD;
    home_team_id INTEGER := 1; -- Assuming our main team has ID 1
    player_rec RECORD;
    counter INTEGER;
BEGIN
    -- Create team_member_match for each match
    FOR match_rec IN 
        SELECT id_match, id_team 
        FROM match 
        WHERE name LIKE '%vs %' 
        ORDER BY id_match DESC 
        LIMIT 10
    LOOP
        -- Add our team players (assuming team ID 1)
        counter := 0;
        FOR player_rec IN 
            SELECT id_player 
            FROM team_member 
            WHERE id_team = home_team_id 
            ORDER BY jersey_number 
            LIMIT 12
        LOOP
            INSERT INTO team_member_match (starting_lineup, in_game, id_team, id_player, id_match)
            VALUES (
                CASE WHEN counter < 5 THEN true ELSE false END, -- First 5 are starters
                true, -- All players participate
                home_team_id,
                player_rec.id_player,
                match_rec.id_match
            );
            counter := counter + 1;
        END LOOP;
        
        -- Add opponent team players
        counter := 0;
        FOR player_rec IN 
            SELECT id_player 
            FROM team_member 
            WHERE id_team = match_rec.id_team 
            ORDER BY jersey_number 
            LIMIT 12
        LOOP
            INSERT INTO team_member_match (starting_lineup, in_game, id_team, id_player, id_match)
            VALUES (
                CASE WHEN counter < 5 THEN true ELSE false END, -- First 5 are starters
                true, -- All players participate
                match_rec.id_team,
                player_rec.id_player,
                match_rec.id_match
            );
            counter := counter + 1;
        END LOOP;
    END LOOP;
END $$;

-- Generate events for each match (approximately 100 events per match)
DO $$
DECLARE 
    match_rec RECORD;
    i INTEGER;
    j INTEGER;
    period_num VARCHAR(20);
    period_time INTEGER;
    event_type VARCHAR(20);
    player_id INTEGER;
    team_id INTEGER;
    event_time TIMESTAMP WITH TIME ZONE;
    home_team_id INTEGER := 1; -- Assuming our main team has ID 1
    events_generated INTEGER;
    random_val NUMERIC;
    
    -- Event type arrays for random selection
    personal_events VARCHAR(20)[] := ARRAY['+2p', '+3p', '+ft', '2p', '3p', 'assist', 'block', 'foul', 'ft', 'reb def', 'reb of', 'steal', 'substitution in', 'substitution out'];
    team_events VARCHAR(20)[] := ARRAY['timeout', 'technical foul', 'formation', 'defense'];
    general_events VARCHAR(20)[] := ARRAY['period start', 'period end', 'pause start', 'pause end', 'break start', 'break end'];
BEGIN
    -- Generate events for each match
    FOR match_rec IN 
        SELECT id_match, id_team, scheduled_at 
        FROM match 
        WHERE name LIKE '%vs %' 
        ORDER BY id_match DESC 
        LIMIT 10
    LOOP
        events_generated := 0;
        
        -- Generate events for each period (4 periods)
        FOR j IN 1..4 LOOP
            period_num := j::VARCHAR(20);
            
            -- Period start event
            INSERT INTO general_event (creation_time, notes, type, period, period_time, id_match)
            VALUES (
                match_rec.scheduled_at + INTERVAL '15 minutes' * (j-1),
                'Period ' || j || ' started',
                'period start',
                period_num,
                0,
                match_rec.id_match
            );
            events_generated := events_generated + 1;
            
            -- Generate 20-25 events per period
            FOR i IN 1..(20 + floor(random() * 6)) LOOP
                period_time := floor(random() * 600); -- 0-600 seconds (10 minutes)
                event_time := match_rec.scheduled_at + INTERVAL '15 minutes' * (j-1) + INTERVAL '1 second' * period_time;
                random_val := random();
                
                -- 70% personal events, 20% team events, 10% general events
                IF random_val < 0.7 THEN
                    -- Personal event
                    event_type := personal_events[1 + floor(random() * array_length(personal_events, 1))];
                    
                    -- Choose random team (50% our team, 50% opponent)
                    IF random() < 0.5 THEN
                        team_id := home_team_id;
                    ELSE
                        team_id := match_rec.id_team;
                    END IF;
                    
                    -- Get random player from the team
                    SELECT tm.id_player INTO player_id 
                    FROM team_member_match tm 
                    WHERE tm.id_match = match_rec.id_match 
                      AND tm.id_team = team_id 
                      AND tm.in_game = true 
                    ORDER BY random() 
                    LIMIT 1;
                    
                    IF player_id IS NOT NULL THEN
                        INSERT INTO personal_event (creation_time, notes, type, period, period_time, id_team, id_player, id_match)
                        VALUES (
                            event_time,
                            'Player action: ' || event_type,
                            event_type,
                            period_num,
                            period_time,
                            team_id,
                            player_id,
                            match_rec.id_match
                        );
                        events_generated := events_generated + 1;
                    END IF;
                    
                ELSIF random_val < 0.9 THEN
                    -- Team event
                    event_type := team_events[1 + floor(random() * array_length(team_events, 1))];
                    
                    -- Choose random team
                    IF random() < 0.5 THEN
                        team_id := home_team_id;
                    ELSE
                        team_id := match_rec.id_team;
                    END IF;
                    
                    INSERT INTO team_event (creation_time, notes, type, period, period_time, id_team, id_match)
                    VALUES (
                        event_time,
                        'Team action: ' || event_type,
                        event_type,
                        period_num,
                        period_time,
                        team_id,
                        match_rec.id_match
                    );
                    events_generated := events_generated + 1;
                    
                ELSE
                    -- General event (pause, break, etc.)
                    event_type := general_events[1 + floor(random() * array_length(general_events, 1))];
                    
                    INSERT INTO general_event (creation_time, notes, type, period, period_time, id_match)
                    VALUES (
                        event_time,
                        'Game event: ' || event_type,
                        event_type,
                        period_num,
                        period_time,
                        match_rec.id_match
                    );
                    events_generated := events_generated + 1;
                END IF;
            END LOOP;
            
            -- Period end event
            INSERT INTO general_event (creation_time, notes, type, period, period_time, id_match)
            VALUES (
                match_rec.scheduled_at + INTERVAL '15 minutes' * j,
                'Period ' || j || ' ended',
                'period end',
                period_num,
                600,
                match_rec.id_match
            );
            events_generated := events_generated + 1;
            
            -- Add break between periods (except after 4th)
            IF j < 4 THEN
                INSERT INTO general_event (creation_time, notes, type, period, period_time, id_match)
                VALUES (
                    match_rec.scheduled_at + INTERVAL '15 minutes' * j + INTERVAL '1 minute',
                    CASE WHEN j = 2 THEN 'Halftime break started' ELSE 'Break started' END,
                    'break start',
                    period_num,
                    600,
                    match_rec.id_match
                );
                
                INSERT INTO general_event (creation_time, notes, type, period, period_time, id_match)
                VALUES (
                    match_rec.scheduled_at + INTERVAL '15 minutes' * (j+1) - INTERVAL '1 minute',
                    CASE WHEN j = 2 THEN 'Halftime break ended' ELSE 'Break ended' END,
                    'break end',
                    period_num,
                    600,
                    match_rec.id_match
                );
                events_generated := events_generated + 2;
            END IF;
        END LOOP;
        
        RAISE NOTICE 'Generated % events for match %', events_generated, match_rec.id_match;
    END LOOP;
END $$;

-- Summary:
-- This script creates:
-- 1. 5 new teams (Cedevita Olimpija, Budućnost VOLI, Igokea m:tel, Krka Novo mesto, Split)
-- 2. 60 players total (12 players per team) with realistic Serbian/regional names
-- 3. Team member relationships for all players
-- 4. 10 finished matches against these teams (mix of home and away games)
-- 5. Match tracking data for all 10 matches with realistic scores
-- 6. Team member match participation for all players in all matches
-- 7. Approximately 100 events per match including:
--    - Personal events (70%): shots, assists, fouls, rebounds, etc.
--    - Team events (20%): timeouts, technical fouls, formations
--    - General events (10%): period starts/ends, breaks, pauses
--
-- Total events generated: ~1000 events across all matches
-- Each match has realistic game flow with 4 periods and breaks between them