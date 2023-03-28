import React, { useEffect, useState } from "react";
import { Grid, Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Tooltip } from "@mui/material";
import CheckIcon from "@mui/icons-material/Check";
import { FilterRange, MediaItemDto } from "./models";
import Path from "./path";
import { formatBytes } from "./format-bytes";
import ListFilter, { OnFilterChangeHandler } from "./list-filter";
import References from "./References";
import EditLink from "./EditLink";
import Paging from "./Paging";

interface MediaItemRow {
    item: MediaItemDto;
}

const MediaItemRow = ({ item }: MediaItemRow) => {
    return (
        <TableRow>
            <TableCell>
                <Tooltip arrow title={<img src={item.publicUrl} />}>
                    <img src={item.thumbnailUrl} />
                </Tooltip>
            </TableCell>
            <TableCell component="th" scope="row">
                <EditLink link={item} />
            </TableCell>
            <TableCell>{item.lastModified}</TableCell>
            <TableCell>
                <Path path={item.hierarchy} />
            </TableCell>
            <TableCell>
                <Grid container direction="column">
                    <Grid item>{formatBytes(item.size)}</Grid>
                    {(item.height || 0) > 0 && (item.width || 0) > 0 && (
                        <Grid item>{`(${item.width}X${item.height})`}</Grid>
                    )}
                </Grid>
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
    totalItems: number;
    onFilterChange: OnFilterChangeHandler;
    onPageChange: (pageIndex: number) => void;
}

export function MediaReportComponent({
    items,
    filterRange,
    totalItems,
    onFilterChange,
    onPageChange,
}: MediaReportComponent) {
    return (
        <>
            <Grid container marginBottom={1}>
                <Grid item xs={12}>
                    {" "}
                    <ListFilter filterRange={filterRange} onFilterChange={onFilterChange} />
                </Grid>
            </Grid>
            <TableContainer component={Paper}>
                <Table sx={{ minWidth: 650 }} aria-label="simple table">
                    <TableHead>
                        <TableRow>
                            <TableCell/>
                            <TableCell>Name</TableCell>
                            <TableCell>Last modified</TableCell>
                            <TableCell>Path</TableCell>
                            <TableCell>Size</TableCell>
                            <TableCell>Is Local</TableCell>
                            <TableCell>References</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {(items || []).map((media) => (
                            <MediaItemRow key={media.contentLink} item={media} />
                        ))}
                    </TableBody>
                </Table>
            </TableContainer>
            <Grid container marginTop={2} justifyContent="flex-end">
                <Grid item>
                    <Paging totalItems={totalItems} onPageChange={onPageChange} />
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
    const [totalItems, setTotalItems] = useState(0);

    const refreshItems = () => {
        const xhr = new XMLHttpRequest();
        xhr.onreadystatechange = function () {
            console.log("loading items");
            if (xhr.readyState === 4 && xhr.status === 200) {
                const response = JSON.parse(xhr.response);
                setMediaItems(response.items);
                setFilterRange(response.filterRange);
                setTotalItems(response.totalItems);
            }
        };
        xhr.open("get", "/MediaReport/GetMedia");
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
        alert(minSize);
        //TODO: implement filters
        refreshItems();
    };

    const onPageChanged = (pageIndex: number) => {
        //TODO: media report paging
        alert(pageIndex);
    };

    return (
        <MediaReportComponent
            items={mediaItems}
            filterRange={filterRange}
            totalItems={totalItems}
            onFilterChange={onFilterChange}
            onPageChange={onPageChanged}
        />
    );
};

export default MediaReport;
