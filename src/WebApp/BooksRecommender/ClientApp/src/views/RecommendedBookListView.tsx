import {Form, Select, Typography} from "antd"
import { LoadingOutlined } from '@ant-design/icons';

import React, { useContext, useEffect, useState } from 'react';
import { SmileOutlined } from '@ant-design/icons';
import "antd/dist/antd.css";
import { Row, Col } from "antd";
import { globalContext } from "../reducers/GlobalStore";
import BookListItem from "./BookListItem";
import { BookItem } from "../classes/BookItem";

const { Title } = Typography;
const { Option } = Select;


const RecommendedBookListView: React.FC = () => {
    const { globalState } = useContext(globalContext);
    const [books, setBooks] = useState<BookItem[]>();
    const [dataLoaded, setDataLoaded] = useState(false);
    let recommendType = ("favorites");
    const [loading, setLoading] = useState(false);

    function fetchData() {
        setLoading(true);
        let url = "/api/books/recommend/" + recommendType + "/" + globalState.loggedUser; 
        fetch(url, {
            method: 'GET'
        })
            .then(response => response.json())
            .then(
                (data) => {
                    if(data !== undefined) {
                        setBooks(data);
                        setDataLoaded(true);
                    }
                    setLoading(false);
                },
                (error) => {
                    console.error(error);
                }
            )
    }
    const onRecommendTypeHandler = (value : string) => {
        recommendType = value;
        fetchData();
    }

    useEffect(() => {
        fetchData();
    }, [])


    return (
        <div>
            <Row style={{ marginTop: 50 }}>
                <Col flex="auto">
                    <div className="site-layout-content">
                        <Title>Recommended books for me</Title>

                        <Form.Item label="Recommend books based on" name="recommendBy">
                            <Select
                                placeholder="Favourites"
                                onChange={onRecommendTypeHandler}
                            >
                                <Option value="favorites">Favourites</Option>
                                <Option value="average">Average</Option>
                            </Select>
                        </Form.Item>

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

export default RecommendedBookListView;