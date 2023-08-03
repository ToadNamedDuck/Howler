import { useEffect, useState } from "react";
import { Card, CardBody, CardHeader, Spinner } from "reactstrap";
import { getPackMembers } from "../../Modules/packManager";
import UserPartial from "../UserComponents/UserPartial";

export default function PackUserList({pack}){
    const [members, setMembers] = useState(null);

    useEffect(() => {
        getPackMembers(pack.id).then(members => setMembers(members))
    },[pack])

    if(pack === undefined || members === null){
        return <Spinner className="app-spinner dark"/>;
    }

    return <Card>
        <CardHeader>
            <h2>Pack Members</h2>
        </CardHeader>
        <CardBody>
            {
                members.map(member => <UserPartial userInfo={member} key={`pack-${pack.id}-member-${member.id}`}/>) 
            }
        </CardBody>
    </Card>
}