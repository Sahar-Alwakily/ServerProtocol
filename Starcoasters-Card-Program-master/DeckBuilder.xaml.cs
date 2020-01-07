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
    /// Interaction logic for DeckBuilder.xaml
    /// </summary>
    public partial class DeckBuilder : Window
    {
        public DeckBuilder()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //When The Window Starts, Connect to the database
            Functions.DatabaseConnect();
            UpdateSetList();
        }
        public void UpdateSetList()
        {
            try
            {
                //In Case There Is Anything In The ListView Clear It
                LIV_Sets.Items.Clear();
                //Get information on all the tables in the database
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
                        //Add the item to the listview
                        LIV_Sets.Items.Add(SetItem);
                    }
                }
            }
            catch (Exception ex)
            {
                // If something goes wrong somehow show an error explaining what went wrong then kill the application
                MessageBox.Show($"An error occured {ex}");
                Application.Current.Shutdown();
            }

        }
        public void UpdateCardList(string SetToView)
        {
            //updates the card set list
            try
            {
                //In case it contains anything clear the listview 
                LIV_Cards.Items.Clear();
                //Get all the data from the table selected with a query
                string GetCardQuery = $"SELECT * FROM {SetToView}";
                SQLiteCommand GetCardCommand = new SQLiteCommand(GetCardQuery, Globals.GlobalVars.DatabaseConnection);
                SQLiteDataReader GetCardReader = GetCardCommand.ExecuteReader();
                //Go through every card in the returned table
                while (GetCardReader.Read())
                {
                    //while there are still cards in the reader to go over, add them to the table 
                    Classes.CardOverview ReaderCard = new Classes.CardOverview();
                    //Fill in the new card with details of the card pulled from the database
                    ReaderCard.CardSetCode = GetCardReader["card_code"].ToString();
                    ReaderCard.CardName = GetCardReader["name_primary"].ToString();
                    ReaderCard.CardNameSecondary = GetCardReader["name_secondary"].ToString();
                    ReaderCard.CardCost = GetCardReader["cost"].ToString();
                    ReaderCard.CardHP = int.Parse(GetCardReader["hp"].ToString());
                    ReaderCard.CardATK = int.Parse(GetCardReader["atk"].ToString());
                    ReaderCard.CardDEF = int.Parse(GetCardReader["def"].ToString());
                    //now the tricky bit, getting the species out of the array of card keywords, however species is always 2nd so thats nice
                    //get the array of keywords
                    string[] CardKeywords = GetCardReader["keywords"].ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    //Get this keyword into the ReaderCard
                    ReaderCard.CardSpecies = CardKeywords[1];
                    //now gotta get the number of abilities
                    string[] CardAbilities = GetCardReader["ability"].ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    int AbilityCount = 0;
                    //cycle through the split array to set the Ability Count
                    foreach (string Ability in CardAbilities)
                    {
                        AbilityCount++;
                    }
                    //set the ability count to the reader cards ability count
                    ReaderCard.CardAbilityCount = AbilityCount;
                    //Make a listview item 
                    ListViewItem Card = new ListViewItem();
                    //make the ReaderCard both the items tag and content
                    Card.Tag = ReaderCard;
                    Card.Content = ReaderCard;
                    //Add the item to the list view
                    LIV_Cards.Items.Add(Card);
                }
            }
            catch (Exception ex)
            {
                //If something goes wrong somehow show an error explaining what went wrong then kill the application
                MessageBox.Show($"An error occured {ex}");
            }
        }
        private void LIV_Sets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //Extract the listview item that is selected
                if (LIV_Sets.SelectedItem != null)
                {
                    //extract the listview item from the listview
                    ListViewItem SelectedItem = (ListViewItem)LIV_Sets.SelectedItem;
                    //Get the SetOverview from the listview item
                    Classes.SetOverview SelectedSet = (Classes.SetOverview)SelectedItem.Tag;
                    //extract the name from the SetOverview
                    string SelectedSetName = SelectedSet.SetName;
                    UpdateCardList(SelectedSetName);
                }
                else
                {
                    // if they havent selected anything just return the function
                    return;
                }
            }
            catch (Exception ex)
            {
                // If something goes wrong somehow show an error explaining what went wrong then kill the application
                MessageBox.Show($"An error occured {ex}");
                Application.Current.Shutdown();
            }
        }

        private void BTN_add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Extract the listview item that is selected
                if (LIV_Cards.SelectedItem != null)
                {
                    //extract the listview item from the listview
                    ListViewItem SelectedItem = (ListViewItem)LIV_Cards.SelectedItem;
                    //Get the SetOverview from the listview item
                    Classes.CardOverview SelectedSet = (Classes.CardOverview)SelectedItem.Tag;
                    //extract the name from the SetOverview
                    ListViewItem NewItem = new ListViewItem();
                    NewItem.Tag = SelectedSet;
                    NewItem.Content = SelectedSet;
                    LIV_Deck.Items.Add(NewItem);
                }
                else
                {
                    // if they havent selected anything just return the function
                    return;
                }
            }
            catch (Exception ex)
            {
                // If something goes wrong somehow show an error explaining what went wrong then kill the application
                MessageBox.Show($"An error occured {ex}");
                Application.Current.Shutdown();
            }
        }

        private void BTN_remove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //if an item is selected
                if (LIV_Deck.SelectedItem != null)
                {
                    //Remove it
                    LIV_Deck.Items.Remove(LIV_Deck.SelectedItem);
                    //it's that simple
                }
                else
                {
                    // if they havent selected anything just return the function
                    return;
                }
            }
            catch(Exception ex)
            {
                // If something goes wrong somehow show an error explaining what went wrong then kill the application
                MessageBox.Show($"An error occured {ex}");
                Application.Current.Shutdown();
            }
            
        }
    }
}
