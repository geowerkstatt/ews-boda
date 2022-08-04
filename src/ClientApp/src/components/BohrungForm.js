import React, { useState, useEffect } from "react";
import { Controller, useForm } from "react-hook-form";
import {
  Autocomplete,
  Box,
  Button,
  DialogActions,
  DialogContent,
  DialogTitle,
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
import EditIcon from "@mui/icons-material/Edit";
import PreviewIcon from "@mui/icons-material/Preview";
import DeleteIcon from "@mui/icons-material/Delete";
import ContentCopyIcon from "@mui/icons-material/ContentCopy";
import AddCircleIcon from "@mui/icons-material/AddCircle";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import ArrowLeftIcon from "@mui/icons-material/ArrowLeft";
import ArrowRightIcon from "@mui/icons-material/ArrowRight";
import DetailMap from "./DetailMap";
import DateUserInputs from "./DateUserInputs";
import ConfirmationDialog from "./ConfirmationDialog";
import { CodeTypes } from "./Codetypes";
import { getDistance } from "ol/sphere";
import { transform } from "ol/proj";
import { register as registerProjection } from "ol/proj/proj4";
import proj4 from "proj4";

export default function BohrungForm(props) {
  const {
    currentStandort,
    currentBohrung,
    setCurrentBohrung,
    setCurrentBohrprofil,
    currentBohrprofil,
    handleNext,
    handleBack,
    addBohrung,
    editBohrung,
    deleteBohrprofil,
    readOnly,
  } = props;
  const { control, handleSubmit, formState, reset, register, getValues, setValue } = useForm({
    reValidateMode: "onChange",
  });
  const { isDirty } = formState;
  const [ablenkungCodes, setAblenkungCodes] = useState([]);
  const [qualitaetCodes, setQualitaetCodes] = useState([]);
  const [openConfirmation, setOpenConfirmation] = useState(false);
  const [selectedDate, setSelectedDate] = useState();

  const currentBohrungIndex =
    currentStandort.bohrungen?.indexOf(currentStandort.bohrungen.find((b) => b.id === currentBohrung.id)) || 0;
  const numberOfBohrungen = currentStandort.bohrungen?.length || 0;

  // Get codes for dropdowns
  useEffect(() => {
    const getCodes = async () => {
      const ablenkungResponse = await fetch("/code?codetypid=" + CodeTypes.Bohrung_hablenkung);
      const qualitaetResponse = await fetch("/code?codetypid=" + CodeTypes.Bohrung_hquali);
      const ablenkungCodes = await ablenkungResponse.json();
      const qualitaetCodes = await qualitaetResponse.json();
      setAblenkungCodes(ablenkungCodes);
      setQualitaetCodes(qualitaetCodes);
    };
    getCodes();
  }, []);

  // Set form values for coordinates if coordinates are changed from DetailMap component
  useEffect(() => {
    // Update form values if currentBohrung changes, to allow next/previous navigation.
    if (currentBohrung && !currentBohrung.coordinatesChanged) {
      setValue("bezeichnung", currentBohrung?.bezeichnung);
      setValue("bemerkung", currentBohrung?.bemerkung);
      setValue("durchmesserBohrloch", currentBohrung?.durchmesserBohrloch);
      setValue("ablenkungId", currentBohrung?.ablenkungId);
      setValue("qualitaetId", currentBohrung?.qualitaetId);
      setValue("qualitaetBemerkung", currentBohrung?.qualitaetBemerkung);
      setValue("quelleRef", currentBohrung?.quelleRef);
      //DatePicker state must be handled separately from formState in order to preserve the date format (dd.MM.yyy). Otherwise the date will be stored as an unformatted string on manual input.
      setSelectedDate(currentBohrung?.datum || null);
    }
    if (currentBohrung?.geometrie?.coordinates) {
      const x = currentBohrung.geometrie.coordinates[0];
      const y = currentBohrung.geometrie.coordinates[1];
      setValue("x_coordinate", x.toFixed(1));
      setValue("y_coordinate", y.toFixed(1));
      currentBohrung.coordinatesChanged = false;
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

  const currentInteraction = currentBohrung?.id ? "edit" : "add";

  const onSubmit = (formData) => {
    // Save date as UTC date ignoring the current timezone
    const date = new Date(selectedDate);
    if (!isNaN(date)) {
      formData.datum = new Date(date - date.getTimezoneOffset() * 60000).toISOString();
    }

    formData.geometrie = { coordinates: [Number(formData.x_coordinate), Number(formData.y_coordinate)], type: "Point" };
    currentBohrung.id
      ? editBohrung(formData).finally(() => reset(formData))
      : addBohrung(formData).finally(() => reset(formData));
  };

  const onAddBohrprofil = () => {
    let bohrprofil = {
      bohrungId: currentBohrung.id,
      hFormationEndtiefe: CodeTypes.Bohrprofil_fmeto,
      hFormationFels: CodeTypes.Bohrprofil_fmfelso,
      hQualitaet: CodeTypes.Bohrprofil_hquali,
      hTektonik: CodeTypes.Bohrprofil_htektonik,
      schichten: [],
      vorkommnisse: [],
    };
    setCurrentBohrprofil(bohrprofil);
    handleNext();
  };

  const onEditBohrprofil = (bohrprofil) => {
    setCurrentBohrprofil(bohrprofil);
    handleNext();
  };

  const onCopyBohrprofil = (bohrprofil) => {
    let bohrprofilToCopy = structuredClone(bohrprofil);
    delete bohrprofilToCopy.id;
    delete bohrprofilToCopy.schichten;
    delete bohrprofilToCopy.vorkommnisse;
    delete bohrprofilToCopy.erstellungsdatum;
    delete bohrprofilToCopy.mutationsdatum;
    delete bohrprofilToCopy.userMutation;
    // will be preserved via tektonikId, formationfelsId,formationendtiefeId and qualitaetId
    delete bohrprofilToCopy.tektonik;
    delete bohrprofilToCopy.formationfels;
    delete bohrprofilToCopy.formationendtiefe;
    delete bohrprofilToCopy.qualitaet;
    setCurrentBohrprofil(bohrprofilToCopy);
    handleNext();
  };

  const onDeleteBohrprofil = (bohrprofil) => {
    setCurrentBohrprofil(bohrprofil);
    setOpenConfirmation(true);
  };

  const confirmDeleteBohrprofil = (confirmation) => {
    if (confirmation) {
      deleteBohrprofil(currentBohrprofil);
    }
    setOpenConfirmation(false);
  };

  const onNavigateNext = () => setCurrentBohrung(currentStandort.bohrungen[currentBohrungIndex + 1]);
  const onNavigatePrevious = () => setCurrentBohrung(currentStandort.bohrungen[currentBohrungIndex - 1]);

  const validateXCoordinate = (value) => value > 2590000 && value < 2646000;
  const validateYCoordinate = (value) => value > 1212000 && value < 1264000;

  const validateDistance = (x, y) => {
    let isValid = false;
    if (numberOfBohrungen === 0 || (currentInteraction === "edit" && numberOfBohrungen === 1)) {
      isValid = true;
    } else if (x && y) {
      const src = "EPSG:2056";
      const dest = "EPSG:4326";
      const newCoordinates = transform([Number(x), Number(y)], src, dest);
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
        {currentInteraction === "edit" ? "Bohrung bearbeiten" : "Bohrung erstellen"}
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
              defaultValue={currentBohrung?.bezeichnung}
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
                  {...register("bezeichnung", { required: true })}
                  error={error !== undefined}
                  helperText={error ? "Geben Sie eine Bezeichnung ein" : ""}
                />
              )}
            />
            <Controller
              name="bemerkung"
              control={control}
              defaultValue={currentBohrung?.bemerkung}
              render={({ field }) => (
                <TextField
                  {...field}
                  value={field.value}
                  margin="normal"
                  InputLabelProps={{ shrink: field.value != null }}
                  multiline
                  label="Bemerkung zur Bohrung"
                  type="text"
                  fullWidth
                  variant="standard"
                  {...register("bemerkung")}
                />
              )}
            />
            <LocalizationProvider dateAdapter={AdapterDateFns}>
              <DatePicker
                label="Datum des Bohrbeginns"
                disableFuture
                inputFormat="dd.MM.yyyy"
                value={selectedDate}
                onChange={(value) => {
                  setSelectedDate(value);
                  //setValue only needed to dirty form
                  setValue("datum", value, { shouldDirty: true });
                }}
                disabled={readOnly}
                renderInput={(params) => (
                  <TextField
                    InputLabelProps={{ shrink: selectedDate != null }}
                    sx={{ marginRight: "6%", width: "47%" }}
                    margin="normal"
                    variant="standard"
                    {...register("datum")}
                    {...params}
                  />
                )}
              />
            </LocalizationProvider>
            <Controller
              name="durchmesserBohrloch"
              control={control}
              defaultValue={currentBohrung?.durchmesserBohrloch}
              render={({ field }) => (
                <TextField
                  {...field}
                  value={field.value}
                  InputLabelProps={{ shrink: field.value != null }}
                  sx={{ width: "47%" }}
                  margin="normal"
                  label="Durchmesser Bohrloch [mm]"
                  type="number"
                  variant="standard"
                  {...register("durchmesserBohrloch")}
                />
              )}
            />
            <Controller
              name="ablenkungId"
              control={control}
              defaultValue={currentBohrung?.ablenkungId}
              render={({ field }) => (
                <Autocomplete
                  {...field}
                  options={ablenkungCodes.sort((a, b) => a.kurztext.localeCompare(b.kurztext)).map((c) => c.id)}
                  value={field.value}
                  onChange={(_, data) => field.onChange(data)}
                  getOptionLabel={(option) => ablenkungCodes.find((c) => c.id === option)?.kurztext}
                  autoHighlight
                  renderInput={(params) => (
                    <TextField
                      {...params}
                      margin="normal"
                      label="Ablenkung der Bohrung"
                      type="text"
                      sx={{ marginRight: "6%", width: "47%" }}
                      variant="standard"
                    />
                  )}
                />
              )}
            />
            <Controller
              name="qualitaetId"
              control={control}
              defaultValue={currentBohrung?.qualitaetId}
              render={({ field }) => (
                <Autocomplete
                  {...field}
                  options={qualitaetCodes.sort((a, b) => a.kurztext.localeCompare(b.kurztext)).map((c) => c.id)}
                  value={field.value}
                  getOptionLabel={(option) => qualitaetCodes.find((c) => c.id === option)?.kurztext}
                  onChange={(_, data) => field.onChange(data)}
                  autoHighlight
                  renderInput={(params) => (
                    <TextField
                      {...params}
                      sx={{ width: "47%" }}
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
              name="qualitaetBemerkung"
              control={control}
              defaultValue={currentBohrung?.qualitaetBemerkung}
              render={({ field }) => (
                <TextField
                  {...field}
                  value={field.value}
                  InputLabelProps={{ shrink: field.value != null }}
                  sx={{ marginRight: "6%", width: "47%" }}
                  margin="normal"
                  multiline
                  label="Bemerkung zur Qualitätsangabe"
                  type="text"
                  variant="standard"
                  {...register("qualitaetBemerkung")}
                />
              )}
            />
            <Controller
              name="quelleRef"
              control={control}
              defaultValue={currentBohrung?.quelleRef}
              render={({ field }) => (
                <TextField
                  {...field}
                  InputLabelProps={{ shrink: field.value != null }}
                  value={field.value}
                  margin="normal"
                  label="Autor der geologischen Aufnahme"
                  type="text"
                  sx={{ width: "47%" }}
                  variant="standard"
                  {...register("quelleRef")}
                />
              )}
            />
            {currentBohrung?.id && <DateUserInputs formObject={currentBohrung}></DateUserInputs>}
          </Box>
          <Box sx={{ width: { xs: "100%", md: "50%" }, paddingLeft: { xs: 0, md: 4 } }}>
            <Typography>
              {currentInteraction === "add"
                ? "Position der Bohrung durch Klicken in der Karte wählen"
                : "Position der Bohrung durch Klicken in der Karte ändern"}
            </Typography>
            <DetailMap
              bohrungen={[currentBohrung]}
              currentStandort={currentStandort}
              currentForm={"bohrung"}
              setCurrentBohrung={setCurrentBohrung}
              readOnly={readOnly}
            ></DetailMap>
            <Controller
              name="x_coordinate"
              control={control}
              render={({ field, fieldState: { error } }) => (
                <TextField
                  {...field}
                  sx={{ marginRight: "6%", width: "47%" }}
                  margin="normal"
                  InputLabelProps={{ shrink: field.value != null }}
                  value={field.value}
                  onChange={(value) => field.onChange(value)}
                  label="X-Koordinate der Bohrung"
                  type="number"
                  variant="standard"
                  {...register("x_coordinate", {
                    validate: {
                      range: (v) =>
                        validateXCoordinate(v) || "Die X-Koordinate muss zwischen 2590000 und 2646000 liegen",
                      distance: (v) =>
                        validateDistance(v, getValues("y_coordinate")) ||
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
                  InputLabelProps={{ shrink: field.value != null }}
                  value={field.value}
                  onChange={(value) => field.onChange(value)}
                  label="Y-Koordinate der Bohrung"
                  type="number"
                  variant="standard"
                  {...register("y_coordinate", {
                    validate: {
                      range: (v) =>
                        validateYCoordinate(v) || "Die Y-Koordinate muss zwischen 1212000 und 1264000 liegen",
                      distance: (v) =>
                        validateDistance(getValues("x_coordinate"), v) ||
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
              <Tooltip title="Bohrprofil hinzufügen">
                <span>
                  <IconButton
                    color="primary"
                    name="add-button"
                    disabled={readOnly || currentBohrung?.id == null}
                    onClick={onAddBohrprofil}
                  >
                    <AddCircleIcon />
                  </IconButton>
                </span>
              </Tooltip>
            </Typography>
            {currentBohrung?.id == null && (
              <Typography>Bitte speichern Sie die Bohrung bevor Sie Bohrprofile hinzufügen.</Typography>
            )}
            {currentBohrung?.bohrprofile?.length > 0 && (
              <React.Fragment>
                {currentBohrung?.bohrprofile?.length > 0 && (
                  <Table name="bohrprofile-table" size="small">
                    <TableHead>
                      <TableRow>
                        <TableCell>Datum</TableCell>
                        <TableCell>Endteufe [m]</TableCell>
                        <TableCell></TableCell>
                      </TableRow>
                    </TableHead>
                    <TableBody>
                      {currentBohrung.bohrprofile.map((bohrprofil) => (
                        <TableRow key={bohrprofil.id}>
                          <TableCell>
                            {bohrprofil?.datum != null ? new Date(bohrprofil.datum).toLocaleDateString() : null}
                          </TableCell>
                          <TableCell>{bohrprofil.endteufe}</TableCell>
                          <TableCell align="right">
                            <Tooltip title="Bohrprofil editieren">
                              <IconButton
                                onClick={() => onEditBohrprofil(bohrprofil)}
                                name="edit-button"
                                color="primary"
                              >
                                {readOnly ? <PreviewIcon /> : <EditIcon />}
                              </IconButton>
                            </Tooltip>
                            <Tooltip title="Bohrprofil duplizieren">
                              <IconButton
                                onClick={() => onCopyBohrprofil(bohrprofil)}
                                name="copy-button"
                                color="primary"
                                disabled={readOnly}
                              >
                                <ContentCopyIcon />
                              </IconButton>
                            </Tooltip>
                            <Tooltip title="Bohrprofil löschen">
                              <IconButton
                                onClick={() => onDeleteBohrprofil(bohrprofil)}
                                name="delete-button"
                                color="primary"
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
              </React.Fragment>
            )}
          </Box>
        </Stack>
      </DialogContent>
      <DialogActions>
        <Button onClick={handleBack}>{!isDirty || readOnly ? "Schliessen" : "Abbrechen"}</Button>
        <Button type="submit" disabled={!isDirty || readOnly}>
          Bohrung speichern
        </Button>
      </DialogActions>
      <ConfirmationDialog
        open={openConfirmation}
        confirm={confirmDeleteBohrprofil}
        entityName="Bohrprofil"
      ></ConfirmationDialog>
    </Box>
  );
}
