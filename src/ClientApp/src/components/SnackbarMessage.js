import * as React from "react";
import Alert from "@mui/material/Alert";
import Snackbar from "@mui/material/Snackbar";

export default function SnackbarMessage(props) {
  const { showSuccessAlert, setShowSuccessAlert, message, variant } = props;
  return (
    <Snackbar open={showSuccessAlert} autoHideDuration={5000} onClose={() => setShowSuccessAlert(false)}>
      <Alert onClose={() => setShowSuccessAlert(false)} severity={variant} sx={{ width: "100%" }}>
        {message}
      </Alert>
    </Snackbar>
  );
}
