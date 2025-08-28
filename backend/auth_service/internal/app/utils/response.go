package utils

import (
	"github.com/gin-gonic/gin"
)

type Response struct {
	Message string      `json:"message"`
	Code    int         `json:"code"`
	Data    interface{} `json:"data"`
}

func CreateResponse(message string, code int, data interface{}) Response {
	return Response{
		Message: message,
		Code:    code,
		Data:    data,
	}
}

func CreateGinResponse(c *gin.Context, message string, code int, data interface{}) {
	response := CreateResponse(message, code, data)
	c.JSON(code, response)
}
