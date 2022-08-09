import React, { useState, useEffect, useRef } from "react";
import ReactDOMServer from "react-dom/server";
import AllOutIcon from "@mui/icons-material/AllOut";
import Map from "ol/Map";
import View from "ol/View";
import TileLayer from "ol/layer/Tile";
import WMTS from "ol/source/WMTS";
import VectorSource from "ol/source/Vector";
import Feature from "ol/Feature";
import Overlay from "ol/Overlay";
import { optionsFromCapabilities } from "ol/source/WMTS";
import { WMTSCapabilities } from "ol/format";
import { Projection, addProjection } from "ol/proj";
import { Vector } from "ol/layer";
import { Point } from "ol/geom";
import { Style, Circle, Fill, Stroke } from "ol/style";
import { Select } from "ol/interaction";
import { click } from "ol/events/condition";
import { ZoomToExtent, defaults as defaultControls } from "ol/control";
import { ZoomToLatest } from "./ZoomToLatestControl";
import Popup from "./Popup";
import "ol/ol.css";

export default function MainMap(props) {
  const { standorte, unfilteredBohrungenLength, readOnly, setCurrentStandort, setOpenStandortForm } = props;
  const [map, setMap] = useState();
  const [bohrungenLayer, setBohrungenLayer] = useState();
  const [latestExtent, setLatestExtent] = useState();
  const [doZoom, setDoZoom] = useState(true);
  const [selectedFeature, setSelectedFeature] = useState();
  const [select, setSelect] = useState();
  const [popupVisible, setPopupVisible] = useState(false);
  const [popup, setPopup] = useState();

  const mapElement = useRef();
  const popupElement = useRef();

  const handleZoomToLatestExtend = () => setDoZoom(true);
  const closePopup = () => {
    setPopupVisible(false);
    select.getFeatures().clear();
  };

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

  const selectedStyleFunction = (feature) => {
    if (selectedFeature !== feature) {
      setSelectedFeature(feature);
      return new Style({
        image: new Circle({
          radius: 5,
          stroke: new Stroke({
            color: [233, 197, 19, 1],
            width: 5,
          }),
          fill: new Fill({
            color: [25, 118, 210, 0.3],
          }),
        }),
      });
    } else {
      return defaultStyle;
    }
  };

  // Initialize map on first render
  useEffect(() => {
    // Add custom projection for LV95
    const projection = new Projection({
      code: "EPSG:2056",
      extent: [2572670, 1211017, 2664529, 1263980],
      units: "m",
    });
    addProjection(projection);

    // Add controls
    const htmlIcon = ReactDOMServer.renderToStaticMarkup(<AllOutIcon />);
    const icon = new DOMParser().parseFromString(htmlIcon, "text/html").getElementsByTagName("svg")[0];
    icon.setAttribute("style", "padding-right: 2px; padding-bottom: 2px");

    const controls = defaultControls().extend([
      new ZoomToLatest(handleZoomToLatestExtend),
      new ZoomToExtent({
        label: icon,
        extent: projection.getExtent(),
      }),
    ]);

    // Create map and feature layer
    const bohrungenLayer = new Vector({
      zIndex: 1,
      source: new VectorSource(),
      style: defaultStyle,
    });

    const initialMap = new Map({
      controls: controls,
      target: mapElement.current,
      layers: [bohrungenLayer],
      view: new View({
        projection: projection,
        maxZoom: 14,
        zoom: 2,
      }),
    });

    // Add selection logic
    const clearSelect = () => {
      selectClick.getFeatures().clear();
      setSelectedFeature(null);
      popup.setPosition(null);
    };

    const selectClick = new Select({
      condition: click,
      style: selectedStyleFunction,
    });
    initialMap.addInteraction(selectClick);

    // Clear selection on random click
    initialMap.on("click", clearSelect);

    // Add info popup
    let popup = new Overlay({
      element: popupElement.current,
      autoPan: true,
      autoPanAnimation: { duration: 250 },
    });

    initialMap.addOverlay(popup);

    // Save map and vector layer references to state
    setMap(initialMap);
    setBohrungenLayer(bohrungenLayer);
    setPopup(popup);
    setSelect(selectClick);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  // Handle asynchronous calls to layer sources
  // Baselayer
  useEffect(() => {
    const parser = new WMTSCapabilities();
    const url = "https://geo.so.ch/api/wmts/1.0.0/WMTSCapabilities.xml";
    fetch(url)
      .then(function (response) {
        return response.text();
      })
      .then(function (text) {
        const result = parser.read(text);
        const options = optionsFromCapabilities(result, {
          layer: "ch.so.agi.hintergrundkarte_sw",
        });
        const landeskarte = new TileLayer({
          source: new WMTS(options),
          zIndex: 0,
        });
        map && map.addLayer(landeskarte);
      });
  }, [map]);

  // BohrungenLayer
  useEffect(() => {
    if (standorte && bohrungenLayer) {
      let parsedFeatures;
      let bohrungen = standorte?.flatMap((s) => s.bohrungen);
      if (bohrungen.length) {
        parsedFeatures = bohrungen
          .filter((f) => f.geometrie)
          .map(
            (f) =>
              new Feature({
                geometry: new Point([f.geometrie.coordinates[0], f.geometrie.coordinates[1]]),
                bezeichnung: f.bezeichnung,
                gemeinde: standorte.find((s) => s.id === f.standortId).gemeinde,
                grundbuchNr: standorte.find((s) => s.id === f.standortId).grundbuchNr,
                datum: f.datum,
                standortId: f.standortId,
              })
          );
      } else {
        parsedFeatures = [];
      }
      bohrungenLayer.setSource(
        new VectorSource({
          features: parsedFeatures,
        })
      );
      if (bohrungen.length && unfilteredBohrungenLength) {
        let currentExtent;
        if (bohrungen.length !== unfilteredBohrungenLength) {
          currentExtent = bohrungenLayer.getSource().getExtent();
          map.getView().fit(currentExtent, {
            padding: [30, 30, 30, 30],
            maxZoom: 10,
          });
        } else {
          currentExtent = map.getView().getProjection().getExtent();
          map.getView().fit(currentExtent);
        }
        setLatestExtent(currentExtent);
      }
    }

    // Fix bug where map wasnt displayed after page
    setTimeout(() => {
      map && map.updateSize();
    }, 300);
  }, [standorte, bohrungenLayer, map, unfilteredBohrungenLength]);

  // Handle event from zoom to latest control
  useEffect(() => {
    if (map && doZoom && latestExtent) {
      map.getView().fit(latestExtent, {
        padding: [30, 30, 30, 30],
        maxZoom: 10,
      });
      setDoZoom(false);
    }
  }, [doZoom, latestExtent, map]);

  // Handle event from info button control
  useEffect(() => {
    if (selectedFeature) {
      popup && popup.setPosition(selectedFeature.values_.geometry.flatCoordinates);
      popup && popup.setPositioning("top-center");
      setPopupVisible(true);
    }
  }, [map, popup, selectedFeature]);

  return (
    <div>
      <div ref={mapElement} className="map-container"></div>
      <Popup
        closePopup={closePopup}
        selectedFeature={selectedFeature}
        standorte={standorte}
        popupVisible={popupVisible}
        popupElement={popupElement}
        readOnly={readOnly}
        setCurrentStandort={setCurrentStandort}
        setOpenStandortForm={setOpenStandortForm}
      ></Popup>
    </div>
  );
}
