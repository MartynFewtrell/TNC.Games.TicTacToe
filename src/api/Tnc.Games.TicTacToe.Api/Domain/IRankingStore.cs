using System.Collections.Generic;

namespace Tnc.Games.TicTacToe.Api.Domain
{
    /// <summary>
    /// Ranking store for Q-values. Keys MUST be canonical state keys produced by
    /// <see cref="Tnc.Games.TicTacToe.Api.Domain.Symmetry"/> canonicalization.
    ///
    /// Notes:
    /// - Implementations should store and retrieve Q-values using canonical state keys and canonical move indices.
    /// - To support migration from pre-canonical data, a compat/probing layer may be implemented externally or
    ///   feature-flagged inside the store: on read miss for a canonical key, optionally probe symmetric variants
    ///   of the non-canonical key and re-store under the canonical key.
    /// - The interface intentionally does not change; migration and compatibility are implementation details.
    /// </summary>
    public interface IRankingStore
    {
        double? Get(string stateKey, int moveIndex);
        void Set(string stateKey, int moveIndex, double value);
        void Reset();
        object Export();
        void ImportReplace(object doc);
    }
}
