// Определение типов для состояния редьюсера
import { createSlice } from "@reduxjs/toolkit";


const userSlice = createSlice({
    name: 'userSlice',
    initialState: { isLogged: false },
    reducers: {
        setLogged(state, action) {
            state.isLogged = action.payload
        },
    },
});

export const {
    setLogged
} = userSlice.actions;

export default userSlice.reducer;