import React, { useEffect, useState } from "react";
import {
    Grid,
    Link,
    Paper,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    Tooltip,
} from "@mui/material";
import CheckIcon from "@mui/icons-material/Check";
import { MediaItemDto } from "./models";
import Path from "./path";
import { formatBytes } from "./format-bytes";
import ListFilter from "./list-filter";
import References from "./References";
import EditLink from "./EditLink";

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
}

export function MediaReportComponent({ items }: MediaReportComponent) {
    return (
        <>
            <ListFilter />
            <TableContainer component={Paper}>
                <Table sx={{ minWidth: 650 }} aria-label="simple table">
                    <TableHead>
                        <TableRow>
                            <TableCell></TableCell>
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
        </>
    );
}

const MediaReport = () => {
    const [mediaItems, setMediaItems] = useState([]);

    useEffect(() => {
        const xhr = new XMLHttpRequest();
        xhr.onreadystatechange = function () {
            console.log("loading items");
            if (xhr.readyState === 4 && xhr.status === 200) {
                const response = JSON.parse(xhr.response);
                setMediaItems(response.items);
                //TODO: media report paging
            }
        };
        xhr.open("get", "/MediaReport/GetMedia");
        xhr.setRequestHeader("Accept", "application/json");
        xhr.send();
    }, []);

    return <MediaReportComponent items={mediaItems} />;
};

export default MediaReport;
