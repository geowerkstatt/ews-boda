import React from "react";
import Table from "@mui/material/Table";
import TableBody from "@mui/material/TableBody";
import TableCell from "@mui/material/TableCell";
import TableHead from "@mui/material/TableHead";
import TableFooter from "@mui/material/TableFooter";
import TableRow from "@mui/material/TableRow";
import TablePagination from "@mui/material/TablePagination";
import EditIcon from "@mui/icons-material/Edit";
import DeleteIcon from "@mui/icons-material/Delete";
import IconButton from "@mui/material/IconButton";
import Title from "./Title";
import Tooltip from "@mui/material/Tooltip";

export default function SearchResults(props) {
  const { standorte, openEditForm, onDeleteStandort } = props;
  const [page, setPage] = React.useState(0);
  const [rowsPerPage, setRowsPerPage] = React.useState(10);

  const handleChangePage = (event, newPage) => {
    setPage(newPage);
  };

  const handleChangeRowsPerPage = (event) => {
    setRowsPerPage(parseInt(event.target.value, 10));
    setPage(0);
  };
  return (
    <React.Fragment>
      <Title>Standorte</Title>
      <Table name="search-results-table" size="small">
        <TableHead>
          <TableRow>
            <TableCell>Gemeinde</TableCell>
            <TableCell>Grundbuchnummer</TableCell>
            <TableCell>Bezeichnung</TableCell>
            <TableCell>Anzahl Bohrungen</TableCell>
            <TableCell></TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {standorte &&
            standorte.slice(page * rowsPerPage, page * rowsPerPage + rowsPerPage).map((standort) => (
              <TableRow key={standort.id}>
                <TableCell>{standort.gemeinde}</TableCell>
                <TableCell>{standort.grundbuchNr}</TableCell>
                <TableCell>{standort.bezeichnung}</TableCell>
                <TableCell>{standort.bohrungen?.length}</TableCell>
                <TableCell align="right">
                  <Tooltip title="Standort editieren">
                    <IconButton
                      name="edit-button"
                      onClick={() => openEditForm(standort)}
                      color="primary"
                      aria-label="edit standort"
                    >
                      <EditIcon />
                    </IconButton>
                  </Tooltip>
                  <Tooltip title="Standort löschen">
                    <IconButton
                      name="delete-button"
                      onClick={() => onDeleteStandort(standort)}
                      color="primary"
                      aria-label="delete standort"
                    >
                      <DeleteIcon />
                    </IconButton>
                  </Tooltip>
                </TableCell>
              </TableRow>
            ))}
        </TableBody>
        <TableFooter>
          <TableRow>
            <TablePagination
              rowsPerPageOptions={[5, 10, 15, 20, 50]}
              page={page}
              count={standorte.length}
              onPageChange={handleChangePage}
              onRowsPerPageChange={handleChangeRowsPerPage}
              rowsPerPage={rowsPerPage}
              labelRowsPerPage="Standorte pro Seite"
            />
          </TableRow>
        </TableFooter>
      </Table>
    </React.Fragment>
  );
}
