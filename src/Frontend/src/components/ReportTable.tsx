import { ReactElement } from "react";
import { SummarisedReport } from "../data/data.ts";
import { ReportStatus } from "../data/report.ts";
import { getRelativeTimeString } from "../utils/relativeTime.ts";
import { Link } from "react-router-dom";

interface ReportTableProps {
    reports: SummarisedReport[] | undefined;
    currentPage: number;
    onPageChange: (page: number) => void;
    itemCount: number;

    pageSize: number;
    onPageSizeChange: (pageSize: number) => void;

    linkDomain?: boolean;
}

export function ReportTable(props: ReportTableProps): ReactElement<ReportTableProps, string> {
    const reports = props.reports || [];

    const relativeTime = (date: Date) => {
        let text = getRelativeTimeString(date);

        return <time dateTime={date.toISOString()}>{text}</time>
    }

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
        <>
            <p>There {reports.length == 1 ? "is" : "are"} {reports.length} reports</p>

            <table className="table table-striped">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Domain Name</th>
                        <th>Requested</th>
                        <th>Status</th>
                    </tr>
                </thead>
                <tbody>
                    {reports.length === 0 && <tr><td colSpan={4}>No reports to display</td></tr>}
                    {reports.map(report =>
                        <tr key={report.id}>
                            <td>
                                <Link to={`/${report.domainName}/${report.id}`}>{report.id}</Link>
                            </td>
                            <td>
                                {props.linkDomain ?
                                    <Link to={`/${report.domainName}`}>{report.domainName}</Link> :
                                    report.domainName}
                            </td>
                            <td>
                                {relativeTime(new Date(report.requestedAt))}
                            </td>
                            <td>{statusName(report.status)}</td>
                        </tr>
                    )}
                </tbody>
            </table>

            <select name="pageSize" id="pageSize" onChange={e => props.onPageSizeChange(parseInt(e.target.value))}>
                <option value="5">5</option>
                <option value="10">10</option>
                <option value="25">25</option>
                <option value="50">50</option>
                <option value="100">100</option>
            </select>

            {props.itemCount >= 1 && (
                <nav>
                    <button className="page-link" onClick={() => props.onPageChange(props.currentPage - 1)} disabled={props.currentPage === 1}>Previous</button>
                    <button className="page-link" onClick={() => props.onPageChange(props.currentPage + 1)} disabled={props.currentPage * props.pageSize >= props.itemCount}>Next</button>
                </nav>
            )}
        </>
    )
}
