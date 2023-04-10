import React, { useEffect, useRef, useState } from "react";
import { FilterRange, MediaReportSettings } from "./models";
import { ReportPageSize } from "./Paging";
import MediaReportComponent from "./media-report-component";


interface MediaReport {
    settings: MediaReportSettings;
}

const MediaReport = ({ settings }: MediaReport) => {
    const [mediaItems, setMediaItems] = useState([]);
    const [filterRange, setFilterRange] = useState<FilterRange>({
        minSize: 0,
        maxSize: 0,
        minReferences: 0,
        maxReferences: 0,
        minModifiedDate: new Date(),
        maxModifiedDate: new Date(),
        hasErrors: false
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
            queryString.append("showErrors", currentFilterValue.current.showErrors?.toString());
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
        isLocal?: boolean,
        showErrors?: boolean
    ) => {
        currentFilterValue.current = {
            minSize,
            maxSize,
            minReferences,
            maxReferences,
            isLocal,
            showErrors
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
            settings={settings}
        />
    );
};

export default MediaReport;

//TODO: store search filter in query string
