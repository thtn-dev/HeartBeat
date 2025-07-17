# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/ZSocialMedia.WebApi/ZSocialMedia.WebApi.csproj", "ZSocialMedia.WebApi/"]
COPY ["src/ZSocialMedia.Application/ZSocialMedia.Application.csproj", "ZSocialMedia.Application/"]
COPY ["src/ZSocialMedia.Domain/ZSocialMedia.Domain.csproj", "ZSocialMedia.Domain/"]
COPY ["src/ZSocialMedia.Shared/ZSocialMedia.Shared.csproj", "ZSocialMedia.Shared/"]
COPY ["src/ZSocialMedia.Infrastructure/ZSocialMedia.Infrastructure.csproj", "ZSocialMedia.Infrastructure/"]
COPY ["src/ZSocialMedia.Infrastructure.Cache/ZSocialMedia.Infrastructure.Caching.csproj", "ZSocialMedia.Infrastructure.Cache/"]
RUN dotnet restore "./ZSocialMedia.WebApi/ZSocialMedia.WebApi.csproj"
COPY src .
WORKDIR "/src/ZSocialMedia.WebApi"
RUN dotnet build "./ZSocialMedia.WebApi.csproj" \
	-c $BUILD_CONFIGURATION  \
	-o /app/build \
	--no-restore

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ZSocialMedia.WebApi.csproj" \
	-c $BUILD_CONFIGURATION \
	-o /app/publish \
	--no-restore \
	/p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
RUN addgroup -g 1001 -S appgroup && \
    adduser -S appuser -G appgroup -u 1001
RUN apk add curl ca-certificates tzdata
WORKDIR /app
COPY --from=publish /app/publish .
RUN chown -R appuser:appgroup /app
USER appuser
# Environment variables
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "ZSocialMedia.WebApi.dll"]

