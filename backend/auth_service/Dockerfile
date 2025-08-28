# Build stage
FROM golang:1.23.4 AS builder

WORKDIR /app

# Copy go.mod i go.sum
COPY backend/auth_service/go.mod backend/auth_service/go.sum ./

# Download dependencies
RUN go mod download

# Copy the source code
COPY backend/auth_service/ .

# Build the Go app as a static binary
RUN CGO_ENABLED=0 GOOS=linux go build -o main ./main.go

# Production stage
FROM alpine:latest  

WORKDIR /root/

RUN apk --no-cache add ca-certificates curl

# Copy the binary file from the builder stage
COPY --from=builder /app/main .

ENV GO_ENV=release

EXPOSE 8080

CMD ["./main"]
