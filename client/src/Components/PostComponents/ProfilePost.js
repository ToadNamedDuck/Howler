import { Link } from "react-router-dom";
import { Card, CardBody, CardTitle } from "reactstrap";

export default function ProfilePost({ post }) {
    if (post === undefined) {
        return ""
    }
    return <Card>
        <CardTitle>
            <Link to={`/boards/${post.boardId}/posts/${post.id}`}><h3>{post.title}</h3></Link>
        </CardTitle>
        <CardBody>
            {
                post.content.length > 123 ?
                    post.content.substring(0, 123) + "..."
                    :
                    post.content
            }
        </CardBody>
    </Card>
}