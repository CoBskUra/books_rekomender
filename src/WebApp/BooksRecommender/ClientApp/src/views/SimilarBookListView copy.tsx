import {Pagination, Typography} from "antd"
import { LoadingOutlined } from '@ant-design/icons';

import React, { useContext, useEffect, useState } from 'react';
import "antd/dist/antd.css";
import { Row, Col } from "antd";
import { globalContext } from "../reducers/GlobalStore";
import BookListItem from "./BookListItem";
import { BookItem } from "../classes/BookItem";
import { useParams } from "react-router-dom";

const { Title } = Typography;

interface Props {
}

const SimilarBookListView: React.FC<Props> = (props: Props) => {
    const params = useParams();
    const [basedBookTitle, setBasedBookTitle] = useState("");
    const [_pageSize, setPageSize] = useState(5);
    const [totalPages, setTotalPages] = useState<number>(1);
    const { globalState } = useContext(globalContext);
    const [books, setBooks] = useState<BookItem[]>();
    const [loading, setLoading] = useState(false);

    function changePageNumberHandler(pageNumber : number, pageSize: number) {
        setPageSize(pageSize);
        fetchData(pageNumber);
    }
    function getBook() {
        let url = "/api/books/" + params.bookId;
        fetch(url, {
            method: 'GET'
        })
            .then(response => response.json())
            .then(
                (data) => {
                    setBasedBookTitle(data.title);
                    setLoading(false);
                },
                (error) => {
                    console.error(error);
                }
            )
    }
    function fetchData(_pageNumber : number) {
        setLoading(true);
        let url = "/api/books/recommend/basedOnBook/" + globalState.loggedUser + "/" + params.bookId; 
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

    useEffect(() => {
        getBook();
        fetchData(1);
    }, [])


    return (
        <div>
            <Row style={{ marginTop: 50 }}>
                <Col flex="auto">
                    <div className="site-layout-content2">
                    <Title>Books similar to {basedBookTitle}</Title>

                        { !loading ? books?.map((item: BookItem) => (
                                <BookListItem book={item} showSimilar={false}/>)
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

export default SimilarBookListView;