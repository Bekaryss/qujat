using Microsoft.AspNetCore.Mvc;
using Qujat.Core.Data;
using System.IO;
using System;
using System.Threading;
using System.Threading.Tasks;
using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.Style;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Qujat.Core.Data.Entities;
using System.Reflection.Metadata;
using System.Net;
using Qujat.Core.DTOs.Shared;
using Qujat.Core.Exceptions;
using System.Globalization;

namespace Qujat.Backoffice.Api.Controllers
{
    public enum StatisticsAggregationType
    {
        Preview,
        Upload,
        Download
    }

    internal class ReportType1Row
    {
        public string CategoryName { get; set; }
        public int UploadedDocumentsCount { get; set; }
        public int PreviewedDocumentsCount { get; set; }
        public int DownloadedDocumentsCount { get; set; }
    }

    internal class ReportType2Row
    {
        public string CategoryName { get; set; }
        public string SubcategoryName { get; set; }
        public int UploadedDocumentsCount { get; set; }
        public int PreviewedDocumentsCount { get; set; }
        public int DownloadedDocumentsCount { get; set; }
    }

    internal class ReportType3Row
    {
        public string CategoryName { get; set; }
        public string SubcategoryName { get; set; }
        public string DocumentName { get; set; }
        public int PreviewedDocumentsCount { get; set; }
        public int DownloadedDocumentsCount { get; set; }
    }

    public class StatisticsByCategoryDto
    {
        public long CategoryId { get; set; }
        public string CategoryName { get; set; }
        public long TotalUploadedDocuments { get; set; }
        public long TotalPreviewedDocuments { get; set; }
        public long TotalDownloadedDocuments { get; set; }
    }

    public class StatisticsBySubcategoryDto
    {
        public long ParentCategoryId { get; set; }
        public long SubcategoryId { get; set; }
        public string SubcategoryName { get; set; }
        public long TotalUploadedDocuments { get; set; }
        public long TotalPreviewedDocuments { get; set; }
        public long TotalDownloadedDocuments { get; set; }
    }


    public class StatisticsByDocumentDto
    {
        public long ParentCategoryId { get; set; }
        public long ParentSubcategoryId { get; set; }
        public string CategoryName { get; set; }
        public string SubcategoryName { get; set; }
        public long DocumentId { get; set; }
        public string DocumentName { get; set; }
        public long TotalPreviewedDocuments { get; set; }
        public long TotalDownloadedDocuments { get; set; }
    }

    public class DailyStatisticsDto
    {
        public string Day { get; set; }
        public long TotalUploadedDocuments { get; set; }
        public long TotalPreviewedDocuments { get; set; }
        public long TotalDownloadedDocuments { get; set; }
    }


    [ApiController]
    [Route("api/1/statistics/overall")]
    [AuthorizeByAccessToken]
    public class OverallStatisticsController(ApplicationDbContext database) : ControllerBase
    {
        private readonly ApplicationDbContext _database = database;

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<DailyStatisticsDto[]>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetStatistics(
            [FromQuery] string from = null,
            [FromQuery] string to = null,
            [FromQuery] long? categoryId = null,
            [FromQuery] long? subcategoryId = null,
            [FromQuery] long? documentId = null,
            CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(from)) { from = DateTime.Now.Date.AddYears(-1).ToString("yyyy-MM-dd"); };
            if (string.IsNullOrEmpty(to)) { to = DateTime.Now.Date.ToString("yyyy-MM-dd"); };

            if (!DateTime.TryParseExact(from, "yyyy-MM-dd", null, DateTimeStyles.None, out DateTime startDateParsed) ||
                !DateTime.TryParseExact(to, "yyyy-MM-dd", null, DateTimeStyles.None, out DateTime finishDateParsed))
            {
                throw new BadRequestException();
            }

