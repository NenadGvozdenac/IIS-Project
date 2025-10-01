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
    ('Partizan vs FMP', '2025-10-02 19:00:00+01', 'home', 'Serbia', 'Belgrade', 'Stark Arena', TRUE, FALSE, FALSE, TRUE, '2025-10-02 19:00:00+01', 1, 1, 3),
    ('Partizan vs Mega', '2025-10-07 19:00:00+01', 'home', 'Serbia', 'Belgrade', 'Stark Arena', TRUE, FALSE, FALSE, TRUE, '2025-10-07 19:00:00+01', 1, 1, 4);

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
('Marko', 'Petrović', 'coach'),
('Ivan', 'Jovanović', 'assistant coach'),
('Nemanja', 'Ilić', 'doctor'),
('Milan', 'Stojanov', 'therapist'),
('Nikola', 'Kovačević', 'other'),
('Aleksandar', 'Milinković', 'assistant coach'),
('Stefan', 'Đorđević', 'therapist'),
('Bogdan', 'Radonjić', 'doctor'),
('Uroš', 'Lukić', 'coach'),
('Lazar', 'Mihajlović', 'other');


-- ===============================================================
-- SCOUTING SYSTEM DATA
-- ===============================================================

-- Insert Metric Types
INSERT INTO metric_type (type) VALUES 
('Quantitative'),
('Descriptive');

-- Insert Session Status types
INSERT INTO session_status (status) VALUES 
('Pending'),
('Ongoing'),
('Finished'),
('Canceled');

-- Insert Session Types
INSERT INTO session_type (type) VALUES 
('Training'),
('Game'),
('Playoff Game');

-- Insert comprehensive metrics for player evaluation
-- Quantitative metrics (NBA Box Score Stats)
INSERT INTO metrics (name, is_permanent, metric_weight, id_user, id_metric_type) VALUES 
('PTS (Points)', 1, 10, 5, 1),
('FGM (Field Goals Made)', 1, 8, 5, 1),
('FGA (Field Goals Attempted)', 1, 6, 5, 1),
('FG% (Field Goal Percentage)', 1, 9, 5, 1),
('3PM (Three Points Made)', 1, 8, 5, 1),
('3PA (Three Points Attempted)', 1, 6, 5, 1),
('3P% (Three Point Percentage)', 1, 8, 5, 1),
('FTM (Free Throws Made)', 1, 7, 5, 1),
('FTA (Free Throws Attempted)', 1, 6, 5, 1),
('FT% (Free Throw Percentage)', 1, 8, 5, 1),
('OREB (Offensive Rebounds)', 1, 7, 5, 1),
('DREB (Defensive Rebounds)', 1, 8, 5, 1),
('REB (Total Rebounds)', 1, 9, 5, 1),
('AST (Assists)', 1, 9, 5, 1),
('STL (Steals)', 1, 7, 5, 1),
('BLK (Blocks)', 1, 7, 5, 1),
('TOV (Turnovers)', 1, 6, 5, 1),
('PF (Personal Fouls)', 1, 5, 5, 1),
('MIN (Minutes Played)', 1, 6, 5, 1),
('+/- (Plus Minus)', 1, 7, 5, 1);

-- Descriptive metrics (qualitative/observational) - All created by scout
INSERT INTO metrics (name, is_permanent, metric_weight, id_user, id_metric_type) VALUES 
('Leadership Qualities', 1, 9, 5, 2),
('Team Chemistry', 1, 8, 5, 2),
('Communication Skills', 1, 8, 5, 2),
('Work Ethic', 1, 9, 5, 2),
('Basketball IQ', 1, 10, 5, 2),
('Defensive Intensity', 1, 8, 5, 2),
('Clutch Performance', 1, 9, 5, 2),
('Coachability', 1, 9, 5, 2),
('Mental Toughness', 1, 8, 5, 2),
('Court Vision', 1, 9, 5, 2),
('Shooting Form', 1, 7, 5, 2),
('Ball Handling Skills', 1, 8, 5, 2);

-- ===============================================================
-- SCOUTING SESSIONS DATA
-- ===============================================================

-- Insert scouting sessions for Partizan players (players 1-10)
INSERT INTO session (start_time, end_time, id_session_status, id_session_type, id_user, id_player) VALUES 
-- Training sessions
('2025-09-01', '2025-09-01', 3, 1, 5, 1), -- Nikola Jovic - Training - Finished
('2025-09-02', '2025-09-02', 3, 1, 5, 2), -- Aleksa Avramovic - Training - Finished
('2025-09-03', '2025-09-03', 3, 1, 5, 3), -- Uros Trifunovic - Training - Finished
('2025-09-04', '2025-09-04', 3, 1, 5, 4), -- Balsa Koprivica - Training - Finished
('2025-09-05', '2025-09-05', 3, 1, 5, 5), -- Alen Smailagic - Training - Finished

-- Game analysis sessions
('2025-09-06', '2025-09-06', 3, 2, 5, 1), -- Nikola Jovic - Game analysis - Finished
('2025-09-07', '2025-09-07', 3, 2, 5, 2), -- Aleksa Avramovic - Game analysis - Finished
('2025-09-08', '2025-09-08', 3, 2, 5, 3), -- Uros Trifunovic - Game analysis - Finished
('2025-09-09', '2025-09-09', 2, 2, 5, 6), -- Danilo Andjusic - Game analysis - Ongoing
('2025-09-10', '2025-09-10', 3, 2, 5, 7), -- Zach LeDay - Game analysis - Finished

-- Upcoming sessions
('2025-09-25', '2025-09-25', 1, 1, 5, 8), -- James Nunnally - Training - Pending
('2025-09-26', '2025-09-26', 1, 2, 5, 9), -- Yam Madar - Game analysis - Pending
('2025-09-27', '2025-09-27', 1, 3, 5, 10), -- Bruno Caboclo - Playoff Game analysis - Pending

-- Sessions for other team players
('2025-09-11', '2025-09-11', 3, 1, 5, 11), -- Player 11 - Training - Finished
('2025-09-12', '2025-09-12', 3, 2, 5, 12), -- Player 12 - Game analysis - Finished
('2025-09-13', '2025-09-13', 1, 1, 5, 13), -- Player 13 - Training - Pending
('2025-09-14', '2025-09-14', 4, 2, 5, 14), -- Player 14 - Game analysis - Canceled
('2025-09-15', '2025-09-15', 1, 3, 5, 15), -- Player 15 - Playoff Game analysis - Pending

