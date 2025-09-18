MATCH (n) DETACH DELETE n;

CREATE INDEX customer_email_index IF NOT EXISTS FOR (c:Customer) ON (c.email);

CREATE (c1:Customer {
    name: 'Marko',
    surname: 'Petrovic',
    email: 'marko@example.com',
    phone: '+381601111111',
    type: 'customer'
})

CREATE (c2:Customer {
    name: 'Ana',
    surname: 'Jovanovic',
    email: 'ana@example.com',
    phone: '+381602222222',
    type: 'customer'
})

CREATE (c3:Customer {
    name: 'Stefan',
    surname: 'Nikolic',
    email: 'stefan@example.com',
    phone: '+381603333333',
    type: 'customer'
});

CREATE (m1:Match {
    name: 'Partizan vs Crvena Zvezda',
    scheduled_at: datetime('2025-09-05T19:00:00+01:00'),
    type: 'home',
    state: 'Serbia',
    city: 'Belgrade',
    hall: 'Stark Arena',
    is_in_our_hall: true
})

CREATE (m2:Match {
    name: 'Partizan vs FMP',
    scheduled_at: datetime('2025-09-15T19:00:00+01:00'),
    type: 'home',
    state: 'Serbia',
    city: 'Belgrade',
    hall: 'Stark Arena',
    is_in_our_hall: true
})

CREATE (m3:Match {
    name: 'Partizan vs Mega',
    scheduled_at: datetime('2025-09-25T19:00:00+01:00'),
    type: 'home',
    state: 'Serbia',
    city: 'Belgrade',
    hall: 'Stark Arena',
    is_in_our_hall: true
});

UNWIND range(1, 160) AS seat_number
WITH seat_number,
     CASE WHEN (seat_number - 1) % 160 < 40 THEN 'north'
          WHEN (seat_number - 1) % 160 < 80 THEN 'east'
          WHEN (seat_number - 1) % 160 < 120 THEN 'south'
          ELSE 'west' END AS direction,
     ((seat_number - 1) % 40) / 8 + 1 AS row,
     (seat_number - 1) % 8 + 1 AS number
CREATE (:Seat {
    name: 'Seat ' + toString(seat_number),
    row: row,
    number: number,
    direction: direction
});

UNWIND range(161, 560) AS seat_number
WITH seat_number,
     CASE WHEN (seat_number - 161) % 100 < 25 THEN 'north'
          WHEN (seat_number - 161) % 100 < 50 THEN 'east'
          WHEN (seat_number - 161) % 100 < 75 THEN 'south'
          ELSE 'west' END AS direction,
     ((seat_number - 161) % 100) / 10 + 1 AS row,
     (seat_number - 161) % 10 + 1 AS number
CREATE (:Seat {
    name: 'Seat ' + toString(seat_number),
    row: row,
    number: number,
    direction: direction
});

UNWIND range(561, 1160) AS seat_number
WITH seat_number,
     CASE WHEN (seat_number - 561) % 150 < 37 THEN 'north'
          WHEN (seat_number - 561) % 150 < 75 THEN 'east'
          WHEN (seat_number - 561) % 150 < 112 THEN 'south'
          ELSE 'west' END AS direction,
     ((seat_number - 561) % 150) / 15 + 1 AS row,
     (seat_number - 561) % 15 + 1 AS number
CREATE (:Seat {
    name: 'Seat ' + toString(seat_number),
    row: row,
    number: number,
    direction: direction
});

UNWIND range(1161, 1960) AS seat_number
WITH seat_number,
     CASE WHEN (seat_number - 1161) % 200 < 50 THEN 'north'
          WHEN (seat_number - 1161) % 200 < 100 THEN 'east'
          WHEN (seat_number - 1161) % 200 < 150 THEN 'south'
          ELSE 'west' END AS direction,
     ((seat_number - 1161) % 200) / 20 + 1 AS row,
     (seat_number - 1161) % 20 + 1 AS number
CREATE (:Seat {
    name: 'Seat ' + toString(seat_number),
    row: row,
    number: number,
    direction: direction
});

// Kreiranje karata za prvi meč (Partizan vs Crvena Zvezda)
MATCH (m:Match {name: 'Partizan vs Crvena Zvezda'})
MATCH (s:Seat)
WITH m, s, toInteger(replace(s.name, 'Seat ', '')) AS seat_number
CREATE (t:IndividualTicket {
    name: 'Individual Ticket - ' + m.name + ' - ' + s.name,
    description: 'Single match ticket for ' + m.name,
    type: 'individual ticket',
    released_at: date('2025-08-15'),
    price: CASE 
        WHEN seat_number <= 160 THEN 4000
        WHEN seat_number <= 560 THEN 3000
        WHEN seat_number <= 1160 THEN 2000
        ELSE 1000
    END
})
CREATE (t)-[:IS_FOR_SEAT]->(s)
CREATE (t)-[:IS_FOR_MATCH]->(m);

