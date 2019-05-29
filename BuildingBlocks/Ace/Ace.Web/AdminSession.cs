using Ace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;


namespace Ace.Web
{
    public class AdminSession : AceSession<string>
    {
        public string UserName { get; set; }
        public string RealName { get; set; }
        public string DepartmentId { get; set; }
        public string DutyId { get; set; }
        public string RoleId { get; set; }
        public string LoginIP { get; set; }
        public DateTime LoginTime { get; set; }
        public bool IsAdmin { get; set; }


        public List<Claim> ToClaims()
        {
            List<Claim> claims = new List<Claim>();

            claims.Add(new Claim("UserId", this.UserId ?? ""));
            claims.Add(new Claim("UserName", this.UserName ?? ""));
            claims.Add(new Claim("RealName", this.RealName ?? ""));
            claims.Add(new Claim("DepartmentId", this.DepartmentId ?? ""));
            claims.Add(new Claim("DutyId", this.DutyId ?? ""));
            claims.Add(new Claim("RoleId", this.RoleId ?? ""));
            claims.Add(new Claim("LoginIP", this.LoginIP ?? ""));
            claims.Add(new Claim("LoginTime", this.LoginTime.TotalMilliseconds().ToString()));
            claims.Add(new Claim("IsAdmin", this.IsAdmin.ToString()));

            return claims;
        }

        public static AdminSession Parse(IPrincipal principal)
        {
            if (principal is ClaimsPrincipal claims)
            {
                AdminSession session = new AdminSession()
                {
                    //UserId = claims.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value ?? "",
                    //UserName = claims.Claims.FirstOrDefault(x => x.Type == "UserName")?.Value ?? "",
                    //RealName = claims.Claims.FirstOrDefault(x => x.Type == "RealName")?.Value ?? "",
                    //DepartmentId = claims.Claims.FirstOrDefault(x => x.Type == "DepartmentId")?.Value ?? "",
                    //DutyId = claims.Claims.FirstOrDefault(x => x.Type == "DutyId")?.Value ?? "",
                    //RoleId = claims.Claims.FirstOrDefault(x => x.Type == "RoleId")?.Value ?? "",
                    //LoginIP = claims.Claims.FirstOrDefault(x => x.Type == "LoginIP")?.Value ?? "",
                    //LoginTime = DateTimeHelper.Parse(long.Parse(claims.Claims.FirstOrDefault(x => x.Type == "LoginTime")?.Value ?? "0")),
                    //IsAdmin = bool.Parse(claims.Claims.FirstOrDefault(x => x.Type == "IsAdmin")?.Value ?? "false"),
                    UserId = claims.Claims.FirstOrDefault(x => x.Type == "sub")?.Value ?? "",
                    UserName = claims.Claims.FirstOrDefault(x => x.Type == "unique_name")?.Value ?? "",
                    RealName = (claims.Claims.FirstOrDefault(x => x.Type == "last_name")?.Value ?? ""+ claims.Claims.FirstOrDefault(x => x.Type == "name")?.Value ?? ""),
                };
                return session;
            }

            throw new ArgumentException(message: "The principal must be a ClaimsPrincipal", paramName: nameof(principal));
        }
    }
}
