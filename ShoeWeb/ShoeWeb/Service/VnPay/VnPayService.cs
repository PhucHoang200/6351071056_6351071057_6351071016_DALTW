//using ShoeWeb.Helper.VnPay;
//using ShoeWeb.Libraries;
//using System;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.Configuration;
//using System.Diagnostics;
//using System.Linq;
//using System.Web;

//namespace ShoeWeb.Service.VnPay
//{
//    public class VnPayService : IVnPayService
//    {
//        public string CreatePaymentUrl(PaymentInformationModel model, HttpContext context)
//        {
//            // Lấy thông tin múi giờ từ cấu hình web.config
//            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(ConfigurationManager.AppSettings["TimeZoneId"]);
//            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);

//            var tick = DateTime.Now.Ticks.ToString();

//            var pay = new VnPayLibrary();
//            var urlCallBack = ConfigurationManager.AppSettings["PaymentCallBack:ReturnUrl"];

//            // Thêm các tham số vào yêu cầu
//            pay.AddRequestData("vnp_Version", ConfigurationManager.AppSettings["Vnpay:Version"]);
//            pay.AddRequestData("vnp_Command", ConfigurationManager.AppSettings["Vnpay:Command"]);
//            pay.AddRequestData("vnp_TmnCode", ConfigurationManager.AppSettings["Vnpay:TmnCode"]);
//            pay.AddRequestData("vnp_Amount", ((int)model.Amount * 100).ToString());
//            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
//            pay.AddRequestData("vnp_CurrCode", ConfigurationManager.AppSettings["Vnpay:CurrCode"]);
//            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
//            pay.AddRequestData("vnp_Locale", ConfigurationManager.AppSettings["Vnpay:Locale"]);
//            pay.AddRequestData("vnp_OrderInfo", $"{model.Name} {model.OrderDescription} {model.Amount}");
//            pay.AddRequestData("vnp_OrderType", model.OrderType);
//            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
//            pay.AddRequestData("vnp_TxnRef", tick);
//            pay.AddRequestData("vnp_ExpireDate ", DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss"));

//            // Tạo URL thanh toán
//            var paymentUrl = pay.CreateRequestUrl(ConfigurationManager.AppSettings["Vnpay:BaseUrl"], ConfigurationManager.AppSettings["Vnpay:HashSecret"]);
//            Debug.WriteLine(paymentUrl);
//            return paymentUrl;
//        }

//        public PaymentResponseModel PaymentExecute(NameValueCollection collections)
//        {
//            var pay = new VnPayLibrary();
//            var response = pay.GetFullResponseData(collections, ConfigurationManager.AppSettings["Vnpay:HashSecret"]);

//            return response;
//        }
//    }
//}