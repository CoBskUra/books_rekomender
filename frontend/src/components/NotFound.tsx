import { Link } from 'react-router-dom';
import 'antd/dist/antd.css';
import { Result, Button } from 'antd';

const NotFound = () => (
    <Result
        status="404"
        title="404"
        subTitle="Sorry, the page you visited does not exist."
        extra={<Button type="primary"><Link to="/">Back Home</Link></Button>}
    />
);

export default NotFound;