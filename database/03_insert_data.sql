-- Insert Nationality for Serbian players
INSERT INTO nationality (state) VALUES ('Serbia');

-- Insert Users with different roles (no customers)
-- ID 1: Admin User - admin@partizan.rs
-- ID 2: Milos Stojanovic - club manager  
-- ID 3: Aleksandar Milic - club owner (can create ticket price parameters)
-- ID 4: Nikola Radovic - analyst
-- ID 5: Marija Jankovic - scouting manager
-- ID 6: Petar Miletic - team manager
-- All passwords are 123456
INSERT INTO users (name, surname, email, phone, password, type) VALUES 
    ('Admin', 'User', 'admin@partizan.rs', '+381601234567', '$2a$10$y201nvT/gV/aKAddECTdIOkWKeVPb6pXr5.RFpPAP/unWmWiPXq9.', 'admin'),
    ('Milos', 'Stojanovic', 'manager@partizan.rs', '+381601234571', '$2a$10$y201nvT/gV/aKAddECTdIOkWKeVPb6pXr5.RFpPAP/unWmWiPXq9.', 'club manager'),
    ('Aleksandar', 'Milic', 'owner@partizan.rs', '+381601234572', '$2a$10$y201nvT/gV/aKAddECTdIOkWKeVPb6pXr5.RFpPAP/unWmWiPXq9.', 'club owner'),
    ('Nikola', 'Radovic', 'analyst@partizan.rs', '+381601234573', '$2a$10$y201nvT/gV/aKAddECTdIOkWKeVPb6pXr5.RFpPAP/unWmWiPXq9.', 'analyst'),
    ('Marija', 'Jankovic', 'scouting@partizan.rs', '+381601234574', '$2a$10$y201nvT/gV/aKAddECTdIOkWKeVPb6pXr5.RFpPAP/unWmWiPXq9.', 'scouting manager'),
    ('Petar', 'Miletic', 'teammanager@partizan.rs', '+381601234575', '$2a$10$y201nvT/gV/aKAddECTdIOkWKeVPb6pXr5.RFpPAP/unWmWiPXq9.', 'team manager');

-- Insert Position data
INSERT INTO position (name) VALUES 
    ('Point Guard'),
    ('Shooting Guard'),
    ('Small Forward'),
    ('Power Forward'),
    ('Center');

-- Insert current season (2025/26)
INSERT INTO season (started_at, name, tickets_for_sale, tickets_went_on_sale) VALUES
    ('2025-09-01', '2025/26 Season', TRUE, '2025-09-01 19:00:00+01');

-- Insert Partizan team
INSERT INTO team (name, state, city, hall, founded_date, coach, playing_style, key_strengths, key_weaknesses) VALUES 
    ('KK Partizan', 'Serbia', 'Belgrade', 'Stark Arena', '1945-10-04', 'Zeljko Obradovic', 'Defensive', 'Strong defense, experienced players', 'Young bench, inconsistent offense');

-- Insert opponent teams
INSERT INTO team (name, state, city, hall, founded_date, coach, playing_style, key_strengths, key_weaknesses) VALUES 
    ('KK Crvena Zvezda', 'Serbia', 'Belgrade', 'Aleksandar Nikolic Hall', '1945-03-03', 'Ioannis Sfairopoulos', 'Aggressive', 'Fast tempo, good shooters', 'Weak rebounding'),
    ('KK FMP', 'Serbia', 'Belgrade', 'FMP Hall', '1991-01-01', 'Marko Jaric', 'Fast', 'Young talent, energy', 'Lack of experience'),
    ('KK Mega', 'Serbia', 'Belgrade', 'Mega Factory Hall', '2006-01-01', 'Vladimir Jovanovic', 'Defensive', 'Athletic players', 'Poor defense');

-- Insert competition
INSERT INTO competition (name, started_at, number_of_matches) VALUES 
    ('ABA Liga 2025/26', '2025-09-01', 30);

-- Insert multiple zones with 4 sides each
INSERT INTO zone (name, rank, maximum_capacity, status) VALUES 
    ('Zone 100', 100, 160, 'enabled'),  -- 4 sides × 5 rows × 8 seats = 160 seats (least)
    ('Zone 200', 200, 400, 'enabled'),  -- 4 sides × 8 rows × 12.5 → 4 sides × 10 rows × 10 seats = 400 seats
    ('Zone 300', 300, 600, 'enabled'),  -- 4 sides × 10 rows × 15 seats = 600 seats  
    ('Zone 400', 400, 800, 'enabled');  -- 4 sides × 10 rows × 20 seats = 800 seats (most)

-- Insert 3 Partizan matches in the coming days (September 2025)
INSERT INTO match (name, scheduled_at, type, state, city, hall, is_in_our_hall, transportation_required, accommodation_required, tickets_for_sale, tickets_went_on_sale, id_competition, id_season, id_team) VALUES 
    ('Partizan vs Crvena Zvezda', '2025-09-05 19:00:00+01', 'home', 'Serbia', 'Belgrade', 'Stark Arena', TRUE, FALSE, FALSE, TRUE, '2025-09-05 19:00:00+01', 1, 1, 2),
    ('Partizan vs FMP', '2025-09-15 19:00:00+01', 'home', 'Serbia', 'Belgrade', 'Stark Arena', TRUE, FALSE, FALSE, TRUE, '2025-09-15 19:00:00+01', 1, 1, 3),
    ('Partizan vs Mega', '2025-09-25 19:00:00+01', 'home', 'Serbia', 'Belgrade', 'Stark Arena', TRUE, FALSE, FALSE, TRUE, '2025-09-25 19:00:00+01', 1, 1, 4);

