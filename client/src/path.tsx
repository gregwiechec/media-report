import React from "react";
import { MediaItemPath } from "./models";
import NavigateNextIcon from "@mui/icons-material/NavigateNext";
import { Grid } from "@mui/material";

interface Path {
  path: MediaItemPath[];
}

export default function Path({ path }: Path) {
  if (!path) {
    return null;
  }

  return (
    <Grid container alignItems="center">
      {path.map((x, index) => (
        <>
          <Grid item>{x.value}</Grid>

          {index !== path.length - 1 && (
            <Grid item>
              <NavigateNextIcon />
            </Grid>
          )}
        </>
      ))}
    </Grid>
  );
}
