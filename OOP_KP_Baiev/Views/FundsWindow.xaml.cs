using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace OOP_KP_Baiev.Views
{
    public partial class FundsWindow : Window
    {
        private readonly User _user;

        public FundsWindow(User user)
        {
            InitializeComponent();
            _user = user;

            if (!string.IsNullOrEmpty(_user.SavedCardNumber) && _user.SavedCardNumber.ToLower() != "null")
            {
                WithdrawCardTextBox.Text = _user.SavedCardNumber;
            }
            else
            {
                WithdrawCardTextBox.Text = string.Empty;
            }
        }

        private void Deposit_Click(object sender, RoutedEventArgs e)
        {
            ClearTopUpErrors();

            string cardNumber = CardNumberTopUpTextBox.Text.Trim();
            string cvv = CVVTextBox.Text.Trim();
            string expiry = ExpiryDateTextBox.Text.Trim();
            string amountText = AmountTopUpTextBox.Text.Trim();

            bool isValid = true;

            if (!Regex.IsMatch(cardNumber, @"^\d{13,19}$"))
            {
                CardErrorText.Text = "Номер картки має містити від 13 до 19 цифр.";
                isValid = false;
            }
            else if (!(cardNumber.StartsWith("4") || cardNumber.StartsWith("5")))
            {
                CardErrorText.Text = "Номер картки повинен починатися з 4 або 5.";
                isValid = false;
            }

            if (!Regex.IsMatch(cvv, @"^\d{3}$"))
            {
                CVVErrorText.Text = "CVV має містити рівно 3 цифри.";
                isValid = false;
            }

            if (!Regex.IsMatch(expiry, @"^\d{2}/\d{2}$") || !IsFutureDate(expiry))
            {
                ExpiryErrorText.Text = "Некоректна або прострочена дата. Введіть у форматі MM/YY.";
                isValid = false;
            }

            if (!decimal.TryParse(amountText, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amount) ||
                amount < 300 || amount > 100000)
            {
                AmountTopUpErrorText.Text = "Сума має бути числом від 300 до 100000.";
                isValid = false;
            }

            if (!isValid)
                return;

            _user.Balance += amount;
            MessageBox.Show($"Баланс поповнено на {amount:C}", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
            _user.SavedCardNumber = cardNumber;
            this.Close();
        }
        private void ExpiryDateTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, @"^\d+$");
        }

        private void ExpiryDateTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox.Text.Length == 2 && !textBox.Text.Contains("/"))
            {
                textBox.Text += "/";
                textBox.CaretIndex = textBox.Text.Length;
            }
        }
        private void FundsTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FundsTabControl.SelectedIndex == 1) 
            {
                if (!string.IsNullOrEmpty(_user.SavedCardNumber) && _user.SavedCardNumber.ToLower() != "null")
                {
                    WithdrawCardTextBox.Text = _user.SavedCardNumber;
                }
                else
                {
                    WithdrawCardTextBox.Text = string.Empty;
                }
            }
        }

        private void Withdraw_Click(object sender, RoutedEventArgs e)
        {
            ClearWithdrawErrors();

            string cardNumber = WithdrawCardTextBox.Text.Trim();
            string amountText = AmountWithdrawTextBox.Text.Trim();

            bool isValid = true;

            if (!Regex.IsMatch(cardNumber, @"^\d{13,19}$") || !(cardNumber.StartsWith("4") || cardNumber.StartsWith("5")))
            {
                WithdrawCardErrorText.Text = "Номер картки має містити від 13 до 19 цифр і починатися з 4 або 5.";
                isValid = false;
            }

            if (!decimal.TryParse(amountText, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amount) ||
                amount < 500 || amount > 100000)
            {
                AmountWithdrawErrorText.Text = "Сума має бути від 500 до 100000 грн.";
                isValid = false;
            }
            else if (amount > _user.Balance)
            {
                AmountWithdrawErrorText.Text = "Недостатньо коштів на балансі.";
                isValid = false;
            }


            if (!isValid)
                return;

            _user.SavedCardNumber = cardNumber;
            _user.Balance -= amount;

            MessageBox.Show($"Успішно виведено {amount:C}", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void ClearTopUpErrors()
        {
            CardErrorText.Text = "";
            CVVErrorText.Text = "";
            ExpiryErrorText.Text = "";
            AmountTopUpErrorText.Text = "";
        }

        private void ClearWithdrawErrors()
        {
            WithdrawCardErrorText.Text = "";
            AmountWithdrawErrorText.Text = "";
        }

        private bool IsFutureDate(string expiry)
        {
            try
            {
                var parts = expiry.Split('/');
                int month = int.Parse(parts[0]);
                int year = int.Parse(parts[1]) + 2000;

                var expiryDate = new DateTime(year, month, 1).AddMonths(1).AddDays(-1);
                return expiryDate >= DateTime.Today;
            }
            catch
            {
                return false;
            }
        }
    }
}
