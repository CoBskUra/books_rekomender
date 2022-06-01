import React, { useContext, useState } from 'react';
import { Card, Row, Col, Image, Rate, Tag, Divider, Button } from "antd";
import { BookItem } from '../classes/BookItem';

import { Typography } from 'antd';
import { RateTheBookModal } from './RateTheBookModal';
import { globalContext, GlobalStore } from '../reducers/GlobalStore';
const { Text } = Typography;

interface State {
    ratingBook: boolean
}

interface Props {
    book: BookItem
    showSimilar: boolean
}

const BookListItem: React.FC<Props> = (props: Props) => {
    const [ratingBook, setRatingBook] = useState(false);
    const { globalState } = useContext(globalContext);

    const markAsReadClickHandle = () => {
        setRatingBook(true);
    }
    const markAsReadCancelHandle = () => {
        setRatingBook(false);
        console.log("Rating has been canceled");
      }
    const markAsReadConfirmHandle = (_rating : number) => {
        let url = "/api/books/rate/" + globalState.loggedUser + "/" + props.book.id;
        console.log(url);
        fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                rating: _rating
            })
        })

        props.book.readByUser = true;
        setRatingBook(false);
    }
    const showSimilarClickHandle = () => {
        console.log("Show similar books to " + props.book.id);
    } 
    function round(value : number, step : number = 0.5) {
        step || (step = 1.0);
        var inv = 1.0 / step;
        return Math.round(value * inv) / inv;
    }

    return (
       <div className="space-align-block">
            <Card title={"Title: " + props.book.title}>
                <Row justify="space-between">
                    <Col>
                        <Row>
                            <span style={{ marginRight: 8 }}>Authors:</span>
                            {props.book.authors?.map((author, i) => (<Text style={{ marginRight: 2 }}>{author}{i < (Number(props.book.authors?.length.valueOf()) - 1) ? "," : " "}</Text>))}
                        </Row>
                        <Row>
                            Publisher: { props.book.publisher }
                        </Row>
                        <Row>
                            Publication year: {props.book.publicationDate}
                        </Row>
                        <Row>
                            <span style={{ marginRight: 8 }}>Genres:</span>
                            {props.book.genres?.map((genre, i) => (<Text style={{ marginRight: 2 }}>{genre}{i < (Number(props.book.genres?.length.valueOf()) - 1) ? "," : " "}</Text>))}
                        </Row>
                        <Row>
                            Language: {props.book.languageCode}
                        </Row>
                        <Row>
                            Number of pages: {props.book.numPages.toLocaleString()}
                        </Row>
                        <Row>
                            Country of origin: {props.book.country}
                        </Row>
                        <Row>
                            Read this month: {props.book.monthRentals.toLocaleString()}
                        </Row>
                        <Row>
                            <Divider orientation="left" style={{ marginBottom: 7 , marginTop: 10 }}/>
                            {props.book.tags?.map(tag => (<Tag>{tag}</Tag>))}
                        </Row>
                    </Col>

                    <Col>
                        <Row justify="center">
                            <Rate allowHalf disabled value={round(props.book.avgRating)} />
                        </Row>
                        <Row justify="center">
                            {props.book.ratingsCount.toLocaleString()} ratings
                        </Row>
                    </Col>
                </Row>
                <Row justify="end">
                    { 
                        props.showSimilar ? 
                            <Button onClick={showSimilarClickHandle} type="primary">Show similar books</Button> :
                            (props.book.readByUser ? 
                                <Button disabled type="primary">Mark as read</Button> : 
                                <Button onClick={markAsReadClickHandle} type="primary">Mark as read</Button>
                            )
                    }
                </Row>
            </Card>
            <br />

            <RateTheBookModal visible={ratingBook} onCancel={markAsReadCancelHandle} onConfirm={markAsReadConfirmHandle}/>
        </div>
    );
}

export default BookListItem;