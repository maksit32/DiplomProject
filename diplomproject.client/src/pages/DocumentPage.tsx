import { useNavigate } from "react-router-dom";
import { Navbar } from "../components/NavBar"
import { checkAndRemoveToken } from "../data/Functions";



export function DocumentPage() {
    const navigate = useNavigate();
    checkAndRemoveToken(navigate);


    return (
        <>
            <Navbar />
            <p>Documents</p>
        </>
    )
}