            var allCategoriesFromDb = _database.Categories
                .Include(p => p.Subcategories)
                .AsNoTracking()
                .Where(p => !p.IsDeleted)
                .ToArray();


            var allEvents = await _database.DocumentActionEventEntities
                .Where(p => p.CreatedOn >= startDateParsed && p.CreatedOn <= finishDateParsed)
                .OrderBy(p => p.CreatedOn)
                .ToArrayAsync(ct);

            if(categoryId.HasValue)
            {
                var categoryById = allCategoriesFromDb.SingleOrDefault(p => p.Id == categoryId) ??
                    throw new ResourceNotFoundException();

                var subcategoryIds = categoryById.Subcategories?.Select(p => p.Id).ToArray();

                allEvents = allEvents.Where(p => subcategoryIds.Contains(p.SubcategoryId)).ToArray();
            }

            if (subcategoryId.HasValue)
            {
                var subcategoryExist = await _database.Subcategories.AnyAsync(p => p.Id ==  subcategoryId, ct);
                if(!subcategoryExist)
                    throw new ResourceNotFoundException();

                allEvents = allEvents.Where(p => p.SubcategoryId == subcategoryId).ToArray();
            }

            if(documentId.HasValue) 
            {
                allEvents = allEvents.Where(p => p.DocumentId == documentId).ToArray();
            }

            if (finishDateParsed < startDateParsed)
                throw new BadRequestException();

            var response = new List<DailyStatisticsDto>();
            for(DateTime i = startDateParsed; i <= finishDateParsed; i = i.AddDays(1))
            {
                var recordsPerDay = allEvents
                    .Where(p => p.CreatedOn.Date == i.Date)
                    .ToArray();

                if (recordsPerDay.Length == 0)
                {
                    response.Add(new DailyStatisticsDto { Day = i.ToString("yyyy-MM-dd") });
                }
                else
                {
                    var record = new DailyStatisticsDto
                    {
                        Day = i.ToString("yyyy-MM-dd"),
                        TotalDownloadedDocuments = recordsPerDay.Count(p => p.EventType == ActionEventType.Downloaded),
                        TotalPreviewedDocuments = recordsPerDay.Count(p => p.EventType == ActionEventType.Viewed),
                        TotalUploadedDocuments = recordsPerDay.Count(p => p.EventType == ActionEventType.Uploaded),
                    };

                    response.Add(record);
                }
            }

            var apiResponse = ApiResponse<DailyStatisticsDto[]>.Ok(response.ToArray(), 0, response.Count, response.Count);
            return Ok(apiResponse);
        }


