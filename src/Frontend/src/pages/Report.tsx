import { ReactElement, useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { ReportResponse } from "../data/responses";
import { DomainReport } from "../data/report";

export function Report(): ReactElement {
    const params = useParams();
    const domain = params.domain ?? '';
    const reportId = params.reportId ?? '';

    const [report, setReport] = useState<DomainReport | null>(null);

    useEffect(() => {
        (async () => {
            let data: ReportResponse;
            try {
                const response = await fetch(`/api/reports/${domain}/${reportId}`);
                data = await response.json();
            } catch (e) {
                console.error(e);
                return;
            }

            setReport(data.report);
        })()
    }, []);

    if (report === null) {
        return <h1>Loading...</h1>;
    }

    return (
        <>
            <h1><Link to="/">⇤</Link> {domain}</h1>

            <h2>Report {reportId}</h2>

            <h3>Name servers</h3>

            <ul>
                {report.nameServers.map((ns, i) => (
                    <li key={i}>
                        <code>{ns.hostname}</code> ({ns.serviceName})
                    </li>
                ))}
            </ul>

            <h3>IP addresses (apex domain)</h3>

            <ul>
                {report.apexAddresses.map((ip, i) => (
                    <li key={i}>
                        <code>{ip.value}</code> - <code>{ip.reverseName}</code> (owned by {ip.orgName} - {ip.city}, {ip.country})
                    </li>
                ))}
            </ul>

            <h3>TXT records (apex domain)</h3>

            <ul>
                {report.apexText.map((txt, i) => (
                    <li key={i}>
                        <code>{txt}</code>
                    </li>
                ))}
            </ul>

            <hr />

            <small>
                Report requested at {new Date(report.requestedAt).toISOString()}
                {report.completedAt !== null && (
                    <>
                        , completed at {new Date(report.completedAt!).toISOString()}
                    </>)
                }
            </small>
        </>
    );
}