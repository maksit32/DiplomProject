import { useEffect } from "react";

export function CheckJwt(navigate: any) {
    //проверка на jwt
    useEffect(() => {
        const token = localStorage.getItem("jwtToken");
        if (token) {
            navigate("/documents");
        }
    }, [navigate]);

    // Условный рендеринг
    const token = localStorage.getItem("jwtToken");
    if (token) {
        return null;
    }
}

function isTokenExpired(token: any) {
    const decodedToken = JSON.parse(atob(token.split('.')[1]));
    //время получает от сервера
    const expirationTime = decodedToken.exp * 1000;
    return expirationTime < Date.now();
}

export function checkAndRemoveToken(navigate: any) {
    const token = localStorage.getItem('jwtToken');
    if (!token || isTokenExpired(token)) {
        localStorage.removeItem('jwtToken');
        alert("Your session has expired. Please log in again.");
        navigate("/");
    }
}