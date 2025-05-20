using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Model;
using Model.ViewModel;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Net;
using System.Net.Http.Headers;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Web.Models.ApiLoggerExtensions;
namespace Web.Controllers
{
    public class WorkOrderController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly HttpClient client;
        public string accesskey = string.Empty;
        public string secretkey = string.Empty;
        public string bucketName, DocumentsFolderName, s3UserIamgeFolderName;
        private readonly IAntiforgery _antiforgery;
        public WorkOrderController(IConfiguration _configuration,IHttpClientFactory httpClientFactory, IAntiforgery antiforgery)
        {
            this.client = httpClientFactory.CreateClient("HttpClientForApiTimeOut");
            configuration = _configuration;
            string baseUrl = configuration["ApiSettings:BaseUrl"];
            client.BaseAddress = new Uri(baseUrl);

            accesskey = configuration["ApiSettings:AWSAccessKey"];
            secretkey = configuration["ApiSettings:AWSSecretKey"];
            bucketName = configuration["ApiSettings:s3BucketName"];
            DocumentsFolderName = configuration["ApiSettings:s3DocumentsFolderName"];
            s3UserIamgeFolderName = configuration["ApiSettings:s3UserIamgeFolderName"];
            _antiforgery = antiforgery;
        }
        [HttpGet]
        public async Task<IActionResult> WorkOrder()
        {
            int UserID = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (UserID == 0)
            {
                return RedirectToAction("Login", "Login");
            }
            List<MstFinancialYear> lstFY = new List<MstFinancialYear>();
            List<MstCashbook> lstSchemes = new List<MstCashbook>();
            List<MstBillSequence> LstLastBillSequence = new List<MstBillSequence>();
            List<MstWorkStatus> lstWorkStatus = new List<MstWorkStatus>();
            try
            {
                lstFY = await GetFinancialYears();
                lstSchemes = await GetSchemes();
                LstLastBillSequence = await GetLastBillSequences();
                lstWorkStatus = await GetWorkStatus();
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ErrorMessage = "An error occurred while retrieving data. Please try again later.";
            }
            catch (JsonException ex)
            {
                ViewBag.ErrorMessage = "An error occurred while processing data. Please try again later.";
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred. Please try again later.";
            }
            ViewBag.lst_financialYear = new SelectList(lstFY, "Id", "FyName");
            ViewBag.lst_Schemes = new SelectList(lstSchemes, "Id", "CashBookNameEnglish");
            ViewBag.lst_LastBillSequence = new SelectList(LstLastBillSequence, "Id", "Ra");
            ViewBag.lst_WorkStatus = new SelectList(lstWorkStatus, "Id", "WorkStatusNameEnglish");
            return View();
        }
        [HttpGet]
        private async Task<List<MstFinancialYear>> GetFinancialYears()
        {
            DateTime startTime  = DateTime.Now;
            string requesturl = client.BaseAddress + "/WorkOrderApi/GetFinancialYears"; 
            try
            {
                string token = HttpContext.Session.GetString("Token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = client.GetAsync(client.BaseAddress + "/WorkOrderApi/GetFinancialYears").Result;

                if (response.IsSuccessStatusCode)
                {
                    GenerateLogSuccess(response, DateTime.Now - startTime);
                    string financialYears = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<MstFinancialYear>>(financialYears);
                }
                else
                {
                    GenerateLogSuccess(response, DateTime.Now - startTime);
                    return new List<MstFinancialYear>();
                }
            }
            catch (Exception ex)
            {
                GenerateLogError("GET", requesturl, DateTime.Now - startTime, ex.Message.ToString());
                return new List<MstFinancialYear>();
            }
        }
        [HttpGet]
        private async Task<List<MstCashbook>> GetSchemes()
        {
            DateTime startTime = DateTime.Now;
            string requesturl = client.BaseAddress + "/WorkOrderApi/GetScheme";
            try
            {
                string token = HttpContext.Session.GetString("Token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = client.GetAsync(client.BaseAddress + "/WorkOrderApi/GetScheme").Result;
                if (response.IsSuccessStatusCode)
                {
                    GenerateLogSuccess(response, DateTime.Now - startTime);
                    string schemes = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<MstCashbook>>(schemes);
                }
                else
                {
                    GenerateLogSuccess(response, DateTime.Now - startTime);
                    return new List<MstCashbook>();
                }
            }
            catch(Exception ex)
            {
                GenerateLogError("GET", requesturl, DateTime.Now - startTime, ex.Message.ToString());
                return new List<MstCashbook>();
            }
        }
        [HttpGet]
        private async Task<List<MstBillSequence>> GetLastBillSequences()
        {
            DateTime startTime =DateTime.Now;
            string requesturl = client.BaseAddress + "/WorkOrderApi/GetLastBillSequences";
            try
            {
                string token = HttpContext.Session.GetString("Token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = client.GetAsync(client.BaseAddress + "/WorkOrderApi/GetLastBillSequences").Result;
                if (response.IsSuccessStatusCode)
                {
                    GenerateLogSuccess(response, DateTime.Now - startTime);
                    string BillSequence = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<MstBillSequence>>(BillSequence);
                }
                else
                {
                    GenerateLogSuccess(response, DateTime.Now - startTime);
                    return new List<MstBillSequence>();
                }
            } 
            catch(Exception ex)
            {
                GenerateLogError("GET", requesturl, DateTime.Now - startTime, ex.Message.ToString());
                return new List<MstBillSequence>();
            }
        }
        [HttpGet]
        private async Task<List<MstWorkStatus>> GetWorkStatus()
        {
            DateTime startTime = DateTime.Now;
            string requesturl = client.BaseAddress + "/WorkOrderApi/GetWorkStatus";
            try
            {
                string token = HttpContext.Session.GetString("Token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = client.GetAsync(client.BaseAddress + "/WorkOrderApi/GetWorkStatus").Result;
                if (response.IsSuccessStatusCode)
                {
                    GenerateLogSuccess(response, DateTime.Now - startTime);
                    string WorkStatus = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<MstWorkStatus>>(WorkStatus);
                }
                else
                {
                    GenerateLogSuccess(response, DateTime.Now - startTime);
                    return new List<MstWorkStatus>();
                }
            }
            catch(Exception ex)
            {
                GenerateLogError("GET", requesturl, DateTime.Now - startTime, ex.Message.ToString());
                return new List<MstWorkStatus>();
            }
        }
        public async Task<JsonResult> GetComponentBySchemeId(int schemeId)
        {
            List<MstHeadType> lst_components = new List<MstHeadType>();
            lst_components = await GetcomponentsbyId(schemeId);
            return Json(lst_components);
        }
        [HttpGet]
        private async Task<List<MstHeadType>> GetcomponentsbyId(int schemeId)
        {
            DateTime startTime = DateTime.Now;
            string requesturl = client.BaseAddress + "/WorkOrderApi/GetComponent?SchemeId=" + schemeId;
            try
            {
                string token = HttpContext.Session.GetString("Token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = client.GetAsync(client.BaseAddress + "/WorkOrderApi/GetComponent?SchemeId=" + schemeId).Result;
                if (response.IsSuccessStatusCode)
                {
                    GenerateLogSuccess(response, DateTime.Now - startTime);
                    string subschemes = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<MstHeadType>>(subschemes);
                }
                else
                {
                    GenerateLogSuccess(response, DateTime.Now - startTime);
                    return new List<MstHeadType>();
                }
            }
            catch (Exception ex)
            {
                GenerateLogError("GET", requesturl, DateTime.Now - startTime, ex.Message.ToString());
                return new List<MstHeadType>();
            }
        }
        [HttpGet]
        public async Task<JsonResult> Get_AdminApprovalNumber(int Componentid, int SchemeId)
        {
            DateTime startTime = DateTime.Now;
            int Zpid = HttpContext.Session.GetInt32("ZPID") ?? 0;
            int Deptid = HttpContext.Session.GetInt32("DepId") ?? 0;
            int UserID = HttpContext.Session.GetInt32("UserId") ?? 0;
            int DivId = HttpContext.Session.GetInt32("DivID") ?? 0;
            int Ulbid = HttpContext.Session.GetInt32("ULBID") ?? 0;
            string token = HttpContext.Session.GetString("Token") ?? "";
            string requesturl = $"{client.BaseAddress}/WorkOrderApi/GetAdminApprovalNumber?ZPID={Zpid}&SchemeId={SchemeId}&HeadCodeID={Componentid}&userID={UserID}&DivID={DivId}&Ulbid={Ulbid}";
            try
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.GetAsync($"{client.BaseAddress}/WorkOrderApi/GetAdminApprovalNumber?ZPID={Zpid}&SchemeId={SchemeId}&HeadCodeID={Componentid}&userID={UserID}&DivID={DivId}&Ulbid={Ulbid}");
                if (response.IsSuccessStatusCode)
                {
                    GenerateLogSuccess(response, DateTime.Now - startTime);
                    string AdminApprovalNo = await response.Content.ReadAsStringAsync();
                    List<WorkDetailsMaster> lst_adminApprovalNo = new List<WorkDetailsMaster>();
                    lst_adminApprovalNo = JsonConvert.DeserializeObject<List<WorkDetailsMaster>>(AdminApprovalNo);
                    return Json(lst_adminApprovalNo);
                }
                else
                {
                    GenerateLogSuccess(response, DateTime.Now - startTime);
                    return Json("False");
                }
            }
            catch (Exception ex)
            {
                GenerateLogError("GET", requesturl, DateTime.Now - startTime, ex.Message.ToString());
                return Json(HttpStatusCode.InternalServerError);
            }
        }
        [HttpGet]
        public async Task<JsonResult> CheckAAUpdateApproval(int AdminApprovalNo)
        {
            DateTime startTime = DateTime.Now;
            string requesturl = $"{client.BaseAddress}/WorkOrderApi/CheckAAUpdateApproval?AdminApprovalNo={AdminApprovalNo}";
            try
            {
                string token = HttpContext.Session.GetString("Token") ?? "";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.GetAsync($"{client.BaseAddress}/WorkOrderApi/CheckAAUpdateApproval?AdminApprovalNo={AdminApprovalNo}");
                if (response.IsSuccessStatusCode)
                {
                    GenerateLogSuccess(response, DateTime.Now - startTime);
                    string subschemes = await response.Content.ReadAsStringAsync();
                    //return JsonConvert.DeserializeObject<List<MstHeadType>>(subschemes);
                    return Json(subschemes);
                }
                else
                {
                    GenerateLogSuccess(response, DateTime.Now - startTime);
                    return Json("False");
                }
            }
            catch (Exception ex)
            {
                GenerateLogError("GET", requesturl, DateTime.Now - startTime, ex.Message.ToString());
                return Json("");
            } 
        }
        [HttpGet]
        public async Task<JsonResult> Get_AdminApproveNumberDetails(int AdminApprovalNo, int Componentid)
        {
            DateTime startTime = DateTime.Now;            
            int Zpid = HttpContext.Session.GetInt32("ZPID") ?? 0;
            int Deptid = HttpContext.Session.GetInt32("DepId") ?? 0;
            int UserID = HttpContext.Session.GetInt32("UserId") ?? 0;
            int DivId = HttpContext.Session.GetInt32("DivID") ?? 0;
            int Ulbid = HttpContext.Session.GetInt32("ULBID") ?? 0;
            string token = HttpContext.Session.GetString("Token") ?? "";
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            string requesturl = $"{client.BaseAddress}/WorkOrderApi/GetAdminApproveNumberDetails?ZPID={Zpid}&AdminApprovalNo={AdminApprovalNo}&HeadCodeID={Componentid}&userID={UserID}&DivID={DivId}&Ulbid={Ulbid}&Deptid={Deptid}";
            try
            {
                HttpResponseMessage response = await client.GetAsync($"{client.BaseAddress}/WorkOrderApi/GetAdminApproveNumberDetails?ZPID={Zpid}&AdminApprovalNo={AdminApprovalNo}&HeadCodeID={Componentid}&userID={UserID}&DivID={DivId}&Ulbid={Ulbid}&Deptid={Deptid}");
                if (response.IsSuccessStatusCode)
                {
                    GenerateLogSuccess(response, DateTime.Now - startTime);
                    string AdminApprovalDetails = await response.Content.ReadAsStringAsync();
                    List<WorkDetailsMaster> lst_adminApprovalDetails = new List<WorkDetailsMaster>();
                    lst_adminApprovalDetails = JsonConvert.DeserializeObject<List<WorkDetailsMaster>>(AdminApprovalDetails);
                    return Json(lst_adminApprovalDetails);
                }
                else
                {
                    GenerateLogSuccess(response, DateTime.Now - startTime);
                    return Json("False");
                }
            }
            catch (Exception ex)
            {
                GenerateLogError("GET", requesturl, DateTime.Now - startTime, ex.Message.ToString());
                return Json("");
            }
        }
        [HttpGet]
        public async Task<JsonResult> Get_VendorInformation(string SearchBy, string Input,int SchemeID,int ComponentID)
        {
            DateTime startTime = DateTime.Now;            
            int Zpid = HttpContext.Session.GetInt32("ZPID") ?? 0;
            int DivId = HttpContext.Session.GetInt32("DivID") ?? 0;
            int Ulbid = HttpContext.Session.GetInt32("ULBID") ?? 0;
            string requesturl = $"{client.BaseAddress}/WorkOrderApi/GetVendorInformation?ZPID={Zpid}&SearchBy={SearchBy}&Input={Input}&DivID={DivId}&Ulbid={Ulbid}&SchemeID={SchemeID}&ComponentID={ComponentID}";
            try
            {
                string token = HttpContext.Session.GetString("Token") ?? "";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.GetAsync($"{client.BaseAddress}/WorkOrderApi/GetVendorInformation?ZPID={Zpid}&SearchBy={SearchBy}&Input={Input}&DivID={DivId}&Ulbid={Ulbid}&SchemeID={SchemeID}&ComponentID={ComponentID}");
                if (response.IsSuccessStatusCode)
                {
                    GenerateLogSuccess(response, DateTime.Now - startTime);
                    string AdminApprovalDetails = await response.Content.ReadAsStringAsync();
                    List<MstParty> lst_adminApprovalDetails = new List<MstParty>();
                    lst_adminApprovalDetails = JsonConvert.DeserializeObject<List<MstParty>>(AdminApprovalDetails);
                    return Json(lst_adminApprovalDetails);
                }
                else
                {
                    GenerateLogSuccess(response, DateTime.Now - startTime);
                    return Json("False");
                }
            }
            catch (Exception ex)
            {
                GenerateLogError("GET", requesturl, DateTime.Now - startTime, ex.Message.ToString());
                return Json("");
            }
        }
        #region FileUpload
        public async Task<string> FileUpload(IFormFile file)
        {
            string result = string.Empty;
            string currentdate = System.DateTime.Now.ToString("dd-MM-yyyy");
            clsUploadFileValidations objValidation = new clsUploadFileValidations();
            AWSS3Bucket objs3 = new AWSS3Bucket(configuration);
            long iFileSize = file.Length;
            //string Newfilename = Path.GetFileName(currentdate + "_" + file.FileName);
            string Newfilename = file.FileName;
            string Message = objValidation.FileUploadCheck(file.FileName, iFileSize);
            if (Message == "")
            {
                Stream st = file.OpenReadStream();
                if (objs3.UploadDocumentsFileToS3(Newfilename, st))
                    result = "File uploaded successfully";
                else
                    result = "File uploaded failed";
            }
            else
                result = Message;
            return result;
        }
        #endregion
        [HttpPost]
        public async Task<IActionResult> Upload_WorkOrderCopy(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }
            try
            {
                if (file != null && file.Length > 0)
                {
                    string result = await FileUpload(file);
                    result = "File uploaded successfully";
                    return Json(new { Success = true, Message = result });
                }
                return Json(new { Success = false, Message = "No file selected" });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Error uploading file: " + ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> SaveWorkDetails([FromBody] WorkDetailsDummy formData)
        {
            try
            {
                await _antiforgery.ValidateRequestAsync(HttpContext);
            }
            catch (AntiforgeryValidationException ex)
            {
                return StatusCode(400, new { message = "Anti Forgery Token was invalid." });
            }
            int Zpid = HttpContext.Session.GetInt32("ZPID") ?? 0;
            int Deptid = HttpContext.Session.GetInt32("DepId") ?? 0;
            int UserID = HttpContext.Session.GetInt32("UserId") ?? 0;
            int DivId = HttpContext.Session.GetInt32("DivID") ?? 0;
            int Ulbid = HttpContext.Session.GetInt32("ULBID") ?? 0;
            int BlockID = HttpContext.Session.GetInt32("BlockID") ?? 0;
            formData.Zpid = Zpid;
            formData.DeptId = Deptid;
            formData.CreatedBy = UserID;
            formData.DivId = DivId;
            formData.Ulbid = Ulbid;
            formData.BlockId = BlockID;
            var json = JsonConvert.SerializeObject(formData);
            DateTime startTime = DateTime.Now;
            string requesturl = $"{client.BaseAddress}/WorkOrderApi/SaveWorkDetails" + json;
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            try
            {
                string token = HttpContext.Session.GetString("Token") ?? "";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.PostAsync($"{client.BaseAddress}/WorkOrderApi/SaveWorkDetails", content);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    GenerateLogSuccessPost(response, DateTime.Now - startTime, requesturl);
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var responsedata= JsonConvert.DeserializeObject<dynamic>(responseBody);
                    string? workDetailsUID = responsedata.workDetailsUID;                  

                    return Ok(new { WorkDetailsUID = workDetailsUID });
                }
                else
                {
                    GenerateLogSuccessPost(response, DateTime.Now - startTime, requesturl);
                    return StatusCode((int)response.StatusCode, "Failed to save receipt data");
                }
            }
            catch (HttpRequestException ex)
            {
                GenerateLogError("POST", requesturl, DateTime.Now - startTime, ex.Message.ToString());
                return StatusCode(500, "An error occurred while making the HTTP request to the API.");
            }
            catch (Exception ex)
            {
                GenerateLogError("POST", requesturl, DateTime.Now - startTime, ex.Message.ToString());
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetWrokDetailsByID(int HeadCodeID, int AdminApprovalID)
        {
            DateTime startTime = DateTime.Now;            
                int Zpid = HttpContext.Session.GetInt32("ZPID") ?? 0;
                int Deptid = HttpContext.Session.GetInt32("DepId") ?? 0;
                int UserID = HttpContext.Session.GetInt32("UserId") ?? 0;
                int DivId = HttpContext.Session.GetInt32("DIVID") ?? 0;
                int Ulbid = HttpContext.Session.GetInt32("ULBID") ?? 0;
                int BlockID = HttpContext.Session.GetInt32("BlockID") ?? 0;
                string token = HttpContext.Session.GetString("Token") ?? "";
            string requesturl = $"{client.BaseAddress}/WorkOrderApi/GetWrokDetailsByID?ZPID={Zpid}&HeadCodeID={HeadCodeID}&DeptID={Deptid}&UserID={UserID}&AdminApprovalID={AdminApprovalID}&DivID={DivId}&ULBID={Ulbid}";
            try
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.GetAsync($"{client.BaseAddress}/WorkOrderApi/GetWrokDetailsByID?ZPID={Zpid}&HeadCodeID={HeadCodeID}&DeptID={Deptid}&UserID={UserID}&AdminApprovalID={AdminApprovalID}&DivID={DivId}&ULBID={Ulbid}");
                if (response.IsSuccessStatusCode)
                {
                    GenerateLogSuccess(response,DateTime.Now - startTime);
                    List<GetWorkDetailsByAAIDViewModel> lst = new List<GetWorkDetailsByAAIDViewModel>();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    lst = JsonConvert.DeserializeObject<List<GetWorkDetailsByAAIDViewModel>>(responseBody);
                    return Ok(lst);
                }
                else
                {
                    GenerateLogSuccess(response, DateTime.Now - startTime);
                    return StatusCode(500, "An unexpected error occurred.");
                }
            }
            catch (HttpRequestException ex)
            {
                GenerateLogError("GET", requesturl, DateTime.Now - startTime, ex.Message.ToString());
                return StatusCode(500, "An error occurred while making the HTTP request to the API.");
            }
            catch (Exception ex)
            {
                GenerateLogError("GET", requesturl, DateTime.Now - startTime, ex.Message.ToString());
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
        [HttpGet]
        public async Task<JsonResult> CheckWorkisExist(string AdminApprovalNumber, string WorkOrderNumber)
        {
            DateTime startTime = DateTime.Now;
            string requesturl = $"{client.BaseAddress}/WorkOrderApi/CheckExistingWorkNoByID?AANumber={AdminApprovalNumber}&WorkOrderNumber={WorkOrderNumber}";
            try
            {
                int Zpid = HttpContext.Session.GetInt32("ZPID") ?? 0;
                int DivId = HttpContext.Session.GetInt32("DivID") ?? 0;
                int Ulbid = HttpContext.Session.GetInt32("ULBID") ?? 0;
                string token = HttpContext.Session.GetString("Token") ?? "";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.GetAsync($"{client.BaseAddress}/WorkOrderApi/CheckExistingWorkNoByID?AANumber={AdminApprovalNumber}&WorkOrderNumber={WorkOrderNumber}");
                if (response.IsSuccessStatusCode)
                {
                    GenerateLogSuccess(response, DateTime.Now - startTime);
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var responseObject = JsonConvert.DeserializeObject<dynamic>(responseBody);
                    bool? Message = responseObject.message;
                    return Json(new { message = Message });
                }
                else
                {
                    GenerateLogSuccess(response, DateTime.Now - startTime);
                    return Json(new { message = false });
                }
            }
            catch (Exception ex)
            {
                GenerateLogError("GET", requesturl, DateTime.Now - startTime, ex.Message.ToString());
                return Json(new { message = false });
            }
        }
    }
}
