# RetailFlow

A modern microservices-based retail management system built with .NET Core. RetailFlow manages order and inventory operations with event-driven architecture using RabbitMQ message broker.

## 🏗️ Architecture

RetailFlow is built using a **microservices architecture** with the following components:

### Services
- **Order Service** - Manages customer orders and order processing
- **Inventory Service** - Handles inventory management and stock tracking
- **Shared Services** - Common infrastructure and domain models

### Technology Stack
- **Framework**: .NET Core / ASP.NET Core
- **Database**: Microsoft SQL Server 2022
- **Message Broker**: RabbitMQ
- **Containerization**: Docker & Docker Compose
- **Architecture Pattern**: Clean Architecture with Domain-Driven Design

## 📋 Prerequisites

- Docker & Docker Compose
- .NET 6.0 or higher (for local development)
- Visual Studio 2022 (optional, for development)

## 🚀 Quick Start

### Using Docker Compose (Recommended)

1. **Clone the repository**
   ```bash
   git clone https://github.com/AhmedMagdy2001/RetailFlow.git
   cd RetailFlow
   ```

2. **Start all services**
   ```bash
   docker-compose up -d
   ```

   This will start:
   - SQL Server (Port 1433)
   - RabbitMQ (Ports 5672, 15672)
   - Order Service (Port 5000)
   - Inventory Service (Port 5001)

3. **Verify services are running**
   ```bash
   docker-compose ps
   ```

4. **Access RabbitMQ Management UI**
   - URL: http://localhost:15672
   - Credentials: `guest` / `guest`

### Local Development

1. **Prerequisites**
   - Ensure SQL Server and RabbitMQ are running
   - Update connection strings in `appsettings.json` if needed

2. **Build the solution**
   ```bash
   dotnet build
   ```

3. **Run migrations**
   ```bash
   dotnet ef database update
   ```

4. **Run services**
   ```bash
   dotnet run --project src/OrderService/OrderService
   dotnet run --project src/InventoryService/InventoryService
   ```

## 📁 Project Structure

```
RetailFlow/
├── src/
│   ├── OrderService/
│   │   ├── OrderService (API)
│   │   ├── OrderService.Application
│   │   ├── OrderService.Domain
│   │   └── OrderService.Infrastructure
│   ├── InventoryService/
│   │   ├── InventoryService (API)
│   │   ├── InventoryService.Application
│   │   ├── InventoryService.Domain
│   │   └── InventoryService.Infrastructure
│   └── Shared/
│       ├── Shared.Domain
│       └── Shared.Infrastructure
├── docker-compose.yml
├── Dockerfile (services)
└── RetailFlow.sln
```

## 🔌 Services & Ports

| Service | Port | Description |
|---------|------|-------------|
| Order Service | 5000 | Order management API |
| Inventory Service | 5001 | Inventory management API |
| SQL Server | 1433 | Database server |
| RabbitMQ AMQP | 5672 | Message broker |
| RabbitMQ Management | 15672 | RabbitMQ UI |

## 🗄️ Database Configuration

- **SQL Server**: `mcr.microsoft.com/mssql/server:2022-latest`
- **Databases**:
  - `OrderDB` - Order service database
  - `InventoryDB` - Inventory service database
- **Default Credentials**: 
  - Username: `sa`
  - Password: `YourPassword123!`

> ⚠️ **Security Note**: Change the default password in `docker-compose.yml` for production use.

## 📨 Message Queue

RabbitMQ is configured with:
- **Default User**: `guest`
- **Default Password**: `guest`
- **Management UI**: http://localhost:15672

The services communicate asynchronously through RabbitMQ for event-driven operations.

## 🛠️ Development

### Building Docker Images

```bash
docker-compose build
```

### Viewing Logs

```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f orderservice
docker-compose logs -f inventoryservice
```

### Stopping Services

```bash
docker-compose down

# Remove volumes as well
docker-compose down -v
```

### Database Management

#### SQL Server Connection

```bash
# Connect to SQL Server container
docker exec -it mssql-server /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P YourPassword123!
```

#### View Database Logs

```bash
docker logs mssql-server
docker logs rabbitmq-server
```

## 📝 Environment Configuration

Services use the following environment variables (see `docker-compose.yml`):

- `ASPNETCORE_ENVIRONMENT`: Development/Production
- `ConnectionStrings__DefaultConnection`: Database connection string
- `RabbitMQ__HostName`: RabbitMQ server address
- `RabbitMQ__UserName`: RabbitMQ credentials
- `RabbitMQ__Password`: RabbitMQ credentials

