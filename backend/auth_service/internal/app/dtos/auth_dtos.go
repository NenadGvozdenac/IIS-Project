package dtos

// DTO from the client to register a new user
type RegisterDTO struct {
	Name            string `json:"ime_korisnika" binding:"required"`
	Surname         string `json:"prezime_korisnika" binding:"required"`
	Email           string `json:"email_korisnika" binding:"required,email"`
	Phone           string `json:"telefon_korisnika" binding:"required"`
	Password        string `json:"sifra_korisnika" binding:"required"`
	ConfirmPassword string `json:"potvrda_sifre" binding:"required,eqfield=Password"`
	UserType        string `json:"tip_korisnika" binding:"required"`
}

// DTO from the client to login a user
type LoginDTO struct {
	Email    string `json:"email_korisnika" binding:"required,email"`
	Password string `json:"sifra_korisnika" binding:"required"`
}

// DTO to return a JWT to the client
type TokenDTO struct {
	Token string `json:"token"`
}
