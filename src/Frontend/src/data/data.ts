import {ReportStatus} from "./summarisedReport.ts";

export interface SummarisedReport {
    Id: string;
    DomainName: string;
    RequestedAt: Date;
    Status: ReportStatus;
}

