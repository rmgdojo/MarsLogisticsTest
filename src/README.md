# RMParcelTracker


## Running the project

Open command prompt/shell of your choice and change directory to the root folder of the project where RMParcelTracker.sln file is present.

Execute command `dotnet run --project RMParcelTracker.Api` to start the API.

Swagger UI is located at http://localhost:5026/index.html 

IF there are port conflicts, port can be changed in launchSettings.json inside project properties.


## Testing the project

Open command prompt/shell of your choice and change directory to the root folder of the project where RMParcelTracker.sln file is present.

Execute command `dotnet test` to run all tests in the solution.


## Design Choices and TradeOffs

## Design Choices

- FluentValidation  -  To make life easier as it provides helper methods for basic stuff and customisation of error message.
- FluentResults     -  For clear separation of success and failure cases and encapsulating Exceptions into failures.
- XUnit  - As I am more comfortable in that test framework.

## TradeOffs
- Inclusion of PATCH inside same ParcelController , ideally as the consumer/client is different than website we should seperate this out. But due to time constraints kept there.
- Usage of Repository inside ParcelController, for bubbling up better error responses when Parcel already exists and Parcel not found scenarios. Going through another layer will  be unnecessary at this point ,and returning Result with metadata to identify this cases might be overkill for MVP.
- Use of DateOnly in LaunchDate and EstimatedArrivalDate fields as the requirement doesn't mention any time based logic. I sort of assumed its safe to work with Dates only format.
- Ideally should separate out Unit and Integration tests, but due to time constraints kept both in same project
- Purposely didnt split domain into separate projects as this is as an MVP.
- Excluded use of Stateless or similar StateMachine package which can help structure parcel status transitions in a better way, mainly due to time constraints.
- **ParcelRepository is kept very simple. This is not thread safe at the moment and uses reference and due to time constraints didnt want to spend too much on that**.
- Kept the domain's return signature to plain T object than Result(T) as didnt think mixing that was necessary to domain.
- Choice of ArgumentException where a domain exception would have shown clear intent when trying to transition to invalid status.
- It is debatable to bring validation inside to Service layer than use it in controllers, but early exit strategy made me bring validators outside services.
- I omitted most intefaces, and used it only where test double was needed *ie.* IClock.

## Walkthrough

- `Features` folder contains usecase based functionality split on Register Parcel, Retrieve Parcel, Update Parcel Status.
- `Common` folder contains models which are shared between functionalities.
- `RMParcelTracker.Tests` contains both unit and integration tests to cover core functionality tests.
- `UpdateParcelApiTests` and `UpdateParcelTests` has code to verify the transition of state from Created to `OnRocketToMars` based On `TestClock` AdvanceDays method.

## Enterprise Readiness

- Connection to a persistence layer.
- Rate Limiting features can be introduced in API's.
- Separation of management API which is the PATCH function.
- Apply Authentication feature so that only authorised personnel can change status of Parcels.
- More logs to provide tracing of application in case of exceptions/ logic flaws.
- Standardise error responses and prevent leaking of InternalServerError.
- Localisation of error messages.

## AI Usage

 Tool used: Github CoPilot GPT 5 Mini

- To find first wednesday of a month for express delivery.
- To know the usage of CreatedAtAction method to return created Parcel link with Created response.
- Spend time exploring Stateless to see if it can be used in project.
- To find the usage of ValidationProblem(new ValidationProblemDetails ()) to return BadRequest with ProblemDetails response.
- On how to use Cascade(CascadeMode.Stop) in Validator because Must was getting executed in Barcode validator and erroring if length < 25.
- AI took me down wrong path of adding invalid month amount in StandardDelivery, so wrote more test and correcteg logic with simple maths.
- Explored the tests for StandardDeliveryTests and ExpressDeliveryTests and uncovered a logic mistake.

## Optionals 

*How to handle delays to the system?*

May be allow another status called Delay which can happen at any point after `Created`. Run background tasks which analyse state transition delays based on SLA's defined or on specific events which trigger's delays.


*Simulation of lost parcels in the system?*

Run a service which picks up parcels from repository at random which has future launch date and which allows Lost status transition and mark as Lost.


*Basic filtering of another GET list/parcels?*

We could provide query parameters like `api/list/parcels/launchDate={0}&page={1}&limit={2}&status={3}`.
This helps in grabbing important filter fields like status and launch date which can be used by external systems.

- The first approach for given example query will be to introduce some kind of indexing at database layer on (LaunchDate,Status) fields so that it can be queryable efficiently.
- LaunchDate may be have to be limited to the future, because possibly old launch dates are no longer relevant.
- We could use key set pagination for increased performance as we can remember the last fetched parcel barcode and use that inside our query.
   `api/list/parcels/launchDate={0}&lastBarCode={1}&limit={2}&status={3}`.

Resulting in query like 

<pre>
SELECT *
FROM Parcels
WHERE 
LaunchDate = @{0}
AND
BarCode > @{1}
AND
Status = @{3}
OrderBy BarCode 
LIMIT = @{2}
</pre>

## Learnings

It was a great learning opportunity to limit myself to not overdo things and make some tradeoffs which may be oversimplifying things.

Thank you for taking the time to review my work. Please do provide me feedback.

