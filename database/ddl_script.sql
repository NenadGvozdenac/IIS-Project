-- Initial PostgreSQL database setup script
-- Created for IIS Project microservices architecture

-- Drop table if exists (for recreation)
DROP TABLE IF EXISTS "User" CASCADE;

-- Create User table
CREATE TABLE "User" (
    id_user  SERIAL PRIMARY KEY,
    name     VARCHAR(20),
    surname  VARCHAR(20),
    email    VARCHAR(50) UNIQUE,
    phone    VARCHAR(20),
    password VARCHAR(255),
    type     VARCHAR(50)
);

-- Create indexes for better performance
CREATE INDEX idx_user_email ON "User"(email);
CREATE INDEX idx_user_type ON "User"(type);
