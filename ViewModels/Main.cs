using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;
using Sudoku.Models;
using Sudoku.Other;
using Sudoku.Views;

namespace Sudoku.ViewModels
{
    class Main : INotifyPropertyChanged
    {
        MainWindow main;    //MainWindow instance
        private CellsGrid cells = new CellsGrid();
        public CellsGrid Cells { get { if (cells.Count == 81) IsWinned(); return cells; } set { cells = value; } }     //Currently used CellsGrid in PlayPage
        public CellsGrid CellsMarked { get; set; }   //Used for marking hints
        public LanguageIndexer ContentLanguage { get; set; }    //For all buttons, labels, etc. Returns Content in selected language
        public LanguageIndexer TooltipLanguage { get; set; }    //For all buttons, labels, etc. Returns Tooltip in selected language
        public int DifficultySet { get; set; }  //User setted difficulty in PlaySettingsPage
        public int IsCorrectCount { get; set; } //User setted number of IsCorrect hints in PlaySettingsPage
        public int ShowNextCount { get; set; }  //User setted number of ShowNext hints in PlaySettingsPage
        public int SolveNextCount { get; set; } //User setted number of SolveNext hints in PlaySettingsPage
        public long Time { get; set; }  //Amount of time spent in PlayPage
        public string DifficultySolved { get; set; }    //Binded to difficulty label in steps
        public List<CellsGrid> StepsList { get; set; }  //StepsList in StepsPage.ListView 
        private CellsGrid indexStepsList;
        public CellsGrid IndexStepsList { get { return indexStepsList; } set { indexStepsList = value;  OnPropertyChanged(nameof(indexStepsList)); } }   //Index in StepsList    

        private long timeStart; //When user started in DateTime.UtcNow.Ticks
        private Solver solver;  //Solver instance
        private Generator generator;    //Generator instance
        private Dictionary<string, object> pageDictionary;  //Used for switching pages
        private Dictionary<object, string> pageDictionaryReverse;
        private MainMenuPage mainMenuPage;
        private TestPage testPage;
        private SolvePage solvePage;
        private StepsPage stepsPage;
        private PlayPage playPage;
        private PlaySettingsPage playSettingsPage;
        private PlayWinPage playWinPage;
        private PlayLosePage playLosePage;
        private string SAVEPLAYPATH = "..\\..\\..\\Data\\save.xml";
        private string SAVESOLVEPATH = "..\\..\\..\\Data\\saveSolve.xml";
        private string last;

        public int IsCorrectUsed { get; set; }  //User used number of IsCorrect hints in PlayPage showed in PlayWinPage or PlayLosePage
        public int ShowNextUsed { get; set; }   //User used number of ShowNext hints in PlayPage showed in PlayWinPage or PlayLosePage
        public int SolveNextUsed { get; set; }  //User used number of SolveNext hints in PlayPage showed in PlayWinPage or PlayLosePage

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand DebugSolveAll { get; set; }
        public ICommand SolveAllCommand { get; set; }   
        public ICommand ChangePageCommand { get; set; }
        public ICommand ExitCommand { get; set; }
        public ICommand TextBoxMoveCommand { get; set; }
        public ICommand LanguageChangeCommand { get; set; }
        public ICommand GenerateCommand { get; set; }
        public ICommand ClearCellsCommand { get; set; }
        public ICommand ContinueCommand { get; set; }
        public ICommand SaveSolveCommand { get; set; }
        public ICommand LoadSolveCommand { get; set; }
        public ICommand LeaveCommand { get; set; }
        public ICommand LeaveStepsPageCommand { get; set; }

        //Hint commands
        public ICommand IsCorrectCommand { get; set; }
        public ICommand ShowNextCommand { get; set; }
        public ICommand SolveNextCommand { get; set; }
        public ICommand ClearHintsCommand { get; set; }

