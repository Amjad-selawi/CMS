﻿using CMS.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailToInterviewer(string interviewerEmail, InterviewsDTO interview, EmailDTOs emailModel);

        Task SendEmailToInterviewers(List<string> interviewersEmails, InterviewsDTO interview, EmailDTOs emailModel);

        Task<string> GetInterviewerEmail(string interviewerId);

        Task<string> GetHREmail();

        Task<string> GetArchiEmail();

        Task<string> GetGMEmail();

        string GetLoggedInUserName();

        Task ReminderJobAsync(string interviewerId, InterviewsDTO collection);

        void ScheduleInterviewReminder(InterviewsDTO collection);

        Task SendInterviewReminderEmail(InterviewsDTO collection);

        void RetryFailedEmails(EmailDTOs emailmodel);

        Task ResendFailedEmail(EmailDTOs emailToResend);


    }
}
