import React, { useState, useEffect, useRef } from "react";
import Map from "ol/Map";
import View from "ol/View";
import VectorSource from "ol/source/Vector";
import Feature from "ol/Feature";
import { Vector } from "ol/layer";
import { Point } from "ol/geom";
import { Style, Circle, Fill, Stroke } from "ol/style";
import { lv95Projection } from "./lv95Projection";
import { loadBasemapLayer } from "./basemapLayer";
import "ol/ol.css";

export default function DetailMap(props) {
  const { bohrungen, currentStandort, currentForm, setCurrentBohrung, readOnly } = props;
  const [map, setMap] = useState();
  const [bohrungenLayer, setBohrungenLayer] = useState();
  const [geometrie, setGeometrie] = useState();

  const mapElement = useRef();

  const defaultStyle = new Style({
    image: new Circle({
      radius: 4,
      stroke: new Stroke({
        color: [25, 118, 210, 1],
        width: 2,
      }),
      fill: new Fill({
        color: [25, 118, 210, 0.3],
      }),
    }),
  });

  // Initialize map on first render
  useEffect(() => {
    // Create map and feature layer
    const initialBohrungenLayer = new Vector({
      zIndex: 1,
      source: new VectorSource(),
      style: defaultStyle,
    });

    const initialMap = new Map({
      target: mapElement.current,
      layers: [initialBohrungenLayer],
      view: new View({
        projection: lv95Projection,
        maxZoom: 14,
        zoom: 1,
        extent: lv95Projection.getExtent(),
      }),
    });

    // Allow editing of geometry if map is displayed in bohrung form.
    if (currentForm === "bohrung" && !readOnly) {
      initialMap.on("singleclick", function (evt) {
        setGeometrie({ type: "Point", coordinates: evt.coordinate });
      });
    }

    // Save map and vector layer references to state
    setMap(initialMap);
    setBohrungenLayer(initialBohrungenLayer);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  // Handle asynchronous calls to layer sources
  // Baselayer
  useEffect(() => {
    map && loadBasemapLayer().then((basemapLayer) => map.addLayer(basemapLayer));
  }, [map]);

  // BohrungenLayer
  useEffect(() => {
    if (bohrungen && bohrungenLayer) {
      let parsedFeatures;
      if (bohrungen.length && bohrungen.some((bohrung) => bohrung?.geometrie)) {
        parsedFeatures = bohrungen.map(
          (f) =>
            new Feature({
              geometry: new Point([f.geometrie?.coordinates[0], f.geometrie?.coordinates[1]]),
            }),
        );
      } else {
        parsedFeatures = [];
      }
      bohrungenLayer.setSource(
        new VectorSource({
          features: parsedFeatures,
        }),
      );
      let currentExtent;

      // Fit map to extent of Bohrungen if at least one was passed to the DetailMap component
      if (bohrungen.length && bohrungen.some((bohrung) => bohrung?.geometrie)) {
        currentExtent = bohrungenLayer.getSource().getExtent();

        // If no Bohrungen were passed to the DetailMap component, but the current Standort already contains at least one Bohrung, fit extent to first Bohrung of Standort.
      } else if (currentStandort?.bohrungen?.some((bohrung) => bohrung?.geometrie)) {
        const bohrungPoint = new Point([
          currentStandort?.bohrungen[0].geometrie?.coordinates[0],
          currentStandort?.bohrungen[0].geometrie?.coordinates[1],
        ]);
        currentExtent = bohrungPoint.getExtent();

        // If current Standort contains no Bohrungen fit extent to Kanton Solothurn.
      } else {
        currentExtent = map.getView().getProjection().getExtent();
      }
      // Only update map extent if geometry of Bohrung was not changed from the DetailMap component
      if (!geometrie) {
        map.getView().fit(currentExtent, {
          padding: [30, 30, 30, 30],
          maxZoom: 10,
        });
      }
    }
  }, [bohrungen, bohrungenLayer, currentStandort, geometrie, map]);

  // Update currentBohrung on geometry change
  useEffect(() => {
    if (geometrie) {
      const updatedBohrung = {
        ...bohrungen[0],
        geometrie: geometrie,
        "Standort Id": bohrungen[0].standortId,
      };
      setCurrentBohrung(updatedBohrung);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [geometrie]);

  return (
    <div>
      <div ref={mapElement} className="detail-map"></div>
    </div>
  );
}
