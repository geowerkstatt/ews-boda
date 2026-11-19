import React from "react";
import { Controller, useForm } from "react-hook-form";
import { Autocomplete, Box, Button, TextField, Typography } from "@mui/material";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";

const renderSearchDatePicker =
  (label) =>
  ({ field }) => (
    <DatePicker
      label={label}
      name={field.name}
      inputRef={field.ref}
      disableFuture
      format="dd.MM.yyyy"
      value={field.value}
      onChange={field.onChange}
      slotProps={{ textField: { margin: "normal", variant: "standard" } }}
    />
  );

export default function Search(props) {
  const { getStandorte, showSearchResults, gemeinden, setSearchResultsPage } = props;

  const { control, handleSubmit, reset } = useForm({ reValidateMode: "onChange" });

  const onSearch = (formData) => {
    let query = `?gemeinde=${formData.gemeinde || ""}`;
    query += `&gbnummer=${encodeURIComponent(formData.gbnummer)}`;
    query += `&bezeichnung=${encodeURIComponent(formData.bezeichnung)}`;
    query += `&erstellungsdatum=${formData.erstellungsDatum ? new Date(formData.erstellungsDatum).toDateString() : ""}`;
    query += `&mutationsdatum=${formData.mutationsDatum ? new Date(formData.mutationsDatum).toDateString() : ""}`;
    getStandorte(query);
    setSearchResultsPage(0);
  };

  const resetSearch = () => {
    reset({
      gemeinde: null,
      gbnummer: "",
      bezeichnung: "",
      erstellungsDatum: null,
      mutationsDatum: null,
    });
    getStandorte("");
  };

  return (
    <Box
      component="form"
      onSubmit={handleSubmit(onSearch)}
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
      <Controller
        name="gemeinde"
        control={control}
        defaultValue={null}
        render={({ field }) => (
          <Autocomplete
            {...field}
            options={gemeinden}
            value={field.value}
            onChange={(_, data) => field.onChange(data)}
            autoHighlight
            renderInput={(params) => <TextField {...params} label="Gemeinde" type="text" variant="standard" />}
          />
        )}
      />
      <Controller
        name="gbnummer"
        control={control}
        defaultValue={""}
        render={({ field }) => (
          <TextField
            {...field}
            margin="normal"
            sx={{ minWidth: 100 }}
            label="Grundbuchnummer"
            type="text"
            fullWidth
            variant="standard"
          />
        )}
      />
      <Controller
        name="bezeichnung"
        control={control}
        defaultValue={""}
        render={({ field }) => (
          <TextField {...field} margin="normal" label="Bezeichnung" type="text" fullWidth variant="standard" />
        )}
      />
      <LocalizationProvider dateAdapter={AdapterDateFns}>
        <Controller
          name="erstellungsDatum"
          control={control}
          defaultValue={null}
          render={renderSearchDatePicker("Erstellungsdatum")}
        />
        <Controller
          name="mutationsDatum"
          control={control}
          defaultValue={null}
          render={renderSearchDatePicker("Mutationsdatum")}
        />
      </LocalizationProvider>
      <Box sx={{ flexGrow: 1 }}></Box>
      {showSearchResults && (
        <Button sx={{ marginBottom: 1 }} variant="outlined" onClick={resetSearch}>
          Suche zurücksetzen
        </Button>
      )}
      <Button variant="outlined" name="submit-button" type="submit">
        Suchen
      </Button>
    </Box>
  );
}
