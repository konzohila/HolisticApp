using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Newtonsoft.Json.Linq;

namespace HolisticApp.Services
{
public class AwsSecretsService
{
    private readonly AmazonSecretsManagerClient _secretsClient;

    public AwsSecretsService()
    {
        _secretsClient = new AmazonSecretsManagerClient(
            "DEIN_ACCESS_KEY",  // 🔹 Setze deinen AWS Access Key
            "DEIN_SECRET_KEY",  // 🔹 Setze dein AWS Secret Key
            RegionEndpoint.EUCentral1  // 🔹 Stelle sicher, dass dies mit deiner AWS-Region übereinstimmt
        );
    }

    public async Task<JObject> GetSecretAsync(string secretName)
    {
        try
        {
            var request = new GetSecretValueRequest { SecretId = secretName };
            var response = await _secretsClient.GetSecretValueAsync(request);

            if (response.SecretString != null)
            {
                return JObject.Parse(response.SecretString);
            }
            else
            {
                throw new Exception("Secret is empty!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Fehler beim Laden des Secrets: {ex.Message}");
            throw;
        }
    }
}
}