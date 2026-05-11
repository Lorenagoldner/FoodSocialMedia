using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorAppFood.Data
{
    interface ILoginService
    {
        Task<bool> LogUser(string Email, string Password);
    }
}