-- Insert seats for all zones with 4 sides each

-- Zone 100: 4 sides × 5 rows × 8 seats = 160 seats (least)
INSERT INTO seat ("row", "number", type, direction, status, id_zone) 
SELECT 
    row_num,
    seat_num,
    'standard',
    CASE 
        WHEN side_num = 1 THEN 'north'
        WHEN side_num = 2 THEN 'east'
        WHEN side_num = 3 THEN 'south'
        WHEN side_num = 4 THEN 'west'
    END,
    'enabled',
    1  -- Zone 100
FROM generate_series(1, 4) AS side_num,
     generate_series(1, 5) AS row_num,
     generate_series(1, 8) AS seat_num;

-- Zone 200: 4 sides × 10 rows × 10 seats = 400 seats
INSERT INTO seat ("row", "number", type, direction, status, id_zone) 
SELECT 
    row_num,
    seat_num,
    'standard',
    CASE 
        WHEN side_num = 1 THEN 'north'
        WHEN side_num = 2 THEN 'east'
        WHEN side_num = 3 THEN 'south'
        WHEN side_num = 4 THEN 'west'
    END,
    'enabled',
    2  -- Zone 200
FROM generate_series(1, 4) AS side_num,
     generate_series(1, 10) AS row_num,
     generate_series(1, 10) AS seat_num;

-- Zone 300: 4 sides × 10 rows × 15 seats = 600 seats
INSERT INTO seat ("row", "number", type, direction, status, id_zone) 
SELECT 
    row_num,
    seat_num,
    'standard',
    CASE 
        WHEN side_num = 1 THEN 'north'
        WHEN side_num = 2 THEN 'east'
        WHEN side_num = 3 THEN 'south'
        WHEN side_num = 4 THEN 'west'
    END,
    'enabled',
    3  -- Zone 300
FROM generate_series(1, 4) AS side_num,
     generate_series(1, 10) AS row_num,
     generate_series(1, 15) AS seat_num;

-- Zone 400: 4 sides × 10 rows × 20 seats = 800 seats
INSERT INTO seat ("row", "number", type, direction, status, id_zone) 
SELECT 
    row_num,
    seat_num,
    'standard',
    CASE 
        WHEN side_num = 1 THEN 'north'
        WHEN side_num = 2 THEN 'east'
        WHEN side_num = 3 THEN 'south'
        WHEN side_num = 4 THEN 'west'
    END,
    'enabled',
    4  -- Zone 400
FROM generate_series(1, 4) AS side_num,
     generate_series(1, 10) AS row_num,
     generate_series(1, 20) AS seat_num;

-- Insert season tickets for all seats in all zones
INSERT INTO purchase_offer (name, description, type, status, released_at, created_at, expires_at, id_seat)
SELECT 
    'Season Ticket - ' || z.name || ' Row ' || s."row" || ' Seat ' || s."number" || ' (' || s.direction || ')',
    'Full season access to ' || z.name || ', Row ' || s."row" || ', Seat ' || s."number" || ' (' || s.direction || ' side)',
    'season ticket',
    'enabled',
    '2025-08-01',
    '2025-08-01',
    '2026-06-30',
    s.id_seat
FROM seat s 
JOIN zone z ON s.id_zone = z.id_zone;

-- Insert 10 players for Partizan (id_team = 1)
INSERT INTO player (name, surname, birthday, weight, height, id_nationality, id_position) VALUES
('Nikola', 'Jovic', '2002-06-09', 98, 208, 1, 1),
('Aleksa', 'Avramovic', '1994-10-25', 88, 192, 1, 2),
('Uros', 'Trifunovic', '2001-02-05', 90, 197, 1, 3),
('Balsa', 'Koprivica', '2000-05-01', 110, 213, 1, 5),
('Alen', 'Smailagic', '2000-08-18', 102, 208, 1, 4),
('Danilo', 'Andjusic', '1991-04-22', 86, 195, 1, 2),
('Zach', 'LeDay', '1994-05-30', 103, 202, 1, 4),
('James', 'Nunnally', '1990-07-14', 98, 201, 1, 3),
('Yam', 'Madar', '2000-12-21', 81, 190, 1, 1),
('Bruno', 'Caboclo', '1995-09-21', 104, 206, 1, 5);

-- Connect these players as team members for Partizan (id_team = 1)
-- Jersey numbers 1-10, all status 'active'
INSERT INTO team_member (jersey_number, status, id_player, id_team) VALUES
(1, 'active', 1, 1),
(2, 'active', 2, 1),
(3, 'active', 3, 1),
(4, 'active', 4, 1),
(5, 'active', 5, 1),
(6, 'active', 6, 1),
(7, 'active', 7, 1),
(8, 'active', 8, 1),
(9, 'active', 9, 1),
(10, 'active', 10, 1);

