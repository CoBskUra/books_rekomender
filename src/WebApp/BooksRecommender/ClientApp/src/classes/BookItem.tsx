export interface BookItem {
    authors: string[];
    avgRating: number;
    country: string;
    csvId: number;
    genres: string[];
    id: number;
    languageCode: string;
    monthRentals: number;
    numPages: number;
    publicationDate: string;
    publisher: string;
    ratingsCount: number;
    tags: string[];
    targetGroups: string;
    title: string;
    readByUser: boolean;
    isFavourite?: boolean;
}

