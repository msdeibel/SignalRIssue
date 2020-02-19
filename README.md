# SignalRIssue
Exposes a SignalR issue when working with events from the backend

# Exposing the issue
The solution is initially configured to expose the issue.
Whenever a connection (even a normally brief one) is made with the server the client 
receives one additional message.

# Desired behaviour
The desired behaviour to only receive one message in the client requires a single change to the code

In `Startup.cs` change lines 43 and 44 from
```
//endpoints.MapHub<ValueHub.ValueHub>("/valuesHub", options =>
endpoints.MapHub<ValueHub.FaultyValueHub>("/valuesHub", options =>
```
>to
```
endpoints.MapHub<ValueHub.ValueHub>("/valuesHub", options =>
//endpoints.MapHub<ValueHub.FaultyValueHub>("/valuesHub", options =>
```
