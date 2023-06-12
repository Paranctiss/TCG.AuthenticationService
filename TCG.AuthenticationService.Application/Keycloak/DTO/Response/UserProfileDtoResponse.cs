using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCG.AuthenticationService.Domain;

namespace TCG.AuthenticationService.Application.Keycloak.DTO.Response
{
    public class UserProfileDtoResponse
    {
        public int Id { get; set; }
        public Guid Sub { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime LastConnection { get; set; }
        public string Username { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsOwner { get; set; }

        public int UserStateId { get; set; }
    }
}
