using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Win32;
using NorthOBD.NorthOsuFileCreator;
using Button = System.Windows.Controls.Button;
using Cursors = System.Windows.Input.Cursors;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using Label = System.Windows.Controls.Label;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
//using System.IO;

namespace NorthOBD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public string PathOsu = "";
        public string PathNorth = "";
        ReaderOSU.ReaderOsuDB OsuDB;
        ReaderCollection.ReaderCollectionDB CollectionDB;
        NorthOsuFileCreator.NorthCollection[] Collections;
        NorthOsuFileCreator.NorthOsuFileCreator ExortFile;
        private MediaPlayer _mpCurSound;
        Thread ThreadMusic;
        int chek;
        double time = 0;

        //====V3======
        private bool mediaPlayerIsPlaying = false;
        private bool userIsDraggingSlider = false;
        private bool stopButtonIsPress = false;
        //============


        //=====V4======
        private bool chooseBM = false;
        private int chosenBM;
        private bool flagFind = true;
        private int justFlag = 0;
        private int[] massivBmSetAndIndex;
        private int fdg;
        private bool flagLinePlay = false;
        private int lineStartPlay;
        private int numberOfSong;
        //======V4

        //====ОТЛАДЧИК======



        //



        public MainWindow()
        {
            InitializeComponent();
            //BMTextBlock.Text = "";

            BoxCollection.Children.Clear();
            BoxMap.Children.Clear();
            ExportList.Children.Clear();

            //?????????????/
            _mpCurSound = new MediaPlayer();
            //?????????????/

            chek = 0;

            ThreadMusic = new Thread(PlayMusicThread) { IsBackground = true };

            //================V3=======
            {
                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += timer_Tick;
                pbVolume.Value = 0.5;
                _mpCurSound.Volume = 0.5;
                timer.Start();
            }
            //================V3=======

            //======V4====

            //=======V4=========


        }

        //========V3==========

        private void timer_Tick(object sender, EventArgs e)
        {
            if ((_mpCurSound.Source != null) && (_mpCurSound.NaturalDuration.HasTimeSpan) && (!userIsDraggingSlider))
            {
                sliProgress.Minimum = 0;
                sliProgress.Maximum = _mpCurSound.NaturalDuration.TimeSpan.TotalSeconds;
                sliProgress.Value = _mpCurSound.Position.TotalSeconds;
            }
        }

        private void Open_Executed(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Media files (*.mp3;*.mpg;*.mpeg)|*.mp3;*.mpg;*.mpeg|All files (*.*)|*.*";

            openFileDialog.ShowDialog();


            if (openFileDialog.FileName != "")
                //??
                PlayMusic(openFileDialog.FileName);
        }

        private void Play_Executed(object sender, RoutedEventArgs e)
        {
            _mpCurSound.Play();
            mediaPlayerIsPlaying = true;
        }

        private void Pause_Executed(object sender, RoutedEventArgs e)
        {
            _mpCurSound.Pause();
        }

        private void Stop_Executed(object sender, RoutedEventArgs e)
        {
            _mpCurSound.Stop();
            mediaPlayerIsPlaying = false;
            stopButtonIsPress = true;
        }

        private void sliProgress_DragStarted(object sender, DragStartedEventArgs e)
        {
            userIsDraggingSlider = true;
        }

        private void sliProgress_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            userIsDraggingSlider = false;
            _mpCurSound.Position = TimeSpan.FromSeconds(sliProgress.Value);
        }

        private void sliProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lblProgressStatus.Text = TimeSpan.FromSeconds(sliProgress.Value).ToString(@"hh\:mm\:ss");
        }

        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            _mpCurSound.Volume += (e.Delta > 0) ? 0.005 : -0.005;
            pbVolume.Value += (e.Delta > 0) ? 0.005 : -0.005;
        }


        //========V3=========



        //========V4=========


        void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            var key = e.Key;
            if (key == Key.F2 && OsuDB != null)
            {
                ThreadMusic = new Thread(PlayMusicThread) { IsBackground = true };
                stopButtonIsPress = false;
                chek++;
                ThreadMusic.Start();
            }

        }

        private void MouseEnterBM(object sender, MouseEventArgs e)
        {
            var brush = new BrushConverter();
            ((StackPanel)sender).Background = (Brush)brush.ConvertFrom("#66020FFF");
            ((StackPanel)sender).Cursor = Cursors.Hand;

            ThicknessAnimation justAnimation = new ThicknessAnimation();

            justAnimation.From = ((StackPanel)sender).Margin;//new Thickness(0, 15, 55, 0);
            justAnimation.To = new Thickness(50, 15, 5, 0);
            justAnimation.Duration = TimeSpan.FromSeconds(0.3);

            ((StackPanel)sender).BeginAnimation(ScrollViewer.MarginProperty, justAnimation);

        }
        private void MouseLeaveBM(object sender, MouseEventArgs e)
        {
            int numberBmInList = Int32.Parse((Regex.Match(((StackPanel)sender).Name, @"\d+").Value));

            if (numberBmInList - 1 != fdg)
            {
                var brush = new BrushConverter();
                ((StackPanel)sender).Background = (Brush)brush.ConvertFrom("#66000227");

                ThicknessAnimation justAnimation = new ThicknessAnimation();

                justAnimation.From = ((StackPanel)sender).Margin; //  new Thickness(50, 15, 5, 0);
                justAnimation.To = new Thickness(0, 15, 55, 0);
                justAnimation.Duration = TimeSpan.FromSeconds(0.3);

                ((StackPanel)sender).BeginAnimation(ScrollViewer.MarginProperty, justAnimation);
            }

        }
        private void MouseClickBM(object sender, MouseEventArgs e)
        {
            ThreadMusic = new Thread(PlayMusicThread) { IsBackground = true };
            stopButtonIsPress = false;
            chek++;

            chooseBM = true;

            chosenBM = Int32.Parse((Regex.Match(((StackPanel)sender).Name, @"\d+").Value));
            ThreadMusic.Start();
            flagLinePlay = true;

        }


        private void SortBM(object sender, TextChangedEventArgs e)
        {
            //System.Windows.MessageBox.Show(TextFindBM.Text);
            //BoxMap.Children.Clear();
            //RenderBM(TextFindBM.Text);

            string findMap = "";

            foreach (StackPanel x in BoxMap.Children)
            {

                findMap = "";

                findMap += ((TextBlock)((Label)x.Children[0]).Content).Text + " ";
                findMap += ((Label)x.Children[1]).Content + " ";
                findMap += ((Label)x.Children[2]).Content + " ";

                if (!findMap.ToUpper().Contains(TextFindBM.Text.ToUpper()) && TextFindBM.Text != "")
                {
                    x.Visibility = Visibility.Collapsed;
                }
                else
                {

                    x.Visibility = Visibility.Visible;
                }


            }

        }

        private void HiddenContent(object sender, RoutedEventArgs e)
        {
            LogoIm.Visibility = Visibility.Hidden;
            SelectFile.Visibility = Visibility.Hidden;
            BMandCollectionGrid.Visibility = Visibility.Hidden;
            ActivityGrid.Visibility = Visibility.Hidden;
            ExportGrid.Visibility = Visibility.Hidden;

            //MainWindow.Width = 1920;

            //MainWindowScreen.ResizeMode = ResizeMode.NoResize;
            MainWindowScreen.WindowStyle = WindowStyle.None;
        }

        private void VisiblContent(object sender, MouseEventArgs e)
        {
            LogoIm.Visibility = Visibility.Visible;
            SelectFile.Visibility = Visibility.Visible;
            BMandCollectionGrid.Visibility = Visibility.Visible;
            ActivityGrid.Visibility = Visibility.Visible;
            ExportGrid.Visibility = Visibility.Visible;
            MainWindowScreen.WindowStyle = WindowStyle.SingleBorderWindow;
            //MainWindow.Width = 1920;
            MainWindowScreen.ResizeMode = ResizeMode.CanResize;
        }

        //=======V4=========

        //=========V5=========
        private void OpenFileMouseEnter(object sender, MouseEventArgs e)
        {
            
            btnOpenFile.Content = "";
            Image imag = new Image();
            imag.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "/Source/opfld2.png"));
            btnOpenFile.Content = imag;
        }

        private void OpenFileMouseLeave(object sender, MouseEventArgs e)
        {

            btnOpenFile.Content = "";
            Image imag = new Image();
            imag.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "/Source/opfld1.png"));
            btnOpenFile.Content = imag;
        }



        //==========V5========




        public bool BackroundSet()
        {






            Random rnd = new Random();
            int randomSong = rnd.Next(0, (int)OsuDB.NumberOfBM);
            //randomSong = 766;
            justFlag = randomSong;
            if (chooseBM)
            {
                int randomBg = 0;
                int indexSave = 0;
                for (int index = 0; index < OsuDB.NumberOfBM; index++)
                {
                    if (massivBmSetAndIndex[index] == chosenBM)
                    {
                        indexSave = index;

                        while (massivBmSetAndIndex[index] == chosenBM)
                        {
                            randomBg++;
                            index++;
                        }

                        randomSong = rnd.Next(indexSave, randomBg + indexSave - 1);
                        chooseBM = false;
                        lineStartPlay = randomSong;
                        justFlag = randomSong;
                        break;
                    }
                }

            }
            else
            {

                if (flagLinePlay)
                {
                    for (int index = lineStartPlay; index < OsuDB.NumberOfBM; index++)
                    {
                        if (massivBmSetAndIndex[lineStartPlay] != massivBmSetAndIndex[index])
                        {
                            randomSong = index;
                            lineStartPlay = index;
                            //numberOfSong = massivBmSetAndIndex[index];
                            justFlag = randomSong;
                            break;

                        }
                    }
                }
            }




            string nameOsuFile = OsuDB.Beatmaps[randomSong].ArtistName + " - " + OsuDB.Beatmaps[randomSong].SongName + " (" + OsuDB.Beatmaps[randomSong].CreaterName + ") [" + OsuDB.Beatmaps[randomSong].Difficulty + "].osu";
            var invalidChars = System.IO.Path.GetInvalidFileNameChars();
            nameOsuFile = new string(nameOsuFile.Where(x => !invalidChars.Contains(x)).ToArray());


            string nameJpg = "";

            try
            {

                string allFile = System.IO.File.ReadAllText(PathOsu + "\\Songs\\" + OsuDB.Beatmaps[randomSong].SongFolder + "\\" + nameOsuFile, Encoding.Default).Replace("\n", " ");
                nameJpg = ((Regex.Match(allFile, @"(?i)[\w\s'-\.\[\]\^\!\~\\\/\+\&]+\.jpg|[\w\s'-\.\[\]\^\!\~\\\/\+\&]+\.png|[\w\s'-\.\[\]\^\!\~\\\/\+\&]+\.jpeg(?i)").Value));
                if (nameJpg != "")
                    this.Dispatcher.Invoke(() => { ImageBG.Source = new BitmapImage(new Uri(PathOsu + "\\Songs\\" + OsuDB.Beatmaps[randomSong].SongFolder + "\\" + nameJpg)); });
                else
                {
                    Dispatcher.Invoke(() => { ImageBG.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\bgNoBG.png")); });
                    //System.Windows.MessageBox.Show("Ошибка чтения БГшки: \n" + PathOsu + "\\Songs\\" + OsuDB.Beatmaps[randomSong].SongFolder + "\\" + nameOsuFile);
                }


                nameJpg = ((Regex.Match(allFile, @"(?i)[\w\s\(\)\!\.\[\]\'\^\-\~\+\\\/\&]+\.mp3(?i)").Value));
                this.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        PlayMusic(PathOsu + "\\Songs\\" + OsuDB.Beatmaps[randomSong].SongFolder + "\\" + nameJpg.Remove(0, 1));
                    }
                    catch
                    {
                        System.Windows.MessageBox.Show("Ошибка M1: \n" + PathOsu + "\\Songs\\" + OsuDB.Beatmaps[randomSong].SongFolder + "\\" + nameOsuFile);
                    }
                });


                return true;

            }
            catch
            {
                {
                    System.Windows.MessageBox.Show("Ошибка B1: \n" + PathOsu + "\\Songs\\" + OsuDB.Beatmaps[randomSong].SongFolder + "\\" + nameOsuFile);
                }
                return false;
            }



        }

        private void PlayMusic(string path)
        {
            try
            {
                _mpCurSound.Open(new Uri(path, UriKind.Absolute));
                stopButtonIsPress = false;
            }
            catch
            {

            }
        }

        public bool RenderBM(string findBmName)
        {
            Stopwatch stopWatch = new Stopwatch();
            TimeSpan timeReader = stopWatch.Elapsed;


            string path = @"G:";
            try
            {
                BinaryReader readerDb = new BinaryReader(File.Open(PathOsu + "\\osu!.db", FileMode.Open));

                stopWatch = new Stopwatch();
                stopWatch.Start();
                OsuDB = new ReaderOSU.ReaderOsuDB(ref readerDb);
                stopWatch.Stop();
                readerDb.Close();
                timeReader = stopWatch.Elapsed;
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", timeReader.Hours, timeReader.Minutes, timeReader.Seconds, timeReader.Milliseconds / 10);


                massivBmSetAndIndex = new int[OsuDB.NumberOfBM];
                int countSet = 0;


                uint setBMflag = 0;// = (OsuDB.Beatmaps[0].BeatmapSetID == 4294967295) ? UInt32.Parse((Regex.Match(OsuDB.Beatmaps[0].SongFolder, @"\G[0-9]{0,}").Value)) : OsuDB.Beatmaps[0].BeatmapSetID;
                uint numberBmInset = 0;



                for (uint index = 0; index < OsuDB.NumberOfBM; index++)
                {

                    //string fingText = OsuDB.Beatmaps[index].ArtistName + " "
                    //    + OsuDB.Beatmaps[index].BeatmapID + " "
                    //    + OsuDB.Beatmaps[index].BeatmapSetID + " "
                    //    + OsuDB.Beatmaps[index].CreaterName + " "
                    //    + OsuDB.Beatmaps[index].SongName + " "
                    //    + OsuDB.Beatmaps[index].Difficulty;

                    //if ((Regex.Match(fingText, @"" + findBmName)).ToString() != "" || TextFindBM.Text == "")
                    //{


                    StackPanel elementBM = new StackPanel();



                    Label nameSongLabel = new Label();
                    Label nameCreaterLabel = new Label();
                    Label countSetLabel = new Label();




                    nameSongLabel.Style = this.Resources["LabelFv"] as Style;
                    nameCreaterLabel.Style = this.Resources["LabelThd"] as Style;
                    countSetLabel.Style = this.Resources["LabelThd"] as Style;



                    OsuDB.Beatmaps[index].BeatmapSetID = (OsuDB.Beatmaps[index].BeatmapSetID == 4294967295) ? UInt32.Parse((Regex.Match(OsuDB.Beatmaps[0].SongFolder, @"\G[0-9]{0,}").Value)) : OsuDB.Beatmaps[index].BeatmapSetID;
                    if (OsuDB.Beatmaps[index].BeatmapSetID != setBMflag)
                    {
                        if (numberBmInset != 0)
                        {
                            countSetLabel.Content = "Карт в сете: " + numberBmInset;
                            numberBmInset = 0;
                            countSet++;

                            elementBM.Children.Add(nameSongLabel);
                            elementBM.Children.Add(nameCreaterLabel);
                            elementBM.Children.Add(countSetLabel);
                            elementBM.Style = this.Resources["StackPanelFirst"] as Style;
                            elementBM.MouseEnter += MouseEnterBM;
                            elementBM.MouseLeave += MouseLeaveBM;
                            elementBM.MouseDown += MouseClickBM;
                            elementBM.Name = "bm" + countSet;
                            BoxMap.Children.Add(elementBM);
                        }
                        //BMTextBlock.Text += "Карт в сете: " + numberBmInset + "\n\n";
                        numberBmInset = 1;
                        massivBmSetAndIndex[index] = countSet;
                        TextBlock textNameBM = new TextBlock();
                        textNameBM.TextWrapping = TextWrapping.Wrap;
                        textNameBM.Text = OsuDB.Beatmaps[index].ArtistName + " - " + OsuDB.Beatmaps[index].SongName;
                        nameSongLabel.Content = textNameBM;

                        nameCreaterLabel.Content = OsuDB.Beatmaps[index].CreaterName;


                        setBMflag = OsuDB.Beatmaps[index].BeatmapSetID;
                    }
                    else
                    {
                        numberBmInset++;
                        massivBmSetAndIndex[index] = countSet;
                        if (index == OsuDB.NumberOfBM)
                        {
                            elementBM.Children.Add(nameSongLabel);
                            elementBM.Children.Add(nameCreaterLabel);
                            elementBM.Children.Add(countSetLabel);
                            elementBM.Style = this.Resources["StackPanelFirst"] as Style;
                            elementBM.MouseEnter += MouseEnterBM;
                            elementBM.MouseLeave += MouseLeaveBM;
                            elementBM.MouseDown += MouseClickBM;
                            elementBM.Name = "bm" + countSet;
                            BoxMap.Children.Add(elementBM);
                        }
                    }
                }
                //}
                return true;
            }

            catch
            {
                {
                    System.Windows.MessageBox.Show("Это точно папка OSU?\nЗдесь нет osu!.db\nНУ или в ошибка при чтении");
                }
                return false;
            }

        }
        public bool RenderCollection()
        {
            string path = @"G:";

            try
            {
                BinaryReader readerDb = new BinaryReader(File.Open(PathOsu + "\\collection.db", FileMode.Open));
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                CollectionDB = new ReaderCollection.ReaderCollectionDB(ref readerDb);
                stopWatch.Stop();
                readerDb.Close();
                TimeSpan ts = stopWatch.Elapsed;
                string elapsedTime1 = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);

                //CollectionTextBox.Text += "Время чтения: " + elapsedTime1 + "\n";
                //CollectionTextBox.Text += "Версия: " + CollectionDB.Version + "\n";
                //CollectionTextBox.Text += "Количество коллекций: " + CollectionDB.NumberOfCollections + "\n\n";



                Collections = new NorthOsuFileCreator.NorthCollection[0];



                //ПЕРЕБОР КОЛЛЕКЦИЙ
                for (uint index = 0; index < CollectionDB.NumberOfCollections; index++)
                {

                    StackPanel collectionBlock = new StackPanel();
                    collectionBlock.Style = this.Resources["StackSnd"] as Style;
                    Label nameCollection = new Label();




                    nameCollection.Content = CollectionDB.Collections[index].NameCollection;
                    nameCollection.Style = this.Resources["LabelFr"] as Style;
                    collectionBlock.Children.Add(nameCollection);


                    Array.Resize(ref Collections, Collections.Length + 1);
                    Collections[Collections.Length - 1] = new NorthOsuFileCreator.NorthCollection();

                    Collections[Collections.Length - 1].NameCollection = CollectionDB.Collections[index].NameCollection;
                    Collections[Collections.Length - 1].NumberOfBMInCollection = CollectionDB.Collections[index].NumberOfBMInCollection;

                    Collections[Collections.Length - 1].BMsCollection = new List<BMSetShortInfo>();

                    Button addToExport;
                    addToExport = new Button() { Name = "ex" + index, Content = "Экспорт+" };
                    addToExport.Style = Resources["ButtonFs"] as Style;
                    addToExport.Click += AddColletionExport;
                    collectionBlock.Children.Add(addToExport);


                    string flagName = "";
                    //ПЕРЕБОР КАРТ В КОЛЛЕКЦИИ
                    for (uint index2 = 0; index2 < CollectionDB.Collections[index].NumberOfBMInCollection; index2++)
                    {
                        Label nameSong = new Label();
                        nameSong.Style = this.Resources["LabelSx"] as Style;
                        bool existBM = false;



                        //ПОИСК МД5ХЭШ
                        for (uint index3 = 0; index3 < OsuDB.NumberOfBM; index3++)
                        {
                            if (CollectionDB.Collections[index].MD5Hash[index2] == OsuDB.Beatmaps[index3].MD5Hash)
                            {
                                existBM = true;
                                if (flagName != OsuDB.Beatmaps[index3].SongName)
                                {
                                    flagName = OsuDB.Beatmaps[index3].SongName;
                                    nameSong.Content = OsuDB.Beatmaps[index3].ArtistName + " - " + OsuDB.Beatmaps[index3].SongName;
                                    collectionBlock.Children.Add(nameSong);
                                    TextBlock textLink = new TextBlock();
                                    textLink.Style = this.Resources["TextBlockSnd"] as Style;

                                    Hyperlink linkBMCom = new Hyperlink();
                                    linkBMCom.NavigateUri = new Uri("https://osu.ppy.sh/beatmapsets/" + OsuDB.Beatmaps[index3].BeatmapSetID);
                                    linkBMCom.RequestNavigate += HyperlinkRequestNavigate;
                                    linkBMCom.Style = Resources["LinkFs"] as Style;
                                    linkBMCom.Inlines.Add("Ссылка");
                                    textLink.Inlines.Add(linkBMCom);
                                    //CollectionTextBox.Text += "\t" + "\t" + OsuDB.Beatmaps[index3].BeatmapSetID + "\n";
                                    Border border = new Border();
                                    border.Style = Resources["BorderFs"] as Style;

                                    border.Child = textLink;

                                    collectionBlock.Children.Add(border);



                                    //Array.Resize(ref Collections[Collections.Length - 1].BMsCollection, Collections[Collections.Length - 1].BMsCollection.Length + 1);
                                    BMSetShortInfo setInfo = new BMSetShortInfo();
                                    setInfo.DifficultySet = new List<BMShortInfo>();
                                    setInfo.ArtistName = OsuDB.Beatmaps[index3].ArtistName;
                                    setInfo.SongName = OsuDB.Beatmaps[index3].SongName;
                                    setInfo.CreaterName = OsuDB.Beatmaps[index3].CreaterName;
                                    setInfo.LinkSet = "https://osu.ppy.sh/beatmapsets/" + OsuDB.Beatmaps[index3].BeatmapSetID;
                                    Collections[Collections.Length - 1].BMsCollection.Add(setInfo);

                                }
                                Label diffBm = new Label();
                                diffBm.Style = this.Resources["LabelSv"] as Style;
                                diffBm.Content = "\t" + "\t" + "\u27A4 " + OsuDB.Beatmaps[index3].Difficulty;
                                collectionBlock.Children.Add(diffBm);

                                BMShortInfo difInfo = new BMShortInfo();

                                difInfo.Difficulty = OsuDB.Beatmaps[index3].Difficulty;
                                difInfo.MD5Hash = OsuDB.Beatmaps[index3].MD5Hash;

                                Collections[Collections.Length - 1].BMsCollection[Collections[Collections.Length - 1].BMsCollection.Count - 1].DifficultySet.Add(difInfo);


                                break;
                            }

                        }
                        if (!existBM)
                        {
                            Label nameBM = new Label();
                            nameBM.Style = this.Resources["LabelThd"] as Style;
                            nameBM.Content = "Карта отсутствует: " + CollectionDB.Collections[index].MD5Hash[index2];
                            collectionBlock.Children.Add(nameBM);
                        }
                        //CollectionTextBox.Text += "\tКарта отсутствует: " + CollectionDB.Collections[index].MD5Hash[index2] + "\n";

                        //if()
                        //CollectionTextBox.Text += "\t=========Конец карты==========\n";
                    }

                    BoxCollection.Children.Add(collectionBlock);
                    //CollectionTextBox.Text += "===========Конец коллекции=============\n\n";
                }
                return true;

            }
            catch
            {
                {
                    System.Windows.MessageBox.Show("Это точно папка OSU?\nЗдесь нет collection.db\nНУ или в ошибка при чтении");
                }
                return false;
            }


        }

        private void ExportListView()
        {
            ExportList.Children.Clear();
            if (ExortFile.NameCreatorfFile != null)
            {
                Label nameCreator = new Label();
                nameCreator.Style = this.Resources["LabelScnd"] as Style;
                nameCreator.Content = ExortFile.NameCreatorfFile;
                ExportList.Children.Add(nameCreator);



                int index = 0;
                foreach (NorthOsuFileCreator.NorthCollection x in ExortFile.Colletions)
                {
                    StackPanel collectionBlock = new StackPanel();
                    collectionBlock.Style = this.Resources["StackSnd"] as Style;
                    Label nameCollection = new Label();
                    nameCollection.Content = x.NameCollection;
                    nameCollection.Style = this.Resources["LabelFr"] as Style;
                    collectionBlock.Children.Add(nameCollection);

                    Button delToExport;
                    delToExport = new Button() { Name = "de" + index, Content = "-Экспорт" };
                    delToExport.Style = this.Resources["ButtonFs"] as Style;
                    index++;
                    delToExport.Click += DeleteColletionExport;
                    collectionBlock.Children.Add(delToExport);

                    foreach (BMSetShortInfo y in x.BMsCollection)
                    {
                        Label nameSong = new Label();
                        nameSong.Style = this.Resources["LabelSx"] as Style;
                        nameSong.Content = y.SongName;
                        collectionBlock.Children.Add(nameSong);

                        TextBlock textLink = new TextBlock();
                        textLink.Style = this.Resources["TextBlockSnd"] as Style;
                        Hyperlink linkBMCom = new Hyperlink();
                        linkBMCom.NavigateUri = new Uri(y.LinkSet);
                        linkBMCom.RequestNavigate += HyperlinkRequestNavigate;
                        linkBMCom.Style = this.Resources["LinkFs"] as Style;
                        linkBMCom.Inlines.Add("Ссылка");
                        textLink.Inlines.Add(linkBMCom);
                        Border border = new Border();
                        border.Style = this.Resources["BorderFs"] as Style;
                        border.Child = textLink;


                        collectionBlock.Children.Add(border);
                        foreach (BMShortInfo z in y.DifficultySet)
                        {
                            Label diffBm = new Label();
                            diffBm.Style = this.Resources["LabelSv"] as Style;
                            diffBm.Content = "\t" + "\t\u27A4 " + z.Difficulty;
                            collectionBlock.Children.Add(diffBm);
                        }

                    }
                    ExportList.Children.Add(collectionBlock);
                }
            }
        }



        private void OpenOsuFolder(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            DialogResult result = folderBrowser.ShowDialog();
            if (!string.IsNullOrWhiteSpace(folderBrowser.SelectedPath))
            {
                PathOsu = folderBrowser.SelectedPath;
                txtEditor.Text = PathOsu;
                DowloadDB(sender, e);
            }
            
        }
        private void DowloadDB(object sender, RoutedEventArgs e)
        {
            Flex_lug.Visibility = Visibility.Hidden;
            BoxMap.Children.Clear();
            BoxCollection.Children.Clear();
            ExportList.Children.Clear();
            if (RenderBM(""))
                if (RenderCollection())
                    if (BackroundSet())
                    {
                        ExortFile = new NorthOsuFileCreator.NorthOsuFileCreator(OsuDB.AccountName);
                        Flex_lug.Visibility = Visibility;
                    }
        }
        private void HyperlinkRequestNavigate(object sender, RequestNavigateEventArgs e)
        {

            Process.Start("explorer.exe", e.Uri.ToString());
        }

        private void DeleteColletionExport(object sender, RoutedEventArgs e)
        {
            int numberButtonClick = Int32.Parse((Regex.Match(((Button)sender).Name, @"\d+").Value));
            ExortFile.DeleteColletion(numberButtonClick);
            ExportListView();

        }
        private void AddColletionExport(object sender, RoutedEventArgs e)
        {


            //System.Windows.MessageBox.Show("yeeeees");

            StackPanel parentButton = (StackPanel)VisualTreeHelper.GetParent((Button)sender);
            string nameCollection = ((Label)parentButton.Children[0]).Content.ToString();

            int numberButtonClick = Int32.Parse((Regex.Match(((Button)sender).Name, @"\d+").Value));
            bool flagAddCollection = false;
            foreach (NorthOsuFileCreator.NorthCollection x in ExortFile.Colletions)
            {
                if (nameCollection == x.NameCollection)
                    flagAddCollection = true;
            }
            if (!flagAddCollection)
            {
                ExortFile.AddColletion(Collections[numberButtonClick]);
                ExportListView();
            }

            { }

        }
        private void ClearExportList(object sender, RoutedEventArgs e)
        {
            if (OsuDB != null)
                ExortFile = new NorthOsuFileCreator.NorthOsuFileCreator(OsuDB.AccountName);
            else
            {
                ExportList.Children.Clear();
                ExortFile = new NorthOsuFileCreator.NorthOsuFileCreator();
            }
            ExportListView();
        }
        private void ExortFileNorth(object sender, RoutedEventArgs e)
        {
            BinaryWriter writeNorth = new BinaryWriter(File.Open("north.db", FileMode.OpenOrCreate));

            writeNorth.Write(ExortFile.NameCreatorfFile);
            writeNorth.Write(ExortFile.NumberOfCollections);
            foreach (NorthOsuFileCreator.NorthCollection x in ExortFile.Colletions)
            {
                writeNorth.Write(x.NameCollection);
                writeNorth.Write(x.NumberOfBMInCollection);
                foreach (BMSetShortInfo y in x.BMsCollection)
                {
                    writeNorth.Write(y.ArtistName);
                    writeNorth.Write(y.SongName);
                    writeNorth.Write(y.CreaterName);
                    writeNorth.Write(y.LinkSet);
                    writeNorth.Write(y.DifficultySet.Count);
                    foreach (BMShortInfo z in y.DifficultySet)
                    {
                        writeNorth.Write(z.Difficulty);
                        writeNorth.Write(z.MD5Hash);
                    }
                }
            }
            writeNorth.Close();
        }
        private void ImportExportList(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog folderBrowser = new System.Windows.Forms.OpenFileDialog();
            DialogResult result = folderBrowser.ShowDialog();
            if (!string.IsNullOrWhiteSpace(folderBrowser.FileName))
            {
                PathNorth = folderBrowser.FileName;
                CookieFikePath.Text = PathNorth;



                BinaryReader reader = new BinaryReader(File.Open(PathNorth, FileMode.Open));

                ExortFile = new NorthOsuFileCreator.NorthOsuFileCreator(reader.ReadString());
                ExortFile.NumberOfCollections = reader.ReadUInt32();

                for (uint index = 0; index < ExortFile.NumberOfCollections; index++)
                {
                    NorthCollection collection = new NorthCollection();
                    collection.NameCollection = reader.ReadString();
                    collection.NumberOfBMInCollection = reader.ReadUInt32();
                    collection.BMsCollection = new List<BMSetShortInfo>();

                    //string flag = "";
                    int index2 = 0;
                    while (reader.BaseStream.Position != reader.BaseStream.Length && index2 < collection.NumberOfBMInCollection)
                    {
                        BMSetShortInfo setInfo;
                        setInfo.ArtistName = reader.ReadString();
                        setInfo.SongName = reader.ReadString();
                        setInfo.CreaterName = reader.ReadString();
                        setInfo.LinkSet = reader.ReadString();
                        setInfo.NumberDif = reader.ReadUInt32();
                        setInfo.DifficultySet = new List<BMShortInfo>();
                        for (uint index3 = 0; index3 < setInfo.NumberDif; index3++)
                        {
                            BMShortInfo difInfo = new BMShortInfo();
                            difInfo.Difficulty = reader.ReadString();
                            difInfo.MD5Hash = reader.ReadString();
                            setInfo.DifficultySet.Add(difInfo);
                            index2++;
                        }

                        collection.BMsCollection.Add(setInfo);
                    }
                    ExortFile.Colletions.Add(collection);
                }


                ExportListView();
                reader.Close();
            }


        }

        private void ViewBM(object sender, RoutedEventArgs e)
        {
            //ScrollBM.Margin = new Thickness(0, 0, 0, 0);
            //ScrollCollection.Margin = new Thickness(-1000,0,0,0);
            ThicknessAnimation justAnimation = new ThicknessAnimation();

            justAnimation.From = new Thickness(0, 0, 0, 0);
            justAnimation.To = new Thickness(-1000, 0, 0, 0);
            justAnimation.Duration = TimeSpan.FromSeconds(0.3);



            ScrollCollection.BeginAnimation(ScrollViewer.MarginProperty, justAnimation);
            ScrollCollection.Visibility = Visibility.Hidden;
            ScrollBM.Visibility = Visibility.Visible;

            justAnimation.From = new Thickness(-1000, 0, 0, 0);
            justAnimation.To = new Thickness(0, 0, 0, 0);
            justAnimation.Duration = TimeSpan.FromSeconds(0.3);
            ScrollBM.BeginAnimation(ScrollViewer.MarginProperty, justAnimation);

        }

        private void ViewCollection(object sender, RoutedEventArgs e)
        {
            //ScrollBM.Margin = new Thickness(-1000, 0, 0, 0);
            //ScrollCollection.Margin = new Thickness(0, 0, 0, 0);

            ThicknessAnimation justAnimation = new ThicknessAnimation();

            justAnimation.From = new Thickness(0, 0, 0, 0);
            justAnimation.To = new Thickness(-1000, 0, 0, 0);
            justAnimation.Duration = TimeSpan.FromSeconds(0.3);

            ScrollBM.BeginAnimation(ScrollViewer.MarginProperty, justAnimation);
            ScrollBM.Visibility = Visibility.Hidden;
            ScrollCollection.Visibility = Visibility.Visible;


            justAnimation.From = new Thickness(-1000, 0, 0, 0);
            justAnimation.To = new Thickness(0, 0, 0, 0);
            justAnimation.Duration = TimeSpan.FromSeconds(0.3);
            ScrollCollection.BeginAnimation(ScrollViewer.MarginProperty, justAnimation);

        }



        //ПЕРЕПИСАТЬ
        public void PlayMusicThread()
        {
            try
            {
                BackroundSet();
                this.Dispatcher.Invoke(() => { mediaPlayerIsPlaying = true; });


                double position = 0;
                Dispatcher.Invoke(() =>
                {
                    //var sdf = ((StackPanel)(ScrollBM.FindName("BoxMap")));
                    fdg = massivBmSetAndIndex[justFlag];
                    double sroll = 0;
                    for (int index = 0; index < fdg - 1; index++)
                    {

                        var adf = ((StackPanel)(BoxMap.Children[index]));
                        sroll += adf.ActualHeight + 15;
                        ScrollBM.ScrollToVerticalOffset(sroll);


                    }



                    fdg--;

                    var brush = new BrushConverter();
                    ((StackPanel)BoxMap.Children[fdg]).Background = (Brush)brush.ConvertFrom("#66020FFF");
                    ((StackPanel)BoxMap.Children[fdg]).Cursor = Cursors.Hand;

                    ThicknessAnimation justAnimation = new ThicknessAnimation();

                    justAnimation.From = new Thickness(0, 15, 55, 0); //((StackPanel)BoxMap.Children[fdg]).Margin;
                    justAnimation.To = new Thickness(50, 15, 5, 0);
                    justAnimation.Duration = TimeSpan.FromSeconds(0.01);

                    ((StackPanel)BoxMap.Children[fdg]).BeginAnimation(StackPanel.MarginProperty, justAnimation);


                });

                this.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        Thread.Sleep(200);

                        position = _mpCurSound.Position.TotalMilliseconds;
                        time = _mpCurSound.NaturalDuration.TimeSpan.TotalMilliseconds;
                    }
                    catch
                    {
                        System.Windows.MessageBox.Show("Крч дело обстоит так:\nВот этот музон грузиться оч долго(ГОВНО),\nПоэтому ты его слушай, но он может \nрезко закончиться\nОшибка M2: \n" + PathOsu + "\\Songs\\" + OsuDB.Beatmaps[numberOfSong].SongFolder);
                    }
                });
                //this.Dispatcher.Invoke(() => { FF.Content = _mpCurSound.Source.LocalPath; });
                this.Dispatcher.Invoke(() => { _mpCurSound.Play(); });

                while ((position < time && mediaPlayerIsPlaying) || (stopButtonIsPress && position < time))
                {

                    if (chek > 1)
                    {
                        chek--;
                        Dispatcher.Invoke(() =>
                        {

                            var brush = new BrushConverter();
                            ((StackPanel)BoxMap.Children[fdg]).Background = (Brush)brush.ConvertFrom("#66000227");
                            ((StackPanel)BoxMap.Children[fdg]).Cursor = Cursors.Hand;

                            ThicknessAnimation justAnimation = new ThicknessAnimation();

                            justAnimation.From = new Thickness(50, 15, 5, 0); //((StackPanel)BoxMap.Children[fdg]).Margin;
                            justAnimation.To = new Thickness(0, 15, 55, 0);
                            justAnimation.Duration = TimeSpan.FromSeconds(0.01);

                            ((StackPanel)BoxMap.Children[fdg]).BeginAnimation(StackPanel.MarginProperty, justAnimation);
                        });
                        return;
                    }
                    //while (stopButtonIsPress)
                    //{
                    //    Thread.Sleep(10);
                    //}
                    Dispatcher.Invoke(() =>
                    {
                        try
                        {
                            time = _mpCurSound.NaturalDuration.TimeSpan.TotalMilliseconds;
                            position = _mpCurSound.Position.TotalMilliseconds;
                        }
                        catch
                        {
                            System.Windows.MessageBox.Show("Ошибка M3: \n" + PathOsu + "\\Songs\\" + OsuDB.Beatmaps[numberOfSong].SongFolder);
                        }
                    });
                    Thread.Sleep(10);
                    //Thread.Sleep(10);
                    //i += 10;
                    //if(!(position < time))
                    //        mediaPlayerIsPlaying = false;
                }

                this.Dispatcher.Invoke(() => { _mpCurSound.Stop(); });

                Dispatcher.Invoke(() =>
                {

                    var brush = new BrushConverter();
                    ((StackPanel)BoxMap.Children[fdg]).Background = (Brush)brush.ConvertFrom("#66000227");
                    ((StackPanel)BoxMap.Children[fdg]).Cursor = Cursors.Hand;

                    ThicknessAnimation justAnimation = new ThicknessAnimation();

                    justAnimation.From = new Thickness(50, 15, 5, 0); //((StackPanel)BoxMap.Children[fdg]).Margin;
                    justAnimation.To = new Thickness(0, 15, 55, 0);
                    justAnimation.Duration = TimeSpan.FromSeconds(0.01);

                    ((StackPanel)BoxMap.Children[fdg]).BeginAnimation(StackPanel.MarginProperty, justAnimation);
                });

                PlayMusicThread();


            }
            catch
            {
                {
                    System.Windows.MessageBox.Show("Ошибка M4G: \n" + PathOsu + "\\Songs\\" + OsuDB.Beatmaps[numberOfSong].SongFolder);
                }
                Dispatcher.Invoke(() =>
                {

                    var brush = new BrushConverter();
                    ((StackPanel)BoxMap.Children[fdg]).Background = (Brush)brush.ConvertFrom("#66000227");
                    ((StackPanel)BoxMap.Children[fdg]).Cursor = Cursors.Hand;

                    ThicknessAnimation justAnimation = new ThicknessAnimation();

                    justAnimation.From = new Thickness(50, 15, 5, 0); //((StackPanel)BoxMap.Children[fdg]).Margin;
                    justAnimation.To = new Thickness(0, 15, 55, 0);
                    justAnimation.Duration = TimeSpan.FromSeconds(0.3);

                    ((StackPanel)BoxMap.Children[fdg]).BeginAnimation(StackPanel.MarginProperty, justAnimation);
                });
                PlayMusicThread();
            }


        }


        private void FlexLug(object sender, RoutedEventArgs e)
        {
            //if (thread.IsAlive)
            //    chek = false;//thread.Join() ;
            //BackroundSet();

            ThreadMusic = new Thread(PlayMusicThread) { IsBackground = true };
            stopButtonIsPress = false;
            chek++;
            flagLinePlay = false;
            ThreadMusic.Start();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //System.Windows.MessageBox.Show("хакрытие");
            chek = 10000;
        }


        //================v



    }
}
