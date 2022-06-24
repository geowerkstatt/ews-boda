FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
ARG VERSION
ARG REVISION

# Set default shell
SHELL ["/bin/bash", "-c"]

# Install Node.js
RUN curl -sL https://deb.nodesource.com/setup_16.x | bash -
RUN apt-get install -y nodejs

# Restore dependencies and tools
COPY src/EWS.csproj .
RUN dotnet restore "EWS.csproj"

# Create optimized production build
COPY src/ .
ENV GENERATE_SOURCEMAP=false
ENV PUBLISH_DIR=/app/publish
RUN dotnet publish "EWS.csproj" \
  -c Release \
  -p:VersionPrefix=${VERSION} \
  -p:SourceRevisionId=${REVISION} \
  -o ${PUBLISH_DIR}

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
ENV HOME=/app
ENV TZ=Europe/Zurich
ENV ASPNETCORE_ENVIRONMENT=Production
WORKDIR ${HOME}

# Install useful packages
RUN \
  DEBIAN_FRONTEND=noninteractive && \
  mkdir -p /usr/share/man/man1 /usr/share/man/man2 && \
  apt-get update && \
  apt-get install -y curl vim && \
  rm -rf /var/lib/apt/lists/*

EXPOSE 80

# Set default locale
ENV LANG=C.UTF-8
ENV LC_ALL=C.UTF-8

COPY --from=build /app/publish $HOME

HEALTHCHECK CMD curl --fail http://localhost/ || exit 1

ENTRYPOINT ["dotnet", "EWS.dll"]
