import './App.css';
import store from "./store/store"
import { Home } from './pages/Home';
import { DocumentPage } from './pages/DocumentPage';
import { Route, Routes } from 'react-router-dom';
import { Container } from 'react-bootstrap';
import { Provider } from 'react-redux';
import { NotifyPage } from './pages/NotifyPage';


function App() {
    return (
        <Provider store={store}>
            <Container className="mb-4">
                <Routes>
                    <Route path="/" element={<Home />} />
                    <Route path="/documents" element={<DocumentPage />} />
                    <Route path="/notify" element={<NotifyPage />} />
                </Routes>
            </Container>
        </Provider>
    )
}

export default App;