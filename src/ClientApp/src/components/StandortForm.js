import React, { useState, useEffect } from "react";
import { Controller, useForm } from "react-hook-form";
import {
  Box,
  Button,
  DialogActions,
  DialogContent,
  DialogTitle,
  FormControlLabel,
  IconButton,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  TextField,
  Tooltip,
  Typography,
} from "@mui/material";
import DeleteIcon from "@mui/icons-material/Delete";
import ContentCopyIcon from "@mui/icons-material/ContentCopy";
import EditIcon from "@mui/icons-material/Edit";
import PreviewIcon from "@mui/icons-material/Preview";
import AddCircleIcon from "@mui/icons-material/AddCircle";
import DetailMap from "./DetailMap";
import ConfirmationDialog from "./ConfirmationDialog";
import DateUserInputs from "./DateUserInputs";
import Checkbox from "@mui/material/Checkbox";
import FormGroup from "@mui/material/FormGroup";
import { CodeTypes } from "./Codetypes";
import { UserRolesMap } from "../UserRolesMap";

export default function StandortForm(props) {
  const {
    currentStandort,
    currentBohrung,
    setCurrentBohrung,
    handleNext,
    handleClose,
    editStandort,
    addStandort,
    deleteBohrung,
    currentUser,
    readOnly,
  } = props;
  const { control, handleSubmit, formState, reset, register, setValue } = useForm({
    reValidateMode: "onChange",
  });
  const { isDirty } = formState;

  const [openConfirmation, setOpenConfirmation] = useState(false);
  const [afuFreigabe, setAfuFreigabe] = useState();

  const onAddBohrung = () => {
    let bohrung = {
      standortId: currentStandort.id,
      bohrprofile: [],
      hQualitaet: CodeTypes.Bohrung_hquali,
      hAblenkung: CodeTypes.Bohrung_hablenkung,
    };
    setCurrentBohrung(bohrung);
    handleNext();
  };

  const onEditBohrung = (bohrung) => {
    setCurrentBohrung(bohrung);
    handleNext();
  };

  const onCopyBohrung = (bohrung) => {
    let bohrungToCopy = structuredClone(bohrung);
    delete bohrungToCopy.id;
    delete bohrungToCopy.bohrprofile;
    delete bohrungToCopy.erstellungsdatum;
    delete bohrungToCopy.mutationsdatum;
    delete bohrungToCopy.userMutation;
    // will be preserved via ablenkungId and qualitaetId
    delete bohrungToCopy.ablenkung;
    delete bohrungToCopy.qualitaet;
    setCurrentBohrung(bohrungToCopy);
    handleNext();
  };

  const onDeleteBohrung = (bohrung) => {
    setCurrentBohrung(bohrung);
    setOpenConfirmation(true);
  };

  const confirmDeleteBohrung = (confirmation) => {
    if (confirmation) {
      deleteBohrung(currentBohrung);
    }
    setOpenConfirmation(false);
  };

  const onSubmit = (formData) => {
    currentStandort
      ? editStandort(formData).finally(() => reset(formData))
      : addStandort(formData).finally(() => reset(formData));
  };

  useEffect(() => {
    if (afuFreigabe) {
      setValue("afuUser", currentUser.name);
      setValue("afuDatum", new Date().toLocaleDateString());
    } else {
      setValue("afuUser", "");
      setValue("afuDatum", "");
    }
  }, [afuFreigabe, currentUser, setValue]);

  return (
    <Box component="form" name="standort-form" onSubmit={handleSubmit(onSubmit)}>
      <DialogTitle>{currentStandort ? "Standort bearbeiten" : "Standort erstellen"}</DialogTitle>
      <DialogContent>
        <Stack
          direction={{ xs: "column", sm: "column", md: "row" }}
          justifyContent="space-evenly"
          alignItems="flex-start"
          spacing={2}
        >
          <Box sx={{ width: { xs: "100%", sm: "100%", md: "50%" } }}>
            <Controller
              name="bezeichnung"
              control={control}
              defaultValue={currentStandort?.bezeichnung}
              render={({ field, fieldState: { error } }) => (
                <TextField
                  {...field}
                  value={field.value}
                  autoFocus
                  margin="normal"
                  label="Bezeichnung des Standorts"
                  type="text"
                  fullWidth
                  variant="standard"
                  {...register("bezeichnung", { required: true })}
                  error={error !== undefined}
                  helperText={error ? "Geben Sie eine Bezeichnung ein" : ""}
                />
              )}
            />
            <Controller
              name="bemerkung"
              control={control}
              defaultValue={currentStandort?.bemerkung}
              render={({ field }) => (
                <TextField
                  {...field}
                  value={field.value}
                  margin="normal"
                  label="Bemerkung zum Standort"
                  type="text"
                  fullWidth
                  multiline
                  variant="standard"
                  {...register("bemerkung")}
                />
              )}
            />
            <React.Fragment>
              <TextField
                value={currentStandort?.gemeinde}
                fullWidth
                type="text"
                disabled
                variant="standard"
                label="Gemeinde"
              />
              <TextField
                value={currentStandort?.grundbuchNr}
                fullWidth
                type="text"
                inputProps={{
                  maxLength: 40,
                }}
                disabled
                margin="normal"
                label="Grundbuchnummer"
                variant="standard"
              />
            </React.Fragment>
            <React.Fragment>
              <DateUserInputs formObject={currentStandort}></DateUserInputs>
              {currentUser?.role !== UserRolesMap.Extern && (
                <FormGroup sx={{ marginTop: "3%" }}>
                  <FormControlLabel
                    label="Freigabe AfU"
                    control={
                      <Controller
                        name="freigabeAfu"
                        control={control}
                        defaultValue={currentStandort?.freigabeAfu ?? false}
                        render={({ field }) => (
                          <Checkbox
                            {...field}
                            checked={field.value}
                            onChange={(e) => {
                              field.onChange(e.target.checked);
                              setAfuFreigabe(e.target.checked);
                            }}
                          />
                        )}
                        {...register("freigabeAfu")}
                      />
                    }
                  />
                </FormGroup>
              )}
              <React.Fragment>
                <TextField
                  value={currentStandort?.afuUser}
                  InputLabelProps={{ shrink: afuFreigabe }}
                  sx={{ marginRight: "6%", width: "47%" }}
                  disabled
                  margin="normal"
                  label="AfU Freigabe erfolgt durch"
                  type="text"
                  variant="standard"
                  {...register("afuUser")}
                />

                <TextField
                  name="afuDatum"
                  value={currentStandort?.afuDatum ? new Date(currentStandort.afuDatum).toLocaleDateString() : null}
                  InputLabelProps={{ shrink: afuFreigabe }}
                  disabled
                  sx={{ width: "47%" }}
                  margin="normal"
                  label="AfU Freigabe erfolgt am"
                  type="text"
                  variant="standard"
                  {...register("afuDatum")}
                />
              </React.Fragment>
            </React.Fragment>
            {currentStandort?.id == null && (
              <Typography sx={{ marginTop: 3 }}>
                Bitte speichern Sie den Standort bevor Sie Bohrungen hinzufügen.
              </Typography>
            )}
          </Box>
          <Box sx={{ width: { xs: "100%", md: "50%" }, paddingLeft: { xs: 0, md: 4 } }}>
            <Typography>Lokalität der Bohrungen</Typography>
            <DetailMap bohrungen={currentStandort?.bohrungen || []} currentForm={"standort"}></DetailMap>
            <Typography sx={{ marginTop: "15px" }} variant="h6" gutterBottom>
              Bohrungen ({currentStandort?.bohrungen ? currentStandort.bohrungen.length : 0})
              <Tooltip title="Bohrung hinzufügen">
                <span>
                  <IconButton
                    color="primary"
                    name="add-button"
                    onClick={onAddBohrung}
                    disabled={readOnly || currentStandort?.id == null}
                  >
                    <AddCircleIcon />
                  </IconButton>
                </span>
              </Tooltip>
            </Typography>
            {currentStandort?.bohrungen?.length > 0 && (
              <Table name="seach-results-table" size="small">
                <TableHead>
                  <TableRow>
                    <TableCell>Bezeichnung</TableCell>
                    <TableCell>Datum</TableCell>
                    <TableCell></TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {currentStandort.bohrungen.map((bohrung) => (
                    <TableRow key={bohrung.id}>
                      <TableCell>{bohrung.bezeichnung}</TableCell>
                      <TableCell>{(bohrung.datum && new Date(bohrung.datum).toLocaleDateString()) || null}</TableCell>
                      <TableCell align="right">
                        <Tooltip title="Bohrung editieren">
                          <IconButton onClick={() => onEditBohrung(bohrung)} name="edit-button" color="primary">
                            {readOnly ? <PreviewIcon /> : <EditIcon />}
                          </IconButton>
                        </Tooltip>
                        <Tooltip title="Bohrung duplizieren">
                          <IconButton
                            onClick={() => onCopyBohrung(bohrung)}
                            name="copy-button"
                            color="primary"
                            disabled={readOnly}
                          >
                            <ContentCopyIcon />
                          </IconButton>
                        </Tooltip>
                        <Tooltip title="Bohrung löschen">
                          <IconButton
                            onClick={() => onDeleteBohrung(bohrung)}
                            name="delete-button"
                            color="primary"
                            aria-label="delete bohrung"
                            disabled={readOnly}
                          >
                            <DeleteIcon />
                          </IconButton>
                        </Tooltip>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            )}
          </Box>
        </Stack>
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose}>{!isDirty || readOnly ? "Schliessen" : "Abbrechen"}</Button>
        <Button type="submit" disabled={!isDirty || readOnly}>
          Standort speichern
        </Button>
      </DialogActions>
      <ConfirmationDialog
        open={openConfirmation}
        confirm={confirmDeleteBohrung}
        entityName="Bohrung"
      ></ConfirmationDialog>
    </Box>
  );
}