### Local appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=OrderDB;User Id=sa;Password=YourPassword123!;Encrypt=false;TrustServerCertificate=true;"
  },
  "RabbitMQ": {
    "HostName": "localhost",
    "UserName": "guest",
    "Password": "guest"
  }
}
```

## 🔄 Event Flow

The system uses event-driven architecture for inter-service communication:

1. **Order Service** publishes events when orders are created/updated
2. **Inventory Service** subscribes to order events and updates inventory
3. **RabbitMQ** acts as the message broker for asynchronous communication

### Example Events

- `OrderCreated`
- `OrderConfirmed`
- `OrderCancelled`
- `InventoryReserved`
- `InventoryReleased`


## 🚀 Deployment

### Docker Compose (Development/Testing)

```bash
docker-compose up -d
```

### Production Deployment

For production deployment:

1. Update credentials in environment variables
2. Use managed database services (Azure SQL, AWS RDS)
3. Configure proper logging and monitoring
4. Set up CI/CD pipeline
5. Use Kubernetes for orchestration (optional)

## 🔐 Security Best Practices

- [ ] Change default SQL Server password
- [ ] Change default RabbitMQ credentials
- [ ] Use environment variables for sensitive data
- [ ] Enable SSL/TLS for communication
- [ ] Implement API authentication/authorization
- [ ] Use network policies and firewalls
- [ ] Regular security audits and updates

## 🐛 Troubleshooting

### SQL Server Connection Issues

```bash
# Check if SQL Server is running
docker ps | grep mssql

# View SQL Server logs
docker logs mssql-server

# Restart SQL Server
docker restart mssql-server
```

### RabbitMQ Connection Issues

```bash
# Check if RabbitMQ is running
docker ps | grep rabbitmq

# Access RabbitMQ shell
docker exec -it rabbitmq-server /bin/bash

# Check RabbitMQ status
docker exec rabbitmq-server rabbitmq-diagnostics status
```

### Service Health Checks

```bash
# View health status
docker-compose ps

# Expected output shows service status
```

### Rebuild Everything

```bash
docker-compose down -v
docker system prune -f
docker-compose up -d --build
```

## 📊 Monitoring & Logging

### View Application Logs

```bash
# All services
docker-compose logs -f

# Last 100 lines
docker-compose logs --tail=100

# Specific time frame
docker-compose logs --since 10m
```

### RabbitMQ Management

- Access UI: http://localhost:15672
- Default credentials: `guest` / `guest`
- Monitor queues, connections, and channels
- Set up alerts for queue lengths

## 📈 Performance Considerations

- **Connection Pooling**: Configured for SQL Server and RabbitMQ
- **Async/Await**: Used throughout for I/O operations
- **Message Batching**: Optimize RabbitMQ throughput
- **Caching**: Implement caching for frequently accessed data
- **Database Indexing**: Ensure proper indexes on frequently queried columns

## 🤝 Contributing

1. Fork the repository
   ```bash
   git clone https://github.com/AhmedMagdy2001/RetailFlow.git
   ```

2. Create a feature branch
   ```bash
   git checkout -b feature/AmazingFeature
   ```

3. Commit your changes
   ```bash
   git commit -m 'Add some AmazingFeature'
   ```

4. Push to the branch
   ```bash
   git push origin feature/AmazingFeature
   ```

5. Open a Pull Request

### Coding Standards

- Follow Microsoft C# coding conventions
- Use meaningful variable and method names
- Add XML documentation comments
- Write unit tests for new features
- Keep methods focused and small

## 📄 License

This project is open source and available under the MIT License. See the LICENSE file for more details.

## 👤 Author

**Ahmed Magdy**
- GitHub: [@AhmedMagdy2001](https://github.com/AhmedMagdy2001)
- Email: [Your Email]

## 📞 Support & Contact

For issues, questions, or suggestions:

- Open an [issue](https://github.com/AhmedMagdy2001/RetailFlow/issues) on GitHub
- Create a [discussion](https://github.com/AhmedMagdy2001/RetailFlow/discussions)
- Contact the maintainer

## 🔗 Related Links

- [.NET Documentation](https://docs.microsoft.com/dotnet/)
- [RabbitMQ Documentation](https://www.rabbitmq.com/documentation.html)
- [SQL Server Documentation](https://docs.microsoft.com/sql/sql-server/)
- [Docker Documentation](https://docs.docker.com/)

## 📋 Roadmap

- [ ] Add API Gateway
- [ ] Implement service discovery
- [ ] Add distributed tracing
- [ ] Setup CI/CD pipeline
- [ ] Kubernetes deployment manifests
- [ ] GraphQL API
- [ ] Real-time notifications with SignalR
- [ ] Advanced reporting and analytics

## 📝 Changelog

### Version 1.0.0 (Current)
- Initial microservices setup
- Order Service implementation
- Inventory Service implementation
- RabbitMQ event-driven communication
- Docker Compose configuration

---

**Last Updated**: June 2026

**Happy Coding! 🚀**
