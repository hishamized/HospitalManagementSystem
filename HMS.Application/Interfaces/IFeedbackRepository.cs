using System.Threading.Tasks;
using HMS.Application.DTO.Feedback;

namespace HMS.Application.Interfaces
{
    public interface IFeedbackRepository
    {
        // Persists feedback and returns the newly created feedback Id
        Task<int> AddFeedbackAsync(CreateFeedbackDto dto);
        Task<IEnumerable<FeedbackListDto>> GetAllFeedbacksAsync();
        Task<int> DeleteFeedbackAsync(int id);
        Task<IEnumerable<DoctorFeedbackDto>> GetFeedbackByDoctorIdAsync(int doctorId);
    }
}
