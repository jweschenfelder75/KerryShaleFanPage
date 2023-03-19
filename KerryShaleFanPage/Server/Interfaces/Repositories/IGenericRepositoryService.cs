using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace KerryShaleFanPage.Server.Interfaces.Repositories
{
    public interface IGenericRepositoryService<TDto> where TDto : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IList<TDto> GetAll();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TDto? GetLast();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TDto? GetById(long id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<TDto?> UpsertAsync(TDto dto, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<bool> DeleteByIdAsync(long id, CancellationToken cancellationToken = default);
    }
}