-- More training sessions
('2025-09-16', '2025-09-16', 3, 1, 5, 21), -- Player 21 - Training - Finished
('2025-09-17', '2025-09-17', 3, 1, 5, 22), -- Player 22 - Training - Finished
('2025-09-18', '2025-09-18', 2, 1, 5, 23), -- Player 23 - Training - Ongoing
('2025-09-19', '2025-09-19', 1, 2, 5, 24), -- Player 24 - Game analysis - Pending
('2025-09-20', '2025-09-20', 1, 1, 5, 25); -- Player 25 - Training - Pending

-- ===============================================================
-- PHYSICAL METRICS DATA
-- ===============================================================

-- Insert comprehensive physical metrics for players
-- Partizan players (1-10) - High performance data
INSERT INTO physical_metrics (vertical_jump, fat_percentage, bench_press_weight, squat_weight, sprint_speed, weight, height, wingspan, date_of_measurement, id_player) VALUES 
(85, 8, 120, 180, 18, 98, 208, 215, '2025-09-01', 1),  -- Nikola Jovic
(78, 7, 105, 160, 20, 88, 192, 198, '2025-09-01', 2),  -- Aleksa Avramovic
(82, 9, 110, 170, 19, 90, 197, 203, '2025-09-02', 3),  -- Uros Trifunovic
(92, 12, 140, 220, 16, 110, 213, 220, '2025-09-03', 4), -- Balsa Koprivica
(88, 10, 125, 190, 17, 102, 208, 216, '2025-09-04', 5), -- Alen Smailagic
(76, 6, 95, 150, 21, 86, 195, 201, '2025-09-05', 6),   -- Danilo Andjusic
(86, 8, 130, 200, 17, 103, 202, 210, '2025-09-06', 7),  -- Zach LeDay
(80, 7, 115, 175, 18, 98, 201, 207, '2025-09-07', 8),   -- James Nunnally
(75, 5, 90, 145, 22, 81, 190, 196, '2025-09-08', 9),    -- Yam Madar
(94, 11, 135, 210, 16, 104, 206, 218, '2025-09-09', 10); -- Bruno Caboclo

-- Other team players (11-25) - Competitive data
INSERT INTO physical_metrics (vertical_jump, fat_percentage, bench_press_weight, squat_weight, sprint_speed, weight, height, wingspan, date_of_measurement, id_player) VALUES 
(79, 8, 108, 165, 19, 92, 200, 206, '2025-09-10', 11), -- Player 11
(77, 7, 100, 155, 20, 90, 195, 201, '2025-09-10', 12), -- Player 12
(74, 9, 102, 158, 19, 98, 197, 203, '2025-09-11', 13), -- Player 13
(83, 10, 118, 182, 18, 100, 202, 208, '2025-09-11', 14), -- Player 14
(89, 13, 145, 225, 15, 113, 213, 221, '2025-09-12', 15), -- Player 15
(72, 6, 88, 140, 22, 85, 190, 195, '2025-09-12', 21), -- Player 21
(85, 9, 122, 185, 17, 104, 205, 212, '2025-09-13', 22), -- Player 22
(81, 8, 112, 172, 18, 98, 202, 208, '2025-09-13', 23), -- Player 23
(76, 7, 96, 148, 21, 87, 191, 197, '2025-09-14', 24), -- Player 24
(91, 10, 128, 195, 16, 102, 211, 217, '2025-09-14', 25); -- Player 25

-- ===============================================================
-- SESSION METRICS DATA (Evaluation Scores)
-- ===============================================================

-- Session metrics for finished sessions with realistic NBA box score stats
-- For quantitative metrics: actual NBA statistical values
-- For descriptive metrics: 1-10 scale ratings

-- Nikola Jovic training session (session 1)
INSERT INTO session_metrics (value, id_session, id_metrics) VALUES 
('24', 1, 1), 
('9', 1, 2), 
('18', 1, 3),
('50.0', 1, 4),
('3', 1, 5),
('8', 1, 6),
('37.5', 1, 7), 
('3', 1, 8), 
('4', 1, 9), 
('75.0', 1, 10), 
('2', 1, 11), 
('6', 1, 12),
('8', 1, 13),
('7', 1, 14),
('2', 1, 15),
('1', 1, 16),
('8.5', 1, 21),
('9.0', 1, 25);

-- Aleksa Avramovic training session (session 2)
INSERT INTO session_metrics (value, id_session, id_metrics) VALUES 
('19', 2, 1), 
('7', 2, 2), 
('15', 2, 3),
('46.7', 2, 4),
('3', 2, 5),
('7', 2, 6),
('42.9', 2, 7), 
('2', 2, 8), 
('2', 2, 9), 
('100.0', 2, 10), 
('1', 2, 11), 
('4', 2, 12),
('5', 2, 13),
('6', 2, 14),
('3', 2, 15),
('0', 2, 16),
('7.8', 2, 21),
('8.5', 2, 25),
('9.1', 2, 30);

-- Uros Trifunovic training session (session 3)
INSERT INTO session_metrics (value, id_session, id_metrics) VALUES 
('15', 3, 1), 
('6', 3, 2), 
('14', 3, 3),
('42.9', 3, 4),
('1', 3, 5),
('4', 3, 6),
('25.0', 3, 7), 
('2', 3, 8), 
('3', 3, 9), 
('66.7', 3, 10), 
('2', 3, 11), 
('5', 3, 12),
('7', 3, 13),
('4', 3, 14),
('1', 3, 15),
('1', 3, 16),
('7.2', 3, 21), -- Leadership Qualities
('8.8', 3, 25), -- Basketball IQ
('8.0', 3, 32); -- Ball Handling Skills

-- Balsa Koprivica training session (session 4)
INSERT INTO session_metrics (value, id_session, id_metrics) VALUES 
('13', 4, 1), 
('6', 4, 2), 
('10', 4, 3),
('60.0', 4, 4),
('0', 4, 5),
('1', 4, 6),
('0.0', 4, 7), 
('1', 4, 8), 
('2', 4, 9), 
('50.0', 4, 10), 
('4', 4, 11), 
('8', 4, 12),
('12', 4, 13),
('2', 4, 14),
('0', 4, 15),
('3', 4, 16),
('8.8', 4, 21); -- Leadership Qualities

-- Alen Smailagic training session (session 5)
INSERT INTO session_metrics (value, id_session, id_metrics) VALUES 
('17', 5, 1), 
('7', 5, 2), 
('16', 5, 3),
('43.8', 5, 4),
('2', 5, 5),
('6', 5, 6),
('33.3', 5, 7), 
('1', 5, 8), 
('2', 5, 9), 
('50.0', 5, 10), 
('3', 5, 11), 
('6', 5, 12),
('9', 5, 13),
('3', 5, 14),
('1', 5, 15),
('2', 5, 16),
('7.9', 5, 21), -- Leadership Qualities
('8.3', 5, 25), -- Basketball IQ
('7.6', 5, 28); -- Coachability

