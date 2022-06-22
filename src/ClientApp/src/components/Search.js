import React from "react";
import TextField from "@mui/material/TextField";
import Autocomplete from "@mui/material/Autocomplete";
import { Box, Button } from "@mui/material";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import Typography from "@mui/material/Typography";
import { GemeindenMap } from "../GemeindenMap";

export default function Search(props) {
  const {
    search,
    setGemeindenummer,
    setGbnummer,
    setBezeichnung,
    erstellungsDatum,
    setErstellungsDatum,
    mutationsDatum,
    setMutationsDatum,
  } = props;

  return (
    <Box
      component="form"
      onSubmit={search}
      sx={{
        "& .MuiTextField-root": { m: 1, width: "100%", pr: "15px" },
        minHeight: "608px",
        display: "flex",
        flexDirection: "column",
      }}
      autoComplete="off"
    >
      <Typography component="h1" variant="h6" color="inherit" noWrap>
        Nach Standort filtern
      </Typography>
      <Autocomplete
        name="gemeinde"
        onChange={(event, newGemeinde) => {
          setGemeindenummer(Object.keys(GemeindenMap).find((key) => GemeindenMap[key] === newGemeinde));
        }}
        options={Object.values(GemeindenMap)}
        renderInput={(params) => <TextField variant="standard" type="string" {...params} label="Gemeinde" />}
      ></Autocomplete>
      <TextField
        onChange={(event) => {
          setGbnummer(event.target.value);
        }}
        type="number"
        variant="standard"
        label="GB-Nummer"
        sx={{ minWidth: 100 }}
      />
      <TextField
        onChange={(event) => {
          setBezeichnung(event.target.value);
        }}
        type="text"
        variant="standard"
        label="Bezeichnung"
      />
      <LocalizationProvider dateAdapter={AdapterDateFns}>
        <DatePicker
          label="Erstellungsdatum"
          value={erstellungsDatum}
          onChange={(newErstellungsDatum) => {
            setErstellungsDatum(newErstellungsDatum);
          }}
          renderInput={(params) => <TextField variant="standard" {...params} />}
        />
      </LocalizationProvider>
      <LocalizationProvider dateAdapter={AdapterDateFns}>
        <DatePicker
          label="Mutationsdatum"
          value={mutationsDatum}
          onChange={(newMutationsDatum) => {
            setMutationsDatum(newMutationsDatum);
          }}
          renderInput={(params) => <TextField variant="standard" {...params} />}
        />
      </LocalizationProvider>
      <Box sx={{ flexGrow: 1 }}></Box>
      <Button variant="outlined" type="submit">
        Filtern
      </Button>
    </Box>
  );
}
