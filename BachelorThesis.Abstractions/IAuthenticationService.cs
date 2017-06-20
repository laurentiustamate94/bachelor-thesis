﻿namespace BachelorThesis.Abstractions
{
    public interface IAuthenticationService
    {
        string Token { get; }

        void InitializeService(string subscriptionKey);
    }
}