-- Game analysis sessions
-- Nikola Jovic game analysis (session 6)
INSERT INTO session_metrics (value, id_session, id_metrics) VALUES 
('26', 6, 1), 
('10', 6, 2), 
('19', 6, 3),
('52.6', 6, 4),
('4', 6, 5),
('9', 6, 6),
('44.4', 6, 7), 
('2', 6, 8), 
('2', 6, 9), 
('100.0', 6, 10), 
('1', 6, 11), 
('7', 6, 12),
('8', 6, 13),
('8', 6, 14),
('2', 6, 15),
('1', 6, 16),
('34', 6, 19), -- MIN (Minutes Played)
('+8', 6, 20), -- +/- (Plus Minus)
('8.9', 6, 25), -- Basketball IQ
('9.2', 6, 30); -- Court Vision

-- Aleksa Avramovic game analysis (session 7)
INSERT INTO session_metrics (value, id_session, id_metrics) VALUES 
('22', 7, 1), 
('8', 7, 2), 
('16', 7, 3),
('50.0', 7, 4),
('4', 7, 5),
('8', 7, 6),
('50.0', 7, 7), 
('2', 7, 8), 
('2', 7, 9), 
('100.0', 7, 10), 
('0', 7, 11), 
('4', 7, 12),
('4', 7, 13),
('7', 7, 14),
('3', 7, 15),
('0', 7, 16),
('32', 7, 19), -- MIN (Minutes Played)
('+12', 7, 20), -- +/- (Plus Minus)
('8.4', 7, 25), -- Basketball IQ
('8.8', 7, 22); -- Team Chemistry

-- Additional session metrics for other finished sessions
INSERT INTO session_metrics (value, id_session, id_metrics) VALUES 
-- Player 11 training (session 14)
('18', 14, 1), ('7', 14, 2), ('15', 14, 3), ('46.7', 14, 4), ('7.5', 14, 21), ('8.2', 14, 25),

-- Player 12 game analysis (session 15)
('16', 15, 1), ('6', 15, 2), ('13', 15, 3), ('46.2', 15, 4), ('29', 15, 19), ('8.0', 15, 25),

-- Player 21 training (session 18)
('14', 18, 1), ('5', 18, 2), ('12', 18, 3), ('41.7', 18, 4), ('7.1', 18, 21), ('7.8', 18, 25),

-- Player 22 training (session 19)
('12', 19, 1), ('4', 19, 2), ('10', 19, 3), ('40.0', 19, 4), ('7.6', 19, 21), ('8.4', 19, 25);






-- Insert 5 new teams
INSERT INTO team (name, state, city, hall, founded_date, coach, playing_style, key_strengths, key_weaknesses) VALUES 
    ('KK Igokea', 'Bosnia and Herzegovina', 'Aleksandrovac', 'Igokea Arena', '1947-01-01', 'Dragan Bajic', 'Balanced', 'Team chemistry, disciplined play', 'Limited depth, size issues'),
    ('KK Cedevita Olimpija', 'Slovenia', 'Ljubljana', 'Stozice Arena', '2019-07-01', 'Jurica Golemac', 'Fast-paced', 'Speed, three-point shooting', 'Defensive consistency'),
    ('KK Mornar', 'Montenegro', 'Bar', 'Topolica Sports Center', '1979-01-01', 'Marko Jankovic', 'Physical', 'Rebounding, inside game', 'Perimeter shooting'),
    ('KK Mega Soccerbet', 'Serbia', 'Belgrade', 'Mega Factory Hall', '2006-01-01', 'Vladimir Jovanovic', 'Developmental', 'Young talent, athleticism', 'Experience, consistency'),
    ('KK Spartak', 'Serbia', 'Subotica', 'Spartak Hall', '1945-01-01', 'Aleksandar Nikolic', 'Traditional', 'Fundamentals, team play', 'Modern game adaptation');

-- Insert 50 new players (10 per team)
-- Players for KK Igokea
INSERT INTO player (name, surname, birthday, weight, height, id_nationality, id_position) VALUES
    ('Marko', 'Petrovic', '1995-03-15', 85, 190, 1, 1), -- Point Guard
    ('Nikola', 'Jovanovic', '1994-07-22', 90, 195, 1, 2), -- Shooting Guard
    ('Stefan', 'Milic', '1996-11-08', 95, 200, 1, 3), -- Small Forward
    ('Aleksandar', 'Stojanovic', '1993-05-12', 100, 205, 1, 4), -- Power Forward
    ('Milan', 'Radovic', '1992-09-30', 105, 210, 1, 5), -- Center
    ('Luka', 'Nikolic', '1997-01-18', 88, 188, 1, 1), -- Point Guard
    ('Milos', 'Dimitrijevic', '1995-12-03', 92, 193, 1, 2), -- Shooting Guard
    ('Nemanja', 'Stankovic', '1994-04-25', 97, 198, 1, 3), -- Small Forward
    ('Vladimir', 'Popovic', '1993-08-14', 102, 207, 1, 4), -- Power Forward
    ('Dusan', 'Matic', '1991-02-07', 108, 212, 1, 5); -- Center

-- Players for KK Cedevita Olimpija
INSERT INTO player (name, surname, birthday, weight, height, id_nationality, id_position) VALUES
    ('Zoran', 'Dragic', '1994-06-10', 87, 185, 1, 1), -- Point Guard
    ('Bojan', 'Bogdanovic', '1993-03-28', 93, 196, 1, 2), -- Shooting Guard
    ('Goran', 'Novakovic', '1995-09-17', 96, 201, 1, 3), -- Small Forward
    ('Predrag', 'Milosevic', '1992-12-05', 101, 206, 1, 4), -- Power Forward
    ('Vlade', 'Marjanovic', '1990-11-21', 110, 215, 1, 5), -- Center
    ('Darko', 'Radic', '1996-04-09', 89, 190, 1, 1), -- Point Guard
    ('Igor', 'Stamenkovic', '1994-08-16', 94, 194, 1, 2), -- Shooting Guard
    ('Miodrag', 'Andric', '1995-01-12', 98, 203, 1, 3), -- Small Forward
    ('Dejan', 'Savic', '1993-07-30', 103, 208, 1, 4), -- Power Forward
    ('Branko', 'Lazic', '1991-10-23', 107, 211, 1, 5); -- Center

