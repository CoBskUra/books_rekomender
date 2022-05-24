export interface BookItem {
    id: number;
    title: string;
    authors: string;
    publisher: string;
    genres: string[];
    targetGroups: string;
    tags: string[];
    pagesNum: number;
    languageCode: string;
    countryOfOrigin: string;
    publicationDate: number;
    averageRating: number;
    ratingsCount: number;
    monthRentals: number;
    readByUser: boolean;
}