# Overall Structure:
- Main solution, with dockerfile which can be used to spin up a docker image for the API specifically
- The actual web api project, which hosts the authentication and transaction endpoints, following REST principles
- The unit test solution, which tests the transaction controller
- A small, demo app which brings it all together and demonstrates the working

# How to run it:
This solution targets .NET CORE 3.1. In order to run it, you need dotnet core installed, and the CLI available. 
https://dotnet.microsoft.com/download/dotnet-core/3.1

Once this is done, you can clone this repository, so you have a copy of it. Go into the main working folder of the repository, and run the command "dotnet run". This will now have the API running, and waiting for requests

# How to test the operations:
1. Once you have latest on the solution, load it up in Visual Studio (I used 2019 community edition while developing, but should work in 2017 onwards as long as dotnetcore is installed)
2. Run the API project - you can either run it from the solution, by right-clicking, and selecting "Run Project", or you can use the aforementioned dotnetcore CLI with the command "dotnet run", executed within the main project folder (this is wherever interview_suraj.csproj is located). If you run it from VS, you will see the Swagger API homepage, which shows a quick snapshot of the API calls available.
3. Now the API is run - it can be called with a tool like Postman, or making use of the Bank_Demo app provided
4. To use the Bank_Demo app, simply right click on that project, and click on run. It opens up a bare bones app, and follows the following workflow
  - Makes a call to the auth url established in sandbox. 
  - Redirects to this URL
  - Use the mock credentials (john/doe), to allow mock back access
  - Callback is performed back to the controller within the demo app, and a code is returned
  - This code is used to retrieve an access token.
  - This access token is then used to retrieve 2 pieces of information: all account transaction information associated with that access token (user), and a calculation of max, min and average transactions, across all accounts, calculated per category

Note that this repository does not contain the values of client_id and client_secret. They should be configured in user secrets, and should contain your specific client_id and client_secret. Here is a link explaining how:
https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-3.1&tabs=linux
This is the safest way to store secrets, instead of committing it to the repo, and this is how I have implemented it

In order to run the unit tests, go to the UnitTests folder, and run dotnet test. You will need the nunit library: 
https://github.com/nunit/docs/wiki/.NET-Core-and-.NET-Standard

# Caveats, Notes and Misc: 
- This solution makes use of an in-memory db for ease of testing. However, there is a commented line that shows how easy it would be to change to a different persistence layer. All this would need is for us to initialize the SQLLite DB (or other db)as shown here: https://docs.microsoft.com/en-us/ef/core/get-started/?tabs=netcore-cli
- This app is very bare bones, and just used to illustrate a proof of concept. In a more normal application, we'd have separate views, and user directed inputs before performing these calls, instead of just performing them in sequence. In addition, we would probably associate access tokens and refresh tokens with the user, along with their status, instead of getting them every time the user accessed the app

# Extensions/Improvements to API:
- Use of a better persistence layer
- More extensive documentation 
- Making the API calls async so they can respond to long running operations better

# Docker:
(Content heavily sourced from .net and Docker official docs :) )
This solution also contains a docker infrastructure which you can use to build images and create a container specifically for the Web API project. Here is what you need to do:
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
docker pull suri1988/interviewsuraj: This is a public image that can be used to build the image for the API



