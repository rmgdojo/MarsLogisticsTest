# Set up instructions
Unzip the project to a folder.
Then open a terminal, navigate to the extracted folder, and use the following commands to test or run the project

```bash
dotnet test ./MarsParcelTrackerTests/MarsParcelTrackerTests.csproj
dotnet run --project .\MarsParcelTrackingAPI\MarsParcelTrackingAPI.csproj
```

Upon running the API, alter the URL to include /swagger (eg: https://localhost:7144/swagger) and this will display the Swagger UI.

## Summary of design choices and trade-offs
The "Minimal API" approach has been used for this solution.

## Walkthrough of solution, what might be improved for enterprise scale

A mockable clock was created to aid with "time progression" during unit testing and Swagger testing.
For validating barcodes, a Regular Expression has been used.
A standardised way of formatting dates could also be set.
Improvements that could be made include adding checks for duplicate barcodes when creating a parcel, and also JWT token authentication.

## Explanation of design choices
The minimal API style was chosen as it is simply a more up-to-date method of creating an API than the MVC controller approach.
I believe it looks slightly neater and requires slightly less boilerplate code.
To save time, the main worker service (ParcelTrackingService) returns string constants as a means of returning error messages via the API endpoints.
A better approach of using an Enum with error codes would be potentially more useful.
I prefer to use the EF in-memory database over the JSON file approach as I simply haven't used the latter.
The tests required more than one valid barcode to be created, as due to tasks running in parallel, data concurrency issues occurred.
The data fixtures could also be split into a separate class.

## AI usage
This was used to assist with setting up Swagger, and assisting with creating mock tests and some minor functionality such as the PATCH endpoint construction.
I was unable to add Swagger documention for the endpoints, as I'm not very familiar with setting it up.
