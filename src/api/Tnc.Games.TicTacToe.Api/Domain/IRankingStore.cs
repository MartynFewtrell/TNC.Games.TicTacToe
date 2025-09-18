using System.Collections.Generic;

namespace Tnc.Games.TicTacToe.Api.Domain
{
    public interface IRankingStore
    {
        double? Get(string stateKey, int moveIndex);
        void Set(string stateKey, int moveIndex, double value);
        void Reset();
        object Export();
        void ImportReplace(object doc);
    }
}
