using GubaidullinLanguage.Data;
using GubaidullinLanguage.Helpers;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GubaidullinLanguage.Pages
{
    /// <summary>
    /// Interaction logic for ClientPage.xaml
    /// </summary>
    public partial class ClientPage : Page
    {
        int CountInPage = 10;
        int CountRecords;
        int CountPage;
        int CurrentPage = 0;

        List<Client> CurrentPageList = new List<Client>();
        List<Client> TableList;
        public ClientPage()
        {
            InitializeComponent();

            ClientsListView.ItemsSource = GubaidullinLanguageEntities.GetContext().Client.ToList();

            ComboType.SelectedIndex = 0;
            strCount.SelectedIndex = 0;
            TBAllRecords.Text = GubaidullinLanguageEntities.GetContext().Client.ToList().Count().ToString();
            UpdateClients();
        }

        private void UpdateClients()
        {

            var current = GubaidullinLanguageEntities.GetContext().Client.ToList();

            current = current.Where(c => c.LastName.ToLower().Contains(SearchTextBox.Text.ToLower()) 
            || c.FirstName.ToLower().Contains(SearchTextBox.Text.ToLower()) 
            || c.Patronymic.ToLower().Contains(SearchTextBox.Text.ToLower())
            || c.Phone.ToLower().Contains(SearchTextBox.Text.ToLower())
            || c.Email.ToLower().Contains(SearchTextBox.Text.ToLower())).ToList();

            if (ComboType.SelectedIndex == 1)
            {
                current = current.Where(c => c.GenderCode == "м").ToList();
            }
            if (ComboType.SelectedIndex == 2)
            {
                current = current.Where(c => c.GenderCode == "ж").ToList();
            }
            if (Up.IsChecked == true)
            {
                current = current.OrderBy(c => c.FirstName + c.LastName + c.Patronymic).ToList();
            }
            if (Down.IsChecked == true)
            {
                current = current.OrderByDescending(c => c.FirstName + c.LastName + c.Patronymic).ToList();
            }

            TBCount.Text = current.Count.ToString();
            TBAllRecords.Text = GubaidullinLanguageEntities.GetContext().Client.ToList().Count.ToString();

            TBCount.Text = current.Count().ToString();

            ClientsListView.ItemsSource = current;

            TableList = current;

            if (strCount.SelectedIndex == 0)
            {
                CountInPage = 10;
            }
            else if (strCount.SelectedIndex == 1)
            {
                CountInPage = 50;
            }
            else if (strCount.SelectedIndex == 2)
            {
                CountInPage = 200;
            }
            else if (strCount.SelectedIndex == 3)
            {
                CountInPage = 0;
            }
            ClientsListView.ItemsSource = current;
            ChangePage(0, 0);
        }

        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateClients();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateClients();
        }

        private void strCount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateClients();
        }
        private void ChangePage(int direction, int? selectedPage)
        {
            CurrentPageList.Clear();
            CountRecords = TableList.Count;
            if (CountInPage != 0)
            {
                if (CountRecords % CountInPage > 0)
                {
                    CountPage = CountRecords / CountInPage + 1;
                }
                else
                {
                    CountPage = CountRecords / CountInPage;
                }

                Boolean Ifupdate = true;

                int min;

                if (selectedPage.HasValue)
                {
                    if (selectedPage >= 0 && selectedPage <= CountPage)
                    {
                        CurrentPage = (int)selectedPage;
                        min = CurrentPage * CountInPage + CountInPage < CountRecords ? CurrentPage * CountInPage + CountInPage : CountRecords;
                        for (int i = CurrentPage * CountInPage; i < min; i++)
                        {
                            CurrentPageList.Add(TableList[i]);
                        }
                    }
                }
                else
                {
                    switch (direction)
                    {
                        case 1:
                            if (CurrentPage > 0)
                            {
                                CurrentPage--;
                                min = CurrentPage * CountInPage + CountInPage < CountRecords ? CurrentPage * CountInPage + CountInPage : CountRecords;
                                for (int i = CurrentPage * CountInPage; i < min; i++)
                                {
                                    CurrentPageList.Add(TableList[i]);
                                }
                            }
                            else
                            {
                                Ifupdate = false;
                            }
                            break;
                        case 2:
                            if (CurrentPage < CountPage - 1)
                            {
                                CurrentPage++;
                                min = CurrentPage * CountInPage + CountInPage < CountRecords ? CurrentPage * CountInPage + CountInPage : CountRecords;
                                for (int i = CurrentPage * CountInPage; i < min; i++)
                                {
                                    CurrentPageList.Add(TableList[i]);
                                }
                            }
                            else
                            {
                                Ifupdate = false;
                            }
                            break;
                    }
                }
                if (Ifupdate)
                {
                    PageListBox.Items.Clear();
                    for (int i = 1; i <= CountPage; i++)
                    {
                        PageListBox.Items.Add(i);
                    }
                    PageListBox.SelectedIndex = CurrentPage;

                    

                    ClientsListView.ItemsSource = CurrentPageList;

                    ClientsListView.Items.Refresh();
                }
            }
            else
            {
                PageListBox.Items.Clear();
                PageListBox.Items.Add(1);
            }
        }

        private void LeftDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(1, null);
        }

        private void RightDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(2, null);
        }

        private void PageListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ChangePage(0, Convert.ToInt32(PageListBox.SelectedItem.ToString()) - 1);
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (((sender as Button).DataContext as Client).VisitCount == 0)
            {
                if (MessageBox.Show("Вы точно хотите выполнить уаление?", "Внимание", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    GubaidullinLanguageEntities.GetContext().Client.Remove((sender as Button).DataContext as Client);
                    GubaidullinLanguageEntities.GetContext().SaveChanges();

                    ClientsListView.ItemsSource = GubaidullinLanguageEntities.GetContext().Client.ToList();
                    UpdateClients();
                }

            }
            else
            {
                MessageBox.Show("Невозможно удалить", "Ошибка");
            }
            UpdateClients();
        }

        private void Down_Checked(object sender, RoutedEventArgs e)
        {
            UpdateClients();
        }

        private void Up_Checked(object sender, RoutedEventArgs e)
        {
            UpdateClients();
        }
    }
}
