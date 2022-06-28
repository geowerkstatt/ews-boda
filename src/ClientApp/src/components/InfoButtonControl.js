import React from "react";
import ReactDOMServer from "react-dom/server";
import { Control } from "ol/control";
import InfoOutlinedIcon from "@mui/icons-material/InfoOutlined";

export class InfoButton extends Control {
  constructor(handleInfoClick) {
    const button = document.createElement("button");

    const htmlIcon = ReactDOMServer.renderToStaticMarkup(<InfoOutlinedIcon />);
    const icon = new DOMParser().parseFromString(htmlIcon, "text/html").getElementsByTagName("svg")[0];
    icon.setAttribute("style", "padding-right: 2px; padding-bottom: 2px");

    button.appendChild(icon);
    button.setAttribute("type", "button");
    button.classList.add("info-button");

    const element = document.createElement("div");
    element.className = "ol-unselectable ol-control";
    element.appendChild(button);

    super({
      element: element,
    });

    button.addEventListener("click", handleInfoClick, false);
  }
}
