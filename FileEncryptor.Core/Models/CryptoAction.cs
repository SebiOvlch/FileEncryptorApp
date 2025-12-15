using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileEncryptor.Core.Models
{
    /// <summary>
    /// Specifies the cryptographic operation to perform, such as encryption or decryption.
    /// </summary>
    public enum CryptoAction
    {
        Encrypt,
        Decrypt
    }
}
