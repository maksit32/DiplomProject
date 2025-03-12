import React from 'react';
import 'bootstrap/dist/css/bootstrap.min.css';
import { redirectAndRemoveToken } from '../data/Functions';
import { useNavigate } from 'react-router-dom';
import '../styles/header.css';

const Header: React.FC = () => {
    const navigate = useNavigate();

    const handleLogout = () => {
        if (window.confirm("Вы уверены, что хотите выйти?")) {
            redirectAndRemoveToken(navigate);
        }
    };

    return (
        <header className="bg-success text-white py-4 fixed-top">
            <div className="container d-flex justify-content-between align-items-center">
                <h4 className="mb-0 font-weight-bold">Добро пожаловать в СНО и СМУ!</h4>
                <button
                    className="btn btn-primary custom-btn"
                    onClick={handleLogout}
                >
                    Выйти из аккаунта
                </button>
            </div>
        </header>
    );
};

export default Header;
