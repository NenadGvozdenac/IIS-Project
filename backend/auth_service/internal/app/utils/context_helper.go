package utils

import (
	"fmt"
	"strconv"
	"time"

	"github.com/gin-gonic/gin"
)

func ConvertParamToUint(c *gin.Context, param_name string) uint {
	param := c.Param(param_name)
	paramUint, _ := strconv.ParseUint(param, 10, 64)
	return uint(paramUint)
}

func ConvertDatabaseDateFormatToReadableFormat(date string) string {
	// Parse the input string with the expected format
	parsedDate, err := time.Parse(time.RFC3339, date)
	if err != nil {
		// Handle the error if the date string is not in the expected format
		fmt.Println("Error parsing date:", err)
		return ""
	}

	// Format the parsed date into the desired format
	return parsedDate.Format("02.01.2006 15:04:05")
}
