import React from "react";
import "../styles/tgUserModal.css" // Подключаем стили

function TgUserModal({ editingUser, handleInputChange, handleSave, setEditingUser }) {
    return (
        <>
            {editingUser && (
                <div className="modal show d-block" tabIndex="-1" role="dialog">
                    <div className="modal-dialog" role="document">
                        <div className="modal-content">
                            <div className="modal-header">
                                <h5 className="modal-title">Редактировать пользователя</h5>
                                <button
                                    type="button"
                                    className="btn-close-custom"
                                    aria-label="Close"
                                    onClick={() => setEditingUser(null)}
                                >
                                    <span aria-hidden="true">&times;</span>
                                </button>
                            </div>
                            <div className="modal-body">
                                <form>
                                    <div className="form-group">
                                        <label htmlFor="name">Имя:</label>
                                        <input
                                            type="text"
                                            className="form-control"
                                            id="name"
                                            name="name"
                                            value={editingUser.name}
                                            onChange={handleInputChange}
                                        />
                                    </div>
                                    <div className="form-group">
                                        <label htmlFor="surname">Фамилия:</label>
                                        <input
                                            type="text"
                                            className="form-control"
                                            id="surname"
                                            name="surname"
                                            value={editingUser.surname}
                                            onChange={handleInputChange}
                                        />
                                    </div>
                                    <div className="form-group">
                                        <label htmlFor="patronymic">Отчество:</label>
                                        <input
                                            type="text"
                                            className="form-control"
                                            id="patronymic"
                                            name="patronymic"
                                            value={editingUser.patronymic}
                                            onChange={handleInputChange}
                                        />
                                    </div>
                                    <div className="form-group">
                                        <label htmlFor="phoneNumber">Номер телефона:</label>
                                        <input
                                            type="text"
                                            className="form-control"
                                            id="phoneNumber"
                                            name="phoneNumber"
                                            value={editingUser.phoneNumber}
                                            onChange={handleInputChange}
                                        />
                                    </div>
                                    <div className="form-group form-check">
                                        <input
                                            type="checkbox"
                                            className="form-check-input"
                                            id="isSubscribed"
                                            name="isSubscribed"
                                            checked={editingUser.isSubscribed}
                                            onChange={() => setEditingUser({ ...editingUser, isSubscribed: !editingUser.isSubscribed })}
                                        />
                                        <label className="form-check-label" htmlFor="isSubscribed">
                                            Подписка активна?
                                        </label>
                                    </div>
                                    <div className="form-group form-check">
                                        <input
                                            type="checkbox"
                                            className="form-check-input"
                                            id="isAdmin"
                                            name="isAdmin"
                                            checked={editingUser.isAdmin}
                                            onChange={() => setEditingUser({ ...editingUser, isAdmin: !editingUser.isAdmin })}
                                        />
                                        <label className="form-check-label" htmlFor="isAdmin">
                                            Права администратора
                                        </label>
                                    </div>
                                </form>
                            </div>
                            <div className="modal-footer">
                                <button type="button" className="btn btn-outline-primary" onClick={handleSave}>
                                    Сохранить
                                </button>
                                <button type="button" className="btn btn-outline-danger" onClick={() => setEditingUser(null)}>
                                    Отмена
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            )}
        </>
    );
}

export default TgUserModal;