-- Insert 10 players for Crvena Zvezda (id_team = 2)
INSERT INTO player (name, surname, birthday, weight, height, id_nationality, id_position) VALUES
('Ognjen', 'Dobric', '1994-10-27', 92, 200, 1, 3),
('Branko', 'Lazic', '1989-01-12', 90, 195, 1, 2),
('Stefan', 'Markovic', '1988-04-25', 98, 197, 1, 1),
('Dejan', 'Davidovac', '1997-01-17', 100, 202, 1, 4),
('Miroslav', 'Raduljica', '1988-01-05', 113, 213, 1, 5),
('Nikola', 'Ivanovic', '1994-02-19', 85, 190, 1, 1),
('Luka', 'Mitrovic', '1993-03-21', 104, 205, 1, 4),
('Dalibor', 'Ilic', '2000-03-04', 98, 202, 1, 3),
('Nemanja', 'Nedovic', '1991-06-16', 87, 191, 1, 2),
('Filip', 'Petrušev', '2000-04-15', 102, 211, 1, 5);

-- Connect these players as team members for Crvena Zvezda (id_team = 2)
INSERT INTO team_member (jersey_number, status, id_player, id_team) VALUES
(1, 'active', 11, 2),
(2, 'active', 12, 2),
(3, 'active', 13, 2),
(4, 'active', 14, 2),
(5, 'active', 15, 2),
(6, 'active', 16, 2),
(7, 'active', 17, 2),
(8, 'active', 18, 2),
(9, 'active', 19, 2),
(10, 'active', 20, 2);

-- Insert 10 players for FMP (id_team = 3)
INSERT INTO player (name, surname, birthday, weight, height, id_nationality, id_position) VALUES
('Marko', 'Pecarski', '2000-02-12', 98, 208, 1, 4),
('Aleksa', 'Uskokovic', '1999-06-30', 82, 190, 1, 1),
('Stefan', 'Lazarevic', '1996-08-20', 95, 198, 1, 3),
('Nikola', 'Jankovic', '1994-02-13', 110, 206, 1, 5),
('Milos', 'Gajic', '1998-09-09', 90, 195, 1, 2),
('Petar', 'Rikalo', '1992-11-17', 85, 188, 1, 1),
('Filip', 'Stojanovic', '2001-05-22', 100, 203, 1, 4),
('Vuk', 'Radivojevic', '1983-07-30', 92, 197, 1, 2),
('Milan', 'Milovanovic', '1991-06-18', 105, 205, 1, 5),
('Dusan', 'Ristic', '1995-11-27', 110, 210, 1, 5);

-- Connect these players as team members for FMP (id_team = 3)
INSERT INTO team_member (jersey_number, status, id_player, id_team) VALUES
(1, 'active', 21, 3),
(2, 'active', 22, 3),
(3, 'active', 23, 3),
(4, 'active', 24, 3),
(5, 'active', 25, 3),
(6, 'active', 26, 3),
(7, 'active', 27, 3),
(8, 'active', 28, 3),
(9, 'active', 29, 3),
(10, 'active', 30, 3);

-- Insert 10 players for Mega (id_team = 4)
INSERT INTO player (name, surname, birthday, weight, height, id_nationality, id_position) VALUES
('Mihailo', 'Jovicic', '1999-01-24', 80, 188, 1, 1),
('Matej', 'Rudan', '2001-03-21', 98, 208, 1, 4),
('Nikola', 'Miskovic', '1999-01-25', 100, 210, 1, 5),
('Andrija', 'Marjanovic', '1999-01-14', 88, 193, 1, 2),
('Petar', 'Zivkovic', '2002-07-12', 92, 200, 1, 3),
('Marko', 'Kovacevic', '2000-09-09', 95, 198, 1, 3),
('Lazar', 'Vasic', '2001-02-15', 85, 190, 1, 1),
('Vladimir', 'Vukovic', '1998-05-05', 105, 205, 1, 5),
('Stefan', 'Simic', '2000-11-11', 90, 195, 1, 2),
('Uros', 'Plavsic', '1998-12-13', 110, 211, 1, 5);

-- Connect these players as team members for Mega (id_team = 4)
INSERT INTO team_member (jersey_number, status, id_player, id_team) VALUES
(1, 'active', 31, 4),
(2, 'active', 32, 4),
(3, 'active', 33, 4),
(4, 'active', 34, 4),
(5, 'active', 35, 4),
(6, 'active', 36, 4),
(7, 'active', 37, 4),
(8, 'active', 38, 4),
(9, 'active', 39, 4),
(10, 'active', 40, 4);

-- Insert season ticket pricing with different prices per zone
INSERT INTO season_ticket (id_purchase_offer, id_season, ticket_price)
SELECT 
    po.id_purchase_offer,
    1,
    CASE 
        WHEN po.name LIKE '%Zone 100%' THEN 25000  -- Premium zone, highest price
        WHEN po.name LIKE '%Zone 200%' THEN 20000  
        WHEN po.name LIKE '%Zone 300%' THEN 15000  
        WHEN po.name LIKE '%Zone 400%' THEN 10000  -- Furthest zone, lowest price
        ELSE 15000
    END
FROM purchase_offer po
WHERE po.type = 'season ticket';

