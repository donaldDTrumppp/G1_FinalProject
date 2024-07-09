using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Clinic_Management.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Clinic_Management.Services;
using NuGet.Protocol.Plugins;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Web;
using System.Net.Http.Headers;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using RestSharp;
using Microsoft.Data.SqlClient;
using NuGet.Common;

namespace Clinic_Management.Pages.OtpVerification
{
    public class IndexModel : PageModel
    {

        private readonly ISMSService _smsService;

        public IndexModel(ISMSService smsService)
        {
            _smsService = smsService;
        }

        public string Phone { get; set; } = "";

        public string OTP { get; set; } = "";

        public async Task<IActionResult>  OnGetAsync()
        {
            //SendOTP();
            return Page();
        }

        void SendOTP()
        {
            OTP = _smsService.EncodeOtp();
            _smsService.SendMessage("Your OTP is: " + OTP + ". This code will expired after 3 minutes");
        }
    }
}
