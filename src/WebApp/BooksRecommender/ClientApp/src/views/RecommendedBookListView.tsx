import {Form, Pagination, Select, Typography} from "antd"
import { LoadingOutlined } from '@ant-design/icons';

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
const { Option } = Select;

interface Props {

}

const RecommendedBookListView: React.FC<Props> = (props: Props) => {
    const [_pageSize, setPageSize] = useState(5);
    const [totalPages, setTotalPages] = useState<number>(1);
    const { globalState } = useContext(globalContext);
    const [books, setBooks] = useState<BookItem[]>();
    let recommendType = ("favorites");
    const [loading, setLoading] = useState(false);

    function changePageNumberHandler(pageNumber : number, pageSize: number) {
        setPageSize(pageSize);
        fetchData(pageNumber);
    }
    function fetchData(_pageNumber : number) {
        setLoading(true);
        let url = "/api/books/recommend/" + recommendType + "/" + globalState.loggedUser; 
        console.log(url);
        fetch("url", {
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
    const onRecommendTypeHandler = (value : string) => {
        recommendType = value;
        fetchData(1);
    }

    useEffect(() => {
        fetchData(1);
    }, [])


    return (
        <div>
            <Row style={{ marginTop: 50 }}>
                <Col flex="auto">
                    <div className="site-layout-content2">
                    <Title>Recommended books for me</Title>

                    <Form.Item label="Recommend books based on" name="recommendBy">
                        <Select
                            placeholder="favourites"
                            onChange={onRecommendTypeHandler}
                        >
                            <Option value="favourites">Favourites</Option>
                            <Option value="average">Average</Option>
                        </Select>
                    </Form.Item>

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

export default RecommendedBookListView;