-- Insert individual tickets for all seats in all zones for all 3 matches
INSERT INTO purchase_offer (name, description, type, status, released_at, created_at, expires_at, id_seat)
SELECT 
    'Individual Ticket - ' || m.name || ' - ' || z.name || ' Row ' || s."row" || ' Seat ' || s."number" || ' (' || s.direction || ')',
    'Single match ticket for ' || m.name || ' in ' || z.name || ', Row ' || s."row" || ', Seat ' || s."number" || ' (' || s.direction || ' side)',
    'individual ticket',
    'enabled',
    '2025-08-15',
    '2025-08-15',
    m.scheduled_at,
    s.id_seat
FROM seat s 
JOIN zone z ON s.id_zone = z.id_zone
CROSS JOIN match m
WHERE m.id_match IN (1, 2, 3);

-- Insert individual ticket details linking to matches
INSERT INTO individual_ticket (id_purchase_offer, id_match)
SELECT 
    po.id_purchase_offer,
    CASE 
        WHEN po.name LIKE '%Partizan vs Crvena Zvezda%' THEN 1
        WHEN po.name LIKE '%Partizan vs FMP%' THEN 2
        WHEN po.name LIKE '%Partizan vs Mega%' THEN 3
    END
FROM purchase_offer po
WHERE po.type = 'individual ticket';

-- SIMULATE SOME TICKET SALES TO TEST DYNAMIC PRICING
-- Create test customers for simulation
-- All passwords are 123456
INSERT INTO users (name, surname, email, phone, password, type) VALUES 
    ('Marko', 'Petrovic', 'marko@example.com', '+381601111111', '$2a$10$y201nvT/gV/aKAddECTdIOkWKeVPb6pXr5.RFpPAP/unWmWiPXq9.', 'customer'),
    ('Ana', 'Jovanovic', 'ana@example.com', '+381602222222', '$2a$10$y201nvT/gV/aKAddECTdIOkWKeVPb6pXr5.RFpPAP/unWmWiPXq9.', 'customer'),
    ('Stefan', 'Nikolic', 'stefan@example.com', '+381603333333', '$2a$10$y201nvT/gV/aKAddECTdIOkWKeVPb6pXr5.RFpPAP/unWmWiPXq9.', 'customer');

-- Add some credit cards for test customers  
-- Credit cards are: cc: 1234123412341234, cvv: 555
INSERT INTO credit_card (created_at, number, cvv, name, expiration_date, id_user) VALUES 
    ('2025-08-01', 'NqegFjTqg93HknhxnULVXgADsUorREd2xp90DSsm2kc=', 'feYkA5OO5zBFu9AGPBGXTA==', 'Marko Petrovic', '2028-12-31', 7),
    ('2025-08-01', 'NqegFjTqg93HknhxnULVXgADsUorREd2xp90DSsm2kc=', 'feYkA5OO5zBFu9AGPBGXTA==', 'Ana Jovanovic', '2028-12-31', 8),
    ('2025-08-01', 'NqegFjTqg93HknhxnULVXgADsUorREd2xp90DSsm2kc=', 'feYkA5OO5zBFu9AGPBGXTA==', 'Stefan Nikolic', '2028-12-31', 9);

-- Simulate sold tickets for Match 1 (Partizan vs Crvena Zvezda) - 10 tickets sold
-- This will increase zone occupancy and test dynamic pricing
INSERT INTO cart_item (id_cart, id_purchase_offer, added_at, price)
SELECT 
    c.id_cart,
    po.id_purchase_offer,
    '2025-08-20',
    2500 -- Fixed price at time of purchase
FROM cart c 
JOIN users u ON c.id_user = u.id_user
CROSS JOIN (
    SELECT po.id_purchase_offer 
    FROM purchase_offer po 
    JOIN individual_ticket it ON po.id_purchase_offer = it.id_purchase_offer
    WHERE it.id_match = 1 -- Match 1: Partizan vs Crvena Zvezda
    AND po.status = 'enabled'
    LIMIT 10
) po
WHERE u.type = 'customer'
AND c.is_current = true
LIMIT 10;

-- Mark these carts as bought to simulate actual sales
UPDATE cart SET status = 'bought', is_current = false 
WHERE id_cart IN (
    SELECT DISTINCT ci.id_cart 
    FROM cart_item ci 
    JOIN cart c ON ci.id_cart = c.id_cart
    JOIN users u ON c.id_user = u.id_user
    WHERE u.type = 'customer'
);

-- Mark the sold purchase offers as bought
UPDATE purchase_offer SET status = 'bought' 
WHERE id_purchase_offer IN (
    SELECT ci.id_purchase_offer 
    FROM cart_item ci 
    JOIN cart c ON ci.id_cart = c.id_cart
    WHERE c.status = 'bought'
);

-- INSERT REALISTIC TICKET PRICE PARAMETERS FOR ALL ZONES AND ALL MATCHES
-- Different parameters for each match and zone to test dynamic pricing
-- Zone 100 (Premium) > Zone 200 (VIP) > Zone 300 (Standard) > Zone 400 (Economy)

-- Match 1: Partizan vs Crvena Zvezda (September 5, 2025) - High demand derby match

