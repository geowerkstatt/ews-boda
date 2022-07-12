import React, { useState, useEffect } from "react";
import Box from "@mui/material/Box";
import Toolbar from "@mui/material/Toolbar";
import Container from "@mui/material/Container";
import Paper from "@mui/material/Paper";
import Dialog from "@mui/material/Dialog";
import UserList from "../UserList";
import UserInputForm from "../UserInputForm";
import ConfirmationDialog from "../ConfirmationDialog";

export function User() {
  const [users, setUsers] = useState([]);
  const [openForm, setOpenForm] = useState(false);
  const [openConfirmation, setOpenConfirmation] = useState(false);
  const [currentUser, setCurrentUser] = useState(false);

  const onDelete = (user) => {
    setOpenConfirmation(true);
    setCurrentUser(user);
  };

  const confirm = (confirmation) => {
    if (confirmation) {
      deleteUser(currentUser);
    }
    setOpenConfirmation(false);
  };

  const openEditForm = (user) => {
    setCurrentUser(user);
    setOpenForm(true);
  };

  // Edit user with the specified id.
  async function editUser(data) {
    currentUser.role = data.role;
    const response = await fetch("/user", {
      method: "PUT",
      cache: "no-cache",
      credentials: "same-origin",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(currentUser, (key, value) => {
        if (key === "role") {
          return parseInt(value);
        }
        return value;
      }),
    });
    if (response.ok) {
      const updatedUsers = users.map((s) => (s.id === currentUser.id ? currentUser : s));
      setUsers(updatedUsers);
    }
  }

  // Delete user with the specified id.
  async function deleteUser(user) {
    const response = await fetch("/user?id=" + user.id, {
      method: "DELETE",
    });
    if (response.ok) {
      const updatedUsers = users.filter((s) => s.id !== currentUser.id);
      setUsers(updatedUsers);
    }
  }

  // Get all users from database.
  useEffect(() => {
    let fetchurl = "/user";
    fetch(fetchurl)
      .then((response) => response.json())
      .then((users) => {
        setUsers(users);
      });
  }, []);

  return (
    <Box
      component="main"
      sx={{
        flexGrow: 1,
        height: "100vh",
        overflow: "auto",
      }}
    >
      <Toolbar />
      <Container name="user-container" maxWidth="md" sx={{ mt: 4, mb: 4 }}>
        <Paper sx={{ p: 2, display: "flex", flexDirection: "column" }}>
          <UserList users={users} openEditForm={openEditForm} onDelete={onDelete} />
        </Paper>
      </Container>
      <Dialog open={openForm} onClose={() => setOpenForm(false)} fullWidth={true} maxWidth="sm">
        <UserInputForm handleClose={() => setOpenForm(false)} editUser={editUser} user={currentUser} />
      </Dialog>
      <ConfirmationDialog open={openConfirmation} confirm={confirm} entityName="Benutzer"></ConfirmationDialog>
    </Box>
  );
}
