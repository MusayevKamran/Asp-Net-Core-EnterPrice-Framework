﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using App.Infrastructure.CrossCutting.Identity.Models;

namespace App.Infrastructure.CrossCutting.Identity.Interfaces
{
    public interface ILoginManager
    {
        Task<AuthenticationResult> RegisterAsync(string email, string password);
        Task<AuthenticationResult> LoginAsync(string email, string password);
        Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken);
    }
}