-- Zone 100 (Premium - Most Expensive)
INSERT INTO ticket_price_parameter (price_factor, time_factor, minimum_seat_price, maximum_seat_price, id_user, id_zone, id_match)
VALUES (800, 70, 4000, 12000, 3, 1, 1);

-- Zone 200 (VIP)
INSERT INTO ticket_price_parameter (price_factor, time_factor, minimum_seat_price, maximum_seat_price, id_user, id_zone, id_match)
VALUES (800, 70, 3000, 9000, 3, 2, 1);

-- Zone 300 (Standard)
INSERT INTO ticket_price_parameter (price_factor, time_factor, minimum_seat_price, maximum_seat_price, id_user, id_zone, id_match)
VALUES (800, 70, 2000, 6000, 3, 3, 1);

-- Zone 400 (Economy - Least Expensive)
INSERT INTO ticket_price_parameter (price_factor, time_factor, minimum_seat_price, maximum_seat_price, id_user, id_zone, id_match)
VALUES (800, 70, 1000, 3000, 3, 4, 1);

-- Match 2: Partizan vs FMP (September 15, 2025) - Medium demand match

-- Zone 100 (Premium - Most Expensive)
INSERT INTO ticket_price_parameter (price_factor, time_factor, minimum_seat_price, maximum_seat_price, id_user, id_zone, id_match)
VALUES (500, 50, 2500, 7500, 3, 1, 2);

-- Zone 200 (VIP)
INSERT INTO ticket_price_parameter (price_factor, time_factor, minimum_seat_price, maximum_seat_price, id_user, id_zone, id_match)
VALUES (500, 50, 2000, 6000, 3, 2, 2);

-- Zone 300 (Standard)
INSERT INTO ticket_price_parameter (price_factor, time_factor, minimum_seat_price, maximum_seat_price, id_user, id_zone, id_match)
VALUES (500, 50, 1500, 4500, 3, 3, 2);

-- Zone 400 (Economy - Least Expensive)
INSERT INTO ticket_price_parameter (price_factor, time_factor, minimum_seat_price, maximum_seat_price, id_user, id_zone, id_match)
VALUES (500, 50, 800, 2400, 3, 4, 2);

-- Match 3: Partizan vs Mega (September 25, 2025) - Regular match

-- Zone 100 (Premium - Most Expensive)
INSERT INTO ticket_price_parameter (price_factor, time_factor, minimum_seat_price, maximum_seat_price, id_user, id_zone, id_match)
VALUES (400, 40, 2000, 6000, 3, 1, 3);

-- Zone 200 (VIP)
INSERT INTO ticket_price_parameter (price_factor, time_factor, minimum_seat_price, maximum_seat_price, id_user, id_zone, id_match)
VALUES (400, 40, 1500, 4500, 3, 2, 3);

-- Zone 300 (Standard)
INSERT INTO ticket_price_parameter (price_factor, time_factor, minimum_seat_price, maximum_seat_price, id_user, id_zone, id_match)
VALUES (400, 40, 1200, 3600, 3, 3, 3);

-- Zone 400 (Economy - Least Expensive)
INSERT INTO ticket_price_parameter (price_factor, time_factor, minimum_seat_price, maximum_seat_price, id_user, id_zone, id_match)
VALUES (400, 40, 600, 1800, 3, 4, 3);

-- Agencije za smestaj i za prevoz (10 smestajnih i 10 transportnih)
INSERT INTO agency (name, email, type) VALUES
('Blue Horizon Hotels', 'contact@bluehorizon.example', 'accommodation'),
('Comfort Stay Group', 'info@comfortstay.example', 'accommodation'),
('CityCenter Lodgings', 'reservations@citycenter.example', 'accommodation'),
('Seaside Suites', 'hello@seasidesuites.example', 'accommodation'),
('Mountain View Inns', 'booking@mountainview.example', 'accommodation'),
('Heritage Boutique Hotels', 'contact@heritageboutique.example', 'accommodation'),
('BudgetHotels Network', 'support@budgethotels.example', 'accommodation'),
('Grand Plaza Accommodations', 'info@grandplaza.example', 'accommodation'),
('Airport Transit Hotels', 'bookings@airporttransit.example', 'accommodation'),
('Riverside Retreats', 'contact@riversideretreats.example', 'accommodation'),
('Express Transport Co', 'ops@expresstransport.example', 'transportation'),
('BlueLine Coaches', 'info@bluelinecoaches.example', 'transportation'),
('SkyWays Airlines', 'sales@skyways.example', 'transportation'),
('RailLink Services', 'support@raillink.example', 'transportation'),
('CityShuttle Vans', 'contact@cityshuttle.example', 'transportation'),
('Elite Charter Buses', 'charter@elitebuses.example', 'transportation'),
('RapidTransit Logistics', 'logistics@rapidtransit.example', 'transportation'),
('Continental Flights', 'bookings@continentalflights.example', 'transportation'),
('GreenRoute Coaches', 'info@greenroute.example', 'transportation'),
('InterCity Transporters', 'contact@intercity.example', 'transportation');

