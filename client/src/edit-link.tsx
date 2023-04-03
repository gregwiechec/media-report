import React from "react";
import { Link } from "@mui/material";
import { MediaItemDto } from "./models";

interface EditLink {
    link: MediaItemDto;
}

export default function EditLink({ link }: EditLink) {
    const title = link.contentLink + ", Content type: " + link.contentTypeName;

    if (!link.editUrl) {
        return <span title={title}>{link.name}</span>;
    }

    return (
        <Link href={link.editUrl} title={title} target="_blank" underline="none">
            {link.name}
        </Link>
    );
}
