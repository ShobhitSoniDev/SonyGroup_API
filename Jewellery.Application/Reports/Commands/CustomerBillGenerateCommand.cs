using Jewellery.Application.Master.Interfaces;
using Jewellery.Application.Master.Models;
using Jewellery.Application.Services.Interfaces;
using Jewellery.Domain.Entities;
using MediatR;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Jewellery.Domain.Entities.CustomerBillGenerateModel;
// Fix: Unit ambiguous reference between QuestPDF.Infrastructure.Unit and MediatR.Unit
using Unit = QuestPDF.Infrastructure.Unit;

namespace Jewellery.Application.Master.Commands
{
    public enum BillLanguage
    {
        English = 1,
        Hindi = 2,
        HindiEnglishMix = 3
    }

    public class CustomerBillGenerateCommand : IRequest<ResponseModel>
    {
        public string? CustomerCode { get; set; }
        public string Description { get; set; }
        public BillLanguage Language { get; set; } = BillLanguage.HindiEnglishMix;
    }

    public class CustomerBillGenerateCommandHandler
        : IRequestHandler<CustomerBillGenerateCommand, ResponseModel>
    {
        private readonly IReportsRepository _reportsRepository;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IErrorLogRepository _errorLogRepository;
        public CustomerBillGenerateCommandHandler(
            IReportsRepository customerRepository,
            IBlobStorageService blobStorageService,
            IErrorLogRepository errorLogRepository)
        {
            _reportsRepository = customerRepository;
            _blobStorageService = blobStorageService;

            // QuestPDF Community license — free for commercial use
            QuestPDF.Settings.License = LicenseType.Community;
            _errorLogRepository = errorLogRepository;
        }

        public async Task<ResponseModel> Handle(
            CustomerBillGenerateCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // ── Step 1 : Fetch bill header data from DB ───────────────────
                var billGenerateModel = new BillGenerateHistoryModel
                {
                    TypeId = 1,
                    BillGenerateId = null,
                    CustomerCode = request.CustomerCode,
                    BillNo = "",
                    FilePath = "",
                    Description = request.Description,
                    LanguageType = (int)request.Language,
                };

                dynamic getcustomerdetail = await _reportsRepository
                    .BillGenerateHistoryManageAsync(billGenerateModel);

                IDictionary<string, object> row = getcustomerdetail;

                string shopRaw = row.ContainsKey("ShopDetail")
                    ? row["ShopDetail"]?.ToString() ?? "" : "";

                string customerRaw = row.ContainsKey("CustomerDetail")
                    ? row["CustomerDetail"]?.ToString() ?? "" : "";

                string billNo = row.ContainsKey("BillNo")
                    ? row["BillNo"]?.ToString() ?? "" : "";

                string billBlobFolder = row.ContainsKey("BillBlobFolder")
                    ? row["BillBlobFolder"]?.ToString() ?? "" : "";

                var shop = ShopInfo.Parse(shopRaw);
                var customer = CustomerInfo.Parse(customerRaw);
                var bill = ParseBill(request.Description);

                // ── Step 2 : Get labels based on language ─────────────────────
                BillLabels labels = request.Language switch
                {
                    BillLanguage.English => GetLabels_English(),
                    BillLanguage.Hindi => GetLabels_Hindi(),
                    BillLanguage.HindiEnglishMix => GetLabels_Mix(),
                    _ => GetLabels_Mix()
                };

                // ── Step 3 : Generate PDF using QuestPDF ──────────────────────
                // Pure managed .NET — no native DLL, no browser, no binary download
                // Works on: Local Windows, Azure App Service (Windows & Linux), Docker
                byte[] pdfBytes = GeneratePdf(shop, customer, bill, billNo, labels);

                // ── Step 4 : Upload to blob ───────────────────────────────────
                var fileName = $"{Guid.NewGuid()}.pdf";

                var uploadResult = await _blobStorageService.UploadFileAsync(
                    null, fileName, billBlobFolder, 0, 1, 0, pdfBytes, "application/pdf");

                if (!uploadResult.Success)
                    return new ResponseModel { Code = 0, Message = "Upload failed.", Data = null };

                // ── Step 5 : Save bill history ────────────────────────────────
                var billGenerateSave = new BillGenerateHistoryModel
                {
                    TypeId = 2,
                    BillGenerateId = null,
                    CustomerCode = request.CustomerCode,
                    BillNo = billNo,
                    FilePath = billBlobFolder + "/" + fileName,
                    Description = request.Description,
                    LanguageType = (int)request.Language,
                };

                await _reportsRepository.BillGenerateHistoryManageAsync(billGenerateSave);

                return new ResponseModel
                {
                    Code = 1,
                    Message = "SUCCESS",
                    Data = uploadResult.FileUrl
                };
            }
            catch (Exception ex)
            {
                var stackTrace = new StackTrace(ex, true);
                var frame = stackTrace.GetFrame(0);

                int? lineNumber = frame?.GetFileLineNumber();
                string? stackTraceText = ex.StackTrace;
                var errorLog = new ErrorLog
                {
                    ApiName = "CustomerBillGenerateCommand",
                    ErrorMessage = ex.Message,
                    StackTrace = stackTraceText,
                    LineNumber = lineNumber ?? 0,
                    CreatedDate = DateTime.Now
                };
                // ✅ Save Log in DB (via Infrastructure)
                _errorLogRepository.SaveErrorAsync(errorLog);
                return new ResponseModel
                {
                    Code = 0,
                    Message = "Something went wrong. Please try again later."
                };
            }
        }

