import React from "react";
import { MediaItemReference } from "./models";
import { Chip, Grid, Tooltip, tooltipClasses, TooltipProps } from "@mui/material";
import LinkIcon from "@mui/icons-material/Link";
import EditLink from "./EditLink";
import { styled } from "@mui/material/styles";

const LightTooltip = styled(({ className, ...props }: TooltipProps) => (
    <Tooltip {...props} classes={{ popper: className }} />
))(({ theme }) => ({
    [`& .${tooltipClasses.tooltip}`]: {
        backgroundColor: theme.palette.common.white,
        color: "rgba(0, 0, 0, 0.87)",
        boxShadow: theme.shadows[1],
        fontSize: 11,
    },
}));

interface References {
    references: MediaItemReference[];
}

function LinkContent({ references }: References) {
    return (
        <>
            {references.map((x) => (
                <div key={x.contentLink}>
                    <EditLink key={x.contentLink} link={x} />
                </div>
            ))}
        </>
    );
}

export default function References({ references }: References) {
    if (!references || references.length === 0) {
        return null;
    }

    return (
        <LightTooltip title={<LinkContent references={references} />}>
            <Chip icon={<LinkIcon />} label={references.length} />
        </LightTooltip>
    );
}
