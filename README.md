# RaceDay - Event Management System

## Part 1: System Planning and Database
The foundation of the RaceDay system, including the ERD, API Endpoint Plan, and SQL Database Script. (See the `main` branch for Part 1 files).

## Part 2: RESTful API Development

### Setup and Run Instructions
1. Open the solution (`RaceDayAPI.sln`) in Visual Studio 2022.
2. Ensure the `appsettings.json` connection string points to your local SQL Server Express instance.
3. Open the Package Manager Console and run `Update-Database` to create the database.
4. Press `F5` to run the application. Swagger UI will open automatically in your browser.

### API Features
- **Authentication:** Cookie-based session management using BCrypt for password hashing.
- **Role-Based Access Control (RBAC):** Enforced at the API level (Organisers manage events/categories/results; Participants enroll and view their own data).
- **Database:** Entity Framework Core (Code-First approach) connected to SQL Server.
- **Testing:** xUnit and Moq used for unit testing (6/6 tests passing locally and via CI/CD).

### CI/CD Pipeline
The GitHub Actions workflow automatically builds the project and runs all unit tests on every push to the `Part2-API` branch.

![CI/CD Green Build](./ci_screenshot.png)

## User Roles
The system supports two distinct roles:
- **Organiser:** Can create, edit, and delete events, manage event categories, capture participant results, and view all event enrolments.
- **Participant:** Can create an account, browse events, enter an event by selecting a category, view their own enrolments, and track their personal results.

## Project Links
- **YouTube Video Presentation (Part 2):** https://youtu.be/hlMsbHTgr9w
- **Part 1 YouTube Video:** https://youtu.be/NvDGR1lnJ54


## Declaration of AI Usage
In accordance with academic integrity policies, I hereby declare that Artificial Intelligence (AI) tools were used to aid in the completion of this project. Specifically, AI was utilised for the following purposes:
- Assisting in the design and implementation of the Entity Framework Core models and controllers for the RESTful API.
- Providing guidance and troubleshooting for the implementation of Cookie-based session authentication and Role-Based Access Control (RBAC).
- **Workflows (CI/CD):** Assisting in generating the YAML syntax for the GitHub Actions workflows to automate the build and unit test processes.

All AI-generated content and code were reviewed, tested, and adapted by me to ensure it met the specific requirements of the assignment and reflected my own understanding of the system.

## References
GitHub (2024) *GitHub Actions documentation*. Available at: https://docs.github.com/en/actions (Accessed: 20 October 2026).

Microsoft (2024) *ASP.NET Core documentation*. Available at: https://learn.microsoft.com/en-us/aspnet/core/ (Accessed: 20 October 2026).

Microsoft (2024) *Entity Framework Core documentation*. Available at: https://learn.microsoft.com/en-us/ef/core/ (Accessed: 20 October 2026).

Microsoft (2024) *Unit testing C# with xUnit and .NET*. Available at: https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-dotnet-test (Accessed: 20 October 2026).

