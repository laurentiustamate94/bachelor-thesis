using System;

namespace BachelorThesis.Models
{
    [Serializable]
    public sealed class EmailDialogResult
    {
        public string EmailAddress { get; set; }

        public bool WasEmailAddressSubmitted { get; set; }
    }
}