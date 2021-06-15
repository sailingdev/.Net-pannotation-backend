using System;
using System.Collections.Generic;
using System.Text;

namespace Pannotation.Services.Interfaces.Export
{
    public interface IExporter
    {
        /// <summary>
        /// Export data to certain format
        /// </summary>
        /// <typeparam name="T">Exported data format</typeparam>
        /// <param name="data">Data to be exported</param>
        /// <returns>Exported data</returns>
        byte[] Export<T>(T data);
    }
}
