using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using FirstMaui.model;
using FirstMaui.ViewModel;

namespace FirstMaui;

public partial class TicTacToe : ContentPage
{
    public TicTacToe(Mode mode)
    {
        InitializeComponent();
        LabelTitle.Text = "Current Mode: " + mode;
        BindingContext = new TicTacToeViewModel(mode);
    }
}