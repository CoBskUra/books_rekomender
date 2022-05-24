import React from 'react';
import "antd/dist/antd.css";
import { Form, Input, Button, Checkbox, InputNumber } from "antd";


interface Props {
    filterResultsHandler : Function
}

const BookListFilter: React.FC<Props> = (props: Props) => {
    const onFinish = (values: any) => {
        props.filterResultsHandler(values);
      };

    return (
        <Form layout='vertical' onFinish={onFinish}>
            <Form.Item label="Search by title" name="titleSearch">
                <Input placeholder="Title" />
            </Form.Item>

            <Form.Item valuePropName="checked" name="displayRead">
                <Checkbox>Display read books</Checkbox>
            </Form.Item>

            <Form.Item>
                <Button type="primary" htmlType="submit">
                    Filter results
                </Button>
            </Form.Item>

        </Form>
    );
}

export default BookListFilter;