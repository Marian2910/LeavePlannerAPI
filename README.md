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
Customer Controller:
_____________________________________________________________

Route: /api/customer

GetAllCustomers
Method: GET
Description: Retrieve a paginated list of customers with optional sorting, filtering, and search functionality.
Parameters:
  pageNumber (int) - Page number to retrieve.
  pageSize (int) - Number of items per page.
  sortDirection (string?, optional) - Sorting direction (asc or desc).
  sortCriteria (string?, optional) - Field to sort by.
  status (bool?, optional) - Filter by customer status.
  search (string?, optional) - Search keyword for customer names.
Response: Returns a PagedResultDto<Customer> object.

GetCustomerById
Method: GET /{id}
Description: Retrieve a customer by their ID.
Parameters:
  id (int) - Customer ID.
Response: Returns the Customer object or 404 if not found.

AddCustomer
Method: POST
Description: Add a new customer.
Request Body: CustomerDto
Response: Confirmation message on success.

AddDocumentToCustomer
Method: POST /add-documents
Description: Upload documents for a specific customer.
Parameters:
  id (int) - Customer ID.
  files (IEnumerable<IFormFile>) - List of files to upload.
Response: Success message or validation errors.

UpdateCustomer
Method: PUT
Description: Update a customer's details.
Request Body: UpdateCustomerDto
Response: Confirmation message on success.

DeleteCustomer
Method: DELETE /{customerId}
Description: Soft-delete a customer (make inactive).
Parameters:
  customerId (int) - Customer ID.
Response: Success message.

SearchCustomersByName
Method: GET /search
Description: Search for customers by name.
Parameters:
  name (string) - Name to search for.
  pageNumber (int) - Page number.
  pageSize (int) - Items per page.
Response: Paginated list of customers or 404 if not found.

DeleteMultipleCustomers
Method: DELETE
Description: Soft-delete multiple customers by their IDs.
Request Body: int[] (List of customer IDs).
Response: Success message.

DataController
_____________________________________________________________

Route: /api/data

CreateData
Method: POST /createData
Description: Seed the database with default jobs, departments, employees, and customers if they don't exist.
Response: 200 OK upon success.

DocumentController
_____________________________________________________________

Route: /api/document

GetDocumentsByCustomerById
Method: GET /{customerId}
Description: Retrieve documents uploaded by a specific customer.
Parameters:
  customerId (int) - Customer ID.
Response: Returns a list of documents or 404 if none are found.

DeleteDocument
Method: DELETE /{customerId}/{documentId}
Description: Delete a specific document associated with a customer.
Parameters:
  customerId (int) - Customer ID.
  documentId (int) - Document ID.
Response: Success message.

EmployeeController
_____________________________________________________________

Route: /api/employee

GetEmployeeById
Method: GET /{id}
Description: Retrieve an employee by their ID.
Parameters:
  id (int) - Employee ID.
Response: Returns the Employee object.

GetAllEmployees
Method: GET
Description: Retrieve a list of all employees.
Response: Returns a PagedResultDto<Employee> object.

EventController
_____________________________________________________________

Route: /api/event

GetAllEvents
Method: GET
Description: Retrieve all company events.
Response: Returns a list of EventDto objects.

PersonalEventController
_____________________________________________________________

Route: /api/personalevent

AddEvent
Method: POST
Description: Add a new personal event for an employee.
Request Body: PersonalEventDto via FormData.
Response: Confirmation message upon success.

GetAllPersonalEvents
Method: GET
Description: Retrieve all personal events.
Response: Returns a list of PersonalEventDto objects.

GetPersonalEventById
Method: GET /{id}
Description: Retrieve a personal event by its ID.
Parameters:
  id (int) - Event ID.
Response: Returns the PersonalEventDto object or 404 if not found.

GetPersonalEventsByEmployeeId
Method: GET /getByEmployee/{employeeId}
Description: Retrieve personal events for a specific employee.
Parameters:
  employeeId (int) - Employee ID.
Response: Returns a list of PersonalEventDto objects or 404 if not found.
