import React from "react";
import { Grid } from "@mui/material";
import NavigateNextIcon from "@mui/icons-material/NavigateNext";
import { MediaItemPath } from "./models";

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
                            <NavigateNextIcon
                                sx={{
                                    height: 16,
                                    width: 16,
                                }}
                            />
                        </Grid>
                    )}
                </>
            ))}
        </Grid>
    );
}
