import { useEffect, useState } from "react";
import { deleteUserCreatedEventPath, getAllUserCreatedEventsPath, updateUserCreatedEventsPath } from "../data/APIPaths";
import "../styles/userCreatedEventBlock.css";
import UserCreatedEventModal from "./UserCreatedEventModal";


function UserCreatedEventBlock() {
    const [userCreatedEvents, setUserCreatedEvents] = useState([]);
    const [editingUserCreatedEvent, setEditingUserCreatedEvent] = useState(null);


    const fetchUsers = () => {
        fetch(getAllUserCreatedEventsPath)
            .then(response => response.json())
            .then(data => setUserCreatedEvents(data))
            .catch(error => console.error("Ошибка при получении данных:", error));
    };

    useEffect(() => {
        fetchUsers();
    }, []);


    const handleDelete = (id) => {
        if (window.confirm("Вы уверены, что хотите удалить это мероприятие?")) {
            fetch(`${deleteUserCreatedEventPath}?id=${id}`, {
                method: "DELETE",
            })
                .then(response => {
                    if (response.ok) {
                        setUserCreatedEvents(userCreatedEvents.filter(uCreatedEvent => uCreatedEvent.id !== id));
                    } else {
                        console.error("Ошибка при удалении мероприятия.");
                    }
                })
                .catch(error => console.error("Ошибка при удалении мероприятия:", error));
        }
    };

    const handleEditClick = (uCreatedEvent) => {
        console.log(uCreatedEvent);
        setEditingUserCreatedEvent(uCreatedEvent);
    };

    const handleInputChange = (event) => {
        const { name, value } = event.target;
        setEditingUserCreatedEvent({ ...editingUserCreatedEvent, [name]: value });
    };

    const handleSave = () => {
        console.log(JSON.stringify(editingUserCreatedEvent));

        fetch(updateUserCreatedEventsPath, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(editingUserCreatedEvent),
        })
            .then(response => {
                if (response.ok) {
                    console.log("ok");
                    fetchUsers();
                }
                setEditingUserCreatedEvent(null); // Закрытие модального окна после сохранения
            })
            .catch(error => console.error("Ошибка при обновлении мероприятия:", error));
    };


    return (
        <>
            <div className="table-responsive">
                <table className="table table-striped table-bordered">
                    <thead>
                        <tr className="table-primary">
                            <th>Название мероприятия</th>
                            <th>Место проведения</th>
                            <th>Дата проведения</th>
                            <th>Статус победителя</th>
                            <th>Id чата телеграм</th>
                            <th>Действия</th>
                        </tr>
                    </thead>
                    <tbody>
                        {userCreatedEvents.map((uCreatedEvent) => (
                            <tr key={uCreatedEvent.id}>
                                <td>{uCreatedEvent.nameEvent}</td>
                                <td>{uCreatedEvent.placeEvent}</td>
                                <td>{new Date(uCreatedEvent.dateEvent).toLocaleString()}</td>
                                <td>{uCreatedEvent.isWinner ? "Да" : "Нет"}</td>
                                <td>{uCreatedEvent.chatId}</td>
                                <td>
                                    <button
                                        className="btn btn-primary"
                                        onClick={() => handleEditClick(uCreatedEvent)}
                                    >
                                        Изменить
                                    </button>
                                    <button
                                        id="deleteSEv-btn"
                                        className="btn btn-danger"
                                        onClick={() => handleDelete(uCreatedEvent.id)}
                                    >
                                        Удалить
                                    </button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
            <UserCreatedEventModal
                editingUserCreatedEvent={editingUserCreatedEvent}
                handleInputChange={handleInputChange}
                handleSave={handleSave}
                setEditingUserCreatedEvent={setEditingUserCreatedEvent} />
        </>
    );
}

export default UserCreatedEventBlock;
