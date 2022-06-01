import { Col, Row } from 'antd';
import { useContext, useEffect } from 'react';
import {useLocation, useNavigate } from 'react-router-dom';
import { globalContext } from '../reducers/GlobalStore';

import BookListView from '../views/BookListView';
import ReadBookListView from '../views/ReadBookListView';
import RecommendedBookListView from '../views/RecommendedBookListView';

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
  }, [globalState.isUserAuthenticated, navigate]);

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
