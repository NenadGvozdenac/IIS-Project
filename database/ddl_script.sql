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
    double_room    INTEGER,
    triple_room    INTEGER,
    quadruple_room INTEGER,
    breakfast      INTEGER,
    fitness_center INTEGER,
    pool           INTEGER,
    wifi           INTEGER,
    spa            INTEGER,
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
    id_cart        SERIAL NOT NULL,
    created_at     DATE,
    items_number   INTEGER,
    status         VARCHAR(255),
    id_credit_card INTEGER,
    id_user        INTEGER NOT NULL,
    PRIMARY KEY (id_cart)
);

CREATE TABLE cart_item (
    id_cart           INTEGER NOT NULL,
    id_purchase_offer INTEGER NOT NULL,
    added_at          DATE,
    quantity          INTEGER,
    PRIMARY KEY (id_cart, id_purchase_offer)
);

CREATE TABLE competition (
    id_competition    SERIAL NOT NULL,
    name              VARCHAR(50),
    started_at        DATE,
    ended_at          DATE,
    number_of_matches INTEGER,
    PRIMARY KEY (id_competition)
);

CREATE TABLE credit_card (
    id_credit_card SERIAL NOT NULL,
    created_at     DATE,
    number         VARCHAR(16),
    cvv            INTEGER,
    name           VARCHAR(50),
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
    id_individual_ticket INTEGER NOT NULL,
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
    name                    VARCHAR(255),
    created_at              DATE,
    type                    VARCHAR(20) CHECK (type IN ('away', 'home')),
    state                   VARCHAR(255),
    city                    VARCHAR(255),
    hall                    VARCHAR(255),
    is_in_our_hall          INTEGER,
    transportation_required INTEGER,
    accommodation_required  INTEGER,
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
    id_user                    INTEGER UNIQUE,
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
    name              VARCHAR(255),
    description       VARCHAR(255),
    type              VARCHAR(50) CHECK (type IN ('individual ticket', 'season ticket')),
    status            VARCHAR(255),
    released_at       DATE,
    created_at        DATE,
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
    started_at DATE,
    ended_at   DATE,
    name       VARCHAR(255),
    PRIMARY KEY (id_season)
);

CREATE TABLE season_metrics (
    id_season  INTEGER NOT NULL,
    id_metrics INTEGER NOT NULL,
    PRIMARY KEY (id_season, id_metrics)
);

CREATE TABLE season_ticket (
    id_purchase_offer              INTEGER NOT NULL,
    id_season                      INTEGER NOT NULL,
    fixed_promotional_ticket_price INTEGER,
    PRIMARY KEY (id_purchase_offer)
);

CREATE TABLE seat (
    id_seat   SERIAL NOT NULL,
    "row"     INTEGER,
    "number"  INTEGER,
    type      VARCHAR(255),
    direction VARCHAR(255),
    status    VARCHAR(255),
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
    name           VARCHAR(255),
    state          VARCHAR(255),
    city           VARCHAR(255),
    hall           VARCHAR(255),
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
    captain       INTEGER,
    status        VARCHAR(20) CHECK (status IN ('active', 'injured', 'suspended')),
    id_player     INTEGER NOT NULL,
    id_team       INTEGER NOT NULL,
    PRIMARY KEY (id_team, id_player)
);

CREATE TABLE team_member_match (
    starting_lineup INTEGER,
    in_game         INTEGER,
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
    price_factor              INTEGER,
    time_factor               INTEGER,
    minimum_seat_price        INTEGER,
    maximum_seat_price        INTEGER,
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
    equipment_space  INTEGER,
    air_conditioning INTEGER,
    tv               INTEGER,
    wifi             INTEGER,
    restroom         INTEGER,
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
    nationality              VARCHAR(255),
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
    name     VARCHAR(50),
    surname  VARCHAR(50),
    email    VARCHAR(50),
    phone    VARCHAR(20),
    password VARCHAR(255),
    type     VARCHAR(50),
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
    name             VARCHAR(255),
    rank             INTEGER,
    maximum_capacity INTEGER,
    status           VARCHAR(255),
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
