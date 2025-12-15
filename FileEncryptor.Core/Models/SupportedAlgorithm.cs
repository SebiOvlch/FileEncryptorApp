using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileEncryptor.Core.Models
{
    /// <summary>
    /// Specifies the set of symmetric encryption algorithms supported by the cryptographic API.    
    /// </summary>
    public enum SupportedAlgorithm
    {
        Aes,
        Des,
        TripleDes,
        Rijndael,
        Rc2
    }
}
