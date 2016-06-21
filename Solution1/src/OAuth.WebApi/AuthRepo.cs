using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OAuth.WebApi
{
    using System.Collections.Concurrent;
    using System.Runtime.CompilerServices;

    public class AuthRepo
    {
        public List<Client> Clients;
        private static AuthRepo instance = null;
        private static readonly object padlock = new object();

        private List<RefreshToken> refreshTokens; 

        private AuthRepo()
        {
             this.Clients = new List<Client>();
            this.refreshTokens = new List<RefreshToken>();
         }

        public static AuthRepo Instance
        {
            get
            {
                lock (padlock)
                {
                    return instance ?? (instance = new AuthRepo());
                }
            }
        }

        public bool AddRefereshToken(RefreshToken refreshToken)
        {
            var existingToken =
                this.refreshTokens.FirstOrDefault(
                    t => t.Subject == refreshToken.Subject && t.ClientId == refreshToken.ClientId);
            if (existingToken != null)
            {
                RemoveRefreshToken(existingToken.Id);
            }

            this.refreshTokens.Add(refreshToken);
            return true;
        }

        public bool RemoveRefreshToken(string refreshTokenId)
        {
            var tokentobeRemoved = this.refreshTokens.FirstOrDefault(t => t.Id == refreshTokenId);
            if (tokentobeRemoved != null)
            {
                this.refreshTokens.Remove(tokentobeRemoved);
                return true;
            }
            return false;
        }

        public RefreshToken FindRefreshToken(string refreshTokenId)
        {
            return this.refreshTokens.FirstOrDefault(t => t.Id == refreshTokenId);
        }

        public  void AddDefaultClient()
        {
            this.Clients.Add(new Client
                                 {
                                     Id = "AAE-ThickClient", 
                                     Secret = Helper.GetHash("123@abc"), //node lock id
                                     ApplicationType = "Native",
                                     Active = true,
                                     RefreshTokenLifeTime = 14400,
                                     AllowedOrigin = "*"
                                 });
        }
    }
}