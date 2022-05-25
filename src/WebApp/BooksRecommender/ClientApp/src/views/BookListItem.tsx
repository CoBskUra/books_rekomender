import React, { useContext, useState } from 'react';
import { Card, Row, Col, Image, Rate, Tag, Divider, Button } from "antd";
import { BookItem } from '../classes/BookItem';

import { Typography } from 'antd';
import { RateTheBookModal } from './RateTheBookModal';
import { globalContext } from '../reducers/GlobalStore';
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
    const markAsReadConfirmHandle = (rating : number) => {
        console.log("Book " + props.book.id + " has been read by " + globalState.loggedUser + " and rated as " + rating);
        props.book.readByUser = true;
        setRatingBook(false);
    }
    const showSimilarClickHandle = () => {
        console.log("Show similar books to " + props.book.id);
    } 

    return (
       <div className="space-align-block">
            <Card title={"Title: " + props.book.title}>
                <Row justify="space-between">
                    <Col>
                        <Row>
                            Authors: { props.book.authors }
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
                            Number of pages: {props.book.pagesNum.toLocaleString()}
                        </Row>
                        <Row>
                            Country of origin: {props.book.countryOfOrigin}
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
                            <Rate allowHalf disabled defaultValue={props.book.averageRating} />
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