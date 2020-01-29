# Explanation of Solution

This repository contains the solution to the problem described here:
https://docs.google.com/document/d/1ohS-4y9svHZCG3yYXZmCtbC7lJdHk73B0mcffsntx-0/edit

Overall Structure:
- Main solution, with dockerfile which can be used to spin up 
- The actual web api project, which hosts the authentication and transaction endpoints
- The unit test solution, which tests the transaction controller

How to run it:
This solution targets .NET CORE 3.1. In order to run it, you need dotnet core installed, and the CLI available. 
https://dotnet.microsoft.com/download/dotnet-core/3.1

Once this is done, you can clone this repository, so you have a copy of it. Go into the main working folder of the repository, and run
the command "dotnet run". This will now have the API running, and waiting for requests

How to test the operations:
This makes the assumption that you have access to a sandbox account i.e you have a client_id, client_secret and a redirect url. This
redirect url will typically be hosted on the actual application. For simplicity, I have used the default one

Use the following postman calls in sequence, to satisfy the authentication -> transaction workflow
1) Get authentication url, hosted on truelayer
curl --location --request GET 'https://localhost:5001/api/Authentication/AuthUrl' \
--header 'Content-Type: application/json' \
--data-raw ''

2) Go to the address returned. This should return a code. 

3) Make the following call with this code, to get an access token
curl --location --request GET 'https://localhost:5001/api/Authentication/AccessToken?code=8E3SgWFNA_x6ITHo9g0k9hBY1SBIUqOZJjhoLsTGepE%0A' \
--header 'Content-Type: application/x-www-form-urlencoded' \
--data-urlencode 'grant_type=authorization_code' \
--data-urlencode 'client_id={client_id}' \
--data-urlencode 'client_secret={client_secret}' \
--data-urlencode 'redirect_uri=https://console.truelayer-sandbox.com/redirect-page' \
--data-urlencode 'code=7dLKzO8myn6weFTtlGetSwpkTq0x2pS2WaznEssNYhA

Note that this repository does not contain the values of client_id and client_secret. They should be configured in user secrets, and
should contain your specific client_id and client_secret. Here is a link explaining how:
https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-3.1&tabs=linux
This is the safest way to store secrets, instead of committing it to the repo

4) Store this access code and refresh token. You will need them for subsequent calls

5) Retrieving all accounts, tied to an access token
curl --location --request GET 'https://localhost:5001/api/Transactions?token={accessToken}' \

6) Retrieving max, min and avg of all transactions, across accounts, for a single user (via access token)
curl --location --request GET 'https://localhost:5001/api/Transactions/UserMetrics?token={accessToken}' \

7) In case your access token expires, use this call to get a new access and refresh token
curl --location --request GET 'https://localhost:5001/api/Authentication/RenewToken?refreshToken={refreshToken}'

In order to run the unit tests, go to the UnitTests folder, and run dotnet test. You will need the nunit library: 
https://github.com/nunit/docs/wiki/.NET-Core-and-.NET-Standard

Note: This solution makes use of an in-memory db for ease of testing. However, there is a commented line that shows how easy it would
be to change to a different persistence layer. All this would need is for us to initialize the SQLLite DB (or other db)
as shown here: https://docs.microsoft.com/en-us/ef/core/get-started/?tabs=netcore-cli

Extensions/Improvements to API:
- Use of a better persistence layer
- More extensive documentation
- Making the API calls async so they can respond to long running operations better

Docker:
(Content heavily sourced from .net and Docker official docs :) )
This solution also contains a docker infrastructure which you can use to build images and create containers. Here is what you need to do:
1) Go to the main project directory. 
Run: dotnet publish -c Release
This creates the necessary dlls

2) Go one level up, to the solution directory, which includes a Dockerfile
docker build -t final -f Dockerfile .
This will restore packages, build the solution, and have everything needed for our image

3) Run docker images to see a list of images installed

4) Run docker create final to create a container that can then be easily distributed among the team

5) Run docker start {container_name} to start the actual container

6) If you don't want to create a fresh image from scratch, I have prepared a remote image:
docker pull suri1988/interviewsuraj



