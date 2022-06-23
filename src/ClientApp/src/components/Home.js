import React, { useState, useEffect } from "react";
import Box from "@mui/material/Box";
import Toolbar from "@mui/material/Toolbar";
import Container from "@mui/material/Container";
import Grid from "@mui/material/Grid";
import Paper from "@mui/material/Paper";
import SearchResults from "./SearchResults";
import Search from "./Search";
import MainMap from "./MainMap";

export function Home() {
  const [standorte, setStandorte] = useState([]);
  const [gemeindenummer, setGemeindenummer] = useState(null);
  const [gbnummer, setGbnummer] = useState("");
  const [bezeichnung, setBezeichnung] = useState("");
  const [erstellungsDatum, setErstellungsDatum] = useState(null);
  const [mutationsDatum, setMutationsDatum] = useState(null);
  const [hasResults, setHasResults] = useState(false);

  const search = (event) => {
    event.preventDefault();
    let query = `?gemeindenummer=${gemeindenummer ?? ""}`;
    query += `&gbnummer=${gbnummer}&bezeichnung=${bezeichnung}`;
    query += `&erstellungsdatum=${erstellungsDatum ? new Date(erstellungsDatum).toUTCString() : ""}`;
    query += `&mutationsdatum=${mutationsDatum ? new Date(mutationsDatum).toUTCString() : ""}`;

    fetch("/standort" + query)
      .then((response) => response.json())
      .then((fetchedFeatures) => {
        setHasResults(
          fetchedFeatures.length > 0 &&
            // at least one filter paramter is set
            (gemeindenummer || gbnummer || bezeichnung || erstellungsDatum || mutationsDatum)
        );
        setStandorte(fetchedFeatures);
      });
  };

  // Get all Standorte from database
  useEffect(() => {
    let fetchurl = "/standort";
    fetch(fetchurl)
      .then((response) => response.json())
      .then((fetchedFeatures) => {
        setStandorte(fetchedFeatures);
        setStandorte(fetchedFeatures);
      });
  }, []);

  return (
    <Box
      component="main"
      sx={{
        backgroundColor: (theme) =>
          theme.palette.mode === "light" ? theme.palette.grey[100] : theme.palette.grey[900],
        flexGrow: 1,
        height: "100vh",
        overflow: "auto",
      }}
    >
      <Toolbar />
      <Container name="home-container" maxWidth="xl" sx={{ mt: 4, mb: 4 }}>
        <Grid container spacing={3}>
          <Grid item xs={12} sm={12} md={4} lg={3}>
            <Paper
              sx={{
                p: 2,
                display: "flex",
                flexDirection: "column",
              }}
            >
              <Search
                search={search}
                setGbnummer={setGbnummer}
                setGemeindenummer={setGemeindenummer}
                setBezeichnung={setBezeichnung}
                setErstellungsDatum={setErstellungsDatum}
                setMutationsDatum={setMutationsDatum}
                erstellungsDatum={erstellungsDatum}
                mutationsDatum={mutationsDatum}
              ></Search>
            </Paper>
          </Grid>
          <Grid item xs={12} sm={10} md={8} lg={9}>
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
            {hasResults && standorte.lenght !== 0 && (
              <Paper sx={{ p: 2, display: "flex", flexDirection: "column" }}>
                <SearchResults standorte={standorte} />
              </Paper>
            )}
            {standorte.lenght === 0 && (
              <Paper sx={{ p: 2, display: "flex", flexDirection: "column" }}>
                <div>Keine Resultate gefunden</div>
              </Paper>
            )}
          </Grid>
        </Grid>
      </Container>
    </Box>
  );
}
