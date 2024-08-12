import { useNavigate } from "react-router-dom";
import { Navbar } from "../components/NavBar"



export function DocumentPage() {
    const navigate = useNavigate();
    // checkAndRemoveToken(navigate);


    return (
        <>
            <Navbar />
            <p>Documents</p>
        </>
    )
}