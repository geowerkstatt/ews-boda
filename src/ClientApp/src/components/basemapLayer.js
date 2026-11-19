import TileLayer from "ol/layer/Tile";
import WMTS, { optionsFromCapabilities } from "ol/source/WMTS";
import { WMTSCapabilities } from "ol/format";

const capabilitiesUrl = "https://geo.so.ch/api/wmts/1.0.0/WMTSCapabilities.xml";
const basemapLayerName = "ch.so.agi.hintergrundkarte_sw";

/** Loads the geo.so.ch grey basemap as a tile layer drawn below all other layers. */
export async function loadBasemapLayer() {
  const response = await fetch(capabilitiesUrl);
  const capabilities = new WMTSCapabilities().read(await response.text());
  const options = optionsFromCapabilities(capabilities, { layer: basemapLayerName });
  return new TileLayer({
    source: new WMTS(options),
    zIndex: 0,
  });
}
