using Clinic_Management.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Clinic_Management.Utils
{
    public class AppointmentBrotherCode
    {
        private readonly G1_PRJ_DBContext _context;
        
        public AppointmentBrotherCode(G1_PRJ_DBContext context)
        {
            _context = context;
        }

        public string ConvertDateTime(DateTime dateTime)
        {
            // Lấy ngày trong tuần
            string[] daysOfWeek = { "Chủ Nhật", "Thứ Hai", "Thứ Ba", "Thứ Tư", "Thứ Năm", "Thứ Sáu", "Thứ Bảy" };
            string dayOfWeek = daysOfWeek[(int)dateTime.DayOfWeek];

            // Lấy các thành phần ngày, tháng, năm và giờ
            string day = dateTime.Day.ToString();
            string month = dateTime.Month.ToString();
            string year = dateTime.Year.ToString();
            string hour = dateTime.Hour.ToString();

            // Định dạng chuỗi kết quả
            string result = $"{dayOfWeek}, Ngày {day} Tháng {month} Năm {year}";

            return result;
        }


        public string EncodeAppointment(Appointment appointment)
        {
            // Format: {Specialization}-{Date(yyyyMMdd)}-{Id}
            string datePart = appointment.RequestedTime.ToString("yyyyMMdd");
            string specializationPart = appointment.Specialist.HasValue
                ? appointment.Specialist.Value.ToString("D3")
                : "000"; // Use "000" for null specialization
            string idPart = Encode(appointment.AppointmentId);

            return $"{idPart}-{datePart}-{specializationPart}";
        }

        public Appointment? DecodeAppointment(string code)
        {
            try
            {
                int id;
                // Split the code into parts
                if (code.Contains('-'))
                {
                    string[] parts = code.Split('-');
                    if (parts.Length != 3)
                    {
                        throw new ArgumentException("Invalid code format");
                    }

                    string specializationPart = parts[2];
                    string datePart = parts[1];
                    string idPart = parts[0];

                    id = Decode(idPart);
                }
                else
                {
                    id = Int32.Parse(code);
                }

                // Return the appointment
                return _context.Appointments
                    .Include(m => m.SpecialistNavigation)
                    .Include(m => m.StatusNavigation)
                    .Include(d => d.Doctor)
                    .Include(p => p.Patient)
                    .FirstOrDefault(m => m.AppointmentId == id);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Error decoding appointment code", ex);
            }
        }

        private const string AllowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private const int MaxLength = 5; // Độ dài tối đa của chuỗi mã hóa

        public string Encode(int number)
        {
            StringBuilder result = new StringBuilder();

            while (number > 0 && result.Length < MaxLength)
            {
                int index = number % AllowedChars.Length;
                result.Insert(0, AllowedChars[index]);
                number /= AllowedChars.Length;
            }

            return result.ToString();
        }

        public int Decode(string code)
        {
            int number = 0;

            for (int i = 0; i < code.Length; i++)
            {
                int charIndex = AllowedChars.IndexOf(code[i]);
                if (charIndex == -1)
                {
                    throw new ArgumentException("Invalid character in code");
                }
                number = number * AllowedChars.Length + charIndex;
            }
            return number;
        }

    }
}
