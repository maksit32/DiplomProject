import Footer from '../components/Footer'
import { LoginBlock } from '../components/LoginBlock'
import { CONTACT_EMAIL } from '../data/Constants'


export function Home() {
    return (
        <>
            <LoginBlock />
            <Footer year={new Date().getFullYear()} email={CONTACT_EMAIL} />
        </>
    )
}
