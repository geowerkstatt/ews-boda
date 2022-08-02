import React, { useState, useEffect } from "react";
import Box from "@mui/material/Box";
import Toolbar from "@mui/material/Toolbar";
import Container from "@mui/material/Container";
import Grid from "@mui/material/Grid";
import Paper from "@mui/material/Paper";
import InputForm from "../InputForm";
import SearchResults from "../SearchResults";
import Search from "../Search";
import MainMap from "../MainMap";
import Dialog from "@mui/material/Dialog";
import Tooltip from "@mui/material/Tooltip";
import Button from "@mui/material/Button";
import AddIcon from "@mui/icons-material/Add";
import SnackbarMessage from "../SnackbarMessage";
import ConfirmationDialog from "../ConfirmationDialog";

export function Home(props) {
  const { currentUser } = props;
  const [standorte, setStandorte] = useState([]);
  const [showSearchResults, setShowSearchResults] = useState(false);
  const [openStandortForm, setOpenStandortForm] = useState(false);
  const [openConfirmation, setOpenConfirmation] = useState(false);
  const [currentStandort, setCurrentStandort] = useState(false);
  const [showAlert, setShowAlert] = useState(false);
  const [alertVariant, setAlertVariant] = useState("success");
  const [alertMessage, setAlertMessage] = useState("");
  // Cache standorte on client for better performance
  const [unfilteredStandorte, setUnfilteredStandorte] = useState([]);
  const [unfilteredBohrungenLength, setUnfilteredBohrungenLength] = useState([]);

  const handleClose = () => {
    setOpenStandortForm(false);
  };

  const onAddStandort = () => {
    setCurrentStandort(null);
    setOpenStandortForm(true);
  };

  const onEditStandort = (standort) => {
    getAndSetCurrentStandort(standort.id);
    setOpenStandortForm(true);
  };

  const onDeleteStandort = (standort) => {
    setOpenConfirmation(true);
    setCurrentStandort(standort);
  };

  const confirmDeleteStandort = (confirmation) => {
    if (confirmation) {
      deleteStandort(currentStandort);
    }
    setOpenConfirmation(false);
  };

  // Get all standorte
  async function getStandorte(query) {
    if (!query) {
      //Get cached standorte if no query is present
      setShowSearchResults(false);
      setStandorte(unfilteredStandorte);
    } else {
      const response = await fetch("/standort" + query);
      if (response.ok) {
        const features = await response.json();
        setStandorte(features);
        //Show search results only once response has returned.
        setShowSearchResults(true);
      }
    }
  }

  // Get standort by Id
  async function getAndSetCurrentStandort(id) {
    const response = await fetch("/standort/" + id);
    if (response.ok) {
      const standort = await response.json();
      setCurrentStandort(standort);
      setStandorte(standorte.map((s) => (s.id === standort.id ? standort : s)));
      setUnfilteredStandorte(unfilteredStandorte.map((s) => (s.id === standort.id ? standort : s)));
    }
  }

  // Add standort
  async function addStandort(data) {
    data.bohrungen = [];
    const response = await fetch("/standort", {
      method: "POST",
      cache: "no-cache",
      credentials: "same-origin",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data),
    });
    if (response.ok) {
      const addedStandort = await response.json();
      setShowAlert(true);
      setAlertMessage("Standort wurde hinzugefügt");
      setCurrentStandort(addedStandort);
      setUnfilteredStandorte([...unfilteredStandorte, addedStandort]);
    }
  }

  // Edit standort
  async function editStandort(data) {
    const updatedStandort = currentStandort;
    Object.entries(data).forEach(([key, value]) => {
      updatedStandort[key] = value;
    });
    const response = await fetch("/standort", {
      method: "PUT",
      cache: "no-cache",
      credentials: "same-origin",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(updatedStandort),
    });
    if (response.ok) {
      getAndSetCurrentStandort(updatedStandort.id);
      setShowAlert(true);
      setAlertMessage("Standort wurde editiert.");
    }
  }

  // Delete standort
  async function deleteStandort(standort) {
    const response = await fetch("/standort?id=" + standort.id, {
      method: "DELETE",
    });
    if (response.ok) {
      setStandorte(standorte.filter((s) => s.id !== currentStandort.id));
      setUnfilteredStandorte(unfilteredStandorte.filter((s) => s.id !== currentStandort.id));
    }
  }

  // Get all Standorte from database
  useEffect(() => {
    let fetchurl = "/standort";
    fetch(fetchurl)
      .then((response) => response.json())
      .then((fetchedFeatures) => {
        setStandorte(fetchedFeatures);
        setUnfilteredStandorte(fetchedFeatures);
      });
  }, []);

  // Get bohrungen count
  useEffect(() => {
    setUnfilteredBohrungenLength(unfilteredStandorte.flatMap((s) => s.bohrungen).length);
  }, [unfilteredStandorte]);

  return (
    <Box
      component="main"
      sx={{
        flexGrow: 1,
        height: "100vh",
        overflow: "auto",
      }}
    >
      <Toolbar />
      <Container name="home-container" maxWidth="xl" sx={{ mt: 4, mb: 4 }}>
        <Grid container spacing={3}>
          <Grid sx={{ display: "flex", flexDirection: "column" }} item xs={12} sm={12} md={4} lg={3}>
            <Tooltip title="Standort hinzufügen">
              <Button
                sx={{
                  mb: 2,
                  mt: 3,
                }}
                variant="contained"
                onClick={onAddStandort}
              >
                Neuen Standort erstellen
                <AddIcon
                  sx={{
                    ml: 1,
                  }}
                />
              </Button>
            </Tooltip>
            <Paper
              sx={{
                p: 2,
              }}
            >
              <Search
                getStandorte={getStandorte}
                showSearchResults={showSearchResults}
                gemeinden={[
                  ...new Set(
                    unfilteredStandorte
                      .map((s) => s.gemeinde)
                      .filter(Boolean)
                      .sort()
                  ),
                ]}
              ></Search>
            </Paper>
          </Grid>

          <Grid item xs={12} sm={12} md={8} lg={9}>
            <Paper
              sx={{
                p: 2,
                display: "flex",
                flexDirection: "column",
                padding: "0 0 0 0",
              }}
            >
              <MainMap standorte={standorte} unfilteredBohrungenLength={unfilteredBohrungenLength} />
            </Paper>
          </Grid>
          <Grid item xs={12}>
            {showSearchResults && standorte.length > 0 && (
              <Paper sx={{ p: 2, display: "flex", flexDirection: "column" }}>
                <SearchResults
                  standorte={standorte}
                  openEditForm={onEditStandort}
                  onDeleteStandort={onDeleteStandort}
                  currentUser={currentUser}
                />
              </Paper>
            )}
            {showSearchResults && standorte.length === 0 && (
              <Paper sx={{ p: 2, display: "flex", flexDirection: "column" }}>
                <div>Keine passenden Standorte gefunden.</div>
              </Paper>
            )}
          </Grid>
        </Grid>
        <Dialog open={openStandortForm} fullWidth={true} maxWidth="md">
          <InputForm
            handleClose={handleClose}
            editStandort={editStandort}
            addStandort={addStandort}
            currentStandort={currentStandort}
            showAlert={showAlert}
            setShowAlert={setShowAlert}
            setAlertMessage={setAlertMessage}
            getAndSetCurrentStandort={getAndSetCurrentStandort}
            setAlertVariant={setAlertVariant}
            currentUser={currentUser}
          ></InputForm>
        </Dialog>
        <ConfirmationDialog
          open={openConfirmation}
          confirm={confirmDeleteStandort}
          entityName="Standort"
        ></ConfirmationDialog>
        <SnackbarMessage
          showSuccessAlert={showAlert}
          setShowSuccessAlert={setShowAlert}
          message={alertMessage}
          variant={alertVariant}
        ></SnackbarMessage>
      </Container>
    </Box>
  );
}
