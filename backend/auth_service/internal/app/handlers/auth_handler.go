package handlers

import (
	"auth_service/config"
	"auth_service/internal/app/dtos"
	"auth_service/internal/app/repositories"
	"auth_service/internal/app/saga"
	"auth_service/internal/app/utils"
	"auth_service/internal/domain/models"
	"net/http"

	"github.com/gin-gonic/gin"
	"golang.org/x/crypto/bcrypt"
)

// Register a new user and return a JWT
func Register(c *gin.Context) {
	var registerDTO dtos.RegisterDTO
	if err := c.ShouldBindJSON(&registerDTO); err != nil {
		utils.CreateGinResponse(c, "Invalid input", http.StatusBadRequest, nil)
		return
	}

	// Create repository
	userRepo := repositories.NewUserRepository(config.DB)

	// Check if user with this email already exists
	existingUser, err := userRepo.GetByEmail(registerDTO.Email)
	if err != nil {
		utils.CreateGinResponse(c, "Database error: "+err.Error(), http.StatusInternalServerError, nil)
		return
	}
	if existingUser != nil {
		utils.CreateGinResponse(c, "User with this email already exists", http.StatusConflict, nil)
		return
	}

	// Hash the password
	hashedPassword, err := bcrypt.GenerateFromPassword([]byte(registerDTO.Password), bcrypt.DefaultCost)
	if err != nil {
		utils.CreateGinResponse(c, "Failed to hash password", http.StatusInternalServerError, nil)
		return
	}

	// Create new user model
	newUser := &models.User{
		Name:     registerDTO.Name,
		Surname:  registerDTO.Surname,
		Email:    registerDTO.Email,
		Phone:    registerDTO.Phone,
		Password: string(hashedPassword),
		UserType: registerDTO.UserType,
	}

	// Initialize SAGA orchestrator
	ticketServiceClient := saga.NewTicketServiceClient(config.TicketServiceURL)
	sagaOrchestrator := saga.NewSagaOrchestrator(ticketServiceClient)

	// Define user creation and deletion functions for SAGA
	createUserFunc := func() (*models.User, error) {
		return userRepo.Create(newUser)
	}

	deleteUserFunc := func(id uint) error {
		return userRepo.Delete(id)
	}

	// Execute SAGA
	sagaResult := sagaOrchestrator.ExecuteUserRegistrationSaga(newUser, createUserFunc, deleteUserFunc)

	if !sagaResult.Success {
		utils.CreateGinResponse(c, "Failed to register user: "+sagaResult.Error.Error(), http.StatusInternalServerError, nil)
		return
	}

	// Generate access token for the successfully created user
	token, err := utils.GenerateToken(sagaResult.User.ID, sagaResult.User.Email, sagaResult.User.UserType, sagaResult.User.Name)
	if err != nil {
		// User was created but token generation failed - this is a partial failure
		// In a production system, you might want to handle this differently
		utils.CreateGinResponse(c, "User registered but failed to generate token", http.StatusInternalServerError, nil)
		return
	}

	tokenDTO := dtos.TokenDTO{
		Token: token,
	}

	utils.CreateGinResponse(c, "User registered successfully", http.StatusCreated, tokenDTO)
}

// Login a user and return a JWT
func Login(c *gin.Context) {
	var input dtos.LoginDTO

	if err := c.ShouldBindJSON(&input); err != nil {
		utils.CreateGinResponse(c, "Invalid input", http.StatusBadRequest, nil)
		return
	}

	// Create repository
	userRepo := repositories.NewUserRepository(config.DB)

	// Find the user by email
	user, err := userRepo.GetByEmail(input.Email)
	if err != nil {
		utils.CreateGinResponse(c, "Database error: "+err.Error(), http.StatusInternalServerError, nil)
		return
	}
	if user == nil {
		utils.CreateGinResponse(c, "Invalid email or password", http.StatusUnauthorized, nil)
		return
	}

	// Compare passwords
	if err := bcrypt.CompareHashAndPassword([]byte(user.Password), []byte(input.Password)); err != nil {
		utils.CreateGinResponse(c, "Invalid email or password", http.StatusUnauthorized, nil)
		return
	}

	// Generate access token
	token, err := utils.GenerateToken(user.ID, user.Email, user.UserType, user.Name)
	if err != nil {
		utils.CreateGinResponse(c, "Failed to generate token", http.StatusInternalServerError, nil)
		return
	}

	tokenDTO := dtos.TokenDTO{
		Token: token,
	}

	utils.CreateGinResponse(c, "User logged in successfully", http.StatusOK, tokenDTO)
}
