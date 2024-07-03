import {Component, ReactElement, useEffect, useState} from 'react';
import './App.css';
import {BrowserRouter, Route, Router, Routes} from "react-router-dom";
import {Home} from "./pages/Home.tsx";
import {Dos} from "./pages/Dos.tsx";
import {StaticRouter} from "react-router-dom/server";
import {ListReportsViewModel} from "./data/viewModels.ts";
import {SummarisedReport} from "./data/data.ts";

function App(): ReactElement {
    return (
        <BrowserRouter>
            <Routes>
                <Route exact path="/" element={<Home/>}/>
                <Route exact path="/dos" element={<Dos/>}/>
            </Routes>
        </BrowserRouter>
    );
}

export default App;