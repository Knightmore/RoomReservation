#region Copyright © 2023 Patrick Borger - https: //github.com/Knightmore

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 
// Author: Patrick Borger
// GitHub: https://github.com/Knightmore
// Created: 17.01.2023
// Modified: 10.02.2023

#endregion

using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using RoomReservation.Configuration;

namespace RoomReservation.Services;

public class MailSender : IMailSender
{
    private readonly MailSettings _mailSettings;

    public MailSender(IOptionsSnapshot<MailSettings> mailSettings)
    {
        _mailSettings = mailSettings.Value;
    }

    public async Task<bool> SendMailAsync(string toEmail, string subject, string linkText, string confirmationLink)
    {
        using var mail = new MimeMessage();
        mail.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.From));
        mail.Sender = new MailboxAddress(_mailSettings.DisplayName, _mailSettings.From);
        mail.To.Add(MailboxAddress.Parse(toEmail));

        mail.Subject = subject;

        var body = new BodyBuilder();
        body.HtmlBody = $"<a href=\"{confirmationLink}\">{linkText}</a>";
        mail.Body     = body.ToMessageBody();

        using var client = new SmtpClient();
        if (_mailSettings.UseSSL)
            await client.ConnectAsync(_mailSettings.Server, _mailSettings.Port, SecureSocketOptions.SslOnConnect);
        else if (_mailSettings.UseStartTls)
            await client.ConnectAsync(_mailSettings.Server, _mailSettings.Port, SecureSocketOptions.StartTls);
        else
            await client.ConnectAsync(_mailSettings.Server, _mailSettings.Port, SecureSocketOptions.None);

        if (_mailSettings.JSONPassword)
            await client.AuthenticateAsync(_mailSettings.UserName, _mailSettings.Password);
        else
            await client.AuthenticateAsync(_mailSettings.UserName, Environment.GetEnvironmentVariable("MailerPassword"));

        try
        {
            await client.SendAsync(mail);
            await client.DisconnectAsync(true);
            return true;
        }
        catch (Exception ex)
        {
            // ignored for now. Implement ILogger later
        }

        return false;
    }
}