namespace Clinic_Management.Services
{
    public interface ISMSService
    {
        string EncodeOtp();

        DateTime DecodeOtp(string encodedString);

        void SendMessage(string message);
    }
}
