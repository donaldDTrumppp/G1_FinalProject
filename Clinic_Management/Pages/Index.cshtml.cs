using Clinic_Management.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Clinic_Management.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;


        public string? Message { get; set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;

        }



        public async Task OnGetAsync(string? Message)
        {

            Console.WriteLine("MS: " + Message);
            this.Message = Message;
        }
    }
}
