import { useNavigate } from "react-router-dom";
import { Navbar } from "../components/NavBar"
import TgUsersBlock from "../components/TgUsersBlock";



export function TgUsersPage() {
    const navigate = useNavigate();

    // checkAndRemoveToken(navigate);



    return (
        <>
            <Navbar />
            <TgUsersBlock />
        </>
    )
}