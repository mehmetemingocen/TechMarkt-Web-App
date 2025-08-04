# Hepsişurada Mobile API Test Documentation

## API Base URL
`https://localhost:7001/api`

## Authentication Endpoints

### 1. Register User
**POST** `/api/auth/register`
```json
{
  "email": "test@example.com",
  "password": "Test123!",
  "firstName": "Test",
  "lastName": "User",
  "phone": "5551234567"
}
```

### 2. Login User
**POST** `/api/auth/login`
```json
{
  "email": "test@example.com",
  "password": "Test123!"
}
```

### 3. Logout User
**POST** `/api/auth/logout`
*Requires Authorization Header: `Bearer {token}`*

## Product Endpoints

### 1. Get All Products
**GET** `/api/products`

### 2. Get Products by Category
**GET** `/api/products?categoryId=1`

### 3. Get Product by ID
**GET** `/api/products/{id}`

### 4. Search Products
**GET** `/api/products/search?q=apple`

## Category Endpoints

### 1. Get All Categories
**GET** `/api/categories`

### 2. Get Products by Category
**GET** `/api/categories/{id}/products`

## Cart Endpoints (Requires Authentication)

### 1. Get Cart
**GET** `/api/cart`
*Requires Authorization Header: `Bearer {token}`*

### 2. Add Item to Cart
**POST** `/api/cart/items`
*Requires Authorization Header: `Bearer {token}`*
```json
{
  "productId": 1,
  "quantity": 2
}
```

### 3. Update Cart Item
**PUT** `/api/cart/items/{itemId}`
*Requires Authorization Header: `Bearer {token}`*
```json
{
  "quantity": 3
}
```

### 4. Remove Item from Cart
**DELETE** `/api/cart/items/{itemId}`
*Requires Authorization Header: `Bearer {token}`*

### 5. Clear Cart
**DELETE** `/api/cart`
*Requires Authorization Header: `Bearer {token}`*

### 6. Checkout
**POST** `/api/cart/checkout`
*Requires Authorization Header: `Bearer {token}`*

## Testing with curl

### Register a new user:
```bash
curl -X POST https://localhost:7001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Test123!",
    "firstName": "Test",
    "lastName": "User",
    "phone": "5551234567"
  }'
```

### Login:
```bash
curl -X POST https://localhost:7001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Test123!"
  }'
```

### Get all products:
```bash
curl -X GET https://localhost:7001/api/products
```

### Get all categories:
```bash
curl -X GET https://localhost:7001/api/categories
```

## Response Format

All API responses follow this format:
```json
{
  "success": true,
  "data": {...},
  "message": "Success message",
  "errors": null
}
```

## Error Response Format
```json
{
  "success": false,
  "data": null,
  "message": "Error message",
  "errors": ["Error 1", "Error 2"]
}
```

## JWT Token

After successful login/register, you'll receive a JWT token that should be included in the Authorization header for protected endpoints:

```
Authorization: Bearer {your-jwt-token}
```

## CORS Configuration

The API is configured to allow requests from any origin for development purposes. In production, you should restrict this to your mobile app's domain.

## Database Connection

The API uses the following database connection:
- Server: .\MSSQLSERVER2022
- Database: storeDbb
- User: mehmetgocen
- Password: Ek2f9g1^9
- TrustServerCertificate: True 