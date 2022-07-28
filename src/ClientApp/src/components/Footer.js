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
          position: "fixed",
          left: "0",
          bottom: "0",
          width: "98%",
          textAlign: "end",
        }}
      >
        Version: {version}
      </Typography>
    </footer>
  );
}
