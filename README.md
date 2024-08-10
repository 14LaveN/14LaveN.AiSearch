
This project is a microservices-based application that integrates with the ChatGPT API. It uses several key technologies to ensure scalability, reliability, and maintainability, including RabbitMQ for message queuing, Prometheus for monitoring, CQRS (Command Query Responsibility Segregation) for separating read and write operations, and Entity Framework Core (EF Core) for database interaction.

Key Technologies
ChatGPT API: Provides AI-driven natural language processing capabilities.
RabbitMQ: A messaging broker used to handle communication between microservices asynchronously.
Prometheus: A monitoring system used to collect and visualize application metrics.
CQRS: A design pattern that separates the responsibilities of querying and command handling.
EF Core: An object-relational mapper (ORM) used for database operations.
Architecture
The application follows a microservices architecture where each service is responsible for a specific domain. The key components are:

API Gateway: Manages requests from clients and routes them to appropriate services.
Command Service: Handles write operations (commands) using CQRS. This service interacts with the database through EF Core and communicates with other services via RabbitMQ.
Query Service: Handles read operations (queries) using CQRS. This service retrieves data from the database using EF Core.
ChatGPT Service: Manages interactions with the ChatGPT API, processing natural language requests and returning responses.
Message Broker: RabbitMQ is used for message passing between services to ensure decoupling and scalability.
Monitoring: Prometheus collects metrics from the services, enabling monitoring and alerting for system health.
Setup and Installation
Prerequisites
.NET 6 SDK
Docker
RabbitMQ
Prometheus
A ChatGPT API Key
Steps
Clone the repository:


Use Docker Compose to build and start the services:

bash
Копировать код
docker-compose up --build
Apply Migrations:

Run the EF Core migrations to set up the database schema:

The API Gateway will be accessible at http://localhost:5000.
Prometheus will be accessible at http://localhost:9090.
Usage
Send requests to the API Gateway for interacting with the system.
Monitor system performance and metrics via the Prometheus dashboard.
CQRS Implementation
The application implements CQRS to separate read and write operations:

Command Service: Handles all write operations, such as creating, updating, or deleting resources. These operations are processed asynchronously, with results communicated via RabbitMQ.

Query Service: Handles read operations. Queries are optimized for performance, retrieving data directly from the database without the overhead of processing commands.

Monitoring and Metrics
Prometheus: Collects metrics from each microservice, providing insights into system health and performance.
Grafana: (Optional) Can be used with Prometheus to visualize metrics on customizable dashboards.
RabbitMQ Integration
RabbitMQ is used as the messaging backbone for the system, allowing services to communicate asynchronously. This enables better scalability and decoupling of services.

Database
The application uses EF Core for database interactions. Migrations are used to manage schema changes, and the database context is configured to work with a relational database (e.g., SQL Server, PostgreSQL).

