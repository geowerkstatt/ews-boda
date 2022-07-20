import React, { useState } from "react";
import MobileStepper from "@mui/material/MobileStepper";
import Button from "@mui/material/Button";
import KeyboardArrowLeft from "@mui/icons-material/KeyboardArrowLeft";
import StandortForm from "./StandortForm";
import BohrungForm from "./BohrungForm";
import BohrprofilForm from "./BohrprofilForm";

export default function InputForm(props) {
  const {
    handleClose,
    currentStandort,
    editStandort,
    addStandort,
    setShowAlert,
    setAlertMessage,
    setAlertVariant,
    getStandort,
  } = props;

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
    bohrungToAdd.datum = new Date(Date.parse(bohrungToAdd.datum));
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
      setShowAlert(true);
      setAlertMessage("Bohrung wurde hinzugefügt.");
      setAlertVariant("success");
      getStandort(addedBohrung.standortId);
      handleBack();
    } else {
      setShowAlert(true);
      setAlertMessage(
        "Bohrung konnte nicht hinzugefügt werden. Überprüfen Sie, ob sich alle dem Standort zu geordneten Bohrungen in der gleichen Gemeinde im Kanton Solothurn befinden."
      );
      setAlertVariant("error");
      handleBack();
    }
  }

  // Edit Bohrung
  async function editBohrung(data) {
    const updatedBohrung = currentBohrung;
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
      setShowAlert(true);
      setAlertMessage("Bohrung wurde editiert.");
      setAlertVariant("success");
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
      setShowAlert(true);
      setAlertMessage("Bohrung wurde gelöscht.");
      setAlertVariant("success");
    }
  }

  const steps = [
    {
      label: "zum Standort",
      form: (
        <StandortForm
          handleNext={handleNext}
          currentStandort={currentStandort}
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
          currentStandort={currentStandort}
          setCurrentBohrung={setCurrentBohrung}
          currentBohrung={currentBohrung}
          handleNext={handleNext}
          handleBack={handleBack}
          addBohrung={addBohrung}
          editBohrung={editBohrung}
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
  );
}
