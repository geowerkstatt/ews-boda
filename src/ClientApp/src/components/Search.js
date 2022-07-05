import React, { useState } from "react";
import TextField from "@mui/material/TextField";
import { Autocomplete, Box, Button, Typography } from "@mui/material";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { GemeindenMap } from "../GemeindenMap";

export default function Search(props) {
  const {
    getStandorte,
    setGemeindenummer,
    setGbnummer,
    setBezeichnung,
    erstellungsDatum,
    setErstellungsDatum,
    mutationsDatum,
    setMutationsDatum,
    hasFilters,
    resetSearch,
  } = props;
  const [inputValue, setInputValue] = useState("");
  const [value, setValue] = useState(null);

  const search = (event) => {
    event.preventDefault();
    getStandorte();
  };

  const reset = () => {
    setInputValue("");
    setValue(null);
    resetSearch();
  };

  return (
    <Box
      component="form"
      onSubmit={search}
      sx={{
        "& .MuiTextField-root": { m: 1, width: "100%", pr: "15px" },
        minHeight: "500px",
        display: "flex",
        flexDirection: "column",
      }}
      autoComplete="off"
    >
      <Typography component="h1" variant="h6" color="inherit" noWrap>
        Nach Standort suchen
      </Typography>
      <Autocomplete
        name="gemeinde"
        value={value}
        onChange={(event, newGemeinde) => {
          setValue(newGemeinde);
          setGemeindenummer(Object.keys(GemeindenMap).find((key) => GemeindenMap[key] === newGemeinde));
        }}
        inputValue={inputValue}
        onInputChange={(event, newInputValue) => {
          setInputValue(newInputValue);
        }}
        options={Object.values(GemeindenMap)}
        renderInput={(params) => <TextField variant="standard" type="string" {...params} label="Gemeinde" />}
      ></Autocomplete>
      <TextField
        onChange={(event) => {
          setGbnummer(event.target.value);
        }}
        type="string"
        name="gbnummer"
        variant="standard"
        label="Grundbuchnummer"
        sx={{ minWidth: 100 }}
      />
      <TextField
        onChange={(event) => {
          setBezeichnung(event.target.value);
        }}
        type="text"
        name="bezeichnung"
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
      {hasFilters && (
        <Button sx={{ marginBottom: 1 }} variant="outlined" onClick={reset}>
          Suche zurücksetzen
        </Button>
      )}
      <Button variant="outlined" name="submit-button" type="submit">
        Suchen
      </Button>
    </Box>
  );
}
