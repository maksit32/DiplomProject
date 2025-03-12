import { useNavigate } from "react-router-dom";
import { Navbar } from "../components/NavBar"
import { DocumentBlock } from "../components/DocumentBlock";
import { useEffect } from "react";
import Header from "../components/Header";
import("bootstrap/dist/css/bootstrap.min.css");


export function DocumentPage() {
    const navigate = useNavigate();
    // checkAndRemoveToken(navigate);

    useEffect(() => {
        document.title = "MSTUCA Documents";
    }, []);

    return (
        <>
            <Header />
            <Navbar />
            <DocumentBlock />
        </>
    )
}