DROP TABLE IF EXISTS accommodation_offer CASCADE;

DROP TABLE IF EXISTS accommodation_request CASCADE;

DROP TABLE IF EXISTS agency CASCADE;

DROP TABLE IF EXISTS automatic_recommendations CASCADE;

DROP TABLE IF EXISTS cart CASCADE;

DROP TABLE IF EXISTS cart_item CASCADE;

DROP TABLE IF EXISTS competition CASCADE;

DROP TABLE IF EXISTS credit_card CASCADE;

DROP TABLE IF EXISTS general_event CASCADE;

DROP TABLE IF EXISTS individual_ticket CASCADE;

DROP TABLE IF EXISTS management CASCADE;

DROP TABLE IF EXISTS management_member_request CASCADE;

DROP TABLE IF EXISTS match CASCADE;

DROP TABLE IF EXISTS match_tracking CASCADE;

DROP TABLE IF EXISTS metric_type CASCADE;

DROP TABLE IF EXISTS metrics CASCADE;

DROP TABLE IF EXISTS nationality CASCADE;

DROP TABLE IF EXISTS offer CASCADE;

DROP TABLE IF EXISTS personal_event CASCADE;

DROP TABLE IF EXISTS physical_metrics CASCADE;

DROP TABLE IF EXISTS player CASCADE;

DROP TABLE IF EXISTS position CASCADE;

DROP TABLE IF EXISTS purchase_offer CASCADE;

DROP TABLE IF EXISTS request CASCADE;

DROP TABLE IF EXISTS season CASCADE;

DROP TABLE IF EXISTS season_metrics CASCADE;

DROP TABLE IF EXISTS season_ticket CASCADE;

DROP TABLE IF EXISTS seat CASCADE;

DROP TABLE IF EXISTS sent_request CASCADE;

DROP TABLE IF EXISTS "session" CASCADE;

DROP TABLE IF EXISTS session_metrics CASCADE;

DROP TABLE IF EXISTS session_status CASCADE;

DROP TABLE IF EXISTS session_type CASCADE;

DROP TABLE IF EXISTS team CASCADE;

DROP TABLE IF EXISTS team_event CASCADE;

DROP TABLE IF EXISTS team_member CASCADE;

DROP TABLE IF EXISTS team_member_match CASCADE;

DROP TABLE IF EXISTS team_member_request CASCADE;

DROP TABLE IF EXISTS ticket_price_parameter CASCADE;

DROP TABLE IF EXISTS transportation_offer CASCADE;

DROP TABLE IF EXISTS transportation_request CASCADE;

DROP TABLE IF EXISTS travel_information CASCADE;

DROP TABLE IF EXISTS trip CASCADE;

DROP TABLE IF EXISTS "users" CASCADE;

DROP TABLE IF EXISTS visa CASCADE;

DROP TABLE IF EXISTS zone CASCADE;

CREATE TABLE accommodation_offer (
    id_offer       INTEGER NOT NULL,
    name           VARCHAR(255),
    capacity       INTEGER,
    type           VARCHAR(20) CHECK (type IN ('hotel', 'house', 'villa')),
    id_agency      INTEGER NOT NULL,
    id_request     INTEGER NOT NULL,
    double_room    BOOLEAN NOT NULL,
    triple_room    BOOLEAN NOT NULL,
    quadruple_room BOOLEAN NOT NULL,
    breakfast      BOOLEAN NOT NULL,
    fitness_center BOOLEAN NOT NULL,
    pool           BOOLEAN NOT NULL,
    wifi           BOOLEAN NOT NULL,
    spa            BOOLEAN NOT NULL,
    PRIMARY KEY (id_offer, id_agency, id_request)
);

CREATE TABLE accommodation_request (
    id_request       SERIAL NOT NULL,
    number_of_guests INTEGER,
    number_of_rooms  INTEGER,
    check_in_date    DATE,
    check_out_date   DATE,
    type             VARCHAR(20) CHECK (type IN ('hotel', 'house', 'villa')),
    PRIMARY KEY (id_request)
);

CREATE TABLE agency (
    id_agency SERIAL NOT NULL,
    name      VARCHAR(255),
    email     VARCHAR(255),
    type      VARCHAR(20) CHECK (type IN ('accommodation', 'transportation')),
    PRIMARY KEY (id_agency)
);

CREATE TABLE automatic_recommendations (
    priority      VARCHAR(20) CHECK (priority IN ('medium priority', 'not priority', 'urgent')),
    status        VARCHAR(20) CHECK (status IN ('accepted', 'rejected')),
    creation_time DATE,
    id_match      INTEGER NOT NULL,
    PRIMARY KEY (id_match)
);

