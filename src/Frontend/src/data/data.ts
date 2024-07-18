import {ReportStatus} from "./report.ts";

export interface SummarisedReport {
    id: string;
    domainName: string;
    requestedAt: string;
    status: ReportStatus;
}

