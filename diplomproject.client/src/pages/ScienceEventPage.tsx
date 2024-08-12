import { useNavigate } from "react-router-dom";
import { Navbar } from "../components/NavBar"



export function ScienceEventPage() {
    const navigate = useNavigate();
    // checkAndRemoveToken(navigate);


    return (
        <>
            <Navbar />
            <p>ScienceEvent</p>
        </>
    )
}