import React from "react";
import TextField from "@mui/material/TextField";
import Autocomplete from "@mui/material/Autocomplete";

export default function Search(props) {
  const { setValue } = props;
  const options = [
    { title: "The Shawshank Redemption", year: 1994 },
    { title: "The Godfather", year: 1972 },
    { title: "The Godfather: Part II", year: 1974 },
    { title: "The Dark Knight", year: 2008 },
    { title: "12 Angry Men", year: 1957 },
    { title: "Schindler's List", year: 1993 },
    { title: "Pulp Fiction", year: 1994 },
  ];

  return (
    <Autocomplete
      name="search-bar"
      onChange={(event, newValue) => {
        setValue(newValue);
        console.log(newValue);
      }}
      options={options.map((option) => option.title)}
      renderInput={(params) => <TextField {...params} label="Suche" />}
    ></Autocomplete>
  );
}