CREATE TABLE cart (
    id_cart         SERIAL NOT NULL,
    created_at      DATE NOT NULL,
    items_number    INTEGER NOT NULL,
    status          VARCHAR(255) NOT NULL CHECK(status in ('created', 'bought')),
    is_current      BOOLEAN NOT NULL,
    id_credit_card  INTEGER,
    id_user         INTEGER NOT NULL,
    PRIMARY KEY (id_cart)
);

CREATE TABLE cart_item (
    id_cart             INTEGER NOT NULL,
    id_purchase_offer   INTEGER NOT NULL,
    added_at            DATE NOT NULL,
    price               NUMERIC(10,2) NOT NULL,
    PRIMARY KEY (id_cart, id_purchase_offer)
);

CREATE TABLE competition (
    id_competition    SERIAL NOT NULL,
    name              VARCHAR(50) NOT NULL,
    started_at        DATE NOT NULL,
    ended_at          DATE,
    number_of_matches INTEGER NOT NULL,
    PRIMARY KEY (id_competition)
);

CREATE TABLE credit_card (
    id_credit_card SERIAL NOT NULL,
    created_at     DATE NOT NULL,
    number         VARCHAR(255) NOT NULL,
    cvv            VARCHAR(255) NOT NULL,
    name           VARCHAR(50) NOT NULL,
    expiration_date DATE NOT NULL,
    id_user        INTEGER NOT NULL,
    PRIMARY KEY (id_credit_card)
);

CREATE TABLE general_event (
    id_event      SERIAL NOT NULL,
    creation_time DATE,
    notes         VARCHAR(255),
    role          VARCHAR(20) CHECK (role IN ('general', 'personal', 'team')),
    type          VARCHAR(20) CHECK (type IN ('break end', 'break start', 'other', 'pause end', 'pause start', 'period end', 'period start')),
    id_match      INTEGER NOT NULL,
    PRIMARY KEY (id_event)
);

CREATE TABLE individual_ticket (
    id_purchase_offer    INTEGER NOT NULL,
    id_match             INTEGER NOT NULL,
    id_individual_ticket SERIAL NOT NULL,
    PRIMARY KEY (id_purchase_offer),
    UNIQUE (id_individual_ticket)
);

CREATE TABLE management (
    member_id      SERIAL NOT NULL,
    member_name    VARCHAR(255),
    member_surname VARCHAR(255),
    member_role    VARCHAR(20) CHECK (member_role IN ('assistant coach', 'coach', 'doctor', 'other', 'therapist')),
    PRIMARY KEY (member_id)
);

CREATE TABLE management_member_request (
    id_request           INTEGER NOT NULL,
    id_management_member INTEGER NOT NULL,
    PRIMARY KEY (id_request, id_management_member)
);

CREATE TABLE match (
    id_match                SERIAL NOT NULL,
    name                    VARCHAR(255) NOT NULL,
    created_at              TIMESTAMP WITH TIME ZONE NOT NULL,
    type                    VARCHAR(20) NOT NULL CHECK (type IN ('away', 'home')),
    state                   VARCHAR(255) NOT NULL,
    city                    VARCHAR(255) NOT NULL,
    hall                    VARCHAR(255) NOT NULL,
    is_in_our_hall          BOOLEAN NOT NULL,
    transportation_required BOOLEAN NOT NULL,
    accommodation_required  BOOLEAN NOT NULL,
    id_competition          INTEGER,
    id_season               INTEGER NOT NULL,
    id_team                 INTEGER NOT NULL,
    PRIMARY KEY (id_match)
);

CREATE TABLE match_tracking (
    start_time                 DATE,
    end_time                   DATE,
    tracking_status            VARCHAR(20) CHECK (tracking_status IN ('active', 'finished', 'preparation')),
    period_duration            INTEGER,
    current_period             VARCHAR(20) CHECK (current_period IN ('1', '2', '3', '4', 'end')),
    period_status              VARCHAR(20) CHECK (period_status IN ('active', 'finished', 'paused')),
    period_start_time          DATE,
    elapsed_period_time        INTEGER,
    last_pause_start_time      DATE,
    total_pause_time_in_period INTEGER,
    last_update_time           DATE,
    our_points                 INTEGER,
    opponent_points            INTEGER,
    id_user                    INTEGER,
    id_match                   INTEGER NOT NULL,
    PRIMARY KEY (id_match)
);

CREATE TABLE metric_type (
    id_type SERIAL NOT NULL,
    type    VARCHAR(255),
    PRIMARY KEY (id_type)
);

CREATE TABLE metrics (
    id_metrics     SERIAL NOT NULL,
    name           VARCHAR(255),
    is_permanent   INTEGER,
    metric_weight  INTEGER,
    id_user        INTEGER NOT NULL UNIQUE,
    id_metric_type INTEGER NOT NULL,
    PRIMARY KEY (id_metrics)
);

CREATE TABLE nationality (
    id_nationality SERIAL NOT NULL,
    state          VARCHAR(255),
    PRIMARY KEY (id_nationality)
);

