import React from "react";
import { ComponentStory, ComponentMeta } from "@storybook/react";
import { MediaReportComponent } from "./media-report";
import data from "./_tests_/media-report-data.json";

export default {
  title: "Media report",
  component: MediaReportComponent,
  argTypes: {
    items: [],
  },
} as ComponentMeta<typeof MediaReportComponent>;

const Template: ComponentStory<typeof MediaReportComponent> = (args) => (
  <MediaReportComponent {...args} />
);

export const Default = Template.bind({});
Default.args = {
  items: data,
};

export const LongList = Template.bind({});
LongList.args = {
  items: [...Array(300).keys()].map((index) => ({
    name: "Media " + (index + 1),
    contentLink: (index + 1).toString(),
    editUrl: "https://www.google.com",
    publicUrl: "",
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
    thumbnailUrl: "",
    lastModified: "",
    size: (index + 1) * 1000,
    isLocalContent: index % 5 === 1,
    references: []
  })),
};
