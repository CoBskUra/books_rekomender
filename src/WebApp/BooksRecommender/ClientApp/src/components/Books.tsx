import { Col, Row, Button } from 'antd';
import { useCallback, useContext, useEffect, useState } from 'react';
import {useLocation, useNavigate } from 'react-router-dom';
import { TeamOutlined, ShoppingCartOutlined } from '@ant-design/icons';
import { GlobalStore, globalContext } from '../reducers/GlobalStore';

import {Typography} from "antd"
import BookListView from '../views/BookListView';
import ReadBookListView from '../views/ReadBookListView';
import RecommendedBookListView from '../views/RecommendedBookListView';
const { Title } = Typography;


export default function Books() {
  const { globalState } = useContext(globalContext);
  const navigate = useNavigate();
  const location = useLocation(); 

  const getBookList = () => {
    let path = location.pathname;
    if(path.includes('read')) return <ReadBookListView />;
    if(path.includes('recommended')) return <RecommendedBookListView />;
    return <BookListView />;
  }

  useEffect(() => {
    if(!globalState.isUserAuthenticated) {
      navigate('/login', {replace: true});
    }
  }, []);

    return (
      <div>
          <Row justify="space-around" align="middle" style={{minHeight: "81vh" }}>
              <Col flex="auto">
                  {getBookList()}
              </Col>
          </Row>
      </div>
    );

}
