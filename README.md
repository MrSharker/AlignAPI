# AlignAPI

This API allows you to manage missions and find the closest mission based on a given address or geo-coordinates.

## Tech stack

- C# .Net 6
- Docker
- [OpenStreetMap](https://www.openstreetmap.org/) as external API

## Getting Started

### Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Docker](https://www.docker.com/products/docker-desktop)

### Setting Up PostgreSQL with PostGIS

1. Run the PostgreSQL container with PostGIS extension:

    ```sh
    docker-compose up -d
    ```
2. Enter to PostgreSQL
    ```sh
    docker exec -ti <container_name> psql -U <user_name>
    ```
3. Create database
    ```sh
    CREATE DATABASE <db_name>;
    ```

### Setting Up the API

1. Update the `appsettings.json` file with the PostgreSQL connection string:

    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "<your connection string>"
      }
    }
    ```

2. Install the required NuGet packages:

    ```sh
    dotnet add package Microsoft.EntityFrameworkCore.Design
    dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
    dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL.NetTopologySuite
    dotnet add package NetTopologySuite
    ```

 3. Run the API:

    ```sh
    dotnet run
    ```