CREATE TABLE offer (
    id_offer     INTEGER NOT NULL,
    price        INTEGER,
    user_id_user INTEGER,
    id_match     INTEGER NOT NULL,
    id_agency    INTEGER NOT NULL,
    id_request   INTEGER NOT NULL,
    type         VARCHAR(20) CHECK (type IN ('accommodation', 'transportation')),
    PRIMARY KEY (id_offer, id_agency, id_request)
);

CREATE TABLE personal_event (
    id_event      SERIAL NOT NULL,
    creation_time DATE,
    notes         VARCHAR(255),
    role          VARCHAR(20) CHECK (role IN ('general', 'personal', 'team')),
    type          VARCHAR(20) CHECK (type IN ('+2p', '+3p', '+ft', '2p', '3p', 'assist', 'block', 'foul', 'ft', 'other', 'reb def', 'reb of', 'steal', 'substitution in', 'substitution out')),
    id_team       INTEGER NOT NULL,
    id_player     INTEGER NOT NULL,
    id_match      INTEGER NOT NULL,
    PRIMARY KEY (id_event)
);

CREATE TABLE physical_metrics (
    id_physical_metrics SERIAL NOT NULL,
    vertical_jump       INTEGER,
    fat_percentage      INTEGER,
    bench_press_weight  INTEGER,
    squat_weight        INTEGER,
    sprint_speed        INTEGER,
    weight              INTEGER,
    height              INTEGER,
    wingspan            INTEGER,
    date_of_measurement DATE,
    id_player           INTEGER NOT NULL,
    PRIMARY KEY (id_physical_metrics)
);

CREATE TABLE player (
    id_player      SERIAL NOT NULL,
    name           VARCHAR(255),
    surname        VARCHAR(255),
    birthday       DATE,
    id_nationality INTEGER NOT NULL,
    id_position    INTEGER NOT NULL,
    PRIMARY KEY (id_player)
);

CREATE TABLE position (
    id_position SERIAL NOT NULL,
    name        VARCHAR(20),
    PRIMARY KEY (id_position)
);

CREATE TABLE purchase_offer (
    id_purchase_offer SERIAL NOT NULL,
    name              VARCHAR(255) NOT NULL,
    description       VARCHAR(255) NOT NULL,
    type              VARCHAR(50) NOT NULL CHECK (type IN ('individual ticket', 'season ticket')),
    status            VARCHAR(25) NOT NULL CHECK (status IN ('enabled', 'disabled', 'bought')),
    released_at       DATE NOT NULL,
    created_at        DATE NOT NULL,
    expires_at        DATE,
    id_seat           INTEGER NOT NULL,
    PRIMARY KEY (id_purchase_offer)
);

CREATE TABLE request (
    id_request SERIAL NOT NULL,
    state      VARCHAR(255),
    city       VARCHAR(255),
    hall       VARCHAR(255),
    budget     INTEGER,
    id_match   INTEGER NOT NULL,
    type       VARCHAR(20) CHECK (type IN ('accommodation', 'transportation')),
    PRIMARY KEY (id_request)
);

CREATE TABLE season (
    id_season  SERIAL NOT NULL,
    started_at DATE NOT NULL,
    ended_at   DATE,
    name       VARCHAR(255) NOT NULL,
    PRIMARY KEY (id_season)
);

CREATE TABLE season_metrics (
    id_season  INTEGER NOT NULL,
    id_metrics INTEGER NOT NULL,
    PRIMARY KEY (id_season, id_metrics)
);

CREATE TABLE season_ticket (
    id_purchase_offer   INTEGER NOT NULL,
    id_season           INTEGER NOT NULL,
    ticket_price        INTEGER NOT NULL,
    PRIMARY KEY (id_purchase_offer)
);

CREATE TABLE seat (
    id_seat   SERIAL NOT NULL,
    "row"     INTEGER NOT NULL,
    "number"  INTEGER NOT NULL,
    type      VARCHAR(255) NOT NULL,
    direction VARCHAR(255) NOT NULL,
    status    VARCHAR(255) NOT NULL CHECK (status in ('enabled', 'disabled', 'empty')),
    id_zone   INTEGER,
    PRIMARY KEY (id_seat)
);

CREATE TABLE sent_request (
    id_agency  INTEGER NOT NULL,
    id_request INTEGER NOT NULL,
    PRIMARY KEY (id_agency, id_request)
);

CREATE TABLE session (
    id_session        SERIAL NOT NULL,
    start_time        DATE,
    end_time          DATE,
    id_session_status INTEGER NOT NULL,
    id_session_type   INTEGER NOT NULL,
    id_user           INTEGER NOT NULL,
    id_player         INTEGER NOT NULL,
    PRIMARY KEY (id_session)
);

CREATE TABLE session_metrics (
    value      VARCHAR(255),
    id_session INTEGER NOT NULL,
    id_metrics INTEGER NOT NULL,
    PRIMARY KEY (id_metrics, id_session)
);

