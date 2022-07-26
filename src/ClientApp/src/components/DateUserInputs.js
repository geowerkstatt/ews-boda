import React from "react";
import TextField from "@mui/material/TextField";

export default function DateUserInputs(props) {
  const { formObject } = props;
  return (
    <React.Fragment>
      <TextField
        value={formObject.userErstellung}
        sx={{ marginRight: "6%", width: "47%" }}
        InputProps={{
          readOnly: true,
        }}
        margin="normal"
        label="Erstellt durch"
        type="text"
        variant="standard"
      />
      <TextField
        value={new Date(formObject.erstellungsdatum).toLocaleDateString()}
        InputProps={{
          readOnly: true,
        }}
        sx={{ width: "47%" }}
        margin="normal"
        label="Erstellt am"
        type="text"
        variant="standard"
      />
      {formObject.userMutation && (
        <React.Fragment>
          <TextField
            value={formObject.userMutation}
            sx={{ marginRight: "6%", width: "47%" }}
            InputProps={{
              readOnly: true,
            }}
            margin="normal"
            label="Zuletzt geändert durch"
            type="text"
            variant="standard"
          />
          <TextField
            name="mutationsdatum"
            value={formObject.mutationsdatum ? new Date(formObject.mutationsdatum).toLocaleDateString() : null}
            InputProps={{
              readOnly: true,
            }}
            sx={{ width: "47%" }}
            margin="normal"
            label="Zuletzt geändert am"
            type="text"
            variant="standard"
          />
        </React.Fragment>
      )}
    </React.Fragment>
  );
}
