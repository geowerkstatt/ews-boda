import React from "react";
import Card from "@mui/material/Card";
import CardActions from "@mui/material/CardActions";
import CardContent from "@mui/material/CardContent";
import Button from "@mui/material/Button";
import Typography from "@mui/material/Typography";
import Alert from "@mui/material/Alert";

function Popup(props) {
  const { popupElement, closePopup, selectedFeature, popupVisible } = props;

  const relevantAttributes = ["Bezeichnung", "Standort Id"];
  const filteredFeature =
    selectedFeature?.values_ &&
    Object.keys(selectedFeature?.values_)
      .filter((key) => relevantAttributes.includes(key))
      .reduce((obj, key) => {
        return Object.assign(obj, {
          [key]: selectedFeature?.values_[key],
        });
      }, {});

  return (
    <div ref={popupElement} id="popup">
      {popupVisible && selectedFeature && (
        <Card sx={{ minWidth: 275 }}>
          <CardContent>
            <Typography variant="h6" gutterBottom>
              Bohrung Id: {selectedFeature.values_.Id}
            </Typography>
            {Object.keys(filteredFeature).map((key, keyIndex) => (
              <Typography color="text.secondary" key={keyIndex}>
                {key}: {filteredFeature[key]?.toString()}
              </Typography>
            ))}
          </CardContent>
          <CardActions>
            <Button onClick={closePopup} size="small">
              Schliessen
            </Button>
          </CardActions>
        </Card>
      )}
      {popupVisible && !selectedFeature && <Alert severity="warning">Wählen Sie zuerst eine Bohrung!</Alert>}
    </div>
  );
}

export default Popup;
