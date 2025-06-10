FROM mcr.microsoft.com/dotnet/aspnet:9.0
ARG NETENV=Release
WORKDIR /src

# copy csproj and restore as distinct layers
COPY ./ ./
RUN dotnet restore src/movielogger.api/movielogger.api.csproj
RUN dotnet publish src/movielogger.api/movielogger.api.csproj -c $NETENV -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app ./

ENV ASPNETCORE_URLS=http://*:5049
EXPOSE 5049

ARG API_DB_URL
ENV DATABASE_URL=$API_DB_URL

ENTRYPOINT ["dotnet", "movielogger.api.dll"]