        [HttpGet("report/type/1")]
        public async Task<ActionResult> GetReportType1(CancellationToken ct)
        {
            var categories = await _database.Categories
                .Include(p => p.Subcategories)
                .AsNoTracking()
                .Where(p => !p.IsDeleted)
                .ToArrayAsync(ct);

            var documents = await _database.Documents
                .Include(p => p.RelatedParentSubcategories)
                .AsNoTracking()
                .Where(p => !p.IsDeleted)
                .ToArrayAsync(ct);

            var reportRows = new List<ReportType1Row>();

            foreach (var category in categories)
            {
                var subcategoryIds = category.Subcategories
                    .Select(p => p.Id)
                    .ToArray();

                var documentIdsInCategory = documents
                    .Where(p => p.RelatedParentSubcategories
                        .Any(x => subcategoryIds.Contains(x.SubcategoryId)))
                    .Select(p => p.Id)
                    .ToArray();

                var previews = await _database.DocumentActionEventEntities
                    .Where(p => documentIdsInCategory.Contains(p.DocumentId) && p.EventType == ActionEventType.Viewed)
                    .CountAsync(ct);

                var downloads = await _database.DocumentActionEventEntities
                    .Where(p => documentIdsInCategory.Contains(p.DocumentId) && p.EventType == ActionEventType.Downloaded)
                    .CountAsync(ct);

                var reportRow = new ReportType1Row
                {
                    CategoryName = category.NameKz,
                    UploadedDocumentsCount = documentIdsInCategory.Length,
                    PreviewedDocumentsCount = previews,
                    DownloadedDocumentsCount = downloads
                };

                reportRows.Add(reportRow);
            }

            using var ms = new MemoryStream();
            using (var excel = new ExcelPackage())
            {
                var worksheet = excel.Workbook.Worksheets.Add(DateTime.Now.ToString("dd.MM.yyyy"));
                worksheet.DefaultColWidth = 30;
                worksheet.Cells.Style.WrapText = true;
                var firstRow = worksheet.Row(1);

                firstRow.Style.Font.Bold = true;
                firstRow.Height = 30;
                firstRow.Style.Font.Color.SetColor(Color.Black);
                firstRow.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                firstRow.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                var startCol = worksheet.Column(1);
                var directionlCol = worksheet.Column(2);
                var categoryCol = worksheet.Column(3);
                var secondDateCol = worksheet.Column(14);
                var thirdDateCol = worksheet.Column(15);
                var dateCol = worksheet.Column(10);
                var fourthBDateCol = worksheet.Column(11);
                var counterpartyCol = worksheet.Column(12);
                var nameCol = worksheet.Column(4);

                worksheet.Cells[1, 1].Value = "#";
                worksheet.Cells[1, 2].Value = "Наименование категории";
                worksheet.Cells[1, 3].Value = "Количество загруженных в Систему шаблонов документов";
                worksheet.Cells[1, 4].Value = "Количество просмотренных документов";
                worksheet.Cells[1, 5].Value = "Количество выгруженных из Системы документов";

                startCol.Width = 7;
                directionlCol.Width = 15;
                categoryCol.Width = 15;
                dateCol.Width = 40;
                secondDateCol.Width = 40;
                thirdDateCol.Width = 40;
                fourthBDateCol.Width = 40;
                counterpartyCol.Width = 53;
                nameCol.Width = 58;

                worksheet.TabColor = Color.Green;

                int documentNumber = 1;
                int rowNumber = 2;

                foreach (var item in reportRows)
                {
                    worksheet.Cells[rowNumber, 1].Value = documentNumber;
                    worksheet.Cells[rowNumber, 2].Value = item.CategoryName;
                    worksheet.Cells[rowNumber, 3].Value = item.UploadedDocumentsCount;
                    worksheet.Cells[rowNumber, 4].Value = item.PreviewedDocumentsCount;
                    worksheet.Cells[rowNumber, 5].Value = item.DownloadedDocumentsCount;

                    var currentRow = worksheet.Row(rowNumber);
                    currentRow.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    currentRow.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    rowNumber++;
                    documentNumber++;
                }

                excel.SaveAs(ms);
            }
            
            var stream = new MemoryStream(ms.ToArray());
            return File(stream, "application/octet-stream", "Отчет, тип 1.xlsx");
        }

