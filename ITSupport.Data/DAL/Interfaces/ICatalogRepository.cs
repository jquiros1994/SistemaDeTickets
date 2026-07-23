using System.Collections.Generic;

namespace ITSupport.DAL
{
    // SOLID: ISP (Interface Segregation Principle)
    // SOLID: DIP (Dependency Inversion Principle)
    public interface ICatalogRepository<T>
    {
        List<T> GetAll();
    }
}
