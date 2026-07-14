# Quickstart: Cooperative Cancellation Validation

## Prerequisites
- .NET 9 SDK installed
- Local SQLite database creation permitted
- Optional: a test Slack webhook or stub endpoint if validating external notification cancellation manually
- Optional: browser dev tools or debugger access to help simulate mid-request cancellation

## Setup
1. From the repository root, restore and build the solution:
   ```powershell
   dotnet build
   ```
2. Run the existing unit tests:
   ```powershell
   dotnet test .\RevelioII.UnitTests\RevelioII.UnitTests.csproj
   ```
3. If validating external notification behavior, set `SLACK_WEBHOOK_URL` to a safe test endpoint.

## Run the Application
```powershell
dotnet run --project .\RevelioII.csproj
```

## Validation Scenario 1: Graph Query Cancels When the Request Is Aborted
1. Open the home page or trigger the home-page graph data handler that loads `GetGraphViewAsync`.
2. Use browser cancellation, navigation away, or a debugger pause to abort the request while the query is still in flight.
3. Confirm the request does not complete normally and the server handles the cancellation without converting it into an unrelated 500 response.
4. Confirm the data-access path stops at the in-flight EF Core operation instead of continuing to completion.

Expected outcome:
- The request ends as a cancellation rather than a successful graph payload.
- The application remains stable and subsequent requests continue to work.

## Validation Scenario 2: Node Creation Notification Cancels with the Parent Request
1. Start a node creation request from the Nodes create page.
2. Abort the request before the outbound notification completes.
3. Observe the outbound notification behavior at the test endpoint or through debugging.

Expected outcome:
- The outbound HTTP notification is cancelled with the parent request.
- No detached notification continues after the request is abandoned.
- Node creation preserves its normal success path when notification delivery fails for reasons other than request cancellation.

## Regression Checks
1. Create a node successfully without aborting the request.
2. Create a relationship successfully without aborting the request.
3. Load the home page graph data successfully without aborting the request.
4. Re-run unit tests after implementation changes.

Expected outcome:
- Existing graph CRUD behavior remains intact when cancellation is not triggered.
- Unit tests pass with updated async signatures.
