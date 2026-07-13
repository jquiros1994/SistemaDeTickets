using System.Collections.Generic;
using ITSupport.DAL;
using ITSupport.Models;

namespace ITSupport.Business
{
    public class CaseBusiness
    {
        private readonly CaseRepository _repository = new CaseRepository();
        private readonly CaseStatusRepository _statuses = new CaseStatusRepository();
        private readonly PriorityRepository _priorities = new PriorityRepository();
        private readonly ContactRepository _contacts = new ContactRepository();
        private readonly SupportEngineerRepository _engineers = new SupportEngineerRepository();
        private readonly ProgramRepository _programs = new ProgramRepository();

        public List<CaseViewModel> GetAllWithDetails(int? statusId = null, string search = null)
        {
            return _repository.GetAllWithDetails(statusId, search);
        }

        public CaseViewModel GetByIdWithDetails(int caseId)
        {
            return _repository.GetByIdWithDetails(caseId);
        }

        public int Create(Case model)
        {
            return _repository.Insert(model);
        }
        public List<CaseStatus> GetStatuses()
        {
            return _statuses.GetAll();
        }

        public List<Priority> GetPriorities()
        {
            return _priorities.GetAll();
        }

        public List<Contact> GetContacts()
        {
            return _contacts.GetAll();
        }

        public List<SupportEngineer> GetEngineers()
        {
            return _engineers.GetAll();
        }

        public List<SupportProgram> GetPrograms()
        {
            return _programs.GetAll();
        }
    }
}