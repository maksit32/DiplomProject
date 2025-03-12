import { useNavigate } from "react-router-dom";
import { Navbar } from "../components/NavBar";
import { NotifyBlock } from "../components/NotifyBlock";
import { useEffect } from "react";



export function NotifyPage() {
    const navigate = useNavigate();
    // checkAndRemoveToken(navigate);

    useEffect(() => {
        document.title = "MSTUCA Notify users";
    }, []);

    return (
        <>
            <Navbar />
            <NotifyBlock />
        </>
    )
}