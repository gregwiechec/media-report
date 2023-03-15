export interface MediaItemPath {
    key: string;
    value: string;
}

export interface IEditableContent {
    name: string;
    editUrl: string;
}

export interface MediaItemReference extends IEditableContent {
    contentLink: string;
    name: string;
    editUrl: string;
}

export interface MediaItemDto extends IEditableContent {
    contentLink: string;
    name: string;
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
}
