import React from "react";
import Card from "@mui/material/Card";
import CardActions from "@mui/material/CardActions";
import CardContent from "@mui/material/CardContent";
import Button from "@mui/material/Button";
import Typography from "@mui/material/Typography";

function Popup(props) {
  const { popupElement, closePopup, selectedFeature, popupVisible } = props;

  return (
    <div ref={popupElement} id="popup">
      {popupVisible && selectedFeature && (
        <Card sx={{ minWidth: 275 }}>
          <CardContent>
            <Typography variant="h6">{selectedFeature.values_.bezeichnung}</Typography>
            <Typography color="text.secondary">Gemeinde: {selectedFeature.values_.gemeinde}</Typography>
            <Typography color="text.secondary">Grundbuchnummer: {selectedFeature.values_.grundbuchNr}</Typography>
            <Typography color="text.secondary">
              Datum der Bohrung: {new Date(selectedFeature.values_.datum).toLocaleDateString()}
            </Typography>
          </CardContent>
          <CardActions>
            <Button onClick={closePopup} size="small">
              Schliessen
            </Button>
          </CardActions>
        </Card>
      )}
    </div>
  );
}

export default Popup;
