import React, { useState, useEffect, useRef } from "react";
import Map from "ol/Map";
import View from "ol/View";
import TileLayer from "ol/layer/Tile";
import TileWMS from "ol/source/TileWMS";
import VectorSource from "ol/source/Vector";
import GeoJSON from "ol/format/GeoJSON";
import Feature from "ol/Feature";
import Projection from "ol/proj/Projection";
import { addProjection } from "ol/proj";
import { Vector } from "ol/layer";
import { Point } from "ol/geom";
import { Stroke, Style } from "ol/style";
import "ol/ol.css";

export default function Karte() {
  const [map, setMap] = useState();
  const [bohrungen, setBohrungen] = useState();
  const [bohrungenLayer, setBohrungenLayer] = useState();
  const [kantonsgrenze, setKantonsgrenze] = useState();
  const [kantonsgrenzeLayer, setKantonsgrenzeLayer] = useState();

  const mapElement = useRef();
  const mapRef = useRef();
  mapRef.current = map;
  mapElement.current = map;

  // Get Bohrungen from database
  useEffect(() => {
    fetch("/bohrung")
      .then((response) => response.json())
      .then((fetchedFeatures) => {
        const parsedFeatures = fetchedFeatures.map(
          (f) =>
            new Feature({
              geometry: new Point([f.geometrie.coordinates[0], f.geometrie.coordinates[1]]),
              name: f.bezeichnung,
              id: f.id,
            })
        );
        setBohrungen(parsedFeatures);
      });
  }, []);

  // Get Kantonsgrenze
  useEffect(() => {
    fetch(
      "https://api3.geo.admin.ch/rest/services/api/MapServer/ch.swisstopo.swissboundaries3d-kanton-flaeche.fill/11?geometryFormat=geojson&sr=2056"
    )
      .then((response) => response.json())
      .then((fetchedFeatures) => {
        setKantonsgrenze([new GeoJSON().readFeature(fetchedFeatures.feature)]);
      });
  }, []);

  // Initialize map on first render
  useEffect(() => {
    // Add custom projection for LV95
    const projection = new Projection({
      code: "EPSG:2056",
      extent: [2485071.58, 1075346.31, 2828515.82, 1299941.79],
      units: "m",
    });
    addProjection(projection);

    const landeskarte = new TileLayer({
      source: new TileWMS({
        url: "https://wms.geo.admin.ch/?VERSION=1.3.0&lang=de",
        params: { LAYERS: "ch.swisstopo.pixelkarte-grau", TILED: true },
        serverType: "mapserver",
        projection: projection,
        crossOrigin: "anonymous",
      }),
    });

    const bohrungenLayer = new Vector({
      source: new VectorSource(),
    });

    const kantonsgrenzeLayer = new Vector({
      source: new VectorSource(),
      style: new Style({
        stroke: new Stroke({
          color: "#8b0000",
          width: 3,
        }),
      }),
    });

    // Create map and add layers
    const initialMap = new Map({
      target: mapElement.current,
      layers: [landeskarte, bohrungenLayer, kantonsgrenzeLayer],
      view: new View({
        projection: projection,
        zoom: 2,
      }),
    });

    // Save map and vector layer references to state
    setMap(initialMap);
    setBohrungenLayer(bohrungenLayer);
    setKantonsgrenzeLayer(kantonsgrenzeLayer);
  }, []);

  // Set Bohrungen to layer and center map around Bohrungen
  useEffect(() => {
    if (bohrungen?.length) {
      bohrungenLayer.setSource(
        new VectorSource({
          features: bohrungen,
        })
      );
      map.getView().fit(bohrungenLayer.getSource().getExtent(), {
        padding: [30, 30, 30, 30],
      });
    }
  }, [bohrungen, bohrungenLayer, map]);

  // Set Kantonsgrenze to layer
  useEffect(() => {
    if (kantonsgrenze?.length) {
      kantonsgrenzeLayer.setSource(
        new VectorSource({
          features: kantonsgrenze,
        })
      );
    }
  }, [kantonsgrenzeLayer, kantonsgrenze]);

  return (
    <div>
      <div ref={mapElement} className="map-container"></div>
    </div>
  );
}
