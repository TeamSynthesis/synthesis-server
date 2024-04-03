
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using synthesis.api.Mappings;

namespace synthesis.api.Services.Email;

public interface IEmailService
{
    Task<GlobalResponse<string>> SendConfirmationEmail(string body, string recepientEmail);
    Task<GlobalResponse<string>> SendTeamInvitationEmail(string link, string teamToJoin, List<string> recepientEmails);
    Task SendOnBoardingEmail();
}
public class EmailService : IEmailService
{
    private readonly string _mailgunApiKey = "5d65ae6dc3d622c13f0768d8397a44e5-b7b36bc2-947be266";
    private readonly string _mailgunDomain = "sandboxebf1c09e8424413394a98a2ce8e2ae2c.mailgun.org";

    private readonly IHttpClientFactory _httpClient;

    public EmailService(IHttpClientFactory httpClient)
    {
        _httpClient = httpClient;
    }


    public async Task<GlobalResponse<string>> SendConfirmationEmail(string link, string recepientEmail)
    {

        var options = new RestClientOptions
        {
            Authenticator = new HttpBasicAuthenticator("api", _mailgunApiKey),
            BaseUrl = new Uri("https://api.mailgun.net/v3")
        };

        var client = new RestClient(options);

        var linkParams = new { confirmation_link = link };
        var paramJson = JsonConvert.SerializeObject(linkParams);
        RestRequest request = new RestRequest();
        request.AddParameter("domain", "manasseh.me", ParameterType.UrlSegment);
        request.Resource = "{domain}/messages";
        request.AddParameter("from", "Team Synthesis <postmaster@manasseh.me>");
        request.AddParameter("to", $"You <{recepientEmail}>");
        request.AddParameter("subject", "Email Confirmation");
        request.AddParameter("template", "confirm-email");
        request.AddParameter("h:X-Mailgun-Variables", paramJson);
        request.Method = Method.Post;

        var response = await client.ExecuteAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return new GlobalResponse<string>(true, "email sent");
        }
        else
        {
            return new GlobalResponse<string>(false, "an error occured", errors: [response.ErrorMessage]);
        }

    }

    public async Task<GlobalResponse<string>> SendTeamInvitationEmail(string link, string teamToJoin, List<string> recepientEmails)
    {

        var options = new RestClientOptions
        {
            Authenticator = new HttpBasicAuthenticator("api", _mailgunApiKey),
            BaseUrl = new Uri("https://api.mailgun.net/v3")
        };

        var client = new RestClient(options);

        var linkParams = new { invitation_link = link, team = teamToJoin };
        var paramJson = JsonConvert.SerializeObject(linkParams);
        var emails = JsonConvert.SerializeObject(recepientEmails);
        RestRequest request = new RestRequest();
        request.AddParameter("domain", "manasseh.me", ParameterType.UrlSegment);
        request.Resource = "{domain}/messages";
        request.AddParameter("from", "Team Synthesis <postmaster@manasseh.me>");
        foreach (var receipient in recepientEmails)
        {
            request.AddParameter("to", $"You <{receipient}>");
        }
        request.AddParameter("subject", "Team Invitation");
        request.AddParameter("template", "invitation_email");
        request.AddParameter("h:X-Mailgun-Variables", paramJson);
        request.Method = Method.Post;

        var response = await client.ExecuteAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return new GlobalResponse<string>(true, "email sent");
        }
        else
        {
            return new GlobalResponse<string>(false, "an error occured", errors: [response.ErrorMessage]);
        }

    }


    public Task SendOnBoardingEmail()
    {
        throw new NotImplementedException();
    }
}