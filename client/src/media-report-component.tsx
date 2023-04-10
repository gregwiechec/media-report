import React, { useState } from "react";
import {
    Grid,
    Paper,
    SortDirection,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
} from "@mui/material";
import CheckIcon from "@mui/icons-material/Check";
import MediaTooltip from "./media-tooltip";
import EditLink from "./cells/edit-link";
import Path from "./cells/path";
import Size from "./cells/size";
import References from "./References";
import ListFilter, { OnFilterChangeHandler } from "./list-filter";
import EmptyReport from "./empty-report";
import TableHeaderCell from "./table-header-cell";
import Paging from "./Paging";
import { FilterRange, MediaItemDto, MediaReportSettings } from "./models";

interface MediaItemRow {
    item: MediaItemDto;
}

const MediaItemRow = ({ item }: MediaItemRow) => {
    return (
        <TableRow>
            <TableCell>
                <MediaTooltip publicUrl={item.publicUrl} thumbnailUrl={item.thumbnailUrl} />
            </TableCell>
            <TableCell component="th" scope="row">
                <EditLink link={item} />
            </TableCell>
            <TableCell>{item.lastModified}</TableCell>
            <TableCell>
                <Path path={item.hierarchy} />
            </TableCell>
            <TableCell>
                <Size item={item} />
            </TableCell>
            <TableCell>{item.isLocalContent ? <CheckIcon /> : null}</TableCell>
            <TableCell>
                <References references={item.references} />
            </TableCell>
        </TableRow>
    );
};

interface MediaReportComponent {
    items: MediaItemDto[];
    filterRange: FilterRange;
    totalCount: number;
    onFilterChange: OnFilterChangeHandler;
    onPageChange: (pageIndex: number) => void;
    onSortColumn: (sortOrder: string, orderDirection: string) => void;
    settings: MediaReportSettings;
}

export default function MediaReportComponent({
    items,
    filterRange,
    totalCount,
    onFilterChange,
    onPageChange,
    onSortColumn,
    settings,
}: MediaReportComponent) {
    const [orderBy, setOrderBy] = useState("");
    const [orderDirection, setOrderDirection] = useState<SortDirection>("asc");
    const onSort = (sortColumn: string) => {
        let newSortDirection: SortDirection;
        if (sortColumn === orderBy) {
            newSortDirection = orderDirection === "asc" ? "desc" : "asc";
        } else {
            newSortDirection = "asc";
        }
        setOrderBy(sortColumn);
        setOrderDirection(newSortDirection);
        onSortColumn(sortColumn, newSortDirection);
    };

    const columns = [
        {
            name: "name",
            label: "Name",
        },
        {
            name: "modifiedDate",
            label: "Last modified",
            width: 150,
        },
        {
            name: "path",
            label: "Path",
            isSortable: false,
        },
        {
            name: "size",
            label: "Size",
            width: 100,
        },
        {
            name: "isLocalContent",
            label: "Local",
            title: "Is content stored in 'For this page' folder",
            width: 70,
        },
        {
            name: "numberOfReferences",
            label: "Refs",
            title: "Number of references to content",
            width: 70,
        },
    ];

    return (
        <>
            <Grid container marginBottom={1}>
                <Grid item xs={12}>
                    <ListFilter filterRange={filterRange} onFilterChange={onFilterChange} />
                </Grid>
            </Grid>

            {totalCount === 0 && <EmptyReport scheduledJonUrl={settings.mediaReportScheduledJobUrl} />}

            {totalCount > 0 && (
                <TableContainer component={Paper}>
                    <Table sx={{ minWidth: 650 }} aria-label="simple table">
                        <TableHead>
                            <TableRow>
                                <TableCell />
                                {columns.map((x) => (
                                    <TableHeaderCell
                                        key={x.name}
                                        columnName={x.name}
                                        columnLabel={x.label}
                                        title={x.title}
                                        orderBy={orderBy}
                                        orderDirection={orderDirection}
                                        onSort={onSort}
                                        width={x.width}
                                        isSortable={x.isSortable}
                                    />
                                ))}
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {(items || []).map((media) => (
                                <MediaItemRow key={media.contentLink} item={media} />
                            ))}
                        </TableBody>
                    </Table>
                </TableContainer>
            )}
            <Grid container marginTop={2} justifyContent="flex-end">
                <Grid item>
                    <Paging totalCount={totalCount} onPageChange={onPageChange} />
                </Grid>
            </Grid>
        </>
    );
}
