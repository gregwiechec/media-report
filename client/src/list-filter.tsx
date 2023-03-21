import React, { useState } from "react";
import { Button, Grid, MenuItem, Select, SelectChangeEvent, Slider, Stack } from "@mui/material";
import { DateRange } from "@mui/x-date-pickers-pro";

const IS_LOCAL_ALL = "All";
const IS_LOCAL_ONLY_LOCAL = "Only local";
const IS_LOCAL_ONLY_SHARED = "Only shared";
const localFilterOptions: string[] = [IS_LOCAL_ALL, IS_LOCAL_ONLY_LOCAL, IS_LOCAL_ONLY_SHARED];

export default function ListFilter() {
    const [isLocal, setIsLocal] = useState<string>(IS_LOCAL_ALL);
    const [size, setSize] = useState<number[]>([0, 10000]);
    const [references, setReferences] = useState<number[]>([20, 37]);

    const handleSize = (event: Event, newValue: number | number[]) => {
        setSize(newValue as number[]);
    };

    const handleReferences = (event: Event, newValue: number | number[]) => {
        setReferences(newValue as number[]);
    };

    const handleChangeIsLocal = (localContent: string) => {
        setIsLocal(localContent);
    };

    const onSearch = () => {
        alert("search");
    };

    return (
        <Grid container alignItems="baseline" style={{ gap: 16 }}>
            <Grid item>
                <Stack spacing={2} direction="row" sx={{ mb: 1 }} alignItems="center">
                    <div>Size: {0}</div>
                    <Slider aria-label="Size" value={size} onChange={handleSize} valueLabelDisplay="auto" sx={{
                        width: 300
                    }}/>
                    <div>{5000}</div>
                </Stack>
            </Grid>

            <Grid item>
                <Stack spacing={2} direction="row" sx={{ mb: 1 }} alignItems="center">
                    <div>Number of references: {0}</div>
                    <Slider
                        aria-label="References"
                        value={references}
                        valueLabelDisplay="auto"
                        onChange={handleReferences} sx={{
                        width: 300
                    }}
                    />
                    <div>{5000}</div>
                </Stack>
            </Grid>

            <Grid item>
                <Select
                    labelId="media-report-filter-is-local"
                    id="demo-simple-select"
                    value={isLocal}
                    label="Age"
                    onChange={(event) => handleChangeIsLocal(event.target.value)}
                >
                    {localFilterOptions.map((x) => (
                        <MenuItem key={x} value={x}>
                            {x}
                        </MenuItem>
                    ))}
                </Select>
            </Grid>
            <Grid item>
                <Button variant="contained" onClick={onSearch}>Search</Button>
            </Grid>
        </Grid>
    );
}
