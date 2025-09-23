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
})

CREATE (c4:Customer {
    name: 'Milica',
    surname: 'Stojanovic',
    email: 'milica@example.com',
    phone: '+381604444444',
    type: 'customer'
})

CREATE (c5:Customer {
    name: 'Nikola',
    surname: 'Milic',
    email: 'nikola@example.com',
    phone: '+381605555555',
    type: 'customer'
})

CREATE (c6:Customer {
    name: 'Jelena',
    surname: 'Radic',
    email: 'jelena@example.com',
    phone: '+381606666666',
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

// Kupac milica je kupio karte za dva različita meča (premium i srednje sedišta)
MATCH (c4:Customer {email: 'milica@example.com'})
// Premium karte za Partizan vs Crvena Zvezda
MATCH (t10:IndividualTicket)-[:IS_FOR_SEAT]->(s10:Seat {name: 'Seat 50'}), (t10)-[:IS_FOR_MATCH]->(m1:Match {name: 'Partizan vs Crvena Zvezda'})
MATCH (t11:IndividualTicket)-[:IS_FOR_SEAT]->(s11:Seat {name: 'Seat 51'}), (t11)-[:IS_FOR_MATCH]->(m1)
// Srednje karte za Partizan vs FMP
MATCH (t12:IndividualTicket)-[:IS_FOR_SEAT]->(s12:Seat {name: 'Seat 300'}), (t12)-[:IS_FOR_MATCH]->(m2:Match {name: 'Partizan vs FMP'})
MATCH (t13:IndividualTicket)-[:IS_FOR_SEAT]->(s13:Seat {name: 'Seat 301'}), (t13)-[:IS_FOR_MATCH]->(m2)
MATCH (t14:IndividualTicket)-[:IS_FOR_SEAT]->(s14:Seat {name: 'Seat 302'}), (t14)-[:IS_FOR_MATCH]->(m2)
CREATE (c4)-[:BOUGHT {purchased_at: date('2025-08-18'), price: t10.price}]->(t10)
CREATE (c4)-[:BOUGHT {purchased_at: date('2025-08-18'), price: t11.price}]->(t11)
CREATE (c4)-[:BOUGHT {purchased_at: date('2025-08-19'), price: t12.price}]->(t12)
CREATE (c4)-[:BOUGHT {purchased_at: date('2025-08-19'), price: t13.price}]->(t13)
CREATE (c4)-[:BOUGHT {purchased_at: date('2025-08-19'), price: t14.price}]->(t14);

// Kupac nikola je kupio karte za sva tri meča (različite cenovne kategorije)
MATCH (c5:Customer {email: 'nikola@example.com'})
// Jeftina karta za Partizan vs Crvena Zvezda
MATCH (t15:IndividualTicket)-[:IS_FOR_SEAT]->(s15:Seat {name: 'Seat 1500'}), (t15)-[:IS_FOR_MATCH]->(m1:Match {name: 'Partizan vs Crvena Zvezda'})
// Premium karta za Partizan vs FMP
MATCH (t16:IndividualTicket)-[:IS_FOR_SEAT]->(s16:Seat {name: 'Seat 100'}), (t16)-[:IS_FOR_MATCH]->(m2:Match {name: 'Partizan vs FMP'})
// Srednja karta za Partizan vs Mega
MATCH (t17:IndividualTicket)-[:IS_FOR_SEAT]->(s17:Seat {name: 'Seat 800'}), (t17)-[:IS_FOR_MATCH]->(m3:Match {name: 'Partizan vs Mega'})
MATCH (t18:IndividualTicket)-[:IS_FOR_SEAT]->(s18:Seat {name: 'Seat 801'}), (t18)-[:IS_FOR_MATCH]->(m3)
CREATE (c5)-[:BOUGHT {purchased_at: date('2025-08-16'), price: t15.price}]->(t15)
CREATE (c5)-[:BOUGHT {purchased_at: date('2025-08-17'), price: t16.price}]->(t16)
CREATE (c5)-[:BOUGHT {purchased_at: date('2025-08-23'), price: t17.price}]->(t17)
CREATE (c5)-[:BOUGHT {purchased_at: date('2025-08-23'), price: t18.price}]->(t18);

// Kupac jelena je veliki fan - kupio karte za sva tri meča (mix cena)
MATCH (c6:Customer {email: 'jelena@example.com'})
// Mix karata za Partizan vs Crvena Zvezda
MATCH (t19:IndividualTicket)-[:IS_FOR_SEAT]->(s19:Seat {name: 'Seat 80'}), (t19)-[:IS_FOR_MATCH]->(m1:Match {name: 'Partizan vs Crvena Zvezda'})
MATCH (t20:IndividualTicket)-[:IS_FOR_SEAT]->(s20:Seat {name: 'Seat 600'}), (t20)-[:IS_FOR_MATCH]->(m1)
// Premium karte za Partizan vs FMP
MATCH (t21:IndividualTicket)-[:IS_FOR_SEAT]->(s21:Seat {name: 'Seat 25'}), (t21)-[:IS_FOR_MATCH]->(m2:Match {name: 'Partizan vs FMP'})
MATCH (t22:IndividualTicket)-[:IS_FOR_SEAT]->(s22:Seat {name: 'Seat 26'}), (t22)-[:IS_FOR_MATCH]->(m2)
MATCH (t23:IndividualTicket)-[:IS_FOR_SEAT]->(s23:Seat {name: 'Seat 27'}), (t23)-[:IS_FOR_MATCH]->(m2)
// Jeftina karta za Partizan vs Mega
MATCH (t24:IndividualTicket)-[:IS_FOR_SEAT]->(s24:Seat {name: 'Seat 1800'}), (t24)-[:IS_FOR_MATCH]->(m3:Match {name: 'Partizan vs Mega'})
CREATE (c6)-[:BOUGHT {purchased_at: date('2025-08-15'), price: t19.price}]->(t19)
CREATE (c6)-[:BOUGHT {purchased_at: date('2025-08-15'), price: t20.price}]->(t20)
CREATE (c6)-[:BOUGHT {purchased_at: date('2025-08-20'), price: t21.price}]->(t21)
CREATE (c6)-[:BOUGHT {purchased_at: date('2025-08-20'), price: t22.price}]->(t22)
CREATE (c6)-[:BOUGHT {purchased_at: date('2025-08-20'), price: t23.price}]->(t23)
CREATE (c6)-[:BOUGHT {purchased_at: date('2025-08-24'), price: t24.price}]->(t24);