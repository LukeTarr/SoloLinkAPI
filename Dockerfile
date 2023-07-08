# Build
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# ENV Vars
ARG JWT_SECRET
ENV JWT_SECRET=$JWT_SECRET

ARG POSTGRES_DATABASE
ENV POSTGRES_DATABASE=$POSTGRES_DATABASE

WORKDIR /source
COPY . .

RUN dotnet restore "./SoloLinkAPI/SoloLinkAPI.csproj" --disable-parallel
RUN dotnet publish "./SoloLinkAPI/SoloLinkAPI.csproj" -c release -o /app --no-restore


# Serve
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app ./

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "SoloLinkAPI.dll"]


