using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace TheGourmet.Infrastructure.Payments;

public class VNPayLibrary
{
    private readonly SortedList<string, string> _requestData = new SortedList<string, string>(new VNPayCompare());
    private readonly SortedList<string, string> _responseData = new SortedList<string, string>(new  VNPayCompare());

    public void AddRequestData(string key, string value)
    {
        if (!string.IsNullOrEmpty(key)) _requestData.Add(key, value);
    }

    public void AddResponseData(string key, string value)
    {
        if (!string.IsNullOrEmpty(key))  _responseData.Add(key, value);
    }

    public string GetResponseData(string key)
    {
        return _responseData.TryGetValue(key, out var retValue) ? retValue : string.Empty;
    }
    
    public string CreateRequestUrl(string baseUrl, string vnp_HashSecret)
    {
        var data = new StringBuilder();
        foreach (var kv in _requestData)
        {
            if (!string.IsNullOrEmpty(kv.Key) && !string.IsNullOrEmpty(kv.Value))
            {
                data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
            }
        }

        var queryString = data.ToString().TrimEnd('&');
        var vnp_SecureHash = HmacSHA512(vnp_HashSecret, queryString);

        return baseUrl + "?" + queryString + "&vnp_SecureHash=" + vnp_SecureHash;
    }

    // hàm xác thực chữ ký (Kiểm tra xem hacker có thay đổi dữ liệu trả về từ VNPay hay không)
    public bool ValidateSignature(string inputHash, string secretKey)
    {
        var rspRaw = GetResponseData();
        var myCheckSum = HmacSHA512(secretKey, rspRaw);
        return myCheckSum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
    }

    private string GetResponseData()
    {
        var data = new StringBuilder();
        if (_responseData.ContainsKey("vnp_SecureHashType")) _responseData.Remove("vnp_SecureHashType");
        if (_responseData.ContainsKey("vnp_SecureHash")) _responseData.Remove("vnp_SecureHash");

        foreach (var kv in _responseData)
        {
            if (!string.IsNullOrEmpty(kv.Value))
            {
                // Encode lại để match với cách VNPay tạo hash
                data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
            }
        }
        if (data.Length > 0) data.Remove(data.Length - 1, 1);
        return data.ToString();
    }

    private string HmacSHA512(string key, string inputData)
    {
        var hash = new StringBuilder();
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
        using (var hmac = new HMACSHA512(keyBytes))
        {
            byte[] hashValue = hmac.ComputeHash(inputBytes);
            foreach (var b in hashValue)
            {
                hash.Append(b.ToString("x2"));
            }
        }
        return hash.ToString();
    }
}

public class VNPayCompare : IComparer<string>
{
    public int Compare(string x, string y)
    {
        if (x == y) return 0;
        if (x == null) return -1;
        if (y == null) return 1;

        var vnpCompare = string.CompareOrdinal(x, y);
        return vnpCompare;
    }
}