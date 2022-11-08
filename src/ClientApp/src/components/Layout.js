import React, { useState } from "react";
import { useHistory } from "react-router-dom";
import { createTheme, ThemeProvider } from "@mui/material/styles";
import CssBaseline from "@mui/material/CssBaseline";
import {
  Box,
  Divider,
  IconButton,
  List,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Toolbar,
  Tooltip,
  Typography,
} from "@mui/material";
import MenuIcon from "@mui/icons-material/Menu";
import ChevronLeftIcon from "@mui/icons-material/ChevronLeft";
import LogoutIcon from "@mui/icons-material/Logout";
import HomeIcon from "@mui/icons-material/Home";
import GroupIcon from "@mui/icons-material/Group";
import InfoIcon from "@mui/icons-material/Info";
import FileDownloadIcon from "@mui/icons-material/FileDownload";
import { Footer } from "./Footer";
import { AppBar } from "./AppBar";
import { Drawer } from "./Drawer";
import { UserRolesMap } from "../UserRolesMap";

export const drawerWidth = 240;

const mdTheme = createTheme();

export function Layout(props) {
  const { currentUser } = props;
  const [open, setOpen] = useState(false);
  const toggleDrawer = () => {
    setOpen(!open);
  };

  async function handleLogout() {
    const response = await fetch("/ewsboda");
    if (response.ok) {
      history.push("/");
      window.location.reload();
    }
  }

  const history = useHistory();

  return (
    <ThemeProvider theme={mdTheme}>
      <Box sx={{ display: "flex" }}>
        <CssBaseline />
        <AppBar position="absolute" open={open}>
          <Toolbar
            sx={{
              pr: "24px",
            }}
          >
            <IconButton
              edge="start"
              color="inherit"
              aria-label="open drawer"
              onClick={toggleDrawer}
              sx={{
                marginRight: "36px",
                ...(open && { display: "none" }),
              }}
            >
              <MenuIcon />
            </IconButton>
            <Typography component="h1" variant="h6" color="inherit" noWrap sx={{ flexGrow: 1 }}>
              Grundlagedaten EWS
            </Typography>
            <Tooltip title="Abmelden">
              <IconButton onClick={handleLogout} sx={{ color: "white" }}>
                <LogoutIcon />
              </IconButton>
            </Tooltip>
          </Toolbar>
        </AppBar>
        <Drawer variant="permanent" open={open}>
          <Toolbar
            sx={{
              display: "flex",
              alignItems: "center",
              justifyContent: "flex-end",
              px: [1],
            }}
          >
            <IconButton onClick={toggleDrawer}>
              <ChevronLeftIcon />
            </IconButton>
          </Toolbar>
          <Divider />
          <List component="nav">
            <ListItemButton component="a" href="/">
              <ListItemIcon>
                <HomeIcon />
              </ListItemIcon>
              <ListItemText primary="Einstiegsseite" />
            </ListItemButton>
            {currentUser?.role === UserRolesMap.Administrator && (
              <ListItemButton component="a" href="/benutzerverwaltung">
                <ListItemIcon>
                  <GroupIcon />
                </ListItemIcon>
                <ListItemText primary="Benutzerverwaltung" />
              </ListItemButton>
            )}
            <ListItemButton component="a" target="_blank" href="/export">
              <ListItemIcon>
                <FileDownloadIcon />
              </ListItemIcon>
              <ListItemText primary="Daten exportieren" />
            </ListItemButton>
            <ListItemButton component="a" target="_blank" href="/help/articles/einstiegsseite.html">
              <ListItemIcon>
                <InfoIcon />
              </ListItemIcon>
              <ListItemText primary="Hilfe" />
            </ListItemButton>
          </List>
        </Drawer>
        <React.Fragment>{props.children}</React.Fragment>
        <Footer />
      </Box>
    </ThemeProvider>
  );
}
