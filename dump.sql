--
-- EchoShift Game Database Schema
-- PostgreSQL compatible schema for game backend
--

--
-- Table: players
--
CREATE TABLE players (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    username character varying(50) NOT NULL,
    password character varying(255) NOT NULL,
    experience integer DEFAULT 0
);

--
-- Table: player_sessions
--
CREATE TABLE player_sessions (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    session_token character varying(255) NOT NULL,
    player_id uuid NOT NULL,
    is_active boolean DEFAULT true NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    updated_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
);

--
-- Table: runs
--
CREATE TABLE runs (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    player_id uuid NOT NULL,
    time_elapsed double precision DEFAULT 0 NOT NULL,
    score integer DEFAULT 0,
    level_reached integer DEFAULT 1
);

--
-- Primary Keys
--
ALTER TABLE ONLY players
    ADD CONSTRAINT players_pkey PRIMARY KEY (id);

ALTER TABLE ONLY player_sessions
    ADD CONSTRAINT player_sessions_pkey PRIMARY KEY (id);

ALTER TABLE ONLY runs
    ADD CONSTRAINT runs_pkey PRIMARY KEY (id);

--
-- Unique Constraints
--
ALTER TABLE ONLY players
    ADD CONSTRAINT players_username_key UNIQUE (username);

ALTER TABLE ONLY player_sessions
    ADD CONSTRAINT player_sessions_session_token_key UNIQUE (session_token);

--
-- Foreign Keys
--
ALTER TABLE ONLY player_sessions
    ADD CONSTRAINT player_sessions_player_id_fkey 
    FOREIGN KEY (player_id) REFERENCES players(id) ON DELETE CASCADE;

ALTER TABLE ONLY runs
    ADD CONSTRAINT runs_player_id_fkey 
    FOREIGN KEY (player_id) REFERENCES players(id);

