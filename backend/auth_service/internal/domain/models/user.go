package models

type User struct {
	ID       uint   `json:"id_korisnika" db:"id_korisnika"`
	Name     string `json:"ime_korisnika" db:"ime_korisnika"`
	Surname  string `json:"prezime_korisnika" db:"prezime_korisnika"`
	Email    string `json:"email_korisnika" db:"email_korisnika"`
	Phone    string `json:"telefon_korisnika" db:"telefon_korisnika"`
	Password string `json:"sifra_korisnika" db:"sifra_korisnika"`
	UserType string `json:"tip_korisnika" db:"tip_korisnika"`
}
