import React from "react";
import { Controller, useForm } from "react-hook-form";
import { GemeindenMap } from "../GemeindenMap";
import TextField from "@mui/material/TextField";
import Table from "@mui/material/Table";
import IconButton from "@mui/material/IconButton";
import TableBody from "@mui/material/TableBody";
import TableCell from "@mui/material/TableCell";
import TableHead from "@mui/material/TableHead";
import TableRow from "@mui/material/TableRow";
import EditIcon from "@mui/icons-material/Edit";
import DialogActions from "@mui/material/DialogActions";
import DialogContent from "@mui/material/DialogContent";
import DialogTitle from "@mui/material/DialogTitle";
import Typography from "@mui/material/Typography";
import AddCircleIcon from "@mui/icons-material/AddCircle";
import Tooltip from "@mui/material/Tooltip";
import Box from "@mui/material/Box";
import Accordion from "@mui/material/Accordion";
import AccordionSummary from "@mui/material/AccordionSummary";
import AccordionDetails from "@mui/material/AccordionDetails";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import Button from "@mui/material/Button";
import DetailMap from "./DetailMap";

export default function StandortInputForm(props) {
  const { control, handleSubmit } = useForm({ reValidateMode: "onBlur" });
  const { handleClose, standort, editStandort, addStandort } = props;

  const onSubmit = (formData) => {
    handleClose();
    standort ? editStandort(formData) : addStandort(formData);
  };

  return (
    <Box component="form" onSubmit={handleSubmit(onSubmit)}>
      <DialogTitle>{standort ? "Standort bearbeiten" : "Standort erstellen"}</DialogTitle>
      <DialogContent>
        <Controller
          name="bezeichnung"
          control={control}
          rules={{
            required: true,
          }}
          defaultValue={standort?.bezeichnung}
          render={({ field, fieldState: { error } }) => (
            <TextField
              {...field}
              autoFocus
              margin="normal"
              label="Bezeichnung des Standorts"
              type="text"
              fullWidth
              variant="standard"
              error={error !== undefined}
              helperText={error ? "Geben Sie eine Bezeichnung ein" : ""}
            />
          )}
        />
        <Controller
          name="bemerkung"
          control={control}
          defaultValue={standort?.bemerkung}
          render={({ field }) => (
            <TextField
              {...field}
              margin="normal"
              label="Bemerkung zum Standort"
              type="text"
              fullWidth
              variant="standard"
            />
          )}
        />
        {standort && (
          <Controller
            control={control}
            name="gemeinde"
            defaultValue={GemeindenMap[standort.gemeinde]}
            render={({ field }) => (
              <TextField
                {...field}
                fullWidth
                type="string"
                InputProps={{
                  readOnly: true,
                }}
                variant="standard"
                label="Gemeinde"
              />
            )}
          />
        )}
        <Controller
          name="grundbuchNr"
          control={control}
          defaultValue={standort?.grundbuchNr}
          render={({ field }) => (
            <TextField {...field} margin="normal" label="Grundbuchnummer" type="text" fullWidth variant="standard" />
          )}
        />
        {standort && (
          <React.Fragment>
            <Controller
              name="userErstellung"
              control={control}
              defaultValue={standort.userErstellung}
              render={({ field }) => (
                <TextField
                  {...field}
                  sx={{ marginRight: "6%", width: "47%" }}
                  InputProps={{
                    readOnly: true,
                  }}
                  margin="normal"
                  label="Erstellt durch"
                  type="text"
                  variant="standard"
                />
              )}
            />
            <TextField
              defaultValue={new Date(standort.erstellungsdatum).toLocaleDateString()}
              InputProps={{
                readOnly: true,
              }}
              sx={{ width: "47%" }}
              margin="normal"
              label="Erstellt am"
              type="text"
              variant="standard"
            />
            <Controller
              name="userMutation"
              control={control}
              defaultValue={standort.userMutation}
              render={({ field }) => (
                <TextField
                  {...field}
                  sx={{ marginRight: "6%", width: "47%" }}
                  InputProps={{
                    readOnly: true,
                  }}
                  margin="normal"
                  label="Zuletzt geändert durch"
                  type="text"
                  variant="standard"
                />
              )}
            />
            <TextField
              name="mutationsdatum"
              defaultValue={standort.mutationsdatum ? new Date(standort.mutationsdatum).toLocaleDateString() : null}
              InputProps={{
                readOnly: true,
              }}
              sx={{ width: "47%" }}
              margin="normal"
              label="Zuletzt geändert am"
              type="text"
              variant="standard"
            />
            <Controller
              name="afuUser"
              control={control}
              defaultValue={standort?.afuUser}
              render={({ field }) => (
                <TextField
                  {...field}
                  sx={{ marginRight: "6%", width: "47%" }}
                  InputProps={{
                    readOnly: true,
                  }}
                  margin="normal"
                  label="AfU Freigabe erfolgt durch"
                  type="text"
                  variant="standard"
                />
              )}
            />
            <TextField
              name="afuDatum"
              defaultValue={standort?.afuDatum ? new Date(standort?.afuDatum).toLocaleDateString() : null}
              InputProps={{
                readOnly: true,
              }}
              sx={{ width: "47%" }}
              margin="normal"
              label="AfU Freigabe erfolgt am"
              type="text"
              variant="standard"
            />
          </React.Fragment>
        )}
        <Typography sx={{ marginTop: "15px" }} variant="h6" gutterBottom>
          Bohrungen ({standort?.bohrungen ? standort.bohrungen.length : 0})
          <Tooltip title="Bohrung hinzufügen">
            <IconButton color="primary">
              <AddCircleIcon />
            </IconButton>
          </Tooltip>
        </Typography>
        {standort?.bohrungen?.length > 0 && (
          <React.Fragment>
            <Accordion sx={{ boxShadow: "none" }} defaultExpanded={true}>
              <Tooltip title="Übersichtskarte anzeigen">
                <AccordionSummary expandIcon={<ExpandMoreIcon />}></AccordionSummary>
              </Tooltip>
              {standort?.bohrungen?.length > 0 && (
                <AccordionDetails>
                  <DetailMap standorte={[standort]}></DetailMap>
                </AccordionDetails>
              )}
            </Accordion>
            {standort?.bohrungen?.length > 0 && (
              <Table name="search-results-table" size="small">
                <TableHead>
                  <TableRow>
                    <TableCell>Bezeichnung</TableCell>
                    <TableCell>Datum</TableCell>
                    <TableCell></TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {standort.bohrungen.map((bohrung) => (
                    <TableRow key={bohrung.id}>
                      <TableCell>{bohrung.bezeichnung}</TableCell>
                      <TableCell>{new Date(bohrung.datum).toLocaleDateString()}</TableCell>
                      <TableCell>
                        <Tooltip title="Bohrung editieren">
                          <IconButton>
                            <EditIcon />
                          </IconButton>
                        </Tooltip>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            )}
          </React.Fragment>
        )}
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose}>Abbrechen</Button>
        <Button type="submit">Speichern</Button>
        <Button disabled>Standort freigeben</Button>
      </DialogActions>
    </Box>
  );
}
