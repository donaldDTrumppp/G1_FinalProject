﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Clinic_Management.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Clinic_Management.Pages.MedicalRecords
{
    public class IndexModel : PageModel
    {
        private readonly Clinic_Management.Models.G1_PRJ_DBContext _context;

        public IndexModel(Clinic_Management.Models.G1_PRJ_DBContext context)
        {
            _context = context;
        }

       

        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortField { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; }
        public IList<MedicalRecord> MedicalRecord { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public int totalRecords { get; set; }

        public async Task OnGetAsync()
        {
            var query = _context.MedicalRecords
                .Include(m => m.Appointment)
                .Include(m => m.Doctor)
                .Include(m => m.Patient).AsQueryable();

            if (!string.IsNullOrEmpty(SearchString))
            {
                query = query.Where(m =>
                    m.Symptoms.ToLower().Contains(SearchString.ToLower()) ||
                    m.Diagnosis.ToLower().Contains(SearchString.ToLower()) ||
                    m.Treatment.ToLower().Contains(SearchString.ToLower()) ||
                    m.Doctor.Name.ToLower().Contains(SearchString.ToLower()) ||
                    m.Appointment.PatientName.ToLower().Contains(SearchString.ToLower())
                );
            }

            switch (SortField)
            {
                case "VisitTime":
                    query = SortOrder == "desc" ? query.OrderByDescending(r => r.VisitTime) : query.OrderBy(r => r.VisitTime);
                    break;
                case "Doctor":
                    query = SortOrder == "desc" ? query.OrderByDescending(r => r.Doctor.Name) : query.OrderBy(r => r.Doctor.Name);
                    break;
                case "Patient":
                    query = SortOrder == "desc" ? query.OrderByDescending(r => r.Appointment.PatientName) : query.OrderBy(r => r.Appointment.PatientName);
                    break;
                default:
                    query = query.OrderBy(r => r.VisitTime); // Default
                    break;
            }

            totalRecords = await query.CountAsync();
            
            //  MedicalRecord = await query.ToListAsync();
            // Apply pagination
            MedicalRecord = await query.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToListAsync();
        }
    }
}
