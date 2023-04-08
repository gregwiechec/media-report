import React from "react";
import { Grid, Tooltip } from "@mui/material";
import InfoIcon from "@mui/icons-material/Info";
import { formatBytes } from "../format-bytes";
import { MediaItemDto } from "../models";

const CannotReadMediaSize = -1;

interface Size {
    item: MediaItemDto;
}

export default function Size({ item }: Size) {
    if (item?.size === CannotReadMediaSize) {
        return (
            <Tooltip title={item.errorText}>
                <Grid container alignItems="center" gap={1} sx={{ cursor: "pointer", userSelect: "none" }}>
                    <Grid item>Error</Grid>
                    <Grid item sx={{ lineHeight: 0 }}>
                        <InfoIcon />
                    </Grid>
                </Grid>
            </Tooltip>
        );
    }

    return (
        <Grid container direction="column">
            <Grid item>{formatBytes(item.size)}</Grid>
            {(item.height || 0) > 0 && (item.width || 0) > 0 && <Grid item>{`(${item.width}X${item.height})`}</Grid>}
        </Grid>
    );
}
