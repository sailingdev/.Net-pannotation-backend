using Pannotation.Models.Enums;

namespace Pannotation.Services.Interfaces.Export
{
    public interface IExportProvider<T>
    {
        /// <summary>
        /// Export data to specific format
        /// </summary>
        /// <param name="data">Data to be exported</param>
        /// <param name="exportType">Export type</param>
        /// <returns>Exported file</returns>
        byte[] Export(T data, ExportType exportType);
    }
}
