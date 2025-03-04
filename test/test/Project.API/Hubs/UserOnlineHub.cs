using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using System.IdentityModel.Tokens.Jwt;
using test.Project.Application.DTOs.Response.UserOnline;

namespace test.Project.API.Hubs
{
    public class UserOnlineHub : Hub
    {   
        private readonly IMemoryCache _memoryCache;
        private const string UsersCountKey = "UsersCount";
        private const string UserAccessTimesKey = "UserAccessTimes";
        private const string UserConnectionsKey = "UserConnections";

        public UserOnlineHub(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        private Dictionary<string, UserAccessInfo> GetUserAccessTimes()
        {
            return _memoryCache.GetOrCreate(UserAccessTimesKey, entry =>
            {
                // entry.SlidingExpiration = TimeSpan.FromMinutes(60);
                return new Dictionary<string, UserAccessInfo>();
            });
        }

        private Dictionary<string, HashSet<string>> GetUserConnections()
        {
            return _memoryCache.GetOrCreate(UserConnectionsKey, entry =>
            {
                // entry.SlidingExpiration = TimeSpan.FromMinutes(60);
                return new Dictionary<string, HashSet<string>>();
            });
        }

        private int GetUsersCount() => _memoryCache.GetOrCreate(UsersCountKey, entry => 0);

        private void SetUsersCount(int count) => _memoryCache.Set(UsersCountKey, count);
     

     
        public async Task SendUsersCount()
        {
            await Clients.All.SendAsync("GetUsersCount");
        }

        public async Task SendAccessTimes()
        {
            var accessTimes = GetUserAccessTimes()
                .Values
                .Select(user => new
                {
                    name = user.Name,
                    email = user.Email,
                    ConnectedTime = FormatTimeSpan(user.ConnectedTime),
                    TotalConnectedTime = CalculateTotalConnectedTime(user),
                    isOnline = user.IsOnline
                });

            await Clients.Caller.SendAsync("GetUserAccessTime");
        }
      
        private string FormatTimeSpan(DateTime time)
        {
            return time.ToString(@"hh\:mm\:ss");
        }

        private string CalculateTotalConnectedTime(UserAccessInfo user)
        {
            var totalTime = user.IsOnline
                ? user.TotalConnectedTime + (DateTime.Now - user.ConnectedTime)
                : user.TotalConnectedTime;

            return totalTime.ToString(@"hh\:mm\:ss");
        }

        private (string name, string email) ExtractUserInfoFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var name = jwtToken.Claims.FirstOrDefault(c => c.Type == "username")?.Value;
            var email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

            return (name, email);
        }

        private void UpdateUserConnection(string email, string connectionId)
        {
            var userConnections = GetUserConnections();
            if (!userConnections.ContainsKey(email))
            {
                userConnections[email] = new HashSet<string>();
            }
            userConnections[email].Add(connectionId);
            _memoryCache.Set(UserConnectionsKey, userConnections);
        }

        private void UpdateUserAccessInfo(string email, string name)
        {
            var userAccessTimes = GetUserAccessTimes();
            if (!userAccessTimes.ContainsKey(email))
            {
                userAccessTimes[email] = new UserAccessInfo
                {
                    Name = name,
                    Email = email,
                    ConnectedTime = DateTime.Now,
                    IsOnline = true
                };
            }
            else if (!userAccessTimes[email].IsOnline)
            {
                userAccessTimes[email].IsOnline = true;
                userAccessTimes[email].ConnectedTime = DateTime.Now;
            }
            _memoryCache.Set(UserAccessTimesKey, userAccessTimes);
        }
        
        public override async Task OnConnectedAsync()
        {
            var token = Context.GetHttpContext()?.Request.Query["access_token"].ToString();

            if (!string.IsNullOrEmpty(token))
            {
                var (name, email) = ExtractUserInfoFromToken(token);
                
                UpdateUserConnection(email, Context.ConnectionId);
                UpdateUserAccessInfo(email, name);
                SetUsersCount(GetUserConnections().Count);
            }

            await base.OnConnectedAsync();
            await SendUsersCount();
            await SendAccessTimes();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var token = Context.GetHttpContext()?.Request.Query["access_token"].ToString();

            if (!string.IsNullOrEmpty(token))
            {
                var (_, email) = ExtractUserInfoFromToken(token);
                var userConnections = GetUserConnections();

                if (userConnections.ContainsKey(email))
                {
                    userConnections[email].Remove(Context.ConnectionId);

                    if (userConnections[email].Count == 0)
                    {
                        userConnections.Remove(email);
                        UpdateOfflineStatus(email);
                    }

                    _memoryCache.Set(UserConnectionsKey, userConnections);
                    SetUsersCount(userConnections.Count);
                }
            }

            await base.OnDisconnectedAsync(exception);
            await SendUsersCount();
        }

        private void UpdateOfflineStatus(string email)
        {
            var userAccessTimes = GetUserAccessTimes();
            if (userAccessTimes.ContainsKey(email))
            {
                var user = userAccessTimes[email];
                user.IsOnline = false;
                user.TotalConnectedTime += DateTime.Now - user.ConnectedTime;
                _memoryCache.Set(UserAccessTimesKey, userAccessTimes);
            }
        }
    }
}