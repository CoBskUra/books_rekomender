#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
RUN apt-get update && \
        apt-get install -y wget && \
        apt-get install -y gnupg2 && \
        wget -qO- https://deb.nodesource.com/setup_18.x | bash - && \
        apt-get install -y build-essential nodejs

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
RUN apt-get update && \
        apt-get install -y wget && \
        apt-get install -y gnupg2 && \
        wget -qO- https://deb.nodesource.com/setup_18.x | bash - && \
        apt-get install -y build-essential nodejs
WORKDIR /src
COPY ["BooksRecommender/BooksRecommender.csproj", "BooksRecommender/"]
RUN dotnet restore "BooksRecommender/BooksRecommender.csproj"
COPY . .
WORKDIR "/src/BooksRecommender"
RUN dotnet build "BooksRecommender.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BooksRecommender.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BooksRecommender.dll"]