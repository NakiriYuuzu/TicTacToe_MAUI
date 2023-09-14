using CommunityToolkit.Mvvm.ComponentModel;

namespace FirstMaui.model;

public partial class GameBoard: ObservableObject
{
    // Kotlin init
    public GameBoard(int index)
    {
        Index = index;
    }
    
    public int Index { get; set; }

    [ObservableProperty] 
    private string _selectedType;
    
    public int? Player { get; set; }
}