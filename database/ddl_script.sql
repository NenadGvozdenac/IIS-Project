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

DROP TABLE IF EXISTS match_zone_sales_summary CASCADE;

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
    creation_time TIMESTAMP WITH TIME ZONE NOT NULL,
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
    valid_from          DATE,
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
    creation_time TIMESTAMP WITH TIME ZONE NOT NULL,
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
    scheduled_at            TIMESTAMP WITH TIME ZONE NOT NULL,
    type                    VARCHAR(20) NOT NULL CHECK (type IN ('away', 'home')),
    state                   VARCHAR(255) NOT NULL,
    city                    VARCHAR(255) NOT NULL,
    hall                    VARCHAR(255) NOT NULL,
    is_in_our_hall          BOOLEAN NOT NULL,
    tickets_for_sale        BOOLEAN NOT NULL DEFAULT FALSE,
    tickets_went_on_sale    TIMESTAMP WITH TIME ZONE DEFAULT NULL,
    transportation_required BOOLEAN NOT NULL,
    accommodation_required  BOOLEAN NOT NULL,
    id_competition          INTEGER,
    id_season               INTEGER NOT NULL,
    id_team                 INTEGER NOT NULL,
    PRIMARY KEY (id_match)
);

CREATE TABLE match_tracking (
    start_time                 TIMESTAMP WITH TIME ZONE,
    end_time                   TIMESTAMP WITH TIME ZONE,
    tracking_status            VARCHAR(20) CHECK (tracking_status IN ('active', 'finished', 'preparation', 'upcoming')),
    period_duration            INTEGER,
    current_period             VARCHAR(20) CHECK (current_period IN ('1', '2', '3', '4', 'end')),
    period_status              VARCHAR(20) CHECK (period_status IN ('active', 'finished', 'paused')),
    period_start_time          TIMESTAMP WITH TIME ZONE,
    elapsed_period_time        INTEGER,
    last_pause_start_time      TIMESTAMP WITH TIME ZONE,
    total_pause_time_in_period INTEGER,
    last_update_time           TIMESTAMP WITH TIME ZONE,
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
    creation_time TIMESTAMP WITH TIME ZONE NOT NULL,
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
    weight              INTEGER,
    height              INTEGER,
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
    name       VARCHAR(255) NOT NULL,
    started_at DATE NOT NULL,
    ended_at   DATE,
    tickets_for_sale        BOOLEAN NOT NULL DEFAULT FALSE,
    tickets_went_on_sale    TIMESTAMP WITH TIME ZONE DEFAULT NULL,
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
    direction VARCHAR(255) NOT NULL CHECK (direction in ('east', 'north', 'south', 'west')),
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
    playing_style  VARCHAR(255),
    key_strengths  VARCHAR(255),
    key_weaknesses VARCHAR(255),
    PRIMARY KEY (id_team)
);

