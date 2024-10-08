import { useEffect, useState } from "react";
import { deleteUserCreatedEventPath, getAllUserCreatedEventsPath, updateUserCreatedEventsPath } from "../data/APIPaths";
import "../styles/userCreatedEventBlock.css";
import UserCreatedEventModal from "./UserCreatedEventModal";
import axios from "axios";
import IconButton from '@mui/material/IconButton';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import { blue, red } from '@mui/material/colors';
import { useNavigate } from "react-router-dom";
import { checkAndRemoveToken, isTokenExpired } from "../data/Functions";

function UserCreatedEventBlock() {
    const [userCreatedEvents, setUserCreatedEvents] = useState([]);
    const [editingUserCreatedEvent, setEditingUserCreatedEvent] = useState(null);
    const navigate = useNavigate();

    const fetchUsers = () => {
        const token = localStorage.getItem("jwtToken");
        if (!token || isTokenExpired(token)) {
            checkAndRemoveToken(navigate);
            return;
        }

        axios.get(getAllUserCreatedEventsPath, {
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`,
            }
        })
            .then(response => {
                setUserCreatedEvents(response.data);
            })
            .catch(error => {
                console.error("Ошибка при получении данных:", error);
            });
    };

    useEffect(() => {
        fetchUsers();
    }, []);

    const handleDelete = (id) => {
        if (window.confirm("Вы уверены, что хотите удалить это мероприятие?")) {
            const token = localStorage.getItem("jwtToken");
            if (!token || isTokenExpired(token)) {
                checkAndRemoveToken(navigate);
                return;
            }

            axios.delete(`${deleteUserCreatedEventPath}?id=${id}`, {
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`,
                }
            })
                .then(response => {
                    if (response.status === 200) {
                        setUserCreatedEvents(userCreatedEvents.filter(uCreatedEvent => uCreatedEvent.id !== id));
                    } else {
                        console.error("Ошибка при удалении мероприятия.");
                    }
                })
                .catch(error => console.error("Ошибка при удалении мероприятия:", error));
        }
    };

    const handleEditClick = (uCreatedEvent) => {
        setEditingUserCreatedEvent(uCreatedEvent);
    };

    const handleInputChange = (event) => {
        const { name, value } = event.target;
        setEditingUserCreatedEvent({ ...editingUserCreatedEvent, [name]: value });
    };

    const handleSave = () => {
        const token = localStorage.getItem("jwtToken");
        if (!token || isTokenExpired(token)) {
            checkAndRemoveToken(navigate);
            return;
        }

        axios.put(updateUserCreatedEventsPath, editingUserCreatedEvent, {
            headers: {
                "Content-Type": "application/json",
                'Authorization': `Bearer ${token}`,
            }
        })
            .then(response => {
                if (response.status === 200) {
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
                                    <IconButton
                                        sx={{ color: blue[500] }}
                                        onClick={() => handleEditClick(uCreatedEvent)}
                                    >
                                        <EditIcon />
                                    </IconButton>
                                    <IconButton
                                        sx={{ color: red[500] }}
                                        onClick={() => handleDelete(uCreatedEvent.id)}
                                    >
                                        <DeleteIcon />
                                    </IconButton>
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
