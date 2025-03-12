import { useNavigate } from "react-router-dom";
import { Navbar } from "../components/NavBar"
import UserCreatedEventBlock from "../components/UserCreatedEventsBlock";
import { useEffect } from "react";



export function UserCreatedEventPage() {
    const navigate = useNavigate();
    // checkAndRemoveToken(navigate);

    useEffect(() => {
        document.title = "MSTUCA User created events";
    }, []);


    return (
        <>
            <Navbar />
            <UserCreatedEventBlock />
        </>
    )
}