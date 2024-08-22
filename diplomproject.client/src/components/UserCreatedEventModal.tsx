

function UserCreatedEventModal({ editingUserCreatedEvent, handleInputChange, handleSave, setEditingUserCreatedEvent }) {
    return (
        <>
            {editingUserCreatedEvent && (
                <div className="modal show d-block" tabIndex="-1" role="dialog">
                    <div className="modal-dialog" role="document">
                        <div className="modal-content">
                            <div className="modal-header">
                                <h5 className="modal-title">Редактировать мероприятие</h5>
                                <button type="button" className="btn-close-custom" aria-label="Close" onClick={() => setEditingUserCreatedEvent(null)}>
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
                                            value={editingUserCreatedEvent.nameEvent}
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
                                            value={editingUserCreatedEvent.dateEvent}
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
                                            value={editingUserCreatedEvent.placeEvent}
                                            onChange={handleInputChange}
                                        />
                                    </div>
                                    <div className="form-group form-check">
                                        <input
                                            type="checkbox"
                                            className="form-check-input"
                                            id="isSubscribed"
                                            name="isSubscribed"
                                            checked={editingUserCreatedEvent.isWinner}
                                            onChange={() => setEditingUserCreatedEvent({ ...editingUserCreatedEvent, isWinner: !editingUserCreatedEvent.isWinner })}
                                        />
                                        <label className="form-check-label" htmlFor="isSubscribed">
                                            Победитель?
                                        </label>
                                    </div>
                                </form>
                            </div>
                            <div className="modal-footer">
                                <button type="button" className="btn btn-outline-primary" onClick={handleSave}>
                                    Сохранить
                                </button>
                                <button type="button" className="btn btn-outline-danger" onClick={() => setEditingUserCreatedEvent(null)}>
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

export default UserCreatedEventModal;