import React, { useState, useEffect } from "react";
import DialogActions from "@mui/material/DialogActions";
import DialogContent from "@mui/material/DialogContent";
import DialogTitle from "@mui/material/DialogTitle";
import { Controller, useForm } from "react-hook-form";
import TextField from "@mui/material/TextField";
import IconButton from "@mui/material/IconButton";
import Tooltip from "@mui/material/Tooltip";
import Accordion from "@mui/material/Accordion";
import AccordionSummary from "@mui/material/AccordionSummary";
import AccordionDetails from "@mui/material/AccordionDetails";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import { Autocomplete, Box, Button } from "@mui/material";
import ArrowLeftIcon from "@mui/icons-material/ArrowLeft";
import ArrowRightIcon from "@mui/icons-material/ArrowRight";
import DetailMap from "./DetailMap";
import DateUserInputs from "./DateUserInputs";
import { CodeTypes } from "./Codetypes";

export default function SchichtForm(props) {
  const { currentBohrung, currentBohrprofil, currentSchicht, setCurrentSchicht, handleBack, addSchicht, editSchicht } =
    props;
  const { control, handleSubmit, formState, reset, register, setValue } = useForm({
    reValidateMode: "onChange",
  });
  const { isDirty } = formState;
  const [qualitaetCodes, setQualitaetCodes] = useState([]);
  const [codeSchichten, setCodeschichten] = useState([]);

  const [mapExpanded, setMapExpanded] = useState(true);

  const currentSchichtIndex = currentBohrprofil.schichten.indexOf(currentSchicht);
  const numberOfSchichten = currentBohrprofil.schichten.length;

  // Get codes for dropdowns
  useEffect(() => {
    const getCodes = async () => {
      const qualitaetResponse = await fetch("/code?codetypid=" + CodeTypes.Schicht_hquali);
      const codeSchichtResponse = await fetch("/codeschicht");
      const qualitaetCodes = await qualitaetResponse.json();
      const codeSchichten = await codeSchichtResponse.json();
      setQualitaetCodes(qualitaetCodes);
      setCodeschichten(codeSchichten);
    };
    getCodes();
  }, []);

  // Update form values if currentSchicht changes, to allow next/previous navigation
  useEffect(() => {
    if (currentSchicht) {
      setValue("tiefe", currentSchicht?.tiefe);
      setValue("codeSchichtId", currentSchicht?.codeSchichtId);
      setValue("qualitaetId", currentSchicht?.qualitaetId);
      setValue("qualitaetBemerkung", currentSchicht?.qualitaetBemerkung);
      setValue("bemerkung", currentSchicht?.bemerkung);
    }
  }, [currentSchicht, setValue]);

  const currentInteraction = currentSchicht?.id ? "edit" : currentSchicht?.tiefe ? "copy" : "add";

  const onSubmit = (formData) => {
    currentSchicht.id
      ? editSchicht(formData).finally(() => reset(formData))
      : addSchicht(formData).finally(() => reset(formData));
  };

  const onNavigateNext = () => setCurrentSchicht(currentBohrprofil.schichten[currentSchichtIndex + 1]);
  const onNavigatePrevious = () => setCurrentSchicht(currentBohrprofil.schichten[currentSchichtIndex - 1]);

  return (
    <Box component="form" name="schicht-form" onSubmit={handleSubmit(onSubmit)}>
      <DialogTitle>
        {currentInteraction === "edit"
          ? "Schicht bearbeiten"
          : currentInteraction === "copy"
          ? "Schicht kopieren"
          : "Schicht erstellen"}
        {currentSchicht?.id && currentSchichtIndex > 0 && (
          <Tooltip title="Zur vorherigen Schicht">
            <IconButton onClick={onNavigatePrevious} color="primary">
              <ArrowLeftIcon />
            </IconButton>
          </Tooltip>
        )}
        {currentSchicht?.id && currentSchichtIndex < numberOfSchichten - 1 && (
          <Tooltip title="Zur nächsten Schicht">
            <IconButton onClick={onNavigateNext} color="primary">
              <ArrowRightIcon />
            </IconButton>
          </Tooltip>
        )}
      </DialogTitle>
      <DialogContent>
        <Controller
          name="tiefe"
          control={control}
          defaultValue={currentSchicht?.tiefe}
          render={({ field, fieldState: { error } }) => (
            <TextField
              {...field}
              value={field.value}
              sx={{ width: "47%" }}
              margin="normal"
              label="Tiefe [m u. T.]"
              type="number"
              variant="standard"
              {...register("tiefe", { required: true })}
              error={error !== undefined}
              helperText={error ? "Geben Sie die Tiefe ein" : ""}
            />
          )}
        />
        <Controller
          name="codeSchichtId"
          control={control}
          defaultValue={currentSchicht?.codeSchichtId}
          rules={{ required: true }}
          render={({ field, fieldState: { error } }) => (
            <Autocomplete
              {...field}
              sx={{ width: "47%" }}
              options={codeSchichten.map((c) => c.id)}
              value={field.value}
              getOptionLabel={(option) => codeSchichten.find((c) => c.id === option)?.kurztext}
              onChange={(_, data) => field.onChange(data)}
              autoHighlight
              renderInput={(params) => (
                <TextField
                  {...params}
                  margin="normal"
                  label="Schicht (bzw. Schichtgrenze)"
                  type="text"
                  variant="standard"
                  error={error !== undefined}
                  helperText={error ? "Wählen Sie eine Schichtgrenze aus" : ""}
                />
              )}
            />
          )}
        />
        <Controller
          name="bemerkung"
          control={control}
          defaultValue={currentSchicht?.bemerkung}
          render={({ field }) => (
            <TextField
              {...field}
              InputLabelProps={{ shrink: field.value != null }}
              value={field.value}
              margin="normal"
              multiline
              label="Bemerkungen zur Schicht"
              type="text"
              fullWidth
              sx={{ marginRight: "6%" }}
              variant="standard"
              {...register("bemerkung")}
            />
          )}
        />
        <Controller
          name="qualitaetId"
          control={control}
          defaultValue={currentSchicht?.qualitaetId}
          rules={{ required: true }}
          render={({ field, fieldState: { error } }) => (
            <Autocomplete
              {...field}
              options={qualitaetCodes.map((c) => c.id)}
              value={field.value}
              getOptionLabel={(option) => qualitaetCodes.find((c) => c.id === option)?.kurztext}
              onChange={(_, data) => field.onChange(data)}
              sx={{ width: "47%" }}
              autoHighlight
              renderInput={(params) => (
                <TextField
                  {...params}
                  margin="normal"
                  label="Qualität"
                  type="text"
                  variant="standard"
                  error={error !== undefined}
                  helperText={error ? "Wählen Sie eine Qualitätsangabe aus" : ""}
                />
              )}
            />
          )}
        />
        <Controller
          name="qualitaetBemerkung"
          control={control}
          defaultValue={currentSchicht?.qualitaetBemerkung}
          render={({ field }) => (
            <TextField
              {...field}
              InputLabelProps={{ shrink: field.value != null }}
              value={field.value}
              margin="normal"
              multiline
              label="Bemerkungen zur Qualitätsangabe"
              type="text"
              fullWidth
              sx={{ width: "100%" }}
              variant="standard"
              {...register("qualitaetBemerkung")}
            />
          )}
        />
        {currentSchicht?.id && <DateUserInputs formObject={currentSchicht}></DateUserInputs>}
        <Accordion
          sx={{ boxShadow: "none" }}
          expanded={mapExpanded}
          onChange={(_, expanded) => setMapExpanded(expanded)}
        >
          <Tooltip title={mapExpanded ? "Übersichtskarte verbergen" : "Übersichtskarte anzeigen"}>
            <AccordionSummary expandIcon={<ExpandMoreIcon />}>Lokalität der Bohrung</AccordionSummary>
          </Tooltip>
          <AccordionDetails>
            <DetailMap bohrungen={[currentBohrung]} currentForm={"schicht"}></DetailMap>
          </AccordionDetails>
        </Accordion>
      </DialogContent>
      <DialogActions>
        <Button onClick={handleBack}>Abbrechen</Button>
        <Button type="submit" disabled={!isDirty}>
          Schicht speichern
        </Button>
      </DialogActions>
    </Box>
  );
}
