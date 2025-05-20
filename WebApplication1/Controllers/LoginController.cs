using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Model;
using Model.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PRMSecure;
using System.ComponentModel;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Web.Models;
using static Web.Models.ApiLoggerExtensions;
namespace Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly HttpClient client;
        private readonly IHttpContextAccessor httpContextAccessor;
        public string bucketName, DocumentsFolderName, s3UserIamgeFolderName;

        public LoginController(IConfiguration _configuration, IMemoryCache memoryCacheInterface, IHttpClientFactory httpClientFactory)
        {
            configuration = _configuration;
            s3UserIamgeFolderName = configuration["ApiSettings:s3UserIamgeFolderName"];
            string baseUrl = configuration["ApiSettings:BaseUrl"];
            this.client = httpClientFactory.CreateClient("HttpClientForApiTimeOut");
            client.BaseAddress = new Uri(baseUrl);
            if (memoryCacheInterface is MemoryCache concreteMemoryCache)
            {
                concreteMemoryCache.Clear();
            }
        }
        public IActionResult Login(string UserName, string PassWord)
        {
            HttpContext.Session.Clear();
            HttpContext.Response.Cookies.Delete("RequestVerificationToken");
            return View();
        }
        [HttpGet]
        public async Task<JsonResult> LoginUser(string UserName, string PassWord)
        {
            string uName = clsSecure.DecryptString("bqcYtyyQfEB4MrY9/9zvgQ==");
            string Pass = clsSecure.EncryptString("MvFzsmtxIDvwa+Lump+h3w==");
            DateTime startTime = DateTime.Now;
            PassWord = clsSecure.DecryptString(PassWord);
            string requesturl = $"{client.BaseAddress}/LoginApi/GetUserDetails?UserName={UserName}&PassWord={PassWord}";
            try
            {
                HttpResponseMessage response = await client.GetAsync($"{client.BaseAddress}/LoginApi/GetUserDetails?UserName={UserName}&PassWord={PassWord}");
                if (response.IsSuccessStatusCode)
                {
                    GenerateLogSuccess(response, DateTime.Now - startTime);
                    string UserDatajson = response.Content.ReadAsStringAsync().Result;
                    var jsonObject = JObject.Parse(UserDatajson);
                    if (jsonObject.Count > 0)
                    {
                        string token = (string)jsonObject["token"];
                        var userData = jsonObject["userDetails"][0]; // Extract the first object from the array
                                                                     // Retrieve parameters from the JSON object
                        if (!string.IsNullOrEmpty(token)) { 
                        DateTime expirationTime = GetExpirationTime(token);
                        }
                        var userId = (int?)userData["id"] ?? 0;
                        var uTypeID = (int?)userData["uTypeID"] ?? 0;
                        var stateID = (int?)userData["stateID"] ?? 0;
                        var zpid = (int?)userData["zpid"] ?? 0;
                        var deptid = (int?)userData["deptid"] ?? 0;
                        var blockID = (int?)userData["blockID"] ?? 0;
                        string imageurl = (string)userData["userImage"] ?? "";
                        string employeeName = (string)userData["employeeName"] ?? "";
                        var message = (string)userData["message"] ?? "";
                        var ULBID = (int?)userData["ulbid"] ?? 0;
                        var DIVID = (int?)userData["divID"] ?? 0;
                        string Designation = (string?)userData["designation"] ?? "";
                        int IsChargeHandOverTemporory = (int?)userData["isChargeHandOverTemporory"] ?? 0;
                        int ISChargeHandOverAccepted = (int?)userData["isChargeHandOverAccepted"] ?? 0;
                        int PrivacyPolicyAccepted = (int?)userData["privacyPolicyAccepted"] ?? 0;
                        string IAName = (string?)userData["iaName"];
                        if (IAName.IsNullOrEmpty())
                        {
                            IAName = "Maharashtra";
                        }
                        int PassChangeRemainingDay = (int?)userData["passChangeRemainingDay"] ?? 0;
                        if (employeeName != "")
                        {
                            employeeName = FormatedName(employeeName);
                        }
                        if (Designation != "")
                        {
                            Designation = FormatedName(Designation);
                        }
                        if (IAName != "")
                        {
                            IAName = FormatedName(IAName);
                        }
                        // Store parameters in session
                        HttpContext.Session.SetString("Token", token);
                        HttpContext.Session.SetInt32("UserId", userId);
                        HttpContext.Session.SetInt32("UTypeID", uTypeID);
                        HttpContext.Session.SetInt32("StateID", stateID);
                        HttpContext.Session.SetInt32("ZPID", zpid);
                        HttpContext.Session.SetInt32("DepId", deptid);
                        HttpContext.Session.SetInt32("BlockID", blockID);
                        HttpContext.Session.SetString("userImage", imageurl);
                        HttpContext.Session.SetString("EmployeeName", employeeName);
                        HttpContext.Session.SetInt32("ULBID", ULBID);
                        HttpContext.Session.SetInt32("DIVID", DIVID);
                        HttpContext.Session.SetString("Designation", Designation);
                        HttpContext.Session.SetString("IAName", IAName);
                        HttpContext.Session.SetInt32("PassChangeRemainingDay", PassChangeRemainingDay);
                        HttpContext.Session.SetInt32("IsChargeHandOverTemporory", IsChargeHandOverTemporory);
                        HttpContext.Session.SetInt32("ISChargeHandOverAccepted", ISChargeHandOverAccepted);
                        HttpContext.Session.SetInt32("PrivacyPolicyAccepted", PrivacyPolicyAccepted);

                        await GetUserImage();
                        setData();//set Financial Year
                        return new JsonResult(UserDatajson);
                    }
                    else
                    {
                        return new JsonResult(new { errorMessage = $"No user data found" }) { StatusCode = 404 };
                    }
                }
                else
                {
                    GenerateLogSuccess(response, DateTime.Now - startTime);
                    return new JsonResult(new { errorMessage = $"Error occurred while fetching components" }) { StatusCode = 500 };
                }
            }
            catch (Exception ex)
            {
                GenerateLogError("GET", requesturl, DateTime.Now - startTime, ex.Message.ToString());
                return new JsonResult(new { errorMessage = $"Error: {ex.Message}" }) { StatusCode = 500 };
            }
        }
        [HttpGet]
        public async Task<List<ChangeProfileViewModel>> GetUserImage()
        {
            DateTime startTime = DateTime.Now;
            List<ChangeProfileViewModel> lstuser = new List<ChangeProfileViewModel>();
            int UserId = HttpContext.Session.GetInt32("UserId") ?? 0;
            string requesturl = $"{client.BaseAddress}/LoginApi/GetUserDetails?UserName={UserId}";
            try
            {
                HttpResponseMessage response = await client.GetAsync($"{client.BaseAddress}/LoginApi/UserImage?UserId={UserId}");
                if (response.IsSuccessStatusCode)
                {
                    GenerateLogSuccess(response, DateTime.Now - startTime);
                    string UserDatajson = response.Content.ReadAsStringAsync().Result;
                    lstuser = JsonConvert.DeserializeObject<List<ChangeProfileViewModel>>(UserDatajson);

                    var imageurl = lstuser[0].ImageFileName;
                    HttpContext.Session.SetString("userImage", imageurl);
                }
                else
                {
                    GenerateLogSuccess(response, DateTime.Now - startTime);
                }
            }
            catch (Exception ex)
            {
                GenerateLogError("GET", requesturl, DateTime.Now - startTime, ex.Message.ToString());
            }
            return lstuser;
        }

        [NonAction]
        public void setData()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                string lblCurrentDate = DateTime.Now.ToString("dd/MM/yyyy");
                DateTime date = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy"));
                int currentmonth = Convert.ToInt32(date.Month);
                int currentyear = Convert.ToInt32(date.Year);
                string FinYear = "";
                string FinYearPrev = "";
                string FinYearNext = "";
                if (currentmonth <= 3)
                {
                    FinYear = (currentyear - 1).ToString() + "-" + currentyear.ToString().Substring(2);
                    FinYearPrev = (currentyear - 2).ToString() + "-" + (currentyear - 1).ToString().Substring(2);
                    FinYearNext = (currentyear).ToString() + "-" + (currentyear + 1).ToString().Substring(2);
                }
                if (currentmonth >= 4)
                {
                    FinYear = currentyear.ToString() + "-" + (currentyear + 1).ToString().Substring(2);
                    FinYearPrev = (currentyear - 1).ToString() + "-" + (currentyear).ToString().Substring(2);
                    FinYearNext = (currentyear).ToString() + "-" + (currentyear + 1).ToString().Substring(2);
                }
                HttpContext.Session.SetString("FinancialYear", FinYear);
                HttpContext.Session.SetString("FinancialYearPrev", FinYearPrev);
                HttpContext.Session.SetString("FinancialYearNext", FinYearNext);
                string ProjectID = configuration["ApiSettings:ProjectID"] ?? "";
                HttpContext.Session.SetString("ProjectID", ProjectID);
            }
        }
        public IActionResult ChangePassword()
        {
            return View();

        }
        [NonAction]
        public string FormatedName(string Name)
        {
            var nameArray = Name.Split(' ');
            if (nameArray.Length == 1)
            {
                if (nameArray[0] == "IA") return nameArray[0];
                return char.ToUpper(nameArray[0][0]) + nameArray[0].Substring(1).ToLower();
            }
            else if (nameArray.Length > 1)
            {
                string formattedName = "";
                foreach (string word in nameArray)
                {
                    if (word == " " || word == "")
                    {
                        continue;
                    }
                    else if (word == "IA")
                    {
                        formattedName += word + " ";
                        continue;
                    }
                    else
                        formattedName += char.ToUpper(word[0]) + word.Substring(1).ToLower() + " ";
                }
                return formattedName.TrimEnd(); // remove trailing space
            }
            else
            {
                return Name;
            }
        }
        [NonAction]
        public DateTime GetExpirationTime(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            if (jwtToken == null)
                throw new ArgumentException("Invalid token");

            var expClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Exp);

            if (expClaim == null)
                throw new ArgumentException("Expiration claim not found");

            var exp = long.Parse(expClaim.Value);
            var expirationTime = DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;

            return expirationTime;
        }
        [HttpGet]
        public async Task<IActionResult> GetEncryptString(string Input)
        {
            if (Input == null)
            {
                return BadRequest("Please Correct Input");
            }
            string drystring=clsSecure.EncryptString(Input);
            var response = new
            {
                StatusCode = 200,
                result = drystring
            };
            return Ok(response);
        }
    }
}