-- Clanovi strucnog staba (10 unosa)
INSERT INTO management (member_name, member_surname, member_role) VALUES
('Marko', 'Petrovic', 'coach'),
('Ivan', 'Jovanovic', 'assistant coach'),
('Nemanja', 'Ilic', 'doctor'),
('Milan', 'Stojanov', 'therapist'),
('Nikola', 'Kovacevic', 'other'),
('Aleksandar', 'Milinkovic', 'assistant coach'),
('Stefan', 'Djordjevic', 'therapist'),
('Bogdan', 'Radonjic', 'doctor'),
('Uros', 'Lukic', 'coach'),
('Lazar', 'Mihajlovic', 'other');

-- =====================================================
-- PRIMER KORIŠĆENJA DINAMIČKIH CENA:
-- =====================================================
-- Danas je 2. septembar 2025
-- 
-- Match 1: Partizan vs Crvena Zvezda (5. septembar - za 3 dana)
-- - Derby utakmica, visok price_factor (8.0), time_factor (0.7)
-- - Min: 4000, Max: 12000 dinara
-- SELECT calculate_ticket_price(1, 1); 
--
-- Match 2: Partizan vs FMP (15. septembar - za 13 dana)  
-- - Srednji demand, price_factor (5.0), time_factor (0.5)
-- - Min: 2500, Max: 7500 dinara
-- SELECT calculate_ticket_price(2, 1);
--
-- Match 3: Partizan vs Mega (25. septembar - za 23 dana)
-- - Obična utakmica, nizak price_factor (4.0), time_factor (0.4) 
-- - Min: 2000, Max: 6000 dinara
-- SELECT calculate_ticket_price(3, 1);
-- =====================================================

-- ZA TESTIRANJE INDEKSA SANJA RADIC
-- INSERT INTO request (state, city, hall, budget, id_match, type) VALUES 
--     ('Serbia', 'Belgrade', 'Stark Arena', 50000, 1, 'accommodation'),
--     ('Serbia', 'Belgrade', 'Stark Arena', 30000, 1, 'transportation');

-- INSERT INTO request (state, city, hall, budget, id_match, type) VALUES 
--     ('Serbia', 'Belgrade', 'Stark Arena', 45000, 2, 'accommodation'),
--     ('Serbia', 'Belgrade', 'Stark Arena', 25000, 2, 'transportation');

-- INSERT INTO request (state, city, hall, budget, id_match, type) VALUES 
--     ('Serbia', 'Belgrade', 'Stark Arena', 40000, 3, 'accommodation'),
--     ('Serbia', 'Belgrade', 'Stark Arena', 20000, 3, 'transportation');

-- INSERT INTO sent_request (id_agency, id_request)
-- SELECT a.id_agency, r.id_request
-- FROM agency a
-- CROSS JOIN request r
-- WHERE a.type = 'accommodation' AND r.type = 'accommodation';

-- INSERT INTO sent_request (id_agency, id_request)
-- SELECT a.id_agency, r.id_request
-- FROM agency a
-- CROSS JOIN request r
-- WHERE a.type = 'transportation' AND r.type = 'transportation';

-- INSERT INTO accommodation_request (id_request, number_of_guests, number_of_rooms, check_in_date, check_out_date, accommodation_type)
-- SELECT id_request, 25, 15, '2025-09-04', '2025-09-06', 'hotel'
-- FROM request WHERE id_match = 1 AND type = 'accommodation';

-- INSERT INTO accommodation_request (id_request, number_of_guests, number_of_rooms, check_in_date, check_out_date, accommodation_type)
-- SELECT id_request, 25, 15, '2025-09-14', '2025-09-16', 'hotel'
-- FROM request WHERE id_match = 2 AND type = 'accommodation';

-- INSERT INTO accommodation_request (id_request, number_of_guests, number_of_rooms, check_in_date, check_out_date, accommodation_type)
-- SELECT id_request, 25, 15, '2025-09-24', '2025-09-26', 'hotel'
-- FROM request WHERE id_match = 3 AND type = 'accommodation';

-- INSERT INTO transportation_request (id_request, number_of_passengers, start_date, end_date, vehicle_type)
-- SELECT id_request, 25, '2025-09-04', '2025-09-06', 'autobus'
-- FROM request WHERE id_match = 1 AND type = 'transportation';

-- INSERT INTO transportation_request (id_request, number_of_passengers, start_date, end_date, vehicle_type)
-- SELECT id_request, 25, '2025-09-14', '2025-09-16', 'autobus'
-- FROM request WHERE id_match = 2 AND type = 'transportation';

-- INSERT INTO transportation_request (id_request, number_of_passengers, start_date, end_date, vehicle_type)
-- SELECT id_request, 25, '2025-09-24', '2025-09-26', 'autobus'
-- FROM request WHERE id_match = 3 AND type = 'transportation';

-- INSERT INTO offer (id_offer, price, user_id_user, id_match, id_agency, id_request, chosen, type, score)
-- SELECT 
--     ROW_NUMBER() OVER (ORDER BY a.id_agency, gs.n),
--     35000 + (RANDOM() * 25000)::INTEGER, 
--     NULL,
--     1,
--     a.id_agency,
--     1,
--     CASE WHEN a.id_agency = 1 AND gs.n = 1 THEN TRUE ELSE FALSE END, 
--     'accommodation',
--     (RANDOM() * 5)::NUMERIC(5,2) 
-- FROM agency a
-- CROSS JOIN generate_series(1, 8) gs(n) 
-- WHERE a.type = 'accommodation';

