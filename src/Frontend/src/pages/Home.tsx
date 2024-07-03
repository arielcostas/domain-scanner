import {ReactElement, useEffect, useState} from "react";
import {Link} from "react-router-dom";
import {SummarisedReport} from "../data/data.ts";
import {ListReportsViewModel} from "../data/viewModels.ts";
import {ReportTable} from "../components/ReportTable.tsx";

export function Home(): ReactElement {
    const [page, setPage] = useState<number>(1);
    const [reports, setReports] = useState<SummarisedReport[]>();

    useEffect(() => {
        console.log('fetching reports');
        (async () => {
            const response = await fetch('api/reports');
            const data = await response.json() as ListReportsViewModel;
            setReports(data.reports);
        })()
    }, [page]);
    
    return (
        <>
        <h1>Home</h1>
            <ReportTable reports={reports} />
            <Link to="/dos">Dos</Link>
        </>);
}