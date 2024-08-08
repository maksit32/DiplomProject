import React from 'react';
import '../styles/footer.css';

type FooterProps = {
    year: number;
    email: string;
    phone: string;
}

const Footer: React.FC<FooterProps> = ({ year, email, phone }) => {
    return (
        <footer className="footer-container">
            <p>Контакты для связи: <a href={`mailto:${email}`}>{email}</a></p>
            <p>{phone}</p>
            <p>&copy; {year} Все права защищены.</p>
        </footer>
    );
};

export default Footer;