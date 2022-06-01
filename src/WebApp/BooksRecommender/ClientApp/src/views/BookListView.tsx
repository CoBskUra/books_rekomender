import {Form, Pagination, Select, Typography} from "antd"
import { FilterOutlined, LoadingOutlined } from '@ant-design/icons';

import React, { useContext, useEffect, useState } from 'react';
import "antd/dist/antd.css";
import { Card, Row, Col } from "antd";
import { globalContext } from "../reducers/GlobalStore";
import BookListItem from "./BookListItem";
import { BookItem } from "../classes/BookItem";
import BookListFilter from "./BookListFilter";
import { emptyFilter } from "../classes/Filter";


const { Title } = Typography;
const { Option } = Select;

interface Props {
}

const BookListView: React.FC<Props> = (props: Props) => {
    const [_pageSize, setPageSize] = useState(5);
    const [totalPages, setTotalPages] = useState<number>(1);
    const { globalState } = useContext(globalContext);
    const [books, setBooks] = useState<BookItem[]>();
    let filter = emptyFilter;
    let _orderBy = "Rating";
    const [loading, setLoading] = useState(false);

    function changePageNumberHandler(pageNumber : number, pageSize: number) {
        setPageSize(pageSize);
        fetchData(pageNumber);
    }
    function fetchData(_pageNumber : number) {
        setLoading(true);
        fetch("/api/books/filter", {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                pageNumber: _pageNumber,
                pageSize: _pageSize,
                title: filter.title,
                author: filter.author,
                minRating: filter.minRating,
                maxRating: filter.maxRating,
                tag: filter.tag,
                genre: filter.genre,
                email: globalState.loggedUser,
                orderBy: _orderBy
            })
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
        fetchData(1);
    }, [filter, _orderBy])

    const filterResultsHandler = (values: any) => {
        let _filter = {
            ...emptyFilter,
            minRating : (values.ratingFilter === undefined) ? 0 : values.ratingFilter[0],
            maxRating : (values.ratingFilter === undefined) ? 5 : values.ratingFilter[1]
        }

        if(values.titleSearch !== undefined)
        {
            _filter = {
                ..._filter,
                title : values.titleSearch
            }
        }
        if(values.authorSearch !== undefined)
        {
            _filter = {
                ..._filter,
                author : values.authorSearch
            }
        }
        if(values.tagSearch !== undefined)
        {
            _filter = {
                ..._filter,
                tag : values.tagSearch
            }
        }
        if(values.genreSearch !== undefined)
        {
            _filter = {
                ..._filter,
                genre : values.genreSearch
            }
        }
        filter = _filter;  
        fetchData(1);
    };
    const onOrderByChangeHandler = (value : string) => {
        _orderBy = value;
        fetchData(1);
    }


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

                    <Form.Item label="Order by" name="orderBy">
                        <Select
                            placeholder="Rating"
                            onChange={onOrderByChangeHandler}
                        >
                            <Option value="Rating">Rating</Option>
                            <Option value="Title">Title</Option>
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

export default BookListView;