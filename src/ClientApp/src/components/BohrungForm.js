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
  const { currentBohrung, handleNext, handleBack, addBohrung, editBohrung } = props;
  const { control, handleSubmit, formState, reset } = useForm({ reValidateMode: "onChange" });
  const { isDirty } = formState;
  const [ablenkungCodes, setAblenkungCodes] = useState([]);
  const [qualitaetCodes, setQualitaetCodes] = useState([]);

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

  const onSubmit = (formData) => {
    currentBohrung.bezeichnung
      ? editBohrung(formData).finally(() => reset(formData))
      : addBohrung(formData).finally(() => reset(formData));
  };

  return (
    <Box component="form" name="bohrung-form" onSubmit={handleSubmit(onSubmit)}>
      <DialogTitle>{currentBohrung && currentBohrung.id ? "Bohrung bearbeiten" : "Bohrung erstellen"}</DialogTitle>
      <DialogContent>
        <Controller
          name="bezeichnung"
          control={control}
          rules={{
            required: true,
          }}
          defaultValue={currentBohrung?.bezeichnung || ""}
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
          defaultValue={currentBohrung?.bemerkung || ""}
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
            defaultValue={currentBohrung?.datum != null ? currentBohrung.datum : null}
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
          defaultValue={currentBohrung?.durchmesserBohrloch || ""}
          render={({ field }) => (
            <TextField
              {...field}
              value={field.value}
              sx={{ width: "47%" }}
              margin="normal"
              label="Durchmesser Bohrloch"
              type="number"
              variant="standard"
            />
          )}
        />
        <Controller
          name="ablenkungId"
          control={control}
          defaultValue={currentBohrung?.ablenkungId || null}
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
          defaultValue={currentBohrung?.qualitaetId || null}
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
          defaultValue={currentBohrung?.qualitaetBemerkung || ""}
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
          defaultValue={currentBohrung?.quelleRef || ""}
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
        {currentBohrung.id && (
          <React.Fragment>
            <TextField
              defaultValue={currentBohrung?.userErstellung}
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
              defaultValue={new Date(currentBohrung?.erstellungsdatum).toLocaleDateString()}
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
              defaultValue={currentBohrung?.userMutation}
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
              defaultValue={
                currentBohrung?.mutationsdatum ? new Date(currentBohrung?.mutationsdatum).toLocaleDateString() : null
              }
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
                <DetailMap bohrungen={[currentBohrung]}></DetailMap>
              </AccordionDetails>
            </Accordion>
          </React.Fragment>
        )}
        <Typography sx={{ marginTop: "15px" }} variant="h6" gutterBottom>
          Bohrprofile ({currentBohrung?.bohrprofile ? currentBohrung.bohrprofile.length : 0})
          <Tooltip title="Bohrprofil hinzufügen">
            <IconButton color="primary">
              <AddCircleIcon />
            </IconButton>
          </Tooltip>
        </Typography>
        {currentBohrung?.bohrprofile?.length > 0 && (
          <React.Fragment>
            {currentBohrung?.bohrprofile?.length > 0 && (
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
                  {currentBohrung.bohrprofile.map((bohrprofil) => (
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
        <Button onClick={handleBack}>Abbrechen</Button>
        <Button type="submit" disabled={!isDirty}>
          Bohrung Speichern
        </Button>
      </DialogActions>
    </Box>
  );
}
