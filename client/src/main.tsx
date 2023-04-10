import React from "react";
import ReactDOM from "react-dom/client";
import MediaReport from "./media-report";
import { MediaReportSettings } from "./models";

const rootEl = document.getElementById("media-report-root");
const settings: MediaReportSettings = rootEl?.dataset?.settings ? JSON.parse(rootEl?.dataset?.settings) : {};
ReactDOM.createRoot(rootEl as HTMLElement).render(
    <React.StrictMode>
        <MediaReport settings={settings} />
    </React.StrictMode>
);
