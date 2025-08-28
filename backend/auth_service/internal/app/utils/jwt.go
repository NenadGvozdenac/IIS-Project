package utils

import (
	"errors"
	"time"

	"github.com/gin-gonic/gin"
	"github.com/golang-jwt/jwt/v5"
	"github.com/google/uuid"
)

var jwtSecret = []byte("Pq5s9XJ68zQWJY8h2Nx2Q9sYyQJdJf2zEwRZp9LrXUs=")
var jwtDuration = time.Minute * 30
var jwtRefreshDuration = time.Hour * 24 * 7
var jwtIssuer = "iis"
var jwtAudience = "iis"

// GenerateToken generates a JWT for a given user ID
var GenerateToken = func(userID uint, userEmail string, userRole string, userName string) (string, error) {

	jti := uuid.New().String()

	claims := jwt.MapClaims{
		"userID":    userID,
		"userEmail": userEmail,
		"userRole":  userRole,
		"userName":  userName,
		"exp":       time.Now().Add(jwtDuration).Unix(),
		"iss":       jwtIssuer,   // Add issuer
		"aud":       jwtAudience, // Add audience
		"jti":       jti,
	}

	token := jwt.NewWithClaims(jwt.SigningMethodHS256, claims)
	return token.SignedString(jwtSecret)
}

// ValidateToken validates a JWT and returns the claims
func ValidateToken(tokenString string) (jwt.MapClaims, error) {
	token, err := jwt.Parse(tokenString, func(token *jwt.Token) (interface{}, error) {
		if _, ok := token.Method.(*jwt.SigningMethodHMAC); !ok {
			return nil, errors.New("unexpected signing method")
		}
		return jwtSecret, nil
	})

	if err != nil || !token.Valid {
		return nil, errors.New("invalid token")
	}

	claims, ok := token.Claims.(jwt.MapClaims)
	if !ok {
		return nil, errors.New("invalid claims")
	}

	if claims["iss"] != jwtIssuer {
		return nil, errors.New("invalid issuer")
	}
	if claims["aud"] != jwtAudience {
		return nil, errors.New("invalid audience")
	}

	if claims["jti"] == nil || claims["jti"].(string) == "" {
		return nil, errors.New("invalid or missing jti")
	}

	return claims, nil
}

var ExtractUserIdFromContext = func(c *gin.Context) uint {
	userId := c.MustGet("userID").(uint)
	return userId
}

var ExtractUserEmailFromContext = func(c *gin.Context) string {
	userEmail := c.MustGet("userEmail").(string)
	return userEmail
}

var ExtractUserRoleFromContext = func(c *gin.Context) string {
	userRole := c.MustGet("userRole").(string)
	return userRole
}

var ExtractUserNameFromContext = func(c *gin.Context) string {
	userName := c.MustGet("userName").(string)
	return userName
}

// ExtractDataFromContext extracts user data from the context
// and returns the user ID, email, role, and name
var ExtractDataFromContext = func(c *gin.Context) (uint, string, string, string) {
	userId := ExtractUserIdFromContext(c)
	userEmail := ExtractUserEmailFromContext(c)
	userRole := ExtractUserRoleFromContext(c)
	userName := ExtractUserNameFromContext(c)
	return userId, userEmail, userRole, userName
}

// ValidateRefreshToken validates a refresh token and returns the claims
var ValidateRefreshToken = func(refreshToken string) (jwt.MapClaims, error) {
	token, err := jwt.Parse(refreshToken, func(token *jwt.Token) (interface{}, error) {
		if _, ok := token.Method.(*jwt.SigningMethodHMAC); !ok {
			return nil, errors.New("unexpected signing method")
		}
		return jwtSecret, nil
	})

	if err != nil || !token.Valid {
		return nil, errors.New("invalid refresh token")
	}

	claims, ok := token.Claims.(jwt.MapClaims)
	if !ok {
		return nil, errors.New("invalid refresh token claims")
	}

	// Validate issuer and audience for refresh token
	if claims["iss"] != jwtIssuer {
		return nil, errors.New("invalid issuer for refresh token")
	}
	if claims["aud"] != jwtAudience {
		return nil, errors.New("invalid audience for refresh token")
	}

	// Validate jti for refresh token
	if claims["jti"] == nil || claims["jti"].(string) == "" {
		return nil, errors.New("invalid or missing jti for refresh token")
	}

	return claims, nil
}

var GenerateRefreshToken = func(userID uint) (string, error) {

	jti := uuid.New().String()

	claims := jwt.MapClaims{
		"userID": userID,
		"exp":    time.Now().Add(jwtRefreshDuration).Unix(),
		"iss":    jwtIssuer,
		"aud":    jwtAudience,
		"jti":    jti,
	}

	token := jwt.NewWithClaims(jwt.SigningMethodHS256, claims)
	return token.SignedString(jwtSecret)
}
