﻿using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using PosMaster.Dal;
using PosMaster.Dal.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PosMaster.Services
{
	public interface IEmailService
	{
		Task<ReturnData<string>> SendAsync(EmailAddress address, string subject, string message);
	}

	public class EmailService : IEmailService
	{
		private readonly DatabaseContext _context;
		public EmailService(DatabaseContext context)
		{
			_context = context;
		}
		public async Task<ReturnData<string>> SendAsync(EmailAddress address, string subject, string content)
		{
			var res = new ReturnData<string>();
			try
			{
				var emailMessage = new EmailMessage
				{
					Content = content,
					Subject = subject,
					ToAddresses = new List<EmailAddress>() { address }
				};

				var settings = await _context.EmailSettings.
					FirstOrDefaultAsync(s => s.ClientId.Equals(address.ClientId));
				if (settings == null)
				{
					res.Message = "Email settings not Found";
					return res;
				}
				var message = new MimeMessage();
				message.To.AddRange(emailMessage.ToAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
				message.From.Add(new MailboxAddress(settings.SenderFromName, settings.SenderFromEmail));
				message.Subject = emailMessage.Subject;

				var builder = new BodyBuilder();
				//var image = builder.LinkedResources.Add(emailMessage.Logo); 
				builder.HtmlBody = emailMessage.Content;

				message.Body = builder.ToMessageBody();

				using (var emailClient = new SmtpClient())
				{
					var smtpServer = settings.SmtpServer;
					var smtpPort = int.Parse(settings.SmtpPort);
					var smtpUsername = settings.SmtpUsername;
					var smtpPassword = settings.SmtpPassword;
					try
					{
						emailClient.Connect(smtpServer, smtpPort, settings.SocketOptions);
						emailClient.AuthenticationMechanisms.Remove("XOAUTH2");
						emailClient.Authenticate(smtpUsername, smtpPassword);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex);
						res.Message = "Connection failed. Try again later";
						res.ErrorMessage = ex.Message;
						return res;
					}
					await emailClient.SendAsync(message);
					await emailClient.DisconnectAsync(true);
				}
				await AddNotificationAsync(emailMessage);
				res.Success = true;
				res.Message = "Sent";
				res.Data = message.MessageId;
				return res;
			}
			catch (Exception)
			{

				throw;
			}
		}

		private async Task<ReturnData<string>> AddNotificationAsync(EmailMessage message)
		{
			var res = new ReturnData<string>();
			try
			{
				var notifications = new List<Notification>();
				foreach (var address in message.ToAddresses)
				{
					notifications.Add(new Notification
					{
						Title = message.Subject,
						Content = message.Content,
						UserId = address.Id,
						Personnel = message.Personnel,
						ClientId = address.ClientId,
						InstanceId = address.InstanceId
					});
				}
				await _context.Notifications.AddRangeAsync(notifications);
				await _context.SaveChangesAsync();
				res.Message = "Added";
				res.Success = true;
				return res;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				res.Message = "Error occured. Try later";
				res.ErrorMessage = e.Message;
				return res;
			}
		}

	}

}
