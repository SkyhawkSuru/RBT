using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBT
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "RBT By SkyhawkSuru ~ Github";
            // ------------------Variablen ------------------------           
            int MaxLenghtOfObject = 4;
            int gridSaveZone = 3; // no prove if there are still objects
            int gridRow = 10;
            int gridCol = 10;
            int getPoints = 0;

            string[] formType = new string[3];
            formType[0] = "X";
            formType[1] = "0";
            formType[2] = "G";
            int numberFormTypes = 1;

            int[] controlFormProperties = new int[4]; // row + colum + direction (0 horizontal, 1 vertical) + colum lenght + form
            bool lose = false;

            // ------------------Menu-----------------------
            menu(formType, ref gridRow, ref gridCol, ref numberFormTypes, ref MaxLenghtOfObject);
            // changed stats

            gridSaveZone += MaxLenghtOfObject;
            gridRow += gridSaveZone;
            string[,] grid = new string[gridRow, gridCol];
            int rowLength = grid.GetLength(0);
            int colLength = grid.GetLength(1);

            // ------------------Array fill ---------------------
            fillArraywithSpace(grid, rowLength, colLength);

            Random rnd = new Random();
            string inputAgain;

            controlFormProperties[0] = 0; // only first round
            int count = 0;

            do
            {
                int typFormPlace;
                int horizontalVertical;
                int colum;
                int xTime;
                int roundPerObject = 0;
                bool notInGrid = false;
                controlFormProperties[0] = 0;
                
                Console.Clear();

                // Random lenght of form, direction
                randomPlayNumbers(MaxLenghtOfObject, gridCol, rnd, out typFormPlace, out horizontalVertical, out colum, out xTime, out notInGrid, numberFormTypes);

                // fill start properties 
                horizontalVertical = fillPropertiesObject(controlFormProperties, horizontalVertical, colum, xTime);

                //------------------------------------insert form to grid ------------------
                changeObjectInGrid(formType, grid, controlFormProperties, typFormPlace, xTime);

                // 3 rounds per object(form) or release (drop), than new object 
                do
                {
                    count += 1;
                    roundPerObject += 1;

                    //  ------------------->     Output Grid  <------------------------
                    outputGrid(grid, rowLength, colLength, getPoints, roundPerObject, count, gridSaveZone);

                    bool wrongKey = false;
                    do
                    {
                        wrongKey = false;
                        Console.Write($"\tWait for input: ");

                        ConsoleKey key = Console.ReadKey(true).Key;

                        Console.Write($"\n\tYou hit the {key} key:\t");

                        // delete object in the original grid   
                        deleteObjectInGrid(grid, controlFormProperties, xTime);

                        // control object
                        controlObjectByHuman(controlFormProperties, colLength, ref roundPerObject, ref wrongKey, key);

                    } while (wrongKey);
                    // object move one down next round (row one up)
                    controlFormProperties[0] += 1;

                    // after control choice, change position in original grid     
                    changeObjectInGrid(formType, grid, controlFormProperties, typFormPlace, xTime);

                    // -------- next round
                    Console.Write($"\n\tcontinue: Enter | Break up and got to Menu: M ");
                    inputAgain = Console.ReadLine();

                    if (inputAgain == "m" || inputAgain == "M")
                    {
                        getPoints = 0;
                        count = 0;
                        roundPerObject = 0;
                        fillArraywithSpace(grid, rowLength, colLength);
                        menu(formType, ref gridRow, ref gridCol, ref numberFormTypes, ref MaxLenghtOfObject);
                    }

                    // delete object in grid and drop it -> prove if there are objects and set it over
                    if (roundPerObject == 3)
                    {
                        int proveCount = 0;
                        bool insertArray = false;

                        deleteObjectInGrid(grid, controlFormProperties, xTime);

                        proveAndInsertObject(formType[0], formType[1], formType[2], grid, rowLength, controlFormProperties[1], controlFormProperties[3], ref proveCount, ref insertArray, controlFormProperties[0], controlFormProperties[2], gridSaveZone, ref lose, formType, typFormPlace);
                    }

                } while (roundPerObject != 3 && lose != true);

                int fourGetPoints = 4;
                //int fiveGetPoints = 5;
                //int sixGetPoints = 6;
                //int sevenGetPoints = 7;
                //int eightGetPoints = 8;

                string[,] rowCopy = copyOriginalArray(grid); // copy 2D Array - to compare 
                #region Points and reduce equal elements  
                // ---------horizontal and vertical 
                //getPoints = dynamicProveAndGetPointsAndChangeArray(gridRow, form_X, zeile, rowLength, colLength, eightGetPoints, zeileCopy, getPoints); // 8
                //getPoints = dynamicProveAndGetPointsAndChangeArray(gridRow, form_X, zeile, rowLength, colLength, sevenGetPoints, zeileCopy, getPoints); // 7                
                //getPoints = dynamicProveAndGetPointsAndChangeArray(gridRow, form_X, zeile, rowLength, colLength, sixGetPoints, zeileCopy, getPoints); // 6   
                //getPoints = dynamicProveAndGetPointsAndChangeArray(gridRow, form_X, zeile, rowLength, colLength, fiveGetPoints, zeileCopy, getPoints); // 5
                getPoints = dynamicProveAndGetPointsAndChangeArray(gridRow, formType, grid, rowLength, colLength, fourGetPoints, rowCopy, getPoints); // 4
                #endregion               

                if (lose)
                {
                    Console.Clear();
                    Console.WriteLine($"\n\t\t\t\t\t-------------You lost!-------------");

                    Console.ReadKey();
                    getPoints = 0;
                    count = 0;
                    lose = false;
                    fillArraywithSpace(grid, rowLength, colLength);
                    menu(formType, ref gridRow, ref gridCol, ref numberFormTypes, ref MaxLenghtOfObject);
                }
            } while (inputAgain != "n");

            Console.ReadKey();
        }

        private static int menu(string[] formType, ref int gridRow, ref int gridCol, ref int numberFormTypes, ref int MaxLenghtOfObject)
        {
            bool back = false;
            do
            {
                Console.Clear();
                back = false;
                Console.WriteLine($"\n\n\n\n\n");
                Console.WriteLine($"\t\t\t\t\t-------- Menu --------");
                Console.WriteLine($"\t\t\t\t\t Play \t\t\t(P)");
                Console.WriteLine($"\t\t\t\t\t Game Option \t\t(G)");
                Console.WriteLine($"\t\t\t\t\t Info - How to Play \t(I)");
                Console.WriteLine($"\t\t\t\t\t Exit \t\t\t(E)");
                Console.WriteLine($"\n\t\t\t\t\t Back with B");

                int input;
                bool backAfterInsertValue = false;
                ConsoleKey key = Console.ReadKey(true).Key;
                do
                {
                    Console.Clear();
                    if (backAfterInsertValue) // activ, value(Column) input and to load menu 
                    {
                        key = ConsoleKey.G;
                    }
                    backAfterInsertValue = false;

                    switch (key)
                    {
                        case ConsoleKey.P:
                            Console.WriteLine($"\n\n\n\n\n");
                            Console.WriteLine($"\t\t\t\t\t-------- Play --------");
                            Console.WriteLine();
                            Console.WriteLine($"\t\t\tEasy \t(E)\tMedium \t(M)\tDifficult \t(D)\t");
                            Console.WriteLine();
                            Console.WriteLine($"\t\tObject:\t  {formType[0]}\t\t {formType[0]}{formType[1]}\t\t {formType[0]}{formType[1]}{formType[2]}");
                            Console.WriteLine();
                            Console.WriteLine($"\t\tRow:\t {gridRow} \tColum: {gridCol}");
                            Console.WriteLine($"\n\t\t\t\t\t Back with B");

                            key = Console.ReadKey(true).Key;

                            switch (key)
                            {
                                case ConsoleKey.E:
                                    // start game
                                    numberFormTypes = 1;
                                    break;
                                case ConsoleKey.M:
                                    // start game
                                    numberFormTypes = 2;
                                    break;
                                case ConsoleKey.D:
                                    // start game
                                    numberFormTypes = 3;
                                    break;
                                case ConsoleKey.B:
                                    // Go back
                                    back = true;
                                    break;
                                default:
                                    back = true;
                                    Console.WriteLine($"\n\t\t\t\t\twrong key - enter any key to continue");
                                    Console.ReadKey();
                                    break;
                            }
                            break;

                        case ConsoleKey.G:
                            Console.Clear();
                            Console.WriteLine($"\n\n\n\n\n");
                            Console.WriteLine($"\t\t\t\t\t------------- Game -----------");
                            Console.WriteLine($"\t\t\t\t\tGame Field:");
                            Console.WriteLine($"\t\t\t\t\tColum Lenght\t\t{gridCol}(C)");
                            Console.WriteLine($"\t\t\t\t\tRow Lenght\t\t{gridRow}(R)");
                            Console.WriteLine($"\t\t\t\t\tMax-Object Lenght\t{MaxLenghtOfObject} (O)");
                            Console.WriteLine($"\t\t\t\t\tDefault Settings\t  (D)");
                            Console.WriteLine($"\n\t\t\t\t\t Back with B");

                            key = Console.ReadKey(true).Key;

                            switch (key)
                            {
                                case ConsoleKey.C:
                                    Console.Write($"\n\t\t\t\t\tNew Colum Lenght: ");
                                    while (!int.TryParse(Console.ReadLine(), out input) || input < 1)
                                    {
                                        Console.Write($"\t\t\t\t\tInvalid input. Enter Colum Lenght: ");
                                    }
                                    backAfterInsertValue = true;
                                    gridCol = input;
                                    break;
                                case ConsoleKey.R:
                                    Console.Write($"\n\t\t\t\t\tNew Row Lenght: ");
                                    while (!int.TryParse(Console.ReadLine(), out input) || input < 1)
                                    {
                                        Console.Write($"\t\t\t\t\tInvalid input. Enter Row Lenght: ");
                                    }
                                    backAfterInsertValue = true;
                                    gridRow = input;
                                    break;
                                case ConsoleKey.O:
                                    bool richtigeEingabe = false;
                                    do
                                    {
                                        Console.Write($"\n\t\t\t\t\tMax-Object Lenght: ");

                                        if (!int.TryParse(Console.ReadLine(), out input))
                                        {
                                            Console.WriteLine($"\n\t\t\t\t\tInput have to be a Number ");
                                        }
                                        else if (input >= gridRow)
                                        {
                                            Console.WriteLine($"\n\t\t\t\t\tRow number have to be higher than Object Lenght");
                                        }
                                        else if (input < 1)
                                        {
                                            Console.WriteLine($"\n\t\t\t\t\tRow number have to be higher than One");
                                        }
                                        else
                                        {
                                            richtigeEingabe = true;
                                        }
                                    } while (!richtigeEingabe);

                                    MaxLenghtOfObject = input;
                                    backAfterInsertValue = true;
                                    break;
                                case ConsoleKey.D:
                                    // default Value 
                                    MaxLenghtOfObject = 4;
                                    gridRow = 10;
                                    gridCol = 10;
                                    backAfterInsertValue = true;
                                    break;
                                case ConsoleKey.B:
                                    // Go back
                                    back = true;
                                    break;
                                default:
                                    backAfterInsertValue = true;
                                    Console.WriteLine($"\n\t\t\t\t\twrong key - enter any key to continue");
                                    Console.ReadKey();
                                    break;
                            }
                            break;
                        case ConsoleKey.I:
                            back = true;
                            Console.WriteLine($"\n\n\n");
                            Console.WriteLine($"\t\t\t\t\t-------- How to play --------");
                            Console.WriteLine($"\t\t\t\t\t1) It's a Round based Game.");
                            Console.WriteLine($"\t\t\t\t\t2) For every Object, You have 3 rounds to position the Object.");
                            Console.WriteLine($"\t\t\t\t\t3) Controls: Q left turn | E right turn | A left | \n\t\t\t\t\t\t\t D right | S - Drop Form (No more round).");
                            Console.WriteLine($"\t\t\t\t\t4) After 3 rounds the Object drops to the Game field.");
                            Console.WriteLine($"\t\t\t\t\t5) Default setting: 4 in row of the same formtype - Object, get 4 points.");
                            Console.WriteLine($"\t\t\t\t\t6) Default setting: If there are more than 4 in row, No points.");
                            Console.WriteLine($"\t\t\t\t\t7) The upper field (0 - 2) - Position Field");
                            Console.WriteLine($"\t\t\t\t\t8) The lower field (0 - 6) - Game Field");
                            Console.WriteLine($"\t\t\t\t\t9) If you hit the Line between the two Fields, you lost the Game.");
                            Console.WriteLine();

                            #region Grid for Explanation
                            string[,] explaingrid = new string[10, 10];
                            for (int i = 0; i < 10; i++)
                            {
                                for (int j = 0; j < 10; j++)
                                {
                                    explaingrid[i, j] = " ";
                                }
                            }
                            Console.Write($"\t\t\t\t\t\t ");
                            for (int i = 0; i < 10; i++)
                            {
                                Console.Write("_");
                            }
                            Console.WriteLine();
                            for (int i = 0; i < 3; i++)
                            {
                                Console.Write($"\t\t\t\t\t\t|");
                                for (int j = 0; j < 10; j++)
                                {

                                    Console.Write(string.Format("{0}", explaingrid[i, j]));
                                }
                                Console.Write($"|{i}");
                                Console.Write(Environment.NewLine);
                            }
                            Console.Write($"\t\t\t\t\t\t ");
                            for (int i = 0; i < 10; i++)
                            {
                                Console.Write("-");
                            }
                            Console.WriteLine();
                            for (int i = 3; i < 10; i++)
                            {
                                Console.Write($"\t\t\t\t\t\t|");
                                for (int j = 0; j < 10; j++)
                                {

                                    Console.Write(string.Format("{0}", explaingrid[i, j]));
                                }
                                Console.Write($"|{i - 3}");
                                Console.Write(Environment.NewLine);
                            }
                            Console.Write($"\t\t\t\t\t\t ");
                            for (int i = 0; i < 10; i++)
                            {
                                Console.Write("-");
                            }
                            Console.WriteLine();
                            #endregion

                            Console.WriteLine($"\n\t\t\t\t\t Back with B");
                            Console.ReadKey();
                            break;
                        case ConsoleKey.E:
                            Console.WriteLine($"\n\n\n\n\n");
                            Console.Write($"\t\t\t\t\t Close and got to Desktop (Y | N) ");

                            string close = Console.ReadLine();

                            if (close == "y" || close == "Y")
                            {
                                Environment.Exit(-1);  // exit                                
                            }
                            back = true;
                            break;
                        default:
                            back = true;
                            Console.WriteLine($"\n\t\t\t\t\twrong key - enter any key to continue");
                            Console.ReadKey();
                            break;
                    }
                } while (backAfterInsertValue);
            } while (back);

            return numberFormTypes;
            return MaxLenghtOfObject;
            return gridRow;
            return gridCol;
        }

        private static void controlObjectByHuman(int[] controlFormProperties, int colLength, ref int roundPerObject, ref bool wrongKey, ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.Q:
                    Console.WriteLine("turn left");
                    if (controlFormProperties[2] == 0 && controlFormProperties[1] == 0)
                    {
                        controlFormProperties[2] = 1;
                    }
                    else if (controlFormProperties[1] == 0 || (controlFormProperties[1] + 1 - controlFormProperties[3]) < 0)
                    {
                        Console.WriteLine($"\tOne more turn left is not possible");
                        wrongKey = true;
                        break;
                    }
                    else
                    {
                        if (controlFormProperties[2] == 1) // vertical to horizontal
                        {
                            int hlp = controlFormProperties[1];
                            controlFormProperties[1] = hlp - controlFormProperties[3] + 1;
                            controlFormProperties[2] = 0;
                        }
                        else // horizontal to vertical
                        {
                            controlFormProperties[2] = 1;
                        }
                    }
                    break;
                case ConsoleKey.E:
                    Console.WriteLine("turn right");
                    if (controlFormProperties[1] == colLength - 1 || (controlFormProperties[1] + controlFormProperties[3] - 1) > colLength - 1)
                    {
                        Console.WriteLine($"\tOne more turn right is not possible");
                        wrongKey = true;
                        break;
                    }
                    else
                    {
                        if (controlFormProperties[2] == 1) // vertical to horizontal
                        {
                            controlFormProperties[2] = 0;
                        }
                        else // horizontal to vertical
                        {
                            int hlp = controlFormProperties[1];
                            controlFormProperties[1] = hlp + controlFormProperties[3] - 1;
                            controlFormProperties[2] = 1;
                        }
                        break;
                    }
                case ConsoleKey.A:
                    Console.WriteLine("left");
                    if (controlFormProperties[1] > 0)
                    {
                        controlFormProperties[1] -= 1; // move one to left 
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"\tOne more left is not possible");
                        wrongKey = true;
                        break;
                    }
                case ConsoleKey.D:
                    Console.WriteLine("right");
                    if (controlFormProperties[2] == 0)
                    {
                        if (colLength - controlFormProperties[3] > controlFormProperties[1])
                        {
                            controlFormProperties[1] += 1; // move one to right 
                            break;
                        }
                        else
                        {
                            Console.WriteLine($"\tOne more right is not possible");
                            wrongKey = true;
                            break;
                        }
                    }
                    else
                    {
                        if (controlFormProperties[1] < colLength - 1)
                        {
                            controlFormProperties[1] += 1; // move one to right 
                            break;
                        }
                        else
                        {
                            Console.WriteLine($"\tOne more right is not possible");
                            wrongKey = true;
                            break;
                        }
                    }
                case ConsoleKey.S:
                    Console.WriteLine("Down");
                    roundPerObject = 3; // release/drop form if its on the right position
                    break;
                default:
                    Console.WriteLine("wrong key");
                    wrongKey = true;
                    break;
            }
        }

        private static void randomPlayNumbers(int MaxLenghtOfObject, int gridCol, Random rnd, out int typFormPlace, out int horizontalVertical, out int colum, out int xTime, out bool notInGrid, int numberFormTypes)
        {
            typFormPlace = rnd.Next(numberFormTypes);
            xTime = rnd.Next(1, MaxLenghtOfObject);
            horizontalVertical = rnd.Next(2);
            // as long as stays in grid

            do
            {
                notInGrid = false;
                // start colum
                colum = rnd.Next(gridCol);
                if (horizontalVertical == 0 && (colum + xTime - 1 > gridCol - 1)) // if horizontal not in grid, try again
                {
                    notInGrid = true;
                    Console.WriteLine("again");
                }
            } while (notInGrid);
        }

        private static void deleteObjectInGrid(string[,] grid, int[] controlFormProperties, int xTime)
        {
            if (controlFormProperties[2] == 0)
            {
                for (int i = 0; i < xTime; i++)
                {
                    grid[controlFormProperties[0], controlFormProperties[1] + i] = " ";
                }
            }
            else if (controlFormProperties[2] == 1)
            {
                for (int i = 0; i < xTime; i++)
                {
                    grid[controlFormProperties[0] - i, controlFormProperties[1]] = " ";
                }
            }
        }

        private static void changeObjectInGrid(string[] formType, string[,] grid, int[] controlFormProperties, int typFormPlace, int xTime)
        {
            if (controlFormProperties[2] == 0)
            {
                for (int i = 0; i < xTime; i++)
                {
                    grid[controlFormProperties[0], controlFormProperties[1] + i] = formType[typFormPlace];   // stillbugs
                }
            }
            else if (controlFormProperties[2] == 1)
            {
                for (int i = 0; i < xTime; i++)
                {
                    grid[controlFormProperties[0] - i, controlFormProperties[1]] = formType[typFormPlace];
                }
            }
        }
        private static int fillPropertiesObject(int[] controlFormProperties, int horizontalVertical, int colum, int xTime)
        {
            controlFormProperties[1] = colum;
            controlFormProperties[3] = xTime; // colum lenght
                                              // one X use start condionts of horizontal
            if (horizontalVertical == 1 && xTime == 1)
            {
                horizontalVertical = 0;
            }

            if (horizontalVertical == 0)
            {
                controlFormProperties[2] = horizontalVertical; // 0 = horizontal | 1 = vertical
                controlFormProperties[0] = xTime - 1; // row
            }
            else
            {
                controlFormProperties[2] = horizontalVertical; // 0 = horizontal | 1 = vertical
                controlFormProperties[0] += xTime - 1; // row
            }

            return horizontalVertical;
        }
        private static void fillArraywithSpace(string[,] grid, int rowLength, int colLength)
        {
            for (int i = 0; i < rowLength; i++)
            {
                for (int j = 0; j < colLength; j++)
                {
                    grid[i, j] = " ";
                }
            }
        }
        private static void outputGrid(string[,] row, int rowLength, int colLength, int getPoints, int roundPerObject, int count, int gridSaveZone)
        {
            // calculation grid row numbers            

            int divi = colLength / 10;
            int modulo = colLength % 10;
            int countColor = 0;

            Console.WriteLine($"\t\t\t----------------------- Round based Tetris ---------------------");
            Console.WriteLine($"\t\t\t3 Rounds per Object, than automatic Release of Object");
            Console.WriteLine($"\t\t\tQ left turn | E right turn | A left | D right | S - Drop Form");

            // ------------------------Grid -----------------------
            // gridSaveZone
            Console.Write($"\t\t\t\t\t\t ");
            for (int i = 0; i < colLength; i++)
            {
                Console.Write("_");
            }
            Console.WriteLine();
            for (int i = 0; i < gridSaveZone; i++) // i < rowLength
            {
                Console.Write($"\t\t\t\t\t\t|");
                for (int j = 0; j < colLength; j++)
                {

                    Console.Write(string.Format("{0}", row[i, j]));
                }
                Console.Write($"|{i}");
                Console.Write(Environment.NewLine);
            }
            // counting/prove grid
            Console.Write($"\t\t\t\t\t\t ");
            for (int i = 0; i < colLength; i++)
            {
                Console.Write("-");
            }
            Console.WriteLine();
            for (int i = gridSaveZone; i < rowLength; i++)
            {
                Console.Write($"\t\t\t\t\t\t|");
                for (int j = 0; j < colLength; j++)
                {

                    Console.Write(string.Format("{0}", row[i, j]));
                }
                Console.Write($"|{i - gridSaveZone}");
                Console.Write(Environment.NewLine);
            }
            Console.Write($"\t\t\t\t\t\t ");
            for (int i = 0; i < colLength; i++)
            {
                Console.Write("-");
            }
            Console.WriteLine();
            Console.Write($"\t\t\t\t\t\t ");
            for (int a = 0; a < divi; a++)
            {
                int color = countColor % 2;
                if (color == 1)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else
                    Console.ForegroundColor = ConsoleColor.Yellow;
                for (int i = 0; i < 10; i++)
                {
                    Console.Write($"{i}");
                }
                countColor++;
            }
            Console.ForegroundColor = ConsoleColor.White;
            for (int i = 0; i < modulo; i++)
            {
                Console.Write($"{i}");
            }
            Console.WriteLine();

            // --- Counting Rounds/Points
            Console.Write($"\n\tRound {count}: \t");
            Console.Write($"object round: {roundPerObject}    \t");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"\t\t\tPoints: {getPoints}\n\n");
            Console.ForegroundColor = ConsoleColor.White;
        }
        private static int dynamicProveAndGetPointsAndChangeArray(int gridRow, string[] form, string[,] row, int rowLength, int colLength, int pointsInRow, string[,] rowCopy, int getPoints)
        {
            int pairs = 0;
            int hlpHorizontal = 0;
            // ------- horizontal -------- prove if there are x character in row, get points if condition given and remove row and drop all above
            for (int numberForm = 0; numberForm < 3; numberForm++) // hardcoded all 3 form get proved successively (one after one)
            {
                for (int i = rowLength - 1; i >= 0; i--)
                {
                    for (int a = 0; a < colLength - 1; a++)
                    {
                        hlpHorizontal = a;
                        if (row[i, a] == form[numberForm] && row[i, a + 1] == form[numberForm]) // form
                        {
                            pairs += 1;
                        }
                        else if (pairs == pointsInRow - 1)
                        {
                            getPoints += pointsInRow;
                            for (int b = i; b > 0; b--)
                            {
                                for (int c = a - pairs; c < a + 1; c++)
                                {
                                    row[b, c] = rowCopy[b - 1, c];
                                }
                            }
                            rowCopy = copyOriginalArray(row);
                            pairs = 0;
                        }
                        else
                        {
                            pairs = 0;
                        }
                    }
                    // ------- horizontal bottom row --------
                    if (pairs == pointsInRow - 1)
                    {
                        getPoints += pointsInRow;
                        for (int b = i; b > 0; b--)
                        {
                            for (int c = hlpHorizontal - 2; c < hlpHorizontal + 2; c++)
                            {
                                row[b, c] = rowCopy[b - 1, c];
                            }
                        }
                        rowCopy = copyOriginalArray(row);
                    }
                    pairs = 0;
                }

                // ------- vertical --------
                int hlpVertical = 0;
                for (int i = 0; i < colLength; i++)
                {
                    for (int a = rowLength - 1; a > 0; a--)
                    {
                        hlpVertical = a;
                        if (row[a, i] == form[numberForm] && row[a - 1, i] == form[numberForm]) // form
                        {
                            pairs += 1;
                        }
                        else if (pairs == pointsInRow - 1)
                        {
                            getPoints += pointsInRow;

                            for (int c = a + pairs; c > rowLength - pointsInRow - 1; c--) // not out of grid
                            {
                                row[c, i] = rowCopy[c - pointsInRow, i];
                            }

                            rowCopy = copyOriginalArray(row);
                            pairs = 0;
                        }
                    }
                    pairs = 0;
                }
                // prove floating characters
                int one = 0;
                bool pair = false;
                int tempcount = 0;
                int tempcountVertikal = 0;

                for (int i = rowLength - 2; i >= 0; i--)
                {
                    one = 0;
                    for (int a = 0; a < colLength; a++)
                    {
                        pair = false;
                        if (row[i, a] == form[numberForm]) // form
                        {
                            one += 1;
                        }
                        if (a < colLength - 1) // Exception out of grid
                        {
                            if (row[i, a] == form[0] && row[i, a + 1] == form[0]) // form
                            {
                                pair = true;
                            }

                        }
                        // for one character
                        if (one == 1 && pair == false && row[i + 1, a] == " ")
                        {
                            for (int b = i + 1; b < rowLength; b++)
                            {
                                if (row[b, a] == " ")
                                {
                                    tempcountVertikal += 1;
                                }
                            }
                            row[i + tempcountVertikal, a] = row[i, a];
                            row[i, a] = " ";
                            one = 0;
                            tempcountVertikal = 0;
                        }
                        // -- prove first line under the object (two ore more character), if there are nothing, than prove next line and so on
                        else if (one > 1 && pair == false && row[i + 1, a] == " ")
                        {
                            for (int b = a - one + 1; b < a + 1; b++)
                            {
                                if (row[i + 1, b] == " ")
                                {
                                    tempcount += 1;
                                    if (tempcount == one)
                                    {
                                        for (int c = a - one + 1; c < a + 1; c++)
                                        {
                                            row[i + 1, c] = row[i, c];
                                            row[i, c] = " ";
                                        }
                                        one = 0;
                                        tempcount = 0;
                                    }
                                }
                            }
                            tempcount = 0;
                            one = 0;
                        }
                    }
                }
            }
            return getPoints; // before or after pass
        }
        private static string[,] copyOriginalArray(string[,] row)
        {
            return row.Clone() as string[,];
        }
        private static void proveAndInsertObject(string form_0, string form_X, string form_G, string[,] row, int rowLength, int colum, int xTime, ref int count, ref bool insertArray, int line, int horiVerti, int gridSaveZone, ref bool lose, string[] formType, int typFormPlace)
        {
            // prove horizontal
            if (horiVerti == 0 && (xTime == 1 || xTime == 2 || xTime == 3))
            {
                for (int i = line + 1; i < rowLength; i++)
                {
                    for (int a = colum; a < xTime + colum; a++)
                    {
                        if (row[i, a] == form_X || row[i, a] == form_0 || row[i, a] == form_G)
                        {
                            if (i < gridSaveZone + 1)
                            {
                                lose = true;
                                break;
                            }
                            else
                            {
                                for (int b = 0; b < xTime; b++)
                                {
                                    row[i - 1, colum + b] = formType[typFormPlace];
                                }
                                insertArray = true;
                                break;
                            }
                        }
                    }
                    if (insertArray || lose == true)
                    {
                        break;
                    }
                    count++;
                }
            }
            // prove vertical
            else if (horiVerti == 1 && (xTime == 2 || xTime == 3))
            {
                for (int i = line + 1; i < rowLength; i++)
                {
                    if (row[i, colum] == form_X || row[i, colum] == form_0 || row[i, colum] == form_G)
                    {
                        if (i < gridSaveZone + 1 || (i - 1 - xTime) < gridSaveZone + 1)
                        {
                            lose = true;
                            break;
                        }
                        else
                        {
                            for (int a = 0; a < xTime; a++)
                            {
                                row[i - 1 - a, colum] = formType[typFormPlace];
                            }
                            break;
                        }
                    }
                    count++;
                }
            }
            // bootem Line / row
            int hlp = rowLength - 1;
            if (horiVerti == 0 && (count + line + 1 == rowLength))
            {
                for (int b = 0; b < xTime; b++)
                {
                    row[hlp, colum + b] = formType[typFormPlace];
                }
            }
            else if (horiVerti == 1 && (count + line + 1 == rowLength))
            {
                for (int b = 0; b < xTime; b++)
                {
                    row[hlp - b, colum] = formType[typFormPlace];
                }
            }
        }

    }
}



