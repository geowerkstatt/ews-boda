import React, { useState, useEffect, useRef } from "react";
import AllOutIcon from "@mui/icons-material/AllOut";
import Map from "ol/Map";
import View from "ol/View";
import VectorSource from "ol/source/Vector";
import Feature from "ol/Feature";
import Overlay from "ol/Overlay";
import { Point } from "ol/geom";
import { Select, defaults as defaultInteractions } from "ol/interaction";
import { click } from "ol/events/condition";
import { unByKey } from "ol/Observable";
import { ZoomToExtent, defaults as defaultControls } from "ol/control";
import { ZoomToLatest } from "./ZoomToLatestControl.jsx";
import Popup from "./Popup";
import { lv95Projection } from "./lv95Projection";
import { createBohrungenLayer, selectedBohrungStyle } from "./bohrungenLayer";
import { useOlMap } from "./useOlMap";
import { createMapControlIconFactory } from "./mapControlIcon";
import "ol/ol.css";

function createMapParts() {
  return {
    bohrungenLayer: createBohrungenLayer(),
    select: new Select({ condition: click, style: selectedBohrungStyle }),
    popup: new Overlay({ positioning: "top-center", autoPan: { animation: { duration: 250 } } }),
    zoomToLatest: new ZoomToLatest(),
  };
}

const createZoomToExtentIcon = createMapControlIconFactory(<AllOutIcon />);

function createMap({ bohrungenLayer, select, popup, zoomToLatest }) {
  return new Map({
    controls: defaultControls().extend([
      zoomToLatest,
      new ZoomToExtent({ label: createZoomToExtentIcon(), extent: lv95Projection.getExtent() }),
    ]),
    interactions: defaultInteractions().extend([select]),
    overlays: [popup],
    layers: [bohrungenLayer],
    view: new View({
      projection: lv95Projection,
      maxZoom: 14,
      zoom: 2,
      extent: lv95Projection.getExtent(),
    }),
  });
}

function createBohrungFeatures(standorte) {
  return standorte.flatMap((standort) =>
    standort.bohrungen
      .filter((bohrung) => bohrung.geometrie)
      .map(
        (bohrung) =>
          new Feature({
            geometry: new Point([bohrung.geometrie.coordinates[0], bohrung.geometrie.coordinates[1]]),
            bezeichnung: bohrung.bezeichnung,
            gemeinde: standort.gemeinde,
            grundbuchNr: standort.grundbuchNr,
            datum: bohrung.datum,
            standortId: bohrung.standortId,
          }),
      ),
  );
}

export default function MainMap(props) {
  const { standorte, unfilteredBohrungenLength, currentUser, setCurrentStandort, setOpenStandortForm } = props;
  const [mapParts] = useState(createMapParts);
  const { bohrungenLayer, select, popup, zoomToLatest } = mapParts;
  const { map, mapElement } = useOlMap(() => createMap(mapParts));
  const [selectedFeature, setSelectedFeature] = useState(null);
  const popupElement = useRef(null);

  const closePopup = () => select.clearSelection();

  useEffect(() => {
    popup.setElement(popupElement.current);
    return () => popup.setElement(undefined);
  }, [popup]);

  useEffect(() => {
    const selectKey = select.on("select", () => setSelectedFeature(select.getFeatures().item(0) ?? null));
    return () => unByKey(selectKey);
  }, [select]);

  // Positioned after the popup content has rendered, so that auto-pan measures the filled popup.
  useEffect(() => {
    popup.setPosition(selectedFeature?.getGeometry().getCoordinates());
  }, [popup, selectedFeature]);

  useEffect(() => {
    bohrungenLayer.setSource(new VectorSource({ features: createBohrungFeatures(standorte) }));

    const bohrungenLength = standorte.flatMap((standort) => standort.bohrungen).length;
    if (bohrungenLength && unfilteredBohrungenLength) {
      const isFiltered = bohrungenLength !== unfilteredBohrungenLength;
      const extent = isFiltered ? bohrungenLayer.getSource().getExtent() : lv95Projection.getExtent();
      // The first extent gets the margin of the zoom-to-latest control even when it shows all Bohrungen.
      const isFirstExtent = !zoomToLatest.hasExtent();
      zoomToLatest.setExtent(extent);
      if (isFiltered || isFirstExtent) {
        zoomToLatest.zoomToExtent();
      } else {
        map.getView().fit(extent);
      }
    }
  }, [standorte, unfilteredBohrungenLength, bohrungenLayer, zoomToLatest, map]);

  return (
    <div>
      <div ref={mapElement} className="map-container"></div>
      <Popup
        closePopup={closePopup}
        selectedFeature={selectedFeature}
        standorte={standorte}
        popupElement={popupElement}
        currentUser={currentUser}
        setCurrentStandort={setCurrentStandort}
        setOpenStandortForm={setOpenStandortForm}
      ></Popup>
    </div>
  );
}
