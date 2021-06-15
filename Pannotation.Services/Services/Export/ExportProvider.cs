using Pannotation.Models.Enums;
using Pannotation.Services.Interfaces.Export;
using System;

namespace Pannotation.Services.Services.Export
{
    public class ExportProvider<T> : IExportProvider<T>
    {
        private IExporter _exporter;
        IServiceProvider _serviceProvider;

        public ExportProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public byte[] Export(T data, ExportType exportType)
        {
            switch (exportType)
            {
                case ExportType.Csv:
                    _exporter = (CsvExporter)_serviceProvider.GetService(typeof(CsvExporter));
                    break;
            }

            return _exporter.Export(data);
        }
    }
}
