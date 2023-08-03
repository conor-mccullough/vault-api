using Vault;
using Vault.Api;
using Vault.Client;
using Vault.Model;

public class OidcToken
{
    public static void Main()
    {
        string address = "http://127.0.0.1:8200";
        VaultConfiguration config = new VaultConfiguration(address);

        VaultClient vaultClient = new VaultClient(config);
        vaultClient.SetToken("root");
        --
        Vault.Api.Auth

            // make the HTTP request
            var response = await this.AsynchronousClient.GetAsync<StandardListResponse>("/auth/{jwt_mount_path}/role/", requestOptions, cancellationToken).ConfigureAwait(false);

            if (this.ExceptionFactory != null)
            {
                Exception exception = this.ExceptionFactory("JwtListRoles", response);
                if (exception != null) throw exception;
            }

            return ClientUtils.ToVaultResponse<StandardListResponse>(response.RawContent);
        }

        --
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
