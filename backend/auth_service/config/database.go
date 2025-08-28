package config

import (
	"database/sql"
	"fmt"

	_ "github.com/lib/pq"
)

var DB *sql.DB

func ConnectDatabase() error {
	host := "localhost"
	user := "postgres"
	password := "postgres"
	dbname := "sportsdb"
	port := "5432"

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
