import React from "react";
import { Pagination } from "@mui/material";

interface Paging {
    totalCount: number;
    onPageChange: (pageIndex: number) => void;
}

export const ReportPageSize = 100;

export default function Paging({ totalCount, onPageChange }: Paging) {
    const pageCount = totalCount / ReportPageSize;

    if (pageCount <= 1) {
        return null;
    }

    const handleChange = (event: React.ChangeEvent<unknown>, value: number) => {
        onPageChange(value - 1);
    };

    return <Pagination count={pageCount} onChange={handleChange} />;
}
