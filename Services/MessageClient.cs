using MicrosoftTeamsNotification.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MicrosoftTeamsNotification.Tasks
{
    public static class MessageClient
    {
        public static async Task<bool> SendAsync(string url, TeamsMessage card)
        {
            var json = JsonSerializer.Serialize(card);
            var client = new HttpClient();
            client.BaseAddress = new Uri(url);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var result = await client.PostAsync(client.BaseAddress, content);
            return result.IsSuccessStatusCode;
        }
    }
}