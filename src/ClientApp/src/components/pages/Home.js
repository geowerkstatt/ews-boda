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
import { GemeindenMap } from "../../GemeindenMap";
import SnackbarMessage from "../SnackbarMessage";
import ConfirmationDialog from "../ConfirmationDialog";

export function Home() {
  const [standorte, setStandorte] = useState([]);
  const [gemeindenummer, setGemeindenummer] = useState(null);
  const [gbnummer, setGbnummer] = useState("");
  const [bezeichnung, setBezeichnung] = useState("");
  const [erstellungsDatum, setErstellungsDatum] = useState(null);
  const [mutationsDatum, setMutationsDatum] = useState(null);
  const [hasFilters, setHasFilters] = useState(false);
  const [openStandortForm, setOpenStandortForm] = useState(false);
  const [openConfirmation, setOpenConfirmation] = useState(false);
  const [currentStandort, setCurrentStandort] = useState(false);
  const [showSuccessAlert, setShowSuccessAlert] = useState(false);
  const [alertMessage, setAlertMessage] = useState(false);

  const resetSearch = () => {
    setGbnummer("");
    setBezeichnung("");
    setGemeindenummer(null);
    setErstellungsDatum(null);
    setMutationsDatum(null);
    setHasFilters(false);
  };

  const handleClose = () => {
    setOpenStandortForm(false);
  };

  const onAddStandort = () => {
    setCurrentStandort(null);
    setOpenStandortForm(true);
  };

  const onEditStandort = (standort) => {
    setCurrentStandort(standort);
    setOpenStandortForm(true);
  };

  const onDeletStandort = (standort) => {
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
  async function getStandorte() {
    let query = `?gemeindenummer=${gemeindenummer ?? ""}`;
    query += `&gbnummer=${gbnummer}&bezeichnung=${bezeichnung}`;
    query += `&erstellungsdatum=${erstellungsDatum ? new Date(erstellungsDatum).toUTCString() : ""}`;
    query += `&mutationsdatum=${mutationsDatum ? new Date(mutationsDatum).toUTCString() : ""}`;
    const response = await fetch("/standort" + query);
    if (response.ok) {
      const features = await response.json();
      setHasFilters(
        // at least one filter paramter is set
        gemeindenummer || gbnummer || bezeichnung || erstellungsDatum || mutationsDatum
      );
      setStandorte(features);
    }
  }

  // Get standort by Id
  async function getStandort(id) {
    const response = await fetch("/standort/" + id);
    if (response.ok) {
      const standort = await response.json();
      setCurrentStandort(standort);
      setStandorte(standorte.map((s) => (s.id === standort.id ? standort : s)));
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
      setShowSuccessAlert(true);
      setAlertMessage("Standort wurde hinzugefügt");
      resetSearch();
      setCurrentStandort(addedStandort);
    }
  }

  // Edit standort
  async function editStandort(data) {
    const updatedStandort = currentStandort;
    Object.entries(data).forEach(([key, value]) => {
      updatedStandort[key] = value;
    });
    updatedStandort.gemeinde = GemeindenMap[data?.gemeinde];
    // ignore bohrungen on update
    updatedStandort.bohrungen = null;
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
      getStandort(updatedStandort.id);
      setShowSuccessAlert(true);
      setAlertMessage("Standort wurde editiert.");
    }
  }

  // Delete standort
  async function deleteStandort(standort) {
    const response = await fetch("/standort?id=" + standort.id, {
      method: "DELETE",
    });
    if (response.ok) {
      const updatedStandorte = standorte.filter((s) => s.id !== currentStandort.id);
      setStandorte(updatedStandorte);
    }
  }

  // Get all Standorte from database
  useEffect(() => {
    let fetchurl = "/standort";
    fetch(fetchurl)
      .then((response) => response.json())
      .then((fetchedFeatures) => {
        setStandorte(fetchedFeatures);
      });
  }, []);

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
                setGbnummer={setGbnummer}
                setGemeindenummer={setGemeindenummer}
                setBezeichnung={setBezeichnung}
                setErstellungsDatum={setErstellungsDatum}
                setMutationsDatum={setMutationsDatum}
                erstellungsDatum={erstellungsDatum}
                mutationsDatum={mutationsDatum}
                hasFilters={hasFilters}
                resetSearch={resetSearch}
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
              <MainMap standorte={standorte} />
            </Paper>
          </Grid>
          <Grid item xs={12}>
            {hasFilters && standorte.length > 0 && (
              <Paper sx={{ p: 2, display: "flex", flexDirection: "column" }}>
                <SearchResults standorte={standorte} openEditForm={onEditStandort} onDeleteStandort={onDeletStandort} />
              </Paper>
            )}
            {hasFilters && standorte.length === 0 && (
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
            setCurrentStandort={setCurrentStandort}
            showSuccessAlert={showSuccessAlert}
            setShowSuccessAlert={setShowSuccessAlert}
            setAlertMessage={setAlertMessage}
            getStandort={getStandort}
          ></InputForm>
        </Dialog>
        <ConfirmationDialog
          open={openConfirmation}
          confirm={confirmDeleteStandort}
          entityName="Standort"
        ></ConfirmationDialog>
        <SnackbarMessage
          showSuccessAlert={showSuccessAlert}
          setShowSuccessAlert={setShowSuccessAlert}
          message={alertMessage}
          variant="success"
        ></SnackbarMessage>
      </Container>
    </Box>
  );
}
