using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Pannotation.Common.Constants;
using Pannotation.Common.Utilities;
using Pannotation.DAL.Abstract;
using Pannotation.Domain.Entities.Identity;
using Pannotation.Models.Enums;
using Pannotation.Models.ResponseModels;
using Pannotation.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Pannotation.Services.Services
{
    public class JWTService : IJWTService
    {
        private UserManager<ApplicationUser> _userManager = null;
        private HashUtility _hashService = null;
        private IUnitOfWork _unitOfWork;

        public JWTService(UserManager<ApplicationUser> userManager, HashUtility hashService, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _hashService = hashService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ClaimsIdentity> GetIdentity(ApplicationUser user, bool isRefreshToken)
        {
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var claims = new List<Claim>
                    {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim("isRefresh", isRefreshToken.ToString())
                    };

                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role));
                }

                ClaimsIdentity claimsIdentity =
                    new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                        ClaimsIdentity.DefaultRoleClaimType);

                return claimsIdentity;
            }

            return null;
        }

        public JwtSecurityToken CreateToken(DateTime now, ClaimsIdentity identity, DateTime lifetime)
        {
            return new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: lifetime,
                    //signingCredentials: AuthOptions.GetSigningCredentials()
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
                    );
        }

        public async Task<TokenResponseModel> CreateUserTokenAsync(ApplicationUser user, int? accessTokenLifetime = null, bool isRefresh = false)
        {
            DateTime now = DateTime.UtcNow;

            #region remove old tokens

            var tokens = _unitOfWork.UserTokenRepository.Get(x => x.IsActive && x.UserId == user.Id)
                .TagWith(nameof(CreateUserTokenAsync) + "_GetUsersTokens")
                .ToList();

            var tokensToRemove = new List<int>();

            foreach (var token in tokens)
            {
                token.IsActive = false;

                if (token.Type == TokenType.RefreshToken)
                {
                    if (DateTimeOffset.Compare(token.ExpiresDate, DateTime.UtcNow) <= 0)
                        tokensToRemove.Add(token.Id);
                }
                else if (!isRefresh)
                {
                    token.DisposedAt = DateTime.UtcNow;
                    _unitOfWork.UserTokenRepository.Update(token);
                }
            }

            foreach (var tokenId in tokensToRemove)
            {
                _unitOfWork.UserTokenRepository.DeleteById(tokenId);
            }

            #endregion

            #region create access token

            var identity = await GetIdentity(user, false);

            if (identity == null)
            {
                throw new Exception("User not found");
            }

            var lifetime = accessTokenLifetime.HasValue && accessTokenLifetime.Value != 0 ? now.Add(TimeSpan.FromSeconds(accessTokenLifetime.Value)) : now.Add(TimeSpan.FromDays(AuthOptions.ACCESS_TOKEN_LIFETIME));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(CreateToken(now, identity, lifetime));

            user.Tokens.Add(new UserToken
            {
                ExpiresDate = lifetime,
                IsActive = true,
                TokenHash = _hashService.GetHash(encodedJwt),
                Type = TokenType.AccessToken,
                CreatedAt = DateTime.UtcNow
            });

            #endregion

            #region create refresh token

            identity = await GetIdentity(user, true);

            var Rlifetime = now.Add(TimeSpan.FromDays(AuthOptions.REFRESH_TOKEN_LIFETIME));
            //var Rlifetime = now.Add(TimeSpan.FromMinutes(5));

            var encodedRjwt = new JwtSecurityTokenHandler().WriteToken(CreateToken(now, identity, Rlifetime));

            user.Tokens.Add(new UserToken
            {
                ExpiresDate = Rlifetime,
                IsActive = true,
                TokenHash = _hashService.GetHash(encodedRjwt),
                Type = TokenType.RefreshToken,
                CreatedAt = DateTime.UtcNow
            });

            #endregion

            TokenResponseModel response = new TokenResponseModel
            {
                AccessToken = encodedJwt,
                ExpireDate = lifetime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                RefreshToken = encodedRjwt,
                Type = "Bearer"
            };

            _unitOfWork.UserRepository.Update(user);

            return response;
        }

        public async Task<LoginResponseModel> BuildLoginResponse(ApplicationUser user, int? accessTokenLifetime = null)
        {
            user.LastVisitAt = DateTime.UtcNow;
            _unitOfWork.UserRepository.Update(user);

            TokenResponseModel tokenModel = await CreateUserTokenAsync(user, accessTokenLifetime);
            _unitOfWork.SaveChanges();

            var roles = await _userManager.GetRolesAsync(user);
            return new LoginResponseModel
            {
                Id = user.Id,
                Email = user.Email ?? null,
                PhoneNumber = user.PhoneNumber ?? null,
                Token = tokenModel,
                Role = (roles != null) ? roles.SingleOrDefault() : "none"
            };
        }

        public async Task ClearUserTokens(ApplicationUser user)
        {
            var tokensToRemove = new List<int>();

            var tokens = user.Tokens.ToList();

            foreach (var token in tokens)
            {
                if (token.Type == TokenType.RefreshToken)
                {
                    if (DateTime.Compare(token.ExpiresDate, DateTime.UtcNow) <= 0)
                    {
                        tokensToRemove.Add(token.Id);
                    }
                }
                else if (token.IsActive)
                {
                    token.DisposedAt = DateTime.UtcNow;
                }

                token.IsActive = false;
            }

            _unitOfWork.UserRepository.Update(user);

            foreach (var tokenId in tokensToRemove)
            {
                _unitOfWork.UserTokenRepository.DeleteById(tokenId);
            }

            _unitOfWork.SaveChanges();
        }
    }
}
