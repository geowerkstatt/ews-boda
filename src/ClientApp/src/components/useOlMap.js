import { useEffect, useRef, useState } from "react";
import { loadBasemapLayer } from "./basemapLayer";

/**
 * Keeps one OpenLayers map for the lifetime of the component. The map is created once by `createMap`,
 * rendered into the element behind the returned `mapElement` ref, and gets the basemap below its own layers.
 */
export function useOlMap(createMap) {
  const [map] = useState(createMap);
  const mapElement = useRef(null);

  useEffect(() => {
    map.setTarget(mapElement.current);
    return () => map.setTarget(undefined);
  }, [map]);

  useEffect(() => {
    let basemapLayer;
    let isCancelled = false;
    loadBasemapLayer().then((layer) => {
      if (isCancelled) return;
      basemapLayer = layer;
      map.addLayer(layer);
    });
    return () => {
      isCancelled = true;
      if (basemapLayer) map.removeLayer(basemapLayer);
    };
  }, [map]);

  return { map, mapElement };
}
