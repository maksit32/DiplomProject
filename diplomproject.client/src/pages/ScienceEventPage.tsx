import { useNavigate } from "react-router-dom";
import { Navbar } from "../components/NavBar"
import ScienceEventsBlock from "../components/ScienceEventsBlock";


export function ScienceEventPage() {
    const navigate = useNavigate();
    // checkAndRemoveToken(navigate);


    return (
        <>
            <Navbar />
            <ScienceEventsBlock />
        </>
    )
}