using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonetaFMS.Interfaces
{
    /// <summary>
    /// CRUD Interface for SQLite table
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITableService<T>
    {
        List<T> AllItems { get; set; }

        T CreateEntry(T newValue);
        T ReadEntry(int id);
        bool UpdateEntry(T updatedValue);
        bool DeleteEntry(T deletedValue);
    }
}
