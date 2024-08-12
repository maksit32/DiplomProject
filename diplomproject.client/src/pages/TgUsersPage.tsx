import { useNavigate } from "react-router-dom";
import { Navbar } from "../components/NavBar"



export function TgUsersPage() {
    const navigate = useNavigate();
    // checkAndRemoveToken(navigate);


    return (
        <>
            <Navbar />
            <p>TgUsers</p>
        </>
    )
}