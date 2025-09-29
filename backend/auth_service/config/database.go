package config

import (
	"database/sql"
	"fmt"
	"os"

	_ "github.com/lib/pq"
)

var DB *sql.DB

// Configuration for external services
var (
	TicketServiceURL string
)

func init() {
	// Initialize external service URLs
	TicketServiceURL = getEnv("TICKET_SERVICE_URL", "http://localhost:8001")
}

func ConnectDatabase() error {
	host := getEnv("DB_HOST", "localhost")
	user := getEnv("DB_USER", "postgres")
	password := getEnv("DB_PASSWORD", "postgres")
	dbname := getEnv("DB_NAME", "sportsdb")
	port := getEnv("DB_PORT", "5432")

	// Construct the connection string
	dsn := fmt.Sprintf("host=%s user=%s password=%s dbname=%s port=%s sslmode=disable",
		host, user, password, dbname, port)

	// Open the connection to the database
	var err error
	DB, err = sql.Open("postgres", dsn)
	if err != nil {
		return err
	}

	// Test the connection
	if err = DB.Ping(); err != nil {
		return err
	}

	return nil
}

func getEnv(key, defaultValue string) string {
	if value := os.Getenv(key); value != "" {
		return value
	}
	return defaultValue
}
