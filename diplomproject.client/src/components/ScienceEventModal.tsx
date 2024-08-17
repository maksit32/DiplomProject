import React from "react";

function ScEventModal({ editingSEvent, handleInputChange, handleSave, setIsModalOpen, isOpen }) {
    const handleClose = () => {
        setIsModalOpen(false);
    };

    return (
        <>
            {isOpen && (
                <div className="modal show d-block" tabIndex="-1" role="dialog">
                    <div className="modal-dialog" role="document">
                        <div className="modal-content">
                            <div className="modal-header">
                                <h5 className="modal-title">Редактировать мероприятие</h5>
                                <button type="button" className="btn-close-custom" aria-label="Close" onClick={handleClose}>
                                    <span aria-hidden="true">&times;</span>
                                </button>
                            </div>
                            <div className="modal-body">
                                <form>
                                    <div className="form-group">
                                        <label htmlFor="nameEvent">Название мероприятия:</label>
                                        <input
                                            type="text"
                                            className="form-control"
                                            id="nameEvent"
                                            name="nameEvent"
                                            value={editingSEvent.nameEvent}
                                            onChange={handleInputChange}
                                        />
                                    </div>
                                    <div className="form-group">
                                        <label htmlFor="dateEvent">Дата проведения:</label>
                                        <input
                                            type="text"
                                            className="form-control"
                                            id="dateEvent"
                                            name="dateEvent"
                                            value={editingSEvent.dateEvent}
                                            onChange={handleInputChange}
                                        />
                                    </div>
                                    <div className="form-group">
                                        <label htmlFor="placeEvent">Место проведения:</label>
                                        <input
                                            type="text"
                                            className="form-control"
                                            id="placeEvent"
                                            name="placeEvent"
                                            value={editingSEvent.placeEvent}
                                            onChange={handleInputChange}
                                        />
                                    </div>
                                    <div className="form-group">
                                        <label htmlFor="requirementsEvent">Требования:</label>
                                        <input
                                            type="text"
                                            className="form-control"
                                            id="requirementsEvent"
                                            name="requirementsEvent"
                                            value={editingSEvent.requirementsEvent}
                                            onChange={handleInputChange}
                                        />
                                    </div>
                                    <div className="form-group">
                                        <label htmlFor="informationEvent">Дополнительная информация:</label>
                                        <input
                                            type="text"
                                            className="form-control"
                                            id="informationEvent"
                                            name="informationEvent"
                                            value={editingSEvent.informationEvent}
                                            onChange={handleInputChange}
                                        />
                                    </div>
                                </form>
                            </div>
                            <div className="modal-footer">
                                <button type="button" className="btn btn-outline-primary" onClick={handleSave}>
                                    Сохранить
                                </button>
                                <button type="button" className="btn btn-outline-danger" onClick={handleClose}>
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

export default ScEventModal;
