import * as React from "react";
import Table from "@mui/material/Table";
import TableBody from "@mui/material/TableBody";
import TableCell from "@mui/material/TableCell";
import TableHead from "@mui/material/TableHead";
import TableRow from "@mui/material/TableRow";
import Title from "./Title";

function createData(id, date, name) {
  return { id, date, name };
}

const rows = [
  createData(0, "16 Mar, 2019", "Elvis Presley"),
  createData(1, "16 Mar, 2019", "Paul McCartney"),
  createData(2, "16 Mar, 2019", "Tom Scholz"),
  createData(3, "16 Mar, 2019", "Michael Jackson"),
  createData(4, "15 Mar, 2019", "Bruce Springsteen"),
];

export default function Suchresultate() {
  return (
    <React.Fragment>
      <Title>Suchresultate</Title>
      <Table size="small">
        <TableHead>
          <TableRow>
            <TableCell>Datum</TableCell>
            <TableCell>Name</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {rows.map((row) => (
            <TableRow key={row.id}>
              <TableCell>{row.date}</TableCell>
              <TableCell>{row.name}</TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </React.Fragment>
  );
}
