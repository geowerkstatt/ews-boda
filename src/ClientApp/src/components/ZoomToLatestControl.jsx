import React from "react";
import { Control } from "ol/control";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import { createMapControlIconFactory } from "./mapControlIcon";

const createIcon = createMapControlIconFactory(<ArrowBackIcon />);

/** Map control that fits the view to the extent of the latest search result, as set by `setExtent`. */
export class ZoomToLatest extends Control {
  constructor() {
    const button = document.createElement("button");
    button.appendChild(createIcon());
    button.setAttribute("type", "button");
    button.setAttribute("title", "Extent aktuelle Suche");

    const element = document.createElement("div");
    element.className = "ol-unselectable ol-control ol-zoom-to-latest";
    element.appendChild(button);

    super({
      element: element,
    });

    this.extent = undefined;
    button.addEventListener("click", () => this.zoomToExtent(), false);
  }

  hasExtent() {
    return this.extent !== undefined;
  }

  setExtent(extent) {
    this.extent = extent;
  }

  zoomToExtent() {
    if (!this.extent) return;
    const view = this.getMap().getView();
    view.fit(this.extent, {
      padding: [30, 30, 30, 30],
      maxZoom: 10,
    });
  }
}
