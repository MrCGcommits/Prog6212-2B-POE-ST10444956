# RaceDay API Endpoint Plan

| HTTP method | Route | Description | Role required | Request body | Expected response |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **POST** | `/api/auth/register` | Registers a new user (Organiser or Participant). | None (public) | `{email, password, firstName, lastName, role}` | **201** Created. <br>**400** Bad Request (invalid data). <br>**409** Conflict (email exists). |
| **POST** | `/api/auth/login` | Authenticates a user and returns a JWT token. | None (public) | `{email, password}` | **200** OK (Returns JWT Token). <br>**401** Unauthorized (bad credentials). |
| **GET** | `/api/users/profile` | Retrieves the logged-in user's profile information. | Any (logged in) | None | **200** OK (User details). <br>**401** Unauthorized. |
| **PUT** | `/api/users/profile` | Updates the user's profile details. | Any (logged in) | `{firstName, lastName, passwordHash}` | **200** OK. <br>**400** Bad Request. <br>**401** Unauthorized. |
| **GET** | `/api/events` | Retrieves a list of all upcoming events. | None (public) | None | **200** OK (List of events). |
| **POST** | `/api/events` | Creates a new event. | Organiser | `{eventName, eventDate, location, description}` | **201** Created. <br>**400** Bad Request. <br>**403** Forbidden (not Organiser). |
| **GET** | `/api/events/{id}` | Retrieves details of a specific event. | None (public) | None | **200** OK (Event details). <br>**404** Not Found. |
| **PUT** | `/api/events/{id}` | Updates an existing event. | Organiser | `{eventName, eventDate, location, description}` | **200** OK. <br>**403** Forbidden. <br>**404** Not Found. |
| **DELETE** | `/api/events/{id}` | Deletes an event. | Organiser | None | **204** No Content. <br>**403** Forbidden. <br>**404** Not Found. |
| **GET** | `/api/events/{id}/categories` | Gets all categories for a specific event. | None (public) | None | **200** OK (List of categories). <br>**404** Not Found. |
| **POST** | `/api/events/{id}/categories` | Adds a new category (e.g., 10km, 21km) to an event. | Organiser | `{categoryName, distanceKm, maxParticipants}` | **201** Created. <br>**400** Bad Request. <br>**403** Forbidden. |
| **PUT** | `/api/categories/{id}` | Updates an existing category. | Organiser | `{categoryName, distanceKm, maxParticipants}` | **200** OK. <br>**403** Forbidden. <br>**404** Not Found. |
| **DELETE** | `/api/categories/{id}` | Deletes a category. | Organiser | None | **204** No Content. <br>**403** Forbidden. <br>**404** Not Found. |
| **POST** | `/api/enrollments` | Enrolls a participant into a specific event category. | Participant | `{eventId, categoryId}` | **201** Created. <br>**400** Bad Request. <br>**403** Forbidden. <br>**409** Conflict (already enrolled). |
| **GET** | `/api/enrollments/my` | Views all enrollments for the logged-in participant. | Participant | None | **200** OK (List of enrollments). <br>**401** Unauthorized. |
| **GET** | `/api/events/{id}/enrollments` | Organiser views all enrollments for their specific event. | Organiser | None | **200** OK (List of participants). <br>**403** Forbidden. <br>**404** Not Found. |
| **DELETE** | `/api/enrollments/{id}` | Participant cancels their enrollment. | Participant | None | **204** No Content. <br>**403** Forbidden. <br>**404** Not Found. |
| **POST** | `/api/results` | Organiser captures participant results for an event. | Organiser | `{enrollmentId, finishTime, overallPosition, categoryPosition}` | **201** Created. <br>**400** Bad Request. <br>**403** Forbidden. |
| **GET** | `/api/results/my` | Participant tracks their personal results. | Participant | None | **200** OK (List of results). <br>**401** Unauthorized. |
| **GET** | `/api/events/{id}/results` | Organiser views all results for their event. | Organiser | None | **200** OK (List of results). <br>**403** Forbidden. <br>**404** Not Found. |


<!-- Formatting check: Auth endpoints finalised -->
<!-- Formatting check: Event endpoints finalised -->
<!-- Formatting check: Category endpoints finalised -->
<!-- Formatting check: Enrollment endpoints finalised -->