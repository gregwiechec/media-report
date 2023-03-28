import React from "react";
import { Grid, Typography } from "@mui/material";

export default function EmptyReport() {
    return <Grid container marginY={20} alignItems="center" justifyContent="center">
        <Grid item>
            <Typography variant="subtitle1">
                Report has no data. Make sure, that "Media report" scheduled job is running.
            </Typography>
        </Grid>
    </Grid>
}
