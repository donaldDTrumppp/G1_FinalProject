//using Clinic_Management.Models;
using System.Configuration;
using Telesign;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Clinic_Management.Services
{
    public class SMSService : ISMSService
    {

        private readonly IConfiguration? _configuration;
        
        public SMSService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string EncodeOtp()
        {
            DateTime now = DateTime.Now;
            int year = now.Year;
            int month = now.Month;
            int day = now.Day;
            int hour = now.Hour;
            int minute = now.Minute;
            int second = now.Second;

            // Convert to a single integer
            int encoded = (year % 100) * 100000000 + month * 1000000 + day * 10000 + hour * 100 + minute + second;

            // Convert to string and take last 8 digits
            string encodedString = encoded.ToString().Substring(Math.Max(0, encoded.ToString().Length - 8));

            return encodedString;
        }

        public DateTime DecodeOtp(string encodedString)
        {
            if (encodedString.Length != 10)
            {
                throw new ArgumentException("Encoded string must be exactly 10 characters long.");
            }

            int year = int.Parse(encodedString.Substring(0, 2)) + 2000; // Giả định năm trong thế kỷ 21
            int month = int.Parse(encodedString.Substring(2, 2));
            int day = int.Parse(encodedString.Substring(4, 2));
            int hour = int.Parse(encodedString.Substring(6, 2));
            int minute = int.Parse(encodedString.Substring(8, 2));

            DateTime decodedTime = new DateTime(year, month, day, hour, minute, 0);
            return decodedTime;
        }

        public async void SendMessage(string m)
        {
            
            string customerId = "25E5D91F-36F1-4D98-B1F2-CB2BA0A71398";
            string apiKey = "sy6UMSImsdDsAz4Kg240xTBdYRLdZ4VWA4gLbT8Xdsv1Gh2cbggwVPyttCJk4uoyYmUCgjoyza8/+6aAsG5Zkg==";

            // Set the default below to your test phone number. 
            // In your production code, update the phone number dynamically for each transaction.    
            string phoneNumber = "84394672294";

            // (Optional) Pull values from environment variables instead of hardcoding them.
            if (System.Environment.GetEnvironmentVariable("CUSTOMER_ID") != null)
            {
                customerId = System.Environment.GetEnvironmentVariable("CUSTOMER_ID");
            }

            if (System.Environment.GetEnvironmentVariable("API_KEY") != null)
            {
                apiKey = System.Environment.GetEnvironmentVariable("API_KEY");
            }

            if (System.Environment.GetEnvironmentVariable("PHONE_NUMBER") != null)
            {
                phoneNumber = System.Environment.GetEnvironmentVariable("PHONE_NUMBER");
            }

            // Set the message text and type.
            string message = m;
            string messageType = "ARN";

            try
            {
                // Instantiate a messaging client object.
                MessagingClient messagingClient = new MessagingClient(customerId, apiKey);

                // Make the request and capture the response.
                RestClient.TelesignResponse telesignResponse = messagingClient.Message(phoneNumber, message, messageType);

                // Display the response in the console for debugging purposes. 
                // In your production code, you would likely remove this.
                Console.WriteLine("\nResponse HTTP status:\n" + telesignResponse.StatusCode);
                Console.WriteLine("\nResponse body:\n" + telesignResponse.Body);

            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nAn exception occured.\nERROR: " + e.Message + "\n");
                Console.ResetColor();
            }

            Console.WriteLine("Press any key to quit.");

            return;
            
        }
    }
}
