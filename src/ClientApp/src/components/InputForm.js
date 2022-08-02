import React, { useState } from "react";
import MobileStepper from "@mui/material/MobileStepper";
import Button from "@mui/material/Button";
import KeyboardArrowLeft from "@mui/icons-material/KeyboardArrowLeft";
import StandortForm from "./StandortForm";
import BohrungForm from "./BohrungForm";
import BohrprofilForm from "./BohrprofilForm";
import SchichtForm from "./SchichtForm";

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
    currentUser,
  } = props;

  const [currentBohrung, setCurrentBohrung] = useState(null);
  const [currentBohrprofil, setCurrentBohrprofil] = useState(null);
  const [currentSchicht, setCurrentSchicht] = useState(null);

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
        "Bohrung konnte nicht hinzugefügt werden. Überprüfen Sie, ob sich alle dem Standort zugeordneten Bohrungen in der gleichen Gemeinde im Kanton Solothurn befinden."
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
    } else {
      setShowAlert(true);
      setAlertMessage(
        "Bohrung konnte nicht editiert werden. Überprüfen Sie, ob sich alle dem Standort zugeordneten Bohrungen in der gleichen Gemeinde im Kanton Solothurn befinden."
      );
      setAlertVariant("error");
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

  // Get bohrung by Id
  async function getBohrung(id) {
    const response = await fetch("/bohrung/" + id);
    if (response.ok) {
      const bohrung = await response.json();
      setCurrentBohrung(bohrung);
    }
  }

  // Add Bohrprofil
  async function addBohrprofil(data) {
    const bohrprofilToAdd = currentBohrprofil;
    Object.entries(data).forEach(([key, value]) => {
      bohrprofilToAdd[key] = value;
    });
    const response = await fetch("/bohrprofil", {
      method: "POST",
      cache: "no-cache",
      credentials: "same-origin",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(bohrprofilToAdd),
    });
    if (response.ok) {
      const addedBohrprofil = await response.json();
      setShowAlert(true);
      setAlertMessage("Bohrprofil wurde hinzugefügt.");
      setAlertVariant("success");
      getBohrung(addedBohrprofil.bohrungId);
      handleBack();
    }
  }

  // Edit Bohrprofil
  async function editBohrprofil(data) {
    const updatedBohrprofil = currentBohrprofil;
    Object.entries(data).forEach(([key, value]) => {
      updatedBohrprofil[key] = value;
    });
    const response = await fetch("/bohrprofil", {
      method: "PUT",
      cache: "no-cache",
      credentials: "same-origin",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(updatedBohrprofil),
    });
    if (response.ok) {
      setShowAlert(true);
      setAlertMessage("Bohrprofil wurde editiert.");
      setAlertVariant("success");
      getBohrung(updatedBohrprofil.bohrungId);
      handleBack();
    }
  }

  // Delete Bohrprofil
  async function deleteBohrprofil(bohrprofil) {
    const response = await fetch("/bohrprofil?id=" + bohrprofil.id, {
      method: "DELETE",
    });
    if (response.ok) {
      setShowAlert(true);
      setAlertMessage("Bohrprofil wurde gelöscht.");
      setAlertVariant("success");
      getBohrung(bohrprofil.bohrungId);
    }
  }

  // Get Bohrprofil by Id
  async function getBohrprofil(id) {
    const response = await fetch("/bohrprofil/" + id);
    if (response.ok) {
      const bohrprofil = await response.json();
      setCurrentBohrprofil(bohrprofil);
    }
  }

  // Add Schicht
  async function addSchicht(data) {
    const schichtToAdd = currentSchicht;
    Object.entries(data).forEach(([key, value]) => {
      schichtToAdd[key] = value;
    });
    const response = await fetch("/schicht", {
      method: "POST",
      cache: "no-cache",
      credentials: "same-origin",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(schichtToAdd),
    });
    if (response.ok) {
      const addedSchicht = await response.json();
      setShowAlert(true);
      setAlertMessage("Schicht wurde hinzugefügt.");
      setAlertVariant("success");
      getBohrprofil(addedSchicht.bohrprofilId);
      handleBack();
    }
  }

  // Edit Schicht
  async function editSchicht(data) {
    const updatedSchicht = currentSchicht;
    Object.entries(data).forEach(([key, value]) => {
      updatedSchicht[key] = value;
    });
    const response = await fetch("/schicht", {
      method: "PUT",
      cache: "no-cache",
      credentials: "same-origin",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(updatedSchicht),
    });
    if (response.ok) {
      setShowAlert(true);
      setAlertMessage("Schicht wurde editiert.");
      setAlertVariant("success");
      getBohrprofil(updatedSchicht.bohrprofilId);
      handleBack();
    }
  }

  // Delete Schicht
  async function deleteSchicht(schicht) {
    const response = await fetch("/schicht?id=" + schicht.id, {
      method: "DELETE",
    });
    if (response.ok) {
      setShowAlert(true);
      setAlertMessage("Schicht wurde gelöscht.");
      setAlertVariant("success");
      getBohrprofil(schicht.bohrprofilId);
    }
  }

  const steps = [
    {
      label: "zum Standort",
      form: (
        <StandortForm
          currentStandort={currentStandort}
          currentBohrung={currentBohrung}
          setCurrentBohrung={setCurrentBohrung}
          currentUser={currentUser}
          handleNext={handleNext}
          deleteBohrung={deleteBohrung}
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
          currentBohrung={currentBohrung}
          setCurrentBohrung={setCurrentBohrung}
          setCurrentBohrprofil={setCurrentBohrprofil}
          currentBohrprofil={currentBohrprofil}
          handleNext={handleNext}
          handleBack={handleBack}
          addBohrung={addBohrung}
          editBohrung={editBohrung}
          deleteBohrprofil={deleteBohrprofil}
        ></BohrungForm>
      ),
    },
    {
      label: "zum Bohrprofil",
      form: (
        <BohrprofilForm
          currentBohrung={currentBohrung}
          currentBohrprofil={currentBohrprofil}
          setCurrentBohrprofil={setCurrentBohrprofil}
          currentSchicht={currentSchicht}
          setCurrentSchicht={setCurrentSchicht}
          handleNext={handleNext}
          handleBack={handleBack}
          addBohrprofil={addBohrprofil}
          editBohrprofil={editBohrprofil}
          deleteSchicht={deleteSchicht}
        ></BohrprofilForm>
      ),
    },
    {
      label: "zur Schicht",
      form: (
        <SchichtForm
          currentBohrung={currentBohrung}
          currentBohrprofil={currentBohrprofil}
          currentSchicht={currentSchicht}
          setCurrentSchicht={setCurrentSchicht}
          handleBack={handleBack}
          addSchicht={addSchicht}
          editSchicht={editSchicht}
        ></SchichtForm>
      ),
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
