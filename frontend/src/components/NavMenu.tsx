import { Component, useContext } from 'react';
import { NavLink } from 'reactstrap';
import { useLocation, Link } from 'react-router-dom';
import './NavMenu.css';
import 'antd/dist/antd.css';
import { UserOutlined, BookOutlined, ScheduleOutlined } from '@ant-design/icons';

import { Avatar, Layout, Menu } from 'antd';
import { GlobalStore, globalContext } from '../reducers/GlobalStore';
const { Header } = Layout;
const { SubMenu } = Menu;

export function NavMenu() {
  const location = useLocation(); 
  const { globalState } = useContext(globalContext);


  const getSelectedKeyFromPath = () => {
    let path = location.pathname;
    if(path.includes('flats')) return ['Flats'];
    if(path.includes('cars')) return ['Cars'];
    if(path.includes('parking_spots')) return ['ParkingSpots'];
    if(path.includes('users')) return ['Users'];
    return ['AdminPanel'];
  }

  const navigateTo_IfLoggedIn = (to : string) => {
    return globalState.isUserAuthenticated ? to : "/login"
  }

  return (
    <Header >
       { globalState.isUserAuthenticated &&  <div className="user-avatar"><Avatar icon={<UserOutlined />} /></div> }
     

      <Menu theme="dark" mode="horizontal" selectedKeys={getSelectedKeyFromPath()}>
        <Menu.Item key="Books" icon={<BookOutlined />}><NavLink tag={Link} to={navigateTo_IfLoggedIn("/books/all")}>Books</NavLink></Menu.Item>
        <Menu.Item key="ReadBooks" icon={<ScheduleOutlined />}><NavLink tag={Link} to={navigateTo_IfLoggedIn("/books/read")}>My read books</NavLink></Menu.Item>
      </Menu>
      
    </Header>
  );
}
