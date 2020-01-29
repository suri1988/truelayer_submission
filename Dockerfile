FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY Interview_Suraj/interview_suraj.csproj Interview_Suraj/
RUN dotnet restore "Interview_Suraj/interview_suraj.csproj"
COPY . ./
WORKDIR "/src/Interview_Suraj"
RUN dotnet build "interview_suraj.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "interview_suraj.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "interview_suraj.dll"]
