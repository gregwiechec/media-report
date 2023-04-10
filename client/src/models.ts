export interface FilterRange {
    minSize: number;
    maxSize: number;

    minModifiedDate?: Date;
    maxModifiedDate?: Date;

    minReferences: number;
    maxReferences: number;

    hasErrors: boolean;
}

export interface MediaItemPath {
    key: string;
    value: string;
}

export interface MediaItemReference {
    contentLink: string;
    name: string;
    editUrl: string;
}

export interface MediaItemDto {
    contentLink: string;
    name: string;
    contentTypeName: string;
    editUrl: string;
    publicUrl: string;
    thumbnailUrl: string;
    hierarchy: MediaItemPath[];
    size: number;
    width?: number;
    height?: number;
    lastModified: string;
    isLocalContent: boolean;
    references: MediaItemReference[];
    errorText?: string;
}

export interface MediaReportSettings {
    mediaReportScheduledJobUrl: string;
}
