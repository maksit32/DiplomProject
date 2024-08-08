import './App.css';
import store from "./store/store"
import { Navbar } from "./components/NavBar"
import { Home } from './pages/Home';
import { DocumentPage } from './pages/DocumentPage';
import { Route, Routes } from 'react-router-dom';
import { Container } from 'react-bootstrap';
import { Provider } from 'react-redux';


function App() {
    return (
        <Provider store={store}>
            <Navbar />
            <Container className="mb-4">
                <Routes>
                    <Route path="/" element={<Home />} />
                    <Route path="/documents" element={<DocumentPage />} />
                </Routes>
            </Container>
        </Provider>
    )
}

export default App;