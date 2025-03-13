import React from 'react';
import 'bootstrap/dist/css/bootstrap.min.css';
import { redirectAndRemoveToken } from '../data/Functions';
import { useNavigate } from 'react-router-dom';
import '../styles/header.css';
import logo from '../images/mstuca.png';

const Header: React.FC = () => {
    const navigate = useNavigate();

    const handleLogout = () => {
        if (window.confirm("Вы уверены, что хотите выйти?")) {
            redirectAndRemoveToken(navigate);
        }
    };

    return (
        <header className="bg-primary text-white py-4 fixed-top">
            <div className="container d-flex justify-content-between align-items-center">
                <div className="d-flex align-items-center">
                    <div className="logo-background">
                        <img
                            src={logo}
                            alt="Логотип"
                            className="logo-image rounded-circle"
                        />
                    </div>
                    <h4 className="mb-0 font-weight-bold text-white">Добро пожаловать в СНО и СМУ!</h4>
                </div>
                <button
                    className="btn btn-warning custom-btn"
                    onClick={handleLogout}
                >
                    Выйти из аккаунта
                </button>
            </div>
        </header>
    );
};

export default Header;
