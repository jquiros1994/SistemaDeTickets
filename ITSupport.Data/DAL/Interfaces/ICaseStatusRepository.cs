using ITSupport.Models;

namespace ITSupport.DAL
{
    // SOLID: ISP 
    // SOLID: LSP (Liskov Substitution Principle) 
    public interface ICaseStatusRepository : ICatalogRepository<CaseStatus>
    {
        CaseStatus GetById(int statusId);
    }
}