-- Players for KK Mornar
INSERT INTO player (name, surname, birthday, weight, height, id_nationality, id_position) VALUES
    ('Petar', 'Vukcevic', '1995-05-20', 86, 187, 1, 1), -- Point Guard
    ('Marko', 'Simonovic', '1994-02-14', 91, 192, 1, 2), -- Shooting Guard
    ('Veljko', 'Petkovic', '1996-08-07', 94, 199, 1, 3), -- Small Forward
    ('Danilo', 'Andjusic', '1993-11-19', 99, 204, 1, 4), -- Power Forward
    ('Boban', 'Marjanovic', '1988-08-15', 125, 220, 1, 5), -- Center
    ('Filip', 'Petrusev', '1997-12-27', 88, 189, 1, 1), -- Point Guard
    ('Aleksa', 'Avramovic', '1995-06-03', 93, 195, 1, 2), -- Shooting Guard
    ('Ognjen', 'Dobric', '1994-10-11', 97, 202, 1, 3), -- Small Forward
    ('Uros', 'Plavsic', '1992-04-26', 104, 209, 1, 4), -- Power Forward
    ('Nikola', 'Milutinov', '1989-01-08', 112, 213, 1, 5); -- Center

-- Players for KK Mega Soccerbet
INSERT INTO player (name, surname, birthday, weight, height, id_nationality, id_position) VALUES
    ('Luka', 'Doncic', '1999-02-28', 84, 186, 1, 1), -- Point Guard
    ('Bogdan', 'Bogdanovic', '1998-08-18', 89, 191, 1, 2), -- Shooting Guard
    ('Nikola', 'Jokic', '1999-05-15', 92, 197, 1, 3), -- Small Forward
    ('Marko', 'Gudurić', '1997-11-30', 96, 203, 1, 4), -- Power Forward
    ('Alperen', 'Sengun', '2000-07-25', 100, 208, 1, 5), -- Center
    ('Vasilije', 'Micic', '1998-01-13', 85, 183, 1, 1), -- Point Guard
    ('Aleksej', 'Pokusevski', '2001-12-26', 87, 188, 1, 2), -- Shooting Guard
    ('Stefan', 'Jovic', '1999-09-04', 91, 194, 1, 3), -- Small Forward
    ('Nikola', 'Kalinic', '1997-03-22', 95, 200, 1, 4), -- Power Forward
    ('Marko', 'Simonovic', '2000-06-17', 98, 205, 1, 5); -- Center

-- Players for KK Spartak
INSERT INTO player (name, surname, birthday, weight, height, id_nationality, id_position) VALUES
    ('Milos', 'Teodosic', '1987-03-19', 86, 185, 1, 1), -- Point Guard
    ('Nemanja', 'Bjelica', '1988-05-09', 90, 193, 1, 2), -- Shooting Guard
    ('Boban', 'Marjanovic', '1988-08-15', 94, 198, 1, 3), -- Small Forward
    ('Miroslav', 'Raduljica', '1988-01-05', 99, 205, 1, 4), -- Power Forward
    ('Nenad', 'Krstic', '1983-07-25', 107, 213, 1, 5), -- Center
    ('Stefan', 'Markovic', '1994-10-11', 88, 190, 1, 1), -- Point Guard
    ('Marko', 'Keselj', '1995-04-28', 92, 196, 1, 2), -- Shooting Guard
    ('Danilo', 'Ostojic', '1996-07-15', 96, 201, 1, 3), -- Small Forward
    ('Milenko', 'Tepic', '1993-12-02', 101, 207, 1, 4), -- Power Forward
    ('Dusan', 'Ristic', '1991-08-20', 105, 210, 1, 5); -- Center

-- Insert team_member relationships for all new players
-- Using specific values instead of subqueries for better compatibility

-- KK Igokea team_members
INSERT INTO team_member (jersey_number, status, id_player, id_team) 
SELECT 1, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Marko' AND p.surname = 'Petrovic' AND p.birthday = '1995-03-15' AND t.name = 'KK Igokea'
UNION ALL
SELECT 2, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Nikola' AND p.surname = 'Jovanovic' AND p.birthday = '1994-07-22' AND t.name = 'KK Igokea'
UNION ALL
SELECT 3, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Stefan' AND p.surname = 'Milic' AND p.birthday = '1996-11-08' AND t.name = 'KK Igokea'
UNION ALL
SELECT 4, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Aleksandar' AND p.surname = 'Stojanovic' AND p.birthday = '1993-05-12' AND t.name = 'KK Igokea'
UNION ALL
SELECT 5, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Milan' AND p.surname = 'Radovic' AND p.birthday = '1992-09-30' AND t.name = 'KK Igokea'
UNION ALL
SELECT 6, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Luka' AND p.surname = 'Nikolic' AND p.birthday = '1997-01-18' AND t.name = 'KK Igokea'
UNION ALL
SELECT 7, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Milos' AND p.surname = 'Dimitrijevic' AND p.birthday = '1995-12-03' AND t.name = 'KK Igokea'
UNION ALL
SELECT 8, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Nemanja' AND p.surname = 'Stankovic' AND p.birthday = '1994-04-25' AND t.name = 'KK Igokea'
UNION ALL
SELECT 9, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Vladimir' AND p.surname = 'Popovic' AND p.birthday = '1993-08-14' AND t.name = 'KK Igokea'
UNION ALL
SELECT 10, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Dusan' AND p.surname = 'Matic' AND p.birthday = '1991-02-07' AND t.name = 'KK Igokea';

-- KK Cedevita Olimpija team_members
INSERT INTO team_member (jersey_number, status, id_player, id_team) 
SELECT 1, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Zoran' AND p.surname = 'Dragic' AND p.birthday = '1994-06-10' AND t.name = 'KK Cedevita Olimpija'
UNION ALL
SELECT 2, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Bojan' AND p.surname = 'Bogdanovic' AND p.birthday = '1993-03-28' AND t.name = 'KK Cedevita Olimpija'
UNION ALL
SELECT 3, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Goran' AND p.surname = 'Novakovic' AND p.birthday = '1995-09-17' AND t.name = 'KK Cedevita Olimpija'
UNION ALL
SELECT 4, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Predrag' AND p.surname = 'Milosevic' AND p.birthday = '1992-12-05' AND t.name = 'KK Cedevita Olimpija'
UNION ALL
SELECT 5, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Vlade' AND p.surname = 'Marjanovic' AND p.birthday = '1990-11-21' AND t.name = 'KK Cedevita Olimpija'
UNION ALL
SELECT 6, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Darko' AND p.surname = 'Radic' AND p.birthday = '1996-04-09' AND t.name = 'KK Cedevita Olimpija'
UNION ALL
SELECT 7, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Igor' AND p.surname = 'Stamenkovic' AND p.birthday = '1994-08-16' AND t.name = 'KK Cedevita Olimpija'
UNION ALL
SELECT 8, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Miodrag' AND p.surname = 'Andric' AND p.birthday = '1995-01-12' AND t.name = 'KK Cedevita Olimpija'
UNION ALL
SELECT 9, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Dejan' AND p.surname = 'Savic' AND p.birthday = '1993-07-30' AND t.name = 'KK Cedevita Olimpija'
UNION ALL
SELECT 10, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Branko' AND p.surname = 'Lazic' AND p.birthday = '1991-10-23' AND t.name = 'KK Cedevita Olimpija';

