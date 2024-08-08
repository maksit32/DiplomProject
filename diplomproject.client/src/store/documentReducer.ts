// Определение типов для состояния редьюсера
import { createSlice } from "@reduxjs/toolkit";
import { DocumentItem } from "../context/DocumentItem";


const initialState: DocumentItem[] = [];

const documentSlice = createSlice({
    name: 'documentSlice',
    initialState: { documents: initialState },
    reducers: {
        setDocuments(state, action) {
            state.documents = action.payload
        },
        addDocument(state, action) {
            const documentToAdd = action.payload;
            state.documents.push({
                name: documentToAdd.name,
            });
        },
        deleteDocument(state, action) {
            const documentToDelete = action.payload;
            const documentToRemoveIndex = state.documents.findIndex(doc => doc.name === documentToDelete.name);
            if (documentToRemoveIndex !== -1) {
                // Удаление элемента из массива
                state.documents = state.documents.filter(doc => doc.name !== documentToDelete.name);
            }
        },
        deleteAllDocuments(state: any) {
            return {
                ...state,
                documents: []
            };
        }
    },
});

export const {
    setDocuments,
    addDocument,
    deleteDocument,
    deleteAllDocuments
} = documentSlice.actions;

export default documentSlice.reducer;