package interfaces

import "auth_service/internal/domain/models"

type UserRepositoryInterface interface {
	Create(user *models.User) (*models.User, error)
	GetByEmail(email string) (*models.User, error)
	Delete(id uint) error
}
