package models

type User struct {
	ID       uint   `json:"id" db:"id_user"`
	Name     string `json:"name" db:"name"`
	Surname  string `json:"surname" db:"surname"`
	Email    string `json:"email" db:"email"`
	Phone    string `json:"phone" db:"phone"`
	Password string `json:"password" db:"password"`
	UserType string `json:"user_type" db:"type"`
}
