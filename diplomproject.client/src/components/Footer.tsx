import React from 'react';
import '../styles/footer.css';

type FooterProps = {
    year: number;
    email: string;
}

const Footer: React.FC<FooterProps> = ({ year, email }) => {
    return (
        <footer className="footer-container">
            <p><a href={`mailto:${email}`}>{email}</a></p>
            <p>&copy; {year} Все права защищены.</p>
        </footer>
    );
};

export default Footer;