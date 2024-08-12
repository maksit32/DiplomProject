import { useState } from "react";
import "../styles/notifyBlock.css"
import { Form } from 'react-bootstrap';
import { submitText } from "../data/Functions";
import { useNavigate } from "react-router-dom";


export function NotifyBlock() {
    const [notifyChoice, SetChoice] = useState(-1);
    const [messageToSend, SetSendMessage] = useState('');
    const navigate = useNavigate();

    const handleChange = (event: any) => {
        SetChoice(parseInt(event.target.value));
        console.log(event.target.value);
    };

    return (
        <div>
            <div><h2>Введите сообщение в поле ниже и выберите, кого вы хотите оповестить</h2></div>
            <div className="options-container">
                <div className="form-check">
                    <input
                        className="form-check-input"
                        type="radio"
                        id="option1"
                        name="options"
                        value="1"
                        checked={notifyChoice === 1}
                        onChange={handleChange}
                    />
                    <label className="form-check-label" htmlFor="option1">
                        Оповестить администраторов телеграм бота
                    </label>
                </div>
                <div className="form-check">
                    <input
                        className="form-check-input"
                        type="radio"
                        id="option2"
                        name="options"
                        value="2"
                        checked={notifyChoice === 2}
                        onChange={handleChange}
                    />
                    <label className="form-check-label" htmlFor="option2">
                        Оповестить подписчиков телеграм бота
                    </label>
                </div>
                <div className="form-check">
                    <input
                        className="form-check-input"
                        type="radio"
                        id="option3"
                        name="options"
                        value="3"
                        checked={notifyChoice === 3}
                        onChange={handleChange}
                    />
                    <label className="form-check-label" htmlFor="option3">
                        Оповестить всех пользователей телеграм бота
                    </label>
                </div>
            </div>
            <div className="text-container">
                <Form.Group controlId="formText">
                    <Form.Control
                        as="textarea"
                        rows={13}
                        placeholder="Ваш текст..."
                        className="custom-textarea"
                        onChange={e => SetSendMessage(e.target.value)}
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