        public Main()
        {
            IsCorrectCount = 0;
            ShowNextCount = 0;
            SolveNextCount = 0;

            pageDictionary = new Dictionary<string, object>();
            pageDictionaryReverse = new Dictionary<object, string>();
            cells = new CellsGrid();
            CellsMarked = new CellsGrid();
            
            solver = new Solver(cells);

            ContentLanguage = new LanguageIndexer("..\\..\\..\\Data\\contentLanguages.xml");
            TooltipLanguage = new LanguageIndexer("..\\..\\..\\Data\\tooltipLanguages.xml");

            mainMenuPage = new MainMenuPage();
            testPage = new TestPage();
            solvePage = new SolvePage();
            stepsPage = new StepsPage();
            playPage = new PlayPage();
            playSettingsPage = new PlaySettingsPage();
            playWinPage = new PlayWinPage();
            playLosePage = new PlayLosePage();

            GenerateCommand = new CommandHandler(() =>  {
                CellsMarked.Clear();
                generator = new Generator(); 
                generator.Generate(DifficultySet); 
                cells = generator.Generated; 
                timeStart = DateTime.UtcNow.Ticks;
                Time = 0;
                ChangePage("Play");
            }, () => true);

            SolveAllCommand = new CommandHandler(() => { 
                solver.Grid = cells.TrueClone();
                if (solver.SolveEasy()) DifficultySolved = ContentLanguage["EASYDIFFICULTY_LABEL"];
                else if (solver.SolveMedium()) DifficultySolved = ContentLanguage["MEDIUMDIFFICULTY_LABEL"];
                else if (solver.SolveHard()) DifficultySolved = ContentLanguage["HARDDIFFICULTY_LABEL"]; 
                else DifficultySolved = ContentLanguage["NOTSOLVEDDIFFICULTY_LABEL"];

                StepsList = solver.Steps;
                cells = solver.SolvedGrid; 
                ChangePage("Steps"); 
            }, () => true);

            DebugSolveAll = new CommandHandler(() =>
            {
                solver.Grid = cells.TrueClone();
                solver.SolveHard();
                cells = solver.SolvedGrid;
                OnPropertyChanged(nameof(Cells));
            }, () => true);

            #region Hint commands
            IsCorrectCommand = new CommandHandler(x => {
                CellsMarked.Clear();
                if (x?.ToString() != "unlimited") IsCorrectCount--;
                IsCorrectUsed++;
                solver.Grid = cells.TrueClone();
                foreach (var index in solver.IsCorrect(cells))
                {
                    CellsMarked[index[0], index[1]] = 1;
                }
                OnPropertyChanged(nameof(IsCorrectCount));
            }, () => IsCorrectCount > 0 || main.Content is SolvePage);

            ShowNextCommand = new CommandHandler(x => {
                CellsMarked.Clear();
                int[] index = solver.ShowNext(cells);
                if (index[0] == -1) return;
                CellsMarked[index[0], index[1]] = 1;
                if (x?.ToString() != "unlimited") ShowNextCount--;
                ShowNextUsed++;
                OnPropertyChanged(nameof(ShowNextCount));
            }, () => ShowNextCount > 0 || main.Content is SolvePage);

            SolveNextCommand = new CommandHandler(x => {
                CellsMarked.Clear();
                int[] index = solver.ShowNext(cells);
                if (index[0] == -1) return;
                CellsMarked[index[0], index[1]] = 1;
                cells[index[0], index[1]] = index[2];
                if (x?.ToString() != "unlimited") SolveNextCount--;
                SolveNextUsed++;
                OnPropertyChanged(nameof(SolveNextCount));
            }, () => SolveNextCount > 0 || main.Content is SolvePage);

            ClearHintsCommand = new CommandHandler(() => {
                CellsMarked.Clear();
            }, () => true);
            #endregion

            ClearCellsCommand = new CommandHandler(() => cells.Clear(), () => true);
            ChangePageCommand = new CommandHandler(x => ChangePage(x), () => true);
            ExitCommand = new CommandHandler(() => Closing(), () => true);

            LanguageChangeCommand = new CommandHandler(() => { 
                ContentLanguage.ChangeLanguage(); 
                TooltipLanguage.ChangeLanguage(); 
            }, () => true);

            ContinueCommand = new CommandHandler(() => { 
                LoadState(SAVEPLAYPATH); 
            }, () => true);

            SaveSolveCommand = new CommandHandler(() => { 
                SaveState(SAVESOLVEPATH, "Solve");  
            }, () => true);

            LoadSolveCommand = new CommandHandler(() => {
                LoadState(SAVESOLVEPATH);
                OnPropertyChanged(nameof(Cells));
            }, () => true);

            LeaveStepsPageCommand = new CommandHandler(() => {
                cells.Clear();
                if (last == "Play")
                {
                    IsCorrectCount = 0;
                    ShowNextCount = 0;
                    SolveNextCount = 0;
                    SaveState(SAVEPLAYPATH,"PlaySettings");
                }
                ChangePage("MainMenu");
            }, () => true);

            LeaveCommand = new CommandHandler(() => {
                SaveState(SAVEPLAYPATH);
                ChangePage("MainMenu");
            }, () => true);

            pageDictionary.Add("MainMenu", mainMenuPage);
            pageDictionary.Add("TestPage", testPage);
            pageDictionary.Add("Solve", solvePage);
            pageDictionary.Add("Steps", stepsPage);
            pageDictionary.Add("Play", playPage);
            pageDictionary.Add("PlaySettings", playSettingsPage);
            pageDictionary.Add("PlayWin", playWinPage);
            pageDictionary.Add("PlayLose", playLosePage);

            pageDictionaryReverse.Add(mainMenuPage, "MainMenu");
            pageDictionaryReverse.Add(testPage, "TestPage");
            pageDictionaryReverse.Add(solvePage, "Solve");
            pageDictionaryReverse.Add(stepsPage, "Steps");
            pageDictionaryReverse.Add(playPage, "Play");
            pageDictionaryReverse.Add(playSettingsPage, "PlaySettings");
            pageDictionaryReverse.Add(playWinPage, "PlayWin");
            pageDictionaryReverse.Add(playLosePage, "PlayLose");

            main = new MainWindow();
            main.DataContext = this;
            main.Content = pageDictionary["MainMenu"];
            main.Show();
        }

