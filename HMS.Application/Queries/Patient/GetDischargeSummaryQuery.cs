using HMS.Application.DTO.Patient;
using MediatR;


namespace HMS.Application.Queries.Patient
{
    public record GetDischargeSummaryQuery(int VisitId) : IRequest<DischargeSummaryDto>;

    /* This syntactic sugar is equivalent to this code: 
     public class GetDischargeSummaryQuery : IRequest<DischargeSummaryDto>
    {
    public int VisitId { get; init; }

    public GetDischargeSummaryQuery(int visitId)
    {
       VisitId = visitId;
    }
     */
}
