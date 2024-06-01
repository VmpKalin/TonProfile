FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
RUN apt-get update && apt install unzip && apt-get install -y curl && apt-get --assume-yes install nuget
RUN curl "https://awscli.amazonaws.com/awscli-exe-linux-x86_64.zip" -o "awscliv2.zip"
RUN unzip awscliv2.zip && ./aws/install

WORKDIR /artifacts
RUN dotnet new tool-manifest --name manifest
RUN dotnet tool install --ignore-failed-sources AWS.CodeArtifact.NuGet.CredentialProvider
RUN dotnet codeartifact-creds install

COPY ./Argentics.Backend.Profile/src/Argentics.Backend.Profile.Api/*.csproj ./
RUN dotnet restore Argentics.Backend.Profile.Api.csproj

COPY ./Argentics.Backend.Profile/src/Argentics.Backend.Profile.Core/*.csproj ./ 
RUN dotnet restore Argentics.Backend.Profile.Core.csproj

COPY ./Argentics.Backend.Profile/src/Argentics.Backend.Profile.Data/*.csproj ./ 
RUN dotnet restore Argentics.Backend.Profile.Data.csproj

COPY ./Argentics.Backend.Profile/src ./
RUN dotnet publish ./Argentics.Backend.Profile.Api/Argentics.Backend.Profile.Api.csproj -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app

COPY --from=build /out .

ENTRYPOINT ["dotnet", "Argentics.Backend.Profile.Api.dll"]