        [HttpGet("report/type/2")]
        public async Task<ActionResult> GetReport2(CancellationToken ct)
        {
            var allEvents = await _database.DocumentActionEventEntities
                .OrderBy(p => p.CreatedOn)
                .ToArrayAsync(ct);

            var allCategoriesFromDb = _database.Categories
                .Include(p => p.Subcategories)
                .AsNoTracking()
                .Where(p => !p.IsDeleted)
                .ToArray();

            var allDocuments = await _database.DocumentSubcategoryRelations
                .Include(p => p.Subcategory)
                    .ThenInclude(p => p.ParentCategory)
                .Include(p => p.Document)
                .Where(p => !p.IsDeleted)
                .ToArrayAsync(ct);

            var reportRows = new List<ReportType2Row>();

            foreach (var category in allCategoriesFromDb)
            {
                foreach (var subcategory in category.Subcategories)
                {
                    reportRows.Add(new ReportType2Row
                    {
                        CategoryName = category.NameKz,
                        SubcategoryName = subcategory.NameKz,
                        DownloadedDocumentsCount = allEvents.Count(p => p.SubcategoryId == subcategory.Id && p.EventType == ActionEventType.Downloaded),
                        UploadedDocumentsCount = allEvents.Count(p => p.SubcategoryId == subcategory.Id && p.EventType == ActionEventType.Uploaded),
                        PreviewedDocumentsCount = allEvents.Count(p => p.SubcategoryId == subcategory.Id && p.EventType == ActionEventType.Viewed),
                    });
                }
            }

            using var ms = new MemoryStream();
            using (var excel = new ExcelPackage())
            {
                var worksheet = excel.Workbook.Worksheets.Add(DateTime.Now.ToString("dd.MM.yyyy"));
                worksheet.DefaultColWidth = 30;
                worksheet.Cells.Style.WrapText = true;
                var firstRow = worksheet.Row(1);

                firstRow.Style.Font.Bold = true;
                firstRow.Height = 30;
                firstRow.Style.Font.Color.SetColor(Color.Black);
                firstRow.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                firstRow.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                var startCol = worksheet.Column(1);
                var directionlCol = worksheet.Column(2);
                var categoryCol = worksheet.Column(3);
                var secondDateCol = worksheet.Column(14);
                var thirdDateCol = worksheet.Column(15);
                var dateCol = worksheet.Column(10);
                var fourthBDateCol = worksheet.Column(11);
                var counterpartyCol = worksheet.Column(12);
                var nameCol = worksheet.Column(4);

                worksheet.Cells[1, 1].Value = "#";
                worksheet.Cells[1, 2].Value = "Наименование категории";
                worksheet.Cells[1, 3].Value = "Наименование подкатегории";
                worksheet.Cells[1, 4].Value = "Количество загруженных в Систему шаблонов документов";
                worksheet.Cells[1, 5].Value = "Количество просмотренных документов";
                worksheet.Cells[1, 6].Value = "Количество выгруженных из Системы документов";

                startCol.Width = 7;
                directionlCol.Width = 15;
                categoryCol.Width = 15;
                dateCol.Width = 40;
                secondDateCol.Width = 40;
                thirdDateCol.Width = 40;
                fourthBDateCol.Width = 40;
                counterpartyCol.Width = 53;
                nameCol.Width = 58;

                worksheet.TabColor = Color.Green;

                int documentNumber = 1;
                int rowNumber = 2;

                foreach (var item in reportRows)
                {
                    worksheet.Cells[rowNumber, 1].Value = documentNumber;
                    worksheet.Cells[rowNumber, 2].Value = item.CategoryName;
                    worksheet.Cells[rowNumber, 3].Value = item.SubcategoryName;
                    worksheet.Cells[rowNumber, 4].Value = item.UploadedDocumentsCount;
                    worksheet.Cells[rowNumber, 5].Value = item.PreviewedDocumentsCount;
                    worksheet.Cells[rowNumber, 6].Value = item.DownloadedDocumentsCount;

                    var currentRow = worksheet.Row(rowNumber);
                    currentRow.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    currentRow.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    rowNumber++;
                    documentNumber++;
                }

                excel.SaveAs(ms);
            }

            var stream = new MemoryStream(ms.ToArray());
            return File(stream, "application/octet-stream", "Отчет, тип 2.xlsx");

        }

