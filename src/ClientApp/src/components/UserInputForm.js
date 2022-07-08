import React from "react";
import { Controller, useForm } from "react-hook-form";
import { UserRolesMap } from "../UserRolesMap";
import TextField from "@mui/material/TextField";
import DialogActions from "@mui/material/DialogActions";
import DialogContent from "@mui/material/DialogContent";
import DialogTitle from "@mui/material/DialogTitle";
import MenuItem from "@mui/material/MenuItem";

import Box from "@mui/material/Box";
import Button from "@mui/material/Button";

export default function UserInputForm(props) {
  const { control, handleSubmit } = useForm();
  const { handleClose, user, editUser } = props;

  const onSubmit = (formData) => {
    handleClose();
    editUser(formData);
  };

  return (
    <Box component="form" onSubmit={handleSubmit(onSubmit)}>
      <DialogTitle>Benutzer bearbeiten</DialogTitle>
      <DialogContent>
        <Controller
          name="name"
          control={control}
          defaultValue={user.name}
          render={({ field }) => (
            <TextField
              {...field}
              sx={{ width: "45%" }}
              InputProps={{
                readOnly: true,
              }}
              margin="normal"
              label="Benutzername"
              type="text"
              variant="standard"
            />
          )}
        />
        <Controller
          name="role"
          control={control}
          defaultValue={user.role}
          render={({ field }) => (
            <TextField
              {...field}
              sx={{ marginLeft: "6%", marginTop: "10px", width: "45%" }}
              select
              label="Rolle"
              data-cy="select-user-role"
            >
              {Object.entries(UserRolesMap).map(([key, value], index) => (
                <MenuItem key={index} value={key} data-cy={`role-id-${key}`}>
                  {value}
                </MenuItem>
              ))}
            </TextField>
          )}
        />
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose}>Abbrechen</Button>
        <Button type="submit">Speichern</Button>
      </DialogActions>
    </Box>
  );
}
