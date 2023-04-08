import React, { useEffect, useRef, useState } from "react";
import { Button, Checkbox, FormControlLabel, Grid, MenuItem, Select, Slider, Typography } from "@mui/material";
import { FilterRange } from "./models";
import { formatBytes } from "./format-bytes";

const IS_LOCAL_ALL = "All";
const IS_LOCAL_ONLY_LOCAL = "Local";
const IS_LOCAL_ONLY_SHARED = "Shared";
const localFilterOptions: string[] = [IS_LOCAL_ALL, IS_LOCAL_ONLY_LOCAL, IS_LOCAL_ONLY_SHARED];

const WITH_ERRORS_ALL = "All";
const WITH_ERRORS = "With errors";
const WITH_NO_ERRORS = "No errors";
const withErrorOptions: string[] = [WITH_ERRORS_ALL, WITH_ERRORS, WITH_NO_ERRORS];

export type OnFilterChangeHandler = (
    minSize: number,
    maxSize: number,
    minReferences: number,
    maxReferences: number,
    isLocal?: boolean,
    showErrors?: boolean
) => void;

interface ListFilter {
    filterRange: FilterRange;
    onFilterChange: OnFilterChangeHandler;
}

interface FilterControl {
    label: string;
    minValue?: string;
    maxValue?: string;
    children: any;
}

function FilterControl({ label, minValue, maxValue, children }: FilterControl) {
    return (
        <label>
            <Grid container alignItems="center">
                <Grid item>
                    <Typography sx={{ marginRight: 1 }}>{label}:</Typography>
                </Grid>
                {minValue && <Grid item>{minValue}</Grid>}
                <Grid item>{children}</Grid>
                {maxValue && <Grid item>{maxValue}</Grid>}
            </Grid>
        </label>
    );
}

export default function ListFilter({ filterRange, onFilterChange }: ListFilter) {
    const [isLocal, setIsLocal] = useState<string>(IS_LOCAL_ALL);
    const [size, setSize] = useState<number[]>([0, 10000]);
    const [references, setReferences] = useState<number[]>([20, 37]);
    const [showErrors, setShowErrors] = useState<string>(WITH_ERRORS_ALL);
    const currentFilterRange = useRef<FilterRange>({
        minSize: 0,
        maxSize: 0,
        minReferences: 0,
        maxReferences: 0,
        minModifiedDate: undefined,
        maxModifiedDate: undefined,
        hasErrors: false,
    });

    useEffect(() => {
        if (!filterRange) {
            return;
        }

        const current = currentFilterRange.current;
        if (
            (filterRange.minSize !== current.minSize || filterRange.maxSize !== current.maxSize) &&
            filterRange.maxSize > 0
        ) {
            setSize([filterRange.minSize, filterRange.maxSize]);
        }

        if (
            (filterRange.minReferences !== current.minReferences ||
                filterRange.maxReferences !== current.maxReferences) &&
            filterRange.maxReferences > 0
        ) {
            setReferences([filterRange.minReferences, filterRange.maxReferences]);
        }

        currentFilterRange.current = Object.assign({}, filterRange);
    }, [filterRange]);

    const handleSize = (event: Event, newValue: number | number[]) => {
        setSize(newValue as number[]);
    };

    const handleReferences = (event: Event, newValue: number | number[]) => {
        setReferences(newValue as number[]);
    };

    const getIsLocalFilterValue = (): boolean | undefined => {
        switch (isLocal) {
            case IS_LOCAL_ALL:
                return undefined;
            case IS_LOCAL_ONLY_SHARED:
                return false;
            case IS_LOCAL_ONLY_LOCAL:
                return true;
        }
        return undefined;
    };

    const getWithErrorsFilterValue = (): boolean | undefined => {
        switch (showErrors) {
            case WITH_ERRORS_ALL:
                return undefined;
            case WITH_ERRORS:
                return true;
            case WITH_NO_ERRORS:
                return false;
        }
        return undefined;
    };

    const onSearch = () => {
        onFilterChange(
            size[0],
            size[1],
            references[0],
            references[1],
            getIsLocalFilterValue(),
            getWithErrorsFilterValue()
        );
    };

    return (
        <>
            <Grid container alignItems="baseline" gap={3}>
                {filterRange?.maxSize > 0 && (
                    <Grid item>
                        <FilterControl
                            label="Size"
                            minValue={formatBytes(filterRange.minSize)}
                            maxValue={formatBytes(filterRange.maxSize)}
                        >
                            <Slider
                                aria-label="Size"
                                value={size}
                                onChange={handleSize}
                                valueLabelDisplay="auto"
                                valueLabelFormat={(value: number) => formatBytes(value)}
                                sx={{
                                    width: 200,
                                    marginX: 2,
                                }}
                                min={filterRange.minSize}
                                max={filterRange.maxSize}
                            />
                        </FilterControl>
                    </Grid>
                )}

                {filterRange?.maxReferences > 0 && (
                    <Grid item>
                        <FilterControl
                            label="References"
                            minValue={filterRange.minReferences.toString()}
                            maxValue={filterRange.maxReferences.toString()}
                        >
                            <Slider
                                aria-label="References"
                                value={references}
                                valueLabelDisplay="auto"
                                onChange={handleReferences}
                                min={filterRange.minReferences}
                                max={filterRange.maxReferences}
                                sx={{
                                    width: 200,
                                    marginX: 2,
                                }}
                            />
                        </FilterControl>
                    </Grid>
                )}

                <Grid item>
                    <FilterControl label="Local items">
                        <Select
                            labelId="media-report-filter-is-local"
                            id="demo-simple-select"
                            value={isLocal}
                            autoWidth={false}
                            sx={{
                                width: 100,
                            }}
                            onChange={(event) => setIsLocal(event.target.value)}
                        >
                            {localFilterOptions.map((x) => (
                                <MenuItem key={x} value={x}>
                                    {x}
                                </MenuItem>
                            ))}
                        </Select>
                    </FilterControl>
                </Grid>
                <Grid item>
                    <Button variant="contained" onClick={onSearch}>
                        Search
                    </Button>
                </Grid>
            </Grid>
            {filterRange.hasErrors && (
                <Grid container>
                    <Grid item>
                        <FilterControl label="Media with errors">
                            <Select
                                labelId="media-report-filter-is-local"
                                id="demo-simple-select"
                                value={showErrors}
                                autoWidth={false}
                                sx={{
                                    width: 150,
                                }}
                                onChange={(event) => setShowErrors(event.target.value)}
                            >
                                {withErrorOptions.map((x) => (
                                    <MenuItem key={x} value={x}>
                                        {x}
                                    </MenuItem>
                                ))}
                            </Select>
                        </FilterControl>
                    </Grid>
                </Grid>
            )}
        </>
    );
}