-- INSERT INTO accommodation_offer (id_offer, name, capacity, accommodation_type, id_agency, id_request, double_room, triple_room, quadruple_room, breakfast, fitness_center, pool, wifi, spa)
-- SELECT 
--     o.id_offer,
--     ag.name || ' - Offer ' || o.id_offer,
--     25 + (RANDOM() * 15)::INTEGER,
--     'hotel',
--     o.id_agency,
--     o.id_request,
--     RANDOM() < 0.8,
--     RANDOM() < 0.6,
--     RANDOM() < 0.4,
--     RANDOM() < 0.9,
--     RANDOM() < 0.7,
--     RANDOM() < 0.5,
--     RANDOM() < 0.95,
--     RANDOM() < 0.4
-- FROM offer o
-- JOIN agency ag ON o.id_agency = ag.id_agency
-- WHERE o.id_match = 1 AND o.type = 'accommodation';

-- INSERT INTO offer (id_offer, price, user_id_user, id_match, id_agency, id_request, chosen, type, score)
-- SELECT 
--     ROW_NUMBER() OVER (ORDER BY a.id_agency, gs.n) + 80,
--     15000 + (RANDOM() * 20000)::INTEGER,
--     NULL,
--     1,
--     a.id_agency,
--     2,
--     CASE WHEN a.id_agency = 11 AND gs.n = 1 THEN TRUE ELSE FALSE END,
--     'transportation',
--     (RANDOM() * 5)::NUMERIC(5,2)
-- FROM agency a
-- CROSS JOIN generate_series(1, 8) gs(n) 
-- WHERE a.type = 'transportation';

-- INSERT INTO transportation_offer (id_offer, company_name, capacity, type, id_agency, id_request, equipment_space, air_conditioning, tv, wifi, restroom)
-- SELECT 
--     o.id_offer,
--     ag.name || ' Transport',
--     30 + (RANDOM() * 20)::INTEGER,
--     'autobus',
--     o.id_agency,
--     o.id_request,
--     RANDOM() < 0.8,
--     RANDOM() < 0.9,
--     RANDOM() < 0.6,
--     RANDOM() < 0.7,
--     RANDOM() < 0.8
-- FROM offer o
-- JOIN agency ag ON o.id_agency = ag.id_agency
-- WHERE o.id_match = 1 AND o.type = 'transportation';

-- INSERT INTO offer (id_offer, price, user_id_user, id_match, id_agency, id_request, chosen, type, score)
-- SELECT 
--     ROW_NUMBER() OVER (ORDER BY a.id_agency, gs.n) + 160,
--     32000 + (RANDOM() * 23000)::INTEGER,
--     NULL,
--     2,
--     a.id_agency,
--     3,
--     CASE WHEN a.id_agency = 2 AND gs.n = 1 THEN TRUE ELSE FALSE END,
--     'accommodation',
--     (RANDOM() * 5)::NUMERIC(5,2)
-- FROM agency a
-- CROSS JOIN generate_series(1, 8) gs(n)
-- WHERE a.type = 'accommodation';

-- INSERT INTO accommodation_offer (id_offer, name, capacity, accommodation_type, id_agency, id_request, double_room, triple_room, quadruple_room, breakfast, fitness_center, pool, wifi, spa)
-- SELECT 
--     o.id_offer,
--     ag.name || ' - Offer ' || o.id_offer,
--     25 + (RANDOM() * 15)::INTEGER,
--     'hotel',
--     o.id_agency,
--     o.id_request,
--     RANDOM() < 0.8,
--     RANDOM() < 0.6,
--     RANDOM() < 0.4,
--     RANDOM() < 0.9,
--     RANDOM() < 0.7,
--     RANDOM() < 0.5,
--     RANDOM() < 0.95,
--     RANDOM() < 0.4
-- FROM offer o
-- JOIN agency ag ON o.id_agency = ag.id_agency
-- WHERE o.id_match = 2 AND o.type = 'accommodation';

-- INSERT INTO offer (id_offer, price, user_id_user, id_match, id_agency, id_request, chosen, type, score)
-- SELECT 
--     ROW_NUMBER() OVER (ORDER BY a.id_agency, gs.n) + 240,
--     14000 + (RANDOM() * 18000)::INTEGER,
--     NULL,
--     2,
--     a.id_agency,
--     4,
--     CASE WHEN a.id_agency = 12 AND gs.n = 1 THEN TRUE ELSE FALSE END,
--     'transportation',
--     (RANDOM() * 5)::NUMERIC(5,2)
-- FROM agency a
-- CROSS JOIN generate_series(1, 8) gs(n)
-- WHERE a.type = 'transportation';

-- INSERT INTO transportation_offer (id_offer, company_name, capacity, type, id_agency, id_request, equipment_space, air_conditioning, tv, wifi, restroom)
-- SELECT 
--     o.id_offer,
--     ag.name || ' Transport',
--     30 + (RANDOM() * 20)::INTEGER,
--     'autobus',
--     o.id_agency,
--     o.id_request,
--     RANDOM() < 0.8,
--     RANDOM() < 0.9,
--     RANDOM() < 0.6,
--     RANDOM() < 0.7,
--     RANDOM() < 0.8
-- FROM offer o
-- JOIN agency ag ON o.id_agency = ag.id_agency
-- WHERE o.id_match = 2 AND o.type = 'transportation';

