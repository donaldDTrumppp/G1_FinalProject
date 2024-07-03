using Clinic_Management.Models;
using Clinic_Management.Pages.MedicalRecords.utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Clinic_Management.Pages.MedicalRecords
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalRecordsController : ControllerBase
    {
        private readonly G1_PRJ_DBContext _context;
        private AppointmentBrotherCode brotherCode;

        public MedicalRecordsController(G1_PRJ_DBContext context)
        {
            _context = context;
            brotherCode = new AppointmentBrotherCode(_context);
        }

        // GET: api/MedicalRecords
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MedicalRecord>>> GetMedicalRecords()
        {
            if (_context.MedicalRecords == null)
            {
                return NotFound();
            }
            return await _context.MedicalRecords.ToListAsync();
        }

        [HttpGet("doctors")]
        public ActionResult<IEnumerable<Doctor>> GetDoctor()
        {
            var staffWithDoctorRole = _context.Staff
                .Include(s => s.User)
                .ThenInclude(u => u.Role)
                .Include(s => s.DoctorSpecialistNavigation)
                .Where(s => s.User.Role.RoleName == "Doctor")
                .ToList();

            var doctors = staffWithDoctorRole.Select(s => new Doctor
            {
                Id = s.UserId,
                Name = s.User.Name,
                Image = s.Image,
                Specialist = s.DoctorSpecialistNavigation != null ? s.DoctorSpecialistNavigation.SpecialistName : "Unknown"
            }).ToList();

            return Ok(doctors);
        }

        [HttpGet("appointments")]
        public ActionResult<IEnumerable<Appointment>> GetAppointments()
        {
            var apms = _context.Appointments
                .Include(m => m.SpecialistNavigation)
                .Include(m => m.StatusNavigation)
                .Include(d => d.Doctor)
                .Include(p => p.Patient)
                .ToList();
            var basicApms = apms.Select(s => new BasicAppointment
            {
                Id = brotherCode.EncodeAppointment(s),
                Specialization = s.Specialist.GetValueOrDefault(),
                DoctorId = s.DoctorId.GetValueOrDefault(),
                DoctorName = s.Doctor != null ? s.Doctor.Name : "Unknown",
                DoctorSpecialization = s.SpecialistNavigation != null ? s.SpecialistNavigation.SpecialistName : "Other",
                PatientId = s.PatientId,
                PatientName = s.PatientName,
                PatientAddress = s.PatientAddress,
                PatientPhone = s.PatientPhoneNumber,
                PatientEmail = s.PatientEmail,
                PatientDob = s.PatientDob,
                RequestTime = s.RequestedTime
            }).ToList();
            return Ok(basicApms);
        }

        [HttpGet("appointments/{id}")]
        public ActionResult<IEnumerable<Appointment>> GetAppointment(string id)
        {
            Appointment? s = brotherCode.DecodeAppointment(id);
            if (s == null) return NotFound("Not found appointment");
            else return Ok(new BasicAppointment
            {
                Id = brotherCode.EncodeAppointment(s),
                Specialization = s.Specialist.GetValueOrDefault(),
                DoctorId = s.DoctorId.GetValueOrDefault(),
                DoctorName = s.Doctor != null ? s.Doctor.Name : "Unknown",
                DoctorSpecialization = s.SpecialistNavigation != null ? s.SpecialistNavigation.SpecialistName : "Other",
                PatientId= s.PatientId,
                PatientName = s.PatientName,
                PatientAddress = s.PatientAddress,
                PatientPhone = s.PatientPhoneNumber,
                PatientEmail = s.PatientEmail,
                PatientDob = s.PatientDob,
                RequestTime = s.RequestedTime
            });
        }

        // GET: api/MedicalRecords/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MedicalRecord>> GetMedicalRecord(int id)
        {
            if (_context.MedicalRecords == null)
            {
                return NotFound();
            }
            var medicalRecord = await _context.MedicalRecords.FindAsync(id);

            if (medicalRecord == null)
            {
                return NotFound();
            }

            return medicalRecord;
        }

        // PUT: api/MedicalRecords/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMedicalRecord(int id, MedicalRecord medicalRecord)
        {
            if (id != medicalRecord.MedicalrecordId)
            {
                return BadRequest();
            }

            var existingRecord = await _context.MedicalRecords
                .Include(mr => mr.Appointment)
                .Include(mr => mr.Patient)
                .FirstOrDefaultAsync(mr => mr.MedicalrecordId == id);

            if (existingRecord == null)
            {
                return NotFound();
            }

            // Update existing record properties
            existingRecord.AppointmentId = medicalRecord.AppointmentId;
            existingRecord.PatientId = medicalRecord.PatientId;
            existingRecord.DoctorId = medicalRecord.DoctorId;
            existingRecord.VisitTime = medicalRecord.VisitTime;
            existingRecord.Symptoms = medicalRecord.Symptoms;
            existingRecord.Diagnosis = medicalRecord.Diagnosis;
            existingRecord.Treatment = medicalRecord.Treatment;

            // Load related entities if necessary
            existingRecord.Appointment = await _context.Appointments.FindAsync(medicalRecord.AppointmentId);
            existingRecord.Patient = await _context.Users.FindAsync(medicalRecord.PatientId);

            if (existingRecord.DoctorId.HasValue)
            {
                existingRecord.Doctor = await _context.Users.FindAsync(medicalRecord.DoctorId.Value);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MedicalRecordExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/MedicalRecords
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MedicalRecord>> PostMedicalRecord(MedicalRecord medicalRecord)
        {
            if (_context.MedicalRecords == null)
            {
                return Problem("Entity set 'G1_PRJ_DBContext.MedicalRecords'  is null.");
            }
            _context.MedicalRecords.Add(medicalRecord);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMedicalRecord", new { id = medicalRecord.MedicalrecordId }, medicalRecord);
        }

        // DELETE: api/MedicalRecords/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedicalRecord(int id)
        {
            if (_context.MedicalRecords == null)
            {
                return NotFound();
            }
            var medicalRecord = await _context.MedicalRecords.FindAsync(id);
            if (medicalRecord == null)
            {
                return NotFound();
            }

            _context.MedicalRecords.Remove(medicalRecord);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MedicalRecordExists(int id)
        {
            return (_context.MedicalRecords?.Any(e => e.MedicalrecordId == id)).GetValueOrDefault();
        }
    }

    public class Doctor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Specialist { get; set; }
    }

    public class BasicAppointment
    {
        public string Id { get; set; }
        public int Specialization { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string DoctorSpecialization { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public string PatientAddress { get; set; }
        public string PatientPhone { get; set; }
        public string PatientEmail { get; set; }
        public DateTime PatientDob { get; set; }
        public DateTime RequestTime { get; set; }
    }
}
