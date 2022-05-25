import { UserOutlined, LockOutlined } from '@ant-design/icons';
import { Row, Button, Input, Form, message, Alert } from 'antd';
import { useContext } from 'react';
import {useNavigate} from 'react-router-dom';
import { useState } from 'react';
import 'antd/dist/antd.css';
import { globalContext } from '../reducers/GlobalStore';

interface Props {
}

export default function Login(props : Props) {
    const [form] = Form.useForm();
    const { globalState, dispatch } = useContext(globalContext);
    const navigate = useNavigate();
    const [userNotFound, setUserNotFound] = useState(false);

    const passwordValidate = (password : String) => {
        return password.length > 0;
    }  

    const successfullLogIn = (user : any, token : string) => {
        setUserNotFound(false);
        dispatch({ type: 'AUTHENTICATE_USER', payload: true });
        dispatch({ type: 'SET_TOKEN', payload: token });
        dispatch({ type: 'SET_USER', payload: user.email });
        message.success('Logged in succesfully!');
        navigate('/books/all', {replace: true});
    }

    const loginHandler = (user : any) => {
        // props.userService.login(user.email, user.password)
        //     .then(response => {
        //         successfullLogIn(user, "Bearer " + response.jwttoken);
        //     }).catch(error => 
        //         {
        //             setUserNotFound(true);
        //             console.log(user);
        //             console.error(error)
        //         });
        successfullLogIn(user, "Bearer " + "abc");
    }

    return (
        <div>
            <Row justify="space-around" align="middle" style={{minHeight: "81vh" }}>
                <Form 
                    layout='vertical'
                    form={form}
                    name="normal_login"
                    className="login-form"
                    onFinish={loginHandler}
                >
                        <Form.Item name="email" label="Email"
                            rules={[
                                {
                                    required: true,
                                    message: 'Email cannot be empty!',
                                },
                                {
                                    type: 'email',
                                    message: 'Please enter valid email!'
                                },
                        ]}>
                            <Input prefix={<UserOutlined className="site-form-item-icon" />} placeholder="Email" size="large" style={{ width: "60vh" }}/>
                        </Form.Item>
                        <Form.Item label="Password" name="password" hasFeedback
                            rules={[
                            {
                                required: true,
                                message: 'Please input your password!',
                            },
                            ({ }) => ({
                                validator(_: any, value: String) {
                                if (passwordValidate(value)) {
                                    return Promise.resolve();
                                }

                                return Promise.reject();
                                },
                            }),
                            ]}
                        >
                            <Input.Password 
                                prefix={<LockOutlined className="site-form-item-icon" />}
                                type="password"
                                placeholder="Password"
                                size="large"
                                style={{ width: "60vh" }}
                            />
                        </Form.Item>

                        <Form.Item>
                            <Button type="primary" htmlType="submit" className="login-form-button" style={{width: "60vh" }}>Log in</Button>
                        </Form.Item>

                        {
                            userNotFound && 
                            <Alert
                            message="Incorrect data"
                            description="We could not log you in, check if your login data is correct."
                            type="error"
                            showIcon
                            />
                        }
                        
                </Form>
            </Row>
        </div>
    );

}
