﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Clinic_Management.Models;
using System.Composition.Convention;

namespace Clinic_Management.Pages.Appointements
{
    public class EditModel : PageModel
    {
        private readonly Clinic_Management.Models.G1_PRJ_DBContext _context;

        public EditModel(Clinic_Management.Models.G1_PRJ_DBContext context)
        {
            _context = context;
        }

        public Appointment Appointment { get; set; } = default!;
        public User patient { get; set; } = default!;
        public IList<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
        public IList<AppointmentStatus> StatusList { get; set; } = new List<AppointmentStatus>();
        public IList<Staff> doctorList { get; set; }= new List<Staff>();
        public IList<Branch> branchList { get; set; } = new List<Branch>();
        public IList<Specialist> specialistList { get; set; } = new List<Specialist>();
        [BindProperty]
        public int requestedTime { get; set; }

        [BindProperty]
        public DateTime requestedDate { get; set; }
        
        [BindProperty]
        public int doctorId { get; set; }
        [BindProperty]
        public int branchId {  get; set; }
        [BindProperty]
        public int specialistId { get; set; }
        public IList<User> DoctorList { get; set; }

        [BindProperty]
        public string patientError { get; set; }

        [BindProperty]
        public string appointmentError { get; set; }

        [BindProperty]
        public string symptoms { get; set; }

        public string doctorError {  get; set; }
        public string errorMessage { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Appointments == null)
            {
                return NotFound();
            }
            branchList = _context.Branches.ToList();
            specialistList = _context.Specialists.ToList();
            doctorList = _context.Staff.Include(d => d.DoctorDepartment).Include(d => d.DoctorSpecialistNavigation).Include(d => d.User).Where(d => d.User.RoleId == 2).ToList();
            
            var appointment = await _context.Appointments.Include(a => a.Doctor).Include(a => a.Branch)
                .Include(a => a.SpecialistNavigation).Include(a => a.StatusNavigation)
                .Include(a => a.Doctor).Include(a => a.Receptionist).FirstOrDefaultAsync(m => m.AppointmentId == id);
            if (appointment.PatientId != 0)
            {
                patient = _context.Users.Include(p => p.Patient).Include(p => p.Patient.MedicalRecords).FirstOrDefault(p => p.UserId == appointment.PatientId);
                if (patient != null) {
                    MedicalRecords = _context.MedicalRecords.Where(m => m.PatientId == patient.UserId).ToList();
                }
                
            }        
            if (appointment == null)
            {
                return NotFound();
            }
            else
            {
                Appointment = appointment;
            }
            branchId = Appointment.BranchId;
            switch (Appointment.RequestedTime.Hour)
            {
                case 7:
                    requestedTime = 1;
                    break;
                case 8:
                    requestedTime = 2;
                    break;
                case 9:
                    requestedTime = 3;
                    break;
                case 10:
                    requestedTime = 4;
                    break;
                case 13:
                    requestedTime = 5;
                    break;
                case 14:
                    requestedTime = 6;
                    break;
                case 15:
                    requestedTime = 7;
                    break;
                case 16:
                    requestedTime = 8;
                    break;

            }
            StatusList = _context.AppointmentStatuses.ToList();
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(int? AppointmentId)
        {
            Appointment = await _context.Appointments.Include(a => a.Doctor).Include(a => a.Branch)
                .Include(a => a.SpecialistNavigation).Include(a => a.StatusNavigation)
                .Include(a => a.Doctor).Include(a => a.Receptionist).FirstOrDefaultAsync(m => m.AppointmentId == AppointmentId);

            StatusList = _context.AppointmentStatuses.ToList();
            branchList = _context.Branches.ToList();
            specialistList = _context.Specialists.ToList();
            doctorList = _context.Staff.Include(d => d.DoctorDepartment).Include(d => d.DoctorSpecialistNavigation).Include(d => d.User).Where(d => d.User.RoleId == 2).ToList();
            var a = _context.Appointments.FirstOrDefault(a => a.AppointmentId == AppointmentId);

            bool isAppointmentError = false;

            var doctor = _context.Staff.Include(u => u.User).FirstOrDefault(u => u.UserId == doctorId);
            if(doctor != null)
            {
                if (doctor?.DoctorDepartmentId != branchId)
                {
                    appointmentError += "This doctor is currently working on another branch";
                    isAppointmentError = true;
                }

                if (doctor?.DoctorSpecialist != specialistId)
                {
                    appointmentError += ", This specialist is not suitable for this doctor";
                    isAppointmentError = true;
                }
                else
                {
                    switch (requestedTime)
                    {
                        case 1:
                            requestedDate = requestedDate.Date.AddHours(7);
                            break;
                        case 2:
                            requestedDate = requestedDate.Date.AddHours(8);
                            break;
                        case 3:
                            requestedDate = requestedDate.Date.AddHours(9);
                            break;
                        case 4:
                            requestedDate = requestedDate.Date.AddHours(10);
                            break;
                        case 5:
                            requestedDate = requestedDate.Date.AddHours(13);
                            break;
                        case 6:
                            requestedDate = requestedDate.Date.AddHours(14);
                            break;
                        case 7:
                            requestedDate = requestedDate.Date.AddHours(15);
                            break;
                        case 8:
                            requestedDate = requestedDate.Date.AddHours(16);
                            break;

                    }
                    var appointment = _context.Appointments.FirstOrDefault(a => (a.DoctorId == doctorId && a.RequestedTime.Equals(requestedDate) && a.Status == 1) && a.AppointmentId != Appointment.AppointmentId);
                    if (appointment != null)
                    {
                        appointmentError += "The doctor already has an appointment at this time";
                        isAppointmentError = true;
                    }
                    else
                    {
                        a.Description = symptoms;
                        a.DoctorId = doctorId;
                        a.BranchId = branchId;
                        a.Specialist = specialistId;
                        a.RequestedTime = requestedDate;
                    }

                }
            }
            if (isAppointmentError)
            {
                errorMessage = "Error occurs";
                return Page();
            }
            else
            {
                _context.Appointments.Update(a);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index", new { Message = "Appointment updated!" });
            }
        }
        public async Task<IActionResult> OnGetCancelAppointmentAsync(int id)
        {
            var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.AppointmentId == id);
            if (appointment != null)
            {
                appointment.Status = 3;
                await _context.SaveChangesAsync();
            }
            return RedirectToPage("./Index", new { Message = "Appointment canceled!" });
        }
    }
}
