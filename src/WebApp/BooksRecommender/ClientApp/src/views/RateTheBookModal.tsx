import { Component, useState } from 'react';
import 'antd/dist/antd.css';
import { Table, Space, Button, Input, Row, Modal, Form, Rate } from 'antd';
import {Typography} from "antd"
import { BookItem } from '../classes/BookItem';

const { Title } = Typography;
const { Search } = Input;

interface Props {
    visible : boolean,
    onCancel : Function,
    onConfirm  : Function,
}


export const RateTheBookModal: React.FC<Props> = (props: Props) => {
    const [ratingValue, setRatingValue] = useState<number>();

    const passwordValidate = (password : String) => {
        return password.match(/^(?=.*[A-Z])(?=.*[0-9])(?=.{8,})/);
    }
    const ratingChangeHandle = (rating : number) => {
        setRatingValue(rating);
    } 

    return (
        <Modal title="How did you like the book?" visible={props.visible}
            onOk={() => {props.onConfirm(ratingValue);}}
            okText="Confirm"
            onCancel={(e) => props.onCancel()}>
            
            <Row justify='center'>
                <Rate allowHalf defaultValue={0} onChange={ratingChangeHandle} />
            </Row>
            
            
        </Modal>
    );
}