CREATE TABLE session_status (
    id_status SERIAL NOT NULL,
    status    VARCHAR(255),
    PRIMARY KEY (id_status)
);

CREATE TABLE session_type (
    id_type SERIAL NOT NULL,
    type    VARCHAR(255),
    PRIMARY KEY (id_type)
);

CREATE TABLE team (
    id_team        SERIAL NOT NULL,
    name           VARCHAR(255) NOT NULL,
    state          VARCHAR(255) NOT NULL,
    city           VARCHAR(255) NOT NULL,
    hall           VARCHAR(255) NOT NULL,
    founded_date   DATE,
    coach          VARCHAR(255),
    key_strenghts  VARCHAR(255),
    key_weaknesses VARCHAR(255),
    PRIMARY KEY (id_team)
);

CREATE TABLE team_event (
    id_event      SERIAL NOT NULL,
    creation_time DATE,
    notes         VARCHAR(255),
    role          VARCHAR(20) CHECK (role IN ('general', 'personal', 'team')),
    type          VARCHAR(20) CHECK (type IN ('other', 'technical foul', 'timeout')),
    id_team       INTEGER NOT NULL,
    id_match      INTEGER NOT NULL,
    PRIMARY KEY (id_event)
);

CREATE TABLE team_member (
    jersey_number INTEGER,
    status        VARCHAR(20) CHECK (status IN ('active', 'injured', 'suspended')),
    id_player     INTEGER NOT NULL,
    id_team       INTEGER NOT NULL,
    PRIMARY KEY (id_team, id_player)
);

CREATE TABLE team_member_match (
    starting_lineup BOOLEAN NOT NULL,
    in_game         BOOLEAN NOT NULL,
    id_team         INTEGER NOT NULL,
    id_player       INTEGER NOT NULL,
    id_match        INTEGER NOT NULL,
    PRIMARY KEY (id_match, id_team, id_player)
);

CREATE TABLE team_member_request (
    id_team    INTEGER NOT NULL,
    id_player  INTEGER NOT NULL,
    id_request INTEGER NOT NULL,
    PRIMARY KEY (id_team, id_player, id_request)
);

CREATE TABLE ticket_price_parameter (
    id_ticket_price_parameter SERIAL NOT NULL,
    price_factor              INTEGER NOT NULL,
    time_factor               INTEGER NOT NULL,
    minimum_seat_price        INTEGER NOT NULL,
    maximum_seat_price        INTEGER NOT NULL,
    id_user                   INTEGER NOT NULL,
    id_zone                   INTEGER NOT NULL,
    id_match                  INTEGER NOT NULL,
    PRIMARY KEY (id_ticket_price_parameter)
);

CREATE TABLE transportation_offer (
    id_offer         INTEGER NOT NULL,
    company_name     VARCHAR(255),
    capacity         INTEGER,
    type             VARCHAR(20) CHECK (type IN ('autobus', 'plane', 'train', 'van')),
    id_agency        INTEGER NOT NULL,
    id_request       INTEGER NOT NULL,
    equipment_space  BOOLEAN NOT NULL,
    air_conditioning BOOLEAN NOT NULL,
    tv               BOOLEAN NOT NULL,
    wifi             BOOLEAN NOT NULL,
    restroom         BOOLEAN NOT NULL,
    PRIMARY KEY (id_offer, id_agency, id_request)
);

CREATE TABLE transportation_request (
    id_request           INTEGER NOT NULL,
    number_of_passengers INTEGER,
    start_date           DATE,
    end_date             DATE,
    vehicle_type         VARCHAR(20) CHECK (vehicle_type IN ('autobus', 'plane', 'train', 'van')),
    PRIMARY KEY (id_request)
);

CREATE TABLE travel_information (
    id_travel_information    SERIAL NOT NULL,
    passport_number          VARCHAR(30),
    passport_expiration_date DATE,
    phone                    VARCHAR(20),
    email                    VARCHAR(50),
    role                     VARCHAR(20) CHECK (role IN ('management', 'player')),
    id_management_member     INTEGER,
    id_team                  INTEGER,
    id_player                INTEGER,
    PRIMARY KEY (id_travel_information),
    UNIQUE (id_team, id_player),
    UNIQUE (id_management_member)
);

CREATE TABLE trip (
    id_trip                   SERIAL NOT NULL,
    notes                     VARCHAR(255),
    match_id_match            INTEGER NOT NULL UNIQUE,
    id_accommodation_offer    INTEGER,
    id_transportation_offer   INTEGER,
    id_accommodation_agency   INTEGER,
    id_transportation_agency  INTEGER,
    id_accommodation_request  INTEGER,
    id_transportation_request INTEGER,
    PRIMARY KEY (id_trip),
    UNIQUE (id_accommodation_offer, id_accommodation_agency, id_accommodation_request),
    UNIQUE (id_transportation_offer, id_transportation_agency, id_transportation_request)
);

