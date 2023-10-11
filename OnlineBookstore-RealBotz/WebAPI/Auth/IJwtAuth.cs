using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Auth
{
    public interface IJwtAuth
    {
        public string Authentication(string username, string password);
    }
}
