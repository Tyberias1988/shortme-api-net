# short-me API

This is the API for the short-me project. It is a simple URL shortener.

## Used technologies
- [dotnet](https://dotnet.microsoft.com/en-us/download)
- [ef core](https://docs.microsoft.com/de-de/ef/core/)
- [postgres](https://www.postgresql.org/)
- [docker](https://www.docker.com/)
- [docker-compose](https://docs.docker.com/compose/)

## How to run
- Install [docker](https://docs.docker.com/get-docker/) and [docker-compose](https://docs.docker.com/compose/install/)
- Run `cd shortme-api-net && docker-compose up -d`
- Run `dotnet run` to start the server or `dotnet watch` with hot reloading (or use your IDE)
- The API will be available at `https://localhost:7200` and `http://localhost:5146`

### Hints
- `Helpers` already has functions implemented for generating a random string of any length