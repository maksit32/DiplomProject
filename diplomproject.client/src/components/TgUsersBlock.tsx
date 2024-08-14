import { useEffect, useState } from "react";
import { getAdmTgUsersPath, getAllTgUsersPath, getSubTgUsersPath, deleteTgUserPath, updateTgUserPath } from "../data/APIPaths";
import "../styles/tgUsersBlock.css";
import TgUserModal from "./TgUserModal";

function TgUsersBlock() {
    const [choice, setChoice] = useState(1);
    const [users, setUsers] = useState([]);
    const [editingUser, setEditingUser] = useState(null);


    const fetchUsers = () => {
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

        fetch(url)
            .then(response => response.json())
            .then(data => setUsers(data))
            .catch(error => console.error("Ошибка при получении данных:", error));
    };

    useEffect(() => {
        fetchUsers();
    }, [choice]);

    const handleChange = (event) => {
        setChoice(parseInt(event.target.value));
    };

    const handleDelete = (id) => {
        if (window.confirm("Вы уверены, что хотите удалить этого пользователя?")) {
            fetch(`${deleteTgUserPath}?id=${id}`, {
                method: "DELETE",
            })
                .then(response => {
                    if (response.ok) {
                        setUsers(users.filter(user => user.id !== id));
                    } else {
                        console.error("Ошибка при удалении пользователя.");
                    }
                })
                .catch(error => console.error("Ошибка при удалении пользователя:", error));
        }
    };

    const handleEditClick = (user) => {
        console.log(user);
        setEditingUser(user);
    };

    const handleInputChange = (event) => {
        const { name, value } = event.target;
        setEditingUser({ ...editingUser, [name]: value });
    };

    const handleSave = () => {
        console.log(JSON.stringify(editingUser));

        fetch(updateTgUserPath, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(editingUser),
        })
            .then(response => {
                if (response.ok) {
                    console.log("ok");
                    fetchUsers();
                }
                setEditingUser(null); // Закрытие модального окна после сохранения
            })
            .catch(error => console.error("Ошибка при обновлении пользователя:", error));
    };


    return (
        <>
            <div><h2>Выберите действие: </h2></div>
            <div className="options-container">
                <div className="form-check">
                    <input
                        className="form-check-input"
                        type="radio"
                        id="option1"
                        name="options"
                        value="1"
                        checked={choice === 1}
                        onChange={handleChange}
                    />
                    <label className="form-check-label" htmlFor="option1">
                        Список всех пользователей
                    </label>
                </div>
                <div className="form-check">
                    <input
                        className="form-check-input"
                        type="radio"
                        id="option2"
                        name="options"
                        value="2"
                        checked={choice === 2}
                        onChange={handleChange}
                    />
                    <label className="form-check-label" htmlFor="option2">
                        Список администраторов
                    </label>
                </div>
                <div className="form-check">
                    <input
                        className="form-check-input"
                        type="radio"
                        id="option3"
                        name="options"
                        value="3"
                        checked={choice === 3}
                        onChange={handleChange}
                    />
                    <label className="form-check-label" htmlFor="option3">
                        Список подписчиков
                    </label>
                </div>
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
                                    <button
                                        className="btn btn-primary"
                                        onClick={() => handleEditClick(user)}
                                    >
                                        Изменить
                                    </button>
                                    <button
                                        id="delete-btn"
                                        className="btn btn-danger"
                                        onClick={() => handleDelete(user.id)}
                                    >
                                        Удалить
                                    </button>
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
