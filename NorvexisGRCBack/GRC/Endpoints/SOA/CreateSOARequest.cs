namespace GRC.Endpoints.SOA;

public record CreateSOARequest(
    string Section,
    string Control,
    string Tittle,
    string Objective,
    bool Applicable,
    bool Implemented,
    string Justification,
    string Evidence
);