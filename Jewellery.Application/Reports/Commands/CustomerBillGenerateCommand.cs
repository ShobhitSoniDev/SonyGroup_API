using SelectPdf;
using Jewellery.Application.Master.Interfaces;
using Jewellery.Application.Master.Models;
using Jewellery.Application.Services.Interfaces;
using Jewellery.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Jewellery.Domain.Entities.CustomerBillGenerateModel;


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

        // ── No IConverter injection needed — SelectPdf is fully managed ───────
        public CustomerBillGenerateCommandHandler(
            IReportsRepository customerRepository,
            IBlobStorageService blobStorageService)
        {
            _reportsRepository = customerRepository;
            _blobStorageService = blobStorageService;
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

                // ── Step 2 : Build HTML ───────────────────────────────────────
                string htmlBill = request.Language switch
                {
                    BillLanguage.English => GenerateHtmlBill_English(shop, customer, bill, billNo),
                    BillLanguage.Hindi => GenerateHtmlBill_Hindi(shop, customer, bill, billNo),
                    BillLanguage.HindiEnglishMix => GenerateHtmlBill_Mix(shop, customer, bill, billNo),
                    _ => GenerateHtmlBill_Mix(shop, customer, bill, billNo)
                };

                // ── Step 3 : HTML → PDF (pure managed, no native DLL) ─────────
                byte[] pdfBytes = ConvertHtmlToPdf(htmlBill);

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
                return new ResponseModel { Code = 0, Message = ex.Message };
            }
        }

        // ═════════════════════════════════════════════════════════════════════
        // PDF GENERATION — SelectPdf Community Edition
        // Pure managed .NET — no libwkhtmltox, no Chrome, no native DLL.
        // Works on Azure App Service (Windows & Linux), local, Docker.
        // NuGet: Install-Package SelectPdf
        // ═════════════════════════════════════════════════════════════════════
        private static byte[] ConvertHtmlToPdf(string html)
        {
            // HtmlToPdf converter is thread-safe and stateless — create per call
            var converter = new HtmlToPdf();

            // Page settings
            converter.Options.PdfPageSize = PdfPageSize.A4;
            converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
            converter.Options.MarginTop = 10;
            converter.Options.MarginBottom = 10;
            converter.Options.MarginLeft = 10;
            converter.Options.MarginRight = 10;

            // Rendering quality
            converter.Options.WebPageWidth = 720;   // match your HTML max-width
            converter.Options.WebPageHeight = 0;     // 0 = auto height
            converter.Options.RenderingEngine = RenderingEngine.WebKit;

            // Convert HTML string → SelectPdf document → byte[]
            PdfDocument doc = converter.ConvertHtmlString(html);

            using var ms = new MemoryStream();
            doc.Save(ms);
            doc.Close();

            return ms.ToArray();
        }

        // ── Language-specific HTML Wrappers ───────────────────────────────────
        private static string GenerateHtmlBill_English(
            ShopInfo shop, CustomerInfo customer, BillInfo bill, string billNo)
        {
            return GenerateHtmlBill(shop, customer, bill, billNo, new BillLabels
            {
                HtmlLang = "en",
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
                FooterTagline = "Thank you for your trust - Visit us again",
                PrintBtn = ""
            });
        }

        private static string GenerateHtmlBill_Hindi(
            ShopInfo shop, CustomerInfo customer, BillInfo bill, string billNo)
        {
            return GenerateHtmlBill(shop, customer, bill, billNo, new BillLabels
            {
                HtmlLang = "hi",
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
                FooterTagline = "आपके विश्वास के लिए धन्यवाद - पुनः पधारें",
                PrintBtn = ""
            });
        }

        private static string GenerateHtmlBill_Mix(
            ShopInfo shop, CustomerInfo customer, BillInfo bill, string billNo)
        {
            return GenerateHtmlBill(shop, customer, bill, billNo, new BillLabels
            {
                HtmlLang = "hi",
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
                FooterTagline = "Shukriya aapke vishwas ke liye | आपके विश्वास के लिए धन्यवाद",
                PrintBtn = ""
            });
        }

        // ── Core HTML Generator ───────────────────────────────────────────────
        // NOTE: SelectPdf WebKit engine does not support CSS variables (var(--x))
        //       or @import Google Fonts. All styles are written as plain values.
        private static string GenerateHtmlBill(
            ShopInfo shop, CustomerInfo customer, BillInfo bill, string billNo, BillLabels L)
        {
            decimal newTotal = 0, oldTotal = 0;
            foreach (var i in bill.NewItems) newTotal += i.Price;
            foreach (var i in bill.OldItems) oldTotal += i.Price;
            decimal netAmount = newTotal - oldTotal;

            var sb = new StringBuilder();

            sb.Append($@"<!DOCTYPE html>
<html lang='{L.HtmlLang}'>
<head>
<meta charset='UTF-8'>
<title>Bill - {shop.ShopName}</title>
<style>
  * {{ margin:0; padding:0; box-sizing:border-box; }}
  body {{
    background: #E8DFC8;
    font-family: Arial, 'Noto Sans Devanagari', sans-serif;
    padding: 20px;
    font-size: 13px;
    color: #1A1208;
  }}
  .wrap {{
    width: 100%;
    max-width: 700px;
    margin: 0 auto;
    background: #FBF6EC;
    border: 2px solid #C9A84C;
  }}
  .hdr {{
    background: #1A1208;
    color: #D4A017;
    text-align: center;
    padding: 22px 20px 16px;
    border-bottom: 3px solid #C9A84C;
  }}
  .shop-name {{
    font-size: 22px;
    font-weight: bold;
    letter-spacing: 3px;
    text-transform: uppercase;
  }}
  .ornament {{
    color: #C9A84C;
    font-size: 14px;
    margin: 5px 0;
    letter-spacing: 6px;
  }}
  .shop-sub {{
    font-size: 11px;
    color: #C9A84C;
    letter-spacing: 1px;
    margin-top: 4px;
  }}
  .meta {{
    display: table;
    width: 100%;
    padding: 14px 24px;
    background: #F5EDD8;
    border-bottom: 1px solid #C9A84C;
  }}
  .meta-cell {{
    display: table-cell;
    vertical-align: top;
    font-size: 12px;
    color: #555;
    line-height: 1.7;
    width: 33%;
  }}
  .meta-cell.center {{ text-align: center; }}
  .meta-cell.right  {{ text-align: right; }}
  .meta-cell strong {{ color: #1A1208; font-size: 13px; display: block; }}
  .lbl {{
    font-size: 10px;
    text-transform: uppercase;
    letter-spacing: 1.5px;
    color: #B8860B;
    font-weight: bold;
    margin-bottom: 2px;
  }}
  .sec {{ padding: 16px 24px 4px; }}
  .sec-title {{
    font-size: 11px;
    font-weight: bold;
    letter-spacing: 2px;
    text-transform: uppercase;
    color: #B8860B;
    border-bottom: 1px solid #C9A84C;
    padding-bottom: 5px;
    margin-bottom: 10px;
  }}
  .sec-title-old {{
    font-size: 11px;
    font-weight: bold;
    letter-spacing: 2px;
    text-transform: uppercase;
    color: #8B2020;
    border-bottom: 1px solid #E0A0A0;
    padding-bottom: 5px;
    margin-bottom: 10px;
  }}
  table {{ width: 100%; border-collapse: collapse; margin-bottom: 14px; }}
  thead th {{
    font-size: 10px;
    text-transform: uppercase;
    letter-spacing: 1px;
    color: #888;
    font-weight: bold;
    padding: 5px 7px;
    text-align: left;
    border-bottom: 1px solid #DDD;
  }}
  thead th.right {{ text-align: right; }}
  tbody tr {{ border-bottom: 1px dashed #E5DCC5; }}
  tbody td {{
    padding: 8px 7px;
    font-size: 12px;
    color: #1A1208;
    vertical-align: middle;
  }}
  tbody td.right {{ text-align: right; font-weight: bold; }}
  tbody td.red   {{ text-align: right; font-weight: bold; color: #8B2020; }}
  .iname {{ font-weight: bold; }}
  .summ {{
    margin: 4px 24px 18px;
    background: #F5EDD8;
    border: 1px solid #C9A84C;
    padding: 12px 18px;
  }}
  .sr {{
    display: table;
    width: 100%;
    padding: 4px 0;
    font-size: 12px;
    color: #555;
    border-bottom: 1px dashed #DDD;
  }}
  .sr-label {{ display: table-cell; }}
  .sr-value {{ display: table-cell; text-align: right; }}
  .sr.net {{
    font-size: 14px;
    font-weight: bold;
    color: #1A1208;
    border-top: 2px solid #C9A84C;
    border-bottom: none;
    margin-top: 4px;
    padding-top: 8px;
  }}
  .sr.net .sr-value {{ color: #B8860B; }}
  .ded {{ color: #8B2020; }}
  .ftr {{
    background: #1A1208;
    color: #C9A84C;
    text-align: center;
    padding: 14px 20px;
    font-size: 11px;
    letter-spacing: 1px;
  }}
  .ftr .tag {{ font-weight: bold; font-size: 12px; margin-bottom: 4px; }}
  .no-print {{ margin-top: 18px; text-align: center; }}
  .pbtn {{
    background: #B8860B;
    color: #1A1208;
    border: none;
    padding: 9px 28px;
    font-size: 13px;
    font-weight: bold;
    letter-spacing: 2px;
    cursor: pointer;
    text-transform: uppercase;
  }}
  @media print {{
    body {{ background: white; padding: 0; }}
    .no-print {{ display: none; }}
    .wrap {{ box-shadow: none; border: 1px solid #999; max-width: 100%; }}
  }}
</style>
</head>
<body>
<div class='wrap'>

  <div class='hdr'>
    <div class='shop-name'>{shop.ShopName}</div>
    <div class='ornament'>* * *</div>
    <div class='shop-sub'>{shop.Phone} | {L.SubTitle}</div>
  </div>

  <div class='meta'>
    <div class='meta-cell'>
      <div class='lbl'>{L.CustomerLabel}</div>
      <strong>{customer.Name}</strong>
      {customer.Phone}
    </div>
    <div class='meta-cell center'>
      <div class='lbl'>{L.BillNoLabel}</div>
      <strong>{billNo}</strong>
    </div>
    <div class='meta-cell right'>
      <div class='lbl'>{L.DateLabel}</div>
      <strong>{bill.Date:dd MMM yyyy}</strong>
    </div>
  </div>
");

            // ── New items section ─────────────────────────────────────────────
            if (bill.NewItems.Count > 0)
            {
                sb.Append($@"  <div class='sec'>
    <div class='sec-title'>{L.NewSectionTitle}</div>
    <table>
      <thead>
        <tr>
          <th>{L.ColItem}</th>
          <th>{L.ColWeight}</th>
          <th class='right'>{L.ColAmount}</th>
        </tr>
      </thead>
      <tbody>
");
                foreach (var item in bill.NewItems)
                    sb.Append($"        <tr><td><div class='iname'>{item.Name}</div></td><td>{item.Weight:0.##} gm</td><td class='right'>Rs.{item.Price:N0}</td></tr>\n");

                sb.Append("      </tbody>\n    </table>\n  </div>\n");
            }

            // ── Old items section ─────────────────────────────────────────────
            if (bill.OldItems.Count > 0)
            {
                sb.Append($@"  <div class='sec'>
    <div class='sec-title-old'>{L.OldSectionTitle}</div>
    <table>
      <thead style='background:#FFF5F5'>
        <tr>
          <th style='color:#8B2020'>{L.ColItem}</th>
          <th style='color:#8B2020'>{L.ColWeight}</th>
          <th class='right' style='color:#8B2020'>{L.ColAmount}</th>
        </tr>
      </thead>
      <tbody>
");
                foreach (var item in bill.OldItems)
                    sb.Append($"        <tr><td><div class='iname'>{item.Name}</div></td><td>{item.Weight:0.##} gm</td><td class='red'>Rs.{item.Price:N0}</td></tr>\n");

                sb.Append("      </tbody>\n    </table>\n  </div>\n");
            }

            // ── Summary ───────────────────────────────────────────────────────
            sb.Append($@"  <div class='summ'>
    <div class='sr'>
      <span class='sr-label'>{L.SummaryNew}</span>
      <span class='sr-value'>Rs.{newTotal:N0}</span>
    </div>
");
            if (oldTotal > 0)
                sb.Append($@"    <div class='sr'>
      <span class='sr-label'>{L.SummaryOld}</span>
      <span class='sr-value ded'>- Rs.{oldTotal:N0}</span>
    </div>
");

            sb.Append($@"    <div class='sr net'>
      <span class='sr-label'>{L.SummaryNet}</span>
      <span class='sr-value'>Rs.{netAmount:N0}</span>
    </div>
  </div>

  <div class='ftr'>
    <div class='tag'>{L.FooterTagline}</div>
    <div>{shop.ShopName} | {shop.Phone}</div>
  </div>

</div>

</body></html>");

            return sb.ToString();
        }

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
}
