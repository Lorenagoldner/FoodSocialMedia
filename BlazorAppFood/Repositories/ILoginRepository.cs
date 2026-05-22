using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorAppFood.Repositories
{
    interface ILoginRepository
    {
        Task<bool> LogUser(string Email, string Password);
    }
}
