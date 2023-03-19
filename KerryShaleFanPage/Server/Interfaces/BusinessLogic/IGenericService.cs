using System.Collections.Generic;

namespace KerryShaleFanPage.Server.Interfaces.BusinessLogic
{
    public interface IGenericService<TDto> where TDto : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IList<TDto> GetAll();
    }
}
