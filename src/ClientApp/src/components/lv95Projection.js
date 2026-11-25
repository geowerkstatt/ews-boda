import proj4 from "proj4";
import { register } from "ol/proj/proj4";
import { get as getProjection } from "ol/proj";

// Registered at module load so that every transform() involving EPSG:2056 can resolve it;
// OpenLayers throws for projections it does not know.
proj4.defs(
  "EPSG:2056",
  "+proj=somerc +lat_0=46.95240555555556 +lon_0=7.439583333333333 +k_0=1 +x_0=2600000 +y_0=1200000 +ellps=bessel +towgs84=674.374,15.056,405.346,0,0,0,0 +units=m +no_defs",
);
register(proj4);

/**
 * Swiss LV95 projection. Its extent is Kanton Solothurn rather than the whole of Switzerland,
 * because the maps use it as the default view and as the target of the zoom-to-extent control.
 * The geo.so.ch basemap only has tiles inside this extent, so the map views are constrained to it.
 */
export const lv95Projection = getProjection("EPSG:2056");
lv95Projection.setExtent([2572670, 1211017, 2664529, 1263980]);
