using System.Collections.Generic;
using ITSupport.DAL;
using ITSupport.Models;

namespace ITSupport.Business
{
    public class EngineerBusiness
    {
        private readonly SupportEngineerRepository _engineers = new SupportEngineerRepository();

        public List<EngineerViewModel> GetAll()
        {
            return _engineers.GetAllWithLevel();
        }
    }
}
