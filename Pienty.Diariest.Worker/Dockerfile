FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        libgdiplus \
        tzdata \
        wget \
        ca-certificates \
        gnupg \
     && rm -rf /var/lib/apt/lists/*

# Install new relic the agent
RUN echo 'deb http://apt.newrelic.com/debian/ newrelic non-free' | tee /etc/apt/sources.list.d/newrelic.list \
&& wget https://download.newrelic.com/548C16BF.gpg \
&& apt-key add 548C16BF.gpg \
&& apt-get update \
&& apt-get install -y newrelic-dotnet-agent

ENV Logging__Console__FormatterName="Simple"
ARG target
ARG appName
ENV ASPNETCORE_ENVIRONMENT="$target"
ENV DOTNET_PRINT_TELEMETRY_MESSAGE=false
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ENV NR_APP_NAME="$appName"

WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Pienty.Diariest.Worker/Pienty.Diariest.Worker.csproj", "Pienty.Diariest.Worker/"]
COPY ["Pienty.Diariest.Core/Pienty.Diariest.Core.csproj", "Pienty.Diariest.Core/"]
RUN dotnet restore "Pienty.Diariest.Worker/Pienty.Diariest.Worker.csproj"
COPY . .
WORKDIR "/src/Pienty.Diariest.Worker"
RUN dotnet build "Pienty.Diariest.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Pienty.Diariest.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ../start_application.sh /app/start_application.sh
RUN chmod +x /app/start_application.sh
RUN sed -i -e 's/\r$/\n/' /app/start_application.sh
ENTRYPOINT ["/app/start_application.sh"]