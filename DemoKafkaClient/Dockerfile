FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["test.kafka/DemoKafkaClient/", "test.kafka/DemoKafkaClient/"]
RUN dotnet restore "test.kafka/DemoKafkaClient/DemoKafkaClient.csproj"
RUN dotnet publish "test.kafka/DemoKafkaClient/DemoKafkaClient.csproj" -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:3.1
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "DemoKafkaClient.dll"]