-- KK Mornar team_members
INSERT INTO team_member (jersey_number, status, id_player, id_team) 
SELECT 1, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Petar' AND p.surname = 'Vukcevic' AND p.birthday = '1995-05-20' AND t.name = 'KK Mornar'
UNION ALL
SELECT 2, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Marko' AND p.surname = 'Simonovic' AND p.birthday = '1994-02-14' AND t.name = 'KK Mornar'
UNION ALL
SELECT 3, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Veljko' AND p.surname = 'Petkovic' AND p.birthday = '1996-08-07' AND t.name = 'KK Mornar'
UNION ALL
SELECT 4, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Danilo' AND p.surname = 'Andjusic' AND p.birthday = '1993-11-19' AND t.name = 'KK Mornar'
UNION ALL
SELECT 5, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Boban' AND p.surname = 'Marjanovic' AND p.birthday = '1988-08-15' AND p.weight = 125 AND t.name = 'KK Mornar'
UNION ALL
SELECT 6, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Filip' AND p.surname = 'Petrusev' AND p.birthday = '1997-12-27' AND t.name = 'KK Mornar'
UNION ALL
SELECT 7, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Aleksa' AND p.surname = 'Avramovic' AND p.birthday = '1995-06-03' AND t.name = 'KK Mornar'
UNION ALL
SELECT 8, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Ognjen' AND p.surname = 'Dobric' AND p.birthday = '1994-10-11' AND t.name = 'KK Mornar'
UNION ALL
SELECT 9, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Uros' AND p.surname = 'Plavsic' AND p.birthday = '1992-04-26' AND t.name = 'KK Mornar'
UNION ALL
SELECT 10, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Nikola' AND p.surname = 'Milutinov' AND p.birthday = '1989-01-08' AND t.name = 'KK Mornar';

-- KK Mega Soccerbet team_members
INSERT INTO team_member (jersey_number, status, id_player, id_team) 
SELECT 1, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Luka' AND p.surname = 'Doncic' AND p.birthday = '1999-02-28' AND t.name = 'KK Mega Soccerbet'
UNION ALL
SELECT 2, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Bogdan' AND p.surname = 'Bogdanovic' AND p.birthday = '1998-08-18' AND t.name = 'KK Mega Soccerbet'
UNION ALL
SELECT 3, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Nikola' AND p.surname = 'Jokic' AND p.birthday = '1999-05-15' AND t.name = 'KK Mega Soccerbet'
UNION ALL
SELECT 4, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Marko' AND p.surname = 'Gudurić' AND p.birthday = '1997-11-30' AND t.name = 'KK Mega Soccerbet'
UNION ALL
SELECT 5, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Alperen' AND p.surname = 'Sengun' AND p.birthday = '2000-07-25' AND t.name = 'KK Mega Soccerbet'
UNION ALL
SELECT 6, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Vasilije' AND p.surname = 'Micic' AND p.birthday = '1998-01-13' AND t.name = 'KK Mega Soccerbet'
UNION ALL
SELECT 7, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Aleksej' AND p.surname = 'Pokusevski' AND p.birthday = '2001-12-26' AND t.name = 'KK Mega Soccerbet'
UNION ALL
SELECT 8, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Stefan' AND p.surname = 'Jovic' AND p.birthday = '1999-09-04' AND t.name = 'KK Mega Soccerbet'
UNION ALL
SELECT 9, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Nikola' AND p.surname = 'Kalinic' AND p.birthday = '1997-03-22' AND t.name = 'KK Mega Soccerbet'
UNION ALL
SELECT 10, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Marko' AND p.surname = 'Simonovic' AND p.birthday = '2000-06-17' AND t.name = 'KK Mega Soccerbet';

-- KK Spartak team_members
INSERT INTO team_member (jersey_number, status, id_player, id_team) 
SELECT 1, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Milos' AND p.surname = 'Teodosic' AND p.birthday = '1987-03-19' AND t.name = 'KK Spartak'
UNION ALL
SELECT 2, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Nemanja' AND p.surname = 'Bjelica' AND p.birthday = '1988-05-09' AND t.name = 'KK Spartak'
UNION ALL
SELECT 3, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Boban' AND p.surname = 'Marjanovic' AND p.birthday = '1988-08-15' AND p.weight = 94 AND t.name = 'KK Spartak'
UNION ALL
SELECT 4, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Miroslav' AND p.surname = 'Raduljica' AND p.birthday = '1988-01-05' AND t.name = 'KK Spartak'
UNION ALL
SELECT 5, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Nenad' AND p.surname = 'Krstic' AND p.birthday = '1983-07-25' AND t.name = 'KK Spartak'
UNION ALL
SELECT 6, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Stefan' AND p.surname = 'Markovic' AND p.birthday = '1994-10-11' AND t.name = 'KK Spartak'
UNION ALL
SELECT 7, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Marko' AND p.surname = 'Keselj' AND p.birthday = '1995-04-28' AND t.name = 'KK Spartak'
UNION ALL
SELECT 8, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Danilo' AND p.surname = 'Ostojic' AND p.birthday = '1996-07-15' AND t.name = 'KK Spartak'
UNION ALL
SELECT 9, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Milenko' AND p.surname = 'Tepic' AND p.birthday = '1993-12-02' AND t.name = 'KK Spartak'
UNION ALL
SELECT 10, 'active', p.id_player, t.id_team FROM player p, team t WHERE p.name = 'Dusan' AND p.surname = 'Ristic' AND p.birthday = '1991-08-20' AND t.name = 'KK Spartak';

-- Insert 5 new matches against the new teams we created
-- Assuming these will be away matches since they are against new teams
INSERT INTO match (name, scheduled_at, type, state, city, hall, is_in_our_hall, transportation_required, accommodation_required, tickets_for_sale, tickets_went_on_sale, id_competition, id_season, id_team) VALUES 
    ('Partizan vs Igokea', '2025-11-15 19:00:00+01', 'away', 'Bosnia and Herzegovina', 'Aleksandrovac', 'Igokea Arena', FALSE, TRUE, TRUE, FALSE, NULL, 1, 1, (SELECT id_team FROM team WHERE name = 'KK Igokea')),
    ('Partizan vs Cedevita Olimpija', '2025-11-22 20:00:00+01', 'away', 'Slovenia', 'Ljubljana', 'Stozice Arena', FALSE, TRUE, TRUE, FALSE, NULL, 1, 1, (SELECT id_team FROM team WHERE name = 'KK Cedevita Olimpija')),
    ('Partizan vs Mornar', '2025-11-29 18:30:00+01', 'away', 'Montenegro', 'Bar', 'Topolica Sports Center', FALSE, TRUE, TRUE, FALSE, NULL, 1, 1, (SELECT id_team FROM team WHERE name = 'KK Mornar')),
    ('Partizan vs Mega Soccerbet', '2025-12-05 19:30:00+01', 'away', 'Serbia', 'Belgrade', 'Mega Factory Hall', FALSE, TRUE, FALSE, FALSE, NULL, 1, 1, (SELECT id_team FROM team WHERE name = 'KK Mega Soccerbet')),
    ('Partizan vs Spartak', '2025-12-12 20:00:00+01', 'away', 'Serbia', 'Subotica', 'Spartak Hall', FALSE, TRUE, FALSE, FALSE, NULL, 1, 1, (SELECT id_team FROM team WHERE name = 'KK Spartak'));