CREATE TABLE users (
    id_user  SERIAL NOT NULL,
    name     VARCHAR(50) NOT NULL,
    surname  VARCHAR(50) NOT NULL,
    email    VARCHAR(50) NOT NULL,
    phone    VARCHAR(20),
    password VARCHAR(255) NOT NULL,
    type     VARCHAR(50) NOT NULL CHECK (type IN ('customer', 'admin', 'club manager', 'club owner', 'analyst', 'scouting manager', 'team manager')),
    PRIMARY KEY (id_user)
);

CREATE TABLE visa (
    visa_number           VARCHAR(20) NOT NULL,
    state                 VARCHAR(255),
    creation_date         DATE,
    expiration_date       DATE,
    id_travel_information INTEGER NOT NULL,
    PRIMARY KEY (visa_number)
);

CREATE TABLE zone (
    id_zone          SERIAL NOT NULL,
    name             VARCHAR(255) NOT NULL,
    rank             INTEGER NOT NULL,
    maximum_capacity INTEGER NOT NULL,
    status           VARCHAR(255) NOT NULL CHECK (status in ('enabled', 'disabled')),
    PRIMARY KEY (id_zone)
);

-- FOREIGN KEY CONSTRAINTS
ALTER TABLE accommodation_offer
    ADD CONSTRAINT fk_acc_offer_offer
        FOREIGN KEY (id_offer, id_agency, id_request)
        REFERENCES offer (id_offer, id_agency, id_request);

ALTER TABLE accommodation_request
    ADD CONSTRAINT fk_acc_request_request 
        FOREIGN KEY (id_request)
        REFERENCES request (id_request);

ALTER TABLE automatic_recommendations
    ADD CONSTRAINT fk_auto_rec_match_tracking 
        FOREIGN KEY (id_match)
        REFERENCES match_tracking (id_match);

ALTER TABLE cart
    ADD CONSTRAINT fk_cart_credit_card 
        FOREIGN KEY (id_credit_card)
        REFERENCES credit_card (id_credit_card);

ALTER TABLE cart_item
    ADD CONSTRAINT fk_cart_item_cart 
        FOREIGN KEY (id_cart)
        REFERENCES cart (id_cart);

ALTER TABLE cart_item
    ADD CONSTRAINT fk_cart_item_purchase_offer 
        FOREIGN KEY (id_purchase_offer)
        REFERENCES purchase_offer (id_purchase_offer);

ALTER TABLE cart
    ADD CONSTRAINT fk_cart_user 
        FOREIGN KEY (id_user)
        REFERENCES users (id_user);

ALTER TABLE credit_card
    ADD CONSTRAINT fk_credit_card_user 
        FOREIGN KEY (id_user)
        REFERENCES users (id_user);

ALTER TABLE general_event
    ADD CONSTRAINT fk_general_event_match_tracking 
        FOREIGN KEY (id_match)
        REFERENCES match_tracking (id_match);

ALTER TABLE individual_ticket
    ADD CONSTRAINT fk_individual_ticket_match 
        FOREIGN KEY (id_match)
        REFERENCES match (id_match);

ALTER TABLE management_member_request
    ADD CONSTRAINT fk_mgmt_mem_req_management 
        FOREIGN KEY (id_management_member)
        REFERENCES management (member_id);

ALTER TABLE management_member_request
    ADD CONSTRAINT fk_mgmt_mem_req_request 
        FOREIGN KEY (id_request)
        REFERENCES request (id_request);

ALTER TABLE match
    ADD CONSTRAINT fk_match_competition 
        FOREIGN KEY (id_competition)
        REFERENCES competition (id_competition);

ALTER TABLE match
    ADD CONSTRAINT fk_match_season 
        FOREIGN KEY (id_season)
        REFERENCES season (id_season);

ALTER TABLE match
    ADD CONSTRAINT fk_match_team 
        FOREIGN KEY (id_team)
        REFERENCES team (id_team);

ALTER TABLE match_tracking
    ADD CONSTRAINT fk_match_tracking_match 
        FOREIGN KEY (id_match)
        REFERENCES match (id_match);

ALTER TABLE match_tracking
    ADD CONSTRAINT fk_match_tracking_user 
        FOREIGN KEY (id_user)
        REFERENCES users (id_user);

ALTER TABLE metrics
    ADD CONSTRAINT fk_metrics_metric_type 
        FOREIGN KEY (id_metric_type)
        REFERENCES metric_type (id_type);

ALTER TABLE metrics
    ADD CONSTRAINT fk_metrics_user 
        FOREIGN KEY (id_user)
        REFERENCES users (id_user);

ALTER TABLE offer
    ADD CONSTRAINT fk_offer_match 
        FOREIGN KEY (id_match)
        REFERENCES match (id_match);

