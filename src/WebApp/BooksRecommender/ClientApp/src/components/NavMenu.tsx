import { useContext, useState } from 'react';
import { NavLink } from 'reactstrap';
import { useLocation, Link } from 'react-router-dom';
import './NavMenu.css';
import 'antd/dist/antd.css';
import { UserOutlined, BookOutlined, ScheduleOutlined, HeartOutlined } from '@ant-design/icons';

import { Button, Layout, Menu, message } from 'antd';
import { globalContext } from '../reducers/GlobalStore';
const { Header } = Layout;

export function NavMenu() {
  const [avatarClicked, setAvatarClicked] = useState(false);
  const location = useLocation(); 
  const { globalState, dispatch } = useContext(globalContext);

  const getSelectedKeyFromPath = () => {
    let path = location.pathname;
    if(path.includes('read')) return ['ReadBooks'];
    if(path.includes('recommended')) return ['RecommendedBooks'];
    if(path.includes('all')) return ['Books'];
    return ['None'];
  }
  const navigateTo_IfLoggedIn = (to : string) => {
    return globalState.isUserAuthenticated ? to : "/login"
  }
  const avatarClickHandle = () => {
    setAvatarClicked(!avatarClicked);
  }
  const logout = () => {
    dispatch({ type: 'AUTHENTICATE_USER', payload: false });
    setAvatarClicked(false);
    message.success('Logged out succesfully!');
  }

  return (
    <Header >
       { globalState.isUserAuthenticated &&  
        <div className="user-avatar">
          <Button icon={<UserOutlined />} shape="circle" onClick={avatarClickHandle}/>
        </div> }
     
      <Menu theme="dark" mode="horizontal" selectedKeys={getSelectedKeyFromPath()}>
          {
            globalState.isUserAuthenticated && avatarClicked &&
              <Menu.Item key="logout" onClick={logout}><NavLink tag={Link} to="/login">Log out</NavLink></Menu.Item>
          }
        <Menu.Item key="Books" icon={<BookOutlined />}><NavLink tag={Link} to={navigateTo_IfLoggedIn("/books/all")}>Books</NavLink></Menu.Item>
        <Menu.Item key="RecommendedBooks" icon={<HeartOutlined />}><NavLink tag={Link} to={navigateTo_IfLoggedIn("/books/recommended")}>Recommended books</NavLink></Menu.Item>
        <Menu.Item key="ReadBooks" icon={<ScheduleOutlined />}><NavLink tag={Link} to={navigateTo_IfLoggedIn("/books/read")}>Read books</NavLink></Menu.Item>
      </Menu>
      
    </Header>
  );
}
