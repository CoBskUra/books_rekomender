import { Col, Row, Button } from 'antd';
import { useCallback, useContext, useEffect, useState } from 'react';
import {useNavigate } from 'react-router-dom';
import { TeamOutlined, ShoppingCartOutlined } from '@ant-design/icons';
import { GlobalStore, globalContext } from '../reducers/GlobalStore';

import {Typography} from "antd"
import ReadBookListView from '../views/ReadBookListView';
const { Title } = Typography;


export default function ReadBooks() {
  const { globalState } = useContext(globalContext);
  const navigate = useNavigate();

  useEffect(() => {
    if(!globalState.isUserAuthenticated) {
      navigate('/login', {replace: true});
    }
  }, []);

    return (
      <div>
          <Row justify="space-around" align="middle" style={{minHeight: "81vh" }}>
              <Col flex="auto">
                  <ReadBookListView />
              </Col>
          </Row>
      </div>
    );

}
