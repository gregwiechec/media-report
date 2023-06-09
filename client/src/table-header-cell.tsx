import React, { useEffect, useRef, useState } from "react";
import { SortDirection, TableCell, TableRow, TableSortLabel } from "@mui/material";

interface TableHeaderCell {
    columnName: string;
    columnLabel: string;
    orderBy: string;
    orderDirection: SortDirection;
    onSort: (columnName: string) => void;
    width?: number;
    isSortable?: boolean;
    title?: string;
}

export default function TableHeaderCell({
    columnName,
    columnLabel,
    orderBy,
    orderDirection,
    onSort,
    width,
    isSortable = true,
    title = undefined
}: TableHeaderCell) {
    if (!isSortable) {
        return <TableCell width={width}>{columnLabel}</TableCell>;
    }
    return (
        <TableCell width={width} title={title} sortDirection={orderBy === columnName ? orderDirection : undefined}>
            <TableSortLabel
                active={orderBy === columnName}
                direction={orderBy === columnName ? (orderDirection === false ? "asc" : orderDirection) : "asc"}
                onClick={() => onSort(columnName)}
            >
                {columnLabel}
            </TableSortLabel>
        </TableCell>
    );
}
