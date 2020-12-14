using SeaBattle.Helpers;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SeaBattle
{
    public partial class Form1 : Form
    {
        const int mapSize = 11; // Размер поля 10х10 плюс на буквы и цифры
        const int userShiftX = -20, userShiftY = 10; // сдвиг поля игрока
        const int enemyShiftX = 310, enemyShiftY = 10; // сдвиг поля противника 11*35=385
        const int cellSize = 30; // размер ячейки поля
        const string symbols = " АБВГДЕЖЗИК"; // буквы для обзначения столбцов

        bool startGame = false; // для возведения флага, что игра началась

        // для подсчёта количества сбитых кораблей
        int userScore = 0;
        int enemyScore = 0;
        int shootNext = 0; // кто стреляет первым

        Random rnd = new Random(); // для использования генерации псевдо-случайных чисел
        // rnd.Next(a, b) - даёт псевдослучайное число X в диапазоне a <= x < b, т.е. b - не включается.

        Cell[,] userMap = new Cell[mapSize, mapSize];
        Cell[,] enemyMap = new Cell[mapSize, mapSize];

        /// <summary>
        /// Начальная функция работы программы
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            Init();
        }

        // Инициализационная функция программы
        void Init()
        {
            CreateMaps();
        }

        private void btnSetShips_Click(object sender, EventArgs e)
        {
            SetSips(userMap);
        }

        // Генерация полей игрока и противника
        void CreateMaps()
        {
            // В цикле перебором по всему двумерному массиву
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    if (i == 0 && j == 0)
                    {
                        // ничего не делаем с ячейкой верхнего левого угла
                        // переходим к следующей итерации цикла
                        continue;
                    }

                    // для симметричности уберём для игрока столбец с цифрами
                    if (j > 0)
                    {
                        // создаём ячейку для игрока и назначем ей нужные параметры
                        userMap[i, j] = new Cell();
                        userMap[i, j].Visible = true;
                        userMap[i, j].btn = CreateBtn("user", i, j, userShiftX, userShiftY);
                    }

                    // создаём ячейку для противника и назначем ей нужные параметры
                    enemyMap[i, j] = new Cell();
                    enemyMap[i, j].Visible = true;
                    enemyMap[i, j].btn = CreateBtn("enemy", i, j, enemyShiftX, enemyShiftY);
                }
            }
        }

        // Оптимизировали создание ячейки, вынесли повторяющийся код
        Button CreateBtn(string name, int i, int j, int shiftX, int shiftY)
        {
            // создайм новую кнопку
            Button btn = new Button();
            // указываем ей позицию на форме
            btn.Location = new Point(j * cellSize + shiftX, i * cellSize + shiftY);
            // устанавливаем размер
            btn.Size = new Size(cellSize, cellSize);
            // и настройки шрифта, в частности делаем шрифт жирным
            btn.Font = new Font(btn.Font.FontFamily, btn.Font.Size, FontStyle.Bold);

            if (i == 0)
            {
                // верхняя строка - символы
                btn.Text = symbols[j].ToString();
                btn.Enabled = false;
            }
            else if (j == 0)
            {
                // левая строка цифры
                btn.Text = i.ToString();
                btn.Enabled = false;
            }
            else
            {
                // всем остальным, рабочим кнопкам, указываем имя и в имя встраиваем координаты в массиве
                btn.Name = String.Format("user_{0}_{1}", i, j);
            }

            Controls.Add(btn);
            return btn;
        }

        // очищаем поле
        void ClearMap(Cell[,] map)
        {
            for (int i = 1; i < mapSize; i++)
                for (int j = 1; j < mapSize; j++)
                    map[i, j].State = CellState.Empty;

        }

        void SetSips(Cell[,] map)
        {
            ClearMap(map);
            int[] decks = { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 }; // количество кораблей и их палубы

            bool again; // флаг для повторения
            for (int i = 0; i < decks.Length; i++)
            {
                // цикл по количеству кораблей
                do
                {
                    again = false; // сразу сбрасывем флаг, чтобы не уёти в вечный цикл
                    bool busy = false; // флаг занятости ячейки
                    int n = rnd.Next(2); // получаем 0 или 1 для выбора направления размещения корабля горизонт или вертикаль
                    int deckLength = decks[i]; // длина корабля
                    int x;
                    int y;

                    if (n == 0)
                    {
                        // горизонтально
                        x = rnd.Next(1, 12 - deckLength); // от 1 до (11 минус длина корабля, 11-4 = 7, т.е. до 6)
                                                          // сделано, чтобы при установке корабля не выйти за границу поля
                        y = rnd.Next(1, 11); // от 1 до 10
                    }
                    else
                    {
                        // вертикально
                        x = rnd.Next(1, 11); // от 1 до 10
                        y = rnd.Next(1, 12 - deckLength); // от 1 до (11 минус длина корабля, 11-4 = 7, т.е. до 6)
                                                          // сделано, чтобы при установке корабля не выйти за границу поля
                    }

                    switch (n)
                    {
                        case 0:
                            // горизонтально
                            // создаём цикл, который больше корабля на одну клетку, для определения что корабль не будет касаться другого корабля
                            for (int fy = -1; fy < 2; fy++)
                                for (int fx = -1; fx < deckLength + 1; fx++)
                                    if (x + fx > 0 && x + fx < mapSize && y + fy > 0 && y + fy < mapSize) // проверяем не выходим ли мы за границу поля
                                        if (!busy) // если у нас нашлось занятое поле, боле не проверяем
                                            busy = map[x + fx, y + fy].State != CellState.Empty; // проверяем на занятость
                            break;
                        case 1:
                            // вертикально
                            // создаём цикл, который больше корабля на одну клетку, для определения что корабль не будет касаться другого корабля
                            for (int fy = -1; fy < deckLength + 1; fy++)
                                for (int fx = -1; fx < 2; fx++)
                                    if (x + fx > 0 && x + fx < mapSize && y + fy > 0 && y + fy < mapSize) // проверяем не выходим ли мы за границу поля
                                        if (!busy) // если у нас нашлось занятое поле, боле не проверяем
                                            busy = map[x + fx, y + fy].State != CellState.Empty; // проверяем на занятость
                            break;
                    }

                    if (busy)
                    {
                        // если хотя бы одно поле выбранной зоны занято, ищем другую зону
                        again = true;
                    }
                    else
                    {
                        for (int j = 0; j < deckLength; j++)
                        {
                            switch (n)
                            {
                                case 0:
                                    // горизонтально
                                    map[x + j, y].State = CellState.Deck;
                                    break;
                                case 1:
                                    // вертикально
                                    map[x, y + j].State = CellState.Deck;
                                    break;
                            }
                        }
                    }

                } while (again);
            }

        }
    }
}
