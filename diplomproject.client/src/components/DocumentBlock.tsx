import React, { useState, useEffect } from "react";
import axios from "axios";
import { DocumentCart } from "./DocumentCart";
import { Select } from "antd";  // Импортируем Select из Ant Design
import "../styles/documentBlock.css";
import { fileImagePath } from "../data/ImagesPath";
import {
    getSNOFilesPath,
    getSMUFilesPath,
    deleteSNOFilePath,
    deleteSMUFilePath,
    downloadSNOFilePath,
    downloadSMUFilePath
} from "../data/APIPaths";

const { Option } = Select;

export function DocumentBlock() {
    const [documents, setDocuments] = useState([]);
    const [selectedType, setSelectedType] = useState("sno");

    const fetchDocuments = async (path) => {
        try {
            const response = await axios.get(path);
            const data = response.data;
            console.log(data);

            const formattedDocuments = data.map(fileDto => ({
                id: fileDto.id,
                name: fileDto.fileName,
                image: fileImagePath,
                downloadLink: `${path}${fileDto.fileName}`,
            }));
            console.log(formattedDocuments);
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

    const handleDelete = async (id) => {
        try {
            const documentToDelete = documents.find(doc => doc.id === id);
            if (!documentToDelete) {
                console.error(`Документ с id ${id} не найден`);
                return;
            }

            const deletePath = selectedType === "sno" ? deleteSNOFilePath : deleteSMUFilePath;
            await axios.delete(`${deletePath}`, {
                params: {
                    fileName: documentToDelete.name,
                },
            });

            setDocuments(documents.filter(doc => doc.id !== id));
        } catch (error) {
            console.error("Ошибка при удалении документа:", error);
        }
    };

    const handleTypeChange = (value) => {
        setSelectedType(value);
    };

    return (
        <div className="document-block">
            <Select
                defaultValue="sno"
                style={{ width: 200, marginBottom: 20 }}
                onChange={handleTypeChange}
            >
                <Option value="sno">Документы СНО</Option>
                <Option value="smu">Документы СМУ</Option>
            </Select>

            {documents.map(document => (
                <DocumentCart key={document.id} document={document} onDelete={handleDelete} />
            ))}
        </div>
    );
}
