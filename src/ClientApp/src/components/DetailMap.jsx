import React, { useEffect, useEffectEvent, useRef, useState } from "react";
import Map from "ol/Map";
import View from "ol/View";
import VectorSource from "ol/source/Vector";
import Feature from "ol/Feature";
import { Point } from "ol/geom";
import { unByKey } from "ol/Observable";
import { lv95Projection } from "./lv95Projection";
import { createBohrungenLayer } from "./bohrungenLayer";
import { useOlMap } from "./useOlMap";
import "ol/ol.css";

function createMap(bohrungenLayer) {
  return new Map({
    layers: [bohrungenLayer],
    view: new View({
      projection: lv95Projection,
      maxZoom: 14,
      zoom: 1,
      extent: lv95Projection.getExtent(),
    }),
  });
}

function createBohrungFeature(bohrung) {
  return new Feature({
    geometry: new Point([bohrung.geometrie?.coordinates[0], bohrung.geometrie?.coordinates[1]]),
  });
}

function getExtentToShow(bohrungenLayer, hasBohrungGeometrie, currentStandort) {
  // Fit map to extent of Bohrungen if at least one was passed to the DetailMap component
  if (hasBohrungGeometrie) {
    return bohrungenLayer.getSource().getExtent();
  }

  // If no Bohrungen were passed to the DetailMap component, but the current Standort already contains at least one Bohrung, fit extent to first Bohrung of Standort.
  if (currentStandort?.bohrungen?.some((bohrung) => bohrung?.geometrie)) {
    return createBohrungFeature(currentStandort.bohrungen[0]).getGeometry().getExtent();
  }

  // If current Standort contains no Bohrungen fit extent to Kanton Solothurn.
  return lv95Projection.getExtent();
}

export default function DetailMap(props) {
  const { bohrungen, currentStandort, currentForm, setCurrentBohrung, readOnly } = props;
  const [bohrungenLayer] = useState(createBohrungenLayer);
  const { map, mapElement } = useOlMap(() => createMap(bohrungenLayer));
  // Once the Bohrung was moved from this map, the view stays where the user placed it.
  const isBohrungMovedRef = useRef(false);
  const isBohrungEditable = currentForm === "bohrung" && !readOnly;

  const moveBohrung = useEffectEvent((coordinate) => {
    isBohrungMovedRef.current = true;
    setCurrentBohrung({
      ...bohrungen[0],
      geometrie: { type: "Point", coordinates: coordinate },
      "Standort Id": bohrungen[0].standortId,
    });
  });

  useEffect(() => {
    if (!isBohrungEditable) return undefined;
    const clickKey = map.on("singleclick", (event) => moveBohrung(event.coordinate));
    return () => unByKey(clickKey);
  }, [map, isBohrungEditable]);

  useEffect(() => {
    const hasBohrungGeometrie = bohrungen.some((bohrung) => bohrung?.geometrie);
    const features = hasBohrungGeometrie ? bohrungen.map(createBohrungFeature) : [];
    bohrungenLayer.setSource(new VectorSource({ features }));

    if (!isBohrungMovedRef.current) {
      map.getView().fit(getExtentToShow(bohrungenLayer, hasBohrungGeometrie, currentStandort), {
        padding: [30, 30, 30, 30],
        maxZoom: 10,
      });
    }
  }, [bohrungen, bohrungenLayer, currentStandort, map]);

  return (
    <div>
      <div ref={mapElement} className="detail-map"></div>
    </div>
  );
}
