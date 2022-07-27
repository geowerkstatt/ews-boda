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
import ArrowLeftIcon from "@mui/icons-material/ArrowLeft";
import ArrowRightIcon from "@mui/icons-material/ArrowRight";
import DetailMap from "./DetailMap";
import DateUserInputs from "./DateUserInputs";
import { getDistance } from "ol/sphere";
import { transform } from "ol/proj";
import { register as registerProjection } from "ol/proj/proj4";
import proj4 from "proj4";

export default function BohrungForm(props) {
  const { currentStandort, currentBohrung, setCurrentBohrung, handleNext, handleBack, addBohrung, editBohrung } = props;
  const { control, handleSubmit, formState, reset, register, setValue } = useForm({
    reValidateMode: "onChange",
  });
  const { isDirty } = formState;
  const [ablenkungCodes, setAblenkungCodes] = useState([]);
  const [qualitaetCodes, setQualitaetCodes] = useState([]);
  const [xCoordinate, setXCoordinate] = useState();
  const [yCoordinate, setYCoordinate] = useState();

  const currentBohrungIndex = currentStandort.bohrungen?.indexOf(currentBohrung) || 0;
  const numberOfBohrungen = currentStandort.bohrungen?.length || 0;

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

  // Set form values for coordinates if coordinates are changed from DetailMap component
  useEffect(() => {
    if (currentBohrung?.geometrie?.coordinates) {
      const x = currentBohrung.geometrie.coordinates[0];
      const y = currentBohrung.geometrie.coordinates[1];
      setXCoordinate(x);
      setYCoordinate(y);
      setValue("x_coordinate", x.toFixed(1), { shouldValidate: true, shouldTouch: true });
      setValue("y_coordinate", y.toFixed(1), { shouldValidate: true, shouldTouch: true });
    }
  }, [currentBohrung, setValue]);

  // Register projection for distance validation
  useEffect(() => {
    proj4.defs(
      "EPSG:2056",
      "+proj=somerc +lat_0=46.95240555555556 +lon_0=7.439583333333333 +k_0=1 +x_0=2600000 +y_0=1200000 +ellps=bessel +towgs84=674.374,15.056,405.346,0,0,0,0 +units=m +no_defs"
    );
    registerProjection(proj4);
  }, []);

  const currentInteraction = currentBohrung?.id ? "edit" : currentBohrung?.bezeichnung ? "copy" : "add";

  const onSubmit = (formData) => {
    formData.geometrie = { coordinates: [Number(formData.x_coordinate), Number(formData.y_coordinate)], type: "Point" };
    currentBohrung.id
      ? editBohrung(formData).finally(() => reset(formData))
      : addBohrung(formData).finally(() => reset(formData));
  };

  const onNavigateNext = () => setCurrentBohrung(currentStandort.bohrungen[currentBohrungIndex + 1]);
  const onNavigatePrevious = () => setCurrentBohrung(currentStandort.bohrungen[currentBohrungIndex - 1]);

  const validateXCoordinate = (value) => value > 2590000 && value < 2646000;
  const validateYCoordinate = (value) => value > 1212000 && value < 1264000;

  const validateDistance = () => {
    let isValid = false;
    if (numberOfBohrungen === 0 || (currentInteraction === "edit" && numberOfBohrungen === 1)) {
      isValid = true;
    } else if (xCoordinate && yCoordinate) {
      const src = "EPSG:2056";
      const dest = "EPSG:4326";
      const newCoordinates = transform([xCoordinate, yCoordinate], src, dest);
      isValid = currentStandort.bohrungen
        .map((b) => {
          const existingCoordinates = transform([b.geometrie.coordinates[0], b.geometrie.coordinates[1]], src, dest);
          return getDistance(newCoordinates, existingCoordinates);
        })
        .every((distance) => distance <= 200);
    }
    return isValid;
  };

  return (
    <Box component="form" name="bohrung-form" onSubmit={handleSubmit(onSubmit)}>
      <DialogTitle>
        {currentInteraction === "edit"
          ? "Bohrung bearbeiten"
          : currentInteraction === "copy"
          ? "Bohrung kopieren"
          : "Bohrung erstellen"}
        {currentBohrung?.id && currentBohrungIndex > 0 && (
          <Tooltip title="Zur vorherigen Bohrung">
            <IconButton onClick={onNavigatePrevious} color="primary">
              <ArrowLeftIcon />
            </IconButton>
          </Tooltip>
        )}
        {currentBohrung?.id && currentBohrungIndex < numberOfBohrungen - 1 && (
          <Tooltip title="Zur nächsten Bohrung">
            <IconButton onClick={onNavigateNext} color="primary">
              <ArrowRightIcon />
            </IconButton>
          </Tooltip>
        )}
      </DialogTitle>
      <DialogContent>
        <Controller
          name="bezeichnung"
          control={control}
          render={({ field, fieldState: { error } }) => (
            <TextField
              {...field}
              autoFocus
              value={field.value || currentBohrung?.bezeichnung || ""}
              margin="normal"
              label="Bezeichnung der Bohrung"
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
          render={({ field }) => (
            <TextField
              {...field}
              value={field.value || currentBohrung?.bemerkung || ""}
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
            render={({ field }) => (
              <DatePicker
                label="Datum des Bohrbeginns"
                disableFuture
                inputFormat="dd.MM.yyyy"
                value={field.value || currentBohrung?.datum || null}
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
          name="durchmesser"
          control={control}
          render={({ field }) => (
            <TextField
              {...field}
              value={field.value || currentBohrung?.durchmesserBohrloch || ""}
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
          render={({ field }) => (
            <Autocomplete
              {...field}
              options={ablenkungCodes.map((c) => c.id)}
              value={field.value || currentBohrung?.ablenkungId || null}
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
          render={({ field }) => (
            <Autocomplete
              {...field}
              options={qualitaetCodes.map((c) => c.id)}
              value={field.value || currentBohrung?.qualitaetId || null}
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
          render={({ field }) => (
            <TextField
              {...field}
              value={field.value || currentBohrung?.qualitaetBemerkung || ""}
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
          render={({ field }) => (
            <TextField
              {...field}
              value={field.value || currentBohrung?.quelleRef || ""}
              margin="normal"
              label="Autor der geologischen Aufnahme"
              type="text"
              sx={{ width: "47%" }}
              variant="standard"
            />
          )}
        />
        {currentBohrung?.id && <DateUserInputs formObject={currentBohrung}></DateUserInputs>}

        <Accordion sx={{ boxShadow: "none" }} defaultExpanded={true}>
          <Tooltip title="Übersichtskarte anzeigen">
            <AccordionSummary expandIcon={<ExpandMoreIcon />}>
              {currentInteraction === "add"
                ? "Position der Bohrung durch Klicken in der Karte wählen"
                : "Position der Bohrung durch Klicken in der Karte ändern"}
            </AccordionSummary>
          </Tooltip>
          <AccordionDetails>
            <DetailMap
              bohrungen={[currentBohrung]}
              currentForm={"bohrung"}
              setCurrentBohrung={setCurrentBohrung}
            ></DetailMap>
          </AccordionDetails>
        </Accordion>
        <Controller
          name="x_coordinate"
          control={control}
          render={({ field, fieldState: { error } }) => (
            <TextField
              {...field}
              sx={{ marginRight: "6%", width: "47%" }}
              margin="normal"
              InputLabelProps={{ shrink: xCoordinate != null || field.value != null }}
              value={field.value}
              onChange={(value) => field.onChange(value)}
              label="X-Koordinate der Bohrung"
              type="number"
              variant="standard"
              {...register("x_coordinate", {
                validate: {
                  range: (v) => validateXCoordinate(v) || "Die X-Koordinate muss zwischen 2590000 und 2646000 liegen",
                  distance: (v) =>
                    validateDistance(v) ||
                    "Die Distanz der neu erstellten Bohrung darf nicht mehr als 200m zu den bereits vorhandenen Bohrungen betragen",
                },
              })}
              error={error !== undefined}
              helperText={(error && error.message) || ""}
            />
          )}
        />

        <Controller
          name="y_coordinate"
          control={control}
          render={({ field, fieldState: { error } }) => (
            <TextField
              {...field}
              sx={{ width: "47%" }}
              margin="normal"
              InputLabelProps={{ shrink: yCoordinate != null || field.value != null }}
              value={field.value}
              onChange={(value) => field.onChange(value)}
              label="Y-Koordinate der Bohrung"
              type="number"
              variant="standard"
              {...register("y_coordinate", {
                validate: {
                  range: (v) => validateYCoordinate(v) || "Die Y-Koordinate muss zwischen 1212000 und 1264000 liegen",
                  distance: (v) =>
                    validateDistance(v) ||
                    "Die Distanz der neu erstellten Bohrung darf nicht mehr als 200m zu den bereits vorhandenen Bohrungen betragen",
                },
              })}
              error={error !== undefined}
              helperText={(error && error.message) || ""}
            />
          )}
        />

        <Typography sx={{ marginTop: "15px" }} variant="h6" gutterBottom>
          Bohrprofile ({currentBohrung?.bohrprofile ? currentBohrung.bohrprofile.length : 0})
          {currentBohrung?.id != null && (
            <Tooltip title="Bohrprofil hinzufügen">
              <IconButton color="primary">
                <AddCircleIcon />
              </IconButton>
            </Tooltip>
          )}
          {currentBohrung?.id == null && (
            <IconButton color="primary" disabled>
              <AddCircleIcon />
            </IconButton>
          )}
        </Typography>
        {currentBohrung?.id == null && (
          <Typography>Bitte speichern Sie die Bohrung bevor Sie Bohrprofile hinzufügen.</Typography>
        )}
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
