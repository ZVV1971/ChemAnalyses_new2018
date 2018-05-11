using System;

namespace ChemicalAnalyses.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        void Save();
    }
}