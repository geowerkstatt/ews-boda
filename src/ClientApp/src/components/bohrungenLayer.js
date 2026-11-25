import VectorLayer from "ol/layer/Vector";
import VectorSource from "ol/source/Vector";
import { Style, Circle, Fill, Stroke } from "ol/style";

const pointFill = new Fill({ color: [25, 118, 210, 0.3] });

export const selectedBohrungStyle = new Style({
  image: new Circle({
    radius: 5,
    stroke: new Stroke({ color: [233, 197, 19, 1], width: 5 }),
    fill: pointFill,
  }),
});

const bohrungStyle = new Style({
  image: new Circle({
    radius: 4,
    stroke: new Stroke({ color: [25, 118, 210, 1], width: 2 }),
    fill: pointFill,
  }),
});

/** Creates the empty layer that draws Bohrungen as points above the basemap. */
export function createBohrungenLayer() {
  return new VectorLayer({
    zIndex: 1,
    source: new VectorSource(),
    style: bohrungStyle,
  });
}
