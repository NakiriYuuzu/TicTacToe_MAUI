using FirstMaui.model;

namespace FirstMaui;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}
	
	private async void OnModeClicked(object sender, EventArgs e)
	{
		// check which button was clicked
		Button button = (Button)sender;
		string mode = button.Text;

		if (mode == PlayerBtn.Text)
		{
			await Navigation.PushAsync(new TicTacToe(Mode.PLAYER));
		} 
		else if (mode == BotBtn.Text)
		{
			await Navigation.PushAsync(new TicTacToe(Mode.BOT));
		}
	}
}

