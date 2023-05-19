using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Gmail.v1;
using Google.Apis.Util;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public class GoogleAuthHelper
    {

        private static string _clientId = ConfigurationManager.AppSettings["clientId"].ToString();
        private static string _clientSecret = ConfigurationManager.AppSettings["clientSecret"].ToString();

        private UserCredential credential;

        private async Task<UserCredential> GetCredentialAsync()
        {
            string[] scopes = { GmailService.Scope.GmailReadonly };

            using (var stream = new FileStream("/Config/tokens.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "/Config/";

                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    scopes,
                    "usuario",
                    CancellationToken.None,
                    new FileDataStore(credPath)
                );

                if (credential.Token.IsExpired(SystemClock.Default))
                {
                    bool refreshTokenResult = await credential.RefreshTokenAsync(CancellationToken.None);

                    if (!refreshTokenResult)
                    {
                        throw new ApplicationException("Error al refrescar el token");
                    }
                }
            }

            return credential;
        }

        public async Task AuthenticateAsync()
        {
            try
            {
                credential = await GetCredentialAsync();

            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
