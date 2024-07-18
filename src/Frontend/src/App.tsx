import {ReactElement} from 'react';
import {BrowserRouter, Route, Routes} from "react-router-dom";
import {Home} from "./pages/Home.tsx";
import 'simpledotcss/simple.min.css';
import { Domain } from './pages/Domain.tsx';
import { Report } from './pages/Report.tsx';

function App(): ReactElement {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<Home/>}/>
                <Route path="/:domain" element={<Domain />}/>
                <Route path="/:domain/:reportId" element={<Report />}/>
            </Routes>
        </BrowserRouter>
    );
}

export default App;