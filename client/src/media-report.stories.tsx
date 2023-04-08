import React from "react";
import { ComponentStory, ComponentMeta } from "@storybook/react";
import { MediaReportComponent } from "./media-report";
import data from "./__tests__/media-report-data.json";
import { FilterRange, MediaItemDto } from "./models";
import { action } from "@storybook/addon-actions";

export default {
    title: "Media report",
    component: MediaReportComponent,
    args: {
        items: [],
        onFilterChange: (
            minSize: number,
            maxSize: number,
            minReferences: number,
            maxReferences: number,
            isLocal?: boolean,
            showErrors?: boolean
        ) =>
            action(
                `Size: ${minSize} - ${maxSize}, references: ${minReferences} - ${maxReferences}, local: ${isLocal}, show errors: ${showErrors}`
            )(),
        onPageChange: (pageIndex) => action("pageIndex: " + pageIndex)(),
    },
} as ComponentMeta<typeof MediaReportComponent>;

const getDefaultFilter = (hasErrors = false): FilterRange => ({
    minSize: 250,
    maxSize: 2133232,

    minModifiedDate: new Date(),
    maxModifiedDate: new Date(),

    minReferences: 0,
    maxReferences: 11,
    hasErrors: hasErrors,
});

const Template: ComponentStory<typeof MediaReportComponent> = (args) => <MediaReportComponent {...args} />;

export const Default = Template.bind({});
Default.args = {
    items: data,
    filterRange: getDefaultFilter(),
    totalCount: 10,
};

export const LongList = Template.bind({});
LongList.args = {
    items: [...Array(300).keys()].map((index) => ({
        name: "Media " + (index + 1),
        contentLink: (index + 1).toString(),
        editUrl: "https://www.google.com",
        publicUrl: "",
        contentTypeName: "test",
        hierarchy: [
            {
                key: "123",
                value: "Test",
            },
            {
                key: "1234",
                value: "Test2",
            },
            {
                key: "12345",
                value: "Test3",
            },
        ],
        thumbnailUrl: "https://loremflickr.com/320/240?random=" + index,
        lastModified: "2023-01-01 20:22",
        size: (index + 1) * 1000,
        isLocalContent: index % 5 === 1,
        references: [],
        errorText: "",
    })),
    filterRange: getDefaultFilter(),
    totalCount: 500,
};

export const Empty = Template.bind({});
Empty.args = {
    items: [],
    filterRange: getDefaultFilter(),
    totalCount: 0,
};

const getItem = (name: string, size = 100, errorText = ""): MediaItemDto => ({
    name: name,
    contentTypeName: "aaa",
    width: size == -1 ? 0 : 100,
    height: size === -1 ? 0 : 100,
    contentLink: "123",
    editUrl: "",
    lastModified: "",
    isLocalContent: false,
    size: size,
    errorText: errorText,
    hierarchy: [],
    thumbnailUrl: "",
    references: [],
    publicUrl: "",
});

export const WithErrors = Template.bind({});
WithErrors.args = {
    items: [getItem("test"), getItem("test 2"), getItem("test 3", -1, "Cannot read file")],
    filterRange: getDefaultFilter(true),
    totalCount: 100,
};
