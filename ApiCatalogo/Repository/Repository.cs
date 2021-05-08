using ApiCatalogo.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ApiCatalogo.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected AppDbContext _contexto;
        public Repository(AppDbContext contexto)
        {
            _contexto = contexto;
        }
        //A grande diferença entre IEnumerable e  IQueryable é onde o filtro é executado. O primeiro executa no cliente(memória) e outro executa no banco de dados. http://www.macoratti.net/14/11/net_ieiq1.htm
        public IQueryable<T> Get() // Retornar uma lista de entidades, 
        {
            return _contexto.Set<T>().AsNoTracking();//Desabilita o rastreamento de entidade e assim ganha desempenho
        }
        public async Task<T> GetById(Expression<Func<T, bool>> predicate)
        {
            return await _contexto.Set<T>().SingleOrDefaultAsync(predicate);
        }
        public void Add(T entity)
        {
            _contexto.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            _contexto.Set<T>().Remove(entity);
        }
        public void Update(T entity)
        {
            _contexto.Entry(entity).State = EntityState.Modified;//Modificando o contexto da entidade para moficado
            _contexto.Set<T>().Update(entity);
        }
    }
}
