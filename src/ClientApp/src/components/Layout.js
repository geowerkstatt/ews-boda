import React, { useState } from "react";
import { createTheme, ThemeProvider } from "@mui/material/styles";
import CssBaseline from "@mui/material/CssBaseline";
import Box from "@mui/material/Box";
import Toolbar from "@mui/material/Toolbar";
import List from "@mui/material/List";
import Typography from "@mui/material/Typography";
import Divider from "@mui/material/Divider";
import IconButton from "@mui/material/IconButton";
import MenuIcon from "@mui/icons-material/Menu";
import ChevronLeftIcon from "@mui/icons-material/ChevronLeft";
import LogoutIcon from "@mui/icons-material/Logout";
import ListItemButton from "@mui/material/ListItemButton";
import ListItemIcon from "@mui/material/ListItemIcon";
import ListItemText from "@mui/material/ListItemText";
import HomeIcon from "@mui/icons-material/Home";
import GroupIcon from "@mui/icons-material/Group";
import InfoIcon from "@mui/icons-material/Info";
import FileDownloadIcon from "@mui/icons-material/FileDownload";
import Tooltip from "@mui/material/Tooltip";
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
              <IconButton
                onClick={() => {
                  window.location.href = "/login/sls/auth?cmd=logout";
                  setTimeout(window.location.reload, 10);
                  window.location.href = "";
                }}
                color="inherit"
                component="a"
              >
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
