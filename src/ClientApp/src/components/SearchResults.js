import * as React from "react";
import Table from "@mui/material/Table";
import TableBody from "@mui/material/TableBody";
import TableCell from "@mui/material/TableCell";
import TableHead from "@mui/material/TableHead";
import TableRow from "@mui/material/TableRow";
import Title from "./Title";
import { GemeindenMap } from "../GemeindenMap";

export default function SearchResults(props) {
  const { standorte } = props;
  return (
    <React.Fragment>
      <Title>Suchresultate</Title>
      <Table name="seach-results-table" size="small">
        <TableHead>
          <TableRow>
            <TableCell>Gemeinde</TableCell>
            <TableCell>Grundbuchnummer</TableCell>
            <TableCell>Bezeichnung</TableCell>
            <TableCell>Anzahl Bohrungen</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {standorte &&
            standorte.map((standort) => (
              <TableRow key={standort.id}>
                <TableCell>{GemeindenMap[standort.gemeinde]}</TableCell>
                <TableCell>{standort.grundbuchNr}</TableCell>
                <TableCell>{standort.bezeichnung}</TableCell>
                <TableCell>0</TableCell>
              </TableRow>
            ))}
        </TableBody>
      </Table>
    </React.Fragment>
  );
}
