MATCH (n) DETACH DELETE n;

CREATE CONSTRAINT customer_id_unique IF NOT EXISTS FOR (c:Customer) REQUIRE c.id IS UNIQUE;
CREATE CONSTRAINT match_id_unique IF NOT EXISTS FOR (m:Match) REQUIRE m.id IS UNIQUE;
CREATE CONSTRAINT seat_id_unique IF NOT EXISTS FOR (s:Seat) REQUIRE s.id IS UNIQUE;
CREATE CONSTRAINT ticket_id_unique IF NOT EXISTS FOR (t:IndividualTicket) REQUIRE t.id IS UNIQUE;

CREATE INDEX customer_email_index IF NOT EXISTS FOR (c:Customer) ON (c.email);
CREATE INDEX match_scheduled_at_index IF NOT EXISTS FOR (m:Match) ON (m.scheduled_at);
CREATE INDEX seat_zone_index IF NOT EXISTS FOR (s:Seat) ON (s.zone_id);

CREATE (c1:Customer {
    id: 7,
    name: 'Marko',
    surname: 'Petrovic',
    email: 'marko@example.com',
    phone: '+381601111111',
    type: 'customer'
})

CREATE (c2:Customer {
    id: 8,
    name: 'Ana',
    surname: 'Jovanovic',
    email: 'ana@example.com',
    phone: '+381602222222',
    type: 'customer'
})

CREATE (c3:Customer {
    id: 9,
    name: 'Stefan',
    surname: 'Nikolic',
    email: 'stefan@example.com',
    phone: '+381603333333',
    type: 'customer'
});

CREATE (m1:Match {
    id: 1,
    name: 'Partizan vs Crvena Zvezda',
    scheduled_at: datetime('2025-09-05T19:00:00+01:00'),
    type: 'home',
    state: 'Serbia',
    city: 'Belgrade',
    hall: 'Stark Arena',
    is_in_our_hall: true,
    transportation_required: false,
    accommodation_required: false,
    tickets_for_sale: true,
    tickets_went_on_sale: datetime('2025-09-05T19:00:00+01:00'),
    id_competition: 1,
    id_season: 1,
    id_team: 2
})

CREATE (m2:Match {
    id: 2,
    name: 'Partizan vs FMP',
    scheduled_at: datetime('2025-09-15T19:00:00+01:00'),
    type: 'home',
    state: 'Serbia',
    city: 'Belgrade',
    hall: 'Stark Arena',
    is_in_our_hall: true,
    transportation_required: false,
    accommodation_required: false,
    tickets_for_sale: true,
    tickets_went_on_sale: datetime('2025-09-15T19:00:00+01:00'),
    id_competition: 1,
    id_season: 1,
    id_team: 3
})

CREATE (m3:Match {
    id: 3,
    name: 'Partizan vs Mega',
    scheduled_at: datetime('2025-09-25T19:00:00+01:00'),
    type: 'home',
    state: 'Serbia',
    city: 'Belgrade',
    hall: 'Stark Arena',
    is_in_our_hall: true,
    transportation_required: false,
    accommodation_required: false,
    tickets_for_sale: true,
    tickets_went_on_sale: datetime('2025-09-25T19:00:00+01:00'),
    id_competition: 1,
    id_season: 1,
    id_team: 4
});

UNWIND range(1, 160) AS seat_id
WITH seat_id,
     CASE WHEN (seat_id - 1) % 160 < 40 THEN 'north'
          WHEN (seat_id - 1) % 160 < 80 THEN 'east'
          WHEN (seat_id - 1) % 160 < 120 THEN 'south'
          ELSE 'west' END AS direction,
     ((seat_id - 1) % 40) / 8 + 1 AS row,
     (seat_id - 1) % 8 + 1 AS number
CREATE (:Seat {
    id: seat_id,
    row: row,
    number: number,
    type: 'standard',
    direction: direction,
    status: 'enabled',
    zone_id: 1,
    zone_name: 'Zone 100',
    zone_rank: 100
});

UNWIND range(161, 560) AS seat_id
WITH seat_id,
     CASE WHEN (seat_id - 161) % 100 < 25 THEN 'north'
          WHEN (seat_id - 161) % 100 < 50 THEN 'east'
          WHEN (seat_id - 161) % 100 < 75 THEN 'south'
          ELSE 'west' END AS direction,
     ((seat_id - 161) % 100) / 10 + 1 AS row,
     (seat_id - 161) % 10 + 1 AS number
CREATE (:Seat {
    id: seat_id,
    row: row,
    number: number,
    type: 'standard',
    direction: direction,
    status: 'enabled',
    zone_id: 2,
    zone_name: 'Zone 200',
    zone_rank: 200
});

UNWIND range(561, 1160) AS seat_id
WITH seat_id,
     CASE WHEN (seat_id - 561) % 150 < 37 THEN 'north'
          WHEN (seat_id - 561) % 150 < 75 THEN 'east'
          WHEN (seat_id - 561) % 150 < 112 THEN 'south'
          ELSE 'west' END AS direction,
     ((seat_id - 561) % 150) / 15 + 1 AS row,
     (seat_id - 561) % 15 + 1 AS number
