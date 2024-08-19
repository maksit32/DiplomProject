import { useNavigate } from "react-router-dom";
import { Navbar } from "../components/NavBar"
import { DocumentBlock } from "../components/DocumentBlock";
import("bootstrap/dist/css/bootstrap.min.css");


export function DocumentPage() {
    const navigate = useNavigate();
    // checkAndRemoveToken(navigate);

    return (
        <>
            <Navbar />
            <DocumentBlock />
        </>
    )
}