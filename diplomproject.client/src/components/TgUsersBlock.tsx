import { useEffect, useState } from "react";
import { getAdmTgUsersPath, getAllTgUsersPath, getSubTgUsersPath, deleteTgUserPath, updateTgUserPath } from "../data/APIPaths";
import "../styles/tgUsersBlock.css";
import TgUserModal from "./TgUserModal";
import axios from "axios";
import { Tag } from "antd";
import IconButton from '@mui/material/IconButton';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import { blue, red } from '@mui/material/colors';
import { useNavigate } from "react-router-dom";
import { checkAndRemoveToken, isTokenExpired } from "../data/Functions";


function TgUsersBlock() {
    const [choice, setChoice] = useState(1);
    const [users, setUsers] = useState([]);
    const [editingUser, setEditingUser] = useState(null);
    const navigate = useNavigate();

    const fetchUsers = () => {
        const token = localStorage.getItem("jwtToken");
        if (!token || isTokenExpired(token)) {
            checkAndRemoveToken(navigate);
            return;
        }

        let url;
        switch (choice) {
            case 1:
                url = getAllTgUsersPath;
                break;
            case 2:
                url = getAdmTgUsersPath;
                break;
            case 3:
                url = getSubTgUsersPath;
                break;
            default:
                return;
        }

        axios.get(url, {
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`,
            }
        })
            .then(response => {
                setUsers(response.data);
            })
            .catch(error => {
                console.error("Ошибка при получении данных:", error);
            });
    };

    useEffect(() => {
        fetchUsers();
    }, [choice]);

    const handleTagChange = (value) => {
        setChoice(value);
    };

    const handleDelete = (id) => {
        if (window.confirm("Вы уверены, что хотите удалить этого пользователя?")) {
            const token = localStorage.getItem("jwtToken");
            if (!token || isTokenExpired(token)) {
                checkAndRemoveToken(navigate);
                return;
            }

            axios.delete(`${deleteTgUserPath}?id=${id}`, {
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`,
                }
            })
                .then(response => {
                    if (response.status === 200) {
                        setUsers(users.filter(user => user.id !== id));
                    } else {
                        console.error("Ошибка при удалении пользователя.");
                    }
                })
                .catch(error => console.error("Ошибка при удалении пользователя:", error));
        }
    };

    const handleEditClick = (user) => {
        setEditingUser(user);
    };

    const handleInputChange = (event) => {
        const { name, value } = event.target;
        setEditingUser({ ...editingUser, [name]: value });
    };

    const handleSave = () => {
        const token = localStorage.getItem("jwtToken");
        if (!token || isTokenExpired(token)) {
            checkAndRemoveToken(navigate);
            return;
        }

        axios.put(updateTgUserPath, editingUser, {
            headers: {
                "Content-Type": "application/json",
                'Authorization': `Bearer ${token}`,
            }
        })
            .then(response => {
                if (response.status === 200) {
                    fetchUsers();
                }
                setEditingUser(null); // Закрытие модального окна после сохранения
            })
            .catch(error => console.error("Ошибка при обновлении пользователя:", error));
    };

    return (
        <>
            <div><h2>Выберите действие: </h2></div>
            <div>
                <Tag.CheckableTag
                    checked={choice === 1}
                    onChange={() => handleTagChange(1)}
                >
                    Список всех пользователей
                </Tag.CheckableTag>
                <Tag.CheckableTag
                    checked={choice === 2}
                    onChange={() => handleTagChange(2)}
                >
                    Список администраторов
                </Tag.CheckableTag>
                <Tag.CheckableTag
                    checked={choice === 3}
                    onChange={() => handleTagChange(3)}
                >
                    Список подписчиков
                </Tag.CheckableTag>
            </div>
            <div className="table-responsive">
                <table className="table table-striped table-bordered">
                    <thead>
                        <tr className="table-primary">
                            <th>Имя</th>
                            <th>Фамилия</th>
                            <th>Отчество</th>
                            <th>Номер телефона</th>
                            <th>Id чата телеграм</th>
                            <th>Подписка активна?</th>
                            <th>Права администратора</th>
                            <th>Последнее сообщение</th>
                            <th>Действия</th>
                        </tr>
                    </thead>
                    <tbody>
                        {users.map((user) => (
                            <tr key={user.id}>
                                <td>{user.name}</td>
                                <td>{user.surname}</td>
                                <td>{user.patronymic}</td>
                                <td>{user.phoneNumber}</td>
                                <td>{user.tgChatId}</td>
                                <td>{user.isSubscribed ? "Да" : "Нет"}</td>
                                <td>{user.isAdmin ? "Да" : "Нет"}</td>
                                <td>{new Date(user.lastMessageTime).toLocaleString()}</td>
                                <td>
                                    <IconButton
                                        sx={{ color: blue[500] }}
                                        onClick={() => handleEditClick(user)}
                                    >
                                        <EditIcon />
                                    </IconButton>
                                    <IconButton
                                        sx={{ color: red[500] }}
                                        onClick={() => handleDelete(user.id)}
                                    >
                                        <DeleteIcon />
                                    </IconButton>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
            <TgUserModal
                editingUser={editingUser}
                handleInputChange={handleInputChange}
                handleSave={handleSave}
                setEditingUser={setEditingUser} />
        </>
    );
}

export default TgUsersBlock;