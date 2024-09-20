using ApiBRD.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiBRD.Repositories
{
    public class IRepository<T> where T : class
    {
        public LabsystePwaBrdContext Context { get; set; }
        public IRepository(LabsystePwaBrdContext context)
        {
            Context = context;
        }

        public T GetById(int id)
        {
            return Context.Set<T>().Find(id);
        }

        public IList<T> GetAll()
        {
            return Context.Set<T>().ToList();
        }

        public void Insert(T entity)
        {
            Context.Set<T>().Add(entity);
            Context.SaveChanges();

        }

        public void Update(T entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
            Context.SaveChanges();

        }

        public void Delete(T entity)
        {
            Context.Set<T>().Remove(entity);
            Context.SaveChanges();
        }

    }
}
