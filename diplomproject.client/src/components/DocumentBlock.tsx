import { useState, useEffect } from "react";
import axios from "axios";
import { DocumentCart } from "./DocumentCart";
import { Select } from "antd";
import "../styles/documentBlock.css";
import { SMUImagePath, SNOImagePath } from "../data/ImagesPath";
import {
    getSNOFilesPath,
    getSMUFilesPath,
    deleteSNOFilePath,
    deleteSMUFilePath
} from "../data/APIPaths";
import { useNavigate } from "react-router-dom";
import { checkAndRemoveToken, isTokenExpired } from "../data/Functions";

const { Option } = Select;

export function DocumentBlock() {
    const [documents, setDocuments] = useState([]);
    const [selectedType, setSelectedType] = useState("sno");
    const navigate = useNavigate();

    const fetchDocuments = async (path) => {
        try {
            const token = localStorage.getItem("jwtToken");
            if (!token || isTokenExpired(token)) {
                checkAndRemoveToken(navigate);
                return;
            }

            const response = await axios.get(path, {
                headers: {
                    Authorization: `Bearer ${token}`,
                },
            });
            const data = response.data;

            const imagePath = path === getSNOFilesPath ? SNOImagePath : SMUImagePath;

            const formattedDocuments = data.map(fileDto => ({
                id: fileDto.id,
                name: fileDto.fileName,
                image: imagePath,
                downloadLink: `${path}/${fileDto.fileName}`,
            }));
            setDocuments(formattedDocuments);
        } catch (error) {
            console.error("Ошибка при получении документов:", error);
        }
    };

    useEffect(() => {
        if (selectedType === "sno") {
            fetchDocuments(getSNOFilesPath);
        } else if (selectedType === "smu") {
            fetchDocuments(getSMUFilesPath);
        }
    }, [selectedType]);

    const handleDownload = async (doc) => {
        const token = localStorage.getItem("jwtToken");
        if (!token || isTokenExpired(token)) {
            checkAndRemoveToken(navigate);
            return;
        }

        try {
            const response = await axios({
                url: doc.downloadLink,
                method: 'GET',
                responseType: 'blob',
                headers: {
                    'Authorization': `Bearer ${token}`,
                }
            });

            const url = window.URL.createObjectURL(new Blob([response.data]));
            const link = window.document.createElement('a');
            link.href = url;
            link.setAttribute('download', doc.name);
            window.document.body.appendChild(link);
            link.click();
            window.document.body.removeChild(link);
        } catch (error) {
            console.error("Ошибка при скачивании документа:", error);
        }
    };


    const handleDelete = async (id) => {
        try {
            const token = localStorage.getItem("jwtToken");
            if (!token || isTokenExpired(token)) {
                checkAndRemoveToken(navigate);
                return;
            }

            const documentToDelete = documents.find(doc => doc.id === id);
            if (!documentToDelete) {
                console.error(`Документ с id ${id} не найден`);
                return;
            }

            const userConfirmed = window.confirm(`Вы уверены, что хотите удалить документ "${documentToDelete.name}"?`);
            if (!userConfirmed) {
                return;
            }

            const deletePath = selectedType === "sno" ? deleteSNOFilePath : deleteSMUFilePath;

            const response = await axios.delete(`${deletePath}`, {
                headers: {
                    Authorization: `Bearer ${token}`,
                },
                params: {
                    fileName: documentToDelete.name,
                },
            });

            if (response.status === 200) {
                setDocuments(documents.filter(doc => doc.id !== id));
            } else {
                console.error(`Не удалось удалить документ с id ${id}. Статус ответа: ${response.status}`);
            }
        } catch (error) {
            console.error("Ошибка при удалении документа:", error);
        }
    };

    const handleTypeChange = (value) => {
        setSelectedType(value);
    };

    return (
        <>
            <div className="select-div">
                <Select
                    defaultValue="sno"
                    style={{ width: 200, marginBottom: 20 }}
                    onChange={handleTypeChange}
                >
                    <Option value="sno">Документы СНО</Option>
                    <Option value="smu">Документы СМУ</Option>
                </Select>
            </div>
            <div className="document-block">
                {documents.map(document => (
                    <DocumentCart key={document.id} document={document} onDelete={handleDelete} onDownload={handleDownload} />
                ))}
            </div>
        </>
    );
}
