using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.SQLite;

namespace Starcoasters_Card_Generator
{
    /// <summary>
    /// Interaction logic for DeckViewer.xaml
    /// </summary>
    public partial class DeckViewer : Window
    {
        public DeckViewer()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
        public void UpdateDeckList()
        {
            string GetTableNames = "SELECT name FROM sqlite_master WHERE type='table'";
            SQLiteCommand GetSetCommand = new SQLiteCommand(GetTableNames, Globals.GlobalVars.DatabaseConnection);
            SQLiteDataReader SetReader = GetSetCommand.ExecuteReader();
            //Go through the rows returned by the query, mine them for data and uncerimoniously stuff each into the listview
            while (SetReader.Read())
            {
                //Check if the table selected is an internal one for sqlite
                bool TableInternal = SetReader["name"].ToString().Contains("sqlite_");
                //Make sure it isnts pulling shit from the internally designated databases (which contain sqlite_)
                if (TableInternal == false)
                {
                    //Execute a query that selects the table so we can gather data from it
                    string GetSetCode = $"SELECT * FROM {SetReader["name"].ToString()}";
                    SQLiteCommand GetTableCodeCommand = new SQLiteCommand(GetSetCode, Globals.GlobalVars.DatabaseConnection);
                    SQLiteDataReader CodeReader = GetTableCodeCommand.ExecuteReader();
                    //extracts the returned cards code and splits it into two part, the 4 letter code and the 4 digit number 
                    string TempCode = "";
                    if (CodeReader.Read())
                    {
                        string CodeToSplit = CodeReader["card_code"].ToString();
                        string[] SplitCode = CodeToSplit.Split('-');
                        TempCode = SplitCode[0].ToString();
                    }
                    //Do another query to find out the length of the table by looping through every row the data reader can find
                    SQLiteCommand GetLengthCommand = new SQLiteCommand(GetSetCode, Globals.GlobalVars.DatabaseConnection);
                    SQLiteDataReader LengthReader = GetLengthCommand.ExecuteReader();
                    int SetSize = 0;
                    while (LengthReader.Read())
                    {
                        SetSize++;
                    }
                    // Shove all the data just gathered into a Set Overview
                    Classes.SetOverview SetData = new Classes.SetOverview { SetName = SetReader["name"].ToString(), SetCode = TempCode, SetCount = SetSize };
                    //Make a list view item to store this data as a tag
                    ListViewItem SetItem = new ListViewItem();
                    SetItem.Tag = SetData;
                    //also make the set data the item so it displays properly
                    SetItem.Content = SetData;
                }
            }
        }
    }
}