ALTER TABLE offer
    ADD CONSTRAINT fk_offer_sent_request
        FOREIGN KEY (id_agency, id_request)
        REFERENCES sent_request (id_agency, id_request);

ALTER TABLE offer
    ADD CONSTRAINT fk_offer_user 
        FOREIGN KEY (user_id_user)
        REFERENCES users (id_user);

ALTER TABLE personal_event
    ADD CONSTRAINT fk_personal_event_match_tracking 
        FOREIGN KEY (id_match)
        REFERENCES match_tracking (id_match);

ALTER TABLE personal_event
    ADD CONSTRAINT fk_personal_event_team_member
        FOREIGN KEY (id_team, id_player)
        REFERENCES team_member (id_team, id_player);

ALTER TABLE physical_metrics
    ADD CONSTRAINT fk_physical_metrics_player 
        FOREIGN KEY (id_player)
        REFERENCES player (id_player);

ALTER TABLE player
    ADD CONSTRAINT fk_player_nationality 
        FOREIGN KEY (id_nationality)
        REFERENCES nationality (id_nationality);

ALTER TABLE player
    ADD CONSTRAINT fk_player_position 
        FOREIGN KEY (id_position)
        REFERENCES position (id_position);

ALTER TABLE purchase_offer
    ADD CONSTRAINT fk_purchase_offer_seat 
        FOREIGN KEY (id_seat)
        REFERENCES seat (id_seat);

ALTER TABLE request
    ADD CONSTRAINT fk_request_match 
        FOREIGN KEY (id_match)
        REFERENCES match (id_match);

ALTER TABLE season_metrics
    ADD CONSTRAINT fk_season_metrics_metrics 
        FOREIGN KEY (id_metrics)
        REFERENCES metrics (id_metrics);

ALTER TABLE season_metrics
    ADD CONSTRAINT fk_season_metrics_season 
        FOREIGN KEY (id_season)
        REFERENCES season (id_season);

ALTER TABLE season_ticket
    ADD CONSTRAINT fk_season_ticket_season 
        FOREIGN KEY (id_season)
        REFERENCES season (id_season);

ALTER TABLE seat
    ADD CONSTRAINT fk_seat_zone 
        FOREIGN KEY (id_zone)
        REFERENCES zone (id_zone);

ALTER TABLE sent_request
    ADD CONSTRAINT fk_sent_request_agency 
        FOREIGN KEY (id_agency)
        REFERENCES agency (id_agency);

ALTER TABLE sent_request
    ADD CONSTRAINT fk_sent_request_request 
        FOREIGN KEY (id_request)
        REFERENCES request (id_request);

ALTER TABLE session_metrics
    ADD CONSTRAINT fk_session_metrics_metrics 
        FOREIGN KEY (id_metrics)
        REFERENCES metrics (id_metrics);

ALTER TABLE session_metrics
    ADD CONSTRAINT fk_session_metrics_session 
        FOREIGN KEY (id_session)
        REFERENCES session (id_session);

ALTER TABLE session
    ADD CONSTRAINT fk_session_player 
        FOREIGN KEY (id_player)
        REFERENCES player (id_player);

ALTER TABLE session
    ADD CONSTRAINT fk_session_session_status 
        FOREIGN KEY (id_session_status)
        REFERENCES session_status (id_status);

ALTER TABLE session
    ADD CONSTRAINT fk_session_session_type 
        FOREIGN KEY (id_session_type)
        REFERENCES session_type (id_type);

ALTER TABLE session
    ADD CONSTRAINT fk_session_user 
        FOREIGN KEY (id_user)
        REFERENCES users (id_user);

ALTER TABLE team_event
    ADD CONSTRAINT fk_team_event_match_tracking 
        FOREIGN KEY (id_match)
        REFERENCES match_tracking (id_match);

ALTER TABLE team_event
    ADD CONSTRAINT fk_team_event_team 
        FOREIGN KEY (id_team)
        REFERENCES team (id_team);

ALTER TABLE team_member_match
    ADD CONSTRAINT fk_team_mem_match_match_tracking 
        FOREIGN KEY (id_match)
        REFERENCES match_tracking (id_match);

ALTER TABLE team_member_match
    ADD CONSTRAINT fk_team_mem_match_team_member
        FOREIGN KEY (id_team, id_player)
        REFERENCES team_member (id_team, id_player);

ALTER TABLE team_member
    ADD CONSTRAINT fk_team_member_player 
        FOREIGN KEY (id_player)
        REFERENCES player (id_player);

ALTER TABLE team_member_request
    ADD CONSTRAINT fk_team_mem_req_request 
        FOREIGN KEY (id_request)
        REFERENCES request (id_request);

ALTER TABLE team_member_request
    ADD CONSTRAINT fk_team_mem_req_team_member
        FOREIGN KEY (id_team, id_player)
        REFERENCES team_member (id_team, id_player);

