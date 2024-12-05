using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

public class VnPayLibrary
{
    private SortedList<string, string> requestData = new SortedList<string, string>();

    public void AddRequestData(string key, string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            requestData.Add(key, value);
        }
    }

    public string CreateRequestUrl(string baseUrl, string vnp_HashSecret)
    {
        var data = new StringBuilder();
        foreach (var kv in requestData.OrderBy(k => k.Key)) // Sắp xếp theo key
        {
            if (data.Length > 0) data.Append("&");
            data.Append(HttpUtility.UrlEncode(kv.Key) + "=" + HttpUtility.UrlEncode(kv.Value));
        }

        var rawData = data.ToString();
        var secureHash = HmacSHA512(vnp_HashSecret, rawData);
        return baseUrl + "?" + rawData + "&vnp_SecureHash=" + secureHash;
    }


    private string HmacSHA512(string key, string input)
    {
        using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key)))
        {
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
