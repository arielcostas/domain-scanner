import React from "react";
import {SummarisedReport} from "../data/data.ts";
import {ReportStatus} from "../data/summarisedReport.ts";

// Define prop types for ReportTable
interface ReportTableProps {
    reports: SummarisedReport[] | undefined;
}

export function ReportTable(props: ReportTableProps): React.FC<ReportTableProps> {
    const reports = props.reports || [];
    
    const statusName = (status: ReportStatus) => {
        switch (status) {
            case ReportStatus.Created:
                return "Created";
            case ReportStatus.Processing:
                return "Processing";
            case ReportStatus.Failed:
                return "Failed";
            case ReportStatus.Completed:
                return "Completed";
        }
    }

    return (
        <table className="table table-striped">
            <thead>
            <tr>
                <th>ID</th>
                <th>Domain Name</th>
                <th>Requested at</th>
                <th>Status</th>
            </tr>
            </thead>
            <tbody>
            {reports.length === 0 && <tr><td colSpan={4}>No reports to display</td></tr>}
            {reports.map(report =>
                <tr key={report.Id}>
                    <td>{report.Id}</td>
                    <td>{report.DomainName}</td>
                    <td>{report.RequestedAt}</td>
                    <td>{statusName(report.Status)}</td>
                </tr>
            )}
            </tbody>
        </table>
    )
}