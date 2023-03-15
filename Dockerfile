FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
ARG VERSION
ARG REVISION

# Set default shell
SHELL ["/bin/bash", "-c"]

# Install missing packages
RUN curl -sL https://deb.nodesource.com/setup_16.x | bash -
RUN apt-get install -y nodejs mono-complete unzip

# Download and unzip latest DocFX release
RUN curl https://github.com/dotnet/docfx/releases/download/v2.62.2/docfx-linux-x64-v2.62.2.zip -LO --silent --show-error && \
  unzip -o -q docfx-linux-x64-v2.62.2.zip

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
  -p:VersionPrefix=${VERSION} \
  -p:SourceRevisionId=${REVISION} \
  -o ${PUBLISH_DIR}

# Build documentation
COPY docs/ ./docs/
RUN mono docfx.exe build -o ${PUBLISH_DIR}/wwwroot/help docs/docfx.json

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
