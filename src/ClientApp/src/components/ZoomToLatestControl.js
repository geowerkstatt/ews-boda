import React from "react";
import ReactDOMServer from "react-dom/server";
import { Control } from "ol/control";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";

export class ZoomToLatest extends Control {
  constructor(handleZoomToLatestExtend) {
    const button = document.createElement("button");

    const htmlIcon = ReactDOMServer.renderToStaticMarkup(<ArrowBackIcon />);
    const icon = new DOMParser().parseFromString(htmlIcon, "text/html").getElementsByTagName("svg")[0];
    icon.setAttribute("style", "padding-right: 2px; padding-bottom: 2px");

    button.appendChild(icon);
    button.setAttribute("type", "button");
    button.setAttribute("title", "Anischt zurück");

    const element = document.createElement("div");
    element.className = "ol-unselectable ol-control";
    element.appendChild(button);

    super({
      element: element,
    });

    button.addEventListener("click", handleZoomToLatestExtend, false);
  }
}
