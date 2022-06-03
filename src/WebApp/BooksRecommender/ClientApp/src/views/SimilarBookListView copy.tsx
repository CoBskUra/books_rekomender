import {Typography} from "antd"
import { LoadingOutlined } from '@ant-design/icons';

import React, { useContext, useEffect, useState } from 'react';
import { SmileOutlined } from '@ant-design/icons';
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
    const { globalState } = useContext(globalContext);
    const [dataLoaded, setDataLoaded] = useState(false);
    const [books, setBooks] = useState<BookItem[]>();
    const [loading, setLoading] = useState(false);

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
    function fetchData() {
        setLoading(true);
        let url = "/api/books/recommend/basedOnBook/" + globalState.loggedUser + "/" + params.bookId; 
        fetch(url, {
            method: 'GET'
        })
            .then(response => {response.json(); console.log(response)})
            .then(
                (data) => {
                    console.log(data);
                    if(data !== undefined) {
                        setDataLoaded(true);
                    }
                    //setBooks(data.books);
                    setLoading(false);
                },
                (error) => {
                    console.error(error);
                }
            )
    }

    useEffect(() => {
        getBook();
        fetchData();
    }, [])


    return (
        <div>
            <Row style={{ marginTop: 50 }}>
                <Col flex="auto">
                    <div className="site-layout-content2">
                    <Title>Books similar to {basedBookTitle}</Title>
                        { !loading ? 
                            (dataLoaded ? 
                                books?.map((item: BookItem) => (
                                    <BookListItem book={item} showSimilar={false}/>)
                                )
                                :
                                <Row align="middle" justify="center" style={{ marginTop: 50 }}>
                                    <Title level={5} type="secondary">No books recommendations for you now, please check our book offer <SmileOutlined /></Title>
                                </Row>
                            ) : 
                            <Col flex="auto">
                                <Row align="middle" justify="center">
                                    <LoadingOutlined style={{ fontSize: '70px' }}/>
                                </Row>
                            </Col>
                        }
                    </div> 
                    <br />          
                </Col>
            </Row>
        </div>
    );
}

export default SimilarBookListView;