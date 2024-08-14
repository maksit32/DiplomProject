import { useNavigate } from "react-router-dom";
import { Navbar } from "../components/NavBar"
import UserCreatedEventBlock from "../components/UserCreatedEventsBlock";



export function UserCreatedEventPage() {
    const navigate = useNavigate();
    // checkAndRemoveToken(navigate);


    return (
        <>
            <Navbar />
            <UserCreatedEventBlock />
        </>
    )
}