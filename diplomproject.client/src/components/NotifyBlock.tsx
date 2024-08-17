import { useState } from "react";
import { Form } from 'react-bootstrap';
import { submitText } from "../data/Functions";
import { useNavigate } from "react-router-dom";
import Select from '@mui/material/Select';
import MenuItem from '@mui/material/MenuItem';
import InputLabel from '@mui/material/InputLabel';
import FormControl from '@mui/material/FormControl';
import "../styles/notifyBlock.css";

export function NotifyBlock() {
    const [notifyChoice, setChoice] = useState(-1);
    const [messageToSend, setSendMessage] = useState('');
    const navigate = useNavigate();

    const handleChange = (event: any) => {
        setChoice(event.target.value);
        console.log(event.target.value);
    };

    return (
        <div>
            <div><h2>Введите сообщение в поле ниже и выберите, кого вы хотите оповестить</h2></div>
            <div className="options-container">
                <FormControl fullWidth>
                    <InputLabel id="notify-select-label">Оповестить</InputLabel>
                    <Select
                        labelId="notify-select-label"
                        id="notify-select"
                        value={notifyChoice}
                        label="Оповестить"
                        onChange={handleChange}
                    >
                        <MenuItem value={1}>Администраторов телеграм бота</MenuItem>
                        <MenuItem value={2}>Подписчиков телеграм бота</MenuItem>
                        <MenuItem value={3}>Всех пользователей телеграм бота</MenuItem>
                    </Select>
                </FormControl>
            </div>
            <div className="text-container">
                <Form.Group controlId="formText">
                    <Form.Control
                        as="textarea"
                        rows={13}
                        placeholder="Ваш текст..."
                        className="custom-textarea"
                        onChange={e => setSendMessage(e.target.value)}
                    />
                </Form.Group>
            </div>
            <div>
                <button
                    type="button"
                    onClick={() => submitText(notifyChoice, messageToSend, navigate)}
                    className="btn btn-outline-success"
                >
                    Отправить пользователям
                </button>
            </div>
        </div>
    );
}
