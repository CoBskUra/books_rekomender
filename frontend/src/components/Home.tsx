import { Col, Row, Button } from 'antd';
import { useCallback, useContext, useState } from 'react';
import {useNavigate } from 'react-router-dom';
import { TeamOutlined, ShoppingCartOutlined } from '@ant-design/icons';
import { GlobalStore, globalContext } from '../reducers/GlobalStore';

import {Typography} from "antd"
import BookListView from '../views/BookListView';
const { Title } = Typography;


export default function Home() {
  const navigate = useNavigate();
  const handleUsersClick = useCallback(() => navigate('/users', {replace: true}), [navigate]);
  const handleBookingsClick = useCallback(() => navigate('/bookings/flats', {replace: true}), [navigate]);

    return (
      <div>
          <Row justify="space-around" align="middle" style={{minHeight: "70vh" }}>
              <Col flex="auto">
                  <BookListView />
              </Col>
          </Row>
      </div>
    );

}