// Kreiranje karata za drugi meč (Partizan vs FMP)
MATCH (m:Match {name: 'Partizan vs FMP'})
MATCH (s:Seat)
WITH m, s, toInteger(replace(s.name, 'Seat ', '')) AS seat_number
CREATE (t:IndividualTicket {
    name: 'Individual Ticket - ' + m.name + ' - ' + s.name,
    description: 'Single match ticket for ' + m.name,
    type: 'individual ticket',
    released_at: date('2025-08-15'),
    price: CASE 
        WHEN seat_number <= 160 THEN 2500
        WHEN seat_number <= 560 THEN 2000
        WHEN seat_number <= 1160 THEN 1500
        ELSE 800
    END
})
CREATE (t)-[:IS_FOR_SEAT]->(s)
CREATE (t)-[:IS_FOR_MATCH]->(m);

// Kreiranje karata za treći meč (Partizan vs Mega)
MATCH (m:Match {name: 'Partizan vs Mega'})
MATCH (s:Seat)
WITH m, s, toInteger(replace(s.name, 'Seat ', '')) AS seat_number
CREATE (t:IndividualTicket {
    name: 'Individual Ticket - ' + m.name + ' - ' + s.name,
    description: 'Single match ticket for ' + m.name,
    type: 'individual ticket',
    released_at: date('2025-08-15'),
    price: CASE 
        WHEN seat_number <= 160 THEN 2000
        WHEN seat_number <= 560 THEN 1500
        WHEN seat_number <= 1160 THEN 1200
        ELSE 600
    END
})
CREATE (t)-[:IS_FOR_SEAT]->(s)
CREATE (t)-[:IS_FOR_MATCH]->(m);

// Kupac marko je kupio 4 specificne karte za Partizan vs Crvena Zvezda
MATCH (c1:Customer {email: 'marko@example.com'})
MATCH (t1:IndividualTicket)-[:IS_FOR_SEAT]->(s1:Seat {name: 'Seat 1'}), (t1)-[:IS_FOR_MATCH]->(m1:Match {name: 'Partizan vs Crvena Zvezda'})
MATCH (t2:IndividualTicket)-[:IS_FOR_SEAT]->(s2:Seat {name: 'Seat 2'}), (t2)-[:IS_FOR_MATCH]->(m1)
MATCH (t3:IndividualTicket)-[:IS_FOR_SEAT]->(s3:Seat {name: 'Seat 3'}), (t3)-[:IS_FOR_MATCH]->(m1)
MATCH (t4:IndividualTicket)-[:IS_FOR_SEAT]->(s4:Seat {name: 'Seat 4'}), (t4)-[:IS_FOR_MATCH]->(m1)
CREATE (c1)-[:BOUGHT {purchased_at: date('2025-08-20'), price: t1.price}]->(t1)
CREATE (c1)-[:BOUGHT {purchased_at: date('2025-08-20'), price: t2.price}]->(t2)
CREATE (c1)-[:BOUGHT {purchased_at: date('2025-08-20'), price: t3.price}]->(t3)
CREATE (c1)-[:BOUGHT {purchased_at: date('2025-08-20'), price: t4.price}]->(t4);

// Kupac ana je kupio 3 specificne karte za Partizan vs FMP
MATCH (c2:Customer {email: 'ana@example.com'})
MATCH (t5:IndividualTicket)-[:IS_FOR_SEAT]->(s5:Seat {name: 'Seat 10'}), (t5)-[:IS_FOR_MATCH]->(m2:Match {name: 'Partizan vs FMP'})
MATCH (t6:IndividualTicket)-[:IS_FOR_SEAT]->(s6:Seat {name: 'Seat 11'}), (t6)-[:IS_FOR_MATCH]->(m2)
MATCH (t7:IndividualTicket)-[:IS_FOR_SEAT]->(s7:Seat {name: 'Seat 12'}), (t7)-[:IS_FOR_MATCH]->(m2)
CREATE (c2)-[:BOUGHT {purchased_at: date('2025-08-21'), price: t5.price}]->(t5)
CREATE (c2)-[:BOUGHT {purchased_at: date('2025-08-21'), price: t6.price}]->(t6)
CREATE (c2)-[:BOUGHT {purchased_at: date('2025-08-21'), price: t7.price}]->(t7);

// Kupac stefan je kupio 2 specificne karte za Partizan vs Mega
MATCH (c3:Customer {email: 'stefan@example.com'})
MATCH (t8:IndividualTicket)-[:IS_FOR_SEAT]->(s8:Seat {name: 'Seat 20'}), (t8)-[:IS_FOR_MATCH]->(m3:Match {name: 'Partizan vs Mega'})
MATCH (t9:IndividualTicket)-[:IS_FOR_SEAT]->(s9:Seat {name: 'Seat 21'}), (t9)-[:IS_FOR_MATCH]->(m3)
CREATE (c3)-[:BOUGHT {purchased_at: date('2025-08-22'), price: t8.price}]->(t8)
CREATE (c3)-[:BOUGHT {purchased_at: date('2025-08-22'), price: t9.price}]->(t9);