import React, { useState, useEffect } from "react";
import Box from "@mui/material/Box";
import Toolbar from "@mui/material/Toolbar";
import Container from "@mui/material/Container";
import Grid from "@mui/material/Grid";
import Paper from "@mui/material/Paper";
import Suchresultate from "./Suchresultate";
import Search from "./Search";
import Karte from "./Karte";

export function Home() {
  const [bohrungen, setBohrungen] = useState("");
  const [standorte, setStandorte] = useState([]);
  const [gemeindenummer, setGemeindenummer] = useState("");
  const [gbnummer, setGbnummer] = useState("");
  const [bezeichnung, setBezeichnung] = useState("");
  const [erstellungsDatum, setErstellungsDatum] = useState(null);
  const [mutationsDatum, setMutationsDatum] = useState(null);
  const [hasResults, setHasResults] = useState(false);

  const search = (event) => {
    event.preventDefault();
    let query = `?gemeindenummer=${gemeindenummer ?? ""}`;
    query += `&gbnummer=${gbnummer}&bezeichnung=${bezeichnung}`;
    query += `&erstellungsdatum=${erstellungsDatum ? new Date(erstellungsDatum).toLocaleDateString("de-CH") : ""}`;
    query += `&mutationsdatum=${mutationsDatum ? new Date(mutationsDatum).toLocaleDateString("de-CH") : ""}`;

    fetch("/bohrung" + query)
      .then((response) => response.json())
      .then((fetchedFeatures) => {
        setBohrungen(fetchedFeatures);
      });

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

  // Get Bohrungen from database
  useEffect(() => {
    let fetchurl = "/bohrung";
    fetch(fetchurl)
      .then((response) => response.json())
      .then((fetchedFeatures) => {
        setBohrungen(fetchedFeatures);
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
              <Karte bohrungen={bohrungen} />
            </Paper>
          </Grid>
          <Grid item xs={12}>
            {hasResults && (
              <Paper sx={{ p: 2, display: "flex", flexDirection: "column" }}>
                <Suchresultate standorte={standorte} />
              </Paper>
            )}
            {standorte.lenght === 0 && (
              <Paper sx={{ p: 2, display: "flex", flexDirection: "column" }}>
                <div>Keine Results</div>
              </Paper>
            )}
          </Grid>
        </Grid>
      </Container>
    </Box>
  );
}