-- Update match_tracking records only for the newly created matches
-- Set them as finished with 0-0 score for testing purposes
UPDATE match_tracking 
SET 
    tracking_status = 'finished',
    period_status = 'finished',
    current_period = 'end',
    start_time = NOW() - INTERVAL '2 hours', -- Started 2 hours ago
    end_time = NOW(), -- Ended now
    last_update_time = NOW(),
    our_points = 0,
    opponent_points = 0,
    elapsed_period_time = 2400000, -- 40 minutes total (4 periods x 10 minutes x 60 seconds x 1000 ms)
    total_pause_time_in_period = 0
WHERE id_match IN (
    SELECT m.id_match 
    FROM match m 
    INNER JOIN team t ON m.id_team = t.id_team 
    WHERE t.name IN ('KK Igokea', 'KK Cedevita Olimpija', 'KK Mornar', 'KK Mega Soccerbet', 'KK Spartak')
    AND m.name LIKE 'Partizan vs %'
);

-- Insert team_member_match records for all new matches
-- Each match needs our players (id 1-10) + opponent team players

-- Match 1: Partizan vs Igokea
-- Our players (Partizan players with id 1-10)
INSERT INTO team_member_match (starting_lineup, in_game, id_team, id_player, id_match)
SELECT 
    CASE WHEN ROW_NUMBER() OVER (ORDER BY p.id_player) <= 5 THEN TRUE ELSE FALSE END, -- First 5 players in starting lineup
    TRUE, -- All players are in game
    1, -- Partizan team id = 1
    p.id_player,
    (SELECT id_match FROM match WHERE name = 'Partizan vs Igokea')
FROM player p 
WHERE p.id_player BETWEEN 1 AND 10;

-- Opponent players (Igokea players)
INSERT INTO team_member_match (starting_lineup, in_game, id_team, id_player, id_match)
SELECT 
    CASE WHEN ROW_NUMBER() OVER (ORDER BY p.id_player) <= 5 THEN TRUE ELSE FALSE END, -- First 5 players in starting lineup
    TRUE, -- All players are in game
    t.id_team,
    p.id_player,
    (SELECT id_match FROM match WHERE name = 'Partizan vs Igokea')
FROM player p 
INNER JOIN team_member tm ON p.id_player = tm.id_player
INNER JOIN team t ON tm.id_team = t.id_team
WHERE t.name = 'KK Igokea';

-- Match 2: Partizan vs Cedevita Olimpija
-- Our players (Partizan players with id 1-10)
INSERT INTO team_member_match (starting_lineup, in_game, id_team, id_player, id_match)
SELECT 
    CASE WHEN ROW_NUMBER() OVER (ORDER BY p.id_player) <= 5 THEN TRUE ELSE FALSE END,
    TRUE,
    1, -- Partizan team id = 1
    p.id_player,
    (SELECT id_match FROM match WHERE name = 'Partizan vs Cedevita Olimpija')
FROM player p 
WHERE p.id_player BETWEEN 1 AND 10;

-- Opponent players (Cedevita Olimpija players)
INSERT INTO team_member_match (starting_lineup, in_game, id_team, id_player, id_match)
SELECT 
    CASE WHEN ROW_NUMBER() OVER (ORDER BY p.id_player) <= 5 THEN TRUE ELSE FALSE END,
    TRUE,
    t.id_team,
    p.id_player,
    (SELECT id_match FROM match WHERE name = 'Partizan vs Cedevita Olimpija')
FROM player p 
INNER JOIN team_member tm ON p.id_player = tm.id_player
INNER JOIN team t ON tm.id_team = t.id_team
WHERE t.name = 'KK Cedevita Olimpija';

-- Match 3: Partizan vs Mornar
-- Our players (Partizan players with id 1-10)
INSERT INTO team_member_match (starting_lineup, in_game, id_team, id_player, id_match)
SELECT 
    CASE WHEN ROW_NUMBER() OVER (ORDER BY p.id_player) <= 5 THEN TRUE ELSE FALSE END,
    TRUE,
    1, -- Partizan team id = 1
    p.id_player,
    (SELECT id_match FROM match WHERE name = 'Partizan vs Mornar')
FROM player p 
WHERE p.id_player BETWEEN 1 AND 10;

-- Opponent players (Mornar players)
INSERT INTO team_member_match (starting_lineup, in_game, id_team, id_player, id_match)
SELECT 
    CASE WHEN ROW_NUMBER() OVER (ORDER BY p.id_player) <= 5 THEN TRUE ELSE FALSE END,
    TRUE,
    t.id_team,
    p.id_player,
    (SELECT id_match FROM match WHERE name = 'Partizan vs Mornar')
FROM player p 
INNER JOIN team_member tm ON p.id_player = tm.id_player
INNER JOIN team t ON tm.id_team = t.id_team
WHERE t.name = 'KK Mornar';

-- Match 4: Partizan vs Mega Soccerbet
-- Our players (Partizan players with id 1-10)
INSERT INTO team_member_match (starting_lineup, in_game, id_team, id_player, id_match)
SELECT 
    CASE WHEN ROW_NUMBER() OVER (ORDER BY p.id_player) <= 5 THEN TRUE ELSE FALSE END,
    TRUE,
    1, -- Partizan team id = 1
    p.id_player,
    (SELECT id_match FROM match WHERE name = 'Partizan vs Mega Soccerbet')
FROM player p 
WHERE p.id_player BETWEEN 1 AND 10;

-- Opponent players (Mega Soccerbet players)
INSERT INTO team_member_match (starting_lineup, in_game, id_team, id_player, id_match)
SELECT 
    CASE WHEN ROW_NUMBER() OVER (ORDER BY p.id_player) <= 5 THEN TRUE ELSE FALSE END,
    TRUE,
    t.id_team,
    p.id_player,
    (SELECT id_match FROM match WHERE name = 'Partizan vs Mega Soccerbet')
