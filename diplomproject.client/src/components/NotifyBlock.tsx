import { useState } from "react";



export function NotifyBlock() {
    const [notifyChoice, SetChoice] = useState(0);

    return (
        <>
            <div><h2>Введите сообщение в поле ниже и выберите кого вы хотите оповестить</h2></div>
        </>
    );
}