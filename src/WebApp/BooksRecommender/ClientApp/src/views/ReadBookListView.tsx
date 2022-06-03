import {Typography} from "antd"
import { LoadingOutlined } from '@ant-design/icons';

import React, { useContext, useEffect, useState } from 'react';
import "antd/dist/antd.css";
import { Row, Col } from "antd";
import { globalContext } from "../reducers/GlobalStore";
import BookListItem from "./BookListItem";
import { BookItem } from "../classes/BookItem";

const { Title } = Typography;

interface Props {

}

const ReadBookListView: React.FC<Props> = (props: Props) => {
    const { globalState } = useContext(globalContext);
    const [books, setBooks] = useState<BookItem[]>();
    const [loading, setLoading] = useState(false);

    function fetchData() {
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
                    setLoading(false);
                },
                (error) => {
                    console.error(error);
                }
            )
    }
    useEffect(() => {
        fetchData();
    }, [])
    return (
        <div>

            <Row style={{ marginTop: 50 }}>
                <Col flex="auto">
                    <div className="site-layout-content2">
                    <Title>My Read Books</Title>
                        { !loading ? books?.reverse().map((item: BookItem) => (
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
                </Col>
            </Row>
        </div>
    );
}

export default ReadBookListView;