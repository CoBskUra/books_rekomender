import { Component, useContext } from 'react';
import { Route, Routes, Navigate } from "react-router-dom";
import Books from './Books';
import NotFound from './NotFound';
import { globalContext } from '../reducers/GlobalStore';
import Login from '../components/Login';


export const AppRouter: React.FC = () => {
  const { globalState } = useContext(globalContext);

  return (
      <Routes>
            { !globalState.isUserAuthenticated && <Route path='*' element={<Login />}/> }
            <Route path='/login' element={<Login />}/>
            <Route path='/books' element={<Books />} />
            <Route path='*' element={<NotFound />}/>
      </Routes>
  );
}
