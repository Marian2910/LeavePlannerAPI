# Introduction 
This backend application supports leave management and event planning within a company. Employees can log in to submit leave requests to HR, organize personal events, and view company-wide or personal upcoming events. HR personnel have additional capabilities, including approving leave requests, scheduling events for all employees to see, and managing clients, such as viewing contracts and adding or removing clients from the system. The backend is built using .NET and integrates with a database for data persistence, leveraging Entity Framework.
# Getting Started
To get the backend code up and running on your local machine, follow these steps:

1. Clone the repository: git clone ...
2. Install dependencies: Ensure you have the .NET SDK installed, then restore the project dependencies: dotnet restore
3. Database Setup:
  This project uses Entity Framework Core to manage database migrations and schema.

  To create the initial migration:
    dotnet ef migrations add InitialCreate
  To update the database with the migration:
    dotnet ef database update
  The database will be created with the name LeavePlannerDB. Ensure you have configured the connection string in the appsettings.json file to match your environment.

4. Run the application
  To start the backend API, run:
  dotnet run
  By default, the application will run on http://localhost:5077 or http://localhost:7252. Swagger UI is available at /swagger, where you can explore and test the API endpoints.

5. API Documentation
  The application uses Swagger UI for API documentation and testing. After running the app, you can access the Swagger interface at:
  
  https://localhost:5001/swagger
This provides detailed information about all available endpoints, including:
