import axios from "axios";
import { useEffect, useState } from "react";
import { UserLogin } from "../context/LoginData";
import { loginPath } from "../data/APIPaths";
import { loginImagePath } from "../data/ImagesPath";
import { useNavigate } from "react-router-dom";
import { CheckJwt } from "../data/Functions";
import "../styles/loginBlock.css"
import { useDispatch, useSelector } from "react-redux";
import { setPhoneNumber } from "../store/userReducer";

export function LoginBlock() {
    const [loginData, setLoginData] = useState(new UserLogin({ phoneNumber: "", password: "" }));
    const [errorMessage, setError] = useState("");
    const navigate = useNavigate();
    const dispatch = useDispatch();

    // //проверка на jwt
    // CheckJwt(navigate);


    const handleInputChange = (e: any) => {
        const { name, value } = e.target;
        setLoginData(prevState => new UserLogin({
            ...prevState,
            [name]: value
        }));
    };

    const handleSubmit = async (e: any) => {
        e.preventDefault();
        try {
            const response = await axios.post(loginPath, loginData);
            console.log(response.data);
            const token = response.data.token;

            if (token) {
                localStorage.setItem("jwtToken", token);
                console.log("Токен сохранен:", token);
                dispatch(setPhoneNumber(loginData.phoneNumber));
                navigate("/documents");
            }
        } catch (error) {
            console.error("Ошибка авторизации", error);
            setError("Не удалось авторизоваться. Проверьте свои данные.");
        }
    };

    return (
        <>
            <div className="telegramContainer">
                <img id="loginImage" src={loginImagePath} alt="Telegram" />
            </div>
            <div className="login-form">
                <div className="container mt-5">
                    <div className="row">
                        <div className="col-md-6 offset-md-3">
                            <div className="card">
                                <div className="card-body">
                                    <h3 className="card-title text-center">Вход в систему</h3>
                                    <form onSubmit={handleSubmit}>
                                        <div className="form-group">
                                            <label htmlFor="inputPhone">Номер телефона</label>
                                            <input
                                                type="tel"
                                                className="form-control"
                                                id="inputPhone"
                                                placeholder="Введите номер телефона"
                                                name="phoneNumber"
                                                value={loginData.phoneNumber}
                                                onChange={handleInputChange}
                                            />
                                        </div>
                                        <div className="form-group">
                                            <label htmlFor="inputPassword">Ваш пароль</label>
                                            <input
                                                type="password"
                                                className="form-control"
                                                id="inputPassword"
                                                placeholder="Пароль"
                                                name="password"
                                                value={loginData.password}
                                                onChange={handleInputChange}
                                            />
                                        </div>
                                        <div className="btn-center">
                                            <button
                                                type="submit"
                                                className="btn btn-primary btn-block"
                                                onClick={handleSubmit}
                                            >
                                                Войти
                                            </button>
                                        </div>
                                    </form>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            {errorMessage &&
                <div className="alert alert-danger text-center">
                    {errorMessage}
                </div>
            }
        </>
    );
}
