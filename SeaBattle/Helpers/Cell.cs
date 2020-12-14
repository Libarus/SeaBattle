using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SeaBattle.Helpers
{
    /// <summary>
    /// Класс для описания ячейки поля
    /// </summary>
    class Cell
    {
        bool visible = false; // отображать ли данные на поле
        CellState state = CellState.Empty;

        public Button btn = null; // все ячейки будем основывать на кнопках :)
        public CellState State { // поле, отвечающее за текущее состояние ячейки

            get => state;
            set
            {
                state = value;
                Update();
            }
        }

        public bool Visible { get => visible; set { visible = value;  } }


        /// <summary>
        /// Определяем текущее состояние ячейки и меняем визуальное отображение
        /// </summary>
        private void Update()
        {
            // получаем текущее состояние, если надо отобразить ячейку, иначе передаём пустое значение
            CellState _state = visible ? state : CellState.Empty;

            if (btn != null) // проверка на существование кнокпи, т.е. она создана
            {
                switch (_state)
                {
                    case CellState.Empty:
                        btn.Text = "";
                        btn.BackColor = Color.Silver; // назначаем цвет фона кнопки
                        break;
                    case CellState.Deck:
                        btn.Text = "";
                        btn.BackColor = Color.Green; // назначаем цвет фона кнопки
                        break;
                    case CellState.Miss:
                        btn.Text = "\u00B7"; // unicode-символ точки по центру знакоместа
                        btn.BackColor = SystemColors.Control; // назначаем цвет фона кнопки
                        break;
                    case CellState.Sunk:
                        btn.Text = "X";
                        btn.BackColor = Color.Green; // назначаем цвет фона кнопки
                        btn.ForeColor = Color.White; // назначаем цвет текста кнопки
                        break;
                }
            }
        }
    }
}
