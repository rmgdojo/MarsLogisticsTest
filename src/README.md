# Mars Logistics Division

## Overview
- I've opted for a full DDD approach, the core business logic is encapsulated within the Domain project.
- The application project is used to orchestrate the business logic and provide services for the API layer.
- The data access layer (DAL) would typically interact with a database, but for simplicity, I've used in-memory storage as suggested.

## How to Run
1. Clone the Repo
2. Build and Run the Solution
3. I've configured the default profile in the Launch settings to open the Swagger UI on run `http://localhost:5000/swagger` to allow you to interact with the API endpoints

## Testing
- Run all tests with `dotnet test` from application root, or from your IDE
- I've concentrated purely on the domain level unit tests, as this is where the core logic resides. Writing tests for the application and API layers would typically just be testing mocks and dependencies, which is less valuable in this context. Plus I only had '3 hours' to complete the task, and I already went over this by a number I'd rather not mention!
- Had I had more time, I would have added integration tests to cover the API endpoints and e2e experience.

## AI Usage
- I've tried to avoid using AI tools for this task. That being said I do have the Copilot plugin in Rider, which i've allowed the autocomplete feature to generate some boilerplate code and method signatures.
