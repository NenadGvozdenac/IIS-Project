package repositories

import (
	"auth_service/internal/domain/interfaces"
	"auth_service/internal/domain/models"
	"database/sql"
	"time"
)

type UserRepository struct {
	db *sql.DB
}

func NewUserRepository(db *sql.DB) interfaces.UserRepositoryInterface {
	return &UserRepository{db: db}
}

func (r *UserRepository) Create(user *models.User) (*models.User, error) {
	query := `
		INSERT INTO korisnici (ime_korisnika, prezime_korisnika, email_korisnika, telefon_korisnika, sifra_korisnika, tip_korisnika, created_at, updated_at)
		VALUES ($1, $2, $3, $4, $5, $6, $7, $8)
		RETURNING id_korisnika, created_at, updated_at
	`

	now := time.Now()
	var id uint
	var createdAt, updatedAt time.Time

	err := r.db.QueryRow(query, user.Name, user.Surname, user.Email, user.Phone, user.Password, user.UserType, now, now).
		Scan(&id, &createdAt, &updatedAt)

	if err != nil {
		return nil, err
	}

	user.ID = id

	return user, nil
}

func (r *UserRepository) GetByEmail(email string) (*models.User, error) {
	query := `
		SELECT id_korisnika, ime_korisnika, prezime_korisnika, email_korisnika, telefon_korisnika, sifra_korisnika, tip_korisnika, created_at, updated_at
		FROM korisnici 
		WHERE email_korisnika = $1
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
