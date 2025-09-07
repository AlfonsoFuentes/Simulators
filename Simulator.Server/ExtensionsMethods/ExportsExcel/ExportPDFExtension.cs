

using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Simulator.Shared.Commons.FileResults;
using System.Globalization;

namespace Simulator.Server.ExtensionsMethods.ExportsExcel
{
    public static class ExportPDFExtension
    {
        //static byte[] CPLogo = null!;
        //static void GetImageData(IWebHostEnvironment host)
        //{
        //    var path = host.WebRootPath;

        //    if (path == null)
        //    {
               
        //        return;
        //    }
        //    Console.WriteLine(path);
        //    var rutaImagen = Path.Combine(path, "Assets/CPLogo.PNG");
        //    CPLogo = System.IO.File.ReadAllBytes(rutaImagen);

          
           
        //}
        //public static Image _colgatePalmoliveLogo { get; } = Image.FromFile("Assets/Colgate-Palmolive-Logo.PNG");
        public static IQueryable Query = null!;
        public static string FileName = string.Empty;
        public static string ReportName = string.Empty;
        public static FileResult ExportPDF(IQueryable query, string fileName, string reportName)
        {
            //GetImageData();
            Query = query;
            FileName = fileName;
            ReportName = reportName;


            byte[] pdfBytes = GenerateReportBytes();

            FileResult newresult = new()
            {
                Data = pdfBytes,
                ExportFileName = $"{fileName}.pdf",
                ContentType = FileResult.pdfContentType,
            };
            return newresult;

        }

        public static byte[] GenerateReportBytes()
        {
            byte[] reportBytes;
            Document document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(0.8f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Tahoma", "Arial", ""));
                    page.Content().Element(ComposeReportContent);
                    page.Footer().Element(ComposeFooterContent);
                });
            });

            reportBytes = document.GeneratePdf();
            return reportBytes;
        }
        static void ComposeFooterContent(IContainer container)
        {
            container.Column(footer =>
            {
                footer.Item().Row(row =>
                {
                    row.AutoItem().Text($"{DateTime.Today.ToString("ddd MMM dd, yyyy")}").FontSize(8);
                    row.RelativeItem().AlignCenter().Text($"Project Management Tool Develoved by Alfonso Fuentes").FontSize(8);
                    row.AutoItem().AlignRight().Text(text =>
                    {
                        text.Span("Page ").FontSize(8);
                        text.CurrentPageNumber().FontSize(8);
                        text.Span(" of ").FontSize(8);
                        text.TotalPages().FontSize(8);
                    });
                });
            });
        }
        static IContainer DefaultCellStyle(IContainer container, string backgroundColor = "")
        {
            return container
              .Border(1)
              .BorderColor(Colors.Grey.Lighten1)
              .Background(!string.IsNullOrEmpty(backgroundColor) ? backgroundColor : Colors.White)
              .PaddingVertical(7)
              .PaddingHorizontal(3);
        }


        static void ComposeReportContent(IContainer container)
        {
            var querycolumns = ExportHelpers.GetProperties(Query.ElementType);

            int serialNumber = 0;


            container.Column(mainContentColumn =>
            {
                mainContentColumn.Item().Row(row =>
                {
                    //row.AutoItem().Column(column =>
                    //{
                    //    column.Item().Width(1, QuestPDF.Infrastructure.Unit.Inch).Image(_colgatePalmoliveLogo);
                    //});

                    row.RelativeItem().AlignCenter().Column(column =>
                    {
                        column
                            .Item().Text("COLGATE PALMOLIVE")
                            .FontSize(20).SemiBold();

                        column
                            .Item().AlignCenter().PaddingBottom(0.5f, Unit.Centimetre).Text("Cali, Colombia.")
                            .FontSize(13).SemiBold();

                        column
                            .Item().AlignCenter().Text(ReportName).Underline()
                            .FontSize(16);
                    });


                });


                mainContentColumn.Item().PaddingTop(0.8f, Unit.Centimetre).Row(row =>
                {
                    row.RelativeItem().Shrink().Border(1).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40);
                            foreach (var column in querycolumns)
                            {
                                columns.RelativeColumn();
                            }

                        });

                        // please be sure to call the 'header' handler!
                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).AlignCenter().Text("Item").FontSize(9).SemiBold();
                            foreach (var column in querycolumns)
                            {
                                header.Cell().Element(CellStyle).AlignLeft().Text(column.Key).FontSize(9).SemiBold();

                            }



                            // you can extend existing styles by creating additional methods
                            IContainer CellStyle(IContainer container) => DefaultCellStyle(container, Colors.Grey.Lighten3);
                        });

                        foreach (var item in Query)
                        {
                            serialNumber += 1;
                            table.Cell().Element(CellStyle).AlignCenter().Text($"{serialNumber}").FontSize(9);
                            foreach (var column in querycolumns)
                            {
                                var value = ExportHelpers.GetValue(item, column.Key);
                                var stringValue = $"{value}".Trim();



                                var underlyingType = column.Value.IsGenericType &&
                                    column.Value.GetGenericTypeDefinition() == typeof(Nullable<>) ?
                                    Nullable.GetUnderlyingType(column.Value) : column.Value;

                                var typeCode = Type.GetTypeCode(underlyingType);

                                if (typeCode == TypeCode.DateTime)
                                {
                                    if (!string.IsNullOrWhiteSpace(stringValue))
                                    {
                                        table.Cell().Element(CellStyle).Text($"{((DateTime)value).ToOADate().ToString(CultureInfo.InvariantCulture)}").FontSize(9);

                                    }
                                }
                                else if (typeCode == TypeCode.Boolean)
                                {
                                    table.Cell().Element(CellStyle).Text(stringValue.ToLower()).FontSize(9);

                                }
                                else if (ExportHelpers.IsNumeric(typeCode))
                                {
                                    if (value != null)
                                    {
                                        stringValue = Convert.ToString(value, CultureInfo.InvariantCulture);
                                    }
                                    table.Cell().Element(CellStyle).Text(stringValue).FontSize(9);

                                }
                                else
                                {
                                    table.Cell().Element(CellStyle).Text(stringValue).FontSize(9);

                                }


                            }






                            IContainer CellStyle(IContainer container) => DefaultCellStyle(container).ShowOnce();

                        }

                    });
                });


            });
        }
    }

}
