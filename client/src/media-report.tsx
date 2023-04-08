import React, { useEffect, useRef, useState } from "react";
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
import { FilterRange, MediaItemDto } from "./models";
import Path from "./cells/path";
import ListFilter, { OnFilterChangeHandler } from "./list-filter";
import References from "./References";
import EditLink from "./cells/edit-link";
import Paging, { ReportPageSize } from "./Paging";
import EmptyReport from "./empty-report";
import MediaTooltip from "./media-tooltip";
import TableHeaderCell from "./table-header-cell";
import Size from "./cells/size";

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
}

export function MediaReportComponent({
    items,
    filterRange,
    totalCount,
    onFilterChange,
    onPageChange,
    onSortColumn,
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

            {totalCount === 0 && <EmptyReport />}

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

const MediaReport = () => {
    const [mediaItems, setMediaItems] = useState([]);
    const [filterRange, setFilterRange] = useState<FilterRange>({
        minSize: 0,
        maxSize: 0,
        minReferences: 0,
        maxReferences: 0,
        minModifiedDate: new Date(),
        maxModifiedDate: new Date(),
    });
    const [totalCount, setTotalCount] = useState(0);
    const currentFilterValue = useRef<any>();
    const currentPageIndex = useRef(0);
    const currentSortOrder = useRef<any>(null);

    const refreshItems = () => {
        const xhr = new XMLHttpRequest();
        xhr.onreadystatechange = function () {
            console.log("loading items");
            if (xhr.readyState === 4 && xhr.status === 200) {
                const response = JSON.parse(xhr.response);
                setMediaItems(response.items);
                setFilterRange(response.filterRange);
                setTotalCount(response.totalCount);
            }
        };

        const queryString = new URLSearchParams();
        queryString.append("pageIndex", currentPageIndex.current?.toString());
        queryString.append("pageSize", ReportPageSize.toString());
        if (currentFilterValue.current) {
            queryString.append("sizeFrom", currentFilterValue.current.minSize?.toString());
            queryString.append("sizeTo", currentFilterValue.current.maxSize?.toString());
            queryString.append("fromNumberOfReferences", currentFilterValue.current.minReferences?.toString());
            queryString.append("toNumberOfReferences", currentFilterValue.current.maxReferences?.toString());
            queryString.append("isLocalContent", currentFilterValue.current.isLocal?.toString());
        }
        if (currentSortOrder.current) {
            queryString.append("sortBy", currentSortOrder.current.sortOrder?.toString());
            queryString.append("sortOrder", currentSortOrder.current.orderDirection?.toString());
        }

        //TODO: url from server
        xhr.open("get", "/Episerver/Alloy.MediaReport/Report/GetMedia?" + queryString.toString());
        xhr.setRequestHeader("Accept", "application/json");
        xhr.send();
    };

    useEffect(() => {
        refreshItems();
    }, []);

    const onFilterChange = (
        minSize: number,
        maxSize: number,
        minReferences: number,
        maxReferences: number,
        isLocal?: boolean
    ) => {
        currentFilterValue.current = {
            minSize,
            maxSize,
            minReferences,
            maxReferences,
            isLocal,
        };
        refreshItems();
    };

    const onSortColumn = (sortOrder: string, orderDirection: string) => {
        currentSortOrder.current = {
            sortOrder: sortOrder,
            orderDirection: orderDirection,
        };
        refreshItems();
    };

    const onPageChanged = (pageIndex: number) => {
        currentPageIndex.current = pageIndex;
        refreshItems();
    };

    return (
        <MediaReportComponent
            items={mediaItems}
            filterRange={filterRange}
            totalCount={totalCount}
            onFilterChange={onFilterChange}
            onPageChange={onPageChanged}
            onSortColumn={onSortColumn}
        />
    );
};

export default MediaReport;

//TODO: store search filter in query string
