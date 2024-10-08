import { Container, Nav, Navbar as NavbarBs } from "react-bootstrap"
import { NavLink } from "react-router-dom"
import "../styles/navBar.css"

export function Navbar() {
    return (
        <NavbarBs sticky="top" className="navbar-custom bg-white shadow-sm mb-3">
            <Container>
                <Nav className="me-auto">
                    <Nav.Link to="/documents" as={NavLink}>
                        Документы
                    </Nav.Link>
                    <Nav.Link to="/notify" as={NavLink}>
                        Уведомить пользователей
                    </Nav.Link>
                    <Nav.Link to="/tgusers" as={NavLink}>
                        Просмотр пользователей
                    </Nav.Link>
                    <Nav.Link to="/scienceevents" as={NavLink}>
                        Администраторские мероприятия
                    </Nav.Link>
                    <Nav.Link to="/usercreatedevents" as={NavLink}>
                        Пользовательские мероприятия
                    </Nav.Link>
                </Nav>
            </Container>
        </NavbarBs>
    )
}
