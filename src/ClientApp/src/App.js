import React, { useState, useEffect } from "react";
import { Route } from "react-router";
import { Layout } from "./components/Layout";
import { Home } from "./components/pages/Home";
import { User } from "./components/pages/User";
import CssBaseline from "@mui/material/CssBaseline";

import "@fontsource/roboto/300.css";
import "@fontsource/roboto/400.css";
import "@fontsource/roboto/500.css";
import "@fontsource/roboto/700.css";

import "./custom.css";

export default function App() {
  const [currentUser, setCurrentUser] = useState();
  useEffect(() => {
    async function fetchData() {
      const response = await fetch("/user/self");
      setCurrentUser(await response.json());
    }

    fetchData();
  }, []);

  return (
    <React.Fragment>
      <CssBaseline />
      <Layout currentUser={currentUser}>
        <Route exact path="/">
          <Home currentUser={currentUser} />
        </Route>
        <Route exact path="/benutzerverwaltung">
          <User />
        </Route>
      </Layout>
    </React.Fragment>
  );
}
