namespace OAuth.WebApi
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Owin.Security.Infrastructure;

    internal class SimpleRefreshTokenProvider : IAuthenticationTokenProvider
    {
        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }

        public Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var clientid = context.Ticket.Properties.Dictionary["as:client_id"];

            if (string.IsNullOrEmpty(clientid))
            {
                return null;
            }

            var refreshTokenId = Guid.NewGuid().ToString("n");
            var refreshTokenLifeTime = context.OwinContext.Get<string>("as:clientRefreshTokenLifeTime");

            var token = new RefreshToken()
                            {
                                Id = Helper.GetHash(refreshTokenId),
                                ClientId = clientid,
                                Subject = context.Ticket.Identity.Name,
                                IssuedUtc = DateTime.UtcNow,
                                ExpiresUtc = DateTime.UtcNow.AddMinutes(Convert.ToDouble(refreshTokenLifeTime))
                            };

            context.Ticket.Properties.IssuedUtc = token.IssuedUtc;
            context.Ticket.Properties.ExpiresUtc = token.ExpiresUtc;

            token.ProtectedTicket = context.SerializeTicket();

            var result = AuthRepo.Instance.AddRefereshToken(token);

            if (result)
            {
                context.SetToken(refreshTokenId);
            }

            return Task.FromResult<object>(null);
        }
    

    public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }

        public Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            var hashedTokenId = Helper.GetHash(context.Token);
            var refreshToken = AuthRepo.Instance.FindRefreshToken(hashedTokenId);

                if (refreshToken != null)
                {
                    //Get protectedTicket from refreshToken class
                    context.DeserializeTicket(refreshToken.ProtectedTicket);
                    var result = AuthRepo.Instance.RemoveRefreshToken(hashedTokenId);
                }

            return Task.FromResult<object>(null);
        }
    }
}