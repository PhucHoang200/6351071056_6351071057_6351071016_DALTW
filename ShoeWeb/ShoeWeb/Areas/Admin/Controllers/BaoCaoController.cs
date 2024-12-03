using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using ShoeWeb.Data;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNet.Identity;
using ShoeWeb.App_Start;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using Microsoft.Ajax.Utilities;
using DocumentFormat.OpenXml.Vml;
using System.Diagnostics;
using Xceed.Words.NET;
using Spire.Doc;
using Document = Spire.Doc.Document;  // Alias để phân biệt với OpenXml
using Spire.Doc.Fields;
using System.Diagnostics;
using SpireDocTable = Spire.Doc.Table;  // Đặt alias cho Spire.Doc.Table
using SpireDocTableRow = Spire.Doc.TableRow;  // Đặt alias cho Spire.Doc.TableRow
using OpenXmlTable = DocumentFormat.OpenXml.Wordprocessing.Table;  // Đặt alias cho DocumentFormat.OpenXml.Wordprocessing.Table
using OpenXmlTableRow = DocumentFormat.OpenXml.Wordprocessing.TableRow;  // Đặt alias cho DocumentFormat.OpenXml.Wordprocessing.TableRow


namespace ShoeWeb.Areas.Admin.Controllers
{
    public class BaoCaoController : Controller
    {
        private readonly ApplicationDbContext _db;
        private ApplicationUserManager _userManager;

