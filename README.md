[![CI](https://github.com/GeoWerkstatt/ews-boda/actions/workflows/ci.yml/badge.svg)](https://github.com/GeoWerkstatt/ews-boda/actions/workflows/ci.yml)
[![Release](https://github.com/GeoWerkstatt/ews-boda/actions/workflows/release.yml/badge.svg)](https://github.com/GeoWerkstatt/ews-boda/actions/workflows/release.yml)

# Grundlagedaten EWS

Webapplikation zur Verwaltung von Bohrdaten aus Erdwärmesonden (EWS) beim Amt für Umwelt des Kantons Solothurn.
Die Daten enthalten unter anderem Informationen zu geologischen Schichten (Schichtdaten) der Bohrungen sowie über besondere Vorkommnisse bei EWS-Bohrungen (z.B. Wassereintritte, Arteser, Gasaufstösse, etc.).

## Einrichten der Entwicklungsumgebung

Folgende Komponenten müssen auf dem Entwicklungsrechner installiert sein:

* Git
* Docker
* Visual Studio 2022 oder Visual Studio Code
* Node.js 16 LTS
* Optional, um die Onlinehilfe zu erstellen:
  * [DocFX](https://github.com/dotnet/docfx)

## Neue Version erstellen

Ein neuer GitHub _Pre-release_ wird bei jeder Änderung auf [main](https://github.com/GeoWerkstatt/ews-boda) [automatisch](./.github/workflows/pre-release.yml) erstellt. In diesem Kontext wird auch ein neues Docker Image mit dem Tag _:edge_ erstellt und in die [GitHub Container Registry (ghcr.io)](https://github.com/geowerkstatt/ews-boda/pkgs/container/ews-boda) gepusht. Der definitve Release erfolgt, indem die Checkbox _This is a pre-release_ eines beliebigen Pre-releases entfernt wird. In der Folge wird das entsprechende Docker Image in der ghcr.io Registry mit den Tags (bspw.: _:v1.2.3_ und _:latest_) [ergänzt](./.github/workflows/release.yml).
