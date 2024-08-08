import { Container, Nav, Navbar as NavbarBs } from "react-bootstrap"
import { NavLink } from "react-router-dom"
import { useAppSelector } from "../store/store"

export function Navbar() {
    //просмотр состояний
    const { isLogged } = useAppSelector(state => state.user)
    return (
        <NavbarBs sticky="top" className="bg-white shadow-sm mb-3">
            <Container>
                <Nav className="me-auto">
                    <Nav.Link to="/" as={NavLink}>
                        Home
                    </Nav.Link>
                    <Nav.Link to="/documents" as={NavLink}>
                        Documents
                    </Nav.Link>
                    {/* <Nav.Link to="/admin" as={NavLink}>
                        {isLogged === true ? "Sign out" : "Sign in"}
                    </Nav.Link> */}
                </Nav>
            </Container>
        </NavbarBs>
    )
}
