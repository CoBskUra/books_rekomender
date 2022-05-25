import {Pagination, Typography} from "antd"
import { FilterOutlined } from '@ant-design/icons';

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

const BookListView: React.FC<Props> = (props: Props) => {
    const pageSize = 5;
    const [totalElements, setTotalElements] = useState<number>(exampleReadBooks.length);
    const { globalState } = useContext(globalContext);

    function changePageNumberHandler(pageNumber : number) {
        console.log("Changed page to: " + pageNumber);
    }

    return (
        <div>

            <Row style={{ marginTop: 50 }}>
                <Col flex="auto">
                    <div className="site-layout-content">
                    <Title>My Read Books</Title>
                        {exampleReadBooks.map((item: BookItem) => (
                            <BookListItem book={item}/>)
                        )}
                    </div> 
                    <br />
                    <Pagination onChange={changePageNumberHandler} pageSize={pageSize} total={totalElements} />              
                </Col>
            </Row>
        </div>
    );
}

export default BookListView;