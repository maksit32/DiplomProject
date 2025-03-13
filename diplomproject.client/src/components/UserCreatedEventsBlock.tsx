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
import { checkAndRemoveToken, isTokenExpired, redirectAndRemoveToken } from "../data/Functions";

function UserCreatedEventBlock() {
    const [userCreatedEvents, setUserCreatedEvents] = useState([]);
    const [editingUserCreatedEvent, setEditingUserCreatedEvent] = useState(null);
    const [sortOrder, setSortOrder] = useState('asc');
    const [sortCriterion, setSortCriterion] = useState('name');

    const navigate = useNavigate();

    const fetchUsers = () => {
        const token = sessionStorage.getItem("jwtToken");
        if (!token || isTokenExpired(token)) {
            redirectAndRemoveToken(navigate);
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
            const token = sessionStorage.getItem("jwtToken");
            if (!token || isTokenExpired(token)) {
                redirectAndRemoveToken(navigate);
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
        const token = sessionStorage.getItem("jwtToken");
        if (!token || isTokenExpired(token)) {
            redirectAndRemoveToken(navigate);
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

    const sortedEvents = [...userCreatedEvents].sort((a, b) => {
        let comparison = 0;

        if (sortCriterion === 'name') {
            comparison = a.nameEvent.localeCompare(b.nameEvent);
        } else if (sortCriterion === 'city') {
            comparison = a.placeEvent.localeCompare(b.placeEvent);
        } else if (sortCriterion === 'date') {
            comparison = new Date(a.dateEvent) - new Date(b.dateEvent);
        }

        return sortOrder === 'asc' ? comparison : -comparison;
    });

    const toggleSortOrder = () => {
        setSortOrder(sortOrder === 'asc' ? 'desc' : 'asc');
    };

    const handleSortCriterionChange = (event) => {
        setSortCriterion(event.target.value);
    };

    return (
        <>
            <div className="sort-controls mb-4 d-flex align-items-center">
                <label className="me-3" htmlFor="sortCriterion">
                    Сортировать по:
                </label>
                <select
                    id="sortCriterion"
                    className="form-select me-3"
                    value={sortCriterion}
                    onChange={handleSortCriterionChange}
                >
                    <option value="name">Название</option>
                    <option value="city">Город</option>
                    <option value="date">Дата</option>
                </select>
                <button
                    className={`btn ${sortOrder === 'asc' ? 'btn-success' : 'btn-primary'}`}
                    onClick={toggleSortOrder}
                >
                    Порядок: {sortOrder === 'asc' ? 'Возрастающий' : 'Убывающий'}
                </button>
            </div>

            <div className="table-responsive">
                <table className="table table-striped table-bordered">
                    <thead>
                        <tr className="table-primary">
                            <th>
                                Название мероприятия
                            </th>
                            <th>
                                Место проведения
                            </th>
                            <th>
                                Дата проведения
                            </th>
                            <th>Статус победителя</th>
                            <th>Id чата телеграм</th>
                            <th>Действия</th>
                        </tr>
                    </thead>
                    <tbody>
                        {sortedEvents.map((uCreatedEvent) => (
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
