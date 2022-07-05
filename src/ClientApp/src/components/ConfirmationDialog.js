import * as React from "react";
import Button from "@mui/material/Button";
import Dialog from "@mui/material/Dialog";
import DialogActions from "@mui/material/DialogActions";
import DialogContent from "@mui/material/DialogContent";
import DialogContentText from "@mui/material/DialogContentText";
import DialogTitle from "@mui/material/DialogTitle";

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
            OK
          </Button>
        </DialogActions>
      </Dialog>
    </div>
  );
}
