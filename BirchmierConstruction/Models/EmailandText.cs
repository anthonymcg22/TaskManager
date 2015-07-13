using BirchmierConstruction.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace BirchmierConstruction.Models
{
    public class EmailandText
    {
        public MailAddress FromAddress { get; set; }
        public string Name { get; set; }
        public SmtpClient smtp { get; set; }
        public void ProvideCredentials(string name, string email, string password)
        {
            FromAddress = new MailAddress(email, name);
            smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(FromAddress.Address, password),
                Timeout = 15000
            };
        }

        public Dictionary<CellProvider, string> CellEmailPostfix = new Dictionary<CellProvider, string>
        {
            {CellProvider.Alltel, "@message.alltel.co"},
            {CellProvider.AT_T, "@txt.att.net"},
            {CellProvider.Boost, "@myboostmobile.com"},
            {CellProvider.Nextel, "@messaging.nextel.com"},
            {CellProvider.Sprint_PCS, "@messaging.sprintpcs.com"},
            {CellProvider.T_Mobile, "@tmomail.net"},
            {CellProvider.US_Cellular, "@email.uscc.net"},
            {CellProvider.Verizon, "@vtext.com"},
            {CellProvider.Virgin_Mobile, "@vmobl.com"}
        };
    }
}