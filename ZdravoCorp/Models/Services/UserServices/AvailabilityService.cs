using System;
using System.Collections.Generic;
using System.Linq;
using ZdravoCorp.Models.Entities.Users;
using ZdravoCorp.Models.Entities.Appointments;
using ZdravoCorp.Models.Services.AppointmentServices;

namespace ZdravoCorp.Models.Services.UserServices;

public class AvailabilityService
{
    private static bool isDoctorOnExamination(Doctor doctor, DateTime dt)
    {
        foreach (var examination in doctor.Examinations)
        {
            if (dt >= examination.DateTime && dt <= examination.DateTime.AddMinutes(15))
                return true;
            var difference = (dt - examination.DateTime).TotalMinutes;
            if (difference is >= -15 and <= 15)
                return true;
        }
        return false;
    }

    private static bool isDoctorOnOperation(Doctor doctor, DateTime dt)
    {
        foreach (var operation in doctor.Operations)
        {
            var difference = (dt - operation.DateTime).TotalMinutes;
            if (difference <= (double)operation.Duration && difference >= -15)
                return true;
        }
        return false;
    }

    public bool IsDoctorAvailable(Doctor doctor, DateTime dt)
    {
        return !(isDoctorOnExamination(doctor, dt) || isDoctorOnOperation(doctor, dt));
    }
    private static bool isPatientOnExamination(Patient patient, DateTime dt)
    {
        foreach (var examination in patient.Examinations)
        {
            if (dt >= examination.DateTime && dt <= examination.DateTime.AddMinutes(15))
                return true;
            var difference = (dt - examination.DateTime).TotalMinutes;
            if (difference is >= -15 and <= 15)
                return true;
        }

        return false;
    }

    private static bool isPatientOnOperation(Patient patient, DateTime dt)
    {
        foreach (var operation in patient.Operations)
        {
            var difference = (dt - operation.DateTime).TotalMinutes;
            if (difference <= (double)operation.Duration && difference >= -15)
                return true;
        }

        return false;
    }
    
    public bool IsPatientAvailable(Patient patient, DateTime dt)
    {
        return !(isPatientOnExamination(patient, dt) || isPatientOnOperation(patient, dt));
    }


    public static List<Examination> GetExaminationsForDay(Doctor doctor, DateTime dt)
    {
        List<Examination> retVal = new List<Examination>();
        foreach (var examination in doctor.Examinations)
        {
            var difference = (dt - examination.DateTime).TotalHours;
            if (difference is >= 24 and <= 24)
                retVal.Add(examination);
        }
        return retVal;
    }

    public static List<Examination> GetExaminationsForDateRange(Doctor doctor, DateTime startDate, DateTime endDate)
    {
        List<Examination> retVal = new List<Examination>();
        DateTime singleDate = startDate;
        while (singleDate <= endDate)
        {
            retVal.AddRange(GetExaminationsForDay(doctor, singleDate));
            singleDate = singleDate.AddDays(1);
        }
        return retVal;
    }

    public static List<Operation> GetOperationsForDay(Doctor doctor, DateTime dt)
    {
        List<Operation> retVal = new List<Operation>();
        foreach (var operation in doctor.Operations)
        {
            var difference = (dt - operation.DateTime).TotalHours;
            if (difference is >= 24 and <= 24)
                retVal.Add(operation);
        }

        return retVal;
    } 
    public static List<Operation> GetOperationsForDateRange(Doctor doctor, DateTime startDate, DateTime endDate)
    {
        List<Operation> retVal = new List<Operation>();
        DateTime singleDate = startDate;
        while (singleDate <= endDate)
        {
            retVal.AddRange(GetOperationsForDay(doctor, singleDate));
            singleDate = singleDate.AddDays(1);
        }

        return retVal;
    }
    public bool IsDoctorAvailableForDateRange(Doctor d, DateTime startDate, DateTime endDate)
    {
        return !GetExaminationsForDateRange(d, startDate, endDate).Any() && !GetOperationsForDateRange(d, startDate, endDate).Any();
    }
}