CREATE (:Seat {
    id: seat_id,
    row: row,
    number: number,
    type: 'standard',
    direction: direction,
    status: 'enabled',
    zone_id: 3,
    zone_name: 'Zone 300',
    zone_rank: 300
});

UNWIND range(1161, 1960) AS seat_id
WITH seat_id,
     CASE WHEN (seat_id - 1161) % 200 < 50 THEN 'north'
          WHEN (seat_id - 1161) % 200 < 100 THEN 'east'
          WHEN (seat_id - 1161) % 200 < 150 THEN 'south'
          ELSE 'west' END AS direction,
     ((seat_id - 1161) % 200) / 20 + 1 AS row,
     (seat_id - 1161) % 20 + 1 AS number
CREATE (:Seat {
    id: seat_id,
    row: row,
    number: number,
    type: 'standard',
    direction: direction,
    status: 'enabled',
    zone_id: 4,
    zone_name: 'Zone 400',
    zone_rank: 400
});

UNWIND range(1, 1960) AS seat_id
WITH seat_id, (seat_id - 1) * 3 + 1 AS ticket_id
CREATE (:IndividualTicket {
    id: ticket_id,
    purchase_offer_id: ticket_id + 1960, // Offset to avoid conflicts with season tickets
    match_id: 1,
    seat_id: seat_id,
    name: 'Individual Ticket - Partizan vs Crvena Zvezda - Seat ' + toString(seat_id),
    description: 'Single match ticket for Partizan vs Crvena Zvezda',
    type: 'individual ticket',
    status: CASE WHEN ticket_id <= 10 THEN 'bought' ELSE 'enabled' END, // First 10 tickets are sold
    released_at: date('2025-08-15'),
    created_at: date('2025-08-15'),
    expires_at: date('2025-09-05'),
    price: CASE 
        WHEN seat_id <= 160 THEN 4000    // Zone 100 - Premium
        WHEN seat_id <= 560 THEN 3000    // Zone 200 - VIP
        WHEN seat_id <= 1160 THEN 2000   // Zone 300 - Standard
        ELSE 1000                        // Zone 400 - Economy
    END
});

UNWIND range(1, 1960) AS seat_id
WITH seat_id, (seat_id - 1) * 3 + 2 AS ticket_id
CREATE (:IndividualTicket {
    id: ticket_id,
    purchase_offer_id: ticket_id + 1960,
    match_id: 2,
    seat_id: seat_id,
    name: 'Individual Ticket - Partizan vs FMP - Seat ' + toString(seat_id),
    description: 'Single match ticket for Partizan vs FMP',
    type: 'individual ticket',
    status: 'enabled',
    released_at: date('2025-08-15'),
    created_at: date('2025-08-15'),
    expires_at: date('2025-09-15'),
    price: CASE 
        WHEN seat_id <= 160 THEN 2500    // Zone 100 - Premium
        WHEN seat_id <= 560 THEN 2000    // Zone 200 - VIP
        WHEN seat_id <= 1160 THEN 1500   // Zone 300 - Standard
        ELSE 800                         // Zone 400 - Economy
    END
});

UNWIND range(1, 1960) AS seat_id
WITH seat_id, (seat_id - 1) * 3 + 3 AS ticket_id
CREATE (:IndividualTicket {
    id: ticket_id,
    purchase_offer_id: ticket_id + 1960,
    match_id: 3,
    seat_id: seat_id,
    name: 'Individual Ticket - Partizan vs Mega - Seat ' + toString(seat_id),
    description: 'Single match ticket for Partizan vs Mega',
    type: 'individual ticket',
    status: 'enabled',
    released_at: date('2025-08-15'),
    created_at: date('2025-08-15'),
    expires_at: date('2025-09-25'),
    price: CASE 
        WHEN seat_id <= 160 THEN 2000    // Zone 100 - Premium
        WHEN seat_id <= 560 THEN 1500    // Zone 200 - VIP
        WHEN seat_id <= 1160 THEN 1200   // Zone 300 - Standard
        ELSE 600                         // Zone 400 - Economy
    END
});

MATCH (c1:Customer {id: 7}), (t:IndividualTicket {match_id: 1}) 
WHERE t.id IN [1, 4, 7, 10] // Customer 1 bought 4 tickets
CREATE (c1)-[:BOUGHT {purchased_at: date('2025-08-20'), price: t.price}]->(t);

MATCH (c2:Customer {id: 8}), (t:IndividualTicket {match_id: 1})
WHERE t.id IN [2, 5, 8] // Customer 2 bought 3 tickets
CREATE (c2)-[:BOUGHT {purchased_at: date('2025-08-20'), price: t.price}]->(t);

MATCH (c3:Customer {id: 9}), (t:IndividualTicket {match_id: 1})
WHERE t.id IN [3, 6, 9] // Customer 3 bought 3 tickets
CREATE (c3)-[:BOUGHT {purchased_at: date('2025-08-20'), price: t.price}]->(t);

MATCH (t:IndividualTicket), (s:Seat)
WHERE t.seat_id = s.id
CREATE (t)-[:IS_FOR_SEAT]->(s);

MATCH (t:IndividualTicket), (m:Match)
WHERE t.match_id = m.id
CREATE (t)-[:IS_FOR_MATCH]->(m);