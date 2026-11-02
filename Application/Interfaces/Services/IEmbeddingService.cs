using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services
{
    public interface IEmbeddingService
    {
        Task<byte[]> ExtractEmbeddingAsync(byte[] image);
    }
}
