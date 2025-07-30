import React from "react";
import { Button, Card, CardActions, CardContent, IconButton, Tooltip, Typography } from "@mui/material";
import EditIcon from "@mui/icons-material/Edit";
import PreviewIcon from "@mui/icons-material/Preview";
import { UserRolesMap } from "../UserRolesMap";

function Popup(props) {
  const {
    popupElement,
    closePopup,
    selectedFeature,
    standorte,
    popupVisible,
    currentUser,
    setCurrentStandort,
    setOpenStandortForm,
  } = props;

  const standort = selectedFeature && standorte.find((s) => s.id === selectedFeature.values_.standortId);
  const readOnly = standort?.freigabeAfu && currentUser?.role === UserRolesMap.Extern;

  const onEditStandort = () => {
    setCurrentStandort(standort);
    setOpenStandortForm(true);
  };

  return (
    <div ref={popupElement} id="popup">
      {popupVisible && selectedFeature && (
        <Card sx={{ minWidth: 275 }}>
          <CardContent>
            <Typography variant="h6">
              {standort?.bezeichnung}
              <Tooltip title={readOnly ? "Standort anzeigen" : "Standort editieren"}>
                <IconButton onClick={onEditStandort} name="edit-button" color="primary">
                  {readOnly ? <PreviewIcon /> : <EditIcon />}
                </IconButton>
              </Tooltip>
            </Typography>
            <Typography color="text.secondary">Bohrung: {selectedFeature.values_.bezeichnung}</Typography>
            <Typography color="text.secondary">Gemeinde: {selectedFeature.values_.gemeinde}</Typography>
            <Typography color="text.secondary">Grundbuchnummer: {selectedFeature.values_.grundbuchNr}</Typography>
            <Typography color="text.secondary">
              Bohrbeginn: {new Date(selectedFeature.values_.datum).toLocaleDateString()}
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
