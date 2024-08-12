import './App.css';
import store from "./store/store"
import { Home } from './pages/Home';
import { DocumentPage } from './pages/DocumentPage';
import { Route, Routes } from 'react-router-dom';
import { Container } from 'react-bootstrap';
import { Provider } from 'react-redux';
import { NotifyPage } from './pages/NotifyPage';
import { TgUsersPage } from './pages/TgUsersPage';
import { ScienceEventPage } from './pages/ScienceEventPage';
import { UserCreatedEventPage } from './pages/UserCreatedEventPage';


function App() {
    return (
        <Provider store={store}>
            <Container className="mb-4">
                <Routes>
                    <Route path="/" element={<Home />} />
                    <Route path="/documents" element={<DocumentPage />} />
                    <Route path="/notify" element={<NotifyPage />} />
                    <Route path="/tgusers" element={<TgUsersPage />} />
                    <Route path="/scienceevents" element={<ScienceEventPage />} />
                    <Route path="/usercreatedevents" element={<UserCreatedEventPage />} />
                </Routes>
            </Container>
        </Provider>
    )
}

export default App;