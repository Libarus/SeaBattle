using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaBattle.Helpers
{
    enum CellState
    {
        Empty = 0, // пусто
        Deck = 1, // палуба
        Miss = 2, // промах
        Sunk = 3, // попал
        Kill = 4 // потопил
    }
}
