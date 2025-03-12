import { useNavigate } from "react-router-dom";
import { Navbar } from "../components/NavBar"
import TgUsersBlock from "../components/TgUsersBlock";
import { useEffect } from "react";



export function TgUsersPage() {
    const navigate = useNavigate();

    // checkAndRemoveToken(navigate);

    useEffect(() => {
        document.title = "MSTUCA Telegram users";
    }, []);


    return (
        <>
            <Navbar />
            <TgUsersBlock />
        </>
    )
}