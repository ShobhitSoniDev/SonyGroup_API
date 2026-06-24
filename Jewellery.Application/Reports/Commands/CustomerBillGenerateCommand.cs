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

                string htmlBill = request.Language switch
                {
                    BillLanguage.English => GenerateHtmlBill_English(shop, customer, bill, billNo),
                    BillLanguage.Hindi => GenerateHtmlBill_Hindi(shop, customer, bill, billNo),
                    BillLanguage.HindiEnglishMix => GenerateHtmlBill_Mix(shop, customer, bill, billNo),
                    _ => GenerateHtmlBill_Mix(shop, customer, bill, billNo)
                };

                var pdfBytes = await GeneratePdfAsync(htmlBill);
                var fileName = $"{Guid.NewGuid()}.pdf";

                var uploadResult = await _blobStorageService.UploadFileAsync(
                    null, fileName, billBlobFolder, 0, 1, 0, pdfBytes, "application/pdf");

                if (!uploadResult.Success)
                    return new ResponseModel { Code = 0, Message = "Upload failed.", Data = null };

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

        // ── PDF Generation ────────────────────────────────────────────────────
        private static async Task<byte[]> GeneratePdfAsync(string htmlBill)
        {
            await new PuppeteerSharp.BrowserFetcher().DownloadAsync();

            await using var browser = await PuppeteerSharp.Puppeteer.LaunchAsync(
                new PuppeteerSharp.LaunchOptions { Headless = true });

            await using var page = await browser.NewPageAsync();
            await page.SetContentAsync(htmlBill);

            return await page.PdfDataAsync(new PuppeteerSharp.PdfOptions
            {
                Format = PuppeteerSharp.Media.PaperFormat.A4,
                PrintBackground = true
            });
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
                NewSectionTitle = "✦ New Jewellery",
                OldSectionTitle = "⟳ Old Jewellery (Return)",
                ColItem = "Item",
                ColWeight = "Weight (gm)",
                ColAmount = "Amount (₹)",
                SummaryNew = "New Jewellery Total",
                SummaryOld = "Old Jewellery (Deduction)",
                SummaryNet = "Net Payable Amount",
                FooterTagline = "Thank you for your trust — Visit us again",
                PrintBtn = "🖨️ Print Bill"
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
                NewSectionTitle = "✦ नए आभूषण",
                OldSectionTitle = "⟳ पुराने आभूषण (वापसी)",
                ColItem = "वस्तु",
                ColWeight = "वजन (ग्राम)",
                ColAmount = "राशि (₹)",
                SummaryNew = "नए आभूषण कुल",
                SummaryOld = "पुराने आभूषण (कटौती)",
                SummaryNet = "कुल देय राशि",
                FooterTagline = "आपके विश्वास के लिए धन्यवाद — पुनः पधारें",
                PrintBtn = "🖨️ बिल प्रिंट करें"
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
                NewSectionTitle = "✦ Naye Zewarat / नए आभूषण",
                OldSectionTitle = "⟳ Purane Zewarat / पुराने आभूषण (Wapsi)",
                ColItem = "Item / वस्तु",
                ColWeight = "Wajan / वजन (gm)",
                ColAmount = "Rashi / राशि (₹)",
                SummaryNew = "Naye Zewarat Kul / नए आभूषण कुल",
                SummaryOld = "Purane Zewarat (−) / पुराने आभूषण कटौती",
                SummaryNet = "Net Dene Wali Rashi / कुल देय राशि",
                FooterTagline = "Shukriya aapke vishwas ke liye | आपके विश्वास के लिए धन्यवाद",
                PrintBtn = "🖨️ Bill Print Karein / बिल प्रिंट करें"
            });
        }

        // ── Core HTML Generator ───────────────────────────────────────────────
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
<meta name='viewport' content='width=device-width, initial-scale=1.0'>
<title>Bill - {shop.ShopName}</title>
<style>
  @import url('https://fonts.googleapis.com/css2?family=Cinzel:wght@400;600;700&family=Lato:wght@300;400;700&family=Noto+Sans+Devanagari:wght@400;500;700&display=swap');
  *{{margin:0;padding:0;box-sizing:border-box}}
  :root{{--gold:#B8860B;--gold2:#D4A017;--dark:#1A1208;--cream:#FBF6EC;--light:#F5EDD8;--red:#8B2020;--border:#C9A84C}}
  body{{background:#E8DFC8;font-family:'Lato','Noto Sans Devanagari',sans-serif;display:flex;justify-content:center;padding:30px 16px;min-height:100vh}}
  .wrap{{width:100%;max-width:720px;background:var(--cream);border:2px solid var(--border);box-shadow:0 8px 40px rgba(0,0,0,.25);position:relative;overflow:hidden}}
  .wrap::before,.wrap::after{{content:'';position:absolute;width:60px;height:60px;border-color:var(--gold);border-style:solid}}
  .wrap::before{{top:8px;left:8px;border-width:2px 0 0 2px}}
  .wrap::after{{bottom:8px;right:8px;border-width:0 2px 2px 0}}
  .hdr{{background:linear-gradient(135deg,var(--dark) 0%,#3A2A0A 100%);color:var(--gold2);text-align:center;padding:28px 24px 20px;border-bottom:3px solid var(--border)}}
  .shop-name{{font-family:'Cinzel',serif;font-size:2rem;font-weight:700;letter-spacing:3px;text-transform:uppercase;text-shadow:0 2px 8px rgba(0,0,0,.4)}}
  .ornament{{color:var(--gold);font-size:1.2rem;margin:6px 0;letter-spacing:8px}}
  .shop-sub{{font-size:.82rem;color:#C9A84C;letter-spacing:1px;margin-top:4px}}
  .meta{{display:flex;justify-content:space-between;align-items:flex-start;padding:16px 28px;background:var(--light);border-bottom:1px solid var(--border);flex-wrap:wrap;gap:12px}}
  .mb{{font-size:.82rem;color:#555;line-height:1.7}}
  .mb strong{{color:var(--dark);font-size:.88rem;display:block}}
  .lbl{{font-size:.68rem;text-transform:uppercase;letter-spacing:1.5px;color:var(--gold);font-weight:700;margin-bottom:2px}}
  .sec{{padding:20px 28px 4px}}
  .sec-title{{font-family:'Cinzel',serif;font-size:.78rem;letter-spacing:2px;text-transform:uppercase;color:var(--gold);border-bottom:1px solid var(--border);padding-bottom:6px;margin-bottom:12px}}
  table{{width:100%;border-collapse:collapse;margin-bottom:16px}}
  thead th{{font-size:.72rem;text-transform:uppercase;letter-spacing:1px;color:#888;font-weight:700;padding:6px 8px;text-align:left;border-bottom:1px solid #DDD}}
  thead th:last-child{{text-align:right}}
  tbody tr{{border-bottom:1px dashed #E5DCC5}}
  tbody tr:last-child{{border-bottom:none}}
  tbody td{{padding:10px 8px;font-size:.9rem;color:var(--dark);vertical-align:middle}}
  tbody td:last-child{{text-align:right;font-weight:700;color:#3A2A0A}}
  .iname{{font-weight:700}}
  .summ{{margin:4px 28px 20px;background:var(--light);border:1px solid var(--border);padding:14px 20px}}
  .sr{{display:flex;justify-content:space-between;padding:5px 0;font-size:.88rem;color:#555;border-bottom:1px dashed #DDD}}
  .sr:last-child{{border-bottom:none}}
  .sr.net{{font-family:'Cinzel',serif;font-size:1.05rem;font-weight:700;color:var(--dark);border-top:2px solid var(--border);margin-top:4px;padding-top:10px}}
  .sr.net span:last-child{{color:var(--gold)}}
  .ded{{color:var(--red)!important}}
  .ftr{{background:var(--dark);color:#C9A84C;text-align:center;padding:16px 24px;font-size:.75rem;letter-spacing:1px}}
  .ftr .tag{{font-family:'Cinzel',serif;font-size:.8rem;margin-bottom:4px}}
  .no-print{{margin-top:20px;text-align:center}}
  .pbtn{{background:var(--gold);color:var(--dark);border:none;padding:10px 32px;font-family:'Cinzel',serif;font-size:.9rem;letter-spacing:2px;cursor:pointer;text-transform:uppercase}}
  .pbtn:hover{{background:var(--gold2)}}
  @media print{{body{{background:white;padding:0}}.no-print{{display:none}}.wrap{{box-shadow:none;border:1px solid #999;max-width:100%}}}}
</style>
</head>
<body>
<div class='wrap'>
  <div class='hdr'>
    <div class='shop-name'>{shop.ShopName}</div>
    <div class='ornament'>✦ ✦ ✦</div>
    <div class='shop-sub'>📞 {shop.Phone} &nbsp;|&nbsp; {L.SubTitle}</div>
  </div>
  <div class='meta'>
    <div class='mb'>
      <div class='lbl'>{L.CustomerLabel}</div>
      <strong>{customer.Name}</strong>📞 {customer.Phone}
    </div>
    <div class='mb' style='text-align:center'>
      <div class='lbl'>{L.BillNoLabel}</div>
      <strong>{billNo}</strong>
    </div>
    <div class='mb' style='text-align:right'>
      <div class='lbl'>{L.DateLabel}</div>
      <strong>{bill.Date:dd MMM yyyy}</strong>
    </div>
  </div>
");
            if (bill.NewItems.Count > 0)
            {
                sb.Append($@"  <div class='sec'>
    <div class='sec-title'>{L.NewSectionTitle}</div>
    <table>
      <thead><tr><th>{L.ColItem}</th><th>{L.ColWeight}</th><th>{L.ColAmount}</th></tr></thead>
      <tbody>
");
                foreach (var item in bill.NewItems)
                    sb.Append($"        <tr><td><div class='iname'>{item.Name}</div></td><td>{item.Weight:0.##} gm</td><td>₹{item.Price:N0}</td></tr>\n");

                sb.Append("      </tbody>\n    </table>\n  </div>\n");
            }

            if (bill.OldItems.Count > 0)
            {
                sb.Append($@"  <div class='sec'>
    <div class='sec-title' style='color:#8B2020;border-color:#E0A0A0'>{L.OldSectionTitle}</div>
    <table>
      <thead style='background:#FFF5F5'>
        <tr>
          <th style='color:#8B2020'>{L.ColItem}</th>
          <th style='color:#8B2020'>{L.ColWeight}</th>
          <th style='color:#8B2020'>{L.ColAmount}</th>
        </tr>
      </thead>
      <tbody>
");
                foreach (var item in bill.OldItems)
                    sb.Append($"        <tr><td><div class='iname'>{item.Name}</div></td><td>{item.Weight:0.##} gm</td><td style='color:#8B2020'>₹{item.Price:N0}</td></tr>\n");

                sb.Append("      </tbody>\n    </table>\n  </div>\n");
            }

            sb.Append($@"  <div class='summ'>
    <div class='sr'><span>{L.SummaryNew}</span><span>₹{newTotal:N0}</span></div>
");
            if (oldTotal > 0)
                sb.Append($"    <div class='sr'><span>{L.SummaryOld}</span><span class='ded'>− ₹{oldTotal:N0}</span></div>\n");

            sb.Append($@"    <div class='sr net'><span>{L.SummaryNet}</span><span>₹{netAmount:N0}</span></div>
  </div>
  <div class='ftr'>
    <div class='tag'>{L.FooterTagline}</div>
    <div>{shop.ShopName} &nbsp;|&nbsp; {shop.Phone}</div>
  </div>
</div>
<div class='no-print'>
  <button class='pbtn' onclick='window.print()'>{L.PrintBtn}</button>
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