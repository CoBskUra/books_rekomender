import { BookItem } from "../classes/BookItem";
import bookFromJSON from "./exampleBook.json"
import booksFromJSON from "./exampleBooks.json"
import unreadBooksFromJSON from "./exampleUnreadBooks.json"
import readBooksFromJSON from "./exampleReadBooks.json"

export const exampleBook: BookItem = bookFromJSON;
export const exampleBooks: BookItem[] = booksFromJSON;
export const exampleUnreadBooks: BookItem[] = unreadBooksFromJSON;
export const exampleReadBooks: BookItem[] = readBooksFromJSON;