CREATE TABLE team_event (
    id_event      SERIAL NOT NULL,
    creation_time TIMESTAMP WITH TIME ZONE NOT NULL,
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

CREATE TABLE match_zone_sales_summary (
    id_summary       SERIAL NOT NULL,
    id_match         INTEGER NOT NULL,
    id_zone          INTEGER NOT NULL,
    id_ticket_price_parameter INTEGER,
    total_tickets_sold INTEGER NOT NULL DEFAULT 0,
    total_revenue    DECIMAL(12,2) NOT NULL DEFAULT 0.00,
    created_at       TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id_summary),
    UNIQUE (id_match, id_zone)
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
        REFERENCES purchase_offer (id_purchase_offer) ON DELETE CASCADE;

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

ALTER TABLE individual_ticket
    ADD CONSTRAINT fk_individual_ticket_purchase_offer 
        FOREIGN KEY (id_purchase_offer)
        REFERENCES purchase_offer (id_purchase_offer) ON DELETE CASCADE;

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
        REFERENCES season (id_season) ON DELETE CASCADE;

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
        REFERENCES seat (id_seat) ON DELETE CASCADE;

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
        REFERENCES season (id_season) ON DELETE CASCADE;

ALTER TABLE season_ticket
    ADD CONSTRAINT fk_season_ticket_purchase_offer 
        FOREIGN KEY (id_purchase_offer)
        REFERENCES purchase_offer (id_purchase_offer) ON DELETE CASCADE;

ALTER TABLE seat
    ADD CONSTRAINT fk_seat_zone 
        FOREIGN KEY (id_zone)
        REFERENCES zone (id_zone) ON DELETE CASCADE;

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

ALTER TABLE match_zone_sales_summary
    ADD CONSTRAINT fk_match_zone_sales_match 
        FOREIGN KEY (id_match)
        REFERENCES match (id_match) ON DELETE CASCADE;

ALTER TABLE match_zone_sales_summary
    ADD CONSTRAINT fk_match_zone_sales_zone 
        FOREIGN KEY (id_zone)
        REFERENCES zone (id_zone) ON DELETE CASCADE;

ALTER TABLE match_zone_sales_summary
    ADD CONSTRAINT fk_match_zone_sales_price_param 
        FOREIGN KEY (id_ticket_price_parameter)
        REFERENCES ticket_price_parameter (id_ticket_price_parameter) ON DELETE SET NULL;

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

-- SRDJAN ILIC TRIGGER #4 - Automatsko kreiranje MatchTracking-a kada se kreira Match
CREATE OR REPLACE FUNCTION create_match_tracking_for_match()
RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO match_tracking (
        start_time,
        end_time,
        tracking_status,
        period_duration,
        current_period,
        period_status,
        period_start_time,
        elapsed_period_time,
        last_pause_start_time,
        total_pause_time_in_period,
        last_update_time,
        our_points,
        opponent_points,
        id_user,
        id_match
    ) VALUES (
        NULL,                                    -- start_time
        NULL,                                    -- end_time (initially set to scheduled time)
        'upcoming',                              -- tracking_status
        600,                                     -- period_duration (600 seconds default)
        '1',                                     -- current_period
        'active',                                -- period_status
        NULL,                                    -- period_start_time
        0,                                       -- elapsed_period_time
        NULL,                                    -- last_pause_start_time
        0,                                       -- total_pause_time_in_period
        NULL,                                    -- last_update_time
        0,                                       -- our_points
        0,                                       -- opponent_points
        NULL,                                    -- id_user
        NEW.id_match                             -- id_match
    );
    
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_create_match_tracking_for_match
    AFTER INSERT ON match
    FOR EACH ROW
    EXECUTE FUNCTION create_match_tracking_for_match();

-- KRAJ TRIGGERA #4

-- TRIGGER #5 - Set valid_from for cart items when cart is purchased
-- For individual tickets: valid_from = NULL (always valid)
-- For season tickets: valid_from = day after last conflicting individual ticket OR purchase date
CREATE OR REPLACE FUNCTION set_cart_item_valid_from()
RETURNS TRIGGER AS $$
DECLARE
    item_record RECORD;
    latest_match_date DATE;
BEGIN
    -- Only process when cart status changes to 'bought'
    IF OLD.status != 'bought' AND NEW.status = 'bought' THEN
        -- Process each cart item
        FOR item_record IN 
            SELECT ci.id_cart, ci.id_purchase_offer, po.type, po.id_seat
            FROM cart_item ci
            JOIN purchase_offer po ON ci.id_purchase_offer = po.id_purchase_offer
            WHERE ci.id_cart = NEW.id_cart
        LOOP
            -- If it's an individual ticket, leave valid_from as NULL (always valid)
            IF item_record.type = 'individual ticket' THEN
                UPDATE cart_item 
                SET valid_from = NULL 
                WHERE id_cart = item_record.id_cart 
                AND id_purchase_offer = item_record.id_purchase_offer;
            
            -- If it's a season ticket, calculate when it becomes valid
            ELSIF item_record.type = 'season ticket' THEN
                -- Find the latest match date for this seat that has individual tickets already bought
                SELECT MAX(m.scheduled_at::DATE)
                INTO latest_match_date
                FROM match m
                JOIN individual_ticket it ON it.id_match = m.id_match
                JOIN purchase_offer po_individual ON po_individual.id_purchase_offer = it.id_purchase_offer
                WHERE po_individual.id_seat = item_record.id_seat
                AND po_individual.type = 'individual ticket'
                AND po_individual.status = 'bought'
                AND m.scheduled_at > CURRENT_TIMESTAMP;
                
                -- Season ticket becomes valid after the last individual ticket expires
                IF latest_match_date IS NOT NULL THEN
                    UPDATE cart_item 
                    SET valid_from = latest_match_date + INTERVAL '1 day'
                    WHERE id_cart = item_record.id_cart 
                    AND id_purchase_offer = item_record.id_purchase_offer;
                ELSE
                    -- No conflicting individual tickets, season ticket is valid immediately
                    UPDATE cart_item 
                    SET valid_from = CURRENT_DATE
                    WHERE id_cart = item_record.id_cart 
                    AND id_purchase_offer = item_record.id_purchase_offer;
                END IF;
                
                -- Season tickets coexist with individual tickets
                -- Individual tickets remain valid for their specific matches
                -- Season tickets become valid after individual tickets expire
            END IF;
        END LOOP;
    END IF;
    
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_set_cart_item_valid_from
    AFTER UPDATE ON cart
    FOR EACH ROW
    EXECUTE FUNCTION set_cart_item_valid_from();

-- KRAJ TRIGGERA #5

-- DML DATA INSERTION

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

-- FUNKCIJA ZA DINAMIČKO IZRAČUNAVANJE CENE KARATA
-- Implementira formulu: P_zona(t) = [P_min + (P_max - P_min) * ((e^(α*O_zona/K_zona + β*O_stad/C_stad) - 1) / (e^(α + β) - 1))] * w_vreme
-- gde su: α = F * k, β = (1 - F) * k
-- w_vreme = 1 + γ * (1 - (D_utakmice - D_trenutno) / T)

CREATE OR REPLACE FUNCTION calculate_ticket_price(
    p_match_id INTEGER,
    p_zone_id INTEGER
) RETURNS NUMERIC AS $$
DECLARE
    -- Parametri iz tabele ticket_price_parameter
    v_price_factor NUMERIC;
    v_time_factor NUMERIC;
    v_min_price NUMERIC;
    v_max_price NUMERIC;
    
    -- Podaci o zoni i stadionu
    v_zone_occupied INTEGER := 0;
    v_zone_capacity INTEGER;
    v_stadium_occupied INTEGER := 0;
    v_stadium_capacity INTEGER;
    
    -- Datum utakmice
    v_match_date DATE;
    v_current_date DATE := CURRENT_DATE;
    v_days_difference INTEGER;
    
    -- Faktori za formulu
    v_f NUMERIC; -- Ponder između zone i stadiona (0-1)
    v_k NUMERIC; -- Globalni parametar strmine (preporučeno 5)
    v_alpha NUMERIC;
    v_beta NUMERIC;
    v_gamma NUMERIC; -- Vremenski faktor multiplikator (preporučeno 0.5)
    v_t NUMERIC; -- Maksimalni broj dana za vremenski faktor (preporučeno 30)
    
    -- Rezultati izračuna
    v_zone_ratio NUMERIC;
    v_stadium_ratio NUMERIC;
    v_exponential_factor NUMERIC;
    v_base_price NUMERIC;
    v_time_weight NUMERIC;
    v_final_price NUMERIC;
    
BEGIN
    -- Dohvatanje parametara cena iz tabele
    SELECT 
        tpp.price_factor,
        tpp.time_factor,
        tpp.minimum_seat_price,
        tpp.maximum_seat_price
    INTO 
        v_price_factor,
        v_time_factor,
        v_min_price,
        v_max_price
    FROM ticket_price_parameter tpp
    WHERE tpp.id_match = p_match_id 
      AND tpp.id_zone = p_zone_id
    LIMIT 1;
    
    -- Ako nema parametara, vrati osnovnu cenu
    IF v_min_price IS NULL THEN
        RETURN 1000; -- Osnovna cena od 1000 dinara
    END IF;
    
    -- Dohvatanje kapaciteta zone
    SELECT z.maximum_capacity
    INTO v_zone_capacity
    FROM zone z
    WHERE z.id_zone = p_zone_id;
    
    -- Izračunavanje zauzetih mesta u zoni
    SELECT COUNT(*)
    INTO v_zone_occupied
    FROM cart_item ci
    JOIN purchase_offer po ON ci.id_purchase_offer = po.id_purchase_offer
    JOIN seat s ON po.id_seat = s.id_seat
    JOIN individual_ticket it ON po.id_purchase_offer = it.id_purchase_offer
    WHERE it.id_match = p_match_id 
      AND s.id_zone = p_zone_id
      AND po.status = 'bought';
    
    -- Izračunavanje ukupnog kapaciteta stadiona
    SELECT SUM(z.maximum_capacity)
    INTO v_stadium_capacity
    FROM zone z
    WHERE z.status = 'enabled';
    
    -- Izračunavanje ukupno zauzetih mesta u stadionu za taj meč
    SELECT COUNT(*)
    INTO v_stadium_occupied
    FROM cart_item ci
    JOIN purchase_offer po ON ci.id_purchase_offer = po.id_purchase_offer
    JOIN individual_ticket it ON po.id_purchase_offer = it.id_purchase_offer
    WHERE it.id_match = p_match_id
      AND po.status = 'bought';
    
    -- Dohvatanje datuma utakmice
    SELECT m.scheduled_at
    INTO v_match_date
    FROM match m
    WHERE m.id_match = p_match_id;
    
    -- Postavljanje faktora (mogu se prebaciti u tabelu parametara)
    v_f := 0.6; -- 60% uticaj zone, 40% stadiona
    v_k := COALESCE(v_price_factor::NUMERIC / 100.0, 8.0); -- Iz tabele ili default 8
    v_gamma := COALESCE(v_time_factor::NUMERIC / 100.0, 0.7); -- Iz tabele ili default 0.7
    v_t := 30; -- 30 dana maksimalno za vremenski faktor
    
    -- Izračunavanje α i β
    v_alpha := v_f * v_k;
    v_beta := (1 - v_f) * v_k;
    
    -- Izračunavanje odnosa zauzetosti
    v_zone_ratio := CASE 
        WHEN v_zone_capacity > 0 THEN v_zone_occupied::NUMERIC / v_zone_capacity::NUMERIC
        ELSE 0
    END;
    
    v_stadium_ratio := CASE 
        WHEN v_stadium_capacity > 0 THEN v_stadium_occupied::NUMERIC / v_stadium_capacity::NUMERIC
        ELSE 0
    END;
    
    -- Izračunavanje eksponencijalnog faktora
    -- (e^(α*O_zona/K_zona + β*O_stad/C_stad) - 1) / (e^(α + β) - 1)
    v_exponential_factor := (
        exp(v_alpha * v_zone_ratio + v_beta * v_stadium_ratio) - 1
    ) / (
        exp(v_alpha + v_beta) - 1
    );
    
    -- Izračunavanje osnovne cene zone
    v_base_price := v_min_price + (v_max_price - v_min_price) * v_exponential_factor;
    
    -- Izračunavanje vremenskog faktora
    v_days_difference := v_match_date - v_current_date;
    v_time_weight := 1 + v_gamma * (1 - v_days_difference::NUMERIC / v_t);
    
    -- Osiguravanje da vremenski faktor ne bude manji od 0.5
    v_time_weight := GREATEST(v_time_weight, 0.5);
    
    -- Finalna cena
    v_final_price := v_base_price * v_time_weight;
    
    -- Osiguravanje da cena ne prelazi maksimum i nije manja od minimuma
    v_final_price := GREATEST(LEAST(v_final_price, v_max_price * 2), v_min_price * 0.5);
    
    RETURN ROUND(v_final_price, 2);
    
EXCEPTION
    WHEN OTHERS THEN
        -- U slučaju greške, vrati osnovnu cenu
        RETURN COALESCE(v_min_price, 1000);
END;
$$ LANGUAGE plpgsql;

-- PRIMER KORIŠĆENJA DINAMIČKIH CENA:
-- Danas je 2. septembar 2025
-- 
-- Match 1: Partizan vs Crvena Zvezda (5. septembar - za 3 dana)
-- - Derby utakmica, visok price_factor (8.0), time_factor (0.7)
-- - Min: 2000, Max: 8000 dinara
-- SELECT calculate_ticket_price(1, 1); 
--
-- Match 2: Partizan vs FMP (15. septembar - za 13 dana)  
-- - Srednji demand, price_factor (5.0), time_factor (0.5)
-- - Min: 1200, Max: 5000 dinara
-- SELECT calculate_ticket_price(2, 1);
--
-- Match 3: Partizan vs Mega (25. septembar - za 23 dana)
-- - Obična utakmica, nizak price_factor (4.0), time_factor (0.4) 
-- - Min: 1000, Max: 4000 dinara
-- SELECT calculate_ticket_price(3, 1);

-- TRIGGER FUNKCIJA ZA AGREGACIJU PRODAJE KARATA PO ZONAMA
CREATE OR REPLACE FUNCTION aggregate_match_zone_sales()
RETURNS TRIGGER AS $$
BEGIN
    -- Proverava da li je tickets_for_sale promenjeno sa TRUE na FALSE
    -- što označava da je prodaja karata završena za meč
    IF OLD.tickets_for_sale = TRUE AND NEW.tickets_for_sale = FALSE THEN

        -- Briše postojeće podatke za ovaj meč ako postoje
        DELETE FROM match_zone_sales_summary WHERE id_match = NEW.id_match;

        -- Kreira unose za sve zone sa parametrima cena
        INSERT INTO match_zone_sales_summary (id_match, id_zone, id_ticket_price_parameter, total_tickets_sold, total_revenue)
        SELECT 
            NEW.id_match,
            z.id_zone,
            tpp.id_ticket_price_parameter,
            0 as total_tickets_sold,
            0.00 as total_revenue
        FROM zone z
        LEFT JOIN ticket_price_parameter tpp ON tpp.id_zone = z.id_zone AND tpp.id_match = NEW.id_match
        WHERE z.status = 'enabled'
        ORDER BY z.id_zone;

        -- Priprema podatke o prodaji po zonama
        WITH zone_sales AS (
            SELECT 
                s.id_zone,
                COUNT(*) as ticket_count,
                SUM(ci.price) as revenue
            FROM cart_item ci
            JOIN purchase_offer po ON ci.id_purchase_offer = po.id_purchase_offer
            JOIN individual_ticket it ON po.id_purchase_offer = it.id_purchase_offer
            JOIN seat s ON po.id_seat = s.id_seat
            JOIN cart c ON ci.id_cart = c.id_cart
            WHERE it.id_match = NEW.id_match 
              AND c.status = 'bought'
              AND po.type = 'individual ticket'
            GROUP BY s.id_zone
        )
        -- Ažurira podatke o prodaji za zone koje imaju prodane karte
        UPDATE match_zone_sales_summary 
        SET 
            total_tickets_sold = zone_sales.ticket_count,
            total_revenue = zone_sales.revenue
        FROM zone_sales
        WHERE match_zone_sales_summary.id_match = NEW.id_match 
          AND match_zone_sales_summary.id_zone = zone_sales.id_zone;
        
    END IF;
    
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- TRIGGER ZA AUTOMATSKU AGREGACIJU KADA SE PRODAJA KARATA ZAVRŠI
CREATE TRIGGER match_finished_aggregation_trigger
    AFTER UPDATE ON match
    FOR EACH ROW
    EXECUTE FUNCTION aggregate_match_zone_sales();