-- INSERT INTO offer (id_offer, price, user_id_user, id_match, id_agency, id_request, chosen, type, score)
-- SELECT 
--     ROW_NUMBER() OVER (ORDER BY a.id_agency, gs.n) + 320,
--     30000 + (RANDOM() * 20000)::INTEGER,
--     NULL,
--     3,
--     a.id_agency,
--     5,
--     CASE WHEN a.id_agency = 3 AND gs.n = 1 THEN TRUE ELSE FALSE END,
--     'accommodation',
--     (RANDOM() * 5)::NUMERIC(5,2)
-- FROM agency a
-- CROSS JOIN generate_series(1, 8) gs(n)
-- WHERE a.type = 'accommodation';

-- INSERT INTO accommodation_offer (id_offer, name, capacity, accommodation_type, id_agency, id_request, double_room, triple_room, quadruple_room, breakfast, fitness_center, pool, wifi, spa)
-- SELECT 
--     o.id_offer,
--     ag.name || ' - Offer ' || o.id_offer,
--     25 + (RANDOM() * 15)::INTEGER,
--     'hotel',
--     o.id_agency,
--     o.id_request,
--     RANDOM() < 0.8,
--     RANDOM() < 0.6,
--     RANDOM() < 0.4,
--     RANDOM() < 0.9,
--     RANDOM() < 0.7,
--     RANDOM() < 0.5,
--     RANDOM() < 0.95,
--     RANDOM() < 0.4
-- FROM offer o
-- JOIN agency ag ON o.id_agency = ag.id_agency
-- WHERE o.id_match = 3 AND o.type = 'accommodation';

-- INSERT INTO offer (id_offer, price, user_id_user, id_match, id_agency, id_request, chosen, type, score)
-- SELECT 
--     ROW_NUMBER() OVER (ORDER BY a.id_agency, gs.n) + 400,
--     13000 + (RANDOM() * 17000)::INTEGER,
--     NULL,
--     3,
--     a.id_agency,
--     6,
--     CASE WHEN a.id_agency = 13 AND gs.n = 1 THEN TRUE ELSE FALSE END,
--     'transportation',
--     (RANDOM() * 5)::NUMERIC(5,2)
-- FROM agency a
-- CROSS JOIN generate_series(1, 8) gs(n)
-- WHERE a.type = 'transportation';

-- INSERT INTO transportation_offer (id_offer, company_name, capacity, type, id_agency, id_request, equipment_space, air_conditioning, tv, wifi, restroom)
-- SELECT 
--     o.id_offer,
--     ag.name || ' Transport',
--     30 + (RANDOM() * 20)::INTEGER,
--     'autobus',
--     o.id_agency,
--     o.id_request,
--     RANDOM() < 0.8,
--     RANDOM() < 0.9,
--     RANDOM() < 0.6,
--     RANDOM() < 0.7,
--     RANDOM() < 0.8
-- FROM offer o
-- JOIN agency ag ON o.id_agency = ag.id_agency
-- WHERE o.id_match = 3 AND o.type = 'transportation';
-- INSERT INTO offer (id_offer, price, user_id_user, id_match, id_agency, id_request, chosen, type, score)
-- SELECT 
--     480 + ROW_NUMBER() OVER (ORDER BY RANDOM()),
--     20000 + (RANDOM() * 30000)::INTEGER,
--     NULL,
--     ((ROW_NUMBER() OVER (ORDER BY RANDOM()) - 1) % 3) + 1,
--     ((ROW_NUMBER() OVER (ORDER BY RANDOM()) - 1) % 10) + 1, 
--     CASE 
--         WHEN ((ROW_NUMBER() OVER (ORDER BY RANDOM()) - 1) % 3) + 1 = 1 THEN 1
--         WHEN ((ROW_NUMBER() OVER (ORDER BY RANDOM()) - 1) % 3) + 1 = 2 THEN 3
--         ELSE 5
--     END,
--     FALSE, 
--     'accommodation',
--     (RANDOM() * 5)::NUMERIC(5,2)
-- FROM generate_series(1, 20);

-- INSERT INTO accommodation_offer (id_offer, name, capacity, accommodation_type, id_agency, id_request, double_room, triple_room, quadruple_room, breakfast, fitness_center, pool, wifi, spa)
-- SELECT 
--     o.id_offer,
--     ag.name || ' - Additional Offer ' || o.id_offer,
--     25 + (RANDOM() * 15)::INTEGER,
--     'hotel',
--     o.id_agency,
--     o.id_request,
--     RANDOM() < 0.8,
--     RANDOM() < 0.6,
--     RANDOM() < 0.4,
--     RANDOM() < 0.9,
--     RANDOM() < 0.7,
--     RANDOM() < 0.5,
--     RANDOM() < 0.95,
--     RANDOM() < 0.4
-- FROM offer o
-- JOIN agency ag ON o.id_agency = ag.id_agency
-- WHERE o.id_offer >= 480 AND o.type = 'accommodation';