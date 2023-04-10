import React from "react";
import { Grid, Typography } from "@mui/material";

interface EmptyReport {
    scheduledJonUrl: string;
}

export default function EmptyReport({ scheduledJonUrl }: EmptyReport) {
    return (
        <Grid container marginY={20} alignItems="center" justifyContent="center">
            <Grid item>
                <Typography variant="subtitle1">
                    Report has no data. Make sure, that "Media report" <a href={scheduledJonUrl}>scheduled job</a> was
                    executed.
                </Typography>
            </Grid>
        </Grid>
    );
}
