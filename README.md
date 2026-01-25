# Inventory Management API - Backend Project

## 📌 Portfolio Project

This is a backend project developed to showcase technical skills in RESTful API development, software architecture, and database management. It's part of my personal portfolio as a .NET developer.

## 🎯 Project Purpose

Demonstrate competencies in:

- REST API development with .NET Core
- Implementation of clean, scalable architectures
- Relational database management with Entity Framework
- JWT security and authentication implementation
- Development best practices and testing

## 🏗️ Technology Stack

### Backend

- **.NET 10.0** - Main framework
- **C# 14.0** - Programming language
- **ASP.NET Core Web API** - For REST API construction
- **Entity Framework Core** - ORM for data access
- **PostgreSQL** - Relational database
- **JWT** - Token-based authentication
- **BCrypt** - Password encryption
- **XUnit + Moq** - Unit testing

### Frontend (Complementary Project)

- **Angular 21** - Frontend framework
- **TypeScript** - JavaScript superset
- **RxJS** - Reactive programming
- **Angular Material** - UI components

🔗 **Frontend available at:** [https://github.com/your-username/inventory-frontend](https://github.com/your-username/inventory-frontend)

## 📊 Technical Features Demonstrated

### 1. Architecture & Patterns

- **REST Architecture** with clear separation of concerns
- **Repository Pattern** for data access abstraction
- **Controller-Service-Repository** for modular organization
- **DTOs** for secure data transfer
- **Dependency Injection** with .NET Core

### 2. Security

- **JWT (JSON Web Tokens)** based authentication
- **BCrypt** for secure password hashing
- Custom authorization middleware
- Input validation on all endpoints

### 3. Database

- Normalized design with **PostgreSQL**
- **Entity Framework Core** with migrations
- Optimized indexes for frequent queries
- Integrity constraints at database level

### 4. Code Quality

- Unit tests with **XUnit** and **Moq**
- SOLID principles applied
- Clean, maintainable code
- Endpoint documentation

## 📁 Project Structure

```
InventoryBackend/
├── Controllers/          # API REST Controllers
│   ├── AuthController.cs
│   ├── ProductsController.cs
│   └── InventoryController.cs
├── Services/            # Business logic
│   ├── AuthService.cs
│   ├── ProductService.cs
│   └── InventoryService.cs
├── Models/              # Domain entities
│   ├── User.cs
│   ├── Product.cs
│   └── Movement.cs
├── Data/               # EF Context and migrations
│   └── AppDbContext.cs
├── DTOs/               # Data Transfer Objects
├── Helpers/            # Utilities and extensions
├── Middlewares/        # Custom middleware
└── Tests/              # Unit tests
```

## 🔧 Local Installation

### Requirements

- Visual Studio 2022 or VS Code
- .NET SDK 10.0
- PostgreSQL 16+
- Git

### Quick Steps

```bash
# 1. Clone repository
git clone https://github.com/your-username/inventory-backend.git

# 2. Configure database
# Edit appsettings.json with your PostgreSQL connection

# 3. Restore dependencies
dotnet restore

# 4. Run migrations
dotnet ef database update

# 5. Run application
dotnet run
```

## 📡 Main Endpoints

| Method | Endpoint             | Description        |
| ------ | -------------------- | ------------------ |
| POST   | `/api/auth/register` | User registration  |
| POST   | `/api/auth/login`    | JWT authentication |
| GET    | `/api/products`      | List products      |
| POST   | `/api/products`      | Create product     |
| GET    | `/api/inventory`     | Check inventory    |
| POST   | `/api/movements`     | Register movement  |

## 🧪 Testing

```bash
# Run unit tests
dotnet test

# Test coverage >80% on critical components
```

## 📸 Visual Demonstration

### Architecture Diagram

_(Include system architecture diagram here)_

### Database Model

_(Include PostgreSQL ER diagram here)_

### Use Cases Implemented

![Diagrama de Gestión de Inventario](/Docs/diagrama-de-actividades-de-gestión-de-inventario.png)

https://github.com/MazMorrDev/MiniMazErpBack/blob/main/Docs/Diagrama%20de%20Actividades%20de%20Gesti%C3%B3n%20de%20Movimientos.drawio.png

https://github.com/MazMorrDev/MiniMazErpBack/blob/main/Docs/Diagrama%20de%20Actividades%20de%20Inicio%20de%20Sesi%C3%B3n.drawio.png
https://github.com/MazMorrDev/MiniMazErpBack/blob/main/Docs/Diagrama%20de%20Actividades%20de%20Registro%20de%20usuario.drawio.png

### API Tests

_(Include Postman/Thunder Client screenshots here)_

## 🎓 Skills Demonstrated

### Backend Development

- RESTful APIs with ASP.NET Core
- JWT authentication and authorization
- Entity Framework Core and migrations
- Relational database design
- Scalable software architecture

### Best Practices

- SOLID principles
- Dependency injection
- Unit testing
- Version control with Git
- API documentation

### Basic DevOps

- Environment configuration
- Database connection management
- Environment variables
- Migration scripts

## 🔄 Frontend Integration

This backend is designed to work with an Angular frontend application that consumes the API through authenticated HTTP requests. Communication is handled via JSON and JWT tokens.

## 📚 Key Learnings

During this project's development, I reinforced:

- Professional RESTful API design
- Web application security
- Database query optimization
- .NET application testing
- Technical project documentation

## 📄 License

Educational project - Free for personal and portfolio use.

## 👤 Contact

**Developer:** Marco Antonio Romero Albanez  
**Email:** marconchelo12@gmail.com  
**LinkedIn:** [Marco Antonio Romero Albanez](https://linkedin.com/in/marco-antonio-romero-albanez-2653372b2)  
**GitHub:** [MazMorrDev](https://github.com/MazMorrDev)

---

_This project is part of my personal portfolio as a .NET developer. All functionalities have been implemented for demonstrative and educational purposes._
