using MineSweeper.Service.DataContracts;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MineSweeper.DesktopClient
{
    public class FieldButton : Button
    {
        public static int ButtonDimensions = 30;
        public FieldData Field { get; set; }
        public bool IsFlag { get; set; }
        public TextBlock FieldButtonText { get; set; }
        public StackPanel ActualContent { get; set; }


        public FieldButton()
        {
            VerticalAlignment = VerticalAlignment.Center;
            HorizontalAlignment = HorizontalAlignment.Center;
            FieldButtonText = new TextBlock
            {
                Foreground = Brushes.Black,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            ActualContent = new StackPanel();
            Content = ActualContent;
            SetSize();
        }

        public void UpdateButton()
        {
            SetButtonContent();
            if (Field.PressedByPlayer != null)
            {
                if (GameWindow.PlayerColors.TryGetValue(Field.PressedByPlayer.Id, out SolidColorBrush color))
                {
                    FieldButtonText.Background = color;
                }
            }
        }

        private void SetSize()
        {
            Height = ButtonDimensions;
            Width = ButtonDimensions;
            FieldButtonText.Height = ButtonDimensions;
            FieldButtonText.Width = ButtonDimensions;

        }

        private void SetButtonContent()
        {
            if (Field.IsPressed)
            {
                if (Field.IsMine)
                {
                    ShowMine();
                }
                else if (Field.AdjacentMines > 0)
                {
                    FieldButtonText.Text = Field.AdjacentMines.ToString();
                }
                IsEnabled = false;
            }
            else if (IsFlag)
            {
                SetImage(new Uri("Images/Flag.png", UriKind.Relative));
            }
            else if (!IsFlag)
            {
                ResetContent();
            }
        }

        public void ShowMine()
        {
            if (Field.IsMine)
            {
                SetImage(new Uri("Images/Mine_05.png", UriKind.Relative));
                if (Field.IsPressed)
                {
                    IsEnabled = true;
                    Background = Brushes.Red;
                }
            }
        }

        private void SetImage(Uri imageUri)
        {
            Image image = new Image();
            image.Source = new System.Windows.Media.Imaging.BitmapImage(imageUri);
            ActualContent.Children.Clear();
            ActualContent.Children.Add(image);
        }

        private void ResetContent()
        {
            FieldButtonText.Text = "";
            ActualContent.Children.Clear();
            ActualContent.Children.Add(FieldButtonText);
        }

        
    }
}
