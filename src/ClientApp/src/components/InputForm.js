import React, { useState } from "react";
import MobileStepper from "@mui/material/MobileStepper";
import Button from "@mui/material/Button";
import KeyboardArrowLeft from "@mui/icons-material/KeyboardArrowLeft";
import StandortForm from "./StandortForm";
import BohrungForm from "./BohrungForm";
import BohrprofilForm from "./BohrprofilForm";

<<<<<<< HEAD:src/ClientApp/src/components/StandortInputForm.js
export default function StandortInputForm(props) {
  const { control, handleSubmit } = useForm({ reValidateMode: "onBlur" });
  const { handleClose, standort, editStandort, addStandort } = props;
=======
export default function InputForm(props) {
  const {
    handleClose,
    currentStandort,
    setCurrentStandort,
    editStandort,
    addStandort,
    setShowSuccessAlert,
    setAlertMessage,
    getStandort,
  } = props;
>>>>>>> eece0ff (Add bohrung form):src/ClientApp/src/components/InputForm.js

  const [currentBohrung, setCurrentBohrung] = useState(null);

  const handleNext = () => {
    setActiveStep((prevActiveStep) => prevActiveStep + 1);
  };

  const handleBack = () => {
    setActiveStep((prevActiveStep) => prevActiveStep - 1);
  };

  // Add Bohrung
  async function addBohrung(data) {
    const bohrungToAdd = currentBohrung;
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
      getStandort(addedBohrung.standortId);
      handleBack();
    }
  }

  // Edit Bohrung
  async function editBohrung(data) {
    const updatedBohrung = currentBohrung;
    // ignore bohrprofile on update
    updatedBohrung.bohrprofile = null;
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
      getStandort(updatedBohrung.standortId);
      handleBack();
    }
  }

  // Delete Bohrung
  async function deleteBohrung(bohrung) {
    const response = await fetch("/bohrung?id=" + bohrung.id, {
      method: "DELETE",
    });
    if (response.ok) {
      getStandort(bohrung.standortId);
      setShowSuccessAlert(true);
      setAlertMessage("Bohrung wurde gelöscht.");
    }
  }

  const steps = [
    {
      label: "zum Standort",
      form: (
        <StandortForm
          handleNext={handleNext}
          currentStandort={currentStandort}
          setCurrentStandort={setCurrentStandort}
          setCurrentBohrung={setCurrentBohrung}
          deleteBohrung={deleteBohrung}
          currentBohrung={currentBohrung}
          handleClose={handleClose}
          editStandort={editStandort}
          addStandort={addStandort}
        ></StandortForm>
      ),
    },
    {
      label: "zur Bohrung",
      form: (
        <BohrungForm
          currentBohrung={currentBohrung}
          handleNext={handleNext}
          handleBack={handleBack}
          addBohrung={addBohrung}
          ediotBohrung={editBohrung}
        ></BohrungForm>
      ),
    },
    {
      label: "zum Bohrprofil",
      form: <BohrprofilForm handleBack={handleBack}></BohrprofilForm>,
    },
  ];

  const [activeStep, setActiveStep] = React.useState(0);
  const maxSteps = steps.length;

  return (
<<<<<<< HEAD:src/ClientApp/src/components/StandortInputForm.js
    <Box component="form" onSubmit={handleSubmit(onSubmit)}>
      <DialogTitle>{standort ? "Standort bearbeiten" : "Standort erstellen"}</DialogTitle>
      <DialogContent>
        <Controller
          name="bezeichnung"
          control={control}
          rules={{
            required: true,
          }}
          defaultValue={standort?.bezeichnung}
          render={({ field, fieldState: { error } }) => (
            <TextField
              {...field}
              autoFocus
              margin="normal"
              label="Bezeichnung des Standorts"
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
          defaultValue={standort?.bemerkung}
          render={({ field }) => (
            <TextField
              {...field}
              margin="normal"
              label="Bemerkung zum Standort"
              type="text"
              fullWidth
              variant="standard"
            />
          )}
        />
        {standort && (
          <Controller
            control={control}
            name="gemeinde"
            defaultValue={GemeindenMap[standort?.gemeinde]}
            render={({ field }) => (
              <TextField
                {...field}
                fullWidth
                type="string"
                InputProps={{
                  readOnly: true,
                }}
                variant="standard"
                label="Gemeinde"
              />
            )}
          />
        )}
        <Controller
          name="grundbuchNr"
          control={control}
          defaultValue={standort?.grundbuchNr}
          render={({ field }) => (
            <TextField {...field} margin="normal" label="Grundbuchnummer" type="text" fullWidth variant="standard" />
          )}
        />
        {standort && (
          <React.Fragment>
            <Controller
              name="userErstellung"
              control={control}
              defaultValue={standort?.userErstellung}
              render={({ field }) => (
                <TextField
                  {...field}
                  sx={{ marginRight: "6%", width: "47%" }}
                  InputProps={{
                    readOnly: true,
                  }}
                  margin="normal"
                  label="Erstellt durch"
                  type="text"
                  variant="standard"
                />
              )}
            />
            <TextField
              defaultValue={new Date(standort?.erstellungsdatum).toLocaleDateString()}
              InputProps={{
                readOnly: true,
              }}
              sx={{ width: "47%" }}
              margin="normal"
              label="Erstellt am"
              type="text"
              variant="standard"
            />
            <Controller
              name="userMutation"
              control={control}
              defaultValue={standort?.userMutation}
              render={({ field }) => (
                <TextField
                  {...field}
                  sx={{ marginRight: "6%", width: "47%" }}
                  InputProps={{
                    readOnly: true,
                  }}
                  margin="normal"
                  label="Zuletzt geändert durch"
                  type="text"
                  variant="standard"
                />
              )}
            />
            <TextField
              name="mutationsdatum"
              defaultValue={standort?.mutationsdatum ? new Date(standort?.mutationsdatum).toLocaleDateString() : null}
              InputProps={{
                readOnly: true,
              }}
              sx={{ width: "47%" }}
              margin="normal"
              label="Zuletzt geändert am"
              type="text"
              variant="standard"
            />
            <Controller
              name="afuUser"
              control={control}
              defaultValue={standort?.afuUser}
              render={({ field }) => (
                <TextField
                  {...field}
                  sx={{ marginRight: "6%", width: "47%" }}
                  InputProps={{
                    readOnly: true,
                  }}
                  margin="normal"
                  label="AfU Freigabe erfolgt durch"
                  type="text"
                  variant="standard"
                />
              )}
            />
            <TextField
              name="afuDatum"
              defaultValue={standort?.afuDatum ? new Date(standort?.afuDatum).toLocaleDateString() : null}
              InputProps={{
                readOnly: true,
              }}
              sx={{ width: "47%" }}
              margin="normal"
              label="AfU Freigabe erfolgt am"
              type="text"
              variant="standard"
            />
          </React.Fragment>
        )}
        <Typography sx={{ marginTop: "15px" }} variant="h6" gutterBottom>
          Bohrungen ({standort?.bohrungen ? standort.bohrungen.length : 0})
          <Tooltip title="Bohrung hinzufügen">
            <IconButton color="primary">
              <AddCircleIcon />
            </IconButton>
          </Tooltip>
        </Typography>
        {standort?.bohrungen?.length > 0 && (
          <React.Fragment>
            <Accordion sx={{ boxShadow: "none" }} defaultExpanded={true}>
              <Tooltip title="Übersichtskarte anzeigen">
                <AccordionSummary expandIcon={<ExpandMoreIcon />}></AccordionSummary>
              </Tooltip>
              {standort?.bohrungen?.length > 0 && (
                <AccordionDetails>
                  <DetailMap standorte={[standort]}></DetailMap>
                </AccordionDetails>
              )}
            </Accordion>
            {standort?.bohrungen?.length > 0 && (
              <Table name="search-results-table" size="small">
                <TableHead>
                  <TableRow>
                    <TableCell>Bezeichnung</TableCell>
                    <TableCell>Datum</TableCell>
                    <TableCell></TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {standort.bohrungen.map((bohrung) => (
                    <TableRow key={bohrung.id}>
                      <TableCell>{bohrung.bezeichnung}</TableCell>
                      <TableCell>{new Date(bohrung.datum).toLocaleDateString()}</TableCell>
                      <TableCell>
                        <Tooltip title="Bohrung editieren">
                          <IconButton>
                            <EditIcon />
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
        <Button onClick={handleClose}>Abbrechen</Button>
        <Button type="submit">Speichern</Button>
        <Button disabled>Standort freigeben</Button>
      </DialogActions>
    </Box>
=======
    <React.Fragment>
      <MobileStepper
        variant="dots"
        steps={maxSteps}
        position="static"
        activeStep={activeStep}
        backButton={
          <Button
            size="small"
            onClick={handleBack}
            disabled={activeStep === 0}
            sx={{
              "&.Mui-disabled": {
                color: "transparent",
              },
            }}
          >
            <KeyboardArrowLeft />
            Zurück {activeStep !== 0 && steps[activeStep - 1].label}
          </Button>
        }
      />
      <div>{steps[activeStep].form}</div>
    </React.Fragment>
>>>>>>> eece0ff (Add bohrung form):src/ClientApp/src/components/InputForm.js
  );
}
