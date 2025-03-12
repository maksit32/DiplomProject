import { useNavigate } from "react-router-dom";
import { Navbar } from "../components/NavBar";
import { NotifyBlock } from "../components/NotifyBlock";
import { useEffect } from "react";
import Header from "../components/Header";



export function NotifyPage() {
    const navigate = useNavigate();
    // checkAndRemoveToken(navigate);

    useEffect(() => {
        document.title = "MSTUCA Notify users";
    }, []);

    return (
        <>
            <Header />
            <Navbar />
            <NotifyBlock />
        </>
    )
}