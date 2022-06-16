import React, { useEffect, useState } from "react";
import Typography from "@mui/material/Typography";

export function Footer() {
  const [version, setVersion] = useState();
  useEffect(() => {
    async function fetchData() {
      const response = await fetch("/version");
      setVersion(await response.text());
    }

    fetchData();
  }, []);
  return (
    <footer>
      <Typography
        variant="body2"
        color="text.secondary"
        align="center"
        sx={{
          backgroundColor: (theme) =>
            theme.palette.mode === "light" ? theme.palette.grey[100] : theme.palette.grey[900],
          position: "fixed",
          left: "0",
          bottom: "0",
          width: "100%",
          textAlign: "center",
        }}
      >
        Version: {version}
      </Typography>
    </footer>
  );
}
