import React from "react";
import { Link } from "@mui/material";
import { IEditableContent } from "./models";

interface EditLink {
    link: IEditableContent;
}

export default function EditLink({ link }: EditLink) {
    if (!link.editUrl) {
        return <>{link.name}</>;
    }

    return (
        <Link href={link.editUrl} target="_blank" underline="none">
            {link.name}
        </Link>
    );
}
