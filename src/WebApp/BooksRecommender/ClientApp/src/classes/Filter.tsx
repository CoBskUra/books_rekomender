import filterFromJSON from "./../exampleData/emptyFilter.json";

export interface Filter {
    title: string;
    author: string;
    genre: string;
    tag: string;
    minRating: number;
    maxRating: number;
}
export const emptyFilter: Filter = filterFromJSON;

