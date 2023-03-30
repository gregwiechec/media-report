import React, { useEffect, useRef, useState } from "react";
import { styled } from "@mui/material/styles";
import { Tooltip, tooltipClasses, TooltipProps } from "@mui/material";

const StyledImage = styled("img")({
    maxWidth: 300,
});

interface MediaTooltip {
    publicUrl: string;
    thumbnailUrl: string;
}

const LightTooltip = styled(({ className, ...props }: TooltipProps) => (
    <Tooltip {...props} classes={{ popper: className }} />
))(({ theme }) => ({
    [`& .${tooltipClasses.tooltip}`]: {
        backgroundColor: theme.palette.common.white,
        color: "rgba(0, 0, 0, 0.87)",
        boxShadow: theme.shadows[1],
        fontSize: 11,
        border: "1px solid black"
    },
}));

export default function MediaTooltip({ publicUrl, thumbnailUrl }: MediaTooltip) {
    return (
        <LightTooltip arrow title={<StyledImage src={publicUrl} />}>
            <img src={thumbnailUrl} />
        </LightTooltip>
    );
}
