using System;
using System.Collections.Generic;
using System.Text;

namespace BachelorThesis.Abstractions
{
    public interface IAuthenticationService
    {
        string Token { get; }

        void InitializeService(string subscriptionKey);
    }
}
