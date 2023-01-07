using EverrichFakeCrawler.Models;
using OfficeOpenXml;
using System.Drawing;
using EverrichFakeCrawler.Interfaces;

namespace EverrichFakeCrawler.Services
{
    public class ExcelService
    {
        private readonly ILogger<ExcelService> _logger;
        public ExcelService(ILogger<ExcelService> logger)
        {
            _logger = logger;
        }
        public async Task<ExcelResponse> ExportExcelBuffer(List<CrawlerModel> crawlerData)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                string folder = "Template";
                string fileName = "template.xlsx";
                var filepath = Path.Combine(folder, fileName);
                var filename_new = Path.Combine(folder, $"{DateTime.Now.ToString("yyyy-MM-dd")}_Everrich爬蟲.xlsx");

                //開檔
                await using var fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                //載入Excel檔案
                using var ep = new ExcelPackage(fs);
                ep.Settings.TextSettings.PrimaryTextMeasurer = new SkiaSharpTextMeasurer();

                var sheet = ep.Workbook.Worksheets[0]; //取得Sheet1

                for (int i = 0; i < crawlerData.Count; i++)
                {
                    var data = crawlerData[i];
                    var index = i + 2;
                    sheet.Cells["A" + index.ToString()].Value = data.Name.Trim(); // 名稱
                    sheet.Cells["B" + index.ToString()].Value = DateTime.Parse(data.Time).ToString("yyyy/MM/dd"); //日期
                    sheet.Cells["C" + index.ToString()].Value = data.LikeCount.Trim(); //按讚數
                    sheet.Cells["D" + index.ToString()].Value = "連結"; //超連結
                    sheet.Cells["D" + index.ToString()].SetHyperlink(new ExcelHyperLink(data.Hyperlink));
                    sheet.Cells["D" + index.ToString()].Style.Font.UnderLine = true;
                    sheet.Cells["D" + index.ToString()].Style.Font.Color.SetColor(Color.Blue);

                    sheet.Cells["A" + index.ToString()].AutoFitColumns();
                    sheet.Cells["B" + index.ToString()].AutoFitColumns();
                    sheet.Cells["C" + index.ToString()].AutoFitColumns();
                    sheet.Cells["D" + index.ToString()].AutoFitColumns();
                }

                // 建立檔案
                await using var fsNew = new FileStream(filename_new, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                //另存新檔
                await ep.SaveAsAsync(fsNew);

                //開檔
                await using var fsRead = new FileStream(filename_new, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                byte[] buffer = new byte[fsRead.Length];
                await fsRead.ReadAsync(buffer, 0, buffer.Length);

                //File.Delete(filename_new); 

                return new ExcelResponse
                {
                    Data = buffer,
                    Msg = "成功",
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new ExcelResponse
                {
                    Data = null,
                    Msg = ex.Message,
                };
            }
        }
    }
}
