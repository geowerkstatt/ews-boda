import React, { useState, useEffect } from "react";
import { Controller, useForm } from "react-hook-form";
import TextField from "@mui/material/TextField";
import Table from "@mui/material/Table";
import IconButton from "@mui/material/IconButton";
import TableBody from "@mui/material/TableBody";
import TableCell from "@mui/material/TableCell";
import TableHead from "@mui/material/TableHead";
import TableRow from "@mui/material/TableRow";
import EditIcon from "@mui/icons-material/Edit";
import DeleteIcon from "@mui/icons-material/Delete";
import DialogActions from "@mui/material/DialogActions";
import DialogContent from "@mui/material/DialogContent";
import DialogTitle from "@mui/material/DialogTitle";
import AddCircleIcon from "@mui/icons-material/AddCircle";
import Tooltip from "@mui/material/Tooltip";
import Accordion from "@mui/material/Accordion";
import AccordionSummary from "@mui/material/AccordionSummary";
import AccordionDetails from "@mui/material/AccordionDetails";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import { Autocomplete, Box, Button, Typography } from "@mui/material";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import DetailMap from "./DetailMap";

export default function BohrungForm(props) {
  const { bohrung, handleNext, handleBack, setShowSuccessAlert, setAlertMessage, refreshStandort } = props;
  const { control, handleSubmit, formState, reset } = useForm({ reValidateMode: "onBlur" });
  const { isDirty } = formState;
  const [ablenkungCodes, setAblenkungCodes] = useState([]);
  const [qualitaetCodes, setQualitaetCodes] = useState([]);

  const onSubmit = (formData) => {
    bohrung.bezeichnung
      ? submitEditBohrung(formData).finally(() => reset(formData))
      : submitAddBohrung(formData).finally(() => reset(formData));
  };

  // Get codes for dropdowns
  useEffect(() => {
    const getCodes = async () => {
      const ablenkungResponse = await fetch("/code?codetypid=9");
      const qualitaetResponse = await fetch("/code?codetypid=3");
      const ablenkungCodes = await ablenkungResponse.json();
      const qualitaetCodes = await qualitaetResponse.json();
      setAblenkungCodes(ablenkungCodes);
      setQualitaetCodes(qualitaetCodes);
    };
    getCodes();
  }, []);

  async function submitAddBohrung(data) {
    const bohrungToAdd = bohrung;
    Object.entries(data).forEach(([key, value]) => {
      bohrungToAdd[key] = value;
    });
    const response = await fetch("/bohrung", {
      method: "POST",
      cache: "no-cache",
      credentials: "same-origin",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(bohrungToAdd),
    });
    if (response.ok) {
      const addedBohrung = await response.json();
      setShowSuccessAlert(true);
      setAlertMessage("Bohrung wurde hinzugefügt.");
      refreshStandort(addedBohrung.standortId);
      handleBack();
    }
  }

  async function submitEditBohrung(data) {
    console.log(data);
    const updatedBohrung = bohrung;
    Object.entries(data).forEach(([key, value]) => {
      updatedBohrung[key] = value;
    });
    const response = await fetch("/bohrung", {
      method: "PUT",
      cache: "no-cache",
      credentials: "same-origin",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(updatedBohrung),
    });
    if (response.ok) {
      setShowSuccessAlert(true);
      setAlertMessage("Bohrung wurde editiert.");
      refreshStandort(updatedBohrung.standortId);
      handleBack();
    }
  }

  return (
    <Box component="form" name="bohrung-form" onSubmit={handleSubmit(onSubmit)}>
      <DialogTitle>{bohrung && bohrung.id ? "Bohrung bearbeiten" : "Bohrung erstellen"}</DialogTitle>
      <DialogContent>
        <Controller
          name="bezeichnung"
          control={control}
          rules={{
            required: true,
          }}
          defaultValue={bohrung?.bezeichnung}
          render={({ field, fieldState: { error } }) => (
            <TextField
              {...field}
              autoFocus
              value={field.value}
              margin="normal"
              label="Bezeichnung der Bohrung"
              type="text"
              fullWidth
              variant="standard"
              error={error !== undefined}
              helperText={error ? "Geben Sie eine Bezeichnung ein" : ""}
            />
          )}
        />
        <Controller
          name="bemerkung"
          control={control}
          defaultValue={bohrung?.bemerkung}
          render={({ field }) => (
            <TextField
              {...field}
              value={field.value}
              margin="normal"
              label="Bemerkung zur Bohrung"
              type="text"
              fullWidth
              variant="standard"
            />
          )}
        />
        <LocalizationProvider dateAdapter={AdapterDateFns}>
          <Controller
            name="datum"
            control={control}
            defaultValue={bohrung?.datum != null ? bohrung.datum : null}
            render={({ field }) => (
              <DatePicker
                label="Datum des Bohrbeginns"
                disableFuture
                inputFormat="dd.MM.yyyy"
                value={field.value}
                onChange={(value) => field.onChange(value)}
                renderInput={(params) => (
                  <TextField
                    {...field}
                    sx={{ marginRight: "6%", width: "47%" }}
                    margin="normal"
                    variant="standard"
                    {...params}
                  />
                )}
              />
            )}
          />
        </LocalizationProvider>
        <Controller
          name="durchmeser"
          control={control}
          defaultValue={bohrung?.durchmesserBohrloch}
          render={({ field }) => (
            <TextField
              {...field}
              value={field.value}
              sx={{ width: "47%" }}
              margin="normal"
              label="Durchmesser Bohrloch"
              type="text"
              variant="standard"
            />
          )}
        />
        <Controller
          name="ablenkungId"
          control={control}
          defaultValue={bohrung?.ablenkungId || null}
          render={({ field }) => (
            <Autocomplete
              {...field}
              options={ablenkungCodes.map((c) => c.id)}
              value={field.value}
              onChange={(_, data) => field.onChange(data)}
              getOptionLabel={(option) => ablenkungCodes.find((c) => c.id === option)?.kurztext}
              renderInput={(params) => (
                <TextField
                  {...params}
                  margin="normal"
                  label="Ablenkung der Bohrung"
                  type="text"
                  fullWidth
                  variant="standard"
                />
              )}
            />
          )}
        />
        <Controller
          name="qualitaetId"
          control={control}
          defaultValue={bohrung?.qualitaetId || null}
          render={({ field }) => (
            <Autocomplete
              {...field}
              options={qualitaetCodes.map((c) => c.id)}
              value={field.value}
              getOptionLabel={(option) => qualitaetCodes.find((c) => c.id === option)?.kurztext}
              onChange={(_, data) => field.onChange(data)}
              renderInput={(params) => (
                <TextField
                  {...params}
                  fullWidth
                  margin="normal"
                  label="Qualität der Angaben zur Bohrung"
                  type="text"
                  variant="standard"
                />
              )}
            />
          )}
        />
        <Controller
          name="bemerkung-qualitaet"
          control={control}
          defaultValue={bohrung?.qualitaetBemerkung}
          render={({ field }) => (
            <TextField
              {...field}
              value={field.value}
              sx={{ marginRight: "6%", width: "47%" }}
              margin="normal"
              label="Bemerkung zur Qualitätsangabe"
              type="text"
              variant="standard"
            />
          )}
        />
        <Controller
          name="quelleRef"
          control={control}
          defaultValue={bohrung?.quelleRef}
          render={({ field }) => (
            <TextField
              {...field}
              value={field.value}
              margin="normal"
              label="Autor der geologischen Aufnahme"
              type="text"
              sx={{ width: "47%" }}
              variant="standard"
            />
          )}
        />
        {bohrung.id && (
          <React.Fragment>
            <TextField
              defaultValue={bohrung?.userErstellung}
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
              defaultValue={new Date(bohrung?.erstellungsdatum).toLocaleDateString()}
              InputProps={{
                readOnly: true,
              }}
              sx={{ width: "47%" }}
              margin="normal"
              label="Erstellt am"
              type="text"
              variant="standard"
            />
            <TextField
              defaultValue={bohrung?.userMutation}
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
              defaultValue={bohrung?.mutationsdatum ? new Date(bohrung?.mutationsdatum).toLocaleDateString() : null}
              InputProps={{
                readOnly: true,
              }}
              sx={{ width: "47%" }}
              margin="normal"
              label="Zuletzt geändert am"
              type="text"
              variant="standard"
            />
            <Accordion sx={{ boxShadow: "none" }} defaultExpanded={true}>
              <Tooltip title="Übersichtskarte anzeigen">
                <AccordionSummary expandIcon={<ExpandMoreIcon />}>Lokalität der Bohrung</AccordionSummary>
              </Tooltip>
              <AccordionDetails>
                <DetailMap bohrungen={[bohrung]}></DetailMap>
              </AccordionDetails>
            </Accordion>
          </React.Fragment>
        )}
        <Typography sx={{ marginTop: "15px" }} variant="h6" gutterBottom>
          Bohrprofile ({bohrung?.bohrprofile ? bohrung.bohrprofile.length : 0})
          <Tooltip title="Bohrprofil hinzufügen">
            <IconButton color="primary">
              <AddCircleIcon />
            </IconButton>
          </Tooltip>
        </Typography>
        {bohrung?.bohrprofile?.length > 0 && (
          <React.Fragment>
            {bohrung?.bohrprofile?.length > 0 && (
              <Table name="seach-results-table" size="small">
                <TableHead>
                  <TableRow>
                    <TableCell>Datum</TableCell>
                    <TableCell>Endteufe [m]</TableCell>
                    <TableCell></TableCell>
                    <TableCell></TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {bohrung.bohrprofile.map((bohrprofil) => (
                    <TableRow key={bohrprofil.id}>
                      <TableCell>{new Date(bohrprofil.datum).toLocaleDateString()}</TableCell>
                      <TableCell>{bohrprofil.endteufe}</TableCell>
                      <TableCell>
                        <Tooltip title="Bohrprofil editieren">
                          <IconButton onClick={handleNext} color="primary">
                            <EditIcon />
                          </IconButton>
                        </Tooltip>
                      </TableCell>
                      <TableCell>
                        <Tooltip title="Bohrprofil löschen">
                          <IconButton color="primary" aria-label="delete bohrprofil">
                            <DeleteIcon />
                          </IconButton>
                        </Tooltip>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            )}
          </React.Fragment>
        )}
      </DialogContent>
      <DialogActions>
        <Button onClick={handleBack}>{isDirty ? "Abbrechen" : "Schliessen"}</Button>
        {isDirty && <Button type="submit">Bohrung Speichern</Button>}
      </DialogActions>
    </Box>
  );
}
