using System;
using System.Collections.Generic;
using ITSupport.Models;

namespace ITSupport.DAL
{
    // SOLID: DIP - la capa Business depende de esta abstraccion
    // DP: Repository 
    public interface ICaseRepository
    {
        List<CaseViewModel> GetAllWithDetails(int? statusId = null, string search = null);
        CaseViewModel GetByIdWithDetails(int caseId);
        Case GetById(int caseId);
        int Insert(Case c);
        bool Update(Case c);
        bool Delete(int caseId);
        bool SetClosedAt(int caseId, DateTime? closedAt);
    }
}
