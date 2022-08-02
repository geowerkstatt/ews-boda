import React from "react";
import { Controller, useForm } from "react-hook-form";
import TextField from "@mui/material/TextField";
import { Autocomplete, Box, Button, Typography } from "@mui/material";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";

export default function Search(props) {
  const { getStandorte, showSearchResults, gemeinden } = props;

  const { control, handleSubmit, reset } = useForm({ reValidateMode: "onChange" });

  const onSearch = (formData) => {
    let query = `?gemeinde=${formData.gemeinde || ""}`;
    query += `&gbnummer=${encodeURIComponent(formData.gbnummer)}`;
    query += `&bezeichnung=${encodeURIComponent(formData.bezeichnung)}`;
    query += `&erstellungsdatum=${formData.erstellungsDatum ? new Date(formData.erstellungsDatum).toDateString() : ""}`;
    query += `&mutationsdatum=${formData.mutationsDatum ? new Date(formData.mutationsDatum).toDateString() : ""}`;
    getStandorte(query);
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
          render={({ field }) => (
            <DatePicker
              label="Erstellungsdatum"
              disableFuture
              inputFormat="dd.MM.yyyy"
              value={field.value}
              onChange={(value) => field.onChange(value)}
              renderInput={(params) => <TextField {...field} margin="normal" variant="standard" {...params} />}
            />
          )}
        />
      </LocalizationProvider>
      <LocalizationProvider dateAdapter={AdapterDateFns}>
        <Controller
          name="mutationsDatum"
          control={control}
          defaultValue={null}
          render={({ field }) => (
            <DatePicker
              label="Mutationsdatum"
              disableFuture
              inputFormat="dd.MM.yyyy"
              value={field.value}
              onChange={(value) => field.onChange(value)}
              renderInput={(params) => <TextField {...field} margin="normal" variant="standard" {...params} />}
            />
          )}
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
