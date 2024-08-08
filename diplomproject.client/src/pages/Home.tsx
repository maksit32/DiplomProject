import Footer from '../components/Footer'
import { CONTACT_EMAIL, SUPPORT_PHONE } from '../data/Constants'


export function Home() {
    return (
        <>
            <Footer year={new Date().getFullYear()} email={CONTACT_EMAIL} phone={SUPPORT_PHONE} />
        </>
    )
}
