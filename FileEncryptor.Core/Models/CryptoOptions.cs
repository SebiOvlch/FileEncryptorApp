using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using FileEncryptor.Core.Enums;

namespace FileEncryptor.Core.Models
{
    /// <summary>
    /// Represents configuration options for a file encryption or decryption operation.
    /// Contains the source and destination file paths, the password/secret, a flag
    /// indicating whether to perform encryption or decryption, and choices for the
    /// algorithm, cipher mode, and padding mode to use.
    /// </summary>
    public class CryptoOptions
    {
        // Properties
        public string? InputFilePath { get; set; }
        public string? OutputFilePath { get; set; }
        public string? Password { get; set; }
        public SupportedAlgorithm Algorithm { get; set; }
        public CryptoAction Action { get; set; }
        public CipherMode Mode { get; set; }
        public PaddingMode Padding { get; set; }
    }
}
