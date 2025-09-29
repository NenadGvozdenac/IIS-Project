package dtos

// DTO from the client to register a new user
type RegisterDTO struct {
	Name            string `json:"name" binding:"required"`
	Surname         string `json:"surname" binding:"required"`
	Email           string `json:"email" binding:"required,email"`
	Phone           string `json:"phone" binding:"required"`
	Password        string `json:"password" binding:"required"`
	ConfirmPassword string `json:"confirm_password" binding:"required,eqfield=Password"`
	UserType        string `json:"user_type" binding:"required"`
}

// DTO from the client to login a user
type LoginDTO struct {
	Email    string `json:"email" binding:"required,email"`
	Password string `json:"password" binding:"required"`
}

// DTO to return a JWT to the client
type TokenDTO struct {
	Token string `json:"token"`
}