        // ═════════════════════════════════════════════════════════════════════
        // PDF GENERATION — QuestPDF Community Edition
        // Pure managed .NET — no native DLL, no browser, no binary downloads.
        // Works on: Local Windows, Azure App Service (Windows & Linux), Docker.
        // NuGet: Install-Package QuestPDF
        // ═════════════════════════════════════════════════════════════════════
        private static byte[] GeneratePdf(
            ShopInfo shop, CustomerInfo customer, BillInfo bill, string billNo, BillLabels L)
        {
            decimal newTotal = bill.NewItems.Sum(i => i.Price);
            decimal oldTotal = bill.OldItems.Sum(i => i.Price);
            decimal netAmount = newTotal - oldTotal;

            return Document.Create(doc =>
            {
                doc.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(10, Unit.Millimetre);
                    page.DefaultTextStyle(t =>
                        t.FontFamily("Arial").FontSize(11).FontColor("#1A1208"));

                    page.Content().Column(col =>
                    {
                        // ── HEADER ────────────────────────────────────────────
                        col.Item()
                           .Background("#1A1208")
                           .BorderBottom(3).BorderColor("#C9A84C")
                           .Padding(18)
                           .Column(hdr =>
                           {
                               hdr.Item().AlignCenter()
                                  .Text(shop.ShopName)
                                  .FontSize(20).Bold()
                                  .FontColor("#D4A017")
                                  .LetterSpacing(0.15f);

                               hdr.Item().AlignCenter().PaddingVertical(4)
                                  .Text("* * *")
                                  .FontSize(12).FontColor("#C9A84C");

                               hdr.Item().AlignCenter()
                                  .Text($"{shop.Phone}  |  {L.SubTitle}")
                                  .FontSize(10).FontColor("#C9A84C");
                           });

                        // ── META ROW (Customer | Bill No | Date) ──────────────
                        col.Item()
                           .Background("#F5EDD8")
                           .BorderBottom(1).BorderColor("#C9A84C")
                           .Padding(12)
                           .Row(row =>
                           {
                               // Customer
                               row.RelativeItem().Column(c =>
                               {
                                   c.Item().Text(L.CustomerLabel)
                                    .FontSize(9).FontColor("#B8860B").Bold();
                                   c.Item().Text(customer.Name)
                                    .Bold().FontSize(13);
                                   c.Item().Text(customer.Phone)
                                    .FontSize(10).FontColor("#555555");
                               });

                               // Bill No
                               row.RelativeItem().AlignCenter().Column(c =>
                               {
                                   c.Item().AlignCenter().Text(L.BillNoLabel)
                                    .FontSize(9).FontColor("#B8860B").Bold();
                                   c.Item().AlignCenter().Text(billNo)
                                    .Bold().FontSize(13);
                               });

                               // Date
                               row.RelativeItem().AlignRight().Column(c =>
                               {
                                   c.Item().AlignRight().Text(L.DateLabel)
                                    .FontSize(9).FontColor("#B8860B").Bold();
                                   c.Item().AlignRight()
                                    .Text(bill.Date.ToString("dd MMM yyyy"))
                                    .Bold().FontSize(13);
                               });
                           });

                        // ── NEW ITEMS SECTION ─────────────────────────────────
                        if (bill.NewItems.Count > 0)
                        {
                            col.Item().Padding(12).Column(sec =>
                            {
                                // Section title
                                sec.Item()
                                   .BorderBottom(1).BorderColor("#C9A84C")
                                   .PaddingBottom(5)
                                   .Text(L.NewSectionTitle)
                                   .FontSize(10).Bold().FontColor("#B8860B");

                                // Table
                                sec.Item().PaddingTop(6).Table(table =>
                                {
                                    table.ColumnsDefinition(c =>
                                    {
                                        c.RelativeColumn(4); // Item name
                                        c.RelativeColumn(2); // Weight
                                        c.RelativeColumn(2); // Amount
                                    });

                                    // Header row
                                    table.Header(h =>
                                    {
                                        h.Cell().BorderBottom(1).BorderColor("#DDDDDD")
                                         .PaddingVertical(5)
                                         .Text(L.ColItem)
                                         .FontSize(9).FontColor("#888888").Bold();

                                        h.Cell().BorderBottom(1).BorderColor("#DDDDDD")
                                         .PaddingVertical(5)
                                         .Text(L.ColWeight)
                                         .FontSize(9).FontColor("#888888").Bold();

                                        h.Cell().BorderBottom(1).BorderColor("#DDDDDD")
                                         .AlignRight().PaddingVertical(5)
                                         .Text(L.ColAmount)
                                         .FontSize(9).FontColor("#888888").Bold();
                                    });

                                    // Data rows
                                    foreach (var item in bill.NewItems)
                                    {
                                        table.Cell()
                                             .BorderBottom(1).BorderColor("#E5DCC5")
                                             .PaddingVertical(7)
                                             .Text(item.Name).Bold();

                                        table.Cell()
                                             .BorderBottom(1).BorderColor("#E5DCC5")
                                             .PaddingVertical(7)
                                             .Text($"{item.Weight:0.##} gm");

                                        table.Cell()
                                             .BorderBottom(1).BorderColor("#E5DCC5")
                                             .AlignRight().PaddingVertical(7)
                                             .Text($"Rs.{item.Price:N0}").Bold();
                                    }
                                });
                            });
                        }

                        // ── OLD ITEMS SECTION ─────────────────────────────────
                        if (bill.OldItems.Count > 0)
                        {
                            col.Item().Padding(12).Column(sec =>
                            {
                                // Section title
                                sec.Item()
                                   .BorderBottom(1).BorderColor("#E0A0A0")
                                   .PaddingBottom(5)
                                   .Text(L.OldSectionTitle)
                                   .FontSize(10).Bold().FontColor("#8B2020");

                                // Table
                                sec.Item().PaddingTop(6).Table(table =>
                                {
                                    table.ColumnsDefinition(c =>
                                    {
                                        c.RelativeColumn(4);
                                        c.RelativeColumn(2);
                                        c.RelativeColumn(2);
                                    });

                                    table.Header(h =>
                                    {
                                        h.Cell().BorderBottom(1).BorderColor("#E0A0A0")
                                         .PaddingVertical(5)
                                         .Text(L.ColItem)
                                         .FontSize(9).FontColor("#8B2020").Bold();

                                        h.Cell().BorderBottom(1).BorderColor("#E0A0A0")
                                         .PaddingVertical(5)
                                         .Text(L.ColWeight)
                                         .FontSize(9).FontColor("#8B2020").Bold();

                                        h.Cell().BorderBottom(1).BorderColor("#E0A0A0")
                                         .AlignRight().PaddingVertical(5)
                                         .Text(L.ColAmount)
                                         .FontSize(9).FontColor("#8B2020").Bold();
                                    });

                                    foreach (var item in bill.OldItems)
                                    {
                                        table.Cell()
                                             .BorderBottom(1).BorderColor("#F5CCCC")
                                             .PaddingVertical(7)
                                             .Text(item.Name).Bold();

                                        table.Cell()
                                             .BorderBottom(1).BorderColor("#F5CCCC")
                                             .PaddingVertical(7)
                                             .Text($"{item.Weight:0.##} gm");

                                        table.Cell()
                                             .BorderBottom(1).BorderColor("#F5CCCC")
                                             .AlignRight().PaddingVertical(7)
                                             .Text($"Rs.{item.Price:N0}")
                                             .Bold().FontColor("#8B2020");
                                    }
                                });
                            });
                        }

                        // ── SUMMARY BOX ───────────────────────────────────────
                        col.Item()
                           .PaddingHorizontal(12).PaddingVertical(4)
                           .Border(1).BorderColor("#C9A84C")
                           .Background("#F5EDD8")
                           .Padding(14)
                           .Column(summ =>
                           {
                               // New total row
                               summ.Item()
                                   .BorderBottom(1).BorderColor("#DDDDDD")
                                   .PaddingVertical(5)
                                   .Row(r =>
                                   {
                                       r.RelativeItem().Text(L.SummaryNew).FontSize(12);
                                       r.RelativeItem().AlignRight()
                                        .Text($"Rs.{newTotal:N0}").FontSize(12);
                                   });

                               // Old deduction row (only if old items exist)
                               if (oldTotal > 0)
                               {
                                   summ.Item()
                                       .BorderBottom(1).BorderColor("#DDDDDD")
                                       .PaddingVertical(5)
                                       .Row(r =>
                                       {
                                           r.RelativeItem().Text(L.SummaryOld).FontSize(12);
                                           r.RelativeItem().AlignRight()
                                            .Text($"- Rs.{oldTotal:N0}")
                                            .FontSize(12).FontColor("#8B2020");
                                       });
                               }

                               // Net payable row
                               summ.Item()
                                   .BorderTop(2).BorderColor("#C9A84C")
                                   .PaddingTop(8)
                                   .Row(r =>
                                   {
                                       r.RelativeItem()
                                        .Text(L.SummaryNet).Bold().FontSize(14);
                                       r.RelativeItem().AlignRight()
                                        .Text($"Rs.{netAmount:N0}")
                                        .Bold().FontSize(14).FontColor("#B8860B");
                                   });
                           });

                        // ── FOOTER ────────────────────────────────────────────
                        col.Item()
                           .Background("#1A1208")
                           .Padding(14)
                           .Column(ftr =>
                           {
                               ftr.Item().AlignCenter()
                                  .Text(L.FooterTagline)
                                  .Bold().FontSize(12).FontColor("#C9A84C");

                               ftr.Item().AlignCenter().PaddingTop(4)
                                  .Text($"{shop.ShopName}  |  {shop.Phone}")
                                  .FontSize(10).FontColor("#C9A84C");
                           });
                    });
                });
            }).GeneratePdf();
        }

        // ── Label Sets ────────────────────────────────────────────────────────
        private static BillLabels GetLabels_English() => new BillLabels
        {
            SubTitle = "Fine Jewellery",
            CustomerLabel = "Customer",
            BillNoLabel = "Bill No.",
            DateLabel = "Date",
            NewSectionTitle = "New Jewellery",
            OldSectionTitle = "Old Jewellery (Return)",
            ColItem = "Item",
            ColWeight = "Weight (gm)",
            ColAmount = "Amount (Rs.)",
            SummaryNew = "New Jewellery Total",
            SummaryOld = "Old Jewellery (Deduction)",
            SummaryNet = "Net Payable Amount",
            FooterTagline = "Thank you for your trust - Visit us again"
        };

        private static BillLabels GetLabels_Hindi() => new BillLabels
        {
            SubTitle = "सोना चाँदी आभूषण",
            CustomerLabel = "ग्राहक",
            BillNoLabel = "बिल नंबर",
            DateLabel = "दिनांक",
            NewSectionTitle = "नए आभूषण",
            OldSectionTitle = "पुराने आभूषण (वापसी)",
            ColItem = "वस्तु",
            ColWeight = "वजन (ग्राम)",
            ColAmount = "राशि (Rs.)",
            SummaryNew = "नए आभूषण कुल",
            SummaryOld = "पुराने आभूषण (कटौती)",
            SummaryNet = "कुल देय राशि",
            FooterTagline = "आपके विश्वास के लिए धन्यवाद - पुनः पधारें"
        };

        private static BillLabels GetLabels_Mix() => new BillLabels
        {
            SubTitle = "Fine Jewellery | सोना चाँदी आभूषण",
            CustomerLabel = "Customer / ग्राहक",
            BillNoLabel = "Bill No. / बिल नं.",
            DateLabel = "Date / दिनांक",
            NewSectionTitle = "Naye Zewarat / नए आभूषण",
            OldSectionTitle = "Purane Zewarat / पुराने आभूषण (Wapsi)",
            ColItem = "Item / वस्तु",
            ColWeight = "Wajan / वजन (gm)",
            ColAmount = "Rashi / राशि (Rs.)",
            SummaryNew = "Naye Zewarat Kul / नए आभूषण कुल",
            SummaryOld = "Purane Zewarat (-) / पुराने आभूषण कटौती",
            SummaryNet = "Net Dene Wali Rashi / कुल देय राशि",
            FooterTagline = "Shukriya aapke vishwas ke liye | आपके विश्वास के लिए धन्यवाद"
        };

        // ── ParseBill ─────────────────────────────────────────────────────────
        private static BillInfo ParseBill(string description)
        {
            var bill = new BillInfo();
            bool isOldSection = false;

            if (string.IsNullOrWhiteSpace(description))
                return bill;

            var lines = description.Split(
                new[] { '\n', '\r' },
                StringSplitOptions.RemoveEmptyEntries);

            foreach (var rawLine in lines)
            {
                var line = rawLine.Trim();
                if (string.IsNullOrWhiteSpace(line)) continue;

                if (TryParseDate(line, out DateTime parsedDate))
                {
                    bill.Date = parsedDate;
                    continue;
                }

                if (IsSectionHeader(line, isNew: false)) { isOldSection = true; continue; }
                if (IsSectionHeader(line, isNew: true)) { isOldSection = false; continue; }

                JewelleryItem item = null;
                if (line.Contains('|')) item = TryParseItem(line, '|');
                else if (line.Contains('=')) item = TryParseItem(line, '=');

                if (item == null) continue;

                item.IsOld = isOldSection;
                if (isOldSection) bill.OldItems.Add(item);
                else bill.NewItems.Add(item);
            }

            return bill;
        }

        private static bool TryParseDate(string line, out DateTime result)
        {
            result = default;
            string datePart = line.StartsWith("Date:", StringComparison.OrdinalIgnoreCase)
                ? line[5..].Trim() : line;

            string[] formats = { "dd-MMM-yyyy", "d-MMM-yyyy", "dd/MM/yyyy", "dd-MM-yyyy" };
            return DateTime.TryParseExact(datePart, formats,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out result);
        }

        private static bool IsSectionHeader(string line, bool isNew)
        {
            if (line.Contains('=') || line.Contains('|')) return false;
            string lower = line.ToLower();
            return isNew
                ? lower.StartsWith("sale") || lower.StartsWith("new")
                : lower.StartsWith("old") || lower.StartsWith("purani");
        }

        private static JewelleryItem TryParseItem(string line, char sep)
        {
            var parts = line.Split(sep);
            if (parts.Length < 3) return null;

            var item = new JewelleryItem { Name = parts[0].Trim() };

            string weightStr = parts[1].Trim()
                .Replace("gm", "", StringComparison.OrdinalIgnoreCase).Trim();

            if (double.TryParse(weightStr, out double w)) item.Weight = w;
            if (decimal.TryParse(parts[2].Trim(), out decimal p)) item.Price = p;

            return item;
        }
    }

    // ── BillLabels DTO ────────────────────────────────────────────────────────
    // (HtmlLang and PrintBtn removed — not needed for QuestPDF)
    internal class BillLabels
    {
        public string SubTitle { get; set; }
        public string CustomerLabel { get; set; }
        public string BillNoLabel { get; set; }
        public string DateLabel { get; set; }
        public string NewSectionTitle { get; set; }
        public string OldSectionTitle { get; set; }
        public string ColItem { get; set; }
        public string ColWeight { get; set; }
        public string ColAmount { get; set; }
        public string SummaryNew { get; set; }
        public string SummaryOld { get; set; }
        public string SummaryNet { get; set; }
        public string FooterTagline { get; set; }
    }
}
