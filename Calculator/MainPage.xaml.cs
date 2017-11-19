using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Calculator
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        /// <summary>
        /// EditorBar class is for handling the business logic.
        /// The button inputs are handled by this class.
        /// </summary>
        EditorBar eBar;

        public MainPage()
        {
            this.InitializeComponent();
            eBar = new EditorBar();
            this.DataContext = eBar;
        }

        /// <summary>
        /// This method is called when a button is clicked on the UI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Stop the animation if it still runs on a previous button.
            // This is required for SetValue.
            ButtonAnimationStoryBoard.Stop();
            
            // Convert the sender object to a button.
            Button button = (Button)sender;
            
            // Set the TargetNameProperty of the animation to the name of the pressed button.
            ButtonAnimation.SetValue(Storyboard.TargetNameProperty, button.Name);
           
            // Start the animation on the button.
            ButtonAnimationStoryBoard.Begin();
            
            // Handle the Conent of the button in the EditorBar class.
            eBar.HandleNextInput(button.Content.ToString());
        }

        /// <summary>
        /// Async method to write the cached calculations into a text file.
        /// This is triggered when the 'S' button is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            // Start the animation for this button.
            SaveButtonAnimation.Begin();

            // Create folder instance
            Windows.Storage.StorageFolder folder =  Windows.Storage.ApplicationData.Current.LocalFolder;
            
            // Create a new file to write, or open if it exists.
            Windows.Storage.StorageFile sampleFile =
                await folder.CreateFileAsync("calculations.txt", Windows.Storage.CreationCollisionOption.OpenIfExists);
            
            // Append the cached computation at the end of the file
            await Windows.Storage.FileIO.AppendLinesAsync(sampleFile, eBar.ContentToWrite);
           
            // Tell the user, that saving is done.
            Windows.UI.Popups.MessageDialog md = new Windows.UI.Popups.MessageDialog("Results saved to file");
            await md.ShowAsync();
        }        
    }
}
