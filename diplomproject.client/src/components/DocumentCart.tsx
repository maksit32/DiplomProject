import { useState } from "react";
import "../styles/documentCart.css";

export function DocumentCart({ document, onDelete }) {
    const [hover, setHover] = useState(false);

    return (
        <div
            className="document-cart"
            onMouseEnter={() => setHover(true)}
            onMouseLeave={() => setHover(false)}
        >
            <img src={document.image} alt={document.name} className="document-image" />
            <p className="document-name">{document.name}</p>
            {hover && (
                <div className="document-actions">
                    <button
                        onClick={() => window.location.href = document.downloadLink}
                        className="btn download-btn"
                    >
                        Скачать
                    </button>
                    <button onClick={() => onDelete(document.id)} className="btn delete-btn">
                        Удалить
                    </button>
                </div>
            )}
        </div>
    );
}
