import ReactDOMServer from "react-dom/server";

/**
 * Renders a MUI icon element to SVG markup once and returns a function that creates a fresh SVG element
 * from it for each map control button, since a DOM element can only be placed in one control.
 */
export function createMapControlIconFactory(iconElement) {
  const markup = ReactDOMServer.renderToStaticMarkup(iconElement);
  return () => {
    const icon = new DOMParser().parseFromString(markup, "text/html").getElementsByTagName("svg")[0];
    icon.setAttribute("style", "padding-right: 2px; padding-bottom: 2px");
    return icon;
  };
}
