import { Card, CardBody, CardTitle } from "reactstrap";
import UserPartial from "../UserComponents/UserPartial";
import { Link } from "react-router-dom";

export default function Post({post, onDetails}){
    return <Card>
        <CardTitle>
            {!onDetails ? <Link to={`posts/${post.id}`}><h2>{post.title}</h2></Link> : <h2>{post.title}</h2>}
        </CardTitle>
        <CardBody>
            {
                post.content.length >= 72 && !onDetails ? post.content.substring(0, 72)+"..." : post.content
            }
        </CardBody>
        <UserPartial userInfo={post.user}/>
    </Card>
}