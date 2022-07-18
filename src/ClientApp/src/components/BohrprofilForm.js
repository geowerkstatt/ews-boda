import React from "react";
import DialogActions from "@mui/material/DialogActions";
import DialogContent from "@mui/material/DialogContent";
import DialogTitle from "@mui/material/DialogTitle";
import { useForm } from "react-hook-form";
import { Box, Button } from "@mui/material";

export default function BohrprofilForm(props) {
  const { handleBack } = props;
  const { handleSubmit } = useForm({ reValidateMode: "onBlur" });

  const onSubmit = (formData) => {
    console.log("submit");
  };
  return (
    <Box component="form" onSubmit={handleSubmit(onSubmit)}>
      <DialogTitle>{"Bohrprofil bearbeiten"}</DialogTitle>
      <DialogContent></DialogContent>
      <DialogActions>
        <Button onClick={handleBack}>Abbrechen</Button>
        <Button type="submit">Bohrprofil Speichern</Button>
      </DialogActions>
    </Box>
  );
}
