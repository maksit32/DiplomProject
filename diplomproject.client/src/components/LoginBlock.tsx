import { useDispatch } from "react-redux";
import { useAppSelector } from "../store/store";
import axios from "axios";
import { useState } from "react";
import { setLogged } from "../store/userReducer";
import "../styles/loginBlock.css"
import { LoginData, UserLogin } from "../context/LoginData";


export function LoginBlock() {
    const dispatch = useDispatch();
    const { isLogged } = useAppSelector(state => state.user);
    const [loginData, setLoginData] = useState<LoginData>({
        phoneNumber: '',
        password: ''
    });
    const [errorMessage, setErrorMessage] = useState<string | null>(null);


    //key --> value
    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setLoginData({ ...loginData, [name]: value });
    };

    const handleSubmit = async (event: any) => {
        event.preventDefault();
        try {
            const user = new UserLogin(loginData);
            console.log(user);
            const response = await axios.post('https://localhost:7186/api/accounts/authentication', user);
            if (response.status === 200) {
                console.log("Успешно");
                setErrorMessage(null);
                //вызываем диспетчера, передаем метод редюсера
                dispatch(setLogged(true));
            }
        } catch (error) {
            console.error('Ошибка при отправке данных:', error);
            setErrorMessage("Ошибка! Номер телефона или пароль неверны!");
        }
    };

    //обработчик наведения мыши на кнопку
    function handleMouseOver(event: any) {
        event.currentTarget.className = 'btn btn-danger btn-block';
    }

    function handleMouseOut(event: any) {
        event.currentTarget.className = 'btn btn-primary btn-block';
    }

    return (
        <>
            {isLogged === true ? (
                <div className="text-div">
                    <h3 style={{ color: "blue" }}>Вы успешно вошли в систему!</h3>
                    {errorMessage &&
                        <div className="alert alert-danger" role="alert" style={{ textAlign: "center" }}>
                            {errorMessage}
                        </div>
                    }
                </div>
            ) : (
                <>
                    <h4 style={{ textAlign: 'center', color: "blue" }}>Вход возможен только администраторам телеграм бота!</h4>
                    <div className="planeContainer">
                        <img src="https://lh5.googleusercontent.com/proxy/MTd0bQEY3aN0KgLNS0Da_29gMx_6qQs0NJh6v0-KZ3wEriGty__rULv_iyZFowmGX3yFs2rmaoa7AJMkQy488cNvSqUaq6qvxprgKQdadIX-whbFrV6djiPh7Gw8" alt="Plane" />
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
                                                        onMouseOver={handleMouseOver}
                                                        onMouseOut={handleMouseOut}
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
                        <div className="alert alert-danger" role="alert" style={{ textAlign: "center" }}>
                            {errorMessage}
                        </div>
                    }
                </>
            )}
        </>
    )
}