import { useNavigate } from "react-router-dom";
import { Navbar } from "../components/NavBar";
import { NotifyBlock } from "../components/NotifyBlock";



export function NotifyPage() {
    const navigate = useNavigate();
    // checkAndRemoveToken(navigate);


    return (
        <>
            <Navbar />
            <NotifyBlock />
        </>
    )
}