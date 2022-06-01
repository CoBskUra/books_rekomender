import {Pagination, Typography} from "antd"
import { LoadingOutlined } from '@ant-design/icons';

import React, { useContext, useEffect, useState } from 'react';
import "antd/dist/antd.css";
import { Card, Row, Col } from "antd";
import { ActivityIndicatorBase } from "react-native";
import { globalContext } from "../reducers/GlobalStore";
import BookListItem from "./BookListItem";
import { exampleReadBooks } from "../exampleData/ExampleItem";
import { BookItem } from "../classes/BookItem";

const { Title } = Typography;

interface Props {

}

const ReadBookListView: React.FC<Props> = (props: Props) => {
    const [_pageSize, setPageSize] = useState(5);
    const [totalPages, setTotalPages] = useState<number>(1);
    const { globalState } = useContext(globalContext);
    const [books, setBooks] = useState<BookItem[]>();
    const [loading, setLoading] = useState(false);

    function fetchData(_pageNumber : number) {
        setLoading(true);
        let url = "/api/books/read/" + globalState.loggedUser;
        console.log(url);
        fetch(url, {
            method: 'GET'
        })
            .then(response => response.json())
            .then(
                (data) => {
                    setBooks(data.books);
                    setTotalPages(data.numberOfPages);
                    setLoading(false);
                },
                (error) => {
                    console.error(error);
                }
            )
    }
    function changePageNumberHandler(pageNumber : number, pageSize: number) {
        setPageSize(pageSize);
        fetchData(pageNumber);
    }
    useEffect(() => {
        fetchData(1);
    }, [])
    return (
        <div>

            <Row style={{ marginTop: 50 }}>
                <Col flex="auto">
                    <div className="site-layout-content2">
                    <Title>My Read Books</Title>
                        { !loading ? books?.map((item: BookItem) => (
                                <BookListItem book={item} showSimilar={true}/>)
                            ) : 
                            <Col flex="auto">
                                <Row align="middle" justify="center">
                                    <LoadingOutlined style={{ fontSize: '70px' }}/>
                                </Row>
                            </Col>
                        }
                    </div> 
                    <br />
                    <Pagination onChange={changePageNumberHandler} pageSize={_pageSize} total={totalPages*_pageSize} />              
                </Col>
            </Row>
        </div>
    );
}

export default ReadBookListView;