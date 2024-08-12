import { notifyAdminUsersPath, notifyAllUsersPath, notifySubUsersPath } from "./APIPaths";

export function CheckJwt(navigate: any) {
    //проверка на jwt
    const token = localStorage.getItem("jwtToken");
    if (token) {
        navigate("/documents");
    }
}

// function isTokenExpired(token: any) {
//     const decodedToken = JSON.parse(atob(token.split('.')[1]));
//     //время получает от сервера
//     const expirationTime = decodedToken.exp * 1000;
//     return expirationTime < Date.now();
// }

// export function checkAndRemoveToken(navigate: any) {
//     const token = localStorage.getItem('jwtToken');
//     if (!token || isTokenExpired(token)) {
//         localStorage.removeItem('jwtToken');
//         navigate("/");
//     }
// }

export function redirectAndRemoveToken(navigate: any) {
    localStorage.removeItem('jwtToken');
    navigate("/");
}

export async function submitText(notifyChoice: number, message: string, navigate: any) {
    const token = localStorage.getItem("jwtToken");

    try {
        let response;
        if (notifyChoice === 1) {
            response = await fetch(notifyAdminUsersPath, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify(message)
            });
        } else if (notifyChoice === 2) {
            response = await fetch(notifySubUsersPath, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify(message)
            });
        } else if (notifyChoice === 3) {
            response = await fetch(notifyAllUsersPath, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify(message)
            });
        }

        if (response.ok) {
            console.log("Notification sent successfully!");
        }
        else if (response.status === 401) {
            // redirectAndRemoveToken(navigate);
        }
        else {
            console.error("Failed to send notification:", response.status, errorMessage);
        }
    } catch (error) {
        console.error("Error sending notification:", error);
    }
}