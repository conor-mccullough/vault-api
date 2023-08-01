using Vault;
using Vault.Client;
using Vault.Model;

namespace Example
{
    public class Example
    {
        public static void Main()
        {
            string address = "http://127.0.0.1:8200";
            VaultConfiguration config = new VaultConfiguration(address);

            VaultClient vaultClient = new VaultClient(config);
            //vaultClient.SetToken("root");
            try
            {
                var secretData = new Dictionary<string, string> { { "mypass", "pass" } };

                // Write a secret
                var kvRequestData = new KvV2WriteRequest(secretData);

                vaultClient.Secrets.KvV2Write("mypath", kvRequestData);

                // Read a secret
                VaultResponse<KvV2ReadResponse> resp = vaultClient.Secrets.KvV2Read("mypath");
                Console.WriteLine(resp.Data.Data);
            }
            catch (VaultApiException e)
            {
                Console.WriteLine("Failed to read secret with message {0}", e.Message);
            }
        }
    }
}