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
    setCurrentStandort,
    editStandort,
    addStandort,
    showSuccessAlert,
    setShowSuccessAlert,
    setAlertMessage,
    refreshStandort,
    setConfirm,
    setOpenConfirmation,
  } = props;

  const [bohrung, setBohrung] = useState(null);

  const handleNext = () => {
    setActiveStep((prevActiveStep) => prevActiveStep + 1);
  };

  const handleBack = () => {
    setActiveStep((prevActiveStep) => prevActiveStep - 1);
  };
  const steps = [
    {
      label: "zum Standort",
      form: (
        <StandortForm
          handleNext={handleNext}
          currentStandort={currentStandort}
          setCurrentStandort={setCurrentStandort}
          setBohrung={setBohrung}
          handleClose={handleClose}
          editStandort={editStandort}
          addStandort={addStandort}
          setShowSuccessAlert={setShowSuccessAlert}
          setAlertMessage={setAlertMessage}
          refreshStandort={refreshStandort}
          setConfirm={setConfirm}
          setOpenConfirmation={setOpenConfirmation}
        ></StandortForm>
      ),
    },
    {
      label: "zur Bohrung",
      form: (
        <BohrungForm
          handleNext={handleNext}
          handleBack={handleBack}
          bohrung={bohrung}
          showSuccessAlert={showSuccessAlert}
          setShowSuccessAlert={setShowSuccessAlert}
          setAlertMessage={setAlertMessage}
          refreshStandort={refreshStandort}
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