ALTER TABLE team_member
    ADD CONSTRAINT fk_team_member_team 
        FOREIGN KEY (id_team)
        REFERENCES team (id_team);

ALTER TABLE ticket_price_parameter
    ADD CONSTRAINT fk_ticket_price_param_match 
        FOREIGN KEY (id_match)
        REFERENCES match (id_match);

ALTER TABLE ticket_price_parameter
    ADD CONSTRAINT fk_ticket_price_param_user 
        FOREIGN KEY (id_user)
        REFERENCES users (id_user);

ALTER TABLE ticket_price_parameter
    ADD CONSTRAINT fk_ticket_price_param_zone 
        FOREIGN KEY (id_zone)
        REFERENCES zone (id_zone);

ALTER TABLE transportation_offer
    ADD CONSTRAINT fk_trans_offer_offer
        FOREIGN KEY (id_offer, id_agency, id_request)
        REFERENCES offer (id_offer, id_agency, id_request);

ALTER TABLE transportation_request
    ADD CONSTRAINT fk_trans_request_request 
        FOREIGN KEY (id_request)
        REFERENCES request (id_request);

ALTER TABLE travel_information
    ADD CONSTRAINT fk_travel_info_management 
        FOREIGN KEY (id_management_member)
        REFERENCES management (member_id);

ALTER TABLE travel_information
    ADD CONSTRAINT fk_travel_info_team_member
        FOREIGN KEY (id_team, id_player)
        REFERENCES team_member (id_team, id_player);

ALTER TABLE trip
    ADD CONSTRAINT fk_trip_accommodation_offer
        FOREIGN KEY (id_accommodation_offer, id_accommodation_agency, id_accommodation_request)
        REFERENCES accommodation_offer (id_offer, id_agency, id_request);

ALTER TABLE trip
    ADD CONSTRAINT fk_trip_match 
        FOREIGN KEY (match_id_match)
        REFERENCES match (id_match);

ALTER TABLE trip
    ADD CONSTRAINT fk_trip_transportation_offer
        FOREIGN KEY (id_transportation_offer, id_transportation_agency, id_transportation_request)
        REFERENCES transportation_offer (id_offer, id_agency, id_request);

ALTER TABLE visa
    ADD CONSTRAINT fk_visa_travel_information 
        FOREIGN KEY (id_travel_information)
        REFERENCES travel_information (id_travel_information);

-- NENAD GVOZDENAC TRIGGER #1 - Kada se kreira kupac, kreira se i korpa
CREATE OR REPLACE FUNCTION create_cart_for_customer()
RETURNS TRIGGER AS $$
BEGIN
    IF NEW.type = 'customer' THEN
        INSERT INTO cart (created_at, items_number, status, is_current, id_user)
        VALUES (CURRENT_DATE, 0, 'created', TRUE, NEW.id_user);
    END IF;
    
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_create_cart_for_customer
    AFTER INSERT ON users
    FOR EACH ROW
    EXECUTE FUNCTION create_cart_for_customer();

-- KRAJ TRIGGERA NENAD GVOZDENAC #1

-- NENAD GVOZDENAC TRIGGER #2 - Kada se status korpe postane 'bought', kreira se nova korpa za korisnika
CREATE OR REPLACE FUNCTION create_new_cart_on_purchase()
RETURNS TRIGGER AS $$
BEGIN
    IF OLD.status != 'bought' AND NEW.status = 'bought' THEN
        UPDATE cart 
        SET is_current = FALSE 
        WHERE id_cart = NEW.id_cart;
        
        INSERT INTO cart (created_at, items_number, status, is_current, id_user)
        VALUES (CURRENT_DATE, 0, 'created', TRUE, NEW.id_user);
    END IF;
    
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_create_new_cart_on_purchase
    AFTER UPDATE ON cart
    FOR EACH ROW
    EXECUTE FUNCTION create_new_cart_on_purchase();

-- KRAJ TRIGGERA NENAD GVOZDENAC #2

-- NENAD GVOZDENAC TRIGGER #3 - Kada se doda ili obrise novi element u korpi, poveca se item count.
CREATE OR REPLACE FUNCTION update_cart_items_count()
RETURNS TRIGGER AS $$
BEGIN
    IF TG_OP = 'INSERT' THEN
        UPDATE cart 
        SET items_number = items_number + 1 
        WHERE id_cart = NEW.id_cart;
        
        RETURN NEW;
    ELSIF TG_OP = 'DELETE' THEN
        UPDATE cart 
        SET items_number = items_number - 1 
        WHERE id_cart = OLD.id_cart;
        
        RETURN OLD;
    END IF;
    
    RETURN NULL;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_cart_item_insert
    AFTER INSERT ON cart_item
    FOR EACH ROW
    EXECUTE FUNCTION update_cart_items_count();

