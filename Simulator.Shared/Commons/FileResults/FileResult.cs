using Simulator.Shared.Commons.FileResults.Generics.Reponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.Shared.Commons.FileResults
{
    public class FileResult: IResponseAll
    {
        public static string OpenxmlExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        public static string ExcelContentType = "application/octet-stream";

        public static string CSVContentType = "text/csv";

        public static string pdfContentType = "application/pdf";

        public string ExportFileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public byte[]? Data { get; set; }
    }
}
