FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
ARG VERSION
ARG REVISION

# Set default shell
SHELL ["/bin/bash", "-c"]

# Install missing packages
RUN curl -sL https://deb.nodesource.com/setup_16.x | bash -
RUN apt-get install -y nodejs unzip

# Install latest DocFX release
RUN dotnet tool update -g docfx

# Restore dependencies and tools
COPY src/EWS.csproj .
RUN dotnet restore "EWS.csproj"

# Set environment variables
ENV PUBLISH_DIR=/app/publish
ENV GENERATE_SOURCEMAP=false

# Create optimized production build
COPY src/ .
RUN dotnet publish "EWS.csproj" \
  -c Release \
  -o ${PUBLISH_DIR}

# Build documentation
COPY docs/ ./docs/
RUN ~/.dotnet/tools/docfx build -o ${PUBLISH_DIR}/wwwroot/help docs/docfx.json

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
ENV HOME=/app
ENV TZ=Europe/Zurich
ENV ASPNETCORE_ENVIRONMENT=Production
WORKDIR ${HOME}

EXPOSE 80

# Set default locale
ENV LANG=C.UTF-8
ENV LC_ALL=C.UTF-8

COPY --from=build /app/publish $HOME

ENTRYPOINT ["dotnet", "EWS.dll"]
