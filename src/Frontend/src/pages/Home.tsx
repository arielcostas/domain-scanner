﻿import { ReactElement, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { SummarisedReport } from "../data/data.ts";
import { ListReportsResponse } from "../data/responses.ts";
import { ReportTable } from "../components/ReportTable.tsx";
import { SearchBar } from "../components/SearchBar.tsx";

export function Home(): ReactElement {
    const [pageSize, setPageSize] = useState<number>(5);
    const [currentPage, setCurrentPage] = useState<number>(1);

    const [itemCount, setItemCount] = useState<number>(1);
    const [reports, setReports] = useState<SummarisedReport[]>();

    const navigate = useNavigate();

    const handleDomainSearch = (query: string) => {
        navigate(`/${query}`);
    }

    useEffect(() => {
        (async () => {
            let data: ListReportsResponse;
            try {
                const response = await fetch(`/api/reports?page=${currentPage}&pageSize=${pageSize}`);
                data = await response.json();
            } catch (e) {
                console.error(e);
                return;
            }
            setReports(data.reports);
            setItemCount(data.total);
        })()
    }, [currentPage, pageSize]);

    return (
        <>
            <h1>Home</h1>

            <SearchBar name={"domains"} label={"Buscar dominio"} placeholder={"example.com"} onSearch={handleDomainSearch} />

            <ReportTable reports={reports}
                currentPage={currentPage}
                onPageChange={setCurrentPage}
                itemCount={itemCount}
                pageSize={pageSize}
                onPageSizeChange={setPageSize}
                linkDomain
            />
        </>);
}