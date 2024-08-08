import { configureStore } from "@reduxjs/toolkit";
import documentReducer from "./documentReducer";
import userReducer from "./userReducer";
import { TypedUseSelectorHook, useDispatch, useSelector } from "react-redux";



//все reducer внутри будут
const store = configureStore({
    reducer: {
        user: userReducer,
        documents: documentReducer,
    },
});

type AppState = ReturnType<typeof store.getState>;
type AddDispatch = typeof store.dispatch;
export const useAppDispatch = () => useDispatch<AddDispatch>();
export const useAppSelector: TypedUseSelectorHook<AppState> = useSelector;
export default store;