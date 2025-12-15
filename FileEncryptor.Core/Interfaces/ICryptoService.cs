using FileEncryptor.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileEncryptor.Core.Interfaces
{
    /// <summary>
    /// Defines a contract for performing cryptographic operations on files asynchronously.
    /// </summary>
    /// <remarks>Implementations of this interface provide methods to process files using cryptographic
    /// algorithms, such as encryption or decryption, in an asynchronous manner. The interface supports progress
    /// reporting and cancellation.</remarks>
    internal interface ICryptoService
    {
        Task ProcessFileAsync(CryptoOptions options, IProgress<double> progress, CancellationToken cancellationToken);
    }
}
