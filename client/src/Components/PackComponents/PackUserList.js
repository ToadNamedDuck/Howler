import { useEffect, useState } from "react";
import { Card, CardBody, CardHeader, Spinner } from "reactstrap";
import { getPackMembers } from "../../Modules/packManager";
import UserPartial from "../UserComponents/UserPartial";

export default function PackUserList({packId}){
    const [members, setMembers] = useState(null);

    useEffect(() => {
        getPackMembers(packId).then(members => setMembers(members))
    },[])

    if(packId === null || packId === undefined || members === null){
        return <Spinner className="app-spinner dark"/>;
    }

    return <Card>
        <CardHeader>
            <h2>Pack Members</h2>
        </CardHeader>
        <CardBody>
            {
                members.map(member => <UserPartial userInfo={member} key={`pack-${packId}-member-${member.id}`}/>) 
            }
        </CardBody>
    </Card>
}