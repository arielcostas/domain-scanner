import { SummarisedReport } from "./data.ts";
import { DomainReport } from "./report.ts";

export interface ListReportsResponse {
    reports: SummarisedReport[];
    total: number;
}

export interface ListReportsBydomainResponse {
    domain: string;
    reports: SummarisedReport[];
    total: number;
}

export interface ReportResponse {
    report: DomainReport;
}