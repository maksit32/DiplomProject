import { useEffect, useState } from "react";
import { getAllScienceEventsPath, getActualScienceEventsPath, deleteScienceEventPath, updateScienceEventPath } from "../data/APIPaths";
import "../styles/scienceEventsBlock.css";
import ScEventModal from "./ScienceEventModal";

function ScienceEventsBlock() {
    const [choice, setChoice] = useState(1);
    const [scienceEvents, setScienceEvents] = useState([]);
    const [editingSEvent, setEditingSEvent] = useState(null);


    const fetchScEvents = () => {
        let url;
        switch (choice) {
            case 1:
                url = getActualScienceEventsPath;
                break;
            case 2:
                url = getAllScienceEventsPath;
                break;
            default:
                return;
        }

        fetch(url)
            .then(response => response.json())
            .then(data => setScienceEvents(data))
            .catch(error => console.error("Ошибка при получении данных:", error));
    };

    useEffect(() => {
        fetchScEvents();
    }, [choice]);

    const handleChange = (event) => {
        setChoice(parseInt(event.target.value));
    };

    const handleDelete = (id) => {
        if (window.confirm("Вы уверены, что хотите удалить это событие?")) {
            fetch(`${deleteScienceEventPath}?id=${id}`, {
                method: "DELETE",
            })
                .then(response => {
                    if (response.ok) {
                        setScienceEvents(scienceEvents.filter(scienceEvent => scienceEvent.id !== id));
                    } else {
                        console.error("Ошибка при удалении мероприятия.");
                    }
                })
                .catch(error => console.error("Ошибка при мероприятия:", error));
        }
    };

    const handleEditClick = (sEvent) => {
        console.log(sEvent);
        setEditingSEvent(sEvent);
    };

    const handleInputChange = (sEvent) => {
        const { name, value } = sEvent.target;
        setEditingSEvent({ ...editingSEvent, [name]: value });
    };

    const handleSave = () => {
        console.log(JSON.stringify(editingSEvent));

        fetch(updateScienceEventPath, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(editingSEvent),
        })
            .then(response => {
                if (response.ok) {
                    console.log("ok");
                    fetchScEvents();
                }
                setEditingSEvent(null); // Закрытие модального окна после сохранения
            })
            .catch(error => console.error("Ошибка при обновлении мероприятия:", error));
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
                        Список активных мероприятий
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
                        Список всех мероприятий
                    </label>
                </div>
            </div>
            <div>
                <button
                    id="addSEventBtn"
                    className="btn btn-success"
                    onClick={() => handleAddSEvent()}>Добавить мероприятие</button>
            </div>
            <div className="table-responsive">
                <table className="table table-striped table-bordered">
                    <thead>
                        <tr className="table-primary">
                            <th>Название мероприятия</th>
                            <th>Дата проведения</th>
                            <th>Место</th>
                            <th>Требования</th>
                            <th>Информация</th>
                            <th>Дата создания мероприятия</th>
                            <th>Добавлен id</th>
                            <th>Действия</th>
                        </tr>
                    </thead>
                    <tbody>
                        {scienceEvents.map((sEvent) => (
                            <tr key={sEvent.id}>
                                <td>{sEvent.nameEvent}</td>
                                <td>{new Date(sEvent.dateEvent).toLocaleString()}</td>
                                <td>{sEvent.placeEvent}</td>
                                <td>{sEvent.requirementsEvent}</td>
                                <td>{sEvent.informationEvent}</td>
                                <td>{new Date(sEvent.dateEventCreated).toLocaleString()}</td>
                                <td>{sEvent.addByAdminChatId}</td>
                                <td>
                                    <button
                                        className="btn btn-primary"
                                        onClick={() => handleEditClick(sEvent)}
                                    >
                                        Изменить
                                    </button>
                                    <button
                                        id="delete-btn"
                                        className="btn btn-danger"
                                        onClick={() => handleDelete(sEvent.id)}
                                    >
                                        Удалить
                                    </button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
            <ScEventModal
                editingSEvent={editingSEvent}
                handleInputChange={handleInputChange}
                handleSave={handleSave}
                setEditingSEvent={setEditingSEvent} />
        </>
    );
}

export default ScienceEventsBlock;
