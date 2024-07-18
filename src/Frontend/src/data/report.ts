export interface DomainReport {
    id: string;
    domainName: string;
    error?: string;

    nameServers: NameServer[];
    apexAddresses: Address[];
    apexText: string[];

    requestedAt: string;
    completedAt?: string;
    status: ReportStatus;
}

export enum ReportStatus {
    Created = 0,
    Processing = 1,
    Failed = 2,
    Completed = 3
}

export interface Address {
    value: string;
    reverseName: string;
    asn: string;

    orgName: string;
    city: string;
    region: string;
    country: string;
}

export interface NameServer {
    hostname: string;
    serviceName: string;
}
