using ITSupport.DAL;
using ITSupport.Models;

namespace ITSupport.Business
{
    public class DashboardBusiness
    {
        private readonly UserRepository _users = new UserRepository();
        private readonly DashboardRepository _dashboard = new DashboardRepository();

        public User ValidateUser(string email, string password)
        {
            return _users.ValidateUser(email, password);
        }

        public void UpdateLastLogin(int userId)
        {
            _users.UpdateLastLogin(userId);
        }

        public int GetOpenCasesCount()
        {
            return _dashboard.GetOpenCasesCount();
        }

        public int GetCriticalCasesCount()
        {
            return _dashboard.GetCriticalCasesCount();
        }

        public int GetResolvedTodayCount()
        {
            return _dashboard.GetResolvedTodayCount();
        }

        public int GetActiveEngineersCount()
        {
            return _dashboard.GetActiveEngineersCount();
        }

        public object GetRecentCases(int count)
        {
            return _dashboard.GetRecentCases(count);
        }
    }
}