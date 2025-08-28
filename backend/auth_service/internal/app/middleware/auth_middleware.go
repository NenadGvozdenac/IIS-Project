package middleware

import (
	"auth_service/internal/app/utils"
	"net/http"
	"strings"

	"github.com/gin-gonic/gin"
)

func AuthMiddleware() gin.HandlerFunc {
	return func(c *gin.Context) {
		authHeader := c.GetHeader("Authorization")
		if authHeader == "" || !strings.HasPrefix(authHeader, "Bearer ") {
			utils.CreateGinResponse(c, "Authorization header is required", http.StatusUnauthorized, nil)
			c.Abort()
			return
		}

		token := strings.TrimPrefix(authHeader, "Bearer ")
		claims, err := utils.ValidateToken(token)
		if err != nil {
			utils.CreateGinResponse(c, "Invalid token", http.StatusUnauthorized, nil)
			c.Abort()
			return
		}

		userIDFloat, ok := claims["userID"].(float64)
		if !ok {
			utils.CreateGinResponse(c, "Invalid user ID format in token", http.StatusUnauthorized, nil)
			c.Abort()
			return
		}

		userEmail, ok := claims["userEmail"].(string)
		if !ok {
			utils.CreateGinResponse(c, "Invalid user email format in token", http.StatusUnauthorized, nil)
			c.Abort()
			return
		}

		userRole, ok := claims["userRole"].(string)
		if !ok {
			utils.CreateGinResponse(c, "Invalid user role format in token", http.StatusUnauthorized, nil)
			c.Abort()
			return
		}

		userName, ok := claims["userName"].(string)
		if !ok {
			utils.CreateGinResponse(c, "Invalid user name format in token", http.StatusUnauthorized, nil)
			c.Abort()
			return
		}

		userID := uint(userIDFloat)
		c.Set("userID", userID)
		c.Set("userEmail", userEmail)
		c.Set("userRole", userRole)
		c.Set("userName", userName)

		c.Next()
	}
}
