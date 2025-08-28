package routes

import (
	"auth_service/internal/app/handlers"

	"github.com/gin-gonic/gin"
)

func SetupRoutes(router *gin.Engine) {
	// Define the main API group with the prefix "/api"
	api := router.Group("/api")

	// Setup public routes (includes metrics)
	setupPublicRoutes(api)
}

func setupPublicRoutes(api *gin.RouterGroup) {
	api.POST("/register", handlers.Register)
	api.POST("/login", handlers.Login)
}
