import { useNavigate } from "react-router-dom";
import { Navbar } from "../components/NavBar"
import ScienceEventsBlock from "../components/ScienceEventsBlock";
import { useEffect } from "react";
import Header from "../components/Header";


export function ScienceEventPage() {
    const navigate = useNavigate();
    // checkAndRemoveToken(navigate);

    useEffect(() => {
        document.title = "MSTUCA Admin created events";
    }, []);

    return (
        <>
            <Header />
            <Navbar />
            <ScienceEventsBlock />
        </>
    )
}