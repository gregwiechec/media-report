import React from "react";
import { Pagination } from "@mui/material";

interface Paging {
    totalItems: number;
    onPageChange: (pageIndex: number) => void;
}

export const ReportPageSize = 100;

export default function Paging({ totalItems, onPageChange }: Paging) {
    const pageCount = totalItems / ReportPageSize;

    if (pageCount < 0) {
        return null;
    }

    const handleChange = (event: React.ChangeEvent<unknown>, value: number) => {
        onPageChange(value - 1);
    };

    return <Pagination count={pageCount} onChange={handleChange} />;
}
