import {Pagination, Typography} from "antd"
import { FilterOutlined } from '@ant-design/icons';

import React, { useContext, useEffect, useState } from 'react';
import "antd/dist/antd.css";
import { Card, Row, Col } from "antd";
import { ActivityIndicatorBase } from "react-native";
import { globalContext } from "../reducers/GlobalStore";

const { Title } = Typography;

interface Props {

}

const BookListView: React.FC<Props> = (props: Props) => {
    const pageSize = 100;
    const [bookingsList, setBookingsList] = useState([]);
    const [carList, setCarList] = useState([]);
    const [totalPages, setTotalPages] = useState<number | undefined>(1);
    const { globalState } = useContext(globalContext);

    function changePageNumberHandler(pageNumber : number) {
        console.log("Changed page to: " + pageNumber);
    }

    return (
        <div>

            <Row>
                <Col flex="400px">
                    <Card>
                        <Title level={2}><FilterOutlined /> Filter results</Title>
                    </Card>
                </Col>

                <Col flex="20px" />

                <Col flex="auto">
                    <div className="site-layout-content">
                    <Title>Books</Title>
                        
                    </div> 
                    <br />
                    <Pagination onChange={changePageNumberHandler} pageSize={pageSize} total={totalPages} />              
                </Col>
            </Row>
        </div>
    );
}

export default BookListView;