        public BaoCaoController()
        {
            _db = new ApplicationDbContext();
        }
        public BaoCaoController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }



        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [HttpGet]
        public async Task<ActionResult> ExportRevenueToPDF(DateTime fromDate, DateTime toDate)
        {
            // Lấy danh sách đơn hàng trong khoảng thời gian
            var orders = _db.Orders
                .Where(o => o.CreatedDate >= fromDate && o.CreatedDate <= toDate)
                .Select(o => new
                {
                    o.Code,
                    o.CreatedDate,
                    o.TotalAmount,
                    CustomerName = o.CustomerName,
                    CustomerPhone = o.Phone,
                    Products = o.OrderDetails.Select(od => new
                    {
                        Name = od.Product.productName,
                        Quantity = od.Quantity,
                        Price = od.Product.price,
                        UnitPrice = od.Price
                    }).ToList()
                }).ToList();

            // Tổng doanh thu
            var totalRevenue = orders.Sum(o => o.TotalAmount);

            // Đường dẫn template và file xuất
            string pathTemplate = Server.MapPath("~/BaoCao/Bao_Cao_Doanh_Thu_Template.docx");
            string pathExportDocx = Server.MapPath("~/BaoCao/Bao_Cao_Doanh_Thu_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".docx");
            string pathExportPdf = Server.MapPath("~/BaoCao/Bao_Cao_Doanh_Thu_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf");

            // Mở template
            Document document = new Document();
            document.LoadFromFile(pathTemplate);

            // Thay thế các placeholder tổng quan
            document.Replace("{NgayLap}", DateTime.Now.ToString("dd/MM/yyyy"), false, true);
            document.Replace("{fromDate}", fromDate.ToString("dd/MM/yyyy"), false, true);
            document.Replace("{toDate}", toDate.ToString("dd/MM/yyyy"), false, true);
            document.Replace("{totalRevenue}", totalRevenue.ToString("N0") , false, true);
            document.Replace("{Quantity}", orders.Count().ToString(), false, true);

            // Tạo dữ liệu cho bảng sản phẩm
            var productDetails = orders.SelectMany(o => o.Products)
                                       .GroupBy(p => p.Name)
                                       .Select(g => new
                                       {
                                           ProductName = g.Key,
                                           ProductQuantity = g.Sum(p => p.Quantity),
                                           ProductPrice = g.First().Price,
                                           ProductUnitPrice = g.Sum(p => p.Quantity * p.UnitPrice)
                                       }).ToList();

            // Lấy bảng đầu tiên trong tài liệu
            Spire.Doc.Table table = (Spire.Doc.Table)document.Sections[0].Tables[0];

            // Lấy hàng mẫu (giả sử hàng mẫu là hàng đầu tiên trong bảng)
            Spire.Doc.TableRow templateRow = table.Rows[1];

            // Lặp qua từng sản phẩm và thêm dữ liệu vào bảng
            foreach (var item in productDetails)
            {
                // Nhân bản hàng mẫu
                Spire.Doc.TableRow newRow = templateRow.Clone() as Spire.Doc.TableRow;

                // Điền dữ liệu vào các ô
                newRow.Cells[0].Paragraphs[0].Text = item.ProductName;
                newRow.Cells[1].Paragraphs[0].Text = item.ProductQuantity.ToString();
                newRow.Cells[2].Paragraphs[0].Text = item.ProductPrice.ToString("N0");
                newRow.Cells[3].Paragraphs[0].Text = item.ProductUnitPrice.ToString("N0");

                // Thêm hàng mới vào bảng
                table.Rows.Add(newRow);
            }

            // Xóa hàng mẫu (nếu không cần hiển thị trong kết quả)
            table.Rows.Remove(templateRow);


            // Thay thế doanh thu theo kênh bán hàng
            string salesChannels = "Online\t" + totalRevenue.ToString("N0") + " VNĐ\t100%\n" + "Cửa hàng\t0 VNĐ\t0%";
            document.Replace("{salesChannels}", salesChannels, false, true);

            // Lưu file Word mới
            document.SaveToFile(pathExportDocx, FileFormat.Docx);

            // Chuyển đổi file Word sang PDF
            Spire.Doc.Document wordDoc = new Spire.Doc.Document();
            wordDoc.LoadFromFile(pathExportDocx);
            wordDoc.SaveToFile(pathExportPdf, Spire.Doc.FileFormat.PDF);

            // Mở file PDF sau khi xuất
            Process.Start(pathExportPdf);

            // Trả về kết quả
            return RedirectToAction("Index", "ThongKe");
        }

        //    [HttpGet]
        //public ActionResult ExportRevenueToPDF(DateTime fromDate, DateTime toDate)
        //{
        //    if (fromDate > DateTime.UtcNow || toDate <= fromDate)
        //    {
        //        // Xử lý lỗi nếu cần
        //        ModelState.AddModelError("", "Ngày không hợp lệ.");
        //        return RedirectToAction("Index", "ThongKe");
        //    }

        //    // Lấy dữ liệu từ CSDL
        //var orders = _db.Orders
        //.Where(o => o.CreatedDate >= fromDate && o.CreatedDate <= toDate)
        //.Select(o => new
        //{
        //    o.Code,
        //    o.CreatedDate,
        //    o.TotalAmount,
        //    CustomerName = o.CustomerName,
        //    CustomerPhone = o.Phone,
        //    Products = o.OrderDetails.Select(od => new
        //    {
        //        Name = od.Product.productName,
        //        Quantity = od.Quantity,
        //        Price = od.Product.price,
        //        UnitPrice = od.Price,
        //    }).ToList()
        //}).ToList();





        //    // Tạo stream bộ nhớ để lưu file PDF
        //    var stream = new MemoryStream();
        //    var document = new Document(PageSize.A4);
        //    var writer = PdfWriter.GetInstance(document, stream);
        //    document.Open();

        //    // Tiêu đề báo cáo với Font tùy chỉnh
        //    var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20);
        //    var title = new Paragraph("BÁO CÁO DOANH THU", titleFont)
        //    {
        //        Alignment = Element.ALIGN_CENTER // Căn giữa
        //    };
        //    document.Add(title);

        //    var subtitleFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
        //    var subtitle = new Paragraph($"Từ ngày: {fromDate:dd/MM/yyyy} - Đến ngày: {toDate:dd/MM/yyyy}", subtitleFont)
        //    {
        //        Alignment = Element.ALIGN_CENTER // Căn giữa
        //    };
        //    subtitle.SpacingAfter = 20; // Khoảng cách sau đoạn văn
        //    document.Add(subtitle);

        //    // Tạo bảng chứa thông tin đơn hàng
        //    var table = new PdfPTable(8);
        //    table.WidthPercentage = 100;
        //    table.AddCell("Mã Đơn Hàng");
        //    table.AddCell("Ngày Đặt Hàng");
        //    table.AddCell("Tên Khách Hàng");
        //    table.AddCell("Số Điện Thoại");
        //    table.AddCell("Sản Phẩm");
        //    table.AddCell("Số Lượng");
        //    table.AddCell("Giá");
        //    table.AddCell("Tổng Tiền");

        //    foreach (var order in orders)
        //    {
        //        foreach (var product in order.Products)
        //        {
        //            table.AddCell(order.Code.ToString());
        //            table.AddCell(order.CreatedDate.ToString("dd/MM/yyyy"));
        //            table.AddCell(order.CustomerName);
        //            table.AddCell(order.CustomerPhone);
        //            table.AddCell(product.Name);
        //            table.AddCell(product.Quantity.ToString());
        //            table.AddCell(product.Price.ToString("C"));
        //            table.AddCell(product.UnitPrice.ToString("C"));
        //        }
        //    }

        //    document.Add(table);

        //    // Tổng doanh thu
        //    decimal totalRevenue = orders.Sum(o => o.TotalAmount);
        //    var totalRevenueFont = FontFactory.GetFont(FontFactory.HELVETICA, 14);
        //    var totalRevenueParagraph = new Paragraph($"Tổng Doanh Thu: {totalRevenue:C}", totalRevenueFont)
        //    {
        //        Alignment = Element.ALIGN_RIGHT // Căn phải
        //    };
        //    totalRevenueParagraph.SpacingBefore = 20; // Khoảng cách trước đoạn văn
        //    document.Add(totalRevenueParagraph);

        //    document.Close();

        //    // Trả file PDF về client
        //    var fileName = $"DoanhThu_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}.pdf";
        //    return File(stream.ToArray(), "application/pdf", fileName);
        //}
    }
}