FROM player p 
INNER JOIN team_member tm ON p.id_player = tm.id_player
INNER JOIN team t ON tm.id_team = t.id_team
WHERE t.name = 'KK Mega Soccerbet';

-- Match 5: Partizan vs Spartak
-- Our players (Partizan players with id 1-10)
INSERT INTO team_member_match (starting_lineup, in_game, id_team, id_player, id_match)
SELECT 
    CASE WHEN ROW_NUMBER() OVER (ORDER BY p.id_player) <= 5 THEN TRUE ELSE FALSE END,
    TRUE,
    1, -- Partizan team id = 1
    p.id_player,
    (SELECT id_match FROM match WHERE name = 'Partizan vs Spartak')
FROM player p 
WHERE p.id_player BETWEEN 1 AND 10;

-- Opponent players (Spartak players)
INSERT INTO team_member_match (starting_lineup, in_game, id_team, id_player, id_match)
SELECT 
    CASE WHEN ROW_NUMBER() OVER (ORDER BY p.id_player) <= 5 THEN TRUE ELSE FALSE END,
    TRUE,
    t.id_team,
    p.id_player,
    (SELECT id_match FROM match WHERE name = 'Partizan vs Spartak')
FROM player p 
INNER JOIN team_member tm ON p.id_player = tm.id_player
INNER JOIN team t ON tm.id_team = t.id_team
WHERE t.name = 'KK Spartak';

-- Insert 200 personal events for each match (1000 total events)
-- Match 1: Partizan vs Igokea
DO $$ 
DECLARE 
    match_id INTEGER;
    player_ids INTEGER[];
    team_ids INTEGER[];
    i INTEGER;
    current_player_id INTEGER;
    current_team_id INTEGER;
    period_num INTEGER;
    event_types TEXT[] := ARRAY['+2p', '+3p', '+ft', '2p', '3p', 'ft', 'assist', 'reb def', 'reb of', 'steal', 'block', 'foul', 'substitution in', 'substitution out', 'other'];
    event_type TEXT;
    base_time TIMESTAMP := '2025-11-15 19:00:00';
BEGIN
    -- Get match ID
    SELECT id_match INTO match_id FROM match WHERE name = 'Partizan vs Igokea';
    
    -- Get all players and teams for this match
    SELECT ARRAY_AGG(tmm.id_player), ARRAY_AGG(tmm.id_team) 
    INTO player_ids, team_ids
    FROM team_member_match tmm 
    WHERE tmm.id_match = match_id;
    
    -- Insert 200 events
    FOR i IN 1..200 LOOP
        -- Calculate period (1-4, cycling every 50 events)
        period_num := ((i - 1) / 50) + 1;
        IF period_num > 4 THEN period_num := 4; END IF;
        
        -- Select player/team cyclically
        current_player_id := player_ids[((i - 1) % array_length(player_ids, 1)) + 1];
        current_team_id := team_ids[((i - 1) % array_length(team_ids, 1)) + 1];
        
        -- Select event type cyclically
        event_type := event_types[((i - 1) % array_length(event_types, 1)) + 1];
        
        INSERT INTO personal_event (creation_time, notes, type, period, period_time, id_team, id_player, id_match)
        VALUES (
            base_time + INTERVAL '1 second' * (i * 30 + FLOOR(RANDOM() * 100)),
            CASE 
                WHEN event_type IN ('+2p', '+3p', '+ft') THEN 'Successful shot'
                WHEN event_type IN ('2p', '3p', 'ft') THEN 'Missed shot'
                WHEN event_type = 'assist' THEN 'Great pass'
                WHEN event_type = 'foul' THEN 'Personal foul'
                ELSE ''
            END,
            event_type,
            period_num::VARCHAR(20),
            (i * 30000) + FLOOR(RANDOM() * 100000)::INTEGER,
            current_team_id,
            current_player_id,
            match_id
        );
    END LOOP;
END $$;

-- Match 2: Partizan vs Cedevita Olimpija
DO $$ 
DECLARE 
    match_id INTEGER;
    player_ids INTEGER[];
    team_ids INTEGER[];
    i INTEGER;
    current_player_id INTEGER;
    current_team_id INTEGER;
    period_num INTEGER;
    event_types TEXT[] := ARRAY['+2p', '+3p', '+ft', '2p', '3p', 'ft', 'assist', 'reb def', 'reb of', 'steal', 'block', 'foul', 'substitution in', 'substitution out', 'other'];
    event_type TEXT;
    base_time TIMESTAMP := '2025-11-22 20:00:00';
BEGIN
    -- Get match ID
    SELECT id_match INTO match_id FROM match WHERE name = 'Partizan vs Cedevita Olimpija';
    
    -- Get all players and teams for this match
    SELECT ARRAY_AGG(tmm.id_player), ARRAY_AGG(tmm.id_team) 
    INTO player_ids, team_ids
    FROM team_member_match tmm 
    WHERE tmm.id_match = match_id;
    
    -- Insert 200 events
    FOR i IN 1..200 LOOP
        -- Calculate period (1-4, cycling every 50 events)
        period_num := ((i - 1) / 50) + 1;
        IF period_num > 4 THEN period_num := 4; END IF;
        
        -- Select player/team cyclically
        current_player_id := player_ids[((i - 1) % array_length(player_ids, 1)) + 1];
        current_team_id := team_ids[((i - 1) % array_length(team_ids, 1)) + 1];
        
        -- Select event type cyclically
        event_type := event_types[((i - 1) % array_length(event_types, 1)) + 1];
        
        INSERT INTO personal_event (creation_time, notes, type, period, period_time, id_team, id_player, id_match)
        VALUES (
            base_time + INTERVAL '1 second' * (i * 30 + FLOOR(RANDOM() * 100)),
            CASE 
                WHEN event_type IN ('+2p', '+3p', '+ft') THEN 'Successful shot'
                WHEN event_type IN ('2p', '3p', 'ft') THEN 'Missed shot'
                WHEN event_type = 'assist' THEN 'Great pass'
                WHEN event_type = 'foul' THEN 'Personal foul'
                ELSE ''
            END,
            event_type,
            period_num::VARCHAR(20),
            (i * 30000) + FLOOR(RANDOM() * 100000)::INTEGER,
            current_team_id,
            current_player_id,
            match_id
        );
    END LOOP;
END $$;-- Match 3: Partizan vs Mornar
DO $$ 
DECLARE 
    match_id INTEGER;
    player_ids INTEGER[];
    team_ids INTEGER[];
    i INTEGER;
    current_player_id INTEGER;
    current_team_id INTEGER;
    period_num INTEGER;
    event_types TEXT[] := ARRAY['+2p', '+3p', '+ft', '2p', '3p', 'ft', 'assist', 'reb def', 'reb of', 'steal', 'block', 'foul', 'substitution in', 'substitution out', 'other'];
    event_type TEXT;
    base_time TIMESTAMP := '2025-11-29 18:30:00';
