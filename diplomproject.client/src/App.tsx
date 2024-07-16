import { useEffect, useState } from 'react';
import './App.css';



function App() {
    const [password, setPassword] = useState<string>();

    useEffect(() => {
        testApi();
    });

    return (
        <div>
            <h1 id="tabelLabel">Weather forecast</h1>
            <p>This component demonstrates fetching data from the server.</p>
            {password}
        </div>
    );

    async function testApi() {
        const response = await fetch('api/passwords/get/123');
        const data = await response.json();
        setPassword(data);
    }
}

export default App;