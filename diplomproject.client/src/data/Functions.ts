import axios from "axios";
import { notifyAdminUsersPath, notifyAllUsersPath, notifySubUsersPath } from "./APIPaths";

export function CheckJwt(navigate: any) {
    //проверка на jwt
    const token = sessionStorage.getItem("jwtToken");
    if (token) {
        navigate("/documents");
    }
}

export function isTokenExpired(token: string | null): boolean {
    if (!token) return true;

    try {
        const decodedToken = JSON.parse(atob(token.split('.')[1]));
        const expirationTime = decodedToken.exp * 1000; // Преобразуем время в миллисекунды
        return expirationTime < Date.now(); // Проверяем, истек ли токен
    } catch (error) {
        console.error('Ошибка при декодировании токена:', error);
        return true;
    }
}

export function checkAndRemoveToken(navigate: any) {
    const token = sessionStorage.getItem('jwtToken');

    if (isTokenExpired(token)) {
        sessionStorage.removeItem('jwtToken');
        navigate("/");
    }
}

export function redirectAndRemoveToken(navigate: any) {
    sessionStorage.removeItem('jwtToken');
    navigate("/");
}

export async function submitText(notifyChoice: number, message: string, navigate: any) {
    const token = sessionStorage.getItem("jwtToken");
    if (!token || isTokenExpired(token)) {
        redirectAndRemoveToken(navigate);
        return;
    }

    const headers = {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
    };

    try {
        let response;
        if (notifyChoice === 1) {
            response = await axios.post(notifyAdminUsersPath, JSON.stringify(message), { headers });
        } else if (notifyChoice === 2) {
            response = await axios.post(notifySubUsersPath, JSON.stringify(message), { headers });
        } else if (notifyChoice === 3) {
            response = await axios.post(notifyAllUsersPath, JSON.stringify(message), { headers });
        }

        if (response.status === 200) {
            console.log("Notification sent successfully!");
        }
        else {
            console.error("Failed to send notification:", response.status, response.data);
        }
    } catch (error) {
        console.error("Error sending notification:", error);
    }
}