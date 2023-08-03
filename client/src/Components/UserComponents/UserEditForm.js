import { useEffect, useState } from "react";
import { editUser, getById } from "../../Modules/userManager";
import { Button, Card, CardFooter, CardHeader } from "reactstrap";

export default function UserEditForm({ loggedInUser, userUpdater, editStateUpdater, userId, userStateChange, userPack }) {
    const [newName, setName] = useState("");
    const [newPfp, setPfp] = useState("");
    const [leavePack, setLeave] = useState(false);

    useEffect(() => {
        getById(userId).then(user => {
            setName(user.displayName);
            setPfp(user.profilePictureUrl);
        })
    }, [])

    if (loggedInUser === null || loggedInUser === undefined || userUpdater === undefined || editStateUpdater === undefined || userId === undefined) {
        return <div></div>
    }

    if (newPfp === null) {
        setPfp(`https://robohash.org/${loggedInUser.displayName}?set=set4`)
    }

    function saveEditClick(e) {
        e.preventDefault();
        const userToSend = { ...loggedInUser }
        userToSend.displayName = newName;

        if (newPfp === "") {
            userToSend.profilePictureUrl = null
        }
        else {
            userToSend.profilePictureUrl = newPfp
        }
        if (leavePack) {
            userToSend.packId = null
        }

        if (!(userToSend.displayName.length >= 3)) {
            alert("Display name must be at least 3 characters long!")
        }
        else {
            editUser(userId, userToSend).then(() => userUpdater()).then(() => editStateUpdater(false)).then(() => userStateChange())
        }
    }

    return <Card>
        <CardHeader className="d-flex flex-column align-items-start">
            <input type="text" value={newName} placeholder="New Display Name..." onChange={(e) => { setName(e.target.value) }} style={{ width: "25%" }} />
            <input type="text" value={newPfp} placeholder="New Profile Picture URL..." onChange={(e) => { setPfp(e.target.value) }} style={{ width: "25%" }} />
            {
                userPack !== null && loggedInUser.packId !== null && userPack.packLeaderId !== loggedInUser.id ?
                    <>
                        <label htmlFor="leavePack" style={{ color: "var(--bs-warning)", fontSize: "18px" }}>Leave pack?</label>
                        <input type="checkbox" id="leavePack" value={leavePack} onChange={(e) => { setLeave(e.target.checked) }} style={{ marginBottom: "2%" }} />
                    </>
                    :
                    ""
            }
            {
                (newPfp !== "" || newPfp !== null) && newPfp && newPfp.includes("wikia.nocookie") ?
                    <img src={newPfp} crossOrigin="anonymous" referrerPolicy="no-referrer" alt="New Profile Picture" height="75px" width="75px" />
                    :
                    <img src={newPfp} width="75px" height="75px" alt="New Profile Picture" />
            }
        </CardHeader>
        <CardFooter className="d-flex flex-row">
            <Button color="info" onClick={(e) => { saveEditClick(e) }} style={{ width: "25%" }}>Save Changes</Button>
            <Button color="warning" onClick={() => { editStateUpdater(false) }} style={{ width: "25%" }}>Cancel Changes</Button>
        </CardFooter>
    </Card>
}