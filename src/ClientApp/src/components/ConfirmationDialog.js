import * as React from "react";
import { Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle } from "@mui/material";

export default function ConfirmationDialog(props) {
  const { entityName, open, confirm } = props;

  return (
    <div>
      <Dialog open={open} onClose={() => confirm(false)} aria-labelledby="Bestätigungsdialog">
        <DialogTitle>{entityName + " löschen?"}</DialogTitle>
        <DialogContent>
          <DialogContentText>Bestätigen Sie den Löschvorgang.</DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => confirm(false)}>Abbrechen</Button>
          <Button onClick={() => confirm(true)} autoFocus>
            Löschen
          </Button>
        </DialogActions>
      </Dialog>
    </div>
  );
}