CREATE TRIGGER trigger_cart_item_delete
    AFTER DELETE ON cart_item
    FOR EACH ROW
    EXECUTE FUNCTION update_cart_items_count();

-- KRAJ TRIGGERA NENAD GVOZDENAC #3

-- DML DATA INSERTION

-- Insert Nationality for Serbian players
INSERT INTO nationality (state) VALUES ('Serbia');

-- Insert Position data
INSERT INTO position (name) VALUES 
    ('Point Guard'),
    ('Shooting Guard'),
    ('Small Forward'),
    ('Power Forward'),
    ('Center');

-- Insert current season (2024/25)
INSERT INTO season (started_at, name) VALUES 
    ('2024-09-01', '2024/25 Season');

-- Insert Partizan team
INSERT INTO team (name, state, city, hall, founded_date, coach, key_strenghts, key_weaknesses) VALUES 
    ('KK Partizan', 'Serbia', 'Belgrade', 'Stark Arena', '1945-10-04', 'Zeljko Obradovic', 'Strong defense, experienced players', 'Young bench, inconsistent offense');

-- Insert opponent teams
INSERT INTO team (name, state, city, hall, founded_date, coach, key_strenghts, key_weaknesses) VALUES 
    ('KK Crvena Zvezda', 'Serbia', 'Belgrade', 'Aleksandar Nikolic Hall', '1945-03-03', 'Ioannis Sfairopoulos', 'Fast tempo, good shooters', 'Weak rebounding'),
    ('KK FMP', 'Serbia', 'Belgrade', 'FMP Hall', '1991-01-01', 'Marko Jaric', 'Young talent, energy', 'Lack of experience'),
    ('KK Mega', 'Serbia', 'Belgrade', 'Mega Factory Hall', '2006-01-01', 'Vladimir Jovanovic', 'Athletic players', 'Poor defense');

-- Insert competition
INSERT INTO competition (name, started_at, number_of_matches) VALUES 
    ('ABA Liga 2024/25', '2024-09-15', 30);

-- Insert Zone 400
INSERT INTO zone (name, rank, maximum_capacity, status) VALUES 
    ('Zone 400', 400, 60, 'enabled');

-- Insert 3 Partizan matches
INSERT INTO match (name, created_at, type, state, city, hall, is_in_our_hall, transportation_required, accommodation_required, id_competition, id_season, id_team) VALUES 
    ('Partizan vs Crvena Zvezda', '2024-09-01', 'home', 'Serbia', 'Belgrade', 'Stark Arena', TRUE, FALSE, FALSE, 1, 1, 2),
    ('Partizan vs FMP', '2024-09-15', 'home', 'Serbia', 'Belgrade', 'Stark Arena', TRUE, FALSE, FALSE, 1, 1, 3),
    ('Partizan vs Mega', '2024-10-01', 'home', 'Serbia', 'Belgrade', 'Stark Arena', TRUE, FALSE, FALSE, 1, 1, 4);

-- Insert 50+ seats for Zone 400 (10 rows x 6 seats = 60 seats)
INSERT INTO seat ("row", "number", type, direction, status, id_zone) 
SELECT 
    row_num,
    seat_num,
    'standard',
    'north',
    'enabled',
    1
FROM generate_series(1, 10) AS row_num,
     generate_series(1, 6) AS seat_num;

-- Insert season tickets for all seats in zone 400
INSERT INTO purchase_offer (name, description, type, status, released_at, created_at, expires_at, id_seat)
SELECT 
    'Season Ticket - Zone 400 Row ' || s."row" || ' Seat ' || s."number",
    'Full season access to Zone 400, Row ' || s."row" || ', Seat ' || s."number",
    'season ticket',
    'enabled',
    '2024-08-01',
    '2024-08-01',
    '2025-06-30',
    s.id_seat
FROM seat s 
WHERE s.id_zone = 1;

-- Insert season ticket pricing
INSERT INTO season_ticket (id_purchase_offer, id_season, ticket_price)
SELECT 
    po.id_purchase_offer,
    1,
    15000  -- Price in dinars
FROM purchase_offer po
WHERE po.type = 'season ticket';

-- Insert individual tickets for all seats for all 3 matches
INSERT INTO purchase_offer (name, description, type, status, released_at, created_at, expires_at, id_seat)
SELECT 
    'Individual Ticket - ' || m.name || ' - Zone 400 Row ' || s."row" || ' Seat ' || s."number",
    'Single match ticket for ' || m.name || ' in Zone 400, Row ' || s."row" || ', Seat ' || s."number",
    'individual ticket',
    'enabled',
    '2024-08-15',
    '2024-08-15',
    m.created_at + INTERVAL '30 days',
    s.id_seat
FROM seat s 
CROSS JOIN match m
WHERE s.id_zone = 1 AND m.id_match IN (1, 2, 3);

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