        /// <summary>
        /// Changes visible page of window.
        /// </summary>
        /// <param name="page">Name of the page to change to</param>
        public void ChangePage(object page)
        {
            last = pageDictionaryReverse[main.Content];
            if (page.ToString() == "Solve" && last != "Solve") cells.Clear();
            if (page.ToString() == "Steps") indexStepsList = StepsList.Last();
            main.Content = pageDictionary[page.ToString()];
        }

        /// <summary>
        /// Called when window is closing.
        /// </summary>
        public void Closing()
        {
            if (main.Content is PlayPage) SaveState(SAVEPLAYPATH);
            
            System.Windows.Application.Current.Shutdown();
        }

        /// <summary>
        /// Saves state to save.xml.
        /// </summary>
        /// <param name="nextPage">Next page to swiched to</param>
        public void SaveState(string path, string nextPage = "Play")
        {
            if (!(main.Content is StepsPage || main.Content is PlayPage || main.Content is PlayWinPage || main.Content is PlayLosePage || main.Content is SolvePage && path.Contains("Solve"))) return;
            CellsGrid tempGrid = cells.TrueClone();
            XElement element;
            using (Stream reader = new FileStream(path, FileMode.Open))
            {
                element = XElement.Load(reader);
            }

            string writeSudokuValue = "";
            for (int x = 0; x < 9; x++)
            {
                writeSudokuValue += "\n";
                for (int y = 0; y < 9; y++)
                {
                    writeSudokuValue += tempGrid[x, y] + " ";
                }
            }
            writeSudokuValue += "\n";

            element.Element("sudoku").Value = writeSudokuValue;

            if (Time != 0 || timeStart != 0)
                element.Element("time").Value = (Time + DateTime.UtcNow.Ticks - timeStart).ToString();

            element.Element("correct").Value = IsCorrectCount.ToString();
            element.Element("show").Value = ShowNextCount.ToString();
            element.Element("solve").Value = SolveNextCount.ToString();
            element.Element("next").Value = nextPage;

            using (Stream writer = new FileStream(path, FileMode.Create))
            {
                writer.Seek(0, SeekOrigin.Begin);
                element.Save(writer);
            }
           
        }

        /// <summary>
        /// Loads state from file
        /// </summary>
        public void LoadState(string path)
        {
            CellsGrid tempGrid = new CellsGrid();
            XElement element;
            using (Stream reader = new FileStream(path, FileMode.Open))
            {
                element = XElement.Load(reader);
            }

            var temp = element.Element("sudoku").Value.Split("\n").Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x.ToString())).ToArray().Select(x => x.Split(" ").Select(d => int.Parse(d))).ToArray();
            Time = long.Parse(element.Element("time").Value);
            IsCorrectCount = int.Parse(element.Element("correct").Value);
            ShowNextCount = int.Parse(element.Element("show").Value);
            SolveNextCount = int.Parse(element.Element("solve").Value);

            int x = 0;
            int y = 0;
            foreach (var item in temp)
            {
                if (x > 8) x = 0;
                foreach (var num in item)
                {
                    if (y > 8) y = 0;
                    tempGrid[x, y] = num;
                    y++;
                }
                x++;
            }
            cells = tempGrid.Clone();
            solver.Grid = cells.TrueClone();
            if(cells.Count != 0) solver.SolveHard();    //When is cells clear it takes long time
            timeStart = DateTime.UtcNow.Ticks;          
            ChangePage(element.Element("next").Value);
        }

        /// <summary>
        /// Checks if grid is solved and changes to PlayWinPage if Solved and PlayLosePage if was not solved.
        /// </summary>
        public void IsWinned() 
        { 
            if (!(main.Content is PlayPage)) return;
            Time += DateTime.UtcNow.Ticks - timeStart;
            if (solver.IsSolved()) ChangePage("PlayWin");
            else ChangePage("PlayLose"); //Change to LosePage when ready
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
