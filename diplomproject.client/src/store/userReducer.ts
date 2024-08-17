// Определение типов для состояния редьюсера
import { createSlice } from "@reduxjs/toolkit";


const userSlice = createSlice({
    name: 'userSlice',
    initialState: { phoneNumber: "" },
    reducers: {
        setPhoneNumber(state, action) {
            state.phoneNumber = action.payload
        },
    },
});

export const {
    setPhoneNumber
} = userSlice.actions;

export default userSlice.reducer;