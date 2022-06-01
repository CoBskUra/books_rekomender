import { useState } from 'react';
import 'antd/dist/antd.css';
import {  Row, Modal, Rate } from 'antd';

interface Props {
    visible : boolean,
    onCancel : Function,
    onConfirm  : Function,
}


export const RateTheBookModal: React.FC<Props> = (props: Props) => {
    const [ratingValue, setRatingValue] = useState<number>();

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