        [HttpGet("report/type/3")]
        public async Task<ActionResult> GetReport3(CancellationToken ct)
        {
            var allEvents = await _database.DocumentActionEventEntities
                .OrderBy(p => p.CreatedOn)
                .ToArrayAsync(ct);

            var allCategoriesFromDb = _database.Categories
                .Include(p => p.Subcategories)
                .AsNoTracking()
                .Where(p => !p.IsDeleted)
                .ToArray();

            var allDocuments = await _database.DocumentSubcategoryRelations
                .Include(p => p.Subcategory)
                    .ThenInclude(p => p.ParentCategory)
                .Include(p => p.Document)
                .Where(p => !p.IsDeleted)
                .ToArrayAsync(ct);

            var reportRows = new List<ReportType3Row>();

            foreach (var row in allDocuments)
            {
                reportRows.Add(new ReportType3Row
                {
                    CategoryName = allCategoriesFromDb.SingleOrDefault(p => p.Id == row.Subcategory.ParentCategoryId).NameKz,
                    DocumentName = row.Document.NameKz,
                    DownloadedDocumentsCount = allEvents.Count(p => 
                        p.DocumentId == row.DocumentId && p.SubcategoryId == row.SubcategoryId &&
                        p.EventType == ActionEventType.Downloaded),
                    PreviewedDocumentsCount = allEvents.Count(p =>
                        p.DocumentId == row.DocumentId && p.SubcategoryId == row.SubcategoryId &&
                        p.EventType == ActionEventType.Viewed),
                    SubcategoryName = row.Subcategory.NameKz,
                });
            }

            using var ms = new MemoryStream();
            using (var excel = new ExcelPackage())
            {
                var worksheet = excel.Workbook.Worksheets.Add(DateTime.Now.ToString("dd.MM.yyyy"));
                worksheet.DefaultColWidth = 30;
                worksheet.Cells.Style.WrapText = true;
                var firstRow = worksheet.Row(1);

                firstRow.Style.Font.Bold = true;
                firstRow.Height = 30;
                firstRow.Style.Font.Color.SetColor(Color.Black);
                firstRow.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                firstRow.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                var startCol = worksheet.Column(1);
                var directionlCol = worksheet.Column(2);
                var categoryCol = worksheet.Column(3);
                var secondDateCol = worksheet.Column(14);
                var thirdDateCol = worksheet.Column(15);
                var dateCol = worksheet.Column(10);
                var fourthBDateCol = worksheet.Column(11);
                var counterpartyCol = worksheet.Column(12);
                var nameCol = worksheet.Column(4);

                worksheet.Cells[1, 1].Value = "#";
                worksheet.Cells[1, 2].Value = "Наименование категории";
                worksheet.Cells[1, 3].Value = "Наименование подкатегории";
                worksheet.Cells[1, 4].Value = "Наименование документа";
                worksheet.Cells[1, 5].Value = "Количество просмотренных документов";
                worksheet.Cells[1, 6].Value = "Количество выгруженных из Системы документов";

                startCol.Width = 7;
                directionlCol.Width = 15;
                categoryCol.Width = 15;
                dateCol.Width = 40;
                secondDateCol.Width = 40;
                thirdDateCol.Width = 40;
                fourthBDateCol.Width = 40;
                counterpartyCol.Width = 53;
                nameCol.Width = 58;

                worksheet.TabColor = Color.Green;

                int documentNumber = 1;
                int rowNumber = 2;

                foreach (var item in reportRows)
                {
                    worksheet.Cells[rowNumber, 1].Value = documentNumber;
                    worksheet.Cells[rowNumber, 2].Value = item.CategoryName;
                    worksheet.Cells[rowNumber, 3].Value = item.SubcategoryName;
                    worksheet.Cells[rowNumber, 4].Value = item.DocumentName;
                    worksheet.Cells[rowNumber, 5].Value = item.PreviewedDocumentsCount;
                    worksheet.Cells[rowNumber, 6].Value = item.DownloadedDocumentsCount;

                    var currentRow = worksheet.Row(rowNumber);
                    currentRow.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    currentRow.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    rowNumber++;
                    documentNumber++;
                }

                excel.SaveAs(ms);
            }

            var stream = new MemoryStream(ms.ToArray());
            return File(stream, "application/octet-stream", "Отчет, тип 3.xlsx");
        }
    }
}
