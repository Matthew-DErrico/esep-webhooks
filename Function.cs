using System.Text;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using System.Net.Http;
using System.IO;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace EsepWebhook;

public class Function
{
    /// <summary>
    /// A simple function that processes a webhook payload and sends a message to Slack
    /// </summary>
    /// <param name="input">The event input from GitHub Webhook</param>
    /// <param name="context">Lambda context for logging</param>
    /// <returns></returns>
    public string FunctionHandler(object input, ILambdaContext context)
    {
        context.Logger.LogInformation($"FunctionHandler received: {input}");

        // Deserialize the input to dynamic to extract necessary information
        dynamic json = JsonConvert.DeserializeObject<dynamic>(input.ToString());

        // Construct the message payload for Slack
        string payload = $"{{'text':'Issue Created: {json.issue.html_url}'}}";
        
        // Send the message to Slack
        var client = new HttpClient();
        var webRequest = new HttpRequestMessage(HttpMethod.Post, Environment.GetEnvironmentVariable("SLACK_URL"))
        {
            Content = new StringContent(payload, Encoding.UTF8, "application/json")
        };
    
        // Send the request and read the response
        var response = client.Send(webRequest);
        using var reader = new StreamReader(response.Content.ReadAsStream());
            
        return reader.ReadToEnd();
    }
}
