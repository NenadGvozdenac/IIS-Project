package repositories

import (
	"auth_service/internal/domain/interfaces"
	"auth_service/internal/domain/models"
	"database/sql"
)

type UserRepository struct {
	db *sql.DB
}

func NewUserRepository(db *sql.DB) interfaces.UserRepositoryInterface {
	return &UserRepository{db: db}
}

func (r *UserRepository) Create(user *models.User) (*models.User, error) {
	query := `
		INSERT INTO "User" (name, surname, email, phone, password, type)
		VALUES ($1, $2, $3, $4, $5, $6)
		RETURNING id_user
	`

	var id uint
	err := r.db.QueryRow(query, user.Name, user.Surname, user.Email, user.Phone, user.Password, user.UserType).
		Scan(&id)

	if err != nil {
		return nil, err
	}

	user.ID = id

	return user, nil
}

func (r *UserRepository) GetByEmail(email string) (*models.User, error) {
	query := `
		SELECT id_user, name, surname, email, phone, password, type
		FROM "User" 
		WHERE email = $1
	`

	user := &models.User{}
	err := r.db.QueryRow(query, email).Scan(
		&user.ID,
		&user.Name,
		&user.Surname,
		&user.Email,
		&user.Phone,
		&user.Password,
		&user.UserType,
	)

	if err != nil {
		if err == sql.ErrNoRows {
			return nil, nil
		}
		return nil, err
	}

	return user, nil
}
