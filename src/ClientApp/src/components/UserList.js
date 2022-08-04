import React from "react";
import {
  IconButton,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  TableFooter,
  TablePagination,
  Tooltip,
} from "@mui/material";
import EditIcon from "@mui/icons-material/Edit";
import DeleteIcon from "@mui/icons-material/Delete";
import Title from "./Title";
import { UserRolesMap } from "../UserRolesMap";

export default function UserList(props) {
  const { users, openEditForm, onDelete } = props;
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
      <Title>Benutzerverwaltung</Title>
      <Table name="user-list-table" size="small">
        <TableHead>
          <TableRow>
            <TableCell>Name</TableCell>
            <TableCell>Rolle</TableCell>
            <TableCell></TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {users &&
            users.slice(page * rowsPerPage, page * rowsPerPage + rowsPerPage).map((user) => (
              <TableRow key={user.id}>
                <TableCell>{user.name}</TableCell>
                <TableCell>{Object.keys(UserRolesMap).find((r) => UserRolesMap[r] === user.role)}</TableCell>
                <TableCell align="right">
                  <Tooltip title="Benutzer editieren">
                    <IconButton onClick={() => openEditForm(user)} color="primary" aria-label="edit user">
                      <EditIcon />
                    </IconButton>
                  </Tooltip>
                  <Tooltip title="Benutzer löschen">
                    <IconButton onClick={() => onDelete(user)} color="primary" aria-label="delete user">
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
              count={users.length}
              onPageChange={handleChangePage}
              onRowsPerPageChange={handleChangeRowsPerPage}
              rowsPerPage={rowsPerPage}
              labelRowsPerPage="Benutzer pro Seite"
            />
          </TableRow>
        </TableFooter>
      </Table>
    </React.Fragment>
  );
}
