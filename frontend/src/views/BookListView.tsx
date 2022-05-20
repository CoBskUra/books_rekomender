import {Pagination, Typography} from "antd"
import { FilterOutlined } from '@ant-design/icons';

import React, { useContext, useEffect, useState } from 'react';
import "antd/dist/antd.css";
import { Card, Row, Col } from "antd";
import { ActivityIndicatorBase } from "react-native";
import { globalContext } from "../reducers/GlobalStore";
import BookListItem from "./BookListItem";
import { exampleBook } from "../exampleData/ExampleItem";

const { Title } = Typography;

interface Props {

}

const BookListView: React.FC<Props> = (props: Props) => {
    const pageSize = 5;
    const [totalElements, setTotalElements] = useState<number | undefined>(1);
    const { globalState } = useContext(globalContext);

    function changePageNumberHandler(pageNumber : number) {
        console.log("Changed page to: " + pageNumber);
    }

    return (
        <div>

            <Row style={{ marginTop: 50 }}>
                <Col flex="400px">
                    <Card>
                        <Title level={2}><FilterOutlined /> Filter results</Title>
                    </Card>
                </Col>

                <Col flex="20px" />

                <Col flex="auto">
                    <div className="site-layout-content">
                    <Title>Books</Title>
                        {Array(2).fill(null).map((item, index) => (
                            <BookListItem book={exampleBook}/>)
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