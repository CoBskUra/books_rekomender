import React from 'react';
import "antd/dist/antd.css";
import { Form, Input, Button, Checkbox, InputNumber, Slider } from "antd";


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

            <Form.Item label="Search by author" name="authorSearch">
                <Input placeholder="Author" />
            </Form.Item>

            <Form.Item label="Search by tag" name="tagSearch">
                <Input placeholder="Tag" />
            </Form.Item>

            <Form.Item label="Search by genre" name="genreSearch">
                <Input placeholder="Genre" />
            </Form.Item>

            <Form.Item label="Filter books by rating" name="ratingFilter">
                <Slider range defaultValue={[0, 5]} disabled={false} max={5} step={0.5} />
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