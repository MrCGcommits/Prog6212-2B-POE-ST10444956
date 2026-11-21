## 🤖 Declaration of AI Usage

In accordance with academic integrity policies, I hereby declare that Artificial Intelligence (AI) tools were used to aid in the completion of this project. Specifically, AI was utilised for the following purposes:

*   **Code Troubleshooting:** AI was used to assist in troubleshooting complex integration issues between the MVC frontend, the RESTful API, and Azure Blob Storage.
*   **Debugging CI/CD and Docker:** AI provided guidance on resolving environment-specific errors in the GitHub Actions pipeline (such as Windows-to-Linux cache conflicts) and assisted in structuring the multi-stage Dockerfile.
*   **Code Explanation and Documentation:** AI was used to explain complex concepts (like cookie forwarding in `HttpClient`) to ensure a deep understanding of the implemented architecture, and to assist in formatting the README 


# RaceDay - Event Management System

RaceDay is a full-stack web application designed for the South African road running, walking, and cycling community. It allows Organisers to create and manage events, and Participants to browse, enrol, and track their results.

## 🚀 Technologies Used
* **Backend:** ASP.NET Core Web API (.NET 8), Entity Framework Core, SQL Server
* **Frontend:** ASP.NET Core MVC (.NET 8), Bootstrap 5
* **Storage:** Azure Blob Storage (via Azurite Emulator locally)
* **Testing:** xUnit
* **DevOps:** Docker, GitHub Actions

## 🛠️ Local Setup Instructions

### Prerequisites
* Visual Studio 2022 (.NET 8 SDK)
* SQL Server Express / LocalDB
* Node.js (for Azurite emulator)

### 1. Database Setup
1. Open the solution in Visual Studio.
2. Open the **Package Manager Console** (`View -> Other Windows -> Package Manager Console`).
3. Ensure the Default project is set to `RaceDayAPI`.
4. Run `Update-Database` to create the SQL database.

### 2. Azurite (Azure Storage Emulator) Setup
1. Open a terminal/command prompt.
2. Install Azurite globally: `npm install -g azurite`
3. Start the emulator: `azurite --silent --location c:\azurite`
4. **Leave this terminal open** while running the app.

### 3. Running the Application
1. In Visual Studio, right-click the Solution -> **Configure Startup Projects**.
2. Select **Multiple startup projects**.
3. Set both `RaceDayAPI` and `RaceDayMVC` to **Start**.
4. Press `F5` or click the green Play button.
5. The API will run on `http://localhost:5115` and the MVC app on `http://localhost:5227`.

## 🐳 Docker Setup
To run the MVC application in a Docker container, run the following commands from the root directory of the solution:

```bash
docker build -t raceday-mvc -f RaceDayMVC/Dockerfile .
docker run -d -p 8080:8080 --name raceday-mvc-container raceday-mvc
```

The app will be accessible at http://localhost:8080.

## 🎥 Video Presentation
https://youtu.be/1Ar-4MrjhaM 

## ✅ CI/CD Pipeline
![CI Build Success](./ci-success.png)

## 📚 References

*   Microsoft (2024) *ASP.NET Core MVC Documentation*. Available at: https://learn.microsoft.com/en-us/aspnet/core/mvc/overview (Accessed: 18 November 2026).
*   Microsoft (2024) *Azure Blob Storage Documentation*. Available at: https://learn.microsoft.com/en-us/azure/storage/blobs/ (Accessed: 18 November 2026).
*   Microsoft (2024) *Entity Framework Core Documentation*. Available at: https://learn.microsoft.com/en-us/ef/core/ (Accessed: 18 November 2026).
*   Microsoft (2024) *Docker Documentation*. Available at: https://docs.docker.com/ (Accessed: 18 November 2026).
*   Microsoft (2024) *GitHub Actions Documentation*. Available at: https://docs.github.com/en/actions (Accessed: 18 November 2026).
*   Microsoft (2024) *Azurite Emulator Documentation*. Available at: https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite (Accessed: 19 November 2026).
*   W3Schools (2024) *Bootstrap 5 Tutorial*. Available at: https://www.w3schools.com/bootstrap5/ (Accessed: 19 November 2026).


