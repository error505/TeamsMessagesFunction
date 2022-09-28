using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using MicrosoftTeamsNotification.Models;
using MicrosoftTeamsNotification.Tasks;
using Newtonsoft.Json;

namespace MicrosoftTeamsNotification;

public class ProcessMessage
{
	[FunctionName("ProcessMessage")]
	public async Task Run([ServiceBusTrigger("testqueue", Connection = "ServiceBusConnection")] string myQueueItem, ILogger log)
	{
		try
		{
			var message = JsonConvert.DeserializeObject<MessageToProcess>(myQueueItem);
			//Save message to database
			//await _context.Message.AddAsync(message, cancellationToken);
			//await _context.SaveChangesAsync(cancellationToken);

			//Send message to teams
			await SendTeamsWebHookNotificationMessage(message);
		}
		catch (Exception ex)
		{
			log.LogError(ex.ToString());
		}
	}

	private async Task SendTeamsWebHookNotificationMessage(MessageToProcess message)
	{
			var webHookUrl = $"{Environment.GetEnvironmentVariable("WebHook")}";
			TeamsMessage teamsNotificationMessage = CreateTeamsMessage(message);
			await MessageClient.SendAsync(webHookUrl, teamsNotificationMessage);
	}

	private TeamsMessage CreateTeamsMessage(MessageToProcess message)
	{
		var teamsNotificationMessage = new TeamsMessage();
		teamsNotificationMessage.ThemeColor = TeamsMessageConstants.ThemeColor;
		teamsNotificationMessage.Summary = $"The item {message.Name} has been added to cart. You can see the details here.";
		teamsNotificationMessage.Sections.Add(new Section
		{
			ActivityTitle = $"The item {message.Name} has been added to cart. You can see the details here.",
			ActivitySubtitle = $"Subtitle",
			ActivityImage = TeamsMessageConstants.Logo,
			Markdown = true,
			Facts = new List<Fact>()
				{
					new()
					{
						Name = TeamsMessageConstants.Item,
						Value = ""
					},
					new()
					{
						Name = TeamsMessageConstants.DeliveryDate,
						Value =  DateTime.Now.ToString()
					}
				},
		});
		return teamsNotificationMessage;
	}
}

