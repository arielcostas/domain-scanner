import {ReactElement} from "react";
import {Link} from "react-router-dom";

export function Dos(): ReactElement {
    return (
        <>
            <h1>Dos</h1>

            <Link to="/">Home</Link>
        </>
    );
}