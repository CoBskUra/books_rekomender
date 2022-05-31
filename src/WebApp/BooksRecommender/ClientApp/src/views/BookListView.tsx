import {Pagination, Typography} from "antd"
import { FilterOutlined } from '@ant-design/icons';

import React, { useContext, useEffect, useState } from 'react';
import "antd/dist/antd.css";
import { Card, Row, Col } from "antd";
import { ActivityIndicatorBase } from "react-native";
import { globalContext } from "../reducers/GlobalStore";
import BookListItem from "./BookListItem";
import { exampleBooks, exampleUnreadBooks } from "../exampleData/ExampleItem";
import { BookItem } from "../classes/BookItem";
import BookListFilter from "./BookListFilter";
import { validateLocaleAndSetLanguage } from "typescript";


const { Title } = Typography;

interface Props {

}

const BookListView: React.FC<Props> = (props: Props) => {
    const _pageSize = 5;
    const [totalElements, setTotalElements] = useState<number>(exampleBooks.length);
    const { globalState } = useContext(globalContext);
    const [books, setBooks] = useState(exampleBooks);

    function changePageNumberHandler(pageNumber : number) {
        console.log("Changed page to: " + pageNumber);
    }

    useEffect(() => {
        // tu dane ma pobieraæ
        let url = "/api/books/filter";
        console.log(url);
        fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
                // 'Content-Type': 'application/x-www-form-urlencoded',
            },
            body: JSON.stringify({
                pageNumber: 1,
                pageSize: _pageSize,
                minRating: 0,
                maxRating: 5
            })
        })
            .then(response => response.json())
            .then(
                (data) => {
                    console.log(data);
                },
                (error) => {
                    console.error(error);
                }
            )
    }, [])

    // to jest funkcja jedynie podgladowa do danych przykladowych
    const filterResultsHandler = (values: any) => {
        let books = exampleBooks;

        if(!values.displayRead) books = books.filter(book => book.readByUser != true);
        if(values.titleSearch !== undefined)
        {
            books = books.filter(book => book.title.toLocaleLowerCase().includes(values.titleSearch.toLowerCase())); 
        }
        
        setBooks(books);
        console.log("Filtered Books " + values.titleSearch + " " + values.displayRead);
    };


    return (
        <div>
            <Row style={{ marginTop: 50 }}>
                <Col flex="400px">
                    <Card>
                        <Title level={2}><FilterOutlined /> Filter results</Title>
                        <BookListFilter filterResultsHandler={filterResultsHandler} />
                    </Card>
                </Col>

                <Col flex="20px" />

                <Col flex="auto">
                    <div className="site-layout-content">
                    <Title>Books</Title>
                        {books.map((item: BookItem) => (
                            <BookListItem book={item} showSimilar={false}/>)
                        )}
                    </div> 
                    <br />
                    <Pagination onChange={changePageNumberHandler} pageSize={_pageSize} total={totalElements} />              
                </Col>
            </Row>
        </div>
    );
}

export default BookListView;