BEGIN
    -- Get match ID
    SELECT id_match INTO match_id FROM match WHERE name = 'Partizan vs Mornar';
    
    -- Get all players and teams for this match
    SELECT ARRAY_AGG(tmm.id_player), ARRAY_AGG(tmm.id_team) 
    INTO player_ids, team_ids
    FROM team_member_match tmm 
    WHERE tmm.id_match = match_id;
    
    -- Insert 200 events
    FOR i IN 1..200 LOOP
        -- Calculate period (1-4, cycling every 50 events)
        period_num := ((i - 1) / 50) + 1;
        IF period_num > 4 THEN period_num := 4; END IF;
        
        -- Select player/team cyclically
        current_player_id := player_ids[((i - 1) % array_length(player_ids, 1)) + 1];
        current_team_id := team_ids[((i - 1) % array_length(team_ids, 1)) + 1];
        
        -- Select event type cyclically
        event_type := event_types[((i - 1) % array_length(event_types, 1)) + 1];
        
        INSERT INTO personal_event (creation_time, notes, type, period, period_time, id_team, id_player, id_match)
        VALUES (
            base_time + INTERVAL '1 second' * (i * 30 + FLOOR(RANDOM() * 100)),
            CASE 
                WHEN event_type IN ('+2p', '+3p', '+ft') THEN 'Successful shot'
                WHEN event_type IN ('2p', '3p', 'ft') THEN 'Missed shot'
                WHEN event_type = 'assist' THEN 'Great pass'
                WHEN event_type = 'foul' THEN 'Personal foul'
                ELSE ''
            END,
            event_type,
            period_num::VARCHAR(20),
            (i * 30000) + FLOOR(RANDOM() * 100000)::INTEGER,
            current_team_id,
            current_player_id,
            match_id
        );
    END LOOP;
END $$;

-- Match 4: Partizan vs Mega Soccerbet
DO $$ 
DECLARE 
    match_id INTEGER;
    player_ids INTEGER[];
    team_ids INTEGER[];
    i INTEGER;
    current_player_id INTEGER;
    current_team_id INTEGER;
    period_num INTEGER;
    event_types TEXT[] := ARRAY['+2p', '+3p', '+ft', '2p', '3p', 'ft', 'assist', 'reb def', 'reb of', 'steal', 'block', 'foul', 'substitution in', 'substitution out', 'other'];
    event_type TEXT;
    base_time TIMESTAMP := '2025-12-05 19:30:00';
BEGIN
    -- Get match ID
    SELECT id_match INTO match_id FROM match WHERE name = 'Partizan vs Mega Soccerbet';
    
    -- Get all players and teams for this match
    SELECT ARRAY_AGG(tmm.id_player), ARRAY_AGG(tmm.id_team) 
    INTO player_ids, team_ids
    FROM team_member_match tmm 
    WHERE tmm.id_match = match_id;
    
    -- Insert 200 events
    FOR i IN 1..200 LOOP
        -- Calculate period (1-4, cycling every 50 events)
        period_num := ((i - 1) / 50) + 1;
        IF period_num > 4 THEN period_num := 4; END IF;
        
        -- Select player/team cyclically
        current_player_id := player_ids[((i - 1) % array_length(player_ids, 1)) + 1];
        current_team_id := team_ids[((i - 1) % array_length(team_ids, 1)) + 1];
        
        -- Select event type cyclically
        event_type := event_types[((i - 1) % array_length(event_types, 1)) + 1];
        
        INSERT INTO personal_event (creation_time, notes, type, period, period_time, id_team, id_player, id_match)
        VALUES (
            base_time + INTERVAL '1 second' * (i * 30 + FLOOR(RANDOM() * 100)),
            CASE 
                WHEN event_type IN ('+2p', '+3p', '+ft') THEN 'Successful shot'
                WHEN event_type IN ('2p', '3p', 'ft') THEN 'Missed shot'
                WHEN event_type = 'assist' THEN 'Great pass'
                WHEN event_type = 'foul' THEN 'Personal foul'
                ELSE ''
            END,
            event_type,
            period_num::VARCHAR(20),
            (i * 30000) + FLOOR(RANDOM() * 100000)::INTEGER,
            current_team_id,
            current_player_id,
            match_id
        );
    END LOOP;
END $$;

-- Match 5: Partizan vs Spartak
DO $$ 
DECLARE 
    match_id INTEGER;
    player_ids INTEGER[];
    team_ids INTEGER[];
    i INTEGER;
    current_player_id INTEGER;
    current_team_id INTEGER;
    period_num INTEGER;
    event_types TEXT[] := ARRAY['+2p', '+3p', '+ft', '2p', '3p', 'ft', 'assist', 'reb def', 'reb of', 'steal', 'block', 'foul', 'substitution in', 'substitution out', 'other'];
    event_type TEXT;
    base_time TIMESTAMP := '2025-12-12 20:00:00';
BEGIN
    -- Get match ID
    SELECT id_match INTO match_id FROM match WHERE name = 'Partizan vs Spartak';
    
    -- Get all players and teams for this match
    SELECT ARRAY_AGG(tmm.id_player), ARRAY_AGG(tmm.id_team) 
    INTO player_ids, team_ids
    FROM team_member_match tmm 
    WHERE tmm.id_match = match_id;
    
    -- Insert 200 events
    FOR i IN 1..200 LOOP
        -- Calculate period (1-4, cycling every 50 events)
        period_num := ((i - 1) / 50) + 1;
        IF period_num > 4 THEN period_num := 4; END IF;
        
        -- Select player/team cyclically
        current_player_id := player_ids[((i - 1) % array_length(player_ids, 1)) + 1];
        current_team_id := team_ids[((i - 1) % array_length(team_ids, 1)) + 1];
        
        -- Select event type cyclically
        event_type := event_types[((i - 1) % array_length(event_types, 1)) + 1];
        
        INSERT INTO personal_event (creation_time, notes, type, period, period_time, id_team, id_player, id_match)
        VALUES (
            base_time + INTERVAL '1 second' * (i * 30 + FLOOR(RANDOM() * 100)),
            CASE 
                WHEN event_type IN ('+2p', '+3p', '+ft') THEN 'Successful shot'
                WHEN event_type IN ('2p', '3p', 'ft') THEN 'Missed shot'
                WHEN event_type = 'assist' THEN 'Great pass'
                WHEN event_type = 'foul' THEN 'Personal foul'
                ELSE ''
            END,
            event_type,
            period_num::VARCHAR(20),
            (i * 30000) + FLOOR(RANDOM() * 100000)::INTEGER,
            current_team_id,
            current_player_id,
            match_id
        );
    END LOOP;
END $$;
