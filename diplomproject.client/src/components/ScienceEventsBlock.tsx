import { useEffect, useState } from "react";
import { getAllScienceEventsPath, getActualScienceEventsPath, deleteScienceEventPath, updateScienceEventPath, addScienceEventPath } from "../data/APIPaths";
import "../styles/scienceEventsBlock.css";
import ScEventModal from "./ScienceEventModal";
import axios from "axios";
import Select from '@mui/material/Select';
import MenuItem from '@mui/material/MenuItem';
import InputLabel from '@mui/material/InputLabel';
import FormControl from '@mui/material/FormControl';
import IconButton from '@mui/material/IconButton';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import { blue, red } from '@mui/material/colors';
import { useAppSelector } from "../store/store";

function ScienceEventsBlock() {
    const [choice, setChoice] = useState(1);
    const [scienceEvents, setScienceEvents] = useState([]);
    const [editingSEvent, setEditingSEvent] = useState(null);
    const [isModalOpen, setIsModalOpen] = useState(false);

    const phoneNumber = useAppSelector(state => state.user.phoneNumber)

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

        axios.get(url)
            .then(response => {
                setScienceEvents(response.data);
            })
            .catch(error => {
                console.error("Ошибка при получении данных:", error);
            });
    };

    useEffect(() => {
        fetchScEvents();
    }, [choice]);

    const handleChange = (event) => {
        setChoice(event.target.value);
    };

    const handleDelete = (id) => {
        if (window.confirm("Вы уверены, что хотите удалить это событие?")) {
            axios.delete(`${deleteScienceEventPath}?id=${id}`)
                .then(response => {
                    if (response.status === 200) {
                        setScienceEvents(scienceEvents.filter(scienceEvent => scienceEvent.id !== id));
                    } else {
                        console.error("Ошибка при удалении мероприятия.");
                    }
                })
                .catch(error => console.error("Ошибка при удалении мероприятия:", error));
        }
    };

    const handleEditClick = (sEvent) => {
        setEditingSEvent(sEvent);
        setIsModalOpen(true); // Открываем модальное окно
    };

    const handleAddSEvent = () => {
        const now = new Date();
        setEditingSEvent({
            nameEvent: '',
            dateEvent: new Date(Date.UTC(
                now.getUTCFullYear(),
                now.getUTCMonth(),
                now.getUTCDate(),
                12, 0, 0, 0 // Установить время на 12:00:00.000 (полдень)
            )).toISOString(),
            placeEvent: '',
            requirementsEvent: '',
            informationEvent: '',
            adminPhoneNumber: phoneNumber
        });
        setIsModalOpen(true); // Открываем модальное окно
    };

    const handleInputChange = (event) => {
        const { name, value } = event.target;
        setEditingSEvent({ ...editingSEvent, [name]: value });
    };

    const handleSave = () => {
        const url = editingSEvent.id ? updateScienceEventPath : addScienceEventPath;
        const requestMethod = editingSEvent.id ? 'put' : 'post';

        axios({
            method: requestMethod,
            url: url,
            data: editingSEvent,
            headers: {
                "Content-Type": "application/json",
            }
        })
            .then(response => {
                if (response.status === 200 || response.status === 201 || response.status === 204) {
                    fetchScEvents();
                    console.log("fetched!");
                }
                setIsModalOpen(false); // Закрытие модального окна после сохранения
            })
            .catch(error => console.error("Возникла ошибка при отправке данных:", error));
    };

    return (
        <>
            <div><h2>Выберите действие: </h2></div>
            <div className="options-container">
                <FormControl fullWidth>
                    <InputLabel id="science-select-label">Список</InputLabel>
                    <Select
                        labelId="science-select-label"
                        id="science-select"
                        value={choice}
                        label="Действие"
                        onChange={handleChange}
                    >
                        <MenuItem value={1}>Активные мероприятия</MenuItem>
                        <MenuItem value={2}>Все мероприятия</MenuItem>
                    </Select>
                </FormControl>
            </div>
            <div>
                <button
                    id="addSEventBtn"
                    className="btn btn-success"
                    onClick={handleAddSEvent}>Добавить мероприятие</button>
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
                                    <IconButton
                                        sx={{ color: blue[500] }}
                                        onClick={() => handleEditClick(sEvent)}
                                    >
                                        <EditIcon />
                                    </IconButton>
                                    <IconButton
                                        sx={{ color: red[500] }}
                                        onClick={() => handleDelete(sEvent.id)}
                                    >
                                        <DeleteIcon />
                                    </IconButton>
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
                isOpen={isModalOpen}
                setIsModalOpen={setIsModalOpen}
            />
        </>
    );
}

export default